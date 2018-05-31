/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: AllClientDetails.cs
 * File description: Wrapper for all client's details recovered from the iChess server.
 */

using ProtoBuf;
using System.Collections.Generic;

namespace iChessClient
{
    /// <summary>
    /// Wrapper for all client's details recovered from the iChess server.
    /// </summary>
    [ProtoContract]
    public class AllClientsDetails
    {
        #region Properties

        /// <summary>
        /// The list containing every client's ClientDetails.
        /// </summary>
        [ProtoMember(1)]
        public List<ClientDetails> ClientList { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AllClientsDetails class.
        /// </summary>
        public AllClientsDetails()
        {
            this.ClientList = new List<ClientDetails>();
        }

        #endregion
    }
}
