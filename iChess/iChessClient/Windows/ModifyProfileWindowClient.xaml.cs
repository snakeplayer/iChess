/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: ModifyProfileWindowClient.xaml.cs
 * File description: The modify profile user interface.
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
    /// Logique d'interaction pour ModifyProfileWindowClient.xaml
    /// </summary>
    public partial class ModifyProfileWindowClient : Window
    {
        #region Properties

        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        public ModifyProfileWindowClient(ClientConnection connection)
        {
            InitializeComponent();

            // Custom icon
            Uri iconUri = new Uri(@"C:\Users\Administrateur\Documents\T_DIPL\Documentation\Poster\Logo_iChess.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            this.MyConnection = connection;

            this.UpdateView();
        }

        #endregion

        #region Methods

        public void UpdateView()
        {
            tbxUsername.Text = this.MyConnection.GetUsername();
            tbxPassword.Text = string.Empty;
            tbxPasswordConfirmation.Text = string.Empty;
        }

        #endregion

        #region Methods (Events)

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        #endregion
    }
}
