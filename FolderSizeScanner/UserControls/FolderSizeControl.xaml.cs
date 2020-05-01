using System;
using System.Diagnostics;
using System.Windows;
using FolderSizeScanner.Model;

namespace FolderSizeScanner.UserControls
{
    public partial class FolderSizeControl
    {
        public ScanContext ScanContext => DataContext as ScanContext;

        public Action<ScanContext> RequestClose;

        public FolderSizeControl()
        {
            InitializeComponent();
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
    }
}
