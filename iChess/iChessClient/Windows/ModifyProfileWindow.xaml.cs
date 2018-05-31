/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ModifyProfileWindow.xaml.cs
 * File description: The modify profile user interface.
 */

using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace iChessClient
{
    /// <summary>
    /// The modify profile user interface.
    /// </summary>
    public partial class ModifyProfileWindow : Window
    {
        #region Properties

        /// <summary>
        /// Handles connection with the iChess server.
        /// </summary>
        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ModifyProfileWindow class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ModifyProfileWindow(ClientConnection connection)
        {
            InitializeComponent();

            this.MyConnection = connection;

            this.UpdateView();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the user interface.
        /// </summary>
        public void UpdateView()
        {
            tbxUsername.Text = this.MyConnection.Details.Username;
            tbxPassword.Text = string.Empty;
            tbxPasswordConfirmation.Text = string.Empty;
        }

        #endregion

        #region Methods (Events)

        /// <summary>
        /// Called when btnConfirm is clicked. Ask the server to modify the profile.
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (tbxPassword.Text == tbxPasswordConfirmation.Text)
            {
                int updateResult = this.MyConnection.ModifyClientProfile(tbxUsername.Text, tbxPassword.Text);

                switch (updateResult)
                {
                    case -1:
                        MessageBox.Show("Une erreur est survenue lors de la tentative d'accès au serveur.");
                        break;

                    case 0:
                        MessageBox.Show("Modification effectuée.");
                        this.MyConnection.DisconnectFromServer();
                        this.Hide();
                        this.Owner.Hide();
                        this.Owner.Owner.Hide();
                        this.Owner.Owner.Owner.Show();
                        break;

                    case 1:
                        MessageBox.Show("Le serveur a refusé la modification.");
                        break;

                    default:
                        break;
                }
            }
            else
            {
                MessageBox.Show("Les deux mots de passe doivent être identiques !");
            }
        }

        /// <summary>
        /// Called when btnCancel is clicked.
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        #endregion
    }
}
