/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessGameRoom.cs
 * File description: Represents a chess game room.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Represents a chess game room.
    /// </summary>
    public class ChessGameRoom
    {
        #region Properties

        /// <summary>
        /// Represents the uniq ID of the chess game room.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Represents the room's name. The room's name is, for the moment, the username of the HostClient.
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// The number of minutes each player have when the game starts.
        /// </summary>
        public int MinutesPerPlayer { get; set; }

        /// <summary>
        /// Represents the client who created the game room.
        /// </summary>
        public AuthenticatedClient HostClient { get; set; }

        /// <summary>
        /// Represents the client who joined a created game room.
        /// </summary>
        public AuthenticatedClient GuestClient { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ChessGameRoom class.
        /// </summary>
        public ChessGameRoom(int id, string roomName, int minutesPerPlayer)
        {
            this.ID = id;
            this.RoomName = roomName;
            this.MinutesPerPlayer = minutesPerPlayer;
        }

        #endregion

        #region Methods

        public bool JoinRoom(AuthenticatedClient authenticatedClient)
        {
            bool hasJoined = false;

            if (this.RoomName == authenticatedClient.Username)
            {
                if (this.HostClient == null)
                {
                    this.HostClient = authenticatedClient;
                    hasJoined = true;
                }
            }
            else
            {
                if (this.GuestClient == null)
                {
                    this.GuestClient = authenticatedClient;
                    hasJoined = true;
                }
            }

            return hasJoined;
        }

        #endregion
    }
}
