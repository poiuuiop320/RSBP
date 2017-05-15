using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using JWLibrary.Core.ExecuteLocation;
using JWLibrary.FFmpeg;
using JWLibrary.OSI;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfScreenHelper;

namespace RSBP.Controls.Recorder
{
    public class RecorderViewModel : INotifyPropertyChanged, IDisposable
    {
        public delegate void RecordingStartEvent(object sender);
        public delegate void RecordingStopEvent(object sender);

        public event RecordingStartEvent RecordingStart;
        public event RecordingStopEvent RecordingStop;

        private const string RECORD_FILENAME_KEYWORD = "CREATE_FILE";
        private string RecordingFileName;
        #region icommand
        public ICommand InitSettingCommand { get; set; }
        public ICommand AcceptResolutionCommand { get; set; }
        public ICommand RepairResolutionCommand { get; set; }
        public ICommand RecordingCommand { get; set; }
        public ICommand RecordingStopCommand { get; set; }
        public ICommand OpenRecordFolderCommand { get; set; }
        public ICommand SnapShotCommand { get; set; }
        public ICommand YoutubeUploadCommand { get; set; }
        public ICommand MonitorIdentityCommand { get; set; }
        #endregion icommand

        #region property
        private int _idx;
        public int Idx
        {
            get
            {
                return _idx;
            }
            set
            {
                _idx = value;
                RaisePropertyChanged("Idx");
            }
        }
        
        public List<string> FrameRates {
            get
            {
                return FFmpegCommandParameterSupport.GetSupportFrameRate();
            }            
        }
                
        public string SelectFrameRate
        {
            get; set;
        }       

        public List<string> AudioQualities
        {
            get
            {
                return FFmpegCommandParameterSupport.GetSupportAudioQuality();
            }
        }

        public string SelectAudioQulity
        {
            get;set;
        }
             
        public List<string> Presets
        {
            get
            {
                return FFmpegCommandParameterSupport.GetSupportPreset();
            }
        }

        public string SelectPreset
        {
            get;set;
        }
        
        private string _fps = "00";
        public string FPS
        {
            get
            {
                return _fps;
            }
            set
            {
                _fps = value;
                RaisePropertyChanged("FPS");
            }
        }

        private string _frame;
        public string Frame {
            get { return _frame; }
            set { _frame = value; RaisePropertyChanged("Frame"); } }

        private string _time = "00:00:00.00";
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                RaisePropertyChanged("Time");
            }
        }

        private int _cpuUsage;
        public int CpuUsage
        {
            get
            {
                return _cpuUsage;
            }
            set
            {
                _cpuUsage = value;
                RaisePropertyChanged("CpuUsage");
            }
        }

        private string _status = "READY";
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                this._status = value;
                RaisePropertyChanged("Status");
            }
        }        

        #endregion property        

        private Screen _pScreen;
        private CpuInfo _cpuUseageHandler;
        private JWLibrary.FFmpeg.FFMpegCaptureAV _ffmpegCav;

        #region constructor
        public RecorderViewModel()
        {
            _ffmpegCav = new FFMpegCaptureAV();
            _ffmpegCav.FFmpegDataReceived += (s, e) =>
            {
                this.FPS = e.Fps;
                this.Frame = e.Frame;
                this.Time = e.Time;
            };
            _pScreen = Screen.PrimaryScreen;

            _cpuUseageHandler = new CpuInfo();            
            _cpuUseageHandler.TotalCpuUseRateChanged += (s, e) =>
            {
                this.CpuUsage = (int)e.TotalCpuUseRate;
            };

            // add event code
            //_ffmpegCav.FrameDroped += (s, e) =>
            //{
            //    Debug.WriteLine("Frame drop!!!");
            //};

            // add event code
            //_ffmpegCav.ErrorOccured += (s, e) => {
            //    MessageBox.Show("Error!");
            //};

            RecordingCommand = new RelayCommand(o => { Record(); }, o => true);
            RecordingStopCommand = new RelayCommand(o => { RecordStop(); }, o => true);
            MonitorIdentityCommand = new RelayCommand(o => { ModitorIdentity(); }, o => true);
            OpenRecordFolderCommand = new RelayCommand(o => { OpenRecordFolder(); }, o => true);
            YoutubeUploadCommand = new RelayCommand(o => { YoutubeUpload(); }, o => true);
        }
        #endregion constructor

        #region user func
        private async void YoutubeUpload()
        {
            //if (!File.Exists(RecordingFileName))
            //{
            //    await DialogManager.ShowMessageAsync((MetroWindow)Application.Current.MainWindow, "안내", "업로드 할 파일이 없습니다.", MessageDialogStyle.Affirmative);
            //    return;
            //}

            UserCredential credential;
            using (var stream = new FileStream("./client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var youTubeUploadWindow = new YouTubeUploadWindow();
            bool isDone = (bool)youTubeUploadWindow.ShowDialog();

            if (isDone)
            {
                var video = new Video();
                video.Snippet = new VideoSnippet();
                video.Snippet.Title = youTubeUploadWindow.YoutubeTitle;
                video.Snippet.Description = youTubeUploadWindow.YoutubeDescription;
                video.Snippet.Tags = youTubeUploadWindow.YoutubeTags;
                
                // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                video.Snippet.CategoryId = youTubeUploadWindow.CategoryItem.Id.ToString();// "22"; 

                video.Status = new VideoStatus();
                video.Status.PrivacyStatus = "public"; // or "private" or "public"

                var progressController = await DialogManager.ShowProgressAsync((MetroWindow)Application.Current.MainWindow, "YouTube Upload", "유튜브 업로드를 시작합니다.", true);

                using (var fileopen = File.Open(RecordingFileName, FileMode.Open))
                {
                    progressController.Maximum = fileopen.Length;
                    fileopen.Close();
                }

                using (var fileStream = new FileStream(RecordingFileName, FileMode.Open))
                {
                    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                    videosInsertRequest.ProgressChanged += (progress) =>
                    {
                        switch (progress.Status)
                        {
                            case UploadStatus.Uploading:
                                progressController.SetProgress(progress.BytesSent);
                                //Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                                break;

                            case UploadStatus.Failed:
                                progressController.SetMessage(string.Format("An error prevented the upload from completing.\n{0}", progress.Exception));
                                //Console.WriteLine();
                                break;
                        }
                    };

                    videosInsertRequest.ResponseReceived += (param) =>
                    {
                        progressController.SetMessage(string.Format("{0} was successfully uploaded.", param.Id));
                    };

                    await videosInsertRequest.UploadAsync();
                }

                await progressController.CloseAsync();

                if (progressController.IsCanceled)
                {
                    await DialogManager.ShowMessageAsync((MetroWindow)Application.Current.MainWindow, "업로드 중단", "업로드를 중단합니다.");
                }
                else
                {
                    await DialogManager.ShowMessageAsync((MetroWindow)Application.Current.MainWindow, "업로드 완료", "업로드를 완료했습니다.");
                }
            }
        }

        private void ModitorIdentity()
        {
            _pScreen = Screen.PrimaryScreen;

            if (_pScreen != null)
            {
                MonitorWindow window = new MonitorWindow((int)_pScreen.Bounds.X, (int)_pScreen.Bounds.Y);
                window.ShowDialog();
            }
        }
        #endregion user func

        #region command event
        public void Record()
        {
            string exePath = System.IO.Path.Combine(PathInfo.GetApplicationPath(), "ffmpeg.exe");
            string fileSavePath = System.IO.Path.Combine(RSBP.Properties.Settings.Default.SaveFolderPath, "Record");

            if (!System.IO.Directory.Exists(fileSavePath))
            {
                System.IO.Directory.CreateDirectory(fileSavePath);
            }

            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");
            string day = DateTime.Now.ToString("dd");
            string hour = DateTime.Now.ToString("HH");
            string minuite = DateTime.Now.ToString("mm");
            string second = DateTime.Now.ToString("ss");

            RecordingFileName = Path.Combine(fileSavePath, string.Format(RECORD_FILENAME_KEYWORD + "{0}_{1}_{2}_{3}_{4}_{5}.mp4", year, month, day, hour, minuite, second));

            if (!_ffmpegCav.IsRunning)
            {
                if (_ffmpegCav.Initialize())
                {
                    _ffmpegCav.Register();

                    JWLibrary.FFmpeg.FFmpegCommandModel model = new JWLibrary.FFmpeg.FFmpegCommandModel
                    {
                        AudioQuality = SelectAudioQulity,
                        Format = "mp4",
                        FrameRate = SelectFrameRate,
                        Height = _pScreen.Bounds.Height.ToString(),
                        Width = _pScreen.Bounds.Width.ToString(),
                        OffsetX = _pScreen.Bounds.X.ToString(),
                        OffsetY = _pScreen.Bounds.Y.ToString(),
                        Preset = JWLibrary.FFmpeg.FFmpegCommandParameterSupport.GetSupportPreset()[0],
                        FullFileName = RecordingFileName
                    };
                    var command = JWLibrary.FFmpeg.FFmpegCommandBuilder.BuildRecordingCommand(JWLibrary.FFmpeg.RecordingTypes.Local, model);
                    _ffmpegCav.RecordingStart(command);
                }

                if (RecordingStart != null)
                {
                    RecordingStart(this);
                }

                this.Status = "RECORDING";

                _cpuUseageHandler.StartTotalCpuUseRateCheck();
            }
        }

        public void RecordStop()
        {
            if (_ffmpegCav.IsRunning)
            {
                _ffmpegCav.RecordingStop();
                _ffmpegCav.UnRegister();

                if (RecordingStop != null)
                {
                    RecordingStop(this);
                }
            }

            if (_cpuUseageHandler != null)
            {
                _cpuUseageHandler.Stop();
            }

            this.Status = "READY";
            this.FPS = "00";
            this.Frame = "00";
            this.Time = "00:00:00.00";
            this.CpuUsage = 0;
        }

        public void OpenRecordFolder()
        {
            string fileSavePath = Path.Combine(RSBP.Properties.Settings.Default.SaveFolderPath, "Record");

            if (!System.IO.Directory.Exists(fileSavePath))
            {
                System.IO.Directory.CreateDirectory(fileSavePath);
            }

            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = fileSavePath;
            prc.Start();
        }
        #endregion command event

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged

        public void Dispose()
        {
            if (_ffmpegCav != null)
            {
                _ffmpegCav.Dispose();
            }

            if (_cpuUseageHandler != null)
            {
                _cpuUseageHandler.Dispose();
            }
        }
    }
}
