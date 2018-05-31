/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: LoginWindow.xaml.cs
 * File description: The login user interface.
 */

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace iChessClient
{
    /// <summary>
    /// The login user interface.
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of LoginWindow class.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
            
            this.MyConnection = new ClientConnection();
        }

        #endregion
        
        #region Methods (Events)

        /// <summary>
        /// Called when btnConfirm is clicked. Tries to connect to an iChess server.
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            int connectionState = this.MyConnection.ConnectToServer(tbxIpAddress.Text, Convert.ToInt32(tbxPort.Text), tbxUsername.Text, tbxPassword.Text);

            switch (connectionState)
            {
                case -1:
                    MessageBox.Show("Une erreur s'est produite lors de la tentative de connexion au server.");
                    break;

                case 0:
                    MainWindow mainMenu = new MainWindow(this.MyConnection);
                    mainMenu.Owner = this;
                    this.Hide();
                    mainMenu.Show(); // TODO : Function to open child window!
                    break;

                case 1:
                    MessageBox.Show("Le serveur a refusé la connexion.");
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Called when btnRegistration is clicked. Tries a registration on an iChess server.
        /// </summary>
        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            Window registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            this.Hide();
            registerWindow.Show();
        }

        /// <summary>
        /// Called when btnRegistration is clicked. Quits the application.
        /// </summary>
        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
