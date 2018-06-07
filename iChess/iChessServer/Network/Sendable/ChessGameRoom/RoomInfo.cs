/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: RoomInfo.cs
 * File description: Wrapper for room's informations recovering.
 */

using System;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Wrapper for room's informations recovering.
    /// </summary>
    [ProtoContract]
    public class RoomInfo
    {
        [ProtoMember(1)]
        public string HostPlayerName { get; set; }

        [ProtoMember(2)]
        public string GuestPlayerName { get; set; }

        [ProtoMember(3)]
        public int HostPlayerSecondsLeft { get; set; }

        [ProtoMember(4)]
        public int GuestPlayerSecondsLeft { get; set; }

        [ProtoMember(5)]
        public List<string> HostPlayerPiecesOut { get; set; }

        [ProtoMember(6)]
        public List<string> GuestPlayerPiecesOut { get; set; }

        /// <summary>
        /// True == host, false == guest.
        /// </summary>
        [ProtoMember(7)]
        public bool PlayerTurn { get; set; }

        [ProtoMember(8)]
        public string ChatMessages { get; set; }

        [ProtoMember(9)]
        public ChessBoardSerializable ChessBoard { get; set; }
    }
}
