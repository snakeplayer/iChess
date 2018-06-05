/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RoomsWindow.xaml.cs
 * File description: The show rooms user interface.
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
    /// The show rooms user interface.
    /// </summary>
    public partial class RoomsWindow : Window
    {
        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of RoomWindow class.
        /// </summary>
        /// <param name="myConnection">The connection./param>
        public RoomsWindow(ClientConnection myConnection)
        {
            InitializeComponent();

            this.MyConnection = myConnection;

            RoomItemList itemList = this.MyConnection.GetRoomList();

            foreach (RoomItem item in itemList.List)
            {
                lwRooms.Items.Add(item);
            }

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
        /// Called when imgLogout is clicked and released.
        /// </summary>
        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
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
        private void imgBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.Owner.Show();
        }

        /// <summary>
        /// Called when btnJoin is clicked.
        /// </summary>
        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            if (lwRooms.SelectedIndex > -1)
            {
                int roomID = Convert.ToInt32((lwRooms.SelectedItems[0] as RoomItem).RoomID);
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
                    // TODO : HANDLE ERRORS
                }
            }
        }

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        private void Rooms_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
