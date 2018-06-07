/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: AbstractChessPiece.cs
 * File description: An abstract class for chess pieces creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Possible types for chess pieces.
    /// </summary>
    public enum ChessPieceType
    {
        Pawn,
        Rook,
        Knight_Left,
        Knight_Right,
        Bishop,
        Queen,
        King
    }

    /// <summary>
    /// An abstract class for chess pieces creation.
    /// </summary>
    public abstract class AbstractChessPiece
    {
        #region Consts

        public static string WHITE_PAWN_NAME = "Dawn_White";
        public static string WHITE_ROOK_NAME = "Rook_White";
        public static string WHITE_KNIGHT_LEFT_NAME = "Knight_Left_White";
        public static string WHITE_KNIGHT_RIGHT_NAME = "Knight_Right_White";
        public static string WHITE_BISHOP_NAME = "Bishop_White";
        public static string WHITE_QUEEN_NAME = "Queen_White";
        public static string WHITE_KING_NAME = "King_White";

        public static string BLACK_PAWN_NAME = "Dawn_Black";
        public static string BLACK_ROOK_NAME = "Rook_Black";
        public static string BLACK_KNIGHT_LEFT_NAME = "Knight_Left_Black";
        public static string BLACK_KNIGHT_RIGHT_NAME = "Knight_Right_Black";
        public static string BLACK_BISHOP_NAME = "Bishop_Black";
        public static string BLACK_QUEEN_NAME = "Queen_Black";
        public static string BLACK_KING_NAME = "King_Black";

        #endregion

        #region Properties

        public string Name { get; set; }
        public ChessPieceType Type { get; set; }
        public List<ChessMove> Moves { get; set; }
        public bool IsWhite { get; set; }
        public bool HasMoved { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AbstractChessPiece abstract class.
        /// </summary>
        /// <param name="pType">The type of the piece, from ChessPieceType enum.</param>
        /// <param name="pIsWhite">True if the piece is white, false if the piece is black.</param>
        public AbstractChessPiece(string pName, ChessPieceType pType, bool pIsWhite)
        {
            this.Name = pName;
            this.Type = pType;
            this.Moves = new List<ChessMove>();
            this.IsWhite = pIsWhite;
            this.HasMoved = false;
        }

        /// <summary>
        /// Clone an AbstractChessPiece (ugly, I'm sorry)
        /// </summary>
        /// <returns>A cloned ChessPiece</returns>
        public AbstractChessPiece Clone()
        {
            AbstractChessPiece clonedChessPiece = new ChessPieceWhitePawn();

            if (this.Type == ChessPieceType.Pawn)
            {
                if (this.IsWhite)
                {
                    clonedChessPiece = new ChessPieceWhitePawn();
                }
                else
                {
                    clonedChessPiece = new ChessPieceBlackPawn();
                }
            }
            else if (this.Type == ChessPieceType.Rook)
            {
                if (this.IsWhite)
                {
                    clonedChessPiece = new ChessPieceWhiteRook();
                }
                else
                {
                    clonedChessPiece = new ChessPieceBlackRook();
                }
            }
            else if (this.Type == ChessPieceType.Knight_Left)
            {
                if (this.IsWhite)
                {
                    clonedChessPiece = new ChessPieceWhiteKnightLeft();
                }
                else
                {
                    clonedChessPiece = new ChessPieceBlackKnightLeft();
                }
            }
            else if (this.Type == ChessPieceType.Knight_Right)
            {
                if (this.IsWhite)
                {
                    clonedChessPiece = new ChessPieceWhiteKnightRight();
                }
                else
                {
                    clonedChessPiece = new ChessPieceBlackKnightRight();
                }
            }
            else if (this.Type == ChessPieceType.Bishop)
            {
                if (this.IsWhite)
                {
                    clonedChessPiece = new ChessPieceWhiteBishop();
                }
                else
                {
                    clonedChessPiece = new ChessPieceBlackBishop();
                }
            }
            else if (this.Type == ChessPieceType.Queen)
            {
                if (this.IsWhite)
                {
                    clonedChessPiece = new ChessPieceWhiteQueen();
                }
                else
                {
                    clonedChessPiece = new ChessPieceBlackQueen();
                }
            }
            else if (this.Type == ChessPieceType.King)
            {
                if (this.IsWhite)
                {
                    clonedChessPiece = new ChessPieceWhiteKing();
                }
                else
                {
                    clonedChessPiece = new ChessPieceBlackKing();
                }
            }

            clonedChessPiece.Name = this.Name;
            clonedChessPiece.Type = this.Type;
            clonedChessPiece.Moves = this.Moves;
            clonedChessPiece.IsWhite = this.IsWhite;
            clonedChessPiece.HasMoved = this.HasMoved;

            return clonedChessPiece;
        }

        #endregion
    }
}
