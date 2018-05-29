/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: AllClientDetails.cs
 * File description: Wrapper for all client's details recovered from the iChess server.
 */

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    [ProtoContract]
    public class AllClientsDetails
    {
        [ProtoMember(1)]
        public List<ClientDetails> ClientList { get; set; }

        public AllClientsDetails()
        {
            this.ClientList = new List<ClientDetails>();
        }

        public AllClientsDetails(List<ClientDetails> clientList)
        {
            this.ClientList = clientList;
        }
    }
}
