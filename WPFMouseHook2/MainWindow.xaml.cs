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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TouchHook;

namespace WPFMouseHook2
{
    public partial class MainWindow : Window
    {
        IntPtr _hwnd;
        WM_MouseHook mh;
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var proc = Process.Start($@"C:\Work\Shortcuts\AngryBirdsSpace.exe.lnk");
            await Task.Delay(1000);
            _hwnd = Process.GetCurrentProcess().MainWindowHandle;
            mh = new WM_MouseHook(_hwnd);
            mh.InstallHook();
            mh.MouseDown += Mh_MouseDown;
            mh.MouseMove += delegate { };
        }

        private void Mh_MouseDown(object sender, TouchHook.MouseEventArgs e)
        {
            textBox.AppendText($"clicked\n");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mh.UninstallHook();
        }
    }
}
