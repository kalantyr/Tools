using System.Windows;
using ScreenClock.Properties;

namespace ScreenClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const double ResizeRatio = 1.5d;

        private readonly bool tuning;

        public MainWindow()
        {
            InitializeComponent();

            tuning = true;

            Width = Settings.Default.WindowSize.Width;
            Height = Settings.Default.WindowSize.Height;
            Left = Settings.Default.WindowPosition.X;
            Top = Settings.Default.WindowPosition.Y;

            tuning = false;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Handled)
                return;

            DragMove();
        }

        private void CloseMenuIncreaseSizeClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.WindowSize = new Size(Settings.Default.WindowSize.Width * ResizeRatio, Settings.Default.WindowSize.Height * ResizeRatio);
            Width = Settings.Default.WindowSize.Width;
            Height = Settings.Default.WindowSize.Height;
            SaveSettings();
        }

        private void CloseMenuDecreaseSizeClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.WindowSize = new Size(Settings.Default.WindowSize.Width / ResizeRatio, Settings.Default.WindowSize.Height / ResizeRatio);
            Width = Settings.Default.WindowSize.Width;
            Height = Settings.Default.WindowSize.Height;
            SaveSettings();
        }

        private void CloseMenuItemClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnLocationChanged(System.EventArgs e)
        {
            base.OnLocationChanged(e);

            if (tuning)
                return;

            Settings.Default.WindowPosition = new Point(Left, Top);
            SaveSettings();
        }

        private static void SaveSettings()
        {
            try
            {
                Settings.Default.Save();
            }
            catch
            {
            }
        }
    }
}
