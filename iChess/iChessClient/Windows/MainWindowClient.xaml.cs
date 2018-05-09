/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: MainWindowClient.xaml.cs
 * File description: The login user interface.
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iChessClient
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            // Custom icon
            Uri iconUri = new Uri(@"C:\Users\Administrateur\Documents\T_DIPL\Documentation\Poster\Logo_iChess.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            this.MyConnection = new ClientConnection();
        }

        #endregion
        
        #region Methods (Events)

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            int connectionState = this.MyConnection.ConnectToServer(tbxIpAddress.Text, Convert.ToInt32(tbxPort.Text), tbxUsername.Text, tbxPassword.Text);

            switch (connectionState)
            {
                case -1:
                    MessageBox.Show("Une erreur s'est produite lors de la tentative de connexion au server.");
                    break;

                case 0:
                    MainMenuClient mainMenu = new MainMenuClient(this.MyConnection);
                    mainMenu.Owner = this;
                    this.Hide();
                    mainMenu.Show();
                    break;

                case 1:
                    MessageBox.Show("Le serveur a refusé la connexion.");
                    break;

                default:
                    break;
            }
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            Window registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            this.Hide();
            registerWindow.Show();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
