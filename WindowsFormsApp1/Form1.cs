using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            System.Diagnostics.Process[] bunchProcesses = System.Diagnostics.Process.GetProcesses().Where(x => x.MainWindowTitle != "").ToArray();
            foreach (System.Diagnostics.Process item in bunchProcesses)
            {
                if (!item.ProcessName.Contains("devenv") && !item.ProcessName.Contains("explorer") && !item.ProcessName.Contains("TeamViewer"))
                {
                    textBox1.AppendText(item.ProcessName+"\n");
                }
            }
            System.Diagnostics.Process[] frameHost = System.Diagnostics.Process.GetProcessesByName("ApplicationFrameHost");
            foreach (System.Diagnostics.Process item in frameHost)
            {
                item.Kill();
            }
        }
    }
}
