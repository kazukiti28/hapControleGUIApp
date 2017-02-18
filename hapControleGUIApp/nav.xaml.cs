using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using System.Windows.Threading;
using Codeplex.Data;

namespace hapControlGUIApp
{
    /// <summary>
    /// nav.xaml の相互作用ロジック
    /// </summary>
    public partial class nav : Page
    {
        static int nowVolume = 0;
        static string nowPlaying = null;
        static string album = null;
        static string artist = null;
        static string freq = null;
        static string uri = null;
        static double positionSec = 0;
        static int posSec = 0;
        static int posMin = 0;
        static string coverArtUrl = null;
        static string hostUrl = null;
        static int noCoverArt = 0;
        static int noAlbum = 0;
        static int noArtist = 0;
        static string musicId = null;
        static string prevMusicId = null;
        static string nowMusicCover = null;
        static string r = null;
        static string g = null;
        static string b = null;
        static string a = null;
        static string myDocument = null;
        static string ip = null;
        static string totalfigure = null;
        DispatcherTimer dispatcherTimer;

        public nav()
        {
            InitializeComponent();
            myDocument = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/HAPControlApp";
            DirectoryInfo di = new DirectoryInfo(myDocument);
            di.Create();
            musicName.Text = cont.nowPlaying;
            musicAlbum.Text = cont.album;
            volIn.Text = cont.nowVolume.ToString();

            BitmapImage bgimg = new BitmapImage();
            bgimg.BeginInit();
            bgimg.UriSource = new Uri(cont.shabgurl);
            bgimg.EndInit();
            bgimage.Source = bgimg;

            ipaddInput.Text = cont.ip;
            ipadd.Foreground = new SolidColorBrush(Colors.White);

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            setJunbi("getmusicinfo");
            BG.Background =
                new SolidColorBrush(Color.FromArgb(Convert.ToByte(a, 16), Convert.ToByte(r, 16),
                    Convert.ToByte(g, 16), Convert.ToByte(g, 16)));

            serializeJson(getVolumeInfo(), "audio", 1);
            downloadCoverArt();
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(nowMusicCover);
            img.EndInit();

            coverArt.Source = img;
            musicName.Text = nowPlaying;
            musicAlbum.Text = album;
            volIn.Text = nowVolume.ToString();
        }

        void getAllAlbumInfo()
        {
            string setUrl = cont.rawip + ":60200/sony/contentdb/v100/audio/albums";

            var req = WebRequest.Create(setUrl);
            var res = req.GetResponse();

            dynamic data = DynamicJson.Parse(res.GetResponseStream());
            totalfigure = data.paging.total;

            setUrl = setUrl + "?offset=0&limit=" + totalfigure;
            req = WebRequest.Create(setUrl);
            res = req.GetResponse();
            data = DynamicJson.Parse(res.GetResponseStream());//これにたくさん入っている


        }

        void downloadCoverArt()
        {
            if (musicId == null)
            {
                musicId = "NULL";
                noCoverArt = 1;
            }
            WebClient wc = new WebClient();
            nowMusicCover = myDocument + "/" + musicId;//現在のカバーアートのローカルアドレス
            if (musicId.IndexOf(".") == -1 && noCoverArt != 1)
            {
                nowMusicCover = nowMusicCover + ".jpg";
                if (!File.Exists(nowMusicCover))
                {
                    wc.DownloadFile(coverArtUrl, nowMusicCover);
                }
            }
            if (noCoverArt == 1)
            {
                nowMusicCover = myDocument + "/" + "default.png";
                if (!File.Exists(nowMusicCover))
                {
                    string ipad = hostUrl.Replace(":60200/sony/", "");
                    string noCoverUrl = ipad + ":60100/img/album_default.png";
                    wc.DownloadFile(noCoverUrl, nowMusicCover);
                }
            }
            wc.Dispose();
        }

        void makeMutejson(int mode)//ミュート状態により処理変更
        {
            string str = null;
            if (mode == 1)
            {
                str = "off";
                Mute.Content = "Mute Off";
            }
            else
            {
                str = "on";
                Mute.Content = "Mute On";
            }
            var obj = new
            {
                @params = new[] {
                    new
                    {
                        mute = str,
                    }
                },
                method = "setAudioMute",
                version = "1.1",
                id = 1,
            };
            serializeJson(obj, "audio", 0);
        }

        void serializeJson(dynamic obje, string url, int isNeedParse)
        {
            var json = DynamicJson.Serialize(obje);
            connectHap(json, url, isNeedParse);
        }

        void connectHap(dynamic json, string url, int isNeedParse)
        {
            HttpClient client = new HttpClient();
            string setUrl = cont.hostUrl + url;
            Uri Url = new Uri(setUrl);
            client.DefaultRequestHeaders.ExpectContinue = false;
            StringContent theContent = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            dynamic result = client.PostAsync(Url, theContent).Result.Content.ReadAsStringAsync().Result;
            dynamic data = DynamicJson.Parse(result);
            if (isNeedParse == 1)//現在音量取得
            {
                nowVolume = (int)data.result[0].volume;
                if (data.result[0].mute == "on")
                {
                    Mute.Content = "Mute On";
                }
                if (data.result[0].mute == "off")
                {
                    Mute.Content = "Mute Off";
                }
            }
            else if (isNeedParse == 2)//楽曲情報取得
            {
                nowPlaying = data.result[0].title;
                noAlbum = 0;
                try
                {
                    album = data.result[0].albumName;
                }
                catch
                {
                    noAlbum = 1;
                    album = "undefined album";
                }
                noArtist = 0;
                try
                {
                    artist = data.result[0].artist;
                }
                catch
                {
                    noArtist = 1;
                    artist = "undefined artist";
                }
                freq = data.result[0].audioInfo[0].frequency;
                double f = double.Parse(freq);
                f = f / 1000;
                freq = f.ToString();
                noCoverArt = 0;
                try
                {
                    coverArtUrl = data.result[0].coverArtUrl;
                    int findsla = coverArtUrl.LastIndexOf("/") + 1;
                    musicId = coverArtUrl.Substring(findsla);
                    if (prevMusicId == null)
                    {
                        prevMusicId = musicId;
                    }

                }
                catch (Exception e)
                {
                    noCoverArt = 1;
                }
                if (noCoverArt != 1)
                {
                    r = Convert.ToString((int)data.result[0].backgroundColorR, 16);
                    g = Convert.ToString((int)data.result[0].backgroundColorG, 16);
                    b = Convert.ToString((int)data.result[0].backgroundColorB, 16);
                    a = Convert.ToString((int)data.result[0].backgroundColorA, 16);
                }
                positionSec = data.result[0].positionSec;
                posSec = (int)positionSec;
                posMin = posSec / 60;//分数
                posSec = posSec % 60;//秒数

            }
            else if (isNeedParse == 3)//ミュートチェック
            {
                if (data.result[0].mute == "on")
                {
                    makeMutejson(1);
                }
                if (data.result[0].mute == "off")
                {
                    makeMutejson(0);
                }
            }
        }


        void setJunbi(string mode)//コマンドの場合分け
        {
            string url = null;
            if (mode == "previous")//前の曲(頭出し)
            {
                var obj = new
                {
                    @params = new[] { new { } },
                    method = "setPlayPreviousContent",
                    version = "1.0",
                    id = 1,
                };
                url = "avContent";
                serializeJson(obj, url, 0);
            }
            if (mode == "poweron")//電源オン
            {
                var obj = new
                {
                    @params = new[] {
                        new
                        {
                            status = "active",
                            standbyDetail = "",
                        }
                    },
                    method = "setPowerStatus",
                    version = "1.1",
                    id = 1,
                };
                url = "system";
                serializeJson(obj, url, 0);
            }
            if (mode == "poweroff")//電源オフ
            {
                var obj = new
                {
                    @params = new[] {
                        new
                        {
                            status = "off",
                            standbyDetail = "",
                        }
                    },
                    method = "setPowerStatus",
                    version = "1.1",
                    id = 1,
                };
                url = "system";
                serializeJson(obj, url, 0);
            }
            if (mode == "start" || mode == "stop")//再生、停止
            {
                var obj = new
                {
                    @params = new[] { new { } },
                    method = "pausePlayingContent",
                    version = "1.0",
                    id = 1,
                };
                url = "avContent";
                serializeJson(obj, url, 0);
            }
            if (mode == "next")//次の曲
            {
                var obj = new
                {
                    @params = new[] { new { } },
                    method = "setPlayNextContent",
                    version = "1.0",
                    id = 1,
                };
                url = "avContent";
                serializeJson(obj, url, 0);
            }
            if (mode == "volumeup")//音量↑
            {
                dynamic getVolumeObj = getVolumeInfo();
                url = "audio";
                serializeJson(getVolumeObj, url, 1);//ここまで現情報取得
                nowVolume += 1;//ここから現ボリュームに1足した値をJSONで送信
                string upvolume = nowVolume.ToString();
                var obj = new
                {
                    @params = new[] {
                        new
                        {
                            volume = upvolume,
                        }
                    },
                    method = "setAudioVolume",
                    version = "1.0",
                    id = 1,
                };
                serializeJson(obj, url, 0);
            }
            if (mode == "volumedown")//音量↓
            {
                dynamic getVolumeObj = getVolumeInfo();
                url = "audio";
                serializeJson(getVolumeObj, url, 1);
                nowVolume -= 1;
                string downvolume = nowVolume.ToString();
                var obj = new
                {
                    @params = new[] {
                        new
                        {
                            volume = downvolume,
                        }
                    },
                    method = "setAudioVolume",
                    version = "1.0",
                    id = 1,
                };
                serializeJson(obj, url, 0);
            }
            if (mode == "getmusicinfo")//再生中楽曲情報取得
            {
                var obj = new
                {
                    @params = new[] {
                        new
                        {
                            level ="detail",
                        }
                    },
                    method = "getPlayingContentInfo",
                    version = "1.2",
                    id = 3,
                };
                url = "avContent";
                serializeJson(obj, url, 2);
            }
            if (mode == "mute")//ミュートオンオフ
            {
                dynamic setMuteObj = getVolumeInfo();
                url = "audio";
                serializeJson(setMuteObj, url, 3);
            }
        }

        dynamic getVolumeInfo()//音量情報の取得用JSON生成
        {
            var getVolumeObj = new
            {
                @params = new[] { new { } },
                method = "getVolumeInformation",
                version = "1.1",
                id = 4,
            };
            return getVolumeObj;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            setJunbi("start");
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            setJunbi("next");
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            setJunbi("previous");
        }

        private void volMin_Click(object sender, RoutedEventArgs e)
        {
            setJunbi("volumedown");
        }

        private void volPlu_Click(object sender, RoutedEventArgs e)
        {
            setJunbi("volumeup");
        }

        private void Mute_OnClick(object sender, RoutedEventArgs e)
        {
            setJunbi("mute");
        }
        
        private void change_click(object sender, RoutedEventArgs e)
        {
            cont con = new cont();
            NavigationService?.Navigate(con);
        }
    }
}
