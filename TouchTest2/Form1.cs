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

namespace TouchTest2
{
    public partial class Form1 : Form
    {
        WM_TouchHook hook;
        Process proc;
        IntPtr hwnd;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void Hook_TouchDown(object sender, TouchHook.TouchEventArgs e)
        {
            //richTextBox1.AppendText($"clicked {e.x},{e.y}\n");
            richTextBox1.AppendText($"clicked\n");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            hook.UninstallHook();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            proc = Process.GetCurrentProcess();
            //proc = Process.Start($@"C:\Work\Shortcuts\Launch Shantae and the Pirate's Curse.lnk");
            hwnd = proc.MainWindowHandle;
            hook = new WM_TouchHook(hwnd, HookType.WH_GETMESSAGE);
            hook.InstallHook();
            hook.TouchDown += Hook_TouchDown;
        }
    }
}
