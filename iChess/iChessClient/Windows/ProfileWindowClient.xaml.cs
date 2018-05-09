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

        }

        #endregion

        #region Methods (Events)

        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Hide();
            this.Owner.Show();
        }

        private void WindowProfile_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
