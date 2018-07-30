using System;
using System.Windows;

namespace Kalantyr.SizeScanner
{
	public partial class App
	{
		public App()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var error = (Exception) e.ExceptionObject;
			ShowError(error);
		}

		internal static void ShowError(Exception error)
		{
			var text = error.Message + Environment.NewLine + Environment.NewLine + error;
			MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
