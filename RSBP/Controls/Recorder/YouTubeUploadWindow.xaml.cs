using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using RSBP.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace RSBP.Controls.Recorder
{

	/// <summary>
	/// YouTubeUploadDialog.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class YouTubeUploadWindow : MetroWindow, INotifyPropertyChanged {
		private string _youtubeTitle;
		public string YoutubeTitle { get { return _youtubeTitle; } set { _youtubeTitle = value; RaisePropertyChanged("YoutubeTitle"); } }

		private string _youtubeDescription;
		public string YoutubeDescription { get { return _youtubeDescription; } set { _youtubeDescription = value; RaisePropertyChanged("YoutubeDescription"); } }

		private string _youtubeTag;

		public string YoutubeTag
		{
			get
			{
				return _youtubeTag;
			}
			set
			{
				_youtubeTag = value;
				RaisePropertyChanged("YoutubeTag");
			}
		}

		public string[] YoutubeTags
		{
			get
			{
				if (string.IsNullOrEmpty(_youtubeTag)) return null;
				return _youtubeTag.Split(new string[] { "," }, StringSplitOptions.None);
			}
		}

        public IEnumerable<YoutubeCategory> CategoryItems
        {
            get
            {
                return new YoutubeCategoryHelper().YoutubeCategories;
            }
        }

        public YoutubeCategory CategoryItem { get; set; }

        public MessageDialogResult Result { get; set; }

		public YouTubeUploadWindow () {
			this.DataContext = this;
			InitializeComponent();
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the PropertyChanged event if needed.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed.</param>
		protected virtual void RaisePropertyChanged (string propertyName) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion INotifyPropertyChanged

		private void Button_Click (object sender, RoutedEventArgs e) {
			this.DialogResult = true;
			this.Close();
		}
	}
}