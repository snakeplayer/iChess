/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RankingItem.cs
 * File description: Used to display ranking items.
 */

namespace iChessClient
{
    /// <summary>
    /// Used to display ranking items.
    /// </summary>
    public class RankingItem
    {
        /// <summary>
        /// The rank of the player.
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// The username of the player.
        /// </summary>
        public string Player { get; set; }

        /// <summary>
        /// The EloRating score of the player.
        /// </summary>
        public string  EloRating { get; set; }

        /// <summary>
        /// The win/loose ratio of the player.
        /// </summary>
        public string WLRatio { get; set; }
    }
}
