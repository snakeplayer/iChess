/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: MainWindowServer.xaml.cs
 * File description: The iChess server user interface.
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

namespace iChessServer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserverWindow
    {
        #region Properties

        public ServerConnection MyServer { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.MyServer = new ServerConnection();
            this.MyServer.RegisterObserver(this);
        }

        #endregion

        #region Methods

        public void UpdateView()
        {
            List<string> lstClients = this.MyServer.GetAuthenticatedClients();
            this.Dispatcher.Invoke(() =>
            {
                this.tbxLogs.Text = this.MyServer.GetLogs();
                this.tbxLogs.ScrollToEnd();

                this.lbxClients.Items.Clear();
                lstClients.ToList().ForEach(client => lbxClients.Items.Add(client));
            });
        }

        #endregion

        #region Methods (Events)

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.MyServer.StartServer();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.MyServer.StopServer();
        }

        #endregion        
    }
}
