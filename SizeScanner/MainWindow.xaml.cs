using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;
using Kalantyr.SizeScanner.UserControls;
using SizeScanner;
using SizeScanner.Properties;
using MessageBox = System.Windows.MessageBox;

namespace Kalantyr.SizeScanner
{
	public partial class MainWindow
	{
		public Model Model
		{
			get; private set;
		}

		public MainWindow()
		{
			InitializeComponent();

			Model = new Model
			        	{
			        		TargetPath = Settings.Default.LastSelectedFolder
			        	};
			DataContext = Model;
		}

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (TaskbarItemInfo.ProgressState == TaskbarItemProgressState.Paused)
            {
                TaskbarItemInfo.ProgressValue = 0;
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            }
        }

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.LastSelectedFolder = Model.TargetPath;
			Settings.Default.Save();

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            Model.IsScanning = true;
			Model.IsCanceling = false;

			var scanner = new Scanner(Settings.Default.LastSelectedFolder, OnError, OnRootScan, OnComplette, OnProgress, Dispatcher);
			var thread = new Thread(scanner.Scan)
			{
			    IsBackground = true,
			    Name = "Сканирование размера папок",
			    Priority = ThreadPriority.BelowNormal
			};
			thread.Start();
		}

		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
			Model.IsCanceling = true;
		}

		private void OnComplette()
		{
			Model.IsScanning = false;
			if (Model.ScanResult[0] != null)
				Model.ScanResult[0].FoldersView.Refresh();
			Model.IsCanceling = false;
			Model.ScanningFolder = null;

            if (IsActive)
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            else
            {
                TaskbarItemInfo.ProgressValue = 1;
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
            }
		}

		private void OnError(Exception error)
		{
			var message = error.Message + Environment.NewLine + Environment.NewLine + error;
			MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void OnRootScan(Folder folder)
		{
			Model.ScanResult.Add(folder);
		}

		private void OnProgress(ProgressEventArgs progressEventArgs)
		{
			Model.ScanningFolder = progressEventArgs.Folder;
			if (Model.IsCanceling)
				progressEventArgs.Cancel = true;
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			Model.SelectedFolder = e.NewValue as Folder;
		}

		private void MenuItem_OpenFolder_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("\"" + Model.SelectedFolder.DirectoryInfo.FullName + "\"");
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog
			{
				Description = "Выберите папку для сканирования",
				ShowNewFolderButton = false
			};

			if (Directory.Exists(Model.TargetPath))
				dialog.SelectedPath = Model.TargetPath;

			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

			Model.TargetPath = dialog.SelectedPath;
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Очистить результаты поиска?", "Подтвердите", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
			if (result != MessageBoxResult.Yes)
				return;

			Model.ScanResult.Clear();
		}

		private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show(string.Format("Удалить безвозвратно папку '{0}', включая вложенные папки?", Model.SelectedFolder.DirectoryInfo.FullName), "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
			if (result != MessageBoxResult.Yes)
				return;

			try
			{
				Delete(Model.SelectedFolder);
			}
			catch (Exception error)
			{
				App.ShowError(error);
			}
		}

		private static void Delete(Folder folder)
		{
			try
			{
				folder.DirectoryInfo.Delete(true);

				if (folder.Parent != null)
					folder.Parent.Folders.Remove(folder);
			}
			catch (Exception exc)
			{
				throw new InvalidOperationException(string.Format("Не удалось удалить папку '{0}'.", folder.DirectoryInfo.FullName), exc);
			}
		}

		private void MenuItem_ExtensionsAnalysis_Click(object sender, RoutedEventArgs e)
	    {
            var control = new ExtensionsAnalysisControl(Model.SelectedFolder);
	        var window = new AnalysisWindow
            {
                Owner = this,
                Title = "Распределение файлов по расширениям",
                Content = control
            };
	        window.ShowDialog();
	    }
	}

	public class FileSizeConverter: IValueConverter
	{
		public static string ToString(long value)
		{
			var formatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
			formatInfo.NumberDecimalDigits = 0;
			return value.ToString("N", formatInfo);
		}

		public static string SizeToString(long size)
		{
			const int thousand = 1024;

			if (size < thousand)
				return ToString(size) + " B";

			size = size / thousand;
			if (size < thousand)
				return ToString(size) + " KB";

			size = size / thousand;
			if (size < thousand)
				return ToString(size) + " MB";

			size = size / thousand;
			if (size < thousand)
				return ToString(size) + " GB";

			size = size / thousand;
			if (size < thousand)
				return ToString(size) + " TB";

			size = size / thousand;
			if (size < thousand)
				return ToString(size) + " PB";

			return ToString(size);
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return SizeToString((long) value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class FolderToolTipConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var folder = (Folder)value;
			return string.Join(Environment.NewLine, new[]
			                                        	{
			                                        		folder.DirectoryInfo.FullName,
			                                        		"Размер (байт): " + FileSizeConverter.ToString(folder.Size),
			                                        		"Файлов: " + FileSizeConverter.ToString(folder.Files.Count)
			                                        	});
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class IsEnabledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return false;
			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class SizeToColorConverter: IValueConverter
	{
		private static readonly SolidColorBrush _bBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
		private static readonly SolidColorBrush _kbBrush = new SolidColorBrush(Color.FromRgb(64, 0, 0));
		private static readonly SolidColorBrush _mbBrush = new SolidColorBrush(Color.FromRgb(128, 0, 0));
		private static readonly SolidColorBrush _gbBrush = new SolidColorBrush(Color.FromRgb(192, 0, 0));
		private static readonly SolidColorBrush _tbBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var size = (long) value;

			if (size < 1024)
				return _bBrush;

			if (size < 1024 * 1024)
				return _kbBrush;

			if (size < 1024 * 1024 * 1024)
				return _mbBrush;

			if (size < 1024 * 1024 * 1024)
				return _gbBrush;

			return _tbBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
