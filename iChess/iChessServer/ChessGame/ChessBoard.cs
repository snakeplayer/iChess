/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessBoard.cs
 * File description: Represents the board of the chess game.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Represents the board of a chess game.
    /// Handle ChessSquare, ChessPiece and ChessMove validation.
    /// Allows to simply use the board from the outside with methods like MoveChessPiece() or GetPossiblePositions().
    /// </summary>
    public class ChessBoard : Object
    {
        #region Consts

        public const int DEFAULT_SIZE_X = 8;
        public const int DEFAULT_SIZE_Y = 8;

        #endregion

        #region Properties

        public ChessSquare[,] ChessSquares { get; set; }
        public List<AbstractChessPiece> PiecesOut { get; set; }
        public TempChessBoard TempBoard { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ChessBoard class.
        /// </summary>
        /// <param name="pSizeX">The width of the board, in squares.</param>
        /// <param name="pSizeY">The height of the board, in squares.</param>
        public ChessBoard(int pSizeX, int pSizeY)
        {
            this.ChessSquares = new ChessSquare[pSizeX, pSizeY];
            this.PiecesOut = new List<AbstractChessPiece>();
            this.InitializeBoard();

            // Temporary ChessBoard
            this.TempBoard = new TempChessBoard(this);
            this.UpdateTempBoard();
        }

        public ChessBoard() : this(DEFAULT_SIZE_X, DEFAULT_SIZE_Y) { }

        #endregion

        #region Methods (Utils)

        /// <summary>
        /// Initialize the board : creates empty squares.
        /// </summary>
        public void InitializeBoard()
        {
            // Go through all squares initialize them
            bool isWhite = false;
            for (int i = 0; i < this.ChessSquares.GetLength(0); i++)
            {
                isWhite = i % 2 == 1;
                for (int j = 0; j < this.ChessSquares.GetLength(1); j++)
                {
                    this.ChessSquares[i, j] = new ChessSquare(isWhite);
                    isWhite = !isWhite;
                }
            }
        }

        /// <summary>
        /// Update the temporary board (used to test check / check mate).
        /// </summary>
        public void UpdateTempBoard()
        {
            this.TempBoard.Update();
        }

        /// <summary>
        /// Test if the given position is on board.
        /// </summary>
        /// <param name="pPosX">The X position.</param>
        /// <param name="pPosY">The Y position.</param>
        /// <returns>True if the position is on board, false if it's not.</returns>
        public bool IsOnBoard(int pPosX, int pPosY)
        {
            bool isOnBoard = false;

            if (pPosX < this.ChessSquares.GetLength(0) && pPosY < this.ChessSquares.GetLength(1) && pPosX >= 0 && pPosY >= 0)
            {
                isOnBoard = true;
            }

            return isOnBoard;
        }

        /// <summary>
        /// Place a ChessPiece on the ChessSquare.
        /// </summary>
        /// <param name="pChessPiece">The ChessPiece.</param>
        /// <param name="pPosX">The X position.</param>
        /// <param name="pPosY">The Y position.</param>
        /// <returns>True if the ChessPiece has been removed, false if it has not.</returns>
        public bool PlaceChessPiece(AbstractChessPiece pChessPiece, int pPosX, int pPosY)
        {
            bool hasBennPlaced = false;

            if (IsOnBoard(pPosX, pPosY))
            {
                this.ChessSquares[pPosX, pPosY].ChessPiece = pChessPiece;
                hasBennPlaced = true;
            }

            return hasBennPlaced;
        }

        /// <summary>
        /// Remove a ChessPiece from ChessSquares.
        /// </summary>
        /// <param name="pChessPiece">The ChessPiece.</param>
        /// <returns>True if the ChessPiece has been removed, false if it has not.</returns>
        public bool RemoveChessPiece(AbstractChessPiece pChessPiece)
        {
            int[] posXY = GetChessPiecePosition(pChessPiece);
            return RemoveChessPiece(posXY[0], posXY[1]);
        }

        /// <summary>
        /// Remove a ChessPiece from ChessSquares.
        /// </summary>
        /// <param name="pPosX">The X position of the ChessPiece.</param>
        /// <param name="pPosY">The X position of the ChessPiece.</param>
        /// <returns>True if the ChessPiece has been removed, false if it has not.</returns>
        public bool RemoveChessPiece(int pPosX, int pPosY)
        {
            bool hasBeenRemoved = false;

            if (IsOnBoard(pPosX, pPosY))
            {
                if (this.ChessSquares[pPosX, pPosY].ChessPiece != null)
                {
                    this.ChessSquares[pPosX, pPosY].ChessPiece = null;
                    hasBeenRemoved = true;
                }
            }

            return hasBeenRemoved;
        }

        /// <summary>
        /// Get a ChessPiece position.
        /// </summary>
        /// <param name="pChessPiece">The ChessPiece.</param>
        /// <returns>The ChessPiece position (int[]) if it has been found, int[-1, -1] if not found.</returns>
        public int[] GetChessPiecePosition(AbstractChessPiece pChessPiece)
        {
            int[] positionXY = new int[2];

            // If the piece is not found, returns -1, -1
            positionXY[0] = -1;
            positionXY[1] = -1;

            // Go through all squares to find the piece's position
            for (int i = 0; i < this.ChessSquares.GetLength(0); i++)
            {
                for (int j = 0; j < this.ChessSquares.GetLength(1); j++)
                {
                    if (this.ChessSquares[i, j].ChessPiece == pChessPiece)
                    {
                        positionXY[0] = i;
                        positionXY[1] = j;
                    }
                }
            }

            return positionXY;
        }

        #endregion

        #region Methods (Moves)

        /// <summary>
        /// Get the new position of a ChessPiece using a ChessMove. 
        /// </summary>
        /// <param name="pChessPiece">The ChessPiece.</param>
        /// <param name="pMove">The ChessMove.</param>
        /// <returns>The new position of the ChessPiece.</returns>
        public int[] GetNewPositionXY(AbstractChessPiece pChessPiece, ChessMove pMove)
        {
            int[] oldPositionXY = GetChessPiecePosition(pChessPiece);
            int[] newPositionXY = new int[2];

            newPositionXY[0] = oldPositionXY[0] + pMove.OffsetXY[0];
            newPositionXY[1] = oldPositionXY[1] + pMove.OffsetXY[1];

            return newPositionXY;
        }

        /// <summary>
        /// Try a ChessMove on a ChessPiece.
        /// </summary>
        /// <param name="pChessPiece">The ChessPiece.</param>
        /// <param name="pMove">The ChessMove.</param>
        /// <returns>True if the ChessPiece is on board after the move.</returns>
        public bool TryMoveIsOnBoard(AbstractChessPiece pChessPiece, ChessMove pMove)
        {
            int[] newPositionXY = GetNewPositionXY(pChessPiece, pMove);

            return IsOnBoard(newPositionXY[0], newPositionXY[1]);
        }

        /// <summary>
        /// Try if a future position for a ChessPiece is allowed (if the move will not make the player check).
        /// </summary>
        /// <param name="pPosXY">The position X and Y of a ChessPiece.</param>
        /// <param name="pOffsetXY">The offset X and Y where the piece will move.</param>
        /// <returns></returns>
        public bool TryMoveIsAllowed(int[] pPosXY, int[] pOffsetXY)
        {
            this.UpdateTempBoard();
            AbstractChessPiece piece = this.TempBoard.ChessSquares[pPosXY[0], pPosXY[1]].ChessPiece;
            this.TempBoard.MoveChessPiece(piece, pPosXY[0] + pOffsetXY[0], pPosXY[1] + pOffsetXY[1]);
            this.TempBoard.RemoveChessPiece(pPosXY[0], pPosXY[1]);
            return !this.TempBoard.IsCheck(piece.IsWhite);
        }

        /// <summary>
        /// Get all future possible positions for a ChessPiece, using his ChessMoves.
        /// </summary>
        /// <param name="pChessPiece">The ChessPiece.</param>
        /// <returns>A List of int[] containing all possible positions.</returns>
        public List<int[]> GetPossiblePositions(AbstractChessPiece pChessPiece)
        {
            List<int[]> lstPossiblePositions = new List<int[]>();
            int[] posXY = GetChessPiecePosition(pChessPiece);
            int offsetX, offsetY;

            foreach (ChessMove move in pChessPiece.Moves)
            {
                offsetX = move.OffsetXY[0];
                offsetY = move.OffsetXY[1];

                if (move.CanGoOver)
                {
                    // Can go over & is repeatable (never used in chess game)
                    if (move.Repeatable)
                    {
                        while (IsOnBoard(posXY[0] + offsetX, posXY[1] + offsetY))
                        {
                            // MOVE
                            if (this.ChessSquares[posXY[0] + offsetX, posXY[1] + offsetY].ChessPiece == null)
                            {
                                lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                            }
                            else // ATTACK
                            {
                                // TODO
                            }
                            offsetX += move.OffsetXY[0];
                            offsetY += move.OffsetXY[1];
                        }
                    }
                    else // Can go over & isn't repeatable (Knight)
                    {
                        if (IsOnBoard(posXY[0] + offsetX, posXY[1] + offsetY))
                        {
                            // MOVE
                            if (this.ChessSquares[posXY[0] + offsetX, posXY[1] + offsetY].ChessPiece == null)
                            {
                                // Test if the move make the player check (if yes, don't allow the move)
                                if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                                {
                                    lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY }); // The move is allowed and added to the list
                                }
                            }
                            else // ATTACK
                            {
                                if (this.ChessSquares[posXY[0] + offsetX, posXY[1] + offsetY].ChessPiece.IsWhite != pChessPiece.IsWhite)
                                {
                                    // Test if the move make the player check (if yes, don't allow the move)
                                    if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                                    {
                                        lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                                    }
                                }
                            }
                        }
                    }
                }
                else // Can't go over & is repeatable (Bishop, Rook, Queen)
                {
                    if (move.Repeatable)
                    {
                        bool over = false;
                        while (IsOnBoard(posXY[0] + offsetX, posXY[1] + offsetY) && !over)
                        {
                            // MOVE
                            if (this.ChessSquares[posXY[0] + offsetX, posXY[1] + offsetY].ChessPiece == null)
                            {
                                // Test if the move make the player check (if yes, don't allow the move)
                                if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                                {
                                    lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                                }
                            }
                            else // ATTACK
                            {
                                if (this.ChessSquares[posXY[0] + offsetX, posXY[1] + offsetY].ChessPiece.IsWhite != pChessPiece.IsWhite)
                                {
                                    // Test if the move make the player check (if yes, don't allow the move)
                                    if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                                    {
                                        lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                                    }
                                }
                                over = true;
                            }

                            offsetX += move.OffsetXY[0];
                            offsetY += move.OffsetXY[1];
                        }
                    }
                    else // Can't go over & isn't repeatable (Pawn, King)
                    {
                        if (IsOnBoard(posXY[0] + offsetX, posXY[1] + offsetY))
                        {
                            // MOVE
                            if (this.ChessSquares[posXY[0] + offsetX, posXY[1] + offsetY].ChessPiece == null)
                            {
                                if (!move.IsAttackMoveOnly)
                                {
                                    // Test if the move make the player check (if yes, don't allow the move)
                                    if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                                    {
                                        lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                                    }
                                }
                            }
                            else // ATTACK
                            {
                                if (this.ChessSquares[posXY[0] + offsetX, posXY[1] + offsetY].ChessPiece.IsWhite != pChessPiece.IsWhite)
                                {
                                    if (!move.IsNotAttackMove)
                                    {
                                        // Test if the move make the player check (if yes, don't allow the move)
                                        if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                                        {
                                            lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Special moves, hard-coded (sorry)
            if (!pChessPiece.HasMoved)
            {
                // White Pawn
                if (pChessPiece.Type == ChessPieceType.Pawn && pChessPiece.IsWhite)
                {
                    if (this.ChessSquares[posXY[0], posXY[1] + 1].ChessPiece == null &&
                        this.ChessSquares[posXY[0], posXY[1] + 2].ChessPiece == null)
                    {
                        // Test if the move make the player check (if yes, don't allow the move)
                        offsetX = 0;
                        offsetY = 2;
                        if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                        {
                            lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                        }
                    }
                }

                // Black Pawn
                if (pChessPiece.Type == ChessPieceType.Pawn && !pChessPiece.IsWhite)
                {
                    if (this.ChessSquares[posXY[0], posXY[1] - 1].ChessPiece == null &&
                        this.ChessSquares[posXY[0], posXY[1] - 2].ChessPiece == null)
                    {
                        // Test if the move make the player check (if yes, don't allow the move)
                        offsetX = 0;
                        offsetY = -2;
                        if (TryMoveIsAllowed(posXY, new int[2] { offsetX, offsetY }))
                        {
                            lstPossiblePositions.Add(new int[] { posXY[0] + offsetX, posXY[1] + offsetY });
                        }
                    }
                }
            }

            return lstPossiblePositions;
        }

        /// <summary>
        /// Move a ChessPiece.
        /// </summary>
        /// <param name="pChessPiece">The ChessPiece to move.</param>
        /// <param name="pPosX">The future X position of the ChessPiece.</param>
        /// <param name="pPosY">The future Y position of the ChessPiece.</param>
        /// <returns>True if the ChessPiece has been moved, false if it's not (for example, if it want to move on a friendly ChessPiece).</returns>
        public bool MoveChessPiece(AbstractChessPiece pChessPiece, int pPosX, int pPosY)
        {
            bool hasMoved = false;
            int[] oldPosXY = GetChessPiecePosition(pChessPiece);

            if (this.ChessSquares[pPosX, pPosY].ChessPiece != null)
            {
                if (this.ChessSquares[pPosX, pPosY].ChessPiece.IsWhite != pChessPiece.IsWhite)
                {
                    this.PiecesOut.Add(this.ChessSquares[pPosX, pPosY].ChessPiece);
                }
            }

            PlaceChessPiece(pChessPiece, pPosX, pPosY);
            RemoveChessPiece(oldPosXY[0], oldPosXY[1]);

            pChessPiece.HasMoved = true;

            this.UpdateTempBoard();

            return hasMoved;
        }

        #endregion

        #region Methods (Check / checkmate)

        /// <summary>
        /// Test is a ChessPlayer is check.
        /// </summary>
        /// <param name="pIsWhitePlayer">True if you want to test the white player, false if you want to test the black.</param>
        /// <returns>True if the player is check, false if it's not.</returns>
        public bool IsCheck(bool pIsWhitePlayer)
        {
            AbstractChessPiece king;
            AbstractChessPiece piece;
            List<int[]> lstPositions = new List<int[]>();
            int[] posXYKing = new int[2];
            bool isCheck = false;


            // Go through all squares to find the king
            for (int i = 0; i < this.ChessSquares.GetLength(0); i++)
            {
                for (int j = 0; j < this.ChessSquares.GetLength(1); j++)
                {
                    if (this.ChessSquares[i, j].ChessPiece != null &&
                        this.ChessSquares[i, j].ChessPiece.Type == ChessPieceType.King &&
                        this.ChessSquares[i, j].ChessPiece.IsWhite == pIsWhitePlayer)
                    {
                        king = this.ChessSquares[i, j].ChessPiece;
                        posXYKing[0] = i;
                        posXYKing[1] = j;
                    }
                }
            }

            // Go through all piece to find if an enemy piece can move to the king
            for (int i = 0; i < this.ChessSquares.GetLength(0); i++)
            {
                for (int j = 0; j < this.ChessSquares.GetLength(1); j++)
                {
                    if (this.ChessSquares[i, j].ChessPiece != null)
                    {
                        piece = this.ChessSquares[i, j].ChessPiece;

                        lstPositions = this.GetPossiblePositions(piece);

                        // Go through all piece's possible positions
                        foreach (int[] posXYPiece in lstPositions)
                        {
                            if (posXYPiece[0] == posXYKing[0] &&
                                posXYPiece[1] == posXYKing[1])
                            {
                                isCheck = true;
                            }
                        }
                    }
                }
            }

            return isCheck;
        }

        /// <summary>
        /// Test is a ChessPlayer is check.
        /// </summary>
        /// <param name="pPlayer">The ChessPlayer you want to test.</param>
        /// <returns>True if the player is check, false if it's not.</returns>
        public bool IsCheck(ChessPlayer pPlayer)
        {
            return this.IsCheck(pPlayer.IsWhite);
        }

        /// <summary>
        /// Test if a ChessPlayer is checkmate.
        /// </summary>
        /// <param name="pIsWhitePlayer">True if you want to test the white player, false if you want to test the black.</param>
        /// <returns>True if the player is checkmate, false if it's not.</returns>
        public bool IsCheckMate(bool pIsWhitePlayer)
        {
            bool isCheckMate = true;
            List<int[]> lstPositions = new List<int[]>();

            for (int i = 0; i < this.ChessSquares.GetLength(0); i++)
            {
                for (int j = 0; j < this.ChessSquares.GetLength(1); j++)
                {
                    if (this.ChessSquares[i, j].ChessPiece != null)
                    {
                        if (this.ChessSquares[i, j].ChessPiece.IsWhite == pIsWhitePlayer)
                        {
                            lstPositions = this.GetPossiblePositions(this.ChessSquares[i, j].ChessPiece);
                            if (lstPositions.Count > 0)
                            {
                                isCheckMate = false;
                            }
                        }
                    }
                }
            }

            return isCheckMate;
        }

        /// <summary>
        /// Test if a ChessPlayer is checkmate.
        /// </summary>
        /// <param name="pPlayer">The ChessPlayer you want to test.</param>
        /// <returns>True if the player is checkmate, false if it's not.</returns>
        public bool IsCheckMate(ChessPlayer pPlayer)
        {
            return this.IsCheckMate(pPlayer.IsWhite);
        }

        #endregion
    }
}
