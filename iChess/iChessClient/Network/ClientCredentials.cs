/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ClientCredentials.cs
 * File description: Wrapper to send client's credentials to the server.
 */

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessClient
{
    [ProtoContract]
    public class ClientCredentials
    {
        #region Properties

        [ProtoMember(1)]
        public string Username { get; set; }

        [ProtoMember(2)]
        public string Password { get; set; }

        #endregion

        #region Constructors

        public ClientCredentials()
        {

        }

        public ClientCredentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        #endregion

        #region Methods



        #endregion
    }
}
