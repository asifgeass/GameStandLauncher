using ExceptionManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool isRe1Param = false;
            Ex.Log($"App.cs: args count={e.Args.Length}");
            for (int i = 0; i != e.Args.Length; ++i)
            {
                Ex.Log($"App.cs: arg={e.Args[i]};");
                if (e.Args[i].Contains("re1"))
                {
                    Ex.Log($"App.cs: arg re1 found");
                    isRe1Param = true;                    
                }
            }

            MainWindow mainWindow = new MainWindow(isRe1Param);
            //mainWindow.WindowState = WindowState.Maximized;
            mainWindow.Show();            
        }
    }
}
