/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessGameModel.cs
 * File description: Represents a chess game itself.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// The model of the chess game.
    /// You can easily start a game and play with methods like TakePiece() or PlaceCurrentPiece().
    /// </summary>
    public class ChessGameModel : Object
    {
        #region Properties

        public ChessBoard Board { get; set; }

        public ChessPlayer Player1 { get; set; }

        public ChessPlayer Player2 { get; set; }

        public bool PlayerTurn { get; set; } // False = Player1, True = Player2

        public ChessPlayer CurrentPlayer
        {
            get
            {
                return !PlayerTurn ? Player1 : Player2;
            }
        }

        public AbstractChessPiece CurrentPiece
        {
            get
            {
                return this.CurrentPlayer.CurrentPiece;
            }
            set
            {
                this.CurrentPlayer.CurrentPiece = value;
            }
        }

        public List<int[]> PossiblePositionsForCurrentPiece
        {
            get
            {
                if (this.CurrentPiece != null)
                {
                    return this.GetPossiblePositions(this.CurrentPlayer.CurrentPiece);
                }
                return new List<int[]>();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of ChessGameModel class.
        /// </summary>
        /// <param name="pNamePlayer1">Name of Player1.</param>
        /// <param name="pNamePlayer2">Name of Player1.</param>
        /// <param name="pTimeInSeconds">Time for each player, in seconds. If == -1 --> no time in the game.</param>
        public ChessGameModel(string pNamePlayer1, string pNamePlayer2, int pTimeInSeconds)
        {
            this.Board = new ChessBoard();

            // White pieces
            this.Board.ChessSquares[0, 0].ChessPiece = new ChessPieceWhiteRook();
            this.Board.ChessSquares[1, 0].ChessPiece = new ChessPieceWhiteKnightLeft();
            this.Board.ChessSquares[2, 0].ChessPiece = new ChessPieceWhiteBishop();
            this.Board.ChessSquares[3, 0].ChessPiece = new ChessPieceWhiteQueen();
            this.Board.ChessSquares[4, 0].ChessPiece = new ChessPieceWhiteKing();
            this.Board.ChessSquares[5, 0].ChessPiece = new ChessPieceWhiteBishop();
            this.Board.ChessSquares[6, 0].ChessPiece = new ChessPieceWhiteKnightRight();
            this.Board.ChessSquares[7, 0].ChessPiece = new ChessPieceWhiteRook();

            for (int i = 0; i < 8; i++)
            {
                this.Board.ChessSquares[i, 1].ChessPiece = new ChessPieceWhitePawn();
            }

            // Black pieces
            this.Board.ChessSquares[0, 7].ChessPiece = new ChessPieceBlackRook();
            this.Board.ChessSquares[1, 7].ChessPiece = new ChessPieceBlackKnightLeft();
            this.Board.ChessSquares[2, 7].ChessPiece = new ChessPieceBlackBishop();
            this.Board.ChessSquares[3, 7].ChessPiece = new ChessPieceBlackQueen();
            this.Board.ChessSquares[4, 7].ChessPiece = new ChessPieceBlackKing();
            this.Board.ChessSquares[5, 7].ChessPiece = new ChessPieceBlackBishop();
            this.Board.ChessSquares[6, 7].ChessPiece = new ChessPieceBlackKnightRight();
            this.Board.ChessSquares[7, 7].ChessPiece = new ChessPieceBlackRook();

            for (int i = 0; i < 8; i++)
            {
                this.Board.ChessSquares[i, 6].ChessPiece = new ChessPieceBlackPawn();
            }

            // Players
            this.Player1 = new ChessPlayer(pNamePlayer1, pTimeInSeconds, false, this);
            this.Player2 = new ChessPlayer(pNamePlayer2, pTimeInSeconds, true, this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the squares from the ChessBoard.
        /// </summary>
        /// <returns>The ChessSquare[,] from the ChessBoard.</returns>
        public ChessSquare[,] GetSquares()
        {
            return this.Board.ChessSquares;
        }

        /// <summary>
        /// Get the piece out of the game.
        /// </summary>
        /// <returns>A list of AbstractChessPieces with all pieces out.</returns>
        public List<AbstractChessPiece> GetPiecesOut()
        {
            return this.Board.PiecesOut;
        }

        /// <summary>
        /// Get the name of the current ChessPlayer.
        /// </summary>
        /// <returns>The name of the current ChessPlayer.</returns>
        public string GetCurrentPlayerName()
        {
            return this.CurrentPlayer.Name;
        }

        /// <summary>
        /// Get the position of a ChessPiece.
        /// </summary>
        /// <param name="pPiece">The ChessPiece we want to know the position on the ChessBoard.</param>
        /// <returns>A int[] with the ChessPiece's coordonates.</returns>
        public int[] GetChessPiecePosition(AbstractChessPiece pPiece)
        {
            return this.Board.GetChessPiecePosition(pPiece);
        }

        /// <summary>
        /// Get all future possible positions for a ChessPiece.
        /// </summary>
        /// <param name="pPiece">The ChessPiece.</param>
        /// <returns>A list of int[] containing all future possible positions.</returns>
        public List<int[]> GetPossiblePositions(AbstractChessPiece pPiece)
        {
            return this.Board.GetPossiblePositions(pPiece);
        }

        public List<int[]> GetPossiblePositionsForCurrentPiece()
        {
            List<int[]> possiblePositions = new List<int[]>();

            if (this.CurrentPiece != null)
            {
                possiblePositions = this.GetPossiblePositions(this.CurrentPiece);
            }

            return possiblePositions;
        }

        /// <summary>
        /// Take a piece with the current ChessPlayer.
        /// </summary>
        /// <param name="pPiece">The ChessPiece to take.</param>
        public void TakePiece(AbstractChessPiece pPiece)
        {
            this.CurrentPlayer.TakePiece(pPiece);
        }

        /// <summary>
        /// Place a ChessPiece on the ChessBoard.
        /// </summary>
        /// <param name="pPiece">The ChessPiece to place.</param>
        /// <param name="pPosX">The future X position of the ChessPiece.</param>
        /// <param name="pPosY">The future Y position of the ChessPiece.</param>
        public void PlacePiece(AbstractChessPiece pPiece, int pPosX, int pPosY)
        {
            this.Board.MoveChessPiece(pPiece, pPosX, pPosY);
            this.PlayerTurn = !this.PlayerTurn;
        }

        /// <summary>
        /// Place the current ChessPiece with the current ChessPlayer.
        /// </summary>
        /// <param name="pPosX">The future X position of the ChessPiece.</param>
        /// <param name="pPosY">The future Y position of the ChessPiece.</param>
        public void PlaceCurrentPiece(int pPosX, int pPosY)
        {
            this.CurrentPlayer.PlaceCurrentPiece(pPosX, pPosY);
        }

        public Dictionary<int[], string> GetAllPiecesPositions()
        {
            ChessSquare[,] chessSquares = this.GetSquares();
            Dictionary<int[], string> piecesPositions = new Dictionary<int[], string>();
            for (int x = 0; x < 8; x++) // TODO : MAGIC VALUES !
            {
                for (int y = 0; y < 8; y++)
                {
                    if (chessSquares[x, y].ChessPiece != null)
                    {
                        piecesPositions.Add(new int[2] { x, y }, chessSquares[x, y].ChessPiece.Name);
                    }
                }
            }
            return piecesPositions;
        }

        public ChessBoardSerializable GetChessBoardSerializable() // TODO : DELEGATE TO CHESSBOARD !
        {
            // Init
            ChessSquare[,] chessSquares = this.GetSquares();
            ChessSquareSerializable[] chessSquaresSerializable = new ChessSquareSerializable[64];

            // Pieces positions
            Dictionary<int[], string> piecesPositions = this.GetAllPiecesPositions();

            // Fill empty squares
            for (int i = 0; i < 64; i++)
            {
                chessSquaresSerializable[i] = new ChessSquareSerializable(string.Empty, false, false);
            }

            // Chess pieces
            foreach (KeyValuePair<int[], string> piece in piecesPositions)
            {
                int position = piece.Key[0] + piece.Key[1] * 8;
                chessSquaresSerializable[position].ChessPiece = piece.Value; //= new ChessSquareSerializable(pieces.Value, false, false);
            }

            // Possible moves
            List<int[]> possibleMovesPositions = this.GetPossiblePositionsForCurrentPiece();
            foreach (int[] possibleMove in possibleMovesPositions)
            {
                int position = possibleMove[0] + possibleMove[1] * 8;
                chessSquaresSerializable[position].IsPossibleMove = true;
                if (chessSquaresSerializable[position].ChessPiece != string.Empty)
                {
                    // TODO : ATTACK
                }
            }

            // Attack move
            

            ChessBoardSerializable chessBoardSerializable = new ChessBoardSerializable(chessSquaresSerializable);

            return chessBoardSerializable;
        }

        public void ExecuteCommand(ChessCommand command)
        {
            int posX = command.PositionXY[0];
            int posY = command.PositionXY[1];
            ChessSquare[,] chessSquares = this.GetSquares();

            AbstractChessPiece chessPiece = chessSquares[posX, posY].ChessPiece;

            if (chessPiece != null)
            {
                if (this.CurrentPiece != null && this.CurrentPiece.IsWhite != chessPiece.IsWhite)
                {
                    this.PlaceCurrentPiece(posX, posY);
                }
                else
                {
                    this.TakePiece(chessPiece);
                }

                //TestIfCheck();
            }
            else
            {
                this.PlaceCurrentPiece(posX, posY);
                //TestIfCheck();
            }
        }

        #endregion
    }
}
