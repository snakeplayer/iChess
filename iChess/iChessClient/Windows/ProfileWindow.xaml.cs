/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ProfilWindow.xaml.cs
 * File description: The profil user interface.
 */

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace iChessClient
{
    /// <summary>
    /// The profil user interface.
    /// </summary>
    public partial class ProfileWindow : Window
    {
        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ProfileWindow class.
        /// </summary>
        /// <param name="myConnection">The connection.</param>
        public ProfileWindow(ClientConnection myConnection)
        {
            InitializeComponent();

            this.MyConnection = myConnection;

            this.UpdateView();
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

            // Stats
            this.lblNbWins.Content = string.Format("Victoires : {0}", clientDetails.NumberOfWins.ToString());
            this.lblNbDefeats.Content = string.Format("Défaites : {0}", clientDetails.NumberOfDefeats.ToString());
            this.lblNbTies.Content = string.Format("Égalités : {0}", clientDetails.NumberOfTies.ToString());
            this.lblNbTotal.Content = string.Format("Total : {0}", clientDetails.NumberOfGamesPlayed.ToString());

            // Infos
            this.lblInfoUsername.Content = string.Format("Nom d'utilisateur : {0}", clientDetails.Username);
            this.lblInfoEloRating.Content = string.Format("EloRating : {0}", clientDetails.EloRating.ToString());
            this.lblRegistrationDate.Content = string.Format("Inscrit depuis le {0}", DateTimeOffset.FromUnixTimeSeconds(clientDetails.RegistrationDate).ToString());
        }

        #endregion

        #region Methods (Events)

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
        /// Called when btnModifyProfile is clicked.
        /// </summary>
        private void btnModifyProfile_Click(object sender, RoutedEventArgs e)
        {
            ModifyProfileWindow modifyProfileWindow = new ModifyProfileWindow(this.MyConnection);
            modifyProfileWindow.Owner = this;
            modifyProfileWindow.ShowDialog();
        }

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        private void WindowProfile_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
