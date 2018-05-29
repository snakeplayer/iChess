/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessClient
 * Project description: A local network chess game. 
 * File: RankingWindowClient.xaml.cs
 * File description: The ranking user interface.
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
    /// Logique d'interaction pour RankingWindowClient.xaml
    /// </summary>
    public partial class RankingWindowClient : Window
    {
        #region Properties

        public ClientConnection MyConnection { get; set; }

        #endregion

        #region Constructors

        public RankingWindowClient(ClientConnection myConnection)
        {
            InitializeComponent();

            // Custom icon
            Uri iconUri = new Uri(@"C:\Users\Administrateur\Documents\T_DIPL\Documentation\Poster\Logo_iChess.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            this.MyConnection = myConnection;

            this.UpdateView();

            //DEBUG
            /*
            lstRanking.Add(new RankingItem { Rank = "1", Player = "David", EloRating = "2340", WLRatio = "1.54" });
            lstRanking.Add(new RankingItem { Rank = "2", Player = "Jean", EloRating = "1210", WLRatio = "2.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            lstRanking.Add(new RankingItem { Rank = "3", Player = "Paul", EloRating = "1100", WLRatio = "1.24" });
            */
            
            List<RankingItem> lstRanking = new List<RankingItem>();
            AllClientsDetails acd = this.MyConnection.GetAllClientsDetails();

            //acd.ClientList.Sort();
            int i = 1;
            acd.ClientList.ToList().ForEach(c => lstRanking.Add(new RankingItem { Rank = (i++).ToString(), Player = c.Username, EloRating = c.EloRating.ToString(), WLRatio = c.NumberOfDefeats > 0 ? (Math.Round(Convert.ToDouble(c.NumberOfWins) / Convert.ToDouble(c.NumberOfDefeats), 2)).ToString() : "1" }));

            lstRanking.ToList().ForEach(line => lwRanking.Items.Add(line));
        }

        #endregion

        #region Methods

        public void UpdateView()
        {
            // Recovering client details
            ClientDetails clientDetails = this.MyConnection.GetMyDetails();

            // Header
            this.lblUsername.Content = clientDetails.Username;
            this.lblEloRating.Content = clientDetails.EloRating.ToString();
        }

        #endregion

        #region Methods (Events)

        private void lblUsername_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ProfileWindowClient profileWindow = new ProfileWindowClient(this.MyConnection);
            profileWindow.Owner = this;
            this.Hide();
            profileWindow.Show();
        }

        private void imgLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Hide();
            this.Owner.Owner.Show();
        }

        private void imgBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.Owner.Show();
        }

        private void WindowRanking_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MyConnection.DisconnectFromServer();
            this.Owner.Close();
        }

        #endregion
    }
}
