using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using FolderSizeScanner.Model;
using FolderSizeScanner.UserControls;
using FolderSizeScanner.Windows;

namespace FolderSizeScanner
{
    public partial class MainWindow
    {
        private readonly ICollection<ScanContext> _scanContexts = new ObservableCollection<ScanContext>();
        
        private readonly TabItem _addTabItem;

        public MainWindow()
        {
            InitializeComponent();

            var textBlock = new TextBlock { Text = "+" };
            textBlock.MouseDown += (sender, e) => Add();
            _addTabItem = new TabItem { Header = textBlock };
            _tabControl.Items.Add(_addTabItem);

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!_scanContexts.Any())
                Add();
        }

        private void Add()
        {
            var window = new SelectFolderWindow { Owner = this };
            if (window.ShowDialog() != true)
                return;

            var newScanContext = new ScanContext
            {
                Root = new DirectoryInfo(window.SelectedFolder)
            };
            var folderSizeControl = new FolderSizeControl
            {
                DataContext = newScanContext,
                RequestClose = Remove
            };
            var tabItem = new TabItem
            {
                Content = folderSizeControl,
                Header = newScanContext.Root.Name,
                ToolTip = newScanContext.Root.FullName,
                DataContext = newScanContext
            };
            _tabControl.Items.Insert(0, tabItem);
            _scanContexts.Add(newScanContext);
            _tabControl.SelectedItem = tabItem;
        }

        private void Remove(ScanContext scanContext)
        {
            TabItem tabItemToRemove = null;
            foreach (TabItem tabItem in _tabControl.Items)
                if (tabItem.DataContext == scanContext)
                {
                    tabItemToRemove = tabItem;
                    break;
                }
            _tabControl.Items.Remove(tabItemToRemove);

            _scanContexts.Remove(scanContext);
        }
    }
}
