/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: AuthenticatedClient.cs
 * File description: Represents an authenticated client.
 */

using NetworkCommsDotNet.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Represents an authenticated client.
    /// </summary>
    public class AuthenticatedClient
    {
        #region Constants

        // Game state changed
        private const string PACKET_TYPE_GAME_STATE_CHANGED = "GameStateChanged";

        #endregion

        #region Properties

        /// <summary>
        /// Represents the connection.
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// The username of the client.
        /// </summary>
        public string Username { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public AuthenticatedClient()
        {

        }

        /// <summary>
        /// Constructor of AuthenticatedClient class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="username">The username.</param>
        public AuthenticatedClient(Connection connection, string username)
        {
            this.Connection = connection;
            this.Username = username;
        }

        /// <summary>
        /// Notify the client that the game state has changed.
        /// </summary>
        public void NotifyClient()
        {
            this.Connection.SendObject<int>(PACKET_TYPE_GAME_STATE_CHANGED, 1);
        }

        #endregion
    }
}
