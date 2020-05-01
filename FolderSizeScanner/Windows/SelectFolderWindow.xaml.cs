using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace FolderSizeScanner.Windows
{
    public partial class SelectFolderWindow
    {
        public string SelectedFolder => _tbFolder.Text;

        public SelectFolderWindow()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(SelectedFolder))
                    throw new DirectoryNotFoundException($"Folder {SelectedFolder} not exists");

                DialogResult = true;
            }
            catch (Exception error)
            {
                App.ShowError(error);
            }
        }

        private void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
                _tbFolder.Text = new FileInfo(ofd.FileName).DirectoryName;
        }
    }
}
