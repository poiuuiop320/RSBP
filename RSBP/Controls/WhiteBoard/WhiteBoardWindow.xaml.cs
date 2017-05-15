using RSBP.Properties;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfScreenHelper;
using Xceed.Wpf.Toolkit;

namespace RSBP.Controls.WhiteBoard
{
    /// <summary>
    /// WhiteBoardWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WhiteBoardWindow : Window, IDisposable
    {
        private static WhiteBoardWindow _instance;
        public static WhiteBoardWindow Instance {
            get
            {
                if(_instance == null)
                {
                    _instance = new WhiteBoardWindow();
                }

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public bool IsToolBoxVisible
        {
            set
            {
                if (value)
                    ControlPanel.Visibility = System.Windows.Visibility.Visible;
                else
                    ControlPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public WhiteBoardWindow()
        {
            DataContext = this;
            _instance = this;

            InitializeComponent();

            this.DrawColorPicker.StandardColors.Clear();
            this.DrawColorPicker.AvailableColors.Clear();

            PopulateColorList();

            this.DrawGroundInk.DefaultDrawingAttributes.Width = Settings.Default.WhiteBoardSize;
            this.DrawGroundInk.DefaultDrawingAttributes.Height = Settings.Default.WhiteBoardSize;

            if (this.DrawGroundInk.IsGestureRecognizerAvailable)
            {
                this.DrawGroundInk.EditingMode = InkCanvasEditingMode.InkAndGesture;
                this.DrawGroundInk.Gesture += DrawGroundInk_Gesture;
                this.DrawGroundInk.SetEnabledGestures(new ApplicationGesture[] {
                    ApplicationGesture.UpDown,
                    ApplicationGesture.DownUp
                });
            }

            this.DrawGroundInk.MouseWheel += DrawGroundInk_MouseWheel;
            this.DrawColorPicker.SelectedColor = Properties.Settings.Default.WhiteBoardSelectedColor;

            var primaryScreen = Screen.PrimaryScreen;

            this.Left = primaryScreen.Bounds.X;
            this.Top = primaryScreen.Bounds.Y;
            this.Width = primaryScreen.Bounds.Width;
            this.Height = primaryScreen.Bounds.Height;

            this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void DrawGroundInk_Gesture(object sender, InkCanvasGestureEventArgs e)
        {
            ReadOnlyCollection<GestureRecognitionResult> gestureResults =
                e.GetGestureRecognitionResults();

            if (gestureResults[0].RecognitionConfidence == RecognitionConfidence.Strong)
            {
                switch (gestureResults[0].ApplicationGesture)
                {
                    case ApplicationGesture.DownUp:
                        if (_point == null)
                        {
                            _point = Mouse.GetPosition(this);

                            scaleT.CenterX = _point.Value.X;
                            scaleT.CenterY = _point.Value.Y;
                        }

                        scaleT.ScaleX = scaleT.ScaleY = _scaleRate;
                        break;
                    case ApplicationGesture.UpDown:
                        if (_point != null)
                        {
                            _point = null;
                        }

                        scaleT.ScaleX = scaleT.ScaleY = 1.0;
                        break;
                }
            }
        }

        private const double _scaleRate = 1.80;
        private Point? _point;
        private void DrawGroundInk_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (_point == null)
                {
                    _point = e.GetPosition(this);

                    scaleT.CenterX = _point.Value.X;
                    scaleT.CenterY = _point.Value.Y;
                }

                scaleT.ScaleX = scaleT.ScaleY = _scaleRate;
            }
            else
            {
                if (_point != null)
                    _point = null;

                if (scaleT.ScaleX <= 1.0)
                {
                    return;
                }

                scaleT.ScaleX = scaleT.ScaleY = 1.0;
            }
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


        

        private async void ControlPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    StackPanel panel = sender as StackPanel;

                    if (panel != null)
                    {
                        DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
                        panel.BeginAnimation(StackPanel.OpacityProperty, animation);
                    }
                });
            });
        }

        private async void ControlPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    StackPanel panel = sender as StackPanel;

                    if (panel != null)
                    {
                        DoubleAnimation animation = new DoubleAnimation(0.3, TimeSpan.FromSeconds(0.5));
                        panel.BeginAnimation(StackPanel.OpacityProperty, animation);
                    }
                });
            });
        }

        private void FontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button fsBtn = sender as Button;

            if (fsBtn != null)
            {
                FontSizeChage(int.Parse(fsBtn.ToolTip.ToString()), int.Parse(fsBtn.ToolTip.ToString()));
            }
        }

        private void DrawColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            DrawColorChange((Color)this.DrawColorPicker.SelectedColor);
        }

        private void FunctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(this.EraseButton))
            {
                this.DrawGroundInk.EditingMode = InkCanvasEditingMode.EraseByStroke;
            }
            else if (sender.Equals(this.SelectButton))
            {
                this.DrawGroundInk.EditingMode = InkCanvasEditingMode.Select;
            }
            else if (sender.Equals(this.ClearButton))
            {
                this.DrawGroundInk.Strokes.Clear();
            }
            else if (sender.Equals(this.CloseButton))
            {
                this.Hide();
            }
        }

        public void FontSizeChage(int width, int height)
        {
            this.DrawGroundInk.DefaultDrawingAttributes.Width = width;
            this.DrawGroundInk.DefaultDrawingAttributes.Height = height;
            this.DrawGroundInk.EditingMode = InkCanvasEditingMode.InkAndGesture;

            Settings.Default.WhiteBoardSize = width;
        }

        public void DrawColorChange(Color color)
        {
            this.DrawGroundInk.DefaultDrawingAttributes.Color = color;
            this.DrawColorPicker.SelectedColor = color;
            WhiteBoardControl.Instance.DrawColorPicker.SelectedColor = color;
            Settings.Default.WhiteBoardSelectedColor = (Color)this.DrawColorPicker.SelectedColor;
        }

        public void FunctionChange(string type)
        {
            switch (type.ToUpper())
            {
                case "ERASE":
                    this.DrawGroundInk.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;

                case "SELECT":
                    this.DrawGroundInk.EditingMode = InkCanvasEditingMode.Select;
                    break;

                case "CLEAR":
                    this.DrawGroundInk.Strokes.Clear();
                    break;
            }
        }

        public void WhiteBoardShow()
        {
            var screen = Screen.PrimaryScreen;
            using (var screenBmp = new System.Drawing.Bitmap((int)screen.Bounds.Width, (int)screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = System.Drawing.Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    bmpGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    bmpGraphics.CopyFromScreen((int)screen.Bounds.Left, (int)screen.Bounds.Top, 0, 0, screenBmp.Size);

                    this.InkCanvasBackground.ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                    screenBmp.GetHbitmap(),
                                    IntPtr.Zero,
                                    Int32Rect.Empty,
                                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
            }

            this.Show();
        }

        public void Dispose()
        {
            
        }
    }
}
