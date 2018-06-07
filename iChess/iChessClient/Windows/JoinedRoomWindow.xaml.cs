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
using System.Runtime.InteropServices;
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
        private const int OFFSET_IN_SQUARE = 3;

        public const string WHITE_PAWN_NAME = "Dawn_White";
        public const string WHITE_ROOK_NAME = "Rook_White";
        public const string WHITE_KNIGHT_LEFT_NAME = "Knight_Left_White";
        public const string WHITE_KNIGHT_RIGHT_NAME = "Knight_Right_White";
        public const string WHITE_BISHOP_NAME = "Bishop_White";
        public const string WHITE_QUEEN_NAME = "Queen_White";
        public const string WHITE_KING_NAME = "King_White";

        public const string BLACK_PAWN_NAME = "Dawn_Black";
        public const string BLACK_ROOK_NAME = "Rook_Black";
        public const string BLACK_KNIGHT_LEFT_NAME = "Knight_Left_Black";
        public const string BLACK_KNIGHT_RIGHT_NAME = "Knight_Right_Black";
        public const string BLACK_BISHOP_NAME = "Bishop_Black";
        public const string BLACK_QUEEN_NAME = "Queen_Black";
        public const string BLACK_KING_NAME = "King_Black";

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
                for (int x = 0; x < NUMBER_OF_SQUARES_PER_LINE; x++) // Lines
                {
                    position.X = squareSize.Width * x;

                    for (int y = 0; y < NUMBER_OF_SQUARES_PER_COLUMN; y++) // Columns
                    {
                        // Drawing sqaure
                        position.Y = squareSize.Height * y; // Calculate position
                        this.ActualBrush = color ? System.Drawing.Brushes.LightGray : System.Drawing.Brushes.DarkGreen; // Choosing color
                        this.GraphicsFromImage.FillRectangle(this.ActualBrush, new System.Drawing.Rectangle(position, squareSize)); // Drawing on the image object

                        // Inverting color for the next column
                        color = !color;
                    }

                    // Inverting the color for the next line
                    color = !color;
                }

                // Add ChessPieces, AttackMoves and PossibleMoves
                int posX;
                int posY;
                for (int x = 0; x < 64; x++)
                {
                    posX = x % 8;
                    posY = x / 8;
                    position.X = ((squareSize.Width * posX) + OFFSET_IN_SQUARE);
                    position.Y = (Image.Size.Height + OFFSET_IN_SQUARE) - ((squareSize.Height * posY) + OFFSET_IN_SQUARE) - squareSize.Height + OFFSET_IN_SQUARE;

                    // Drawing ChessPieces, AttackMoves and PossibleMoves
                    string chessPiece = this.RoomInfo.ChessBoard.ChessSquares[x].ChessPiece; // TODO : tell don't ask
                    bool isAttack = this.RoomInfo.ChessBoard.ChessSquares[x].IsAttack;
                    bool isPossibleMove = this.RoomInfo.ChessBoard.ChessSquares[x].IsPossibleMove;

                    // ChessPiece
                    if (chessPiece != string.Empty) // ChessPiece
                    {
                        switch (chessPiece)
                        {
                            case WHITE_PAWN_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Pawn_White, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case WHITE_ROOK_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Rook_White, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case WHITE_KNIGHT_LEFT_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Knight_Left_White, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case WHITE_KNIGHT_RIGHT_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Knight_Right_White, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case WHITE_BISHOP_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Bishop_White, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case WHITE_QUEEN_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Queen_White, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case WHITE_KING_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.King_White, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case BLACK_PAWN_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Pawn_Black, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case BLACK_ROOK_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Rook_Black, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case BLACK_KNIGHT_LEFT_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Knight_Left_Black, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case BLACK_KNIGHT_RIGHT_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Knight_Right_Black, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case BLACK_BISHOP_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Bishop_Black, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case BLACK_QUEEN_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.Queen_Black, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            case BLACK_KING_NAME:
                                this.GraphicsFromImage.DrawImage(Properties.Resources.King_Black, position.X, position.Y, squareSize.Width - OFFSET_IN_SQUARE * 2, squareSize.Height - OFFSET_IN_SQUARE * 2);
                                break;

                            default:
                                break;
                        }
                    }

                    // PossibleMove
                    if (isPossibleMove)
                    {
                        this.GraphicsFromImage.DrawRectangle(Pens.Yellow, new System.Drawing.Rectangle(position, squareSize));
                    }

                    // AttackMove
                    if (isAttack)
                    {
                        this.GraphicsFromImage.DrawRectangle(Pens.Red, new System.Drawing.Rectangle(position, squareSize));
                    }
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

        /// <summary>
        /// Get a XY position from a Point.
        /// </summary>
        /// <param name="pPosition">The point.</param>
        /// <returns>A int[] with the position XY.</returns>
        public int[] GetPositionFromPoint(System.Windows.Point pPosition)
        {
            System.Drawing.Size squareSize = new System.Drawing.Size((int)(imgBoard.Width / NUMBER_OF_SQUARES_PER_LINE), (int)(imgBoard.Height / NUMBER_OF_SQUARES_PER_COLUMN));
            int[] posXY = new int[2];
            posXY[0] = (int)(pPosition.X / squareSize.Width);
            posXY[1] = (NUMBER_OF_SQUARES_PER_COLUMN - 1) - (int)(pPosition.Y / squareSize.Height);
            return posXY;
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
            this.MyConnection.SendCommand(new ChessCommand(this.RoomID, new int[2] { 0, 2 }));
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            this.MyConnection.SendCommand(new ChessCommand(this.RoomID, new int[2] { 0, 1 }));
        }

        private void imgBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mousePos = e.GetPosition(imgBoard);
            int[] posXY = this.GetPositionFromPoint(mousePos);
            this.MyConnection.SendCommand(new ChessCommand(this.RoomID, new int[2] { posXY[0], posXY[1] }));
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
