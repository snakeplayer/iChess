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
using NetworkCommsDotNet.DPSBase;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        /// Contains client's usernames and passwords from SQLite database.
        /// </summary>
        private Dictionary<string, string> DBClients
        {
            get
            {
                return ServerDatabase.GetClientsFromDB();
            }
        }

        /// <summary>
        /// Contains Connections and usernames of authenticated clients.
        /// </summary>
        private Dictionary<Connection, string> AuthenticatedClients
        {
            get
            {
                return _authenticatedClients;
            }
            set
            {
                _authenticatedClients = value;
                this.NotifyObservers();
            }
        }

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
            this.AuthenticatedClients = new Dictionary<Connection, string>();
            this.Logs = string.Empty;

            try
            {
                // Handles connections closing
                NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);

                // Handles registration requests
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_REGISTRATION_REQUEST, HandleRegistrationRequested);

                // Handles login requests
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_LOGIN_REQUEST, HandleLoginRequested);

                // Handles client's details requests
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_MYDETAILS_REQUEST, HandleMyDetailsRequested);

                // Handles all clients details requests
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_ALLCLIENTSDETAILS_REQUEST, HandleAllClientsDetailsRequested);

                // Handles profile modifications requests
                NetworkComms.AppendGlobalIncomingPacketHandler<ClientCredentials>(PACKET_TYPE_MODIFYPROFILE_REQUEST, HandleModifyProfileRequested);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Methods (Get)

        /// <summary>
        /// Returns the list of authenticated clients.
        /// </summary>
        /// <returns>A list of string conatining authenticated clients.</returns>
        public List<string> GetAuthenticatedClients()
        {
            List<string> clients = new List<string>();
            lock (this.AuthenticatedClients)
            {
                this.AuthenticatedClients.ToList().ForEach(authClient => clients.Add(authClient.Value));
            }
            return clients;
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

        #region Methods (Utilities)

        /// <summary>
        /// Tests if the incoming connection is from an authenticated client.
        /// </summary>
        /// <param name="connection">The incoming connection.</param>
        /// <returns>true == the connection is from an authenticated client, false == the connection is NOT from an authenticated client.</returns>
        private bool IsClientAuthenticated(Connection connection)
        {
            return this.AuthenticatedClients.ContainsKey(connection);
        }

        #endregion

        #region Methods (Start and Stop)

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <returns>True if the server started successfully, false if not.</returns>
        public bool StartServer()
        {
            bool hasServerStarted = false;

            try
            {
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

            this.NotifyObservers();
        }

        #endregion

        #region Methods (Handler)

        /// <summary>
        /// Called when a registration request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        private void HandleRegistrationRequested(PacketHeader packetHeader, Connection connection, string credentials)
        {
            // Parse the necessary information out of the provided string
            string username = credentials.Split(':').First();
            string password = credentials.Split(':').Last();

            // Try to register the client
            bool registrationAccomplished = ServerDatabase.RegisterClient(username, password);

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
        private void HandleLoginRequested(PacketHeader packetHeader, Connection connection, string credentials)
        {
            lock (this.AuthenticatedClients)
            {
                // Parse the necessary information out of the provided string
                string username = credentials.Split(':').First();
                string password = credentials.Split(':').Last();

                // If the client is already connected, remove it from the AuthenticatedClients dictionnary
                if (this.AuthenticatedClients.ContainsKey(connection))
                {
                    this.AuthenticatedClients.Remove(connection);
                }

                // If the client's account is not already used and if credentials are correct
                bool loginAllowed = !this.AuthenticatedClients.ContainsValue(username) &&
                                        (this.DBClients.ContainsKey(username) &&
                                        this.DBClients[username] == password);

                if (loginAllowed)
                {
                    // Add the client to the list
                    this.AuthenticatedClients.Add(connection, username);

                    // Add a message to logs
                    this.Logs += string.Format("{0} is now connected !\n", this.AuthenticatedClients[connection]);
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
                ClientDetails clientDetails = ServerDatabase.GetClientDetails(this.AuthenticatedClients[connection]);

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
                AllClientsDetails allClientsDetails = ServerDatabase.GetAllClientsDetails();

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
                if (ServerDatabase.ModifyClientProfile(this.AuthenticatedClients[connection], newClientCredentials))
                {
                    connection.SendObject<bool>(PACKET_TYPE_MODIFYPROFILE_REPLY, true);
                    connection.CloseConnection(false); // The client have to reconnect
                }
                else
                {
                    connection.SendObject<bool>(PACKET_TYPE_MODIFYPROFILE_REPLY, false);
                }

                this.NotifyObservers();
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
                if (this.AuthenticatedClients.ContainsKey(connection))
                {
                    lock (this.Logs)
                    {
                        this.Logs += string.Format("{0} disconnected from the server.\n", this.AuthenticatedClients[connection]);
                    }

                    // Remove the client from the list
                    this.AuthenticatedClients.Remove(connection);
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
