/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: AuthenticatedClients.cs
 * File description: Handles all authenticated clients on the server.
 */

using NetworkCommsDotNet.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    /// <summary>
    /// Handles all authenticated clients on the server.
    /// </summary>
    public class AuthenticatedClients
    {
        #region Properties

        /// <summary>
        /// Represents all authenticated clients on the server.
        /// </summary>
        private List<AuthenticatedClient> ClientList { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AuthenticatedClients class.
        /// </summary>
        public AuthenticatedClients()
        {
            this.ClientList = new List<AuthenticatedClient>();
        }

        #endregion

        #region Methods (Add & remove)

        /// <summary>
        /// Adds a client to the list.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="username">The username.</param>
        public void AddClient(Connection connection, string username)
        {
            lock (this.ClientList)
            {
                this.ClientList.Add(new AuthenticatedClient(connection, username));
            }
        }

        /// <summary>
        /// Removes a client from the list.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void RemoveClient(Connection connection)
        {
            lock (this.ClientList)
            {
                List<AuthenticatedClient> newClientList = new List<AuthenticatedClient>();

                foreach (AuthenticatedClient c in this.ClientList)
                {
                    if (c.Connection != connection)
                    {
                        newClientList.Add(c);
                    }
                }

                this.ClientList = newClientList;
            }
        }

        /// <summary>
        /// Removes a client from the list.
        /// </summary>
        /// <param name="username">The username.</param>
        public void RemoveClient(string username)
        {
            lock (this.ClientList)
            {
                List<AuthenticatedClient> newClientList = new List<AuthenticatedClient>();

                foreach (AuthenticatedClient c in this.ClientList)
                {
                    if (c.Username != username)
                    {
                        newClientList.Add(c);
                    }
                }

                this.ClientList = newClientList;
            }
        }

        /// <summary>
        /// Clears the client list.
        /// </summary>
        public void Clear()
        {
            lock (this.ClientList)
            {
                this.ClientList = new List<AuthenticatedClient>();
            }
        }

        #endregion

        #region Methods (Tests)

        /// <summary>
        /// Checks if the connection is already in the client list.
        /// </summary>
        /// <param name="connection">The connection./param>
        /// <returns>True == the connection is contained in the client list, false == the connection is NOT contained in the client list.</returns>
        public bool ContainsConnection(Connection connection)
        {
            bool isContained = false;

            lock (this.ClientList)
            {
                foreach (AuthenticatedClient c in this.ClientList)
                {
                    if (c.Connection == connection)
                    {
                        isContained = true;
                    }
                }
            }
            
            return isContained;
        }

        /// <summary>
        /// Checks if the username is already in the client list.
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>True == the username is contained in the client list, false == the username is NOT contained in the client list.</returns>
        public bool ContainsUsername(string username)
        {
            bool isContained = false;

            lock (this.ClientList)
            {
                foreach (AuthenticatedClient c in this.ClientList)
                {
                    if (c.Username == username)
                    {
                        isContained = true;
                    }
                }
            }

            return isContained;
        }

        #endregion

        #region Methods (Get)

        /// <summary>
        /// Get the username of a given connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The username of a given connection.</returns>
        public string GetUsername(Connection connection)
        {
            string username = string.Empty;

            foreach (AuthenticatedClient c in this.ClientList)
            {
                if (c.Connection == connection)
                {
                    username = c.Username;
                }
            }

            return username;
        }

        /// <summary>
        /// Get the list of usernames.
        /// </summary>
        /// <returns>A list of string containg usernames.</returns>
        public List<string> GetUsernameList()
        {
            List<string> usernameList = new List<string>();

            lock (this.ClientList)
            {
                this.ClientList.ToList().ForEach(c => usernameList.Add(c.Username));
            }

            return usernameList;
        }

        /// <summary>
        /// Get the AuthenticatedClient object reference of a given connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The AuthenticatedClient object reference.</returns>
        public AuthenticatedClient GetAuthenticatedClientFromConnection(Connection connection)
        {
            AuthenticatedClient authClient = new AuthenticatedClient();

            foreach (AuthenticatedClient cli in this.ClientList)
            {
                if (cli.Connection == connection)
                {
                    authClient = cli;
                }
            }

            return authClient;
        }

        #endregion
    }
}
