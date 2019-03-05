using System;
using System.Windows;
using System.Windows.Input;
using Timer.Properties;

namespace Timer
{
	public partial class MainWindow
	{
		private TimerData _timerData;
		private System.Threading.Timer _timer;

		public MainWindow()
		{
			InitializeComponent();

			Loaded += MainWindow_Loaded;

			Unloaded += (sender, e) =>
			{
				_timer?.Dispose();
			};
		}

		private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_timer = new System.Threading.Timer(TimerCallback, this, TimeSpan.Zero, TimeSpan.FromSeconds(0.5));

			_timerData = new TimerData(DateTime.Now, Settings.Default.Duration);
		}

		private void TimerCallback(object state)
		{
			if (_timerData == null)
				return;

			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => TimerCallback(state));
				return;
			}

			_textBox.Text = ToText(_timerData.Remain);
		}

		private static string ToText(TimeSpan t)
		{
			return t.Minutes + ":" + t.Seconds.ToString("##");
		}

		private void OnResetClick(object sender, RoutedEventArgs e)
		{
			_timerData.Reset();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Handled)
				return;

			if (e.IsDown)
				switch (e.Key)
				{
					case Key.F5:
					case Key.R:
						_timerData.Reset();
						e.Handled = true;
						break;
				}
		}
	}
}
