/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessPieces.cs
 * File description: This file contains all chess pieces inherited from AbstractChessPiece.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// White Pawn.
    /// </summary>
    public class ChessPieceWhitePawn : AbstractChessPiece
    {
        public ChessPieceWhitePawn() : base(WHITE_PAWN_NAME, ChessPieceType.Pawn, true)
        {
            this.Moves.Add(new ChessMove(0, 1, false, false, false, true));
            this.Moves.Add(new ChessMove(1, 1, false, false, true, false));
            this.Moves.Add(new ChessMove(-1, 1, false, false, true, false));
        }
    }

    /// <summary>
    /// White Rook.
    /// </summary>
    public class ChessPieceWhiteRook : AbstractChessPiece
    {
        public ChessPieceWhiteRook() : base(WHITE_ROOK_NAME, ChessPieceType.Rook, true)
        {
            this.Moves.Add(new ChessMove(1, 0, false, true));
            this.Moves.Add(new ChessMove(0, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 0, false, true));
            this.Moves.Add(new ChessMove(0, -1, false, true));
        }
    }

    /// <summary>
    /// White Knight Left.
    /// </summary>
    public class ChessPieceWhiteKnightLeft : AbstractChessPiece
    {
        public ChessPieceWhiteKnightLeft() : base(WHITE_KNIGHT_LEFT_NAME, ChessPieceType.Knight_Left, true)
        {
            this.Moves.Add(new ChessMove(1, 2, true, false));
            this.Moves.Add(new ChessMove(2, 1, true, false));
            this.Moves.Add(new ChessMove(-1, 2, true, false));
            this.Moves.Add(new ChessMove(-2, 1, true, false));
            this.Moves.Add(new ChessMove(-2, -1, true, false));
            this.Moves.Add(new ChessMove(-1, -2, true, false));
            this.Moves.Add(new ChessMove(1, -2, true, false));
            this.Moves.Add(new ChessMove(2, -1, true, false));
        }
    }

    /// <summary>
    /// White Knight Right.
    /// </summary>
    public class ChessPieceWhiteKnightRight : AbstractChessPiece
    {
        public ChessPieceWhiteKnightRight() : base(WHITE_KNIGHT_RIGHT_NAME, ChessPieceType.Knight_Right, true)
        {
            this.Moves.Add(new ChessMove(1, 2, true, false));
            this.Moves.Add(new ChessMove(2, 1, true, false));
            this.Moves.Add(new ChessMove(-1, 2, true, false));
            this.Moves.Add(new ChessMove(-2, 1, true, false));
            this.Moves.Add(new ChessMove(-2, -1, true, false));
            this.Moves.Add(new ChessMove(-1, -2, true, false));
            this.Moves.Add(new ChessMove(1, -2, true, false));
            this.Moves.Add(new ChessMove(2, -1, true, false));
        }
    }

    /// <summary>
    /// White Bishop.
    /// </summary>
    public class ChessPieceWhiteBishop : AbstractChessPiece
    {
        public ChessPieceWhiteBishop() : base(WHITE_BISHOP_NAME, ChessPieceType.Bishop, true)
        {
            this.Moves.Add(new ChessMove(1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, -1, false, true));
            this.Moves.Add(new ChessMove(1, -1, false, true));
        }
    }

    /// <summary>
    /// White Queen.
    /// </summary>
    public class ChessPieceWhiteQueen : AbstractChessPiece
    {
        public ChessPieceWhiteQueen() : base(WHITE_QUEEN_NAME, ChessPieceType.Queen, true)
        {
            this.Moves.Add(new ChessMove(1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, -1, false, true));
            this.Moves.Add(new ChessMove(1, -1, false, true));
            this.Moves.Add(new ChessMove(1, 0, false, true));
            this.Moves.Add(new ChessMove(0, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 0, false, true));
            this.Moves.Add(new ChessMove(0, -1, false, true));
        }
    }

    /// <summary>
    /// White King.
    /// </summary>
    public class ChessPieceWhiteKing : AbstractChessPiece
    {
        public ChessPieceWhiteKing() : base(WHITE_KING_NAME, ChessPieceType.King, true)
        {
            this.Moves.Add(new ChessMove(1, 1, false, false));
            this.Moves.Add(new ChessMove(-1, 1, false, false));
            this.Moves.Add(new ChessMove(-1, -1, false, false));
            this.Moves.Add(new ChessMove(1, -1, false, false));
            this.Moves.Add(new ChessMove(1, 0, false, false));
            this.Moves.Add(new ChessMove(0, 1, false, false));
            this.Moves.Add(new ChessMove(-1, 0, false, false));
            this.Moves.Add(new ChessMove(0, -1, false, false));
        }
    }

    /// <summary>
    /// Black Pawn.
    /// </summary>
    public class ChessPieceBlackPawn : AbstractChessPiece
    {
        public ChessPieceBlackPawn() : base(BLACK_PAWN_NAME, ChessPieceType.Pawn, false)
        {
            this.Moves.Add(new ChessMove(0, -1, false, false, false, true));
            this.Moves.Add(new ChessMove(-1, -1, false, false, true, false));
            this.Moves.Add(new ChessMove(1, -1, false, false, true, false));
        }
    }

    /// <summary>
    /// Black Rook.
    /// </summary>
    public class ChessPieceBlackRook : AbstractChessPiece
    {
        public ChessPieceBlackRook() : base(BLACK_ROOK_NAME, ChessPieceType.Rook, false)
        {
            this.Moves.Add(new ChessMove(1, 0, false, true));
            this.Moves.Add(new ChessMove(0, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 0, false, true));
            this.Moves.Add(new ChessMove(0, -1, false, true));
        }
    }

    /// <summary>
    /// Balck Knight Left.
    /// </summary>
    public class ChessPieceBlackKnightLeft : AbstractChessPiece
    {
        public ChessPieceBlackKnightLeft() : base(BLACK_KNIGHT_LEFT_NAME, ChessPieceType.Knight_Left, false)
        {
            this.Moves.Add(new ChessMove(1, 2, true, false));
            this.Moves.Add(new ChessMove(2, 1, true, false));
            this.Moves.Add(new ChessMove(-1, 2, true, false));
            this.Moves.Add(new ChessMove(-2, 1, true, false));
            this.Moves.Add(new ChessMove(-2, -1, true, false));
            this.Moves.Add(new ChessMove(-1, -2, true, false));
            this.Moves.Add(new ChessMove(1, -2, true, false));
            this.Moves.Add(new ChessMove(2, -1, true, false));
        }
    }

    /// <summary>
    /// White Knight Right.
    /// </summary>
    public class ChessPieceBlackKnightRight : AbstractChessPiece
    {
        public ChessPieceBlackKnightRight() : base(BLACK_KNIGHT_RIGHT_NAME, ChessPieceType.Knight_Right, false)
        {
            this.Moves.Add(new ChessMove(1, 2, true, false));
            this.Moves.Add(new ChessMove(2, 1, true, false));
            this.Moves.Add(new ChessMove(-1, 2, true, false));
            this.Moves.Add(new ChessMove(-2, 1, true, false));
            this.Moves.Add(new ChessMove(-2, -1, true, false));
            this.Moves.Add(new ChessMove(-1, -2, true, false));
            this.Moves.Add(new ChessMove(1, -2, true, false));
            this.Moves.Add(new ChessMove(2, -1, true, false));
        }
    }

    /// <summary>
    /// Black Bishop.
    /// </summary>
    public class ChessPieceBlackBishop : AbstractChessPiece
    {
        public ChessPieceBlackBishop() : base(BLACK_BISHOP_NAME, ChessPieceType.Bishop, false)
        {
            this.Moves.Add(new ChessMove(1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, -1, false, true));
            this.Moves.Add(new ChessMove(1, -1, false, true));
        }
    }

    /// <summary>
    /// Black Queen.
    /// </summary>
    public class ChessPieceBlackQueen : AbstractChessPiece
    {
        public ChessPieceBlackQueen() : base(BLACK_QUEEN_NAME, ChessPieceType.Queen, false)
        {
            this.Moves.Add(new ChessMove(1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 1, false, true));
            this.Moves.Add(new ChessMove(-1, -1, false, true));
            this.Moves.Add(new ChessMove(1, -1, false, true));
            this.Moves.Add(new ChessMove(1, 0, false, true));
            this.Moves.Add(new ChessMove(0, 1, false, true));
            this.Moves.Add(new ChessMove(-1, 0, false, true));
            this.Moves.Add(new ChessMove(0, -1, false, true));
        }
    }

    /// <summary>
    /// White King.
    /// </summary>
    public class ChessPieceBlackKing : AbstractChessPiece
    {
        public ChessPieceBlackKing() : base(BLACK_KING_NAME, ChessPieceType.King, false)
        {
            this.Moves.Add(new ChessMove(1, 1, false, false));
            this.Moves.Add(new ChessMove(-1, 1, false, false));
            this.Moves.Add(new ChessMove(-1, -1, false, false));
            this.Moves.Add(new ChessMove(1, -1, false, false));
            this.Moves.Add(new ChessMove(1, 0, false, false));
            this.Moves.Add(new ChessMove(0, 1, false, false));
            this.Moves.Add(new ChessMove(-1, 0, false, false));
            this.Moves.Add(new ChessMove(0, -1, false, false));
        }
    }
}
