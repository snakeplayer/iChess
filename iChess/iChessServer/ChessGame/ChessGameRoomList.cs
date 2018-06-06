/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessGameRoomList.cs
 * File description: Contains all chess game rooms.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Contains all chess game rooms.
    /// </summary>
    public class ChessGameRoomList
    {
        #region Properties

        /// <summary>
        /// A list of ChessGameRoom containing all chess game rooms.
        /// </summary>
        public List<ChessGameRoom> GameRoomList { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ChessGameRoomList class.
        /// </summary>
        public ChessGameRoomList()
        {
            this.GameRoomList = new List<ChessGameRoom>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a uniq ID.
        /// </summary>
        /// <returns>A uniq ID.</returns>
        private int GetNewID()
        {
            Random random = new Random();
            int id = random.Next(0, 100000);
            bool alreadyExists = false;

            foreach (ChessGameRoom cgr in this.GameRoomList)
            {
                if (cgr.ID == id)
                {
                    alreadyExists = true;
                }
            }

            if (alreadyExists)
            {
                return GetNewID();
            }
            else
            {
                return id;
            }
        }

        /// <summary>
        /// Gets a ChessGameRoom object reference for a given ID.
        /// </summary>
        /// <param name="roomID">The room's ID.</param>
        /// <returns>A ChessGameRoom object.</returns>
        public ChessGameRoom GetRoomFromID(int roomID)
        {
            ChessGameRoom gameRoom = new ChessGameRoom();

            foreach (ChessGameRoom cgr in this.GameRoomList)
            {
                if (cgr.ID == roomID)
                {
                    gameRoom = cgr;
                }
            }

            return gameRoom;
        }

        /// <summary>
        /// Gets a RoomItemList object containing the list of all server's RoomItem.
        /// </summary>
        /// <returns>A RoomItemList object containing the list of all server's RoomItem.</returns>
        public RoomItemList GetRoomItemList()
        {
            RoomItemList itemList = new RoomItemList();
            foreach (ChessGameRoom gameRoom in this.GameRoomList)
            {
                itemList.List.Add(gameRoom.GetRoomItem());
            }
            return itemList;
        }

        /// <summary>
        /// Gets a RoomInfo object reference containing room informations.
        /// </summary>
        /// <param name="roomID">The uniq ID of the room.</param>
        /// <returns>A RoomInfo object reference containing room informations.</returns>
        public RoomInfo GetRoomInfo(int roomID)
        {
            RoomInfo roomInfo = new RoomInfo();
            ChessGameRoom gameRoom = this.GetRoomFromID(roomID);
            lock (this.GameRoomList)
            {
                roomInfo = gameRoom.GetRoomInfo();
            }

            return roomInfo;
        }

        /// <summary>
        /// Adds a game room the the list.
        /// </summary>
        /// <param name="username">The username of the room creator.</param>
        /// <param name="minutesPerPlayer">The time per player when the game starts.</param>
        /// <returns>The ID of the room created.</returns>
        public int AddGameRoom(string username, int minutesPerPlayer)
        {
            ChessGameRoom gameRoom;

            lock (this.GameRoomList)
            {
                gameRoom = new ChessGameRoom(GetNewID(), username, minutesPerPlayer);
                this.GameRoomList.Add(gameRoom);
            }

            return gameRoom.ID;
        }

        /// <summary>
        /// Adds a client to a chess game room.
        /// </summary>
        /// <param name="authenticatedClient">The authenticated client.</param>
        /// <param name="roomID">The room's ID.</param>
        /// <returns>True == the client has joined, false == an error occured.</returns>
        public bool AddClientToRoom(AuthenticatedClient authenticatedClient, int roomID)
        {
            bool hasJoined = false;

            lock (this.GameRoomList)
            {
                
                foreach (ChessGameRoom cgr in this.GameRoomList)
                {
                    if (cgr.ID == roomID)
                    {
                        hasJoined = cgr.JoinRoom(authenticatedClient);
                    }
                }

                //this.GetRoomFromID(roomID).JoinRoom(authenticatedClient);
            }

            return hasJoined;
        }

        /// <summary>
        /// Removes a client from a game room.
        /// </summary>
        /// <param name="authenticatedClient">The client to remove.</param>
        /// <param name="roomID">The uniq ID of the room.</param>
        /// <returns>True == the client has been removed, false == an error occured.</returns>
        public bool RemoveClientFromRoom(AuthenticatedClient authenticatedClient, int roomID)
        {
            bool hasBeenRemoved = false;

            lock (this.GameRoomList)
            {
                foreach (ChessGameRoom cgr in this.GameRoomList)
                {
                    if (cgr.ID == roomID)
                    {
                        hasBeenRemoved = cgr.LeaveRoom(authenticatedClient);
                    }
                }
            }

            return hasBeenRemoved;
        }

        /// <summary>
        /// Removes a client from all game rooms.
        /// </summary>
        /// <param name="authenticatedClient">The client to remove.</param>
        public void RemoveClientFromAllRooms(AuthenticatedClient authenticatedClient)
        {
            lock (this.GameRoomList)
            {
                foreach (ChessGameRoom gameRoom in this.GameRoomList)
                {
                    if (gameRoom.IsClientInRoom(authenticatedClient))
                    {
                        gameRoom.LeaveRoom(authenticatedClient);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a command in the given room.
        /// </summary>
        /// <param name="roomID">The room's ID.</param>
        public void ExecuteCommandInRoom(AuthenticatedClient client, ChessCommand command)
        {
            ChessGameRoom gameRoom = this.GetRoomFromID(command.RoomID);
            gameRoom.ExecuteCommand(client, command);
        }

        #endregion
    }
}
