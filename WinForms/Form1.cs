using DisableDevice;
using Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms
{
    public partial class Form1 : Form
    {
        string ShortcutDirectory = @"C:\Work\Shortcuts";
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await GameManager.RunGame(listBox1.SelectedItem.ToString());
        }

        private async void buttonSHG8200_Click(object sender, EventArgs e)
        {
            string guidPanel = "745a17a0-74d3-11d0-b6fe-00a0c90f57da";
            //var getx = DeviceManagerApi.GetInstancePaths(guidPanel);
            //await Task.Delay(5000);
            //DeviceManagerApi.ToogleSHG8200(true);
            //var hz = DeviceManagerApi.IsSensorExist();
            buttonSHG8200.Text = DeviceManagerApi.DeviceCount().ToString();
        }

        private async void buttonComDevice_Click(object sender, EventArgs e)
        {
            DeviceManagerApi.ToogleComPortDevice(new System.IO.Ports.SerialPort("COM1"), false);
            await Task.Delay(5000);
            DeviceManagerApi.ToogleComPortDevice(new System.IO.Ports.SerialPort("COM1"), true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenDialog();
        }

        private void OpenDialog()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите директорию";
                dialog.SelectedPath = textBox1.Text;
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = dialog.SelectedPath;
                    var allFiles = Directory.GetFiles(dialog.SelectedPath);
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(allFiles);
                }                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                GameManager.TestRun(listBox1.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n"+ex.StackTrace);
            }
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //mh.UnHook();
        }
    }
}
