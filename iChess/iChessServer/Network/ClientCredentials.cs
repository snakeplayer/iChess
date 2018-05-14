﻿/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ClientCredentials.cs
 * File description: Wrapper to recieve client's credentials.
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
