using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace hapControlGUIApp
{
    /// <summary>
    /// nav.xaml の相互作用ロジック
    /// </summary>
    public partial class nav : Page
    {
        public nav()
        {
            InitializeComponent();
            BG.Background = new SolidColorBrush(Color.FromArgb(Convert.ToByte(cont.a, 16), Convert.ToByte(cont.r, 16), Convert.ToByte(cont.g, 16), Convert.ToByte(cont.g, 16)));
            BitmapImage bgimg = new BitmapImage();
            bgimg.BeginInit();
            bgimg.UriSource = new Uri(cont.shabgurl);
            bgimg.EndInit();
            bgimage.Source = bgimg;
            ipaddInput.Text = cont.ip;
            ipadd.Foreground = new SolidColorBrush(Colors.White);
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(cont.nowMusicCover);
            img.EndInit();
            coverArt.Source = img;

            musicName.Text = cont.nowPlaying;

            musicAlbum.Text = cont.album;
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void volPlu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void volMin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Mute_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void change_click(object sender, RoutedEventArgs e)
        {
            cont con = new cont();
            NavigationService?.Navigate(con);
        }
    }
}
