/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ClientDetails.cs
 * File description: Wrapper for clients details recovered from the iChess server.
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
    public class ClientDetails
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Username { get; set; }

        [ProtoMember(3)]
        public int RegistrationDate { get; set; }

        [ProtoMember(4)]
        public int NumberOfWins { get; set; }

        [ProtoMember(5)]
        public int NumberOfDefeats { get; set; }

        [ProtoMember(6)]
        public int NumberOfTies { get; set; }

        [ProtoMember(7)]
        public int NumberOfGamesPlayed { get; set; }

        [ProtoMember(8)]
        public int EloRating { get; set; }

        /// <summary>
        /// Parameterless constructor required for protobuf
        /// </summary>
        public ClientDetails()
        {

        }

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
    }
}
