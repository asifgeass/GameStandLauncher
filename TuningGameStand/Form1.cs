using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuningGameStand
{
    public partial class Form1 : Form
    {
        private string AppPath = null;
        private readonly string AppName = "GameStand";
        private readonly string SettingsAppName = "TuningGameStand";
        private readonly string RegAutostart = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private readonly string RegSwipeEdge = @"SOFTWARE\Policies\Microsoft\Windows\EdgeUI";
        private readonly string RegExplorerRestart = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
        private readonly string swipeRegValue = "AllowEdgeSwipe";
        private readonly string explorerRestartRegValue = "AutoRestartShell";
        public Form1()
        {
            InitializeComponent();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            AppPath = Path.Combine(baseDir, $"{AppName}.exe");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegBlockNotepad();
        }

        private void RegBlockNotepad()
        {
            string path = @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun";
            ChangeRegistry(path, "1", "notepad.exe", 3);
        }

        private void ChangeRegistry(string path, string name, object data, int count=1)
        {
            try
            {
                RegistryKey key = RegPath.GetCreatePath(path, count);
                key.SetValue(name, data); //sets 'someData' in 'someValue'
                key.Close();
            }
            catch (Exception ex)
            {
                Show(ex.Message);
            }         
        }

        private void ReadRegistry()
        {
            try
            {
                ReadAutostartReg();
                ReadSwipeEdgeReg();
            }
            catch (Exception ex)
            {

                Show(ex.Message);
            }
        }

        private void ReadSwipeEdgeReg()
        {
            try
            {
                ReadSwipeEdgeMachine();
            }
            catch (Exception ex)
            {

                Show(ex.Message);
            }
        }

        private void ReadSwipeEdgeMachine()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(RegSwipeEdge);
            checkBoxSwipeEdgeMachine.Checked = false;

            if (key == null)
            { return; }

            string data = key.GetValue(swipeRegValue)?.ToString();
            key.Close();

            if (data == null)
            { return; }

            checkBoxSwipeEdgeMachine.Checked = data == "0";
        }

        private void ReadAutostartReg()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegAutostart);
            if (key == null)
            {
                checkBoxAutoStart.Checked = false;
                return;
            }
            string data = key.GetValue("GameStand")?.ToString();
            key.Close();
            if (data == null)
            { checkBoxAutoStart.Checked = false; return; }
            checkBoxAutoStart.Checked = data == AppPath;
        }

        private void SetAutostartReg(bool isChecked)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegAutostart, true);
            if(key==null)
            { throw new Exception($"Путь реестра не найден:\n'{RegAutostart}'\nvoid SetAutostartReg(bool isChecked)"); }
            if(!File.Exists(AppPath))
            { throw new Exception($"Файл не найден:\n'{AppPath}'\nTuningGameStand.exe должен находиться в папке GameStand."); }
            if (isChecked)
            {
                key.SetValue(AppName, AppPath);                
            }
            else
            {
                key.DeleteValue(AppName);
            }
            key.Close();
        }

        private static void Show(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void checkBoxAutoStart_Click(object sender, EventArgs e)
        {
            try
            {
                SetAutostartReg(checkBoxAutoStart.Checked);
            }
            catch (Exception ex)
            {
                checkBoxAutoStart.Checked = !checkBoxAutoStart.Checked;
                Show(ex.Message);
            }
        }
        private void SetRegDisableSwipeEdgeUser(bool isChecked)
        {
            RegistryKey key = RegPath.GetCreatePath(RegSwipeEdge);
            if (isChecked)
            {
                key.SetValue(swipeRegValue, 0 , RegistryValueKind.DWord);
            }
            else
            {
                key.DeleteValue(swipeRegValue);
            }
            key.Close();
        }

        private void checkBoxSwipeEdgeMachine_Click(object sender, EventArgs e)
        {
            try
            {
                SetRegDisableSwipeEdgeMachine(checkBoxSwipeEdgeMachine.Checked);
            }
            catch (SecurityException ex1)
            {
                checkBoxSwipeEdgeMachine.Checked = !checkBoxSwipeEdgeMachine.Checked;
                Show(ex1.Message+"\n\nЗапустите от имени администратора для изменения реестра.");
            }
            catch (Exception ex)
            {
                checkBoxSwipeEdgeMachine.Checked = !checkBoxSwipeEdgeMachine.Checked;
                Show(ex.Message);
            }            
        }

        private void SetRegDisableSwipeEdgeMachine(bool isChecked)
        {
            RegistryKey key = RegPath.GetCreatePath(RegSwipeEdge, 1, true);
            if (isChecked)
            {
                key.SetValue(swipeRegValue, 0, RegistryValueKind.DWord);
            }
            else
            {
                key.DeleteValue(swipeRegValue);
            }
            key.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadRegistry();
        }
    }
}
