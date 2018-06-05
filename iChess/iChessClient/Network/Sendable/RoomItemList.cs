/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RoomItemList.cs
 * File description: List used to display rooms.
 */

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessClient
{
    /// <summary>
    /// List used to display rooms.
    /// </summary>
    [ProtoContract]
    public class RoomItemList
    {
        /// <summary>
        /// The list of RoomItem.
        /// </summary>
        [ProtoMember(1)]
        public List<RoomItem> List { get; set; }

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public RoomItemList()
        {
            this.List = new List<RoomItem>();
        }
    }
}
