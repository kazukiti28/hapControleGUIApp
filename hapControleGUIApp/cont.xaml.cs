using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Codeplex.Data;

namespace hapControlGUIApp
{
    /// <summary>
    /// cont.xaml の相互作用ロジック
    /// </summary>
    public partial class cont : Page
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
        public static string r = null;
        public static string g = null;
        public static string b = null;
        public static string a = null;
        public static string myDocument = null;
        public static string ip = null;
        public static string nowRepeat = null;
        public static string nowShuffle = null;
        public static string shabgurl;
        public static string rawip = null;
        public static bool isPlayNow;
        public static bool extinput;
        public static string nextimg = "/next.png";
        public static string previmg = "/prev.png";
        public static string playimg = "/play.png";
        public static string stopimg = "/stop.png";
        public static string muteon = "/muteon.png";
        public static string muteoff = "/muteoff.png";
        public static bool isMute;
        public static string volm = "/minus.png";
        public static string volp = "/plus.png";
        public static double posTime;
        public static double musiclen;
        public static bool isDragging;
        public static int coverartStop = 1;
        public static dynamic queueData;
        public static string extMode;
        DispatcherTimer dispatcherTimer;
        static double playlistModifiedVersion;
        static string playlistUri;

        public void hiddenall()
        {
            coverArt.Visibility = Visibility.Hidden;
            nextButton.Visibility = Visibility.Hidden;
            prevButton.Visibility = Visibility.Hidden;
            offshu.Visibility = Visibility.Hidden;
            albumshu.Visibility = Visibility.Hidden;
            allshu.Visibility = Visibility.Hidden;
            repoff.Visibility = Visibility.Hidden;
            repall.Visibility = Visibility.Hidden;
            repone.Visibility = Visibility.Hidden;
            startButton.Visibility = Visibility.Hidden;
            Repeat.Visibility = Visibility.Hidden;
            musicAlbum.Visibility = Visibility.Hidden;
            musicArtist.Visibility = Visibility.Hidden;
            musicCodec.Visibility = Visibility.Hidden;
            minsec.Visibility = Visibility.Hidden;
            slider.Visibility = Visibility.Hidden;
            slider.Visibility = Visibility.Hidden;
            shuffle.Visibility = Visibility.Hidden;
            previmage.Visibility = Visibility.Hidden;
            nextimage.Visibility = Visibility.Hidden;
        }
        public void showall()
        {
            musicArtist.Visibility = Visibility.Visible;
            coverArt.Visibility = Visibility.Visible;
            prevButton.Visibility = Visibility.Visible;
            nextButton.Visibility = Visibility.Visible;
            prevButton.Visibility = Visibility.Visible;
            offshu.Visibility = Visibility.Visible;
            albumshu.Visibility = Visibility.Visible;
            allshu.Visibility = Visibility.Visible;
            repoff.Visibility = Visibility.Visible;
            repall.Visibility = Visibility.Visible;
            repone.Visibility = Visibility.Visible;
            startButton.Visibility = Visibility.Visible;
            Repeat.Visibility = Visibility.Visible;
            musicName.Visibility = Visibility.Visible;
            musicAlbum.Visibility = Visibility.Visible;
            musicCodec.Visibility = Visibility.Visible;
            minsec.Visibility = Visibility.Visible;
            slider.Visibility = Visibility.Visible;
            nextButton.Visibility = Visibility.Visible;
            prevButton.Visibility = Visibility.Visible;
            slider.Visibility = Visibility.Visible;
            shuffle.Visibility = Visibility.Visible;
            previmage.Visibility = Visibility.Visible;
            nextimage.Visibility = Visibility.Visible;
            startimage.Visibility = Visibility.Visible;
            pimg.Visibility = Visibility.Visible;
            mimg.Visibility = Visibility.Visible;
            volIn.Visibility = Visibility.Visible;
            browse.Visibility = Visibility.Visible;
        }

        public cont()
        {
            InitializeComponent();
            isDragging = false;
            myDocument = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/HAPControlApp";
            DirectoryInfo di = new DirectoryInfo(myDocument);
            di.Create();

            string bgurl = myDocument + "/bg_overlay.png";
            shabgurl = bgurl;
            Encoding utf = Encoding.GetEncoding("UTF-8");
            string fileName = myDocument + "/ipadd";
            if (File.Exists(fileName))
            {
                ipaButton.Visibility = Visibility.Hidden;
                StreamReader sr = new StreamReader(fileName, utf);
                ip = sr.ReadLine();
                sr.Close();
                hostUrl = ip;
                ipaddInput.Text = ip;
                rawip = ip;
                setJunbi("getmusicinfo");
                if (!File.Exists(myDocument + "/bg_overlay.png"))
                {
                    WebClient wc = new WebClient();
                    string ipad = hostUrl.Replace(":60200/sony/", "");
                    string url = ipad + ":60100/img/bg_overlay.png";
                    wc.DownloadFile(url, bgurl);
                }
                try
                {
                    WebClient wec = new WebClient();
                    wec.DownloadFile(hostUrl.Replace(":60200/sony/", "") + ":60100/img/icon_input_optical.png", myDocument + "/optical.png");
                    wec.DownloadFile(hostUrl.Replace(":60200/sony/", "") + ":60100/img/icon_input_coaxial.png", myDocument + "/coaxial.png");
                    wec.DownloadFile(hostUrl.Replace(":60200/sony/", "") + ":60100/img/icon_input_analog.png", myDocument + "/line.png");
                }
                catch { }
                BitmapImage bgimg = new BitmapImage();
                bgimg.BeginInit();
                bgimg.UriSource = new Uri(bgurl);
                bgimg.EndInit();
                bgimage.Source = bgimg;

                setRightimg();
                setLeftimg();

                setPlusimg();

                setMinusimg();

                dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Start();
                
                ipaButton.Visibility = Visibility.Hidden;
                
                ipadd.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        void setMinusimg()
        {
            BitmapImage volminus = new BitmapImage();
            volminus.BeginInit();
            volminus.UriSource = new Uri(myDocument + volm);
            volminus.EndInit();
            mimg.Source = volminus;
        }

        void setPlusimg()
        {
            BitmapImage volplus = new BitmapImage();
            volplus.BeginInit();
            volplus.UriSource = new Uri(myDocument + volp);
            volplus.EndInit();
            pimg.Source = volplus;
        }

        void setRightimg()
        {
            BitmapImage next = new BitmapImage();
            next.BeginInit();
            next.UriSource = new Uri(myDocument + nextimg);
            next.EndInit();
            nextimage.Source = next;
        }

        void setCenterimg()
        {
            BitmapImage butimg = new BitmapImage();
            butimg.BeginInit();
            if (isPlayNow) butimg.UriSource = new Uri(myDocument + playimg);
            else butimg.UriSource = new Uri(myDocument + stopimg);
            butimg.EndInit();
            startimage.Source = butimg;
        }

        void setLeftimg()
        {
            BitmapImage prev = new BitmapImage();
            prev.BeginInit();
            prev.UriSource = new Uri(myDocument + previmg);
            prev.EndInit();
            previmage.Source = prev;
        }

        void updatePlaylist(int index,string mode)
        {
            string mov = "";
            string dat = "";
            if (mode == "up")
            {
                mov = "," + (index - 1).ToString();
                dat = "types=1,0&trackIds=-1,";
            }
            else if (mode == "down")
            {
                mov = "," + (index + 1).ToString();
                dat = "types=1,0&trackIds=-1,";
            }
            else if (mode == "del")
            {
                mov = "";
                dat = "types=1&trackIds=";
            }
            var obj = new
            {
                data = dat + musicIdarr[index] + "&positions=" + index.ToString() + mov,
                @params = new[] {
                        new
                        {
                            uri = "audio:list?id=" + playlistUri.Replace("audio:playinglist?id=","") + "&originalVersion=" + playlistModifiedVersion.ToString(),
                        }
                    },
                method = "updatePlaylist",
                version = "1.0",
                id = 2,
            };
            serializeJson(obj, "avContent", 0);
        }
        static int first = 1;

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            setJunbi("getmusicinfo");
            if (!extinput)
            {
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
                if (nowRepeat == "all")
                {
                    repall.IsChecked = true;
                }
                else if (nowRepeat == "off")
                {
                    repoff.IsChecked = true;
                }
                else if (nowRepeat == "one")
                {
                    repone.IsChecked = true;
                }
                if (coverartStop == 1)
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(nowMusicCover);
                    img.EndInit();
                    coverArt.Source = img;
                }
                else
                {
                    coverArt.Visibility = Visibility.Hidden;
                }
                BG.Background = new SolidColorBrush(Color.FromArgb(Convert.ToByte(a, 16), Convert.ToByte(r, 16), Convert.ToByte(g, 16), Convert.ToByte(b, 16)));

                if (nowShuffle == "track")
                {
                    allshu.IsChecked = true;
                }
                else if (nowShuffle == "off")
                {
                    offshu.IsChecked = true;
                }
                else if (nowShuffle == "album")
                {
                    albumshu.IsChecked = true;
                }
                setCenterimg();
                if (!isDragging) slider.Value = ((posMin * 60 + posSec) / musiclen) * 100;
                getplayqueue();
                if (first == 1)
                {
                    showall();
                    first = 0;
                }
            }
            else
            {
                hiddenall();
                musicName.Text = extMode;
                dynamic getVolumeObj = getVolumeInfo();
                serializeJson(getVolumeObj, "audio", 1);
                volIn.Text = nowVolume.ToString();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string str = ipaddInput.Text;
            ip = str;
            hostUrl = str + ":60200/sony/";

            StreamWriter writer = new StreamWriter(myDocument + "/ipadd", true, Encoding.GetEncoding("UTF-8"));
            writer.WriteLine(hostUrl);
            writer.Close();

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


            if (nowRepeat == "track")
            {
                repall.IsChecked = true;
            }
            else if (nowRepeat == "off")
            {
                repoff.IsChecked = true;
            }
            else if (nowRepeat == "one")
            {
                repone.IsChecked = true;
            }

            if (nowShuffle == "track")
            {
                allshu.IsChecked = true;
            }
            else if (nowShuffle == "off")
            {
                offshu.IsChecked = true;
            }
            else if (nowShuffle == "album")
            {
                albumshu.IsChecked = true;
            }

            downloadCoverArt();

            string bgurl = myDocument + "/bg_overlay.png";
            if (!File.Exists(bgurl))
            {
                WebClient wc = new WebClient();
                string ipad = hostUrl.Replace(":60200/sony/", "");
                ipad = ipad + ":60100/img/bg_overlay.png";
                wc.DownloadFile(ipad, bgurl);
            }
            BitmapImage bgimg = new BitmapImage();
            bgimg.BeginInit();
            bgimg.UriSource = new Uri(bgurl);
            bgimg.EndInit();
            bgimage.Source = bgimg;

            ipadd.Foreground = new SolidColorBrush(Colors.White);


            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(nowMusicCover);
            img.EndInit();
            coverArt.Source = img;
            BG.Background =
                new SolidColorBrush(Color.FromArgb(Convert.ToByte(a, 16), Convert.ToByte(r, 16), Convert.ToByte(g, 16),
                    Convert.ToByte(b, 16)));
            ipaButton.Visibility = Visibility.Hidden;

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
           
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

        void setMute(string name)
        {
            BitmapImage mute = new BitmapImage();
            mute.BeginInit();
            mute.UriSource = new Uri(myDocument + name);
            mute.EndInit();
            muteimg.Source = mute;
        }

        void connectHap(dynamic json, string url, int isNeedParse)//帰ってきたJSONが意味をなさないやつはこれ
        {
            HttpClient client = new HttpClient();
            string setUrl = hostUrl + url;
            Uri Url = new Uri(setUrl);
            client.DefaultRequestHeaders.ExpectContinue = false;//これは絶対いる
            StringContent theContent = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            dynamic result = client.PostAsync(Url, theContent).Result.Content.ReadAsStringAsync().Result;
            dynamic data = DynamicJson.Parse(result);
            if (isNeedParse == 1)//現在音量取得
            {
                nowVolume = (int)data.result[0].volume;
                if (data.result[0].mute == "on")
                {
                    setMute(muteon);
                    isMute = true;
                }
                if (data.result[0].mute == "off")
                {
                    setMute(muteoff);
                    isMute = false;
                }
            }
            else if (isNeedParse == 2)//楽曲情報取得
            {
                string playmode = data.result[0].state;
                if (playmode == "PAUSED")
                {
                    isPlayNow = false;
                    if(first != 1) showall();
                }
                else if (playmode == "PLAYING")
                {
                    isPlayNow = true;
                    if(first != 1) showall();
                }
                else if (playmode == "STOPPED")
                {
                    string extType = data.result[0].uri;
                    extType = extType.Replace("extInput:", "");
                    string type = extType.Substring(0, 4);
                    if (type == "line")
                    {
                        extMode = "Line In " + extType.Substring(extType.Length - 1, 1) + "(外部入力)";
                    }
                    else if (type == "coax")
                    {
                        extMode = "Coaxial In (外部入力中)";
                    }
                    else if(type == "opti")
                    {
                        extMode = "Optical In (外部入力中)";
                    }
                    extinput = true;
                    return;
                }
                musiclen = data.result[0].durationSec;
                playlistModifiedVersion = data.result[0].playlistModifiedVersion;
                playlistUri = data.result[0].playlistUri;
                nowalbumId = data.result[0].albumID;
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

                nowRepeat = data.result[0].repeatType;
                nowShuffle = data.result[0].shuffleType;
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

        static string nowalbumId;

        void makeMutejson(int mode)//ミュート状態により処理変更
        {
            string str = null;
            if (mode == 1)
            {
                str = "off";
                setMute(muteoff);
            }
            else
            {
                str = "on";
                setMute(muteon);
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

        void serializeJson(dynamic obje, string url, int isNeedParse)//シリアライズして次に流すだけのメソッド
        {
            var json = DynamicJson.Serialize(obje);
            connectHap(json, url, isNeedParse);
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

        void seekPosition()
        {
            var seekPosObj = new
            {
                @params = new[] {
                        new
                        {
                            positionSec = posTime,
                        }
                    },
                method = "setPlayContent",
                version = "1.1",
                id = 1,
            };
            serializeJson(seekPosObj, "avContent", 0);
        }

        static int queueleng;

        dynamic oldqueuedata = null;

        void getplayqueue()
        {
            if (!extinput)
            {
                try
                {
                    string setUrl = rawip + "contentplayer/v100/playqueue/tracks";

                    var req = WebRequest.Create(setUrl);
                    var res = req.GetResponse();

                    dynamic data = DynamicJson.Parse(res.GetResponseStream());
                    string totalfigure = data.paging.total.ToString();

                    setUrl = setUrl + "?offset=0&limit=" + totalfigure;
                    req = WebRequest.Create(setUrl);
                    res = req.GetResponse();
                    if (oldqueuedata == null) oldqueuedata = data;
                    queueData = DynamicJson.Parse(res.GetResponseStream());
                    if (oldqueuedata.ToString() != queueData.ToString())
                    {
                        int numberoftracks = (int)queueData.paging.total; //アルバム収録曲数の取得
                        queueleng = numberoftracks;
                        string[] Tracks = new string[numberoftracks]; //曲の名前を格納する配列
                        int[] Duration = new int[numberoftracks]; //曲長さ格納
                        string[] Codec = new string[numberoftracks]; //コーデック格納
                        string[] Freq = new string[numberoftracks]; //サンプリング周波数
                        string[] Bitwidth = new string[numberoftracks]; //サンプリング周波数
                        string[] Bitrate = new string[numberoftracks];

                        for (int num = 0; num < numberoftracks; num++)
                        {
                            Tracks[num] = queueData.tracks[num].name; //すべての名前の取得
                            Duration[num] = (int)queueData.tracks[num].duration; //曲長さ(秒)
                            Codec[num] = queueData.tracks[num].codec.codec_type; //コーデック
                            Bitrate[num] = (queueData.tracks[num].codec.bit_rate / 1000).ToString(); //ビットレート
                            int fr = (int)queueData.tracks[num].codec.sample_rate / 1000;
                            Freq[num] = fr.ToString(); //サンプリング周波数
                            Bitwidth[num] = queueData.tracks[num].codec.bit_width.ToString(); //ビット深度
                        }
                        string info = "";
                        string min = "";
                        string sec = "";
                        string dur = "";

                        TracksdataList = new List<VisibleItem>();
                        VisibleItem tracklist;

                        for (int cnt = 0; cnt < numberoftracks; cnt++)
                        {
                            min = (Duration[cnt] / 60).ToString();
                            if (Duration[cnt] % 60 < 10)
                            {
                                sec = (Duration[cnt] % 60).ToString();
                                sec = "0" + sec;
                            }
                            else
                            {
                                sec = (Duration[cnt] % 60).ToString();
                            }

                            dur = min + ":" + sec;

                            if (Codec[cnt] == "alac" || Codec[cnt] == "flac" || Codec[cnt] == "aiff" || Codec[cnt] == "wav")//可逆圧縮/非圧縮
                            {
                                info = Codec[cnt].ToUpper() + " " + Freq[cnt] + "kHz" + "/" + Bitwidth[cnt] + "bit  " + dur;
                            }
                            else if (Codec[cnt] == "dsd" || Codec[cnt] == "dsf")//DSD
                            {
                                info = Codec[cnt].ToUpper() + " " + Freq[cnt] + "MHz  " + dur;
                            }
                            else//圧縮音源
                            {
                                info = Codec[cnt].ToUpper() + " " + Bitrate[cnt] + "kbps  " + dur;
                            }

                            tracklist = new VisibleItem();

                            tracklist.ArtistName = "   " + queueData.tracks[cnt].artist.name;
                            tracklist.TrackName = queueData.tracks[cnt].name;
                            tracklist.TrackInfo = info;
                            tracklist.ContentUrl = queueData.tracks[cnt].album.url;
                            tracklist.MusicId = (queueData.tracks[cnt].trackid).ToString();
                            TracksdataList.Add(tracklist);
                            musicIdarr.Add((queueData.tracks[cnt].trackid).ToString());
                            info = "";
                        }
                        listBoxqueue.ItemsSource = TracksdataList;
                    }

                    oldqueuedata = queueData;
                    var border = VisualTreeHelper.GetChild(listBoxqueue, 0) as Border;
                    if (border != null)
                    {
                        dynamic listBoxScroll = border.Child as ScrollViewer;
                        if (listBoxScroll != null)
                        {
                            // スクロールバー非表示
                            listBoxScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        }
                    }
                }
                catch { }
            }
        }

        public List<string> musicIdarr = new List<string>();

        public List<VisibleItem> TracksdataList { get; set; }

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

        private void repall_Checked(object sender, RoutedEventArgs e)
        {
            var obj = new
            {
                @params = new[] {
                        new
                        {
                            type ="all",
                        }
                    },
                method = "setRepeatType",
                version = "1.0",
                id = 0,
            };
            serializeJson(obj, "avContent", 0);
        }

        private void repone_Checked(object sender, RoutedEventArgs e)
        {
            var obj = new
            {
                @params = new[] {
                        new
                        {
                            type ="one",
                        }
                    },
                method = "setRepeatType",
                version = "1.0",
                id = 0,
            };
            serializeJson(obj, "avContent", 0);
        }

        private void repoff_Checked(object sender, RoutedEventArgs e)
        {
            var obj = new
            {
                @params = new[] {
                        new
                        {
                            type ="off",
                        }
                    },
                method = "setRepeatType",
                version = "1.0",
                id = 0,
            };
            serializeJson(obj, "avContent", 0);
        }

        private void allshu_Checked(object sender, RoutedEventArgs e)
        {
            var obj = new
            {
                @params = new[] {
                        new
                        {
                            type ="track",
                        }
                    },
                method = "setShuffleType",
                version = "1.0",
                id = 0,
            };
            serializeJson(obj, "avContent", 0);
        }

        private void albumshu_Checked(object sender, RoutedEventArgs e)
        {
            var obj = new
            {
                @params = new[] {
                        new
                        {
                            type ="album",
                        }
                    },
                method = "setShuffleType",
                version = "1.0",
                id = 0,
            };
            serializeJson(obj, "avContent", 0);
        }

        private void offshu_Checked(object sender, RoutedEventArgs e)
        {
            var obj = new
            {
                @params = new[] {
                        new
                        {
                            type ="off",
                        }
                    },
                method = "setShuffleType",
                version = "1.0",
                id = 0,
            };
            serializeJson(obj, "avContent", 0);
        }

        private void Mute_OnClick(object sender, RoutedEventArgs e)
        {
            setJunbi("mute");
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            coverartStop = 1;
            dispatcherTimer.Stop();
            nav navig = new nav();
            NavigationService?.Navigate(navig);
        }

        private void hover1(object sender, MouseEventArgs e)
        {
            previmg = "/prevhov.png";
            setLeftimg();
        }
        private void hover2(object sender, MouseEventArgs e)
        {
            playimg = "/playhov.png";
            stopimg = "/stophov.png";
            setCenterimg();
        }

        private void hover3(object sender, MouseEventArgs e)
        {
            nextimg = "/nexthov.png";
            setRightimg();
        }

        private void leaved1(object sender, MouseEventArgs e)
        {
            previmg = "/prev.png";
            setLeftimg();
        }

        private void leaved2(object sender,MouseEventArgs e)
        {
            playimg = "/play.png";
            stopimg = "/stop.png";
            setCenterimg();
        }
        private void leaved3(object sender, MouseEventArgs e)
        {
            nextimg = "/next.png";
            setRightimg();
        }

        private void volPlu_MouseEnter(object sender, MouseEventArgs e)
        {
            volp = "/plushov.png";
            setPlusimg();
        }

        private void volPlu_MouseLeave(object sender, MouseEventArgs e)
        {
            volp = "/plus.png";
            setPlusimg();
        }

        private void volMin_MouseEnter(object sender, MouseEventArgs e)
        {
            volm = "/minushov.png";
            setMinusimg();
        }

        private void volMin_MouseLeave(object sender, MouseEventArgs e)
        {
            volm = "/minus.png";
            setMinusimg();
        }

        private void Mute_MouseEnter(object sender, MouseEventArgs e)
        {
            muteon = "/muteonhov.png";
            muteoff = "/muteoffhov.png";
            if (isMute) setMute(muteon);
            else setMute(muteoff);
        }

        private void Mute_MouseLeave(object sender, MouseEventArgs e)
        {
            muteon = "/muteon.png";
            muteoff = "/muteoff.png";
            if (isMute) setMute(muteon);
            else setMute(muteoff);
        }

        private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragging = false;
            posTime = (slider.Value / 100) * musiclen;
            Console.WriteLine(posTime);
            seekPosition();
        }

        private void slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
            Console.WriteLine(sender);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            backButton.Visibility = Visibility.Visible;
            dispatcherTimer.Stop();
            listBoxqueue.Visibility = Visibility.Visible;
            showqueue.Visibility = Visibility.Hidden;
            coverartStop = 0;
            coverArt.Visibility = Visibility.Hidden;
            dispatcherTimer.Start();
            getplayqueue();
            ContextMenu RightClick = new ContextMenu();
            if (queueleng > 1)
            {
                MenuItem up = new MenuItem();
                up.Header = "一つ上に移動";
                up.Click += Up_Click;
                MenuItem down = new MenuItem();
                down.Header = "一つ下に移動";
                down.Click += Down_Click;
                MenuItem del = new MenuItem();
                del.Header = "再生キューから削除";
                del.Click += Del_Click;
                RightClick.Items.Add(up);
                RightClick.Items.Add(down);
                RightClick.Items.Add(del);
                listBoxqueue.ContextMenu = RightClick;
            }
        }

        private void Del_Click(object sender, RoutedEventArgs e)
        {
            updatePlaylist(listBoxqueue.SelectedIndex, "del");
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            updatePlaylist(listBoxqueue.SelectedIndex, "down");
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            updatePlaylist(listBoxqueue.SelectedIndex, "up");
        }

        private void listBoxqueue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = (ListBoxItem)listBoxqueue.ItemContainerGenerator.ContainerFromItem(listBoxqueue.SelectedItem);
            if (listBoxItem.InputHitTest(e.GetPosition(listBoxItem)) != null)
            {
                int item = listBoxqueue.SelectedIndex;
                var obj = new
                {
                    content_type = "track",
                    content_url = rawip + "contentplayer/v100/playqueue/tracks",
                    firstplay_index = item,
                    firstplay_trackid = queueData.tracks[item].trackid,
                    id = 1,
                    method = "playcontent",
                    play_type = "now",
                    repeat_mode = nowRepeat,
                    shuffle_mode = nowShuffle,
                    version = "1.1",
                };
                serializeJson(obj, "contentplayer/v100/operation", 0);

            }
        }

        private void button_Click_3(object sender, RoutedEventArgs e)
        {
            coverartStop = 1;
            listBoxqueue.Visibility = Visibility.Hidden;
            showqueue.Visibility = Visibility.Visible;
            backButton.Visibility = Visibility.Hidden;
            coverArt.Visibility = Visibility.Visible;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            double alid = int.Parse(nowalbumId.Replace("audio:album?id=", ""));
            nav.reqMusicId = alid;
            nav.req = 1;
            nav navig = new nav();
            NavigationService?.Navigate(navig);
        }
    }
}