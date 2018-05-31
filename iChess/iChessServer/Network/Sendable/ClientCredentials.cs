/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ClientCredentials.cs
 * File description: Wrapper to send client's credentials.
 */

using ProtoBuf;

namespace iChessServer
{
    /// <summary>
    /// Wrapper to send client's credentials.
    /// </summary>
    [ProtoContract]
    public class ClientCredentials
    {
        #region Properties

        /// <summary>
        /// The username.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        /// The password.
        /// </summary>
        [ProtoMember(2)]
        public string Password { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor required for protobuf.
        /// </summary>
        public ClientCredentials()
        {

        }

        /// <summary>
        /// Constructor of ClientCredentials class.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public ClientCredentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests if two ClientCredentials are the same.
        /// </summary>
        /// <param name="credentials">The second ClientCredentials object reference.</param>
        /// <returns>True == they are equal, false == they are NOT equal.</returns>
        public bool Equals(ClientCredentials credentials)
        {
            return this.Username == credentials.Username && this.Password == credentials.Password;
        }

        #endregion
    }
}
