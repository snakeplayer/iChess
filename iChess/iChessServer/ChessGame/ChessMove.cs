/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ChessMove.cs
 * File description: Represents a chess move.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Represents a chess move.
    /// </summary>
    public class ChessMove
    {
        #region Consts

        private const bool DEFAULT_IS_ATTACK_MOVE_ONLY = false;
        private const bool DEFAULT_IS_NOT_ATTACK_MOVE = false;

        #endregion

        #region Properties

        public int[] OffsetXY { get; set; }
        public bool CanGoOver { get; set; }
        public bool Repeatable { get; set; }
        public bool IsAttackMoveOnly { get; set; }
        public bool IsNotAttackMove { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructors of ChessMove class.
        /// </summary>
        /// <param name="pOffsetX">The offset in X axis (+ N squares on X).</param>
        /// <param name="pOffsetY">The offset in Y axis (+ N squares on Y).</param>
        /// <param name="pCanGoOver">If the ChessPiece can go over another.</param>
        /// <param name="pRepeatable">If the move is repeatable.</param>
        /// <param name="pIsAttackMoveOnly">If it is an attack move only (move is allowed only if there is an enemy).</param>
        /// <param name="pIsNotAttackMove">If the move is not an attack move (don't allow the move if there's no enemy).</param>
        public ChessMove(int pOffsetX, int pOffsetY, bool pCanGoOver, bool pRepeatable, bool pIsAttackMoveOnly, bool pIsNotAttackMove)
        {
            this.OffsetXY = new int[2];
            this.OffsetXY[0] = pOffsetX;
            this.OffsetXY[1] = pOffsetY;

            this.CanGoOver = pCanGoOver;
            this.Repeatable = pRepeatable;
            this.IsAttackMoveOnly = pIsAttackMoveOnly;
            this.IsNotAttackMove = pIsNotAttackMove;
        }

        public ChessMove(int pOffsetX, int pOffsetY, bool pCanGoOver, bool pRepeatable) : this(pOffsetX, pOffsetY, pCanGoOver, pRepeatable, DEFAULT_IS_ATTACK_MOVE_ONLY, DEFAULT_IS_NOT_ATTACK_MOVE)
        {

        }

        #endregion
    }
}
