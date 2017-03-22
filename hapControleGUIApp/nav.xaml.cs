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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace hapControlGUIApp
{
    /// <summary>
    /// nav.xaml の相互作用ロジック
    /// </summary>
    ///
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
        static string totalfigure = null;//合計アルバム数が入る
        static dynamic allalbumdata;
        private static int i;
        private static bool mute;
        dynamic AlbumData;
        private dynamic[,] FileName;
        DispatcherTimer dispatcherTimer;
        private dynamic[,] head;
        private dynamic dict = new Dictionary<string, int>();
        static dynamic[] playlistid;
        static int playlistLoaded;
        static double playlistModifiedVersion;
        static string playlistUri;
        static int playlistfigure;
        static int addplaylistmusicid;
        public static int req;
        public static double reqMusicId;
        public static int albumChacked = 0;
        static int setmusicId = 0;

        public nav()
        {
            InitializeComponent();
            myDocument = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/HAPControlApp";
            DirectoryInfo di = new DirectoryInfo(myDocument);
            di.Create();
            musicName.Text = cont.nowPlaying;
            musicAlbum.Text = cont.album;
            volIn.Text = cont.nowVolume.ToString();
            BG.Background = new SolidColorBrush(Color.FromArgb(Convert.ToByte(cont.a, 16), Convert.ToByte(cont.r, 16), Convert.ToByte(cont.g, 16), Convert.ToByte(cont.b, 16)));
            BitmapImage bgimg = new BitmapImage();
            bgimg.BeginInit();
            bgimg.UriSource = new Uri(cont.shabgurl);
            bgimg.EndInit();
            bgimage.Source = bgimg;
            
            playlistLoaded = 0;

            ipaddInput.Text = cont.ip;
            ipadd.Foreground = new SolidColorBrush(Colors.White);
            if (albumChacked == 0)
            {
                Console.WriteLine("getCall");
                getAllAlbumInfo();
                albumChacked = 1;
            }
            
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
            setMinusimg();
            setPlusimg();
            setLeftimg();
            setCenterimg();
            setRightimg();

            
            LoadListItems();
            playlistBackButton.Visibility = Visibility.Hidden;
            if (req == 1)
            {
                searchNum();
                req = 0;
                reqMusicId = -1;
                playlistView.Visibility = Visibility.Visible;
                req = 0;
            }
        }

        void searchNum()
        {
            for (int i = 0; i < allalbumdata.paging.total ; i++)
            {
                if (reqMusicId == allalbumdata.albums[i].albumid) DisplayAlbumInfo(i);
            }
        
        }

        void setMinusimg()
        {
            BitmapImage volminus = new BitmapImage();
            volminus.BeginInit();
            volminus.UriSource = new Uri("pack://application:,,,/Image" + cont.volm);
            volminus.EndInit();
            mimg.Source = volminus;
        }

        void setMuteimg(string name)
        {
            BitmapImage mute = new BitmapImage();
            mute.BeginInit();
            mute.UriSource = new Uri("pack://application:,,,/Image" + name);
            mute.EndInit();
            muteimg.Source = mute;
        }

        void setPlusimg()
        {
            BitmapImage volplus = new BitmapImage();
            volplus.BeginInit();
            volplus.UriSource = new Uri("pack://application:,,,/Image" + cont.volp);
            volplus.EndInit();
            pimg.Source = volplus;
        }

        void setRightimg()
        {
            BitmapImage next = new BitmapImage();
            next.BeginInit();
            next.UriSource = new Uri("pack://application:,,,/Image" + cont.nextimg);
            next.EndInit();
            nextimage.Source = next;
        }

        void setCenterimg()
        {
            BitmapImage butimg = new BitmapImage();
            butimg.BeginInit();
            if (cont.isPlayNow) butimg.UriSource = new Uri("pack://application:,,,/Image" + cont.playimg);
            else butimg.UriSource = new Uri("pack://application:,,,/Image" + cont.stopimg);
            butimg.EndInit();
            startimage.Source = butimg;
        }

        void setLeftimg()
        {
            BitmapImage prev = new BitmapImage();
            prev.BeginInit();
            prev.UriSource = new Uri("pack://application:,,,/Image" + cont.previmg);
            prev.EndInit();
            previmage.Source = prev;
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            
            setJunbi("getmusicinfo");
            if (!cont.extinput)
            {
                BG.Background =
                    new SolidColorBrush(Color.FromArgb(Convert.ToByte(a, 16), Convert.ToByte(r, 16),
                        Convert.ToByte(g, 16), Convert.ToByte(b, 16)));
                serializeJson(getVolumeInfo(), "audio", 1);
                downloadCoverArt();
                if (setmusicId != 0)
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(nowMusicCover);
                    img.EndInit();
                    coverArt.Source = img;
                }
                musicName.Text = nowPlaying;
                musicAlbum.Text = album;
                volIn.Text = nowVolume.ToString();
                setCenterimg();
                nextButton.Visibility = Visibility.Visible;
                nextimage.Visibility = Visibility.Visible;
                startButton.Visibility = Visibility.Visible;
                startimage.Visibility = Visibility.Visible;
                prevButton.Visibility = Visibility.Visible;
                previmage.Visibility = Visibility.Visible;
            } else
            {
                musicName.Text = cont.extMode;
                dynamic getVolumeObj = getVolumeInfo();
                serializeJson(getVolumeObj, "audio", 1);
                volIn.Text = nowVolume.ToString();
                nextButton.Visibility = Visibility.Hidden;
                nextimage.Visibility = Visibility.Hidden;
                startButton.Visibility = Visibility.Hidden;
                startimage.Visibility = Visibility.Hidden;
                prevButton.Visibility = Visibility.Hidden;
                previmage.Visibility = Visibility.Hidden;
            }
        }

        void getAllAlbumInfo()
        {
            string setUrl = cont.rawip + "contentdb/v100/audio/albums";

            var req = WebRequest.Create(setUrl);
            var res = req.GetResponse();

            dynamic data = DynamicJson.Parse(res.GetResponseStream());
            totalfigure = data.paging.total.ToString();

            setUrl = setUrl + "?offset=0&limit=" + totalfigure;
            req = WebRequest.Create(setUrl);
            res = req.GetResponse();
            allalbumdata = DynamicJson.Parse(res.GetResponseStream());//これにたくさん入っている
            allalbumdata.albums[0].album_artist.name = "不明なアーティスト";
            allalbumdata.albums[0].name = "不明なアルバム";
        }

        public List<VisibleItem> dataList { get; set; }

        public List<VisibleItem> TracksdataList { get; set; }

        public List<VisibleItem> extList { get; set; }

        public List<string> playlistarr = new List<string>();

        private void LoadListItems()
        {
            status = "AlbumInfo Loading…";
            ProcessingSplash ps = new ProcessingSplash(status, () =>
            {
                dataList = new List<VisibleItem>();
                dataList = getDataList();
            });
            ps.ShowDialog();
            ListBoxConverter.ItemsSource = dataList;
            ListBoxConverter.DataContext = this;
        }
        static string status;

        private List<VisibleItem> getDataList()
        {
            dispatcherTimer.Stop(); //情報定期取得一時停止

            VisibleItem vItem;
            vItem = new VisibleItem();
            //albums[0]のみ例外
            vItem.CoverArt = myDocument + "/" + "default.png";
            vItem.ArtistName = "不明なアーティスト";
            vItem.AlbumName = "不明なアルバム";
            vItem.TrackFigure = "トラック:" + allalbumdata.albums[0].number_of_tracks.ToString();
            vItem.TracksUrl = allalbumdata.albums[0].tracks_url;
            dataList.Add(vItem);
            head = new dynamic[26, 2];

            string h, tmp;
            string prev = "";
            int cnt = 0;
            
                for (i = 1; i < (int)allalbumdata.paging.total; i++)
                {
                    try
                    {
                        noCoverArt = 0;
                        coverArtUrl = allalbumdata.albums[i].image.url;
                        int findsla = coverArtUrl.LastIndexOf("/") + 1;
                        musicId = coverArtUrl.Substring(findsla);
                    }
                    catch
                    {
                        noCoverArt = 1;
                    }
                    status = "Downloading Artwork…";
                    downloadCoverArt();

                    vItem = new VisibleItem();
                    vItem.CoverArt = nowMusicCover;
                    vItem.ArtistName = allalbumdata.albums[i].album_artist.name;
                    vItem.AlbumName = allalbumdata.albums[i].name;
                    vItem.TrackFigure = "トラック:" + allalbumdata.albums[i].number_of_tracks.ToString();
                    vItem.TracksUrl = allalbumdata.albums[0].tracks_url;
                    dataList.Add(vItem);
                    h = allalbumdata.albums[i].name;
                    if (new Regex("^[0-9a-zA-Z]+$").IsMatch(h.Substring(0, 1)))
                    {
                        Combox item;
                        h = allalbumdata.albums[i].name;
                        if (h.Length > 4)
                        {
                            tmp = h.Substring(0, 4);
                            if (tmp.ToUpper() == "THE ") h = h.Remove(0, 4);
                        }

                        if (i != 1 && prev.Substring(0, 1).ToLower() != h.Substring(0, 1).ToLower())
                        {
                            h = h.Substring(0, 1);
                            item = new Combox();
                            item.headChara = h.ToUpper();
                            item.suf = i;
                            //comboBox.Items.Add(item);
                            head[cnt, 0] = h;
                            head[cnt, 1] = i;
                            cnt++;
                        }
                    }
                    prev = h;
                }
            musicId = null;
            dispatcherTimer.Start(); //再開
            return dataList;
        }

        void getPlaylist()
        {
            string url = cont.ip + "contentdb/v100/audio/playlists";
            dynamic playlist = DynamicJson.Parse(gettrackinfo(url));
            double cnt = playlist.paging.total;
            url = url + "?offset=0&limit=" + cnt.ToString();
            playlist = DynamicJson.Parse(gettrackinfo(url));
            VisibleItem tracklist;
            TracksdataList = new List<VisibleItem>();
            playlistid = new dynamic[(int)cnt];
            for (int i = 0; i < cnt; i++) {
                tracklist = new VisibleItem();
                string name = playlist.playlists[i].name;
                if (name == "Newly Added") name = "新しく追加した曲";
                else if (name == "Most Played") name = "再生回数の多い曲";
                else if (name == "Least Played") name = "再生回数の少ない曲";
                else if (name == "Recently Played") name = "最近再生した曲";
                tracklist.TrackName = name;
                playlistid[i] = playlist.playlists[i].playlistid;
                TracksdataList.Add(tracklist);
                playlistarr.Add(name);
            }
            playlistfigure = (int)cnt;
            ListBoxConverter.Visibility = Visibility.Hidden;
            ListBoxTrack.Visibility = Visibility.Hidden;
            playlistView.Visibility = Visibility.Visible;
            playlistView.ItemsSource = TracksdataList;
        }

        public List<string> namearr = new List<string>();

        void DisplayAlbumInfo(int number)
        {
            string AlbumName = allalbumdata.albums[number].name; //アルバム名の取得
            int numberoftracks = (int)allalbumdata.albums[number].number_of_tracks; //アルバム収録曲数の取得
            string[] Tracks = new string[numberoftracks]; //曲の名前を格納する配列
            int[] Duration = new int[numberoftracks]; //曲長さ格納
            string[] Codec = new string[numberoftracks]; //コーデック格納
            string[] Freq = new string[numberoftracks]; //サンプリング周波数
            string[] Bitwidth = new string[numberoftracks]; //サンプリング周波数
            string[] Bitrate = new string[numberoftracks];
            FileName = new dynamic[numberoftracks, 2];

            AlbumData = DynamicJson.Parse(gettrackinfo(allalbumdata.albums[number].tracks_url)); //トラック情報の取得
            numberoftracks = (int)AlbumData.paging.total;
            string urlnew = allalbumdata.albums[number].tracks_url + "?offset=0&limit=" + numberoftracks.ToString();

            AlbumData = DynamicJson.Parse(gettrackinfo(urlnew)); //トラック情報の取得

            for (int num = 0; num < numberoftracks; num++)
            {
                Tracks[num] = AlbumData.tracks[num].name; //すべての名前の取得
                Duration[num] = (int)AlbumData.tracks[num].duration; //曲長さ(秒)
                Codec[num] = AlbumData.tracks[num].codec.codec_type; //コーデック
                Bitrate[num] = (AlbumData.tracks[num].codec.bit_rate / 1000).ToString(); //ビットレート
                int fr = (int)AlbumData.tracks[num].codec.sample_rate / 1000;
                Freq[num] = fr.ToString(); //サンプリング周波数
                Bitwidth[num] = AlbumData.tracks[num].codec.bit_width.ToString(); //ビット深度
                FileName[num, 0] = AlbumData.tracks[num].filename;
                FileName[num, 1] = num;
            }
            string info = "";
            string min = "";
            string sec = "";
            string dur = "";

            ListBoxConverter.Visibility = Visibility.Hidden;
            ListBoxTrack.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;

            TracksdataList = new List<VisibleItem>();
            VisibleItem tracklist;

            for (int i = 0; i < FileName.Length / 2 - 1; i++)
            {
                for (int k = FileName.Length / 2 - 1; k > i; k--)
                {
                    if (FileName[k, 0].CompareTo(FileName[k - 1, 0]) < 0)
                    {
                        dynamic tmp = FileName[k, 0];
                        FileName[k, 0] = FileName[k - 1, 0];
                        FileName[k - 1, 0] = tmp;

                        tmp = FileName[k, 1];
                        FileName[k, 1] = FileName[k - 1, 1];
                        FileName[k - 1, 1] = tmp;
                    }
                }
            }


            for (int cnt = 0; cnt < numberoftracks; cnt++)
            {
                min = (Duration[FileName[cnt, 1]] / 60).ToString();
                if (Duration[FileName[cnt, 1]] % 60 < 10)
                {
                    sec = (Duration[FileName[cnt, 1]] % 60).ToString();
                    sec = "0" + sec;
                }
                else
                    sec = (Duration[FileName[cnt, 1]] % 60).ToString();
                dur = min + ":" + sec;

                if (Codec[FileName[cnt, 1]] == "alac" || Codec[FileName[cnt, 1]] == "flac" || Codec[FileName[cnt, 1]] == "aiff" || Codec[FileName[cnt, 1]] == "wav")//可逆圧縮/非圧縮
                {
                    info = Codec[FileName[cnt, 1]].ToUpper() + " " + Freq[FileName[cnt, 1]] + "kHz" + "/" + Bitwidth[FileName[cnt, 1]] + "bit  " + dur;
                }
                else if (Codec[FileName[cnt, 1]] == "dsd" || Codec[FileName[cnt, 1]] == "dsf")//DSD
                {
                    info = Codec[FileName[cnt, 1]].ToUpper() + " " + Freq[FileName[cnt, 1]] + "MHz  " + dur;
                }
                else//圧縮音源
                {
                    info = Codec[FileName[cnt, 1]].ToUpper() + " " + Bitrate[FileName[cnt, 1]] + "kbps  " + dur;
                }
                tracklist = new VisibleItem();

                tracklist.ArtistName = "   " + AlbumData.tracks[FileName[cnt, 1]].artist.name;
                tracklist.TrackName = (cnt + 1).ToString() + "  " + AlbumData.tracks[FileName[cnt, 1]].name;
                tracklist.TrackInfo = info;
                tracklist.ContentUrl = AlbumData.tracks[FileName[cnt, 1]].album.url;
                tracklist.MusicId = (AlbumData.tracks[FileName[cnt, 1]].trackid).ToString();
                TracksdataList.Add(tracklist);
                info = "";
                namearr.Add((AlbumData.tracks[FileName[cnt, 1]].trackid).ToString());
            }
            ListBoxTrack.ItemsSource = TracksdataList;
            ContextMenu RightClick = new ContextMenu();
            MenuItem next = new MenuItem();
            next.Header = "再生キューの次曲に追加";
            next.Click += Next_Click;
            MenuItem last = new MenuItem();
            last.Header = "再生キューの最後に追加";
            last.Click += Last_Click;
            MenuItem addp = new MenuItem();
            addp.Header = "プレイリストに追加";
            addp.Click += Addp_Click;
            RightClick.Items.Add(next);
            RightClick.Items.Add(last);
            RightClick.Items.Add(addp);
            ListBoxTrack.ContextMenu = RightClick;
        }

        public List<string> originalVerArr = new List<string>();
        public List<string> playlistIdArr = new List<string>();

        private void Addp_Click(object sender, RoutedEventArgs e)
        {
            int index = ListBoxTrack.SelectedIndex;
            addplaylistmusicid = int.Parse(namearr[index]);
            
            ListBoxTrack.Visibility = Visibility.Hidden;
            string url = cont.ip + "contentdb/v100/audio/playlists";
            dynamic playlist = DynamicJson.Parse(gettrackinfo(url));
            double cnt = playlist.paging.total;
            url = url + "?offset=0&limit=" + cnt.ToString();
            playlist = DynamicJson.Parse(gettrackinfo(url));
            VisibleItem tracklist;
            TracksdataList = new List<VisibleItem>();
            playlistid = new dynamic[(int)cnt];
            for (int i = 4; i < cnt; i++)
            {
                tracklist = new VisibleItem();
                string name = playlist.playlists[i].name;
                tracklist.TrackName = name;
                TracksdataList.Add(tracklist);
                originalVerArr.Add((playlist.playlists[i].version).ToString());
                playlistIdArr.Add((playlist.playlists[i].playlistid).ToString());
            }
            playlistselectBox.ItemsSource = TracksdataList;
            playlistselectBox.Visibility = Visibility.Visible;
        }

        private void playlistselectBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ListBoxTrack.SelectedIndex + 4;
            ListBoxTrack.Visibility = Visibility.Visible;
            var obj = new
            {
                data = "types=4&trackIds="  + addplaylistmusicid.ToString()  + "&positions=0",
                @params = new[] {
                        new
                        {
                            uri = "database:list?id=" + playlistIdArr[index] + "&originalVersion=" + originalVerArr[index],
                        }
                    },
                method = "updatePlaylist",
                version = "1.0",
                id = 2,
            };
            serializeJson(obj, "avContent", 0);
            playlistselectBox.Visibility = Visibility.Hidden;
            playlistLoaded = 0;
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            int index = ListBoxTrack.SelectedIndex;
            addQueue(namearr[index], "last");
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            int index = ListBoxTrack.SelectedIndex;
            addQueue(namearr[index], "next");
        }

        dynamic gettrackinfo(string url)
        {
            var req = WebRequest.Create(url);
            var res = req.GetResponse();
            dynamic data = res.GetResponseStream();
            return data;
        }

        void addQueue(string trackid, string mode)
        {
            string mod = playlistModifiedVersion.ToString();
            string par;
            if (mode == "next") par = "types=3&trackIds=";
            else par = "types=4&trackIds=";
            string url;
            var obj = new
            {
                data = par + trackid + "&positions=0",
                @params = new[] {
                        new
                        {
                            uri = "audio:list?id=" + playlistUri.Replace("audio:playinglist?id=","")
                            + "&originalVersion=" + mod,
                        }
                    },
                method = "updatePlaylist",
                version = "1.0",
                id = 2,
            };
            url = "avContent";
            serializeJson(obj, url, 0);
        }

        void downloadCoverArt()
        {
            if (musicId == null)
            {
                musicId = "NULL";
                noCoverArt = 1;
            }
            WebClient wc = new WebClient();
            if (musicId != null)
            {
                nowMusicCover = myDocument + "/" + musicId;//現在のカバーアートのローカルアドレス
                if (musicId.IndexOf(".") == -1 && noCoverArt != 1)
                {
                    nowMusicCover = nowMusicCover + ".jpg";
                    if (!File.Exists(nowMusicCover))
                    {
                        wc.DownloadFile(coverArtUrl, nowMusicCover);
                    }
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
                mute = false;
            }
            else
            {
                str = "on";
                Mute.Content = "Mute On";
                mute = true;
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
            ConnectHap(json, url, isNeedParse);
        }

        async void ConnectHap(dynamic json, string url, int isNeedParse)
        {
            HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(2) };
            string setUrl = cont.hostUrl + url;
            Uri Url = new Uri(setUrl);
            client.DefaultRequestHeaders.ExpectContinue = false;
            StringContent theContent = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            dynamic result = null;
            await Task.Run(() =>
            {
                result = client.PostAsync(Url, theContent).Result.Content.ReadAsStringAsync().Result;
            });

            dynamic data = DynamicJson.Parse(result);
            if (isNeedParse == 1)//現在音量取得
            {
                nowVolume = (int)data.result[0].volume;
                if (data.result[0].mute == "on")
                {
                    setMuteimg(cont.muteon);
                    Mute.Content = "Mute On";
                }
                if (data.result[0].mute == "off")
                {
                    setMuteimg(cont.muteoff);
                    Mute.Content = "Mute Off";
                }
            }
            else if (isNeedParse == 2)//楽曲情報取得
            {
                string playmode = data.result[0].state;
                if (playmode == "PAUSED")
                {
                    cont.isPlayNow = false;
                }
                else if (playmode == "PLAYING")
                {
                    cont.isPlayNow = true;
                }
                else if (playmode == "STOPPED")
                {
                    try {
                        string extType = data.result[0].uri;
                        extType = extType.Replace("extInput:", "");
                        string type = extType.Substring(0, 4);
                        if (type == "line")
                        {
                            cont.extMode = "Line In " + extType.Substring(extType.Length - 1, 1) + "(外部入力)";
                        }
                        else if (type == "coax")
                        {
                            cont.extMode = "Coaxial In (外部入力中)";
                        }
                        else if (type == "opti")
                        {
                            cont.extMode = "Optical In (外部入力中)";
                        }
                        cont.extinput = true;
                    }
                    catch
                    {
                        cont.extMode = "";
                    }
                    return;
                }
                cont.musiclen = data.result[0].durationSec;
                playlistModifiedVersion = data.result[0].playlistModifiedVersion;
                playlistUri = data.result[0].playlistUri;

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
                cont.codec = data.result[0].audioInfo[0].codec;
                cont.bandwidth = data.result[0].audioInfo[0].bandwidth;
                cont.bitrate = (double.Parse(data.result[0].audioInfo[0].bitrate)) / 1000;
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
                    setmusicId = 1;
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

                cont.nowRepeat = data.result[0].repeatType;
                cont.nowShuffle = data.result[0].shuffleType;
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
            cont.fst = 1;
            dispatcherTimer.Stop();
            cont.retur = 1;
            cont con = new cont();
            NavigationService?.Navigate(con);
        }

        private void listClick(object sender, RoutedEventArgs e)
        {
            int item = ListBoxConverter.SelectedIndex;
            DisplayAlbumInfo(item);
        }

        private void TrackClick(object sender, RoutedEventArgs e)
        {
            int item = ListBoxTrack.SelectedIndex;
            dynamic index = FileName[item, 1];
            dynamic trackid = AlbumData.tracks[index].trackid;
            dynamic albumid = AlbumData.tracks[index].album.albumid;
            string contenturl = cont.hostUrl + "contentdb/v100/audio/tracks?albumid=" + albumid;

            var playSelectedMusic = new
            {
                content_type = "track",
                content_url = contenturl,
                firstplay_trackid = trackid,
                id = 1,
                method = "playcontent",
                play_type = "now",
                repeat_mode = cont.nowRepeat,
                shuffle_mode = cont.nowShuffle,
                version = "1.1",
            };
            serializeJson(playSelectedMusic, "contentplayer/v100/operation", 0);
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            ListBoxTrack.Visibility = Visibility.Hidden;
            ListBoxConverter.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Hidden;
        }

        private void HeadChanged(object sender, RoutedEventArgs e)
        {
            /*int ind = head[comboBox.SelectedIndex, 1];
            
            ListBoxTrack.ScrollIntoView(allalbumdata.albums[ind].name);*/

            /*VisibleItem s = new VisibleItem();
            ListBoxTrack.ScrollIntoView(dataList[head[comboBox.SelectedIndex, 1]].ToString());*/
        }

        static dynamic playlistData;
        static int selectedPlaylist;

        private void playlistView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = (ListBoxItem)playlistView.ItemContainerGenerator.ContainerFromItem(playlistView.SelectedItem);
            // ヒットテストでアイテム上
            if (listBoxItem.InputHitTest(e.GetPosition(listBoxItem)) != null)
            {
                int item = playlistView.SelectedIndex;
                selectedPlaylist = (int)playlistid[item];

                string setUrl = cont.ip + "contentdb/v100/audio/playlists/" + playlistid[item] + "/tracks";

                var req = WebRequest.Create(setUrl);
                var res = req.GetResponse();

                playlistData = DynamicJson.Parse(res.GetResponseStream());
                string totalfigure = playlistData.paging.total.ToString();
                setUrl = setUrl + "?offset=0&limit=" + totalfigure;
                req = WebRequest.Create(setUrl);
                res = req.GetResponse();
                playlistData = DynamicJson.Parse(res.GetResponseStream());
                TracksdataList = new List<VisibleItem>();
                VisibleItem tracklist;
                string min = "";
                string sec = "";
                string info = "";
                string dur = "";

                playlistTrackid = new int[(int)playlistData.paging.total];

                for (int cnt = 0; cnt < playlistData.paging.total; cnt++)
                {
                    min = ((int)playlistData.tracks[cnt].duration / 60).ToString();
                    if ((int)playlistData.tracks[cnt].duration % 60 < 10)
                    {
                        sec = ((int)playlistData.tracks[cnt].duration % 60).ToString();
                        sec = "0" + sec;
                    }
                    else
                        sec = ((int)playlistData.tracks[cnt].duration % 60).ToString();
                    dur = min + ":" + sec;

                    if (playlistData.tracks[cnt].codec.codec_type == "alac" || playlistData.tracks[cnt].codec.codec_type == "flac" || playlistData.tracks[cnt].codec.codec_type == "aiff" || playlistData.tracks[cnt].codec.codec_type == "wav")//可逆圧縮/非圧縮
                    {
                        info = playlistData.tracks[cnt].codec.codec_type.ToUpper() + " " + playlistData.tracks[cnt].codec.sample_rate / 1000 + "kHz" + "/" + playlistData.tracks[cnt].codec.bit_width + "bit  " + dur;
                    }
                    else if (playlistData.tracks[cnt].codec.codec_type == "dsd" || playlistData.tracks[cnt].codec.codec_type == "dsf")//DSD
                    {
                        info = playlistData.tracks[cnt].codec.codec_type.ToUpper() + " " + playlistData.tracks[cnt].codec.sample_rate / 1000000 + "MHz  " + dur;
                    }
                    else//圧縮音源
                    {
                        info = playlistData.tracks[cnt].codec.codec_type.ToUpper() + " " + playlistData.tracks[cnt].codec.bit_rate / 1000 + "kbps  " + dur;
                    }
                    tracklist = new VisibleItem();

                    tracklist.ArtistName = playlistData.tracks[cnt].artist.name;
                    tracklist.TrackName = playlistData.tracks[cnt].name;
                    tracklist.TrackInfo = info;
                    tracklist.ContentUrl = playlistData.tracks[cnt].album.url;
                    tracklist.MusicId = (playlistData.tracks[cnt].trackid).ToString();
                    TracksdataList.Add(tracklist);
                    info = "";
                }
                playlistTracks.Visibility = Visibility.Visible;
                playlistView.Visibility = Visibility.Hidden;
                playlistTracks.ItemsSource = TracksdataList;
            }
        }
        static int[] playlistTrackid;

        private void extbutton_Click(object sender, RoutedEventArgs e)
        {
            extList = new List<VisibleItem>();
            VisibleItem vItem;
            string[] type = new string[4] {"optical.png","coaxial.png","line.png","line.png", };
            string[] typeName = new string[4] { "Optical In", "Coaxial In", "Line in 1", "Line in 2", };
            for(i = 0;i < 4;i++)
            {
                vItem = new VisibleItem();
                vItem.CoverArt = myDocument + "/" + type[i];
                vItem.AlbumName = typeName[i];
                extList.Add(vItem);
            }
            extBox.ItemsSource = extList;
            extBox.Visibility = Visibility.Visible;
            playlistBackButton.Visibility = Visibility.Hidden;
            playlistTracks.Visibility = Visibility.Hidden;
            playlistView.Visibility = Visibility.Hidden;
            ListBoxTrack.Visibility = Visibility.Hidden;
            ListBoxConverter.Visibility = Visibility.Hidden;
            BackButton.Visibility = Visibility.Hidden;
            albumlistBackButton_Copy.Visibility = Visibility.Hidden;
        }


        private void playlistbutton_Click(object sender, RoutedEventArgs e)
        {
            playlistBackButton.Visibility = Visibility.Visible;
            playlistTracks.Visibility = Visibility.Hidden;
            playlistView.Visibility = Visibility.Visible;
            ListBoxTrack.Visibility = Visibility.Hidden;
            ListBoxConverter.Visibility = Visibility.Hidden;
            extBox.Visibility = Visibility.Hidden;
            albumlistBackButton_Copy.Visibility = Visibility.Hidden;
            if (playlistLoaded != 1)
            {
                getPlaylist();
                playlistLoaded = 1;
            }
        }

        private void playlistTracks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = (ListBoxItem)playlistTracks.ItemContainerGenerator.ContainerFromItem(playlistTracks.SelectedItem);
            // ヒットテストでアイテム上
            if (listBoxItem.InputHitTest(e.GetPosition(listBoxItem)) != null)
            {

                int item = playlistTracks.SelectedIndex;
                var obj = new
                {
                    content_type = "t",
                    content_url = cont.ip + "contentdb/v100/audio/playlists/" + selectedPlaylist + "/tracks",
                    firstplay_index = item,
                    firstplay_trackid = playlistData.tracks[item].trackid,
                    id = 1,
                    method = "playcontent",
                    play_type = "now",
                    repeat_mode = cont.nowRepeat,
                    shuffle_mode = cont.nowShuffle,
                    version = "1.1",
                };
                serializeJson(obj, "contentplayer/v100/operation", 0);
            }
        }

        private void playlistBackButton_Click(object sender, RoutedEventArgs e)
        {
            playlistTracks.Visibility = Visibility.Hidden;
            playlistView.Visibility = Visibility.Visible;
        }

        private void albumlistBackButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("albumlistBackButton_Copy");
            ListBoxConverter.Visibility = Visibility.Visible;
            ListBoxTrack.Visibility = Visibility.Hidden;
        }

        private void albumbutton_Click(object sender, RoutedEventArgs e)
        {
            playlistView.Visibility = Visibility.Hidden;
            playlistTracks.Visibility = Visibility.Hidden;
            ListBoxConverter.Visibility = Visibility.Visible;
            ListBoxTrack.Visibility = Visibility.Hidden;
            playlistBackButton.Visibility = Visibility.Hidden;
            albumlistBackButton_Copy.Visibility = Visibility.Visible;
            extBox.Visibility = Visibility.Hidden;
        }

        private void hover1(object sender, MouseEventArgs e)
        {
            cont.previmg = "/prevhov.png";
            setLeftimg();
        }
        private void hover2(object sender, MouseEventArgs e)
        {
            cont.playimg = "/playhov.png";
            cont.stopimg = "/stophov.png";
            setCenterimg();
        }

        private void hover3(object sender, MouseEventArgs e)
        {
            cont.nextimg = "/nexthov.png";
            setRightimg();
        }

        private void leaved1(object sender, MouseEventArgs e)
        {
            cont.previmg = "/prev.png";
            setLeftimg();
        }

        private void leaved2(object sender, MouseEventArgs e)
        {
            cont.playimg = "/play.png";
            cont.stopimg = "/stop.png";
            setCenterimg();
        }
        private void leaved3(object sender, MouseEventArgs e)
        {
            cont.nextimg = "/next.png";
            setRightimg();
        }

        private void volPlu_MouseEnter(object sender, MouseEventArgs e)
        {
            cont.volp = "/plushov.png";
            setPlusimg();
        }

        private void volPlu_MouseLeave(object sender, MouseEventArgs e)
        {
            cont.volp = "/plus.png";
            setPlusimg();
        }

        private void volMin_MouseEnter(object sender, MouseEventArgs e)
        {
            cont.volm = "/minushov.png";
            setMinusimg();
        }

        private void volMin_MouseLeave(object sender, MouseEventArgs e)
        {
            cont.volm = "/minus.png";
            setMinusimg();
        }

        private void Mute_MouseEnter(object sender, MouseEventArgs e)
        {
            cont.muteon = "/muteonhov.png";
            cont.muteoff = "/muteoffhov.png";
            if (cont.isMute) setMuteimg(cont.muteon);
            else setMuteimg(cont.muteoff);
        }

        private void Mute_MouseLeave(object sender, MouseEventArgs e)
        {
            cont.muteon = "/muteon.png";
            cont.muteoff = "/muteoff.png";
            if (cont.isMute) setMuteimg(cont.muteon);
            else setMuteimg(cont.muteoff);
        }

        static string[] external = new string[4] { "extInput:optical", "extInput:coaxial", "extInput:line?port=1", "extInput:line?port=2", };

        private void extBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int item = extBox.SelectedIndex;
            var listBoxItem = (ListBoxItem)extBox.ItemContainerGenerator.ContainerFromItem(extBox.SelectedItem);
            // ヒットテストでアイテム上
            if (listBoxItem.InputHitTest(e.GetPosition(listBoxItem)) != null)
            {
                var obj = new
                {
                    @params = new[] {
                        new
                        {
                            uri = external[item],
                        }
                    },
                    method = "setAudioInput",
                    version = "1.0",
                    id = 1003+item,
                };
                serializeJson(obj, "avContent", 0);
            }
        }
       
    }
}
