/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RankingItem.cs
 * File description: Used to display ranking items.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessClient
{
    public class RankingItem
    {
        public string Rank { get; set; }
        public string Player { get; set; }
        public string  EloRating { get; set; }
        public string WLRatio { get; set; }
    }
}
