/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ServerDatabase.cs
 * File description: Handle connections with the SQLite database.
 */

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace iChessServer
{
    /// <summary>
    /// Handle connections with the SQLite database.
    /// </summary>
    public class ServerDatabase
    {
        #region Constants

        private string DEFAULT_DB_PATH = "ServerDB.db";
        private string DEFAULT_DB_VERSION = "3";

        private int DEFAULT_USERNAME_MIN_LENGTH = 3;
        private int DEFAULT_USERNAME_MAX_LENGTH = 10;
        private int DEFAULT_PASSWORD_MIN_LENGTH = 3;
        private int DEFAULT_PASSWORD_MAX_LENGTH = 20;

        #endregion

        #region Properties

        /// <summary>
        /// The connection to the SQLite database.
        /// </summary>
        public SQLiteConnection DBConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ServerDatabase class.
        /// </summary>
        public ServerDatabase()
        {

        }

        #endregion

        #region Methods (Open & close)

        /// <summary>
        /// Open the connection with an SQLite database.
        /// </summary>
        /// <returns>True == connection is open, false == an error occured.</returns>
        public bool OpenConnection()
        {
            bool connectionResult = false;

            try
            {
                // Connection
                this.DBConnection = new SQLiteConnection(string.Format("Data Source={0};Version={1};", DEFAULT_DB_PATH, DEFAULT_DB_VERSION));
                this.DBConnection.Open();
                connectionResult = true;
            }
            catch (Exception)
            {
                connectionResult = false;
            }

            return connectionResult;
        }

        /// <summary>
        /// Closes the connection with the SQLite database.
        /// </summary>
        public void CloseConnection()
        {
            this.DBConnection?.Close();
        }

        #endregion

        #region Methods (Utilities)

        /// <summary>
        /// Checks if credentials are complying to the server's expectations..
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>True == credentials are complying, false == credentials are NOT complying.</returns>
        private bool CheckCredentialsCompliance(ClientCredentials credentials)
        {
            bool result = false;
            var regexSpecialChars = new Regex("^[a-zA-Z0-9 ]*$"); // Regex for special chars excluding

            // Username check
            if (credentials.Username.Length >= DEFAULT_USERNAME_MIN_LENGTH && credentials.Username.Length <= DEFAULT_USERNAME_MAX_LENGTH && regexSpecialChars.IsMatch(credentials.Username))
            {
                // Password check
                if (credentials.Password.Length >= DEFAULT_PASSWORD_MIN_LENGTH && credentials.Password.Length <= DEFAULT_PASSWORD_MAX_LENGTH)
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Methods (Get)

        /// <summary>
        /// Retrieves all ClientCredentials from the database.
        /// </summary>
        /// <returns>A list of ClientCredentials with every client's usernames and passwords.</returns>
        public List<ClientCredentials> GetClientsCredentials()
        {
            List<ClientCredentials> clientsFromDB = new List<ClientCredentials>();

            try
            {
                // Querry
                string sql = "select * from Clients";
                SQLiteCommand command = new SQLiteCommand(sql, this.DBConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //clientsFromDB.Add(reader["username"].ToString(), reader["password"].ToString()); // TODO : del this line if ok
                    clientsFromDB.Add(new ClientCredentials(reader["username"].ToString(), reader["password"].ToString()));
                }
            }
            catch (Exception)
            {
                throw new Exception("An error occured. Make sure the connection is open.");
            }

            return clientsFromDB;
        }

        /// <summary>
        /// Retrieves ClientDetails from database.
        /// </summary>
        /// <param name="username">The username of the client.</param>
        /// <returns>The ClientDetails of the client.</returns>
        public ClientDetails GetClientDetails(string username)
        {
            ClientDetails clientDetails = new ClientDetails();

            // Querry
            string sql = string.Format("select * from Clients where username ='{0}'", username);
            SQLiteCommand command = new SQLiteCommand(sql, this.DBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int idClient = Convert.ToInt32(reader["idClient"]);
                int registrationDate = Convert.ToInt32(reader["registrationDate"]);
                int nbWins = Convert.ToInt32(reader["numberOfWins"]);
                int nbDefeats = Convert.ToInt32(reader["numberOfDefeats"]);
                int nbTies = Convert.ToInt32(reader["numberOfTies"]);
                int nbTotal = nbWins + nbDefeats + nbTies;
                int eloRating = Convert.ToInt32(reader["eloRating"]);
                clientDetails = new ClientDetails(idClient, username, registrationDate, nbWins, nbDefeats, nbTies, eloRating);
            }

            return clientDetails;
        }

        /// <summary>
        /// Retrieves AllClientsDetails from database.
        /// </summary>
        /// <returns>An instance of AllClientsDetails containing all clients details.</returns>
        public AllClientsDetails GetAllClientsDetails()
        {
            AllClientsDetails allClientsDetails = new AllClientsDetails();

            // Querry
            string sql = string.Format("select * from Clients order by eloRating desc");
            SQLiteCommand command = new SQLiteCommand(sql, this.DBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int idClient = Convert.ToInt32(reader["idClient"]);
                string username = reader["username"].ToString();
                int registrationDate = Convert.ToInt32(reader["registrationDate"]);
                int nbWins = Convert.ToInt32(reader["numberOfWins"]);
                int nbDefeats = Convert.ToInt32(reader["numberOfDefeats"]);
                int nbTies = Convert.ToInt32(reader["numberOfTies"]);
                int nbTotal = nbWins + nbDefeats + nbTies;
                int eloRating = Convert.ToInt32(reader["eloRating"]);
                allClientsDetails.ClientList.Add(new ClientDetails(idClient, username, registrationDate, nbWins, nbDefeats, nbTies, eloRating));
            }

            return allClientsDetails;
        }

        #endregion

        #region Methods (Set)

        /// <summary>
        /// Adds a client to the database.
        /// </summary>
        /// <param name="username">The username of the client.</param>
        /// <param name="password">The password of the client.</param>
        /// <returns>true if the registration is successful, false if not.</returns>
        public bool RegisterClient(ClientCredentials credentials)
        {
            bool insertSuccess = false;
            var regexSpecialChars = new Regex("^[a-zA-Z0-9 ]*$"); // Regex for special chars excluding

            if (this.CheckCredentialsCompliance(credentials))
            {
                // Querry
                string sql = string.Format("INSERT INTO Clients(username, password, registrationDate) VALUES('{0}','{1}', '{2}')", credentials.Username, credentials.Password, DateTimeOffset.Now.ToUnixTimeSeconds());
                SQLiteCommand command = new SQLiteCommand(sql, this.DBConnection);
                try
                {
                    command.ExecuteNonQuery();
                    insertSuccess = true;
                }
                catch (Exception)
                {
                    insertSuccess = false;
                }
            }

            return insertSuccess;
        }

        /// <summary>
        /// Modify client's credentials.
        /// </summary>
        /// <param name="username">The username of the client which will be changed.</param>
        /// <param name="clientCredentials">The new credentials of the client.</param>
        /// <returns>True == credentials have been modified, false == credentials have NOT been modified.</returns>
        public bool ModifyClientCredentials(string username, ClientCredentials credentials) // ++ use credentials for both !
        {
            bool modifySuccess = false;

            if (this.CheckCredentialsCompliance(credentials))
            {
                // Querry
                string sql = string.Format("UPDATE Clients SET username='{0}', password='{1}' WHERE username='{2}';", credentials.Username, credentials.Password, username);
                SQLiteCommand command = new SQLiteCommand(sql, this.DBConnection);
                try
                {
                    command.ExecuteNonQuery();
                    modifySuccess = true;
                }
                catch (Exception)
                {
                    modifySuccess = false;
                }
            }

            return modifySuccess;
        }

        #endregion
    }
}
