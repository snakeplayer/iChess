/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: CreateRoomWindow.xaml.cs
 * File description: The create room user interface.
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
    /// The create room user interface.
    /// </summary>
    public partial class CreateRoomWindow : Window
    {
        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of CreateRoomWindow class.
        /// </summary>
        /// <param name="myConnection">The connection.</param>
        public CreateRoomWindow(ClientConnection myConnection)
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

        /// <summary>
        /// Updates the content of lblTimePerPlayer.
        /// </summary>
        public void UpdateTimeLabel()
        {
            try
            {
                if (cbxUnlimitedTime.IsChecked == true)
                {
                    this.lblTimePerPlayer.Content = "Minutes par joueur : ∞";
                    sldGameTime.IsEnabled = false;
                }
                else
                {
                    this.lblTimePerPlayer.Content = string.Format("Minutes par joueur : {0:0}", this.sldGameTime.Value);
                    sldGameTime.IsEnabled = true;
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Methods (Events)

        /// <summary>
        /// Called when the window has loaded.
        /// </summary>
        private void CreateRoom_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateTimeLabel();            
        }

        /// <summary>
        /// Called when imgLogout is clicked and released. Disconnects from server and shows login's window.
        /// </summary>
        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Hide();
            this.Owner.Owner.Show();
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
        /// Called when imgBack is clicked and released.
        /// </summary>
        private void imgBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.Owner.Show();
        }

        /// <summary>
        /// Called when the value of sldGameTime has changed.
        /// </summary>
        private void sldGameTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdateTimeLabel();
        }

        /// <summary>
        /// Called when cbxUnlimitedTime is clicked.
        /// </summary>
        private void cbxUnlimitedTime_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateTimeLabel();
        }

        /// <summary>
        /// Called when btnConfirm is clicked.
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            int roomID = this.MyConnection.CreateRoom(Convert.ToInt32(this.sldGameTime.Value));

            if (roomID <= 0)
            {
                MessageBox.Show("Une erreur s'est produite lors de la création du salon.");
            }
            else
            {
                bool hasJoined = this.MyConnection.JoinRoom(roomID);

                if (hasJoined)
                {
                    JoinedRoomWindow joinedRoomWindow = new JoinedRoomWindow(this.MyConnection, roomID);
                    joinedRoomWindow.Owner = this;
                    this.Hide();
                    joinedRoomWindow.Show();
                    this.MyConnection.RegisterObserver(joinedRoomWindow);
                }
                else
                {
                    MessageBox.Show("Impossible de rejoindre le salon.");
                }
            }
        }

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        private void CreateRoom_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
