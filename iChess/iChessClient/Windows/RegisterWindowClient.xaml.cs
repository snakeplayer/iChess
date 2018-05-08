/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RegisterWindowClient.xaml.cs
 * File description: The registration user interface.
 */

using NetworkCommsDotNet;
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
    /// Logique d'interaction pour RegisterWindow.xaml
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

            // Custom icon
            Uri iconUri = new Uri(@"C:\Users\Administrateur\Documents\T_DIPL\Documentation\Poster\Logo_iChess.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
        }

        #endregion

        #region Methods (Events)

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Hide();
        }
        
        private void WindowRegister_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NetworkComms.Shutdown();
            this.Owner.Close();
        }

        #endregion        
    }
}
