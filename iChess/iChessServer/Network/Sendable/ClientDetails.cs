/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ClientDetails.cs
 * File description: Wrapper for clients details recovered from the iChess server.
 */

using ProtoBuf;

namespace iChessServer
{
    /// <summary>
    /// Wrapper for clients details recovered from the iChess server.
    /// </summary>
    [ProtoContract]
    public class ClientDetails
    {
        #region Properties

        /// <summary>
        /// The ID.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        [ProtoMember(2)]
        public string Username { get; set; }

        /// <summary>
        /// The registration date, stored as Unix time.
        /// </summary>
        [ProtoMember(3)]
        public int RegistrationDate { get; set; }

        /// <summary>
        /// The number of wins.
        /// </summary>
        [ProtoMember(4)]
        public int NumberOfWins { get; set; }

        /// <summary>
        /// The number of defeats.
        /// </summary>
        [ProtoMember(5)]
        public int NumberOfDefeats { get; set; }

        /// <summary>
        /// The number of ties.
        /// </summary>
        [ProtoMember(6)]
        public int NumberOfTies { get; set; }

        /// <summary>
        /// The number of games played.
        /// </summary>
        [ProtoMember(7)]
        public int NumberOfGamesPlayed { get; set; }

        /// <summary>
        /// The EloRating.
        /// </summary>
        [ProtoMember(8)]
        public int EloRating { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor required for protobuf
        /// </summary>
        public ClientDetails()
        {

        }

        /// <summary>
        /// Constructor of ClientDetails class.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="registrationDate">The registration date.</param>
        /// <param name="numberOfWins">The number of wins.</param>
        /// <param name="numberOfDefeats">The number of defeats.</param>
        /// <param name="numberOfTies">The number of ties.</param>
        /// <param name="eloRating">The EloRating.</param>
        public ClientDetails(int id, string username, int registrationDate, int numberOfWins, int numberOfDefeats, int numberOfTies, int eloRating)
        {
            this.Username = username;
            this.RegistrationDate = registrationDate;
            this.NumberOfWins = numberOfWins;
            this.NumberOfDefeats = numberOfDefeats;
            this.NumberOfTies = numberOfTies;
            this.NumberOfGamesPlayed = this.NumberOfWins + this.NumberOfDefeats + this.NumberOfTies;
            this.EloRating = eloRating;
        }

        #endregion
    }
}
