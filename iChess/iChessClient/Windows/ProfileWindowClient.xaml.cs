/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ProfilWindowClient.xaml.cs
 * File description: The profil user interface.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iChessClient
{
    /// <summary>
    /// Logique d'interaction pour ProfileWindowClient.xaml
    /// </summary>
    public partial class ProfileWindowClient : Window
    {
        #region Properties

        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MainMenuClient.
        /// </summary>
        /// <param name="myConnection">The connection.</param>
        public ProfileWindowClient(ClientConnection myConnection)
        {
            InitializeComponent();

            // Custom icon
            Uri iconUri = new Uri(@"C:\Users\Administrateur\Documents\T_DIPL\Documentation\Poster\Logo_iChess.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            this.MyConnection = myConnection;

            this.UpdateView();
        }

        #endregion

        #region Methods

        public void UpdateView()
        {
            // Recovering client details
            ClientDetails clientDetails = this.MyConnection.GetMyDetails();

            // Header
            this.lblUsername.Content = clientDetails.Username;
            this.lblEloRating.Content = clientDetails.EloRating.ToString();

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

        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Hide();
            this.Owner.Owner.Show();
        }

        private void imgBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.Owner.Show();
        }

        private void btnModifyProfile_Click(object sender, RoutedEventArgs e)
        {
            ModifyProfileWindowClient modifyProfileWindowClient = new ModifyProfileWindowClient(this.MyConnection);
            modifyProfileWindowClient.Owner = this;
            modifyProfileWindowClient.ShowDialog();
        }

        private void WindowProfile_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
