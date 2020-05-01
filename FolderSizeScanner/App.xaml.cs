using System;
using System.Windows;

namespace FolderSizeScanner
{
    public partial class App
    {
        public static void ShowError(Exception error)
        {
            MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static Window GetWindow(FrameworkElement userControl)
        {
            if (userControl == null) throw new ArgumentNullException(nameof(userControl));

            if (userControl is Window w2)
                return w2;

            var uc = userControl;
            var w = uc.Parent as Window;

            while (w == null)
            {
                uc = (FrameworkElement)uc.Parent;

                if (uc == null)
                    return App.Current.MainWindow;

                w = uc.Parent as Window;
            }

            return w;
        }
    }
}
