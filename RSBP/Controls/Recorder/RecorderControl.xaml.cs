using JWLibrary.Core.ExecuteLocation;
using JWLibrary.OSI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;

namespace RSBP.Controls.Recorder
{
    /// <summary>
    /// ViewWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecorderControl : UserControl, IDisposable
    {
        private static RecorderControl _instance;
        public static RecorderControl Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new RecorderControl();
                }

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public readonly RecorderViewModel recorderViewModel = new RecorderViewModel();
        public RecorderControl()
        {
            _instance = this;

            if (string.IsNullOrEmpty(RSBP.Properties.Settings.Default.SaveFolderPath))
            {
                RSBP.Properties.Settings.Default.SaveFolderPath = PathInfo.GetApplicationPath();
                RSBP.Properties.Settings.Default.Save();
            }

            DataContext = recorderViewModel;

            InitializeComponent();

            recorderViewModel.RecordingStart += RecorderViewModel_RecordingStart;
            recorderViewModel.RecordingStop += RecorderViewModel_RecordingStop;

            this.MasterVolume.ValueChanged += MasterVolume_ValueChanged;
            this.MasterVolume.Value = AudioManager.GetMasterVolume();

            this.cboFrameRate.SelectedIndex = 1;
            this.cboAudioQuality.SelectedIndex = 1;
        }

        private void MasterVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue >= 70) this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound3;
            else if (e.NewValue >= 40) this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound2;
            else if (e.NewValue >= 10) this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound1;
            else this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound0;
        }

        private void RecorderViewModel_RecordingStop(object sender)
        {
            this.btnRecord.Visibility = Visibility.Visible;
            this.btnStop.Visibility = Visibility.Collapsed;
        }

        private void RecorderViewModel_RecordingStart(object sender)
        {
            this.btnRecord.Visibility = Visibility.Collapsed;
            this.btnStop.Visibility = Visibility.Visible;
        }

        private void SoundMute(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;

            if (btn != null)
            {
                bool isMute = (bool)btn.IsChecked;

                AudioManager.SetMasterVolumeMute(isMute);

                if (isMute)
                {
                    this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.SoundMute;
                }
                else
                {
                    var volume = AudioManager.GetMasterVolume();

                    if (volume >= 70) this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound3;
                    else if (volume >= 40) this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound2;
                    else if (volume >= 10) this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound1;
                    else this.SoundIcon.Kind = MahApps.Metro.IconPacks.PackIconModernKind.Sound0;
                }
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AudioManager.SetMasterVolume((float)e.NewValue);
        }

        public void Dispose()
        {
            if (recorderViewModel != null)
            {
                recorderViewModel.Dispose();
            }
        }
    }
}
