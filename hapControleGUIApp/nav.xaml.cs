﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
using Microsoft.Win32;
using System.Collections;

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
        dynamic[,] FileName;
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
            getAllAlbumInfo();
            LoadListItems();
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
            string setUrl = cont.hostUrl + "contentdb/v100/audio/albums";

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

        private void LoadListItems()
        {
            dataList = new List<VisibleItem>();
            dataList = getDataList();
            ListBoxConverter.ItemsSource = dataList;
            ListBoxConverter.DataContext = this;
        }

        private List<VisibleItem> getDataList()
        {
            dispatcherTimer.Stop();//情報定期取得一時停止

            VisibleItem vItem;
            vItem = new VisibleItem();
            //albums[0]のみ例外
            vItem.CoverArt = myDocument + "/" + "default.png";
            vItem.ArtistName = "不明なアーティスト";
            vItem.AlbumName = "不明なアルバム";
            vItem.TrackFigure = "トラック:" + allalbumdata.albums[0].number_of_tracks.ToString();
            vItem.TracksUrl = allalbumdata.albums[0].tracks_url;
            dataList.Add(vItem);

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
                downloadCoverArt();

                vItem = new VisibleItem();
                vItem.CoverArt = nowMusicCover;
                vItem.ArtistName = allalbumdata.albums[i].album_artist.name;
                vItem.AlbumName = allalbumdata.albums[i].name;
                vItem.TrackFigure = "トラック:" + allalbumdata.albums[i].number_of_tracks.ToString();
                vItem.TracksUrl = allalbumdata.albums[0].tracks_url;
                dataList.Add(vItem);
            }
            dispatcherTimer.Start();//再開
            return dataList;
        }

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
            FileName = new dynamic[numberoftracks,2];

            AlbumData = DynamicJson.Parse(gettrackinfo(allalbumdata.albums[number].tracks_url)); //トラック情報の取得
            numberoftracks = (int)AlbumData.paging.total;
            string urlnew = allalbumdata.albums[number].tracks_url + "?offset=0&limit=" + numberoftracks.ToString();
           
            AlbumData = DynamicJson.Parse(gettrackinfo(urlnew)); //トラック情報の取得

            for (int num = 0; num < numberoftracks; num++)
            {
                Tracks[num] = AlbumData.tracks[num].name; //すべての名前の取得
                Duration[num] = (int) AlbumData.tracks[num].duration; //曲長さ(秒)
                Codec[num] = AlbumData.tracks[num].codec.codec_type; //コーデック
                Bitrate[num] = (AlbumData.tracks[num].codec.bit_rate / 1000).ToString(); //ビットレート
                int fr = (int) AlbumData.tracks[num].codec.sample_rate / 1000;
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

            for (int i = 0; i < FileName.Length/2 - 1; i++)
            {
                for (int k = FileName.Length/2 - 1; k > i; k--)
                {
                    if (FileName[k,0].CompareTo(FileName[k - 1,0]) < 0)
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
                tracklist.TrackName = (cnt+1).ToString() + "  " + AlbumData.tracks[FileName[cnt, 1]].name;
                tracklist.TrackInfo = info;
                tracklist.ContentUrl = AlbumData.tracks[FileName[cnt, 1]].album.url;
                tracklist.MusicId = (AlbumData.tracks[FileName[cnt, 1]].trackid).ToString();
                TracksdataList.Add(tracklist);
                info = "";
            }
            ListBoxTrack.ItemsSource = TracksdataList;
            ListBoxTrack.DataContext = this;
        }

        dynamic gettrackinfo(string url)
        {
            var req = WebRequest.Create(url);
            var res = req.GetResponse();
            dynamic data = res.GetResponseStream();
            return data;
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
                    Console.WriteLine(i);
                    wc.DownloadFile(coverArtUrl, nowMusicCover);
                }
            }
            if (noCoverArt == 1)
            {
                nowMusicCover = myDocument + "/" + "default.png";
                if (!File.Exists(nowMusicCover))
                {
                    string ipad = cont.hostUrl.Replace(":60200/sony/", "");
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

        private void listClick(object sender, RoutedEventArgs e)
        {
            int item = ListBoxConverter.SelectedIndex;
            DisplayAlbumInfo(item);//戻るボタン実装
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
    }
}
