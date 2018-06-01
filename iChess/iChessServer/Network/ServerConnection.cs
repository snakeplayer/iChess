/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ServerConnection.cs
 * File description: Handle connections with iChess players.
 */

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace iChessServer
{
    /// <summary>
    /// Handle connections with iChess players.
    /// </summary>
    public class ServerConnection
    {
        #region Constants

        // General
        private const int DEFAULT_PORT = 8080;

        // Registration request
        private const string PACKET_TYPE_REGISTRATION_REQUEST = "RegistrationRequest";
        private const string PACKET_TYPE_REGISTRATION_REPLY = "RegistrationReply";

        // Login request
        private const string PACKET_TYPE_LOGIN_REQUEST = "LoginRequest";
        private const string PACKET_TYPE_LOGIN_REPLY = "LoginReply";

        // MyDetails recovering
        private const string PACKET_TYPE_MYDETAILS_REQUEST = "MyDetailsRequest";
        private const string PACKET_TYPE_MYDETAILS_REPLY = "MyDetailsReply";

        // AllClientsDetails recovering
        private const string PACKET_TYPE_ALLCLIENTSDETAILS_REQUEST = "AllClientsDetailsRequest";
        private const string PACKET_TYPE_ALLCLIENTSDETAILS_REPLY = "AllClientsDetailsReply";

        // ModifyProfile request
        private const string PACKET_TYPE_MODIFYPROFILE_REQUEST = "ModifyProfileRequest";
        private const string PACKET_TYPE_MODIFYPROFILE_REPLY = "ModifyProfileReply";

        // CreateRoom request
        private const string PACKET_TYPE_CREATEROOM_REQUEST = "CreateRoomRequest";
        private const string PACKET_TYPE_CREATEROOM_REPLY = "CreateRoomReply";

        // JoinRoom request
        private const string PACKET_TYPE_JOINROOM_REQUEST = "JoinRoomRequest";
        private const string PACKET_TYPE_JOINROOM_REPLY = "JoinRoomReply";

        #endregion

        #region Fields

        private string _logs;
        private Dictionary<Connection, string> _authenticatedClients;

        #endregion

        #region Properties

        /// <summary>
        /// A list of IObserverWindow, used to call UpdateView() method on attached views.
        /// </summary>
        private List<IObserverWindow> Observers { get; set; }

        /// <summary>
        /// Used to access the SQLite database.
        /// </summary>
        private ServerDatabase Database { get; set; }

        /// <summary>
        /// Contains client's usernames and passwords from SQLite database.
        /// </summary>
        private List<ClientCredentials> DBClients
        {
            get
            {
                return this.Database?.GetClientsCredentials();
            }
        }

        /// <summary>
        /// Contains all authenticated clients.
        /// </summary>
        private AuthenticatedClients AuthenticatedClients { get; set; }

        /// <summary>
        /// Contains all chess game rooms.
        /// </summary>
        private ChessGameRoomList ChessGameRoomList { get; set; }

        /// <summary>
        /// Contains the logs and call NotifyObservers() when the value changes.
        /// </summary>
        private string Logs
        {
            get
            {
                return _logs;
            }
            set
            {
                _logs = value;
                this.NotifyObservers();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ServerConnection class.
        /// </summary>
        public ServerConnection()
        {
            // Initialization
            this.Observers = new List<IObserverWindow>();
            this.Database = new ServerDatabase();
            this.AuthenticatedClients = new AuthenticatedClients();
            this.ChessGameRoomList = new ChessGameRoomList();
            this.Logs = string.Empty;

            try
            {
                // Handles connections closing
                NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);

                // Handles registration requests
                NetworkComms.AppendGlobalIncomingPacketHandler<ClientCredentials>(PACKET_TYPE_REGISTRATION_REQUEST, HandleRegistrationRequested);

                // Handles login requests
                NetworkComms.AppendGlobalIncomingPacketHandler<ClientCredentials>(PACKET_TYPE_LOGIN_REQUEST, HandleLoginRequested);

                // Handles client's details requests
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_MYDETAILS_REQUEST, HandleMyDetailsRequested);

                // Handles all clients details requests
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_ALLCLIENTSDETAILS_REQUEST, HandleAllClientsDetailsRequested);

                // Handles profile modifications requests
                NetworkComms.AppendGlobalIncomingPacketHandler<ClientCredentials>(PACKET_TYPE_MODIFYPROFILE_REQUEST, HandleModifyProfileRequested);

                // Handles gaming room creation requests
                NetworkComms.AppendGlobalIncomingPacketHandler<int>(PACKET_TYPE_CREATEROOM_REQUEST, HandleCreateRoomRequested);

                // Handles gaming room creation requests
                NetworkComms.AppendGlobalIncomingPacketHandler<int>(PACKET_TYPE_JOINROOM_REQUEST, HandleJoinRoomRequested);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Methods (Get data from outside)

        /// <summary>
        /// Returns the list of authenticated clients.
        /// </summary>
        /// <returns>A list of string conatining authenticated clients.</returns>
        public List<string> GetAuthenticatedClients()
        {
            return this.AuthenticatedClients.GetUsernameList();
        }

        /// <summary>
        /// Returns the logs.
        /// </summary>
        /// <returns>The logs of the server.</returns>
        public string GetLogs()
        {
            return this.Logs;
        }

        #endregion

        #region Methods (Start & stop)

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <returns>True == the server started successfully, false == an error occured.</returns>
        public bool StartServer()
        {
            bool hasServerStarted = false;

            try
            {
                // Open the connection with the SQLite database
                this.Database.OpenConnection();

                // Start listening for incoming connections
                Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, DEFAULT_PORT)); // TODO : use const or CONFIG FILE !!

                // Shows Network state
                lock (this.Logs)
                {
                    this.Logs += "Listening for TCP messages on:\n";
                    foreach (IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                        this.Logs += string.Format("{0}:{1}\n", localEndPoint.Address, localEndPoint.Port);
                }
            }
            catch (Exception)
            {
                lock (this.Logs)
                {
                    this.Logs += "An error occurred while starting the server.\n";
                }
            }

            this.NotifyObservers();

            return hasServerStarted;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            NetworkComms.Shutdown();

            lock (this.Logs)
            {
                this.Logs += "Server disconnected.\n";
            }

            lock (this.AuthenticatedClients)
            {
                this.AuthenticatedClients.Clear();
            }

            // Closing the connection with the SQLite database
            this.Database.CloseConnection();

            this.NotifyObservers();
        }

        #endregion

        #region Methods (Utilities)

        /// <summary>
        /// Checks if credentials are correct.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>True == they are correct, false == they are NOT correct.</returns>
        private bool CheckCredentials(ClientCredentials credentials)
        {
            bool loginAllowed = false;

            this.DBClients.ToList().ForEach(clientDB =>
            {
                if (clientDB.Equals(credentials))
                    loginAllowed = true;
            });

            return loginAllowed;
        }

        /// <summary>
        /// Checks if the username is already connected.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>True == it is already connected, false == it is NOT already connected.</returns>
        private bool IsUsernameAlreadyConnected(string username)
        {
            return this.AuthenticatedClients.ContainsUsername(username);
        }

        /// <summary>
        /// Tests if the incoming connection is from an authenticated client.
        /// </summary>
        /// <param name="connection">The incoming connection.</param>
        /// <returns>true == the connection is from an authenticated client, false == the connection is NOT from an authenticated client.</returns>
        private bool IsClientAuthenticated(Connection connection)
        {
            return this.AuthenticatedClients.ContainsConnection(connection);
        }

        #endregion

        #region Methods (Handler)

        /// <summary>
        /// Called when a registration request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        private void HandleRegistrationRequested(PacketHeader packetHeader, Connection connection, ClientCredentials credentials)
        {
            // Try to register the client
            bool registrationAccomplished = this.Database.RegisterClient(credentials);

            // Send a reply to the client
            connection.SendObject<bool>(PACKET_TYPE_REGISTRATION_REPLY, registrationAccomplished);

            this.NotifyObservers();
        }

        /// <summary>
        /// Called when a connection request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        private void HandleLoginRequested(PacketHeader packetHeader, Connection connection, ClientCredentials credentials)
        {
            lock (this.AuthenticatedClients)
            {
                // If the client is already connected, remove it from the AuthenticatedClients dictionnary
                if (this.IsClientAuthenticated(connection))
                {
                    this.AuthenticatedClients.RemoveClient(connection);
                }

                // If the client's account is not already used and if credentials are correct
                bool loginAllowed = !this.IsUsernameAlreadyConnected(credentials.Username) && this.CheckCredentials(credentials);

                if (loginAllowed)
                {
                    // Add the client to the list
                    this.AuthenticatedClients.AddClient(connection, credentials.Username);

                    // Add a message to logs
                    //this.Logs += string.Format("{0} is now connected !\n", this.AuthenticatedClients[connection]);
                    this.Logs += string.Format("{0} is now connected !\n", this.AuthenticatedClients.GetUsername(connection));
                }

                // Send a reply to the client
                connection.SendObject<bool>(PACKET_TYPE_LOGIN_REPLY, loginAllowed);
            }

            this.NotifyObservers();
        }

        /// <summary>
        /// Called when a connection's client's details request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="incomingObject"></param>
        private void HandleMyDetailsRequested(PacketHeader packetHeader, Connection connection, string incomingObject)
        {
            if (this.IsClientAuthenticated(connection))
            {
                // ClientDetails recovering from database
                ClientDetails clientDetails = this.Database.GetClientDetails(this.AuthenticatedClients.GetUsername(connection));

                // Send clientDetails to the client
                connection.SendObject<ClientDetails>(PACKET_TYPE_MYDETAILS_REPLY, clientDetails);

                this.NotifyObservers();
            }
        }

        /// <summary>
        /// Called when an all client's details request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="incomingObject"></param>
        private void HandleAllClientsDetailsRequested(PacketHeader packetHeader, Connection connection, string incomingObject)
        {
            if (this.IsClientAuthenticated(connection))
            {
                // AllClientsDetails recovering from database
                AllClientsDetails allClientsDetails = this.Database.GetAllClientsDetails();

                // Send allClientsDetails to the client
                connection.SendObject<AllClientsDetails>(PACKET_TYPE_ALLCLIENTSDETAILS_REPLY, allClientsDetails);

                this.NotifyObservers();
            }
        }

        /// <summary>
        /// Called when a profile modification request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="newClientCredentials">The new client's credentials.</param>
        private void HandleModifyProfileRequested(PacketHeader packetHeader, Connection connection, ClientCredentials newClientCredentials)
        {
            if (this.IsClientAuthenticated(connection))
            {
                // Modify client's credentials
                bool clientCredentialsModified = this.Database.ModifyClientCredentials(this.AuthenticatedClients.GetUsername(connection), newClientCredentials);

                // Send a reply to the client
                connection.SendObject<bool>(PACKET_TYPE_MODIFYPROFILE_REPLY, clientCredentialsModified);

                // Eventually disconnect the client
                if (clientCredentialsModified)
                    connection.CloseConnection(false); // The client have to reconnect

                this.NotifyObservers();
            }
        }
        
        /// <summary>
        /// Called when a room creation request is made.
        /// </summary>
        /// <param name="packetHeader">The header of the packet.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="minutesPerPlayer">The playing time for each players.</param>
        private void HandleCreateRoomRequested(PacketHeader packetHeader, Connection connection, int minutesPerPlayer)
        {
            lock (this.ChessGameRoomList)
            {
                if (this.IsClientAuthenticated(connection))
                {
                    string username = this.AuthenticatedClients.GetUsername(connection);
                    int roomID = this.ChessGameRoomList.AddGameRoom(username, minutesPerPlayer);
                    connection.SendObject<int>(PACKET_TYPE_CREATEROOM_REPLY, roomID);
                    this.Logs += string.Format("New room created.\n Room ID : {0}\n Username : {1}\n", roomID, username);
                    this.NotifyObservers();
                }
            }
        }

        /// <summary>
        /// Called when a join room request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="roomID">The room's uniq ID.</param>
        private void HandleJoinRoomRequested(PacketHeader packetHeader, Connection connection, int roomID)
        {
            if (this.IsClientAuthenticated(connection))
            {
                string username = this.AuthenticatedClients.GetUsername(connection);
                bool hasJoined = this.ChessGameRoomList.AddClientToRoom(this.AuthenticatedClients.GetAuthenticatedClientFromConnection(connection), roomID);
                connection.SendObject<bool>(PACKET_TYPE_JOINROOM_REPLY, hasJoined);

                if (hasJoined)
                {
                    this.Logs += string.Format("Room joined.\n Room ID : {0}\n Username : {1}\n", roomID, username);
                    this.NotifyObservers();
                }
            }
        }

        /// <summary>
        /// Called when a connection is closed.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private void HandleConnectionClosed(Connection connection)
        {
            lock (this.AuthenticatedClients)
            {
                if (this.AuthenticatedClients.ContainsConnection(connection))
                {
                    lock (this.Logs)
                    {
                        this.Logs += string.Format("{0} disconnected from the server.\n", this.AuthenticatedClients.GetUsername(connection));
                    }

                    // Remove the client from the list
                    this.AuthenticatedClients.RemoveClient(connection);
                    // TODO : remove client from rooms !
                }
            }

            this.NotifyObservers();
        }

        #endregion

        #region Methods (Observers)

        /// <summary>
        /// Add an IObserverWindow to the list.
        /// </summary>
        /// <param name="observerWindow">The IObserverWindow to add.</param>
        public void RegisterObserver(IObserverWindow observerWindow)
        {
            this.Observers.Add(observerWindow);
            this.NotifyObservers();
        }

        /// <summary>
        /// Remove an IObserverWindow from the list.
        /// </summary>
        /// <param name="observerWindow">The IObserverWindow to remove.</param>
        public void UnregisterObserver(IObserverWindow observerWindow)
        {
            this.Observers.Add(observerWindow);
            this.NotifyObservers();
        }

        /// <summary>
        /// Notify all registered IObserverWindow.
        /// </summary>
        public void NotifyObservers()
        {
            lock (this.Observers)
            {
                this.Observers.ToList().ForEach(obs => obs.UpdateView());
            }
        }

        #endregion
    }
}
