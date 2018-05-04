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

        private string PACKET_TYPE_LOGIN_REQUEST = "Client_LoginRequest";
        private string PACKET_TYPE_LOGIN_REPLY = "Server_LoginReply";

        #endregion

        #region Properties

        private Connection MyConnection { get; set; }
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

                    // Handle message from server
                    //this.MyConnection.AppendIncomingPacketHandler<string>("Message_Server", HandleMessageServerReceived);

                    // Handle message from clients (from the server)
                    //this.MyConnection.AppendIncomingPacketHandler<string>("Message_Client", HandleMessageClientReceived);
                }

                // Format credentials
                string credentials = string.Format("{0}:{1}", username, password);

                // Send the connection request and wait 5000ms for a reply
                if (this.MyConnection.SendReceiveObject<string, bool>(PACKET_TYPE_LOGIN_REQUEST, PACKET_TYPE_LOGIN_REPLY, 5000, credentials))
                {
                    connectionAllowed = 0; // The server has accepted the connection
                }
                else
                {
                    connectionAllowed = 1; // The server refused the connection.
                }
            }
            catch (Exception)
            {
                connectionAllowed = -1;
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
