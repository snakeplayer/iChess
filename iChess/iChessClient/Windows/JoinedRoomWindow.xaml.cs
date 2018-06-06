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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
        #region Constants

        private const int NUMBER_OF_SQUARES_PER_LINE = 8;
        private const int NUMBER_OF_SQUARES_PER_COLUMN = 8;

        #endregion

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

        #region Properties (Chess board drawing)

        /// <summary>
        /// The Image we draw on.
        /// </summary>
        public System.Drawing.Image Image { get; set; }

        /// <summary>
        /// The Graphics object we will use to draw on the Image.
        /// </summary>
        public Graphics GraphicsFromImage { get; set; }

        /// <summary>
        /// The Brush we will use to draw on the Image.
        /// </summary>
        public System.Drawing.Brush ActualBrush { get; set; }


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

            // Drawing initialization
            this.Image = new Bitmap((int)imgBoard.Width, (int)imgBoard.Height);
            this.GraphicsFromImage = Graphics.FromImage(this.Image);
            this.ActualBrush = null;

            // To do after drawing initialization
            this.UpdateView();
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
                // ----------------------------------- Informations ------------------------------------------------
                this.UpdateRoomInfo();
                this.lblInfoUsername1.Content = this.RoomInfo.HostPlayerName;
                this.lblInfoUsername2.Content = this.RoomInfo.GuestPlayerName;

                this.lblUsernamePlayer1.Content = this.RoomInfo.HostPlayerName;
                this.lblUsernamePlayer2.Content = this.RoomInfo.GuestPlayerName;

                this.lblTimePlayer1.Content = this.RoomInfo.HostPlayerSecondsLeft.ToString();
                this.lblTimePlayer2.Content = this.RoomInfo.GuestPlayerSecondsLeft.ToString();

                this.lbxPiecesOutPlayer1.Items.Clear();
                foreach (string s in this.RoomInfo.HostPlayerPiecesOut)
                {
                    this.lbxPiecesOutPlayer1.Items.Add(s);
                }

                this.lbxPiecesOutPlayer2.Items.Clear();
                foreach (string s in this.RoomInfo.GuestPlayerPiecesOut)
                {
                    this.lbxPiecesOutPlayer2.Items.Add(s);
                }

                this.tbxDiscussion.Text = this.RoomInfo.ChatMessages;
                this.tbxDiscussion.ScrollToEnd();

                rectTurn.Fill = this.RoomInfo.PlayerTurn ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.White;

                // ----------------------------------- Chess board ------------------------------------------------                
                // Sizes and positions
                System.Drawing.Point position = new System.Drawing.Point();
                System.Drawing.Size squareSize = new System.Drawing.Size((int)(imgBoard.Width / NUMBER_OF_SQUARES_PER_LINE), (int)(imgBoard.Height / NUMBER_OF_SQUARES_PER_COLUMN));

                // Create empty board
                bool color = true; // white = true, black = false
                for (int i = 0; i < NUMBER_OF_SQUARES_PER_LINE; i++) // Lines
                {
                    position.X = squareSize.Width * i;

                    for (int j = 0; j < NUMBER_OF_SQUARES_PER_COLUMN; j++) // Columns
                    {
                        position.Y = squareSize.Height * j; // Calculate position
                        this.ActualBrush = color ? System.Drawing.Brushes.LightGray : System.Drawing.Brushes.DarkGreen; // Choosing color
                        this.GraphicsFromImage.FillRectangle(this.ActualBrush, new System.Drawing.Rectangle(position, squareSize)); // Drawing on the image object

                        // Inverting color for the next column
                        color = !color;
                    }

                    // Inverting the color for the next line
                    color = !color;
                }

                // Displaying on the user interface
                var handle = (this.Image as Bitmap).GetHbitmap();
                imgBoard.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
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
            this.UpdateView();
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            this.MyConnection.SendCommand(new ChessCommand(this.RoomID, 42));
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
