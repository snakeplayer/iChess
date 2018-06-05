/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RoomItem.cs
 * File description: Used to display rooms.
 */

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessClient
{
    /// <summary>
    /// Used to display rooms.
    /// </summary>
    [ProtoContract]
    public class RoomItem
    {
        /// <summary>
        /// Represents the room's ID.
        /// </summary>
        [ProtoMember(1)]
        public string RoomID { get; set; }

        /// <summary>
        /// The name of the host player.
        /// </summary>
        [ProtoMember(2)]
        public string HostPlayerName { get; set; }

        /// <summary>
        /// The number of minutes per players.
        /// </summary>
        [ProtoMember(3)]
        public string MinutesPerPlayer { get; set; }
    }
}
