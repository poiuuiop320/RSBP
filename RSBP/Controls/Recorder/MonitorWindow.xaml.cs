using System;
using System.Windows;
using System.Windows.Threading;

namespace RSBP.Controls.Recorder {

	/// <summary>
	/// MonitorWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MonitorWindow : Window {
		private bool _isLoaded;

		public string MonitorNumberText
		{
			set
			{
				this.outLineText.Text = value;
			}
		}

		public MonitorWindow () {
		}

		public MonitorWindow (int x, int y) {
			InitializeComponent();
			this.Loaded += RectWindow_Loaded;
			this.Closing += RectWindow_Closing;
			this.Left = x;
			this.Top = y;
		}

		private void RectWindow_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
			if (true) {
			}
		}

		private void RectWindow_Loaded (object sender, RoutedEventArgs e) {
			if (!_isLoaded) {
				_isLoaded = true;
				this.WindowState = System.Windows.WindowState.Maximized;
				DispatcherTimer timer = new DispatcherTimer();
				timer.Tick += timer_Tick;
				timer.Interval = new TimeSpan(0, 0, 1);
				timer.Start();
			}
		}

		private void timer_Tick (object sender, EventArgs e) {
			DispatcherTimer timer = sender as DispatcherTimer;

			if (timer != null) {
				if (timer.IsEnabled) {
					timer.IsEnabled = false;
					timer.Stop();
					this.Close();
				}
			}
		}
	}
}