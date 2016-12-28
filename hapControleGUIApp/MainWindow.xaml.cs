﻿using Codeplex.Data;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace hapControleGUIApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int nowVolume = 0;
        public static string nowPlaying = null;
        public static string album = null;
        public static string artist = null;
        public static string codec = null;
        public static string bandwidth = null;
        public static double bitrate = 0;
        public static string freq = null;
        public static string uri = null;
        public static double positionSec = 0;
        public static int posSec = 0;
        public static int posMin = 0;
        public static string coverArtUrl = null;
        public static string hostUrl = null;
        public static int noCoverArt = 0;
        public static int noAlbum = 0;
        public static int noArtist = 0;
        public static string musicId = null;
        public static string prevMusicId = null;
        public static string nowMusicCover = null;
        
        public static string myDocument = null;
        public static string ip = null;
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            setJunbi("getmusicinfo");
            musicName.Text = nowPlaying;
            musicArtist.Text = artist;
            musicAlbum.Text = album;
            string addi = codec.ToUpper() + " " + freq + "kHz/" + bandwidth + "bit " + bitrate + "kbps";
            musicCodec.Text = addi;
            minsec.Text = (posMin.ToString("00")) + ":" + (posSec.ToString("00"));
            dynamic getVolumeObj = getVolumeInfo();
            serializeJson(getVolumeObj, "audio", 1);
            volIn.Text = nowVolume.ToString();
            downloadCoverArt();
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(nowMusicCover);
            img.EndInit();
            coverArt.Source = img;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string str = ipaddInput.Text;
            ip = str;
            hostUrl = str + ":60200/sony/";
            setJunbi("getmusicinfo");
            musicName.Text = nowPlaying;
            musicArtist.Text = artist;
            musicAlbum.Text = album;
            string addi = codec.ToUpper() + " " + freq + "kHz/" + bandwidth + "bit " + bitrate + "kbps";
            musicCodec.Text = addi;
            minsec.Text = (posMin.ToString("00")) + ":" + (posSec.ToString("00"));
            dynamic getVolumeObj = getVolumeInfo();
            serializeJson(getVolumeObj, "audio", 1);
            volIn.Text = nowVolume.ToString();
            coverArt.Visibility = Visibility.Visible;

            myDocument = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/HAPControlApp";
            DirectoryInfo di = new DirectoryInfo(myDocument);
            di.Create();
            downloadCoverArt();

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(nowMusicCover);
            img.EndInit();
            coverArt.Source = img;

            minsec.Visibility = Visibility.Visible;
            musicAlbum.Visibility = Visibility.Visible;
            musicArtist.Visibility = Visibility.Visible;
            musicCodec.Visibility = Visibility.Visible;
            musicName.Visibility = Visibility.Visible;
            coverArt.Visibility = Visibility.Visible;
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }

        static void setCoverArt()
        {
            myDocument = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/HAPControlApp";
            DirectoryInfo di = new DirectoryInfo(myDocument);
            di.Create();
            downloadCoverArt();
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(nowMusicCover);
            img.EndInit();
        }

        static void downloadCoverArt()
        {
            if(musicId == null)
            {
                musicId = "NULL";
            }
            WebClient wc = new WebClient();
            nowMusicCover = myDocument + "/" + musicId;//現在のカバーアートのローカルアドレス
            if (musicId.IndexOf(".") == -1)
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
                    string noCoverUrl = ip + ":60100/img/album_default.png";
                    wc.DownloadFile(noCoverUrl, nowMusicCover);
                }
                
            }
            wc.Dispose();
        }

        static void connectHap(dynamic json, string url, int isNeedParse)//帰ってきたJSONが意味をなさないやつはこれ
        {
            HttpClient client = new HttpClient();
            string setUrl = hostUrl + url;
            Uri Url = new Uri(setUrl);
            client.DefaultRequestHeaders.ExpectContinue = false;//これは絶対いる
            StringContent theContent = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            dynamic result = client.PostAsync(Url, theContent).Result.Content.ReadAsStringAsync().Result;//送信部。帰ってくるJSONはresultに入る
            dynamic data = DynamicJson.Parse(result);
            if (isNeedParse == 1)//現在音量取得
            {
                nowVolume = (int)data.result[0].volume;
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
                codec = data.result[0].audioInfo[0].codec;
                bandwidth = data.result[0].audioInfo[0].bandwidth;
                bitrate = (double.Parse(data.result[0].audioInfo[0].bitrate)) / 1000;
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

        static void makeMutejson(int mode)//ミュート状態により処理変更
        {
            string str = null;
            if (mode == 1)
            {
                str = "off";
            }
            else
            {
                str = "on";
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

        static void serializeJson(dynamic obje, string url, int isNeedParse)//シリアライズして次に流すだけのメソッド
        {
            var json = DynamicJson.Serialize(obje);
            connectHap(json, url, isNeedParse);
        }

        static void setJunbi(string mode)//コマンドの場合分け
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

        static dynamic getVolumeInfo()//音量情報の取得用JSON生成
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
    }
}