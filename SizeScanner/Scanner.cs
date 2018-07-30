using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using SizeScanner;

namespace Kalantyr.SizeScanner
{
	public class Scanner
	{
		private bool _canceling;

		public Action<Exception> OnError
		{
			get; private set;
		}

		public Action<Folder> OnRootScan
		{
			get; private set;
		}

		public Action OnComplette
		{
			get; private set;
		}

		public Action<ProgressEventArgs> OnProgress
		{
			get; private set;
		}

		public Dispatcher Dispatcher
		{
			get; private set;
		}

		public DirectoryInfo Root
		{
			get; private set;
		}

		public Scanner(string root, Action<Exception> onError, Action<Folder> onRootScan, Action onComplette, Action<ProgressEventArgs> onProgress, Dispatcher dispatcher)
		{
			if (onError == null) throw new ArgumentNullException("onError");
			if (onRootScan == null) throw new ArgumentNullException("onRootScan");
			if (onComplette == null) throw new ArgumentNullException("onComplette");
			if (onProgress == null) throw new ArgumentNullException("onProgress");
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");

			OnError = onError;
			OnRootScan = onRootScan;
			OnComplette = onComplette;
			OnProgress = onProgress;
			Dispatcher = dispatcher;

			Root = new DirectoryInfo(root);
		}

		public void Scan()
		{
			try
			{
				Scan(Root, null);
			}
			finally
			{
				Dispatcher.Invoke(new Action(OnComplette));
			}
		}

		public void Scan(DirectoryInfo directory, Folder root)
		{
			if (directory == null) throw new ArgumentNullException("directory");

			if (_canceling)
				return;

			try
			{
				var folder = CreateFolder(directory, root);

				var progressEventArgs = new ProgressEventArgs(folder);
				Dispatcher.Invoke(new Action<ProgressEventArgs>(OnProgress), progressEventArgs);
				if (progressEventArgs.Cancel)
				{
					_canceling = true;
					return;
				}

				if (directory == Root)
					Dispatcher.Invoke(new Action<Folder>(OnRootScan), folder);

				if (root != null)
					Dispatcher.BeginInvoke(new Action<Folder>(root.Folders.Add), folder);

                foreach (var fileInfo in directory.GetFiles())
                    Dispatcher.BeginInvoke(new Action<FileInfo>(folder.Files.Add), fileInfo);

				foreach (var subDirectory in directory.GetDirectories().AsParallel())
					Scan(subDirectory, folder);

				Dispatcher.Invoke(new Action(folder.OnScanComplette));
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (Exception exc)
			{
				Dispatcher.Invoke(new Action<Exception>(OnError), exc);
			}
		}

		private Folder CreateFolder(DirectoryInfo directory, Folder root)
		{
			if (Dispatcher.CheckAccess())
				return new Folder(directory, root);

			return (Folder)Dispatcher.Invoke(new Func<DirectoryInfo, Folder, Folder>(CreateFolder), directory, root);
		}
	}

	public class ProgressEventArgs: EventArgs
	{
		public Folder Folder
		{
			get; private set;
		}

		public bool Cancel
		{
			get; set;
		}

		public ProgressEventArgs(Folder folder)
		{
			Folder = folder;
		}
	}
}
