﻿/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: ServerDatabase.cs
 * File description: Handle connections with the SQLite database.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace iChessServer
{
    /// <summary>
    /// Handle connections with the SQLite database.
    /// </summary>
    public static class ServerDatabase
    {
        #region Constants

        private static string DEFAULT_DB_PATH = "ServerDB.db";
        private static string DEFAULT_DB_VERSION = "3";

        #endregion

        #region Methods (Database)

        /// <summary>
        /// Add a client to the database.
        /// </summary>
        /// <param name="username">The username of the client.</param>
        /// <param name="password">The password of the client.</param>
        /// <returns>true if the registration is successful, false if not.</returns>
        public static bool RegisterClient(string username, string password)
        {
            bool insertSuccess = false;

            // Connection
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version={1};", DEFAULT_DB_PATH, DEFAULT_DB_VERSION));
            connection.Open();

            // Querry
            string sql = string.Format("INSERT INTO Clients(username, password, registrationDate) VALUES('{0}','{1}', '{2}')", username, password, DateTimeOffset.Now.ToUnixTimeSeconds());
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            try
            {
                command.ExecuteNonQuery();
                insertSuccess = true;
            }
            catch (Exception)
            {
                insertSuccess = false;
            }

            connection.Close();

            return insertSuccess;
        }

        /// <summary>
        /// Retrieves clients from the database.
        /// </summary>
        /// <returns>A list of string containing all client's name.</returns>
        public static Dictionary<string, string> GetClientsFromDB()
        {
            Dictionary<string, string> clientsFromDB = new Dictionary<string, string>();

            // Connection
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version={1};", DEFAULT_DB_PATH, DEFAULT_DB_VERSION));
            connection.Open();

            // Querry
            string sql = "select * from Clients";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                clientsFromDB.Add(reader["username"].ToString(), reader["password"].ToString());
            }

            connection.Close();

            return clientsFromDB;
        }

        public static ClientDetails GetClientDetails(string username)
        {
            ClientDetails clientDetails = new ClientDetails();

            // Connection
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version={1};", DEFAULT_DB_PATH, DEFAULT_DB_VERSION));
            connection.Open();

            // Querry
            string sql = string.Format("select * from Clients where username ='{0}'", username);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
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

            connection.Close();

            return clientDetails;
        }

        public static AllClientsDetails GetAllClientsDetails()
        {
            AllClientsDetails allClientsDetails = new AllClientsDetails();

            // Connection
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version={1};", DEFAULT_DB_PATH, DEFAULT_DB_VERSION));
            connection.Open();

            // Querry
            string sql = string.Format("select * from Clients order by eloRating desc");
            SQLiteCommand command = new SQLiteCommand(sql, connection);
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

            connection.Close();

            return allClientsDetails;
        }

        public static bool ModifyClientProfile(string username, ClientCredentials clientCredentials)
        {
            bool modifySuccess = false;

            // Connection
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version={1};", DEFAULT_DB_PATH, DEFAULT_DB_VERSION));
            connection.Open();

            // Querry
            string sql = string.Format("UPDATE Clients SET username='{0}', password='{1}' WHERE username='{2}';", clientCredentials.Username, clientCredentials.Password, username);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            try
            {
                command.ExecuteNonQuery();
                modifySuccess = true;
            }
            catch (Exception)
            {
                modifySuccess = false;
            }

            connection.Close();

            return modifySuccess;
        }

        #endregion
    }
}
