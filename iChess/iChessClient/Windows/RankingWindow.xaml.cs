/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RankingWindow.xaml.cs
 * File description: The ranking user interface.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace iChessClient
{
    /// <summary>
    /// The ranking user interface.
    /// </summary>
    public partial class RankingWindow : Window
    {
        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of RankingWindow class.
        /// </summary>
        /// <param name="myConnection">The connection.</param>
        public RankingWindow(ClientConnection myConnection)
        {
            InitializeComponent();

            this.MyConnection = myConnection;

            this.UpdateView();

            //DEBUG
            /*
            lstRanking.Add(new RankingItem { Rank = "1", Player = "David", EloRating = "2340", WLRatio = "1.54" });
            lstRanking.Add(new RankingItem { Rank = "2", Player = "Jean", EloRating = "1210", WLRatio = "2.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            */
            
            List<RankingItem> lstRanking = new List<RankingItem>();
            AllClientsDetails acd = this.MyConnection.GetAllClientsDetails();

            //acd.ClientList.Sort();
            int i = 1;
            acd.ClientList.ToList().ForEach(c => lstRanking.Add(new RankingItem { Rank = (i++).ToString(), Player = c.Username, EloRating = c.EloRating.ToString(), WLRatio = c.NumberOfDefeats > 0 ? (Math.Round(Convert.ToDouble(c.NumberOfWins) / Convert.ToDouble(c.NumberOfDefeats), 2)).ToString() : "1" }));

            lstRanking.ToList().ForEach(line => lwRanking.Items.Add(line));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the user interface.
        /// </summary>
        public void UpdateView()
        {
            // Recovering client details
            ClientDetails clientDetails = this.MyConnection.GetMyDetails();

            // Header
            this.lblUsername.Content = clientDetails.Username;
            this.lblEloRating.Content = string.Format("EloRating : {0}", clientDetails.EloRating);
        }

        #endregion

        #region Methods (Events)

        /// <summary>
        /// Called when lblUsername is pressed and released.
        /// </summary>
        private void lblUsername_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow(this.MyConnection);
            profileWindow.Owner = this;
            this.Hide();
            profileWindow.Show();
        }

        /// <summary>
        /// Called when imgLogout is pressed and released.
        /// </summary>
        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Hide();
            this.Owner.Owner.Show();
        }

        /// <summary>
        /// Called when imgBack is pressed and released.
        /// </summary>
        private void imgBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.Owner.Show();
        }

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        private void WindowRanking_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
