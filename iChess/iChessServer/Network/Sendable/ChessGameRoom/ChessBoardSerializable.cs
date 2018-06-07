/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessBoardSerializable.cs
 * File description: Wrapper for chess's board recovering.
 */

using System;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    [ProtoContract]
    public class ChessBoardSerializable
    {
        [ProtoMember(1)]
        public ChessSquareSerializable[] ChessSquares { get; set; }

        public ChessBoardSerializable()
        {

        }

        public ChessBoardSerializable(ChessSquareSerializable[] chessSquares)
        {
            this.ChessSquares = chessSquares;
        }
    }
}
