using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kalantyr.SizeScanner.UserControls
{
	public partial class FilesControl
	{
		public static readonly DependencyProperty ItemsSourceProperties = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<FileInfo>), typeof(FilesControl));

		public IEnumerable<FileInfo> ItemsSource
		{
			get
			{
				return (IEnumerable<FileInfo>)GetValue(ItemsSourceProperties);
			}
			set
			{
				SetValue(ItemsSourceProperties, value);
				dataGrid.ItemsSource = value;
			}
		}

		public FilesControl()
		{
			InitializeComponent();
		}

		private void MenuItem_OpenFile_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("\"" + Model.Instance.SelectedFile.FullName + "\"");
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Model.Instance.SelectedFiles = ((DataGrid)sender).SelectedItems.Cast<FileInfo>().ToArray();
		}

		private void MenuItem_OpenDirectory_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("\"" + Model.Instance.SelectedFile.Directory.FullName + "\"");
		}

		private void MenuItem_CopyTo_Click(object sender, RoutedEventArgs e)
		{
			const string copyto = @"C:\Temp\CopyTo";

			if (!Directory.Exists(copyto))
				Directory.CreateDirectory(copyto);

			var counter = 0;
			foreach (var file in Model.Instance.SelectedFiles)
				try
				{
					var destFileName = string.Empty;
					var i = 0;
					do
					{
						var fn = Path.GetFileNameWithoutExtension(file.Name);
						if (i > 0)
							fn += string.Format(" ({0})", i);
						var ex = Path.GetExtension(file.Name);
						destFileName = Path.Combine(copyto, fn + ex);
						i++;
					} while (File.Exists(destFileName));

					file.CopyTo(destFileName);
					counter++;
				}
				catch (Exception exc)
				{
					var text = "Не удалось скопировать файл " + file.FullName + "." + Environment.NewLine + exc.Message;
					MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}

			var msg = string.Format("Копирование файлов ({0} шт.) в папку {1} завершено.", counter, copyto);
			MessageBox.Show(msg, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
		{
			var selectedFiles = Model.Instance.SelectedFiles.ToArray();

			ExtensionsAnalysisControl.Remove(selectedFiles, () =>
			{
				if (Model.Instance.SelectedFolder != null)
					foreach (var selectedFile in selectedFiles)
						Model.Instance.SelectedFolder.Files.Remove(selectedFile);

			});
		}
	}
}
