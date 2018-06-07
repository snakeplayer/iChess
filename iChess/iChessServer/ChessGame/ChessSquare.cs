/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessSquare.cs
 * File description: Represent a chess square where ChessPieces are placed.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Represent a chess square where ChessPieces are placed.
    /// </summary>
    public class ChessSquare : Object
    {
        #region Properties

        public bool IsWhite { get; set; }
        public AbstractChessPiece ChessPiece { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ChessSquare class.
        /// </summary>
        /// <param name="pIsWhite">If the ChessSquare is white.</param>
        public ChessSquare(bool pIsWhite)
        {
            this.IsWhite = pIsWhite;
            this.ChessPiece = null;
        }

        #endregion
    }
}
