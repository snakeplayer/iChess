/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: MainWindow.xaml.cs
 * File description: The main menu user interface.
 */

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace iChessClient
{
    /// <summary>
    /// The main menu user interface.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MainWindow class.
        /// </summary>
        /// <param name="myConnection">The connection.</param>
        public MainWindow(ClientConnection myConnection)
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
            string username = this.MyConnection.Details.Username;
            int eloRating = this.MyConnection.Details.EloRating;

            this.lblUsername.Content = username;
            this.lblEloRating.Content = string.Format("EloRating : {0}", eloRating);
        }

        #endregion

        #region Methods (Events)

        /// <summary>
        /// Called when imgLogout is clicked and released. Disconnects from server and shows login's window.
        /// </summary>
        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Hide();
            this.Owner.Show();
        }

        /// <summary>
        /// Called when lblUsername is clicked and released. Shows profile window.
        /// </summary>
        private void lblUsername_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow(this.MyConnection);
            profileWindow.Owner = this;
            this.Hide();
            profileWindow.Show();
        }

        /// <summary>
        /// Called when btnCreateRoom is clicked.
        /// </summary>
        private void btnCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomWindow createRoomWindow = new CreateRoomWindow(this.MyConnection);
            createRoomWindow.Owner = this;
            this.Hide();
            createRoomWindow.Show();
        }

        /// <summary>
        /// Called when btnRanking is clicked. Shows ranking window.
        /// </summary>
        private void btnRanking_Click(object sender, RoutedEventArgs e)
        {
            RankingWindow rankingWindow = new RankingWindow(this.MyConnection);
            rankingWindow.Owner = this;
            this.Hide();
            rankingWindow.Show();
        }

        /// <summary>
        /// Called when the window is closing. Disconnects from the server.
        /// </summary>
        private void WindowMainMenu_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
