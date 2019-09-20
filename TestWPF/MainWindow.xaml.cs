using System;
using System.Collections.Generic;
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

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MouseHook mh;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mh_MouseDownEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                textBox.AppendText($"clicked {e.X},{e.Y}");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mh.UnHook();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mh = new MouseHook();
            mh.SetHook();
            mh.MouseDownEvent += mh_MouseDownEvent;
        }
    }
}
