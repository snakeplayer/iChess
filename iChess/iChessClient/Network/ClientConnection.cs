/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ClientConnection.cs
 * File description: Handle connection with the iChess server.
 */

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using System;

namespace iChessClient
{
    /// <summary>
    /// Handle connection with the iChess server.
    /// </summary>
    public class ClientConnection
    {
        #region Constants

        // General
        private const int DEFAULT_TIMEOUT = 5000;

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

        #region Properties

        /// <summary>
        /// The connection with the iChess server.
        /// </summary>
        private Connection MyConnection { get; set; }

        /// <summary>
        /// The details of the current connection, for example the Username, the EloRating, etc.
        /// </summary>
        public ClientDetails Details
        {
            get
            {
                if (this.MyConnection != null)
                {
                    return this.GetMyDetails();
                }
                else
                {
                    return new ClientDetails();
                }
            }
        }

        /// <summary>
        /// A flag used to know if Handlers are already defined.
        /// </summary>
        private bool FirstInitialization { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ClientConnection class.
        /// </summary>
        public ClientConnection()
        {
            this.FirstInitialization = true;
        }

        #endregion

        #region Methods (Registration, connect & disconnect)

        /// <summary>
        /// Allows to register on an iChess server.
        /// </summary>
        /// <param name="ipAddress">The ip address of the server.</param>
        /// <param name="port">The port of the server.</param>
        /// <param name="username">The username of the client.</param>
        /// <param name="password">The password of the client.</param>
        /// <returns>-1 == unable to contact the server, 0 == the server has accepted the registration, 1 == the server refused the registration.</returns>
        public static int RegisterToServer(string ipAddress, int port, string username, string password)
        {
            // Initializing the return value
            int registrationAllowed = -1;

            try
            {
                // Format credentials
                ClientCredentials credentials = new ClientCredentials(username, password);

                // Send the registration request and wait 5000ms for a reply
                if (NetworkComms.SendReceiveObject<ClientCredentials, bool>(PACKET_TYPE_REGISTRATION_REQUEST, ipAddress, port, PACKET_TYPE_REGISTRATION_REPLY, DEFAULT_TIMEOUT, credentials))
                {
                    registrationAllowed = 0; // The server has accepted the registration
                }
                else
                {
                    registrationAllowed = 1; // The server has refused the registration
                }
            }
            catch (Exception)
            {
                registrationAllowed = -1; // An error occured
            }

            return registrationAllowed;
        }

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
            int connectionResult = -1;

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
                ClientCredentials credentials = new ClientCredentials(username, password);

                // Send the connection request and wait 5000ms for a reply
                if (this.MyConnection.SendReceiveObject<ClientCredentials, bool>(PACKET_TYPE_LOGIN_REQUEST, PACKET_TYPE_LOGIN_REPLY, DEFAULT_TIMEOUT, credentials))
                {
                    connectionResult = 0; // The server has accepted the connection
                }
                else
                {
                    connectionResult = 1; // The server refused the connection
                }
            }
            catch (Exception)
            {
                connectionResult = -1; // An error occured
            }

            return connectionResult;
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

        #region Methods (Get/send various data from/to the server)

        /// <summary>
        /// Ask the server the current connection's details.
        /// </summary>
        /// <returns>An instance of ClientDetails containing the current connection's client's details.</returns>
        public ClientDetails GetMyDetails()
        {
            try
            {
                return this.MyConnection.SendReceiveObject<ClientDetails>(PACKET_TYPE_MYDETAILS_REQUEST, PACKET_TYPE_MYDETAILS_REPLY, DEFAULT_TIMEOUT);
            }
            catch (Exception)
            {
                throw new Exception("An error occured while trying to recover client's details.");
            }
        }

        /// <summary>
        /// Ask the server the details of all registered clients.
        /// </summary>
        /// <returns>An instance of AllClientsDetails containing all client's details.</returns>
        public AllClientsDetails GetAllClientsDetails()
        {
            try
            {
                return this.MyConnection.SendReceiveObject<AllClientsDetails>(PACKET_TYPE_ALLCLIENTSDETAILS_REQUEST, PACKET_TYPE_ALLCLIENTSDETAILS_REPLY, DEFAULT_TIMEOUT);
            }
            catch (Exception)
            {
                throw new Exception("An error occured while trying to recover all client's details.");
            }
        }

        /// <summary>
        /// Ask the server to modify client's informations.
        /// </summary>
        /// <param name="username">The new username.</param>
        /// <param name="password">The new password.</param>
        /// <returns>-1 == an unknown error occured, 0 == the server has accepted the modification, 1 == the server refused the modification.</returns>
        public int ModifyClientProfile(string username, string password)
        {
            int returnValue = -1;

            try
            {
                if (this.MyConnection.SendReceiveObject<ClientCredentials, bool>(PACKET_TYPE_MODIFYPROFILE_REQUEST, PACKET_TYPE_MODIFYPROFILE_REPLY, DEFAULT_TIMEOUT, new ClientCredentials(username, password))) // TODO : maybe pass ClientCredentials as this functions parameter
                {
                    returnValue = 0;
                }
                else
                {
                    returnValue = 1;
                }
            }
            catch (Exception)
            {
                returnValue = -1;
            }

            return returnValue;
        }

        /// <summary>
        /// Ask the iChess server to create a gaming room.
        /// </summary>
        /// <param name="minutesPerPlayer">The time each player have to play.</param>
        /// <returns>less than 0 == an error occured, 0 == the server refused, more than 0 == the ID of the created gaming room.</returns>
        public int CreateRoom(int minutesPerPlayer)
        {
            int returnValue = -1;
            
            try
            {
                returnValue = this.MyConnection.SendReceiveObject<int, int>(PACKET_TYPE_CREATEROOM_REQUEST, PACKET_TYPE_CREATEROOM_REPLY, DEFAULT_TIMEOUT, minutesPerPlayer);
            }
            catch (Exception)
            {
                returnValue = -1;
            }

            return returnValue;
        }

        /// <summary>
        /// Join a game room.
        /// </summary>
        /// <param name="roomID">The ID of the room.</param>
        /// <returns>True == the room is joined, false == an error occured.</returns>
        public bool JoinRoom(int roomID)
        {
            bool returnValue = false;

            try
            {
                returnValue = this.MyConnection.SendReceiveObject<int, bool>(PACKET_TYPE_JOINROOM_REQUEST, PACKET_TYPE_JOINROOM_REPLY, DEFAULT_TIMEOUT, roomID);
            }
            catch (Exception)
            {
                returnValue = false;
            }

            return returnValue;
        }

        #endregion

        #region Methods (Handler)

        /// <summary>
        /// Called when the connection is lost.
        /// </summary>
        /// <param name="connection">The connection's object reference.</param>
        private void HandleConnectionClosed(Connection connection)
        {
            this.DisconnectFromServer();
        }

        #endregion
    }
}
