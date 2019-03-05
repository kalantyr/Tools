using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Threading.Timer;

namespace ClipboardTool
{
	public partial class MainWindow
	{
		private static readonly TimeSpan TimerPeriod = TimeSpan.FromSeconds(1);

		private readonly ClipboardListener _listener = new ClipboardListener();
		private readonly ObservableCollection<ListItem> _itemsSource = new ObservableCollection<ListItem>();
		private NotifyIcon _notifyIcon;
		private Timer _timer;

		public MainWindow()
		{
			InitializeComponent();

			_lb.ItemsSource = _itemsSource;
			_listener.Add = OnAddDataObject;

			Loaded += (sender, e) =>
			{
				_notifyIcon = new NotifyIcon
				{
					Icon = Properties.Resources.Clipboard
				};
				_notifyIcon.Click += (sender1, e1) =>
				{
					WindowState = WindowState.Normal;
				};

				_timer = new Timer(OnTimer, null, TimeSpan.Zero, TimerPeriod);
			};

			StateChanged += MainWindow_StateChanged;
		}

		private void MainWindow_StateChanged(object sender, EventArgs e)
		{
			switch (WindowState)
			{
				case WindowState.Minimized:
					ShowInTaskbar = false;
					_notifyIcon.Visible = true;
					break;
				default:
					_notifyIcon.Visible = false;
					ShowInTaskbar = true;
					Topmost = true;
					Topmost = false;
					break;
			}
		}

		private void OnAddDataObject(string text)
		{
			RefreshListBox();
		}

		private void RefreshListBox()
		{
			_itemsSource.Clear();

			foreach (var text in _listener.Texts)
				if (string.IsNullOrWhiteSpace(_tbSearch.Text))
					_itemsSource.Add(new ListItem(text));
				else
				{
					if (text.IndexOf(_tbSearch.Text, StringComparison.InvariantCultureIgnoreCase) != -1)
						_itemsSource.Add(new ListItem(text));
				}
		}

		private void OnTimer(object state)
		{
			if (Dispatcher.CheckAccess())
				_listener.CheckClipboard();
			else
				Dispatcher.Invoke(() => OnTimer(state));
		}

		protected override void OnClosed(EventArgs e)
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}

			base.OnClosed(e);
		}

		private void OnListBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var selectedItem = _lb.SelectedItem as ListItem;
			_tb.Text = selectedItem?.Text;
		}

		private void OnClearClick(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show(this, "Clear?", "?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				_listener.Clear();
				RefreshListBox();
			}
		}

		private void OnSearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			RefreshListBox();
		}

		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			_tbSearch.Focus();
		}
	}

	public class ListItem
	{
		private const int MaxHeaderLength = 50;

		public string Text { get; }

		public ListItem(string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));
			Text = text;
		}

		public override string ToString()
		{
			var s = Text;
			if (s.Length > MaxHeaderLength)
				s = s.Substring(0, MaxHeaderLength);
			return s.Replace(Environment.NewLine, @" \n ");
		}
	}
}
