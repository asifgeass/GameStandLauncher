using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TouchHook;

namespace MouseHook2
{
    public partial class Form1 : Form
    {
        WM_MouseHook mh;
        public Form1()
        {
            InitializeComponent();
            var proc = Process.Start($@"C:\Work\Shortcuts\AngryBirdsSpace.exe.lnk");
            mh = new WM_MouseHook(proc.MainWindowHandle);
            mh.MouseDown += Mh_MouseDown;
            mh.InstallHook();     
        }

        private void Mh_MouseDown(object sender, TouchHook.MouseEventArgs e)
        {
            richTextBox1.AppendText($"clicked {e.x},{e.y}\n");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            mh.UninstallHook();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}
