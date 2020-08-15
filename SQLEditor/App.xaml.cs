using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Common;

namespace SQLEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //user closed the app or ShutDown has been called
            Globals.DbConnection?.Dispose();
            Globals.Pswd?.Dispose();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = $"An unhandled exception occurred:\r\n{e.Exception.Message}";

            //show MessageBox and block until the user closes it
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Prevent default unhandled exception processing
            e.Handled = true;

            if (!MainWindow.IsLoaded)
            {
                //if the main window has not been loaded yet, shut it down, 
                //otherwise the user would have to kill the app in the task manager
                Shutdown();
            }
        }
    }
}
