/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessPlayer.cs
 * File description: Represents a chess game player.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Represents a chess game player.
    /// </summary>
    public class ChessPlayer : Object
    {
        #region Properties

        public string Name { get; set; }
        public int Time { get; set; }
        public bool IsWhite { get; set; }

        public ChessGameModel GameModel { get; set; }
        public AbstractChessPiece CurrentPiece { get; set; }
        public List<int[]> LstPossiblePositions
        {
            get
            {
                if (this.CurrentPiece != null)
                {
                    return this.GameModel.GetPossiblePositions(this.CurrentPiece);
                }
                return null;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ChessPlayer class.
        /// </summary>
        /// <param name="pName">The name of the ChessPlayer.</param>
        /// <param name="pTime">The time he has to play.</param>
        /// <param name="pIsWhite">If the ChessPlayer is white.</param>
        /// <param name="pGameModel">The GameModel.</param>
        public ChessPlayer(string pName, int pTime, bool pIsWhite, ChessGameModel pGameModel)
        {
            this.Name = pName;
            this.Time = pTime;
            this.IsWhite = pIsWhite;
            this.GameModel = pGameModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Take a ChessPiece.
        /// </summary>
        /// <param name="pPiece">The ChessPiece to take.</param>
        public void TakePiece(AbstractChessPiece pPiece)
        {
            if (pPiece.IsWhite != this.IsWhite)
            {
                this.CurrentPiece = pPiece;
            }
        }

        /// <summary>
        /// Place the ChessPiece he taked.
        /// </summary>
        /// <param name="pPosX">The future X position of the ChessPiece.</param>
        /// <param name="pPosY">The future Y position of the ChessPiece.</param>
        public void PlaceCurrentPiece(int pPosX, int pPosY)
        {
            if (this.LstPossiblePositions != null)
            {
                foreach (int[] posXY in this.LstPossiblePositions)
                {
                    if (pPosX == posXY[0] && pPosY == posXY[1])
                    {
                        this.GameModel.PlacePiece(this.CurrentPiece, pPosX, pPosY);
                        this.CurrentPiece = null;
                    }
                }
            }
        }

        #endregion
    }
}
