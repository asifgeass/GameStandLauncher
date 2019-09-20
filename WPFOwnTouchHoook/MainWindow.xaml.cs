using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TouchHook;

namespace WPFOwnTouchHoook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowsHook mh;
        Process proc;
        IntPtr hwnd;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mh.UninstallHook();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.Clear();
            //proc = Process.Start($@"C:\Work\Shortcuts\Launch Shantae and the Pirate's Curse.lnk");
            proc = Process.GetCurrentProcess();
            //proc = Process.Start($@"notepad.exe");
            textBox.AppendText($"proc_hndl={proc.MainWindowHandle}\n");
            await Task.Delay(1000);
            hwnd = proc.MainWindowHandle;
            textBox.AppendText($"hwnd={hwnd}\n");
            mh = new WindowsHook(hwnd, HookType.WH_GETMESSAGE);
            mh.InstallHook();            
            //mh.MouseDown += Mh_MouseDown;
            //mh.MouseMove += delegate { };                                
        }

        private void Mh_MouseDown(object sender, TouchHook.MouseEventArgs e)
        {
            textBox.AppendText($"clicked\n");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();
        }
    }
}
