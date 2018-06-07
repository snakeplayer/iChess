/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ChessCommand.cs
 * File description: Wrapper used to send chess command to the server.
 */

using System;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessClient
{
    /// <summary>
    /// Wrapper used to send chess command to the server.
    /// </summary>
    [ProtoContract]
    public class ChessCommand
    {
        [ProtoMember(1)]
        public int RoomID { get; set; }

        [ProtoMember(2)]
        public int[] PositionXY { get; set; }

        public ChessCommand()
        {

        }

        public ChessCommand(int roomID, int[] positionXY)
        {
            this.RoomID = roomID;
            this.PositionXY = positionXY;
        }
    }
}
