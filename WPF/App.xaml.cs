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
            bool isRelaunched = false;
            Ex.Log($"args count={e.Args.Length}");
            for (int i = 0; i != e.Args.Length; ++i)
            {
                Ex.Log($"arg={e.Args[i]};");
                if (e.Args[i].Contains("re1"))
                {
                    Ex.Log($"arg re1 found");
                    isRelaunched = true;                    
                }
            }

            MainWindow mainWindow = new MainWindow(isRelaunched);
            //mainWindow.WindowState = WindowState.Maximized;
            mainWindow.Show();            
        }
    }
}
