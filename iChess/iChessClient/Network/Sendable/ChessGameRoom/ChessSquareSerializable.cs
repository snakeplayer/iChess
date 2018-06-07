/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ChessSquareSerializable.cs
 * File description: Wrapper for chess's square recovering.
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
    /// Wrapper for chess's square recovering.
    /// </summary>
    [ProtoContract]
    public class ChessSquareSerializable
    {
        [ProtoMember(1)]
        public string ChessPiece { get; set; }

        [ProtoMember(2)]
        public bool IsAttack { get; set; }

        [ProtoMember(3)]
        public bool IsPossibleMove { get; set; }

        public ChessSquareSerializable()
        {
            this.ChessPiece = string.Empty;
            this.IsAttack = false;
            this.IsPossibleMove = false;
        }

        public ChessSquareSerializable(string chessPiece, bool isAttack, bool isPossibleMove)
        {
            this.ChessPiece = chessPiece;
            this.IsAttack = isAttack;
            this.IsPossibleMove = isPossibleMove;
        }
    }
}
