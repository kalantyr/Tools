using System;
using System.Diagnostics;
using System.Windows;
using FolderSizeScanner.Model;

namespace FolderSizeScanner.UserControls
{
    public partial class FolderSizeControl
    {
        private Scanner _scanner;

        public ScanContext ScanContext => DataContext as ScanContext;

        public Action<ScanContext> RequestClose;

        public FolderSizeControl()
        {
            InitializeComponent();
            TuneControls();
            Loaded += FolderSizeControl_Loaded;
        }

        private void FolderSizeControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ScanContext != null)
                Start();
        }

        private void Start()
        {
            ScanContext.Result = new ScanResult(ScanContext.Root.FullName);

            _scanner = new Scanner(ScanContext)
            {
                Completed = sc =>
                {
                    _scanner = null;
                    TuneControls();
                }
            };
            _scanner.Start();
            TuneControls();
        }

        private void Stop()
        {
            _scanner?.Stop();
        }

        private void TuneControls()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(TuneControls);
                return;
            }

            _btnStartStop.Content = _scanner == null || _scanner.ScanIsCompletted ? "Start" : "Stop";
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            RequestClose?.Invoke(ScanContext);
        }

        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            var s = ScanContext.Root.FullName;
            //var s = $"\"{ScanContext.Root.FullName}\"";
            Process.Start(new ProcessStartInfo
            {
                FileName = s,
                CreateNoWindow = false,
                UseShellExecute = true,
                WorkingDirectory = ScanContext.Root.FullName
            });
        }

        private void OnStartStopClick(object sender, RoutedEventArgs e)
        {
            if (_scanner == null)
                Start();
            else
                Stop();
        }
    }
}
