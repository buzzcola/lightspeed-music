using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Lightspeed;

namespace LightspeedInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {            
            // handles the dreaded XamlParseException providing extra info.
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            base.OnStartup(e);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            const string CAPTION = "Lightspeed Error";
            const string ERRORMSG_FMT = "Lightspeed has encountered a serious problem.  Here's some more information:{0}{0}{1}";
            var errorMessage = String.Format(ERRORMSG_FMT, Environment.NewLine, e.Exception);
            MessageBox.Show(errorMessage, CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            if (Properties["MainWindow"] != null)
            {
                // save some userproperties on exit.
                var us = UserSettings.Instance;
                var mainWindow = Properties["MainWindow"] as MainWindow;
                var keyboardWindow = Properties["KeyboardWindow"] as Keyboard;

                if (keyboardWindow == null || !keyboardWindow.IsLoaded)
                {
                    // keyboard window closed, or never loaded.
                    us.ShowVirtualKeyboard = false;
                }
                else
                {
                    us.ShowVirtualKeyboard = true;
                    us.WindowLocation_VirtualKeyboard = new Point(keyboardWindow.Left, keyboardWindow.Top);
                }

                us.WindowLocation_Main = new Point(mainWindow.Left, mainWindow.Top);
                us.WindowSize_Main = new Size(mainWindow.Width, mainWindow.Height);

                us.Save();
            }

            UserSettings.Instance.DisposeDevices();            
            base.OnExit(e);
        }
    }
}
