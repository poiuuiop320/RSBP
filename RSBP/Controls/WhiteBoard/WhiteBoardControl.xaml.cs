using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace RSBP.Controls.WhiteBoard
{
    /// <summary>
    /// WhiteControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WhiteBoardControl : UserControl, IDisposable
    {
        private static WhiteBoardControl _instance;
        public static WhiteBoardControl Instance {
            get
            {
                if(_instance == null)
                {
                    _instance = new WhiteBoardControl();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public WhiteBoardControl()
        {
            _instance = this;
            InitializeComponent();

            this.DrawColorPicker.StandardColors.Clear();
            this.DrawColorPicker.AvailableColors.Clear();
            PopulateColorList();
            enabledSwitch_Checked(null, null);
        }

        private void PopulateColorList()
        {
            ObservableCollection<ColorItem> ColorList = new ObservableCollection<ColorItem>();
            ColorList.Add(new ColorItem(Colors.Beige, "Beige"));
            ColorList.Add(new ColorItem(Colors.Black, "Black"));
            ColorList.Add(new ColorItem(Colors.Blue, "Blue"));
            ColorList.Add(new ColorItem(Colors.Pink, "Pink"));
            ColorList.Add(new ColorItem(Colors.Red, "Red"));
            ColorList.Add(new ColorItem(Colors.White, "White"));
            ColorList.Add(new ColorItem(Colors.Yellow, "Yellow"));

            this.DrawColorPicker.StandardColors = ColorList;
            this.DrawColorPicker.AvailableColors = ColorList;
        }

        private void FontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                if (WhiteBoardWindow.Instance != null)
                    WhiteBoardWindow.Instance.FontSizeChage(int.Parse(btn.ToolTip.ToString()), int.Parse(btn.ToolTip.ToString()));
            }
        }

        private void DrawColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (WhiteBoardWindow.Instance != null)
            {
                WhiteBoardWindow.Instance.DrawColorChange((Color)this.DrawColorPicker.SelectedColor);
            }
        }

        private void MethodButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(this.EraseButton))
            {
                if (WhiteBoardWindow.Instance != null)
                    WhiteBoardWindow.Instance.FunctionChange("ERASE");
            }
            else if (sender.Equals(this.SelectButton))
            {
                if (WhiteBoardWindow.Instance != null)
                    WhiteBoardWindow.Instance.FunctionChange("SELECT");
            }
            else if (sender.Equals(this.ClearButton))
            {
                if (WhiteBoardWindow.Instance != null)
                    WhiteBoardWindow.Instance.FunctionChange("CLEAR");
            }
        }

        private void enabledSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (WhiteBoardWindow.Instance != null)
                WhiteBoardWindow.Instance.IsToolBoxVisible = (bool)this.enabledSwitch.IsChecked;
        }

        private void WriteBoardWindowOpen_Click(object sender, RoutedEventArgs e)
        {
            WhiteBoardWindow.Instance.WhiteBoardShow();
        }

        public void Dispose()
        {
            
        }
    }
}
