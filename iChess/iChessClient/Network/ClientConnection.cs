/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ClientConnection.cs
 * File description: Handle connection with the iChess server.
 */

using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessClient
{
    /// <summary>
    /// Handle connection with the iChess server.
    /// </summary>
    public class ClientConnection
    {
        #region Constants

        private string DEFAULT_USERNAME = "NO_USERNAME";

        // Register
        private static string PACKET_TYPE_REGISTER_REQUEST = "Client_RegisterRequest";
        private static string PACKET_TYPE_REGISTER_REPLY = "Server_RegisterReply";

        // Login
        private string PACKET_TYPE_LOGIN_REQUEST = "Client_LoginRequest";
        private string PACKET_TYPE_LOGIN_REPLY = "Server_LoginReply";

        // ClientDetails recovering
        private static string PACKET_TYPE_CLIENTDETAILS_REQUEST = "Client_ClientDetailsRequest";
        private static string PACKET_TYPE_CLIENTDETAILS_REPLY = "Server_ClientDetailsReply";

        // EloRating recovering
        private static string PACKET_TYPE_ELORATING_REQUEST = "Client_EloRatingRequest";
        private static string PACKET_TYPE_ELORATING_REPLY = "Client_EloRatingReply";

        #endregion

        #region Properties

        private Connection MyConnection { get; set; }

        private string Username { get; set; }
        private string ServerIP { get; set; }
        private int ServerPort { get; set; }

        public ClientDetails Details
        {
            get
            {
                if (this.ServerIP != string.Empty && this.ServerPort != -1)
                {
                    return ClientConnection.GetClientDetails(this.GetServerIP(), this.GetServerPort(), this.GetUsername());
                }
                else
                {
                    return new ClientDetails();
                }
            }
        }

        private bool FirstInitialization { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ClientConnection class.
        /// </summary>
        public ClientConnection()
        {
            this.Username = DEFAULT_USERNAME;
            this.ServerIP = string.Empty;
            this.ServerPort = -1;
            this.FirstInitialization = true;
        }

        #endregion

        #region Methods (Tell, Don't Ask)

        public string GetUsername()
        {
            return this.Username;
        }

        public string GetServerIP()
        {
            return this.ServerIP;
        }

        public int GetServerPort()
        {
            return this.ServerPort;
        }

        #endregion

        #region Methods (Database)

        /// <summary>
        /// Allows to register on an iChess server.
        /// </summary>
        /// <param name="ipAddress">The ip address of the server.</param>
        /// <param name="port">The port of the server.</param>
        /// <param name="username">The username of the client.</param>
        /// <param name="password">The password of the client.</param>
        /// <returns></returns>
        public static int RegisterToServer(string ipAddress, int port, string username, string password)
        {
            // Initializing the return value
            int registerAllowed = -1;

            try
            {
                // Format credentials
                string credentials = string.Format("{0}:{1}", username, password);

                // Send the registration request and wait 5000ms for a reply
                if (NetworkComms.SendReceiveObject<string, bool>(PACKET_TYPE_REGISTER_REQUEST, ipAddress, port, PACKET_TYPE_REGISTER_REPLY, 5000, credentials))
                {
                    registerAllowed = 0; // The server has accepted the registration
                }
                else
                {
                    registerAllowed = 1; // The server has refused the registration
                }
            }
            catch (Exception)
            {
                registerAllowed = -1; // An error occured
            }

            return registerAllowed;
        }

        public static int GetEloRating(string ipAddress, int port, string username)
        {
            // Initializing the return value
            int returnValue = -1;

            try
            {
                // Send the EloRating request and wait 5000ms for a reply
                returnValue = NetworkComms.SendReceiveObject<string, int>(PACKET_TYPE_ELORATING_REQUEST, ipAddress, port, PACKET_TYPE_ELORATING_REPLY, 5000, username);
            }
            catch (Exception)
            {
                returnValue = -1; // An error occured
            }

            return returnValue;
        }

        public static ClientDetails GetClientDetails(string ipAddress, int port, string username)
        {
            try
            {
                ClientDetails clientDetails = NetworkComms.SendReceiveObject<string, ClientDetails>(PACKET_TYPE_CLIENTDETAILS_REQUEST, ipAddress, port, PACKET_TYPE_CLIENTDETAILS_REPLY, 5000, username);
                return clientDetails;
            }
            catch (Exception)
            {
                throw new Exception("An error occured while trying to recover client's details.");
            }
        }

        #endregion

        #region Methods (Login & logout)

        /// <summary>
        /// Establishes a connection with the server.
        /// </summary>
        /// <param name="ipAddress">The IP address of the server.</param>
        /// <param name="port">The port of the server.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>-1 == unable to contact the server, 0 == the server has accepted the connection, 1 == the server refused the connection.</returns>
        public int ConnectToServer(string ipAddress, int port, string username, string password)
        {
            // Initializing the return value
            int connectionAllowed = -1;

            // Reset connection
            this.DisconnectFromServer();

            // Try to connect to the server
            try
            {
                // Set connection infos
                ConnectionInfo connInfo = new ConnectionInfo(ipAddress, port);

                // Connect to the server
                this.MyConnection = TCPConnection.GetConnection(connInfo);

                // If it's the first connection
                if (this.FirstInitialization)
                {
                    // Handle connection shutdown
                    this.MyConnection.AppendShutdownHandler(HandleConnectionClosed);
                }

                // Format credentials
                string credentials = string.Format("{0}:{1}", username, password);

                // Send the connection request and wait 5000ms for a reply
                if (this.MyConnection.SendReceiveObject<string, bool>(PACKET_TYPE_LOGIN_REQUEST, PACKET_TYPE_LOGIN_REPLY, 5000, credentials))
                {
                    connectionAllowed = 0; // The server has accepted the connection
                    this.Username = username;
                    this.ServerIP = ipAddress;
                    this.ServerPort = port;
                }
                else
                {
                    connectionAllowed = 1; // The server refused the connection
                }
            }
            catch (Exception)
            {
                connectionAllowed = -1; // An error occured
            }

            return connectionAllowed;
        }

        /// <summary>
        /// Closes the connection with the server.
        /// </summary>
        public void DisconnectFromServer()
        {
            this.MyConnection?.CloseConnection(false);
            NetworkComms.Shutdown();
        }

        #endregion

        #region Methods (Handler)

        /// <summary>
        /// Triggered when the connection is lost.
        /// </summary>
        /// <param name="connection">The connection object reference.</param>
        private void HandleConnectionClosed(Connection connection)
        {
            this.DisconnectFromServer();
        }

        #endregion
    }
}
