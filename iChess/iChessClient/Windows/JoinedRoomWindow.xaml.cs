/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: JoinedRoomWindow.xaml.cs
 * File description: The joined room user interface.
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
    /// The joined room user interface.
    /// </summary>
    public partial class JoinedRoomWindow : Window, IObserverWindow
    {
        #region Fields

        private RoomInfo _roomInfo;

        #endregion

        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        /// <summary>
        /// Represents the room's ID.
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// Contains room's informations.
        /// </summary>
        public RoomInfo RoomInfo { get => _roomInfo; set => _roomInfo = value; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of JoinedRoomWindow class.
        /// </summary>
        /// <param name="myConnection">The connection.</param>
        /// <param name="roomID">The ID of the joined room.</param>
        public JoinedRoomWindow(ClientConnection myConnection, int roomID)
        {
            InitializeComponent();

            this.MyConnection = myConnection;
            this.RoomID = roomID;

            string username = this.MyConnection.Details.Username;
            int eloRating = this.MyConnection.Details.EloRating;

            this.lblUsername.Content = username;
            this.lblEloRating.Content = string.Format("EloRating : {0}", eloRating);

            this.UpdateRoomInfo();
            //this.UpdateView();
        }

        #endregion

        #region Methods (Updates)

        /// <summary>
        /// Updates the RoomInfo property.
        /// </summary>
        public void UpdateRoomInfo()
        {
            this.RoomInfo = this.MyConnection.GetRoomInfo(this.RoomID);
        }

        /// <summary>
        /// Updates the user interface.
        /// </summary>
        public void UpdateView()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.lblInfoUsername1.Content = this.RoomInfo.HostPlayerName;
                this.lblInfoUsername2.Content = this.RoomInfo.GuestPlayerName;

                this.lblUsernamePlayer1.Content = this.RoomInfo.HostPlayerName;
                this.lblUsernamePlayer2.Content = this.RoomInfo.GuestPlayerName;

                this.lblTimePlayer1.Content = this.RoomInfo.HostPlayerSecondsLeft.ToString();
                this.lblTimePlayer2.Content = this.RoomInfo.GuestPlayerSecondsLeft.ToString();

                foreach (string s in this.RoomInfo.HostPlayerPiecesOut)
                {
                    lbxPiecesOutPlayer1.Items.Add(s);
                }

                foreach (string s in this.RoomInfo.GuestPlayerPiecesOut)
                {
                    lbxPiecesOutPlayer2.Items.Add(s);
                }

                tbxDiscussion.Text = this.RoomInfo.ChatMessages;
            });
        }

        #endregion

        #region Methods (Get informations)

        /// <summary>
        /// Retrieves the room's informations.
        /// </summary>
        /// <param name="roomID">The ID of the room.</param>
        /// <returns>A RoomInfo object reference containing room's informations.</returns>
        public RoomInfo GetRoomInfo()
        {
            return this.MyConnection.GetRoomInfo(this.RoomID);
        }

        #endregion

        #region Methods (Events)

        /// <summary>
        /// Called when imgLogout is clicked and released.
        /// </summary>
        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // TODO : MyConnection.LeaveRoom();
            this.MyConnection.DisconnectFromServer();
            this.Hide();
            this.Owner.Owner.Show();
        }

        /// <summary>
        /// Called when lblUsername is clicked and released.
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.MyConnection.LeaveRoom(this.RoomID);
            this.Owner.Show();
        }

        /// <summary>
        /// Called when btnJoin is clicked.
        /// </summary>
        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        private void JoinedRoom_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.LeaveRoom(this.RoomID);
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
