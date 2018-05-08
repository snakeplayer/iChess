/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: MainMenuClient.xaml.cs
 * File description: The main menu user interface.
 */

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
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
    /// Logique d'interaction pour MainMenuClient.xaml
    /// </summary>
    public partial class MainMenuClient : Window
    {
        #region Properties

        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MainMenuClient.
        /// </summary>
        /// <param name="myConnection">The connection.</param>
        public MainMenuClient(ClientConnection myConnection)
        {
            InitializeComponent();

            // Custom icon
            Uri iconUri = new Uri(@"C:\Users\Administrateur\Documents\T_DIPL\Documentation\Poster\Logo_iChess.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            this.MyConnection = myConnection;
        }

        #endregion

        #region Methods (Events)

        private void WindowMainMenu_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NetworkComms.Shutdown();
            this.Owner.Close();
        }

        #endregion
    }
}
