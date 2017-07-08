using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace ClipboardTool
{
	public partial class MainWindow
	{
		private Timer _timer;
		private readonly ClipboardListener _listener = new ClipboardListener();

		private static readonly TimeSpan TimerPeriod = TimeSpan.FromSeconds(1);
		private readonly ObservableCollection<ListItem> _itemsSource = new ObservableCollection<ListItem>();

		public MainWindow()
		{
			InitializeComponent();

			_lb.ItemsSource = _itemsSource;
			_listener.Add = OnAddDataObject;

			Loaded += (sender, e) =>
			{
				_timer = new Timer(OnTimer, null, TimeSpan.Zero, TimerPeriod);
			};
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
				_itemsSource.Clear();
			}
		}

		private void OnSearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			RefreshListBox();
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
