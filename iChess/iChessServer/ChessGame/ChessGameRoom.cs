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
        /// The chess game itself.
        /// </summary>
        public ChessGameModel Model { get; set; }

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

        /// <summary>
        /// Represents the hosts's name.
        /// </summary>
        public string HostClientName
        {
            get
            {
                return this.HostClient == null ? "" : this.HostClient.Username;
            }
        }

        /// <summary>
        /// Represents the guest's name.
        /// </summary>
        public string GuestClientName
        {
            get
            {
                return this.GuestClient == null ? "" : this.GuestClient.Username;
            }
        }

        /// <summary>
        /// True == black (host) can play, false == white (guest) can play.
        /// </summary>
        public bool PlayerTurn {
            get
            {
                return this.Model.PlayerTurn;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ChessGameRoom()
        {

        }

        /// <summary>
        /// Constructor of ChessGameRoom class.
        /// </summary>
        public ChessGameRoom(int id, string roomName, int minutesPerPlayer)
        {
            this.Model = new ChessGameModel("", "", minutesPerPlayer * 60);

            this.ID = id;
            this.RoomName = roomName;
            this.MinutesPerPlayer = minutesPerPlayer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Used to know if a client is in the room.
        /// </summary>
        /// <param name="authenticatedClient">The client.</param>
        /// <returns>True == the client is in the room, false == the client is NOT in the room.</returns>
        public bool IsClientInRoom(AuthenticatedClient authenticatedClient)
        {
            bool isInRoom = false;
            if (this.HostClient == authenticatedClient || this.GuestClient == authenticatedClient)
            {
                isInRoom = true;
            }
            return isInRoom;
        }

        /// <summary>
        /// Gets the RoomItem of this object.
        /// </summary>
        /// <returns>The RoomItem of this object.</returns>
        public RoomItem GetRoomItem()
        {
            RoomItem roomItem = new RoomItem();
            roomItem.RoomID = this.ID.ToString();
            roomItem.HostPlayerName = this.RoomName;
            roomItem.MinutesPerPlayer = this.MinutesPerPlayer.ToString();
            return roomItem;
        }

        /// <summary>
        /// Gets a RoomInfo object reference containing room's informations.
        /// </summary>
        /// <returns>A RoomInfo object reference containing room's informations.</returns>
        public RoomInfo GetRoomInfo()
        {
            RoomInfo roomInfo = new RoomInfo();
            roomInfo.HostPlayerName = this.HostClientName;
            roomInfo.GuestPlayerName = this.GuestClientName;

            roomInfo.HostPlayerSecondsLeft = this.MinutesPerPlayer; // TODO : real values
            roomInfo.GuestPlayerSecondsLeft = this.MinutesPerPlayer;

            roomInfo.HostPlayerPiecesOut = new List<string>() { "Piece1", "Piece2" };
            roomInfo.GuestPlayerPiecesOut = new List<string>() { "Piece3", "Piece4" };

            roomInfo.PlayerTurn = this.PlayerTurn; // TODO : real values
            roomInfo.ChatMessages = "This is the chat boyz !\n";

            // --------------------- ChessBoard & ChessSquare ---------------------------------------------------
            /*ChessSquareSerializable[] chessSquares = new ChessSquareSerializable[64]; // TODO : no magic values

            for (int i = 0; i < 64; i++)
            {
                chessSquares[i] = new ChessSquareSerializable();
            }

            chessSquares[0].ChessPiece = "Dawn_White";
            chessSquares[2].ChessPiece = "Dawn_White";*/

            roomInfo.ChessBoard = this.Model.GetChessBoardSerializable();

            return roomInfo;
        }

        /// <summary>
        /// Adds a client to the room.
        /// </summary>
        /// <param name="authenticatedClient">The client.</param>
        /// <returns>True == the client has been added, false == the client has not been added.</returns>
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

            this.NotifyClients();
            return hasJoined;
        }

        /// <summary>
        /// Removes a client from the room.
        /// </summary>
        /// <param name="authenticatedClient">The client.</param>
        /// <returns>True == the client has been removed, false == the client has not been removed.</returns>
        public bool LeaveRoom(AuthenticatedClient authenticatedClient)
        {
            bool hasBeenRemoved = false;

            if (this.RoomName == authenticatedClient.Username)
            {
                if (this.HostClient == authenticatedClient)
                {
                    this.HostClient = null;
                    hasBeenRemoved = true;
                }
            }
            else
            {
                if (this.GuestClient == authenticatedClient)
                {
                    this.GuestClient = null;
                    hasBeenRemoved = true;
                }
            }

            this.NotifyClients();
            return hasBeenRemoved;
        }

        /// <summary>
        /// Executes a ChessCommand.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="command">The ChessCommand.</param>
        public void ExecuteCommand(AuthenticatedClient client, ChessCommand command)
        {
            bool executionAllowed = false;

            if (this.PlayerTurn)
            {
                if (this.HostClient == client)
                {
                    executionAllowed = true;
                }
            }
            else
            {
                if (this.GuestClient == client)
                {
                    executionAllowed = true;
                }
            }

            if (executionAllowed)
            {
                // Do stuff
                this.Model.ExecuteCommand(command);
            }

            this.NotifyClients();
        }

        /// <summary>
        /// Tells to clients that the game state has changed.
        /// </summary>
        public void NotifyClients()
        {
            this.HostClient?.NotifyClient();
            this.GuestClient?.NotifyClient();
        }

        #endregion
    }
}
