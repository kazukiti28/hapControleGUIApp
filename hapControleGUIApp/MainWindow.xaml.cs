using Codeplex.Data;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Collections.Generic;

namespace hapControleGUIApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private NavigationService _navi;
        private List<Uri> _uriList = new List<Uri>() {
            new Uri("cont.xaml",UriKind.Relative),
            new Uri("nav.xaml",UriKind.Relative),
        };

        private void myFrame_Loaded(object sender, RoutedEventArgs e)//初期ページの指定
        {
            _navi.Navigate(_uriList[0]);
        }


        public MainWindow()
        {
            InitializeComponent();
            _navi = myFrame.NavigationService;
        }

        private void changebutton_Click(object sender, RoutedEventArgs e)
        {
            if (_navi.CanGoForward)
                _navi.GoForward();
            else
            {
                int index = _uriList.FindIndex(p => p == _navi.CurrentSource) + 1;
                _navi.Navigate(_uriList[index]);
            }
        }

    }   
}