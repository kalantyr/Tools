using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using SizeScanner;

namespace Kalantyr.SizeScanner.UserControls
{
    public partial class ExtensionsAnalysisControl
    {
    	public Folder Folder
    	{
    		get; private set;
    	}

    	public ExtensionsAnalysisControl()
        {
            InitializeComponent();
        }

        public ExtensionsAnalysisControl(Folder folder): this()
        {
        	Folder = folder;
        	InitializeComponent();

			Loaded += (sender, e) => Rebuild();

        }

    	private void Rebuild()
    	{
    		try
    		{
    			Cursor = Cursors.Wait;
    			dataGrid.ItemsSource = Analysis();
    		}
    		finally
    		{
    			Cursor = null;
    		}
    	}

    	private IEnumerable<ExtInfo> Analysis()
        {
            ICollection<ExtInfo> collection = new List<ExtInfo>();
            Analysis(Folder, collection);
            return collection.ToArray();
        }

        private static void Analysis(Folder folder, ICollection<ExtInfo> collection)
        {
            foreach (var file in folder.Files)
            {
                var ext = file.Extension.ToLower().Trim();
                
                var extInfo = new ExtInfo { Name = ext, Count = 0, SumSize = 0 };
                foreach (var info in collection.Where(info => info.Name == ext))
                {
                    extInfo = info;
                    break;
                }
                if (!collection.Contains(extInfo))
                    collection.Add(extInfo);

                extInfo.Count++;
                extInfo.SumSize += file.Length;
            }

            foreach (var f in folder.Folders)
                Analysis(f, collection);
        }

		private static void AddByExt(Folder folder, ICollection<FileInfo> collection, ExtInfo extInfo)
		{
			foreach (var file in folder.Files.Where(file => file.Extension.ToLower().Trim() == extInfo.Name))
				collection.Add(file);
			foreach (var f in folder.Folders)
				AddByExt(f, collection, extInfo);
		}

		private IEnumerable<FileInfo> GetSelectedFiles()
		{
			var list = new List<FileInfo>();
			foreach (ExtInfo extInfo in dataGrid.SelectedItems)
				AddByExt(Folder, list, extInfo);
			return list.ToArray();
		}

    	private void MenuItem_Files_Click(object sender, RoutedEventArgs e)
    	{
    		var window = new AnalysisWindow
			{
				Owner = Window.GetWindow(this),
				Title = "Файлы",
				Content = new FilesControl
				{
					ItemsSource = GetSelectedFiles().ToArray(),
				    DataContext = Model.Instance
				}
			};
			window.ShowDialog();
		}

    	private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
    	{
    		var files = GetSelectedFiles().ToArray();

    		Remove(files, Rebuild);
    	}

    	internal static void Remove(IEnumerable<FileInfo> files, Action onSuccess = null)
    	{
    		var q = string.Format("Удалить файлы ({0}) ?", files.Count());
    		if (MessageBox.Show(q, "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
    			return;

            var removed = new List<long>();
            foreach (var file in files)
    			try
    			{
                    var length = file.Length;
                    file.Delete();
    				removed.Add(length);
                }
    			catch (Exception exc)
    			{
    				var msg = "Ошибка: " + exc.Message + Environment.NewLine + "Продолжить удаление других файлов?";
    				var result = MessageBox.Show(msg, "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Error);
    				if (result == MessageBoxResult.No)
    					break;
    			}

			if (onSuccess != null)
				onSuccess();

            var info = "Удалено файлов: " + removed.Count;
            if (removed.Count > 0)
                info += Environment.NewLine + "Суммарный размер удаленных файлов: " + FileSizeConverter.SizeToString(removed.Sum());
    		MessageBox.Show(info, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
    	}
    }

    public class ExtInfo
    {
        /// <summary>
        /// Название расширения
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Количество фалов этого типа
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Суммарный размер
        /// </summary>
        public long SumSize
        {
            get;
            set;
        }

		/// <summary>
		/// Средний размер
		/// </summary>
    	public long AverageSize
    	{
			get { return SumSize / Count; }
    	}
    }

    public class IntToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int) value).ToString("### ### ### ###", CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
