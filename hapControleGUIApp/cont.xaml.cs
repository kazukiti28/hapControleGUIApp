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
using System.Threading.Tasks;
using System.Timers;
using System.Data.SQLite;
using SharpCifs.Smb;
using Microsoft.Win32;

namespace hapControlGUIApp
{
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
        public static int extinput = 0;
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
        static Timer queueTimer = new Timer();
        static Timer myTimer = new Timer();
        static double playlistModifiedVersion;
        static string playlistUri;
        static string power;
        public static int retur = 0;
        public static string db;
        public static string nowmusicId;

        public void hiddenall()
        {
            musicArtist.Visibility = Visibility.Hidden;
            coverArt.Visibility = Visibility.Hidden;
            prevButton.Visibility = Visibility.Hidden;
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
            musicName.Visibility = Visibility.Hidden;
            musicAlbum.Visibility = Visibility.Hidden;
            musicCodec.Visibility = Visibility.Hidden;
            minsec.Visibility = Visibility.Hidden;
            slider.Visibility = Visibility.Hidden;
            nextButton.Visibility = Visibility.Hidden;
            prevButton.Visibility = Visibility.Hidden;
            slider.Visibility = Visibility.Hidden;
            shuffle.Visibility = Visibility.Hidden;
            previmage.Visibility = Visibility.Hidden;
            nextimage.Visibility = Visibility.Hidden;
            startimage.Visibility = Visibility.Hidden;
            pimg.Visibility = Visibility.Hidden;
            mimg.Visibility = Visibility.Hidden;
            volIn.Visibility = Visibility.Hidden;
            browse.Visibility = Visibility.Hidden;
            muteimg.Visibility = Visibility.Hidden;
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
            muteimg.Visibility = Visibility.Visible;
        }

        void checkPower()
        {
            //var req = WebRequest.Create(ip + "contentplayer/v100/powerstate");
            //var res = req.GetResponse();

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            try
            {
                wc.DownloadStringCompleted += Wc_DownloadStringCompleted;
                wc.DownloadStringAsync(new Uri(ip + "contentplayer/v100/powerstate"));
            }
            catch { }
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            dynamic data = DynamicJson.Parse(e.Result);

            if (data.power_state == "off") power = "off";
            else power = "on";
        }

        void powerCont(string mode)
        {
            var obj = new
            {
                @params = new[] {
                        new
                        {
                            standbyDetail = "",
                            status = mode,
                        }
                    },
                method = "setPowerStatus",
                version = "1.1",
                id = 8,
            };
            serializeJson(obj, "system", 0);
        }

        public cont()
        {
            InitializeComponent();
            if (retur == 1)
            {
                dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 333);
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Start();
                showall();
            }
            isDragging = false;
            myDocument = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/HAPControlApp";
            DirectoryInfo di = new DirectoryInfo(myDocument);
            di.Create();
            browse.Visibility = Visibility.Visible;
            if (retur == 1)
            {
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
            }

            fst = 1;
            string bgurl = myDocument + "/bg_overlay.png";
            shabgurl = bgurl;
            Encoding utf = Encoding.GetEncoding("UTF-8");
            string fileName = myDocument + "/ipadd";
            if (File.Exists(fileName))
            {
                try
                {
                    ipaButton.Visibility = Visibility.Hidden;
                    StreamReader sr = new StreamReader(fileName, utf);
                    ip = sr.ReadLine();
                    sr.Close();
                    hostUrl = ip;
                    ipaddInput.Text = ip;
                    smbip = ip;
                    rawip = ip;
                    var req = WebRequest.Create(ip + "contentplayer/v100/powerstate");
                    var res = req.GetResponse();
                    dynamic data = DynamicJson.Parse(res.GetResponseStream());
                    if (data.power_state == "off") power = "off";
                    else power = "on";
                    if (power == "off")
                    {
                        if (MessageBox.Show("電源がOFFです。ONにしますか？", "Information", MessageBoxButton.YesNo,
                        MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            powerCont("active");
                        }
                        ProcessingSplash ps = new ProcessingSplash("起動待ちです。しばらくお待ちください…", () =>
                        {
                            System.Threading.Thread.Sleep(19000);
                        });
                        ps.ShowDialog();
                    }

                    setJunbi("getmusicinfo");
                    if (!File.Exists(myDocument + "/bg_overlay.png"))
                    {
                        WebClient wc = new WebClient();
                        string ipad = hostUrl.Replace(":60200/sony/", "");
                        string url = ipad + ":60100/img/bg_overlay.png";
                        wc.DownloadFile(url, bgurl);
                    }
                    if (retur != 1)
                    {
                        WebClient w = new WebClient();
                        string dbcon = hostUrl + "database/storage/hdd_browse.db";
                        db = myDocument + "/hdd_browse.db";
                        w.DownloadFile(dbcon, db);
                        w.Dispose();

                        
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            WebClient wec = new WebClient();
                            wec.DownloadFile(hostUrl.Replace(":60200/sony/", "") + ":60100/img/icon_input_optical.png", myDocument + "/optical.png");
                            wec.DownloadFile(hostUrl.Replace(":60200/sony/", "") + ":60100/img/icon_input_coaxial.png", myDocument + "/coaxial.png");
                            wec.DownloadFile(hostUrl.Replace(":60200/sony/", "") + ":60100/img/icon_input_analog.png", myDocument + "/line.png");
                            break;
                        }
                        catch { }
                    }
                    BitmapImage bgimg = new BitmapImage();
                    bgimg.BeginInit();
                    bgimg.UriSource = new Uri(bgurl);
                    bgimg.EndInit();
                    bgimage.Source = bgimg;

                    setRightimg();
                    setLeftimg();

                    setPlusimg();

                    setMinusimg();

                    /*dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 333);
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Start();*/

                    checkPower();
                    if (power != "off")
                    {
                        dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 333);
                        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                        dispatcherTimer.Start();
                    }
                    getplayqueue();

                    queueTimer.Enabled = true;
                    queueTimer.AutoReset = true;
                    queueTimer.Interval = 1500;
                    queueTimer.Elapsed += new ElapsedEventHandler(queueTime);

                    myTimer.Enabled = true;
                    myTimer.AutoReset = true;
                    myTimer.Interval = 500;
                    myTimer.Elapsed += new ElapsedEventHandler(Time);

                    ipaButton.Visibility = Visibility.Hidden;

                    ipadd.Foreground = new SolidColorBrush(Colors.White);
                }
                catch
                {
                    Console.WriteLine("IPアドレスが違います");
                    File.Delete(fileName);
                    ipaButton.Visibility = Visibility.Visible;
                    ipaddInput.Text = "http://";
                }
            }
        }


        void setMinusimg()
        {
            BitmapImage volminus = new BitmapImage();
            volminus.BeginInit();
            volminus.UriSource = new Uri("pack://application:,,,/Image" + volm);
            volminus.EndInit();
            mimg.Source = volminus;
        }

        void setPlusimg()
        {
            BitmapImage volplus = new BitmapImage();
            volplus.BeginInit();
            volplus.UriSource = new Uri("pack://application:,,,/Image" + volp);
            volplus.EndInit();
            pimg.Source = volplus;
        }

        void setRightimg()
        {
            BitmapImage next = new BitmapImage();
            next.BeginInit();
            next.UriSource = new Uri("pack://application:,,,/Image" + nextimg);
            next.EndInit();
            nextimage.Source = next;
        }

        void setCenterimg()
        {
            BitmapImage butimg = new BitmapImage();
            butimg.BeginInit();
            if (isPlayNow) butimg.UriSource = new Uri("pack://application:,,,/Image" + playimg);
            else butimg.UriSource = new Uri("pack://application:,,,/Image" + stopimg);
            butimg.EndInit();
            startimage.Source = butimg;
        }

        void setLeftimg()
        {
            BitmapImage prev = new BitmapImage();
            prev.BeginInit();
            prev.UriSource = new Uri("pack://application:,,,/Image" + previmg);
            prev.EndInit();
            previmage.Source = prev;
        }

        void updatePlaylist(int index, string mode)
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
            isPa = 0;
            serializeJson(obj, "avContent", 0);
        }
        static int first = 1;

        void dispatcherTimer_Tick(object sender, EventArgs e)
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
            var json = DynamicJson.Serialize(obj);
            string setUrl = hostUrl + "avContent";

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            try
            {
                wc.UploadStringCompleted += Wc_UploadStringComplete;
                wc.UploadStringAsync(new Uri(setUrl), json);

            }
            catch { }
        }
        void Wc_UploadStringComplete(object sender, UploadStringCompletedEventArgs e)
        {
            resDist = e.Result;
        }

        static dynamic resDist;

        public static int fst = 1;

        static int repoffradioLock;
        static int reponeradioLock;
        static int repallradioLock;
        static int shuoffradioLock;
        static int shualbradioLock;
        static int shuallradioLock;

        private void queueTime(object sender, EventArgs e)
        {
            checkPower();
            if (power == "off")
            {
                try
                {
                    dispatcherTimer.Stop();
                }
                catch { }
            }
            getplayqueue();
        }

        private void Time(object sender, EventArgs e)
        {
            if (resDist != null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    dynamic result = resDist;
                    dynamic data = DynamicJson.Parse(result);
                    string playmode = data.result[0].state;
                    if (playmode == "PAUSED")
                    {
                        isPlayNow = false;
                        if (first != 1) showall();
                        extinput = 0;
                    }
                    else if (playmode == "PLAYING")
                    {
                        isPlayNow = true;
                        if (first != 1) showall();
                        extinput = 0;
                    }
                    else if (playmode == "STOPPED" && data.result[0].uri != "")
                    {
                        string extType = data.result[0].uri;
                        extType = extType.Replace("extInput:", "");
                        string type = extType.Substring(0, 4);
                        if (type == "line")
                        {
                            extMode = "Line In " + extType.Substring(extType.Length - 1, 1) + "(外部入力中)";
                        }
                        else if (type == "coax")
                        {
                            extMode = "Coaxial In (外部入力中)";
                        }
                        else if (type == "opti")
                        {
                            extMode = "Optical In (外部入力中)";
                        }
                        extinput = 1;
                    }
                    if (data.result[0].uri == "") extinput = 2;
                    if (playmode != "STOPPED")
                    {
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
                        catch
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
                        posMin = posSec / 60;
                        posSec = posSec % 60;

                        nowRepeat = data.result[0].repeatType;
                        nowShuffle = data.result[0].shuffleType;
                    }
                    if (extinput == 0)
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
                        if (fst < 3)
                        {
                            if (nowRepeat == "all") repallradioLock = 1;
                            else if (nowRepeat == "off") repoffradioLock = 1;
                            else if (nowRepeat == "one") reponeradioLock = 1;
                            if (nowShuffle == "track") shuallradioLock = 1;
                            else if (nowShuffle == "off") shuoffradioLock = 1;
                            else if (nowShuffle == "album") shualbradioLock = 1;
                        }
                        if (nowRepeat == "all" && repallradioLock == 1/* || fst < 3*/)
                        {
                            repallradioLock = 0;
                            repall.IsChecked = true;
                        }
                        else if (nowRepeat == "off" && repoffradioLock == 1/* || fst < 3*/)
                        {
                            repoffradioLock = 0;
                            repoff.IsChecked = true;
                        }
                        else if (nowRepeat == "one" && reponeradioLock == 1/* || fst < 3*/)
                        {
                            reponeradioLock = 0;
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

                        if (nowShuffle == "track" && shuallradioLock == 1/* || fst < 3*/)
                        {
                            fst++;
                            allshu.IsChecked = true;
                            shuallradioLock = 0;
                        }
                        else if (nowShuffle == "off" && shuoffradioLock == 1/* || fst < 3*/)
                        {
                            fst++;
                            offshu.IsChecked = true;
                            shuoffradioLock = 0;
                        }
                        else if (nowShuffle == "album" && shualbradioLock == 1/* || fst < 3*/)
                        {
                            fst++;
                            albumshu.IsChecked = true;
                            shualbradioLock = 0;
                        }

                        setCenterimg();
                        if (!isDragging) slider.Value = ((posMin * 60 + posSec) / musiclen) * 100;

                        if (first == 1)
                        {
                            showall();
                            first = 0;
                        }
                    }
                    else if (extinput == 1)
                    {
                        hiddenall();
                        browse.Visibility = Visibility.Visible;
                        musicName.Text = extMode;
                        musicName.Visibility = Visibility.Visible;
                        dynamic getVolumeObj = getVolumeInfo();
                        serializeJson(getVolumeObj, "audio", 1);
                        volIn.Text = nowVolume.ToString();
                        volIn.Visibility = Visibility.Visible;
                        mimg.Visibility = Visibility.Visible;
                        pimg.Visibility = Visibility.Visible;
                        muteimg.Visibility = Visibility.Visible;
                        volPlu.Visibility = Visibility.Visible;
                        volMin.Visibility = Visibility.Visible;
                        Mute.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        musicArtist.Visibility = Visibility.Hidden;
                        coverArt.Visibility = Visibility.Hidden;
                        prevButton.Visibility = Visibility.Hidden;
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
                        musicName.Visibility = Visibility.Hidden;
                        musicAlbum.Visibility = Visibility.Hidden;
                        musicCodec.Visibility = Visibility.Hidden;
                        minsec.Visibility = Visibility.Hidden;
                        slider.Visibility = Visibility.Hidden;
                        nextButton.Visibility = Visibility.Hidden;
                        prevButton.Visibility = Visibility.Hidden;
                        slider.Visibility = Visibility.Hidden;
                        shuffle.Visibility = Visibility.Hidden;
                        previmage.Visibility = Visibility.Hidden;
                        nextimage.Visibility = Visibility.Hidden;
                        startimage.Visibility = Visibility.Hidden;
                        muteimg.Visibility = Visibility.Hidden;
                    }
                }));
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = ipaddInput.Text;
                ip = str;
                hostUrl = str + ":60200/sony/";
                var req = WebRequest.Create(hostUrl + "contentplayer/v100/powerstate");
                var res = req.GetResponse();
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
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Start();
            }
            catch
            {
                MessageBox.Show("入力されたIPアドレスの機器が見つかりませんでした。", "エラー", MessageBoxButton.YesNo, MessageBoxImage.Error);
            }
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
            mute.UriSource = new Uri("pack://application:,,,/Image" + name);
            mute.EndInit();
            muteimg.Source = mute;
        }

        async void connectHap(dynamic json, string url, int isNeedParse)//帰ってきたJSONが意味をなさないやつはこれ
        {
            string setUrl = hostUrl + url;
            Uri Url = new Uri(setUrl);

            isPa = isNeedParse;
            dynamic res = null;
            StringContent theContent = new StringContent(json);
            using (var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(5000) })
                await Task.Run(() =>
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    res = client.PostAsync(Url, theContent).Result.Content.ReadAsStringAsync().Result;
                });
            Wc_UploadStringCompleted1(res);
        }
        static int isPa;
        private void Wc_UploadStringCompleted1(dynamic e)
        {
            try
            {
                dynamic result = e;
                dynamic data = DynamicJson.Parse(result);
                if (data.id != 0)
                {
                    if (isPa == 1 && data.id != 1 && data.id != 2)//現在音量取得
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
                    else if (isPa == 2)//楽曲情報取得
                    {
                        string playmode = data.result[0].state;
                        if (playmode == "PAUSED")
                        {
                            isPlayNow = false;
                            if (first != 1) showall();
                            extinput = 0;
                        }
                        else if (playmode == "PLAYING")
                        {
                            isPlayNow = true;
                            if (first != 1) showall();
                            extinput = 0;
                        }
                        else if (playmode == "STOPPED" && data.result[0].uri != "")
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
                            else if (type == "opti")
                            {
                                extMode = "Optical In (外部入力中)";
                            }
                            extinput = 1;
                            return;
                        }
                        musiclen = data.result[0].durationSec;
                        playlistModifiedVersion = data.result[0].playlistModifiedVersion;
                        playlistUri = data.result[0].playlistUri;
                        nowalbumId = data.result[0].albumID;
                        nowmusicId = data.result[0].uri;
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
                        catch
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
                        posMin = posSec / 60;
                        posSec = posSec % 60;

                        nowRepeat = data.result[0].repeatType;
                        nowShuffle = data.result[0].shuffleType;
                    }
                    else if (isPa == 3)//ミュートチェック
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
            }
            catch { }
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
            isPa = isNeedParse;
            connectHap(json, url, isNeedParse);
        }

        void setJunbi(string mode)
        {
            string url = null;
            if (mode == "previous")
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
            if (mode == "poweron")
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
            if (mode == "poweroff")
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
            if (mode == "start" || mode == "stop")
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

        static dynamic queueres;

        async void getplayqueue()
        {
            if (extinput != 1)
            {
                try
                {
                    string setUrl = rawip + "contentplayer/v100/playqueue/tracks";

                    Uri Url = new Uri(setUrl);

                    using (var client = new HttpClient())
                        await Task.Run(() =>
                        {
                            client.DefaultRequestHeaders.ExpectContinue = false;
                            queueres = client.GetAsync(Url).Result.Content.ReadAsStringAsync().Result;
                        });

                    dynamic data = DynamicJson.Parse(queueres);
                    string totalfigure = data.paging.total.ToString();

                    setUrl = setUrl + "?offset=0&limit=" + totalfigure;

                    using (var clients = new HttpClient())
                        await Task.Run(() =>
                        {
                            clients.DefaultRequestHeaders.ExpectContinue = false;
                            queueres = clients.GetAsync(setUrl).Result.Content.ReadAsStringAsync().Result;
                        });

                    if (oldqueuedata == null) oldqueuedata = data;
                    queueData = DynamicJson.Parse(queueres);
                    if (oldqueuedata.ToString() != queueData.ToString())
                    {
                        int numberoftracks = (int)queueData.paging.total;
                        queueleng = numberoftracks;
                        string[] Tracks = new string[numberoftracks];
                        int[] Duration = new int[numberoftracks];
                        string[] Codec = new string[numberoftracks];
                        string[] Freq = new string[numberoftracks];
                        string[] Bitwidth = new string[numberoftracks];
                        string[] Bitrate = new string[numberoftracks];

                        for (int num = 0; num < numberoftracks; num++)
                        {
                            Tracks[num] = queueData.tracks[num].name;
                            Duration[num] = (int)queueData.tracks[num].duration;
                            Codec[num] = queueData.tracks[num].codec.codec_type;
                            Bitrate[num] = (queueData.tracks[num].codec.bit_rate / 1000).ToString();
                            double fr = (double)queueData.tracks[num].codec.sample_rate / 1000;
                            Freq[num] = fr.ToString();
                            Bitwidth[num] = queueData.tracks[num].codec.bit_width.ToString();
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
                        Dispatcher.Invoke(new Action(() =>
                        {
                            listBoxqueue.ItemsSource = TracksdataList;
                        }));
                    }
                    oldqueuedata = queueData;
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
            repallradioLock = 1;
            if (reponeradioLock != 1 && repoffradioLock != 1)
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
                repall.IsChecked = true;
            }
        }

        private void repone_Checked(object sender, RoutedEventArgs e)
        {
            reponeradioLock = 1;
            if (repoffradioLock != 1 && repallradioLock != 1)
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
                repone.IsChecked = true;
            }
        }

        private void repoff_Checked(object sender, RoutedEventArgs e)
        {
            repoffradioLock = 1;
            if (repallradioLock != 1 && reponeradioLock != 1)
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
                repoff.IsChecked = true;
            }
        }

        private void allshu_Checked(object sender, RoutedEventArgs e)
        {
            shuallradioLock = 1;
            if (shuoffradioLock != 1 && shualbradioLock != 1)
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
                allshu.IsChecked = true;
            }
        }

        private void albumshu_Checked(object sender, RoutedEventArgs e)
        {
            shualbradioLock = 1;
            if (shuallradioLock != 1 && shuoffradioLock != 1)
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
                albumshu.IsChecked = true;
            }
        }

        private void offshu_Checked(object sender, RoutedEventArgs e)
        {
            shuoffradioLock = 1;
            if (shuallradioLock != 1 && shualbradioLock != 1)
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
                offshu.IsChecked = true;
            }
        }

        private void Mute_OnClick(object sender, RoutedEventArgs e)
        {
            setJunbi("mute");
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            coverartStop = 1;
            dispatcherTimer.Stop();
            myTimer.Stop();
            queueTimer.Stop();
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

        private void leaved2(object sender, MouseEventArgs e)
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
            isPa = 0;
            isDragging = false;
            posTime = (slider.Value / 100) * musiclen;
            seekPosition();
        }

        private void slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            isPa = 0;
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
            int item = listBoxqueue.SelectedIndex;
            isPa = 0;
            if (item >= 0 && item < queueleng)
            {
                updatePlaylist(item, "del");
            }
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            int item = listBoxqueue.SelectedIndex;
            isPa = 0;
            if (item >= 0 && item < queueleng)
            {
                updatePlaylist(item, "down");
            }
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            int item = listBoxqueue.SelectedIndex;
            isPa = 0;
            if (item >= 0 && item < queueleng)
            {
                updatePlaylist(item, "up");
            }
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

        private void button_Click_5(object sender, RoutedEventArgs e)
        {
            if (powerbutton.Content.ToString() == "PowerOff")
            {
                powerCont("off");
                powerbutton.Content = "PowerOn";
                hiddenall();
                dispatcherTimer.Stop();
                queueTimer.Stop();
                myTimer.Stop();
            }
            else if (powerbutton.Content.ToString() == "PowerOn")
            {
                powerCont("active");
                powerbutton.Content = "PowerOff";
                ProcessingSplash ps = new ProcessingSplash("起動待ちです。しばらくお待ちください…", () =>
                {
                    System.Threading.Thread.Sleep(19000);
                });
                ps.ShowDialog();
                showall();
                dispatcherTimer.Start();
                queueTimer.Start();
                myTimer.Start();
            }
        }
        public static string smbip = "";
        private void sambaconnect_Click(object sender, RoutedEventArgs e)
        {
            string tid = nowmusicId.Replace("audio:track?id=", "");

            SQLiteConnection connection = new SQLiteConnection("Data Source=" + db);
            connection.Open();
            SQLiteCommand cmd = connection.CreateCommand();
            string getFilename = "SELECT ft0002.prop7007 from ft0002 where prop3601 = " + tid;
            cmd.CommandText = getFilename;//ファイル名のみ
            cmd.Parameters.Add(new SQLiteParameter(System.Data.DbType.String, tid));
            cmd.Prepare();
            SQLiteDataReader reader = cmd.ExecuteReader();
            string trackfilename = "";
            while (reader.Read())
            {
                trackfilename = (string)reader[0];
            }
            reader.Close();

            string direct = "select prop7023 from ft0000 where prop3601 = (select prop3006 from ft0002 where prop3601 = " + tid + ")";//ディレクトリの素(よくわからん奴)出てくる
            cmd.CommandText = direct;
            cmd.Parameters.Add(new SQLiteParameter(System.Data.DbType.String, tid));
            cmd.Prepare();
            string getDirectry = "";
            SQLiteDataReader readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                getDirectry = (string)readers[0];//数字ベースの解析元になるやつ
            }
            string editDir = "";//出てきたフォルダ名前の格納
            readers.Close();
            int pathLen = 0;
            pathLen = getDirectry.Length - 1;
            smbip = smbip.Replace("http://", "");
            smbip = smbip.Replace(":60200/sony/", "");
            string source = @"\\"+smbip+@"\HAP_Internal\";

            if (getDirectry != "") {//直下ならnullになるはず
                while (pathLen != 0) {
                    int count = 0;
                    getDirectry = getDirectry.Remove(0,1);
                    pathLen -= 1;
                    for (; ; )
                    {
                        if (char.IsDigit(getDirectry, count))
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    string sqlTarget = getDirectry.Substring(0, count);//albumIDを出す
                    cmd.CommandText = "select prop7020 from ft0000 where prop3601 = " + sqlTarget;//格納フォルダを出す
                    cmd.Parameters.Add(new SQLiteParameter(System.Data.DbType.String, sqlTarget));
                    cmd.Prepare();
                    SQLiteDataReader reader2 = cmd.ExecuteReader();
                    while (reader2.Read())
                    {
                        editDir = (string)reader2[0];
                    }
                    reader2.Close();
                    source += editDir + @"\";//どんどんフォルダ名を追加していく
                    pathLen -= count;//長さを減す
                    getDirectry = getDirectry.Substring(count);//抽出した数字を消す(/は残す)
                }
            } else
            {
                //HAP_Internal直下の場合
            }
            
            //double alid = int.Parse(nowalbumId.Replace("audio:album?id=", "")); --アルバムidが出る
            string dst = myDocument + "/" + trackfilename;
            string src = source+ trackfilename;
            //string terst = @"\\192.168.0.7\HAP_Internal\test.flac";
            File.Copy(src, dst, true);
        }
    }
}