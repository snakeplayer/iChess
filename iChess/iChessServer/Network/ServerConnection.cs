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

        // Register
        private string PACKET_TYPE_REGISTER_REQUEST = "Client_RegisterRequest";
        private string PACKET_TYPE_REGISTER_REPLY = "Server_RegisterReply";

        // Login
        private string PACKET_TYPE_LOGIN_REQUEST = "Client_LoginRequest";
        private string PACKET_TYPE_LOGIN_REPLY = "Server_LoginReply";

        #endregion

        #region Fields

        private string _logs;
        private Dictionary<Connection, string> _authenticatedClients;

        #endregion

        #region Properties

        private List<IObserverWindow> Observers { get; set; }

        private Dictionary<string, string> DBClients {
            get {
                return ServerDatabase.GetClientsFromDB();
            }
        }
        private Dictionary<Connection, string> AuthenticatedClients {
            get {
                return _authenticatedClients;
            }
            set {
                _authenticatedClients = value;
                this.NotifyObservers();
            }
        }

        private string Logs {
            get {
                return _logs;
            }
            set {
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
                // Handle connections closing
                NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);

                // Handle "RegisterRequest" packet type for register request
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_REGISTER_REQUEST, RegisterRequested);

                // Handle "Client_LoginRequest" packet type for login request
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_LOGIN_REQUEST, LoginRequested);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Methods (Tell, Don't Ask)

        /// <summary>
        /// Retrieve the list of authenticated clients.
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
        /// Retrieve the logs.
        /// </summary>
        /// <returns>The logs of the server.</returns>
        public string GetLogs()
        {
            return this.Logs;
        }

        #endregion

        #region Methods (Database)

        /// <summary>
        /// Allows a client to register.
        /// </summary>
        /// <param name="username">The username of the client.</param>
        /// <param name="password">The password of the client.</param>
        /// <returns>True if the client registered successfully, false if not.</returns>
        public bool RegisterClient(string username, string password)
        {
            return ServerDatabase.RegisterClient(username, password);
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
                Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, 8080));

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
        private void RegisterRequested(PacketHeader packetHeader, Connection connection, string credentials)
        {
            // Parse the necessary information out of the provided string
            string username = credentials.Split(':').First();
            string password = credentials.Split(':').Last();

            if (this.RegisterClient(username, password))
            {
                // Send a positive reply to the client
                connection.SendObject<bool>(PACKET_TYPE_REGISTER_REPLY, true);
            }
            else
            {
                // Send a negative reply to the client
                connection.SendObject<bool>(PACKET_TYPE_REGISTER_REPLY, false);
            }

            this.NotifyObservers();
        }

        /// <summary>
        /// Called when a connection request is made.
        /// </summary>
        /// <param name="packetHeader">The packet header.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        private void LoginRequested(PacketHeader packetHeader, Connection connection, string credentials)
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
                if (!this.AuthenticatedClients.ContainsValue(username) && (this.DBClients.ContainsKey(username) && this.DBClients[username] == password))
                {
                    // Add the client to the list
                    this.AuthenticatedClients.Add(connection, username);

                    // Send a reply to the client
                    connection.SendObject<bool>(PACKET_TYPE_LOGIN_REPLY, true);

                    // Add a message to logs
                    this.Logs += string.Format("{0} is now connected !\n", this.AuthenticatedClients[connection]);
                }
                else
                {
                    // Send a reply to the client
                    connection.SendObject<bool>(PACKET_TYPE_LOGIN_REPLY, false);
                }
            }

            this.NotifyObservers();
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
    }
}
