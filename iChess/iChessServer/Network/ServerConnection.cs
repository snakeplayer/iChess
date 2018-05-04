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
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Handle connections with iChess players.
    /// </summary>
    public class ServerConnection
    {
        #region Constants

        private string PACKET_TYPE_LOGIN_REQUEST = "Client_LoginRequest";
        private string PACKET_TYPE_LOGIN_REPLY = "Server_LoginReply";

        #endregion

        #region Properties

        private Dictionary<string, string> DBClients { get; set; } // Fake DB
        private Dictionary<Connection, string> AuthenticatedClients { get; set; }
        public string Logs { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ServerConnection class.
        /// </summary>
        public ServerConnection()
        {
            // Fake DB
            this.DBClients = new Dictionary<string, string>();
            this.DBClients.Add("Username1", "password1");
            this.DBClients.Add("Username2", "password2");
            this.DBClients.Add("Username3", "password3");

            this.AuthenticatedClients = new Dictionary<Connection, string>();

            this.Logs = string.Empty;

            try
            {
                // Handle connections closing
                NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);

                // Handle "Login_Request" packet type for login request
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PACKET_TYPE_LOGIN_REQUEST, LoginRequested);

                // Handle "Message_Client" packet type for message recieved
                //NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message_Client", MessageRecieved);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Methods

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

            return hasServerStarted;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            NetworkComms.Shutdown();
            lock (this.AuthenticatedClients)
            {
                this.AuthenticatedClients.Clear();
            }

            lock (this.Logs)
            {
                this.Logs += "Server disconnected.\n";
            }
        }

        #endregion

        #region Methods (Handler)

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

                // If the client's account is already used
                if (this.AuthenticatedClients.ContainsValue(username))
                {
                    // Send a reply to the client
                    connection.SendObject<bool>("Login_Reply", false);
                }
                else
                {
                    // If the credentials are correct
                    if (this.DBClients.ContainsKey(username) && this.DBClients[username] == password)
                    {
                        // Add the client to the list
                        this.AuthenticatedClients.Add(connection, username);

                        // Send a reply to the client
                        connection.SendObject<bool>("Login_Reply", true);

                        // Show message
                        this.Logs += string.Format("{0} is now connected !\n", this.AuthenticatedClients[connection]);
                    }
                    else
                    {
                        // Send a reply
                        connection.SendObject<bool>("Login_Reply", false);
                    }
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
        }

        #endregion
    }
}
