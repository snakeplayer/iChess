/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RegisterWindow.xaml.cs
 * File description: The registration user interface.
 */

using NetworkCommsDotNet;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace iChessClient
{
    /// <summary>
    /// The registration user interface.
    /// </summary>
    public partial class RegisterWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor of RegisterWindow class.
        /// </summary>
        public RegisterWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods (Events)

        /// <summary>
        /// Called when btnConfirm is clicked.
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (tbxPassword.Text == tbxPasswordConfirmation.Text)
            {
                int registrationState = ClientConnection.RegisterToServer(tbxIpAddress.Text, Convert.ToInt32(tbxPort.Text), tbxUsername.Text, tbxPassword.Text);
                switch (registrationState)
                {
                    case -1:
                        MessageBox.Show("Le serveur ne répond pas.");
                        break;

                    case 0:
                        MessageBox.Show("Inscription effectuée, vous pouvez maintenant vous connecter !");
                        this.Owner.Show();
                        this.Hide();
                        break;

                    case 1:
                        MessageBox.Show("Le serveur a refusé votre inscription.");
                        break;

                    default:
                        break;
                }
            }
            else
            {
                MessageBox.Show("Les mots de passe doivent être identiques !");
            }
        }

        /// <summary>
        /// Called when btnCancel is clicked.
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Hide();
        }
        
        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        private void WindowRegister_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NetworkComms.Shutdown();
            this.Owner.Close();
        }

        #endregion        
    }
}
