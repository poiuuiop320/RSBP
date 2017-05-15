using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using MahApps.Metro.Controls;
using RSBP.Controls.Recorder;
using MahApps.Metro.Controls.Dialogs;
using RSBP.Controls.WhiteBoard;
using RSBP.Properties;

namespace RSBP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {        
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowCloseMessge();
        }

        private async void ShowCloseMessge()
        {
            MessageDialogResult result = await DialogManager.ShowMessageAsync(this, "안내", "종료합니다.", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                RecorderControl.Instance.Dispose();
                WhiteBoardControl.Instance.Dispose();

                Settings.Default.Save();

                Application.Current.Shutdown();
            }
        }
    }
}
