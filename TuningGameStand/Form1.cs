using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logic;

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
            AppPath = Application.ExecutablePath.Replace(SettingsAppName, AppName);
            ReadRegistry();
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
                ReadSwipeEdgeUser();
                ReadSwipeEdgeMachine();
            }
            catch (Exception ex)
            {

                Show(ex.Message);
            }
        }

        private void ReadSwipeEdgeMachine()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(RegSwipeEdge, true);

            if (key == null)
            { checkBoxSwipeEdgeUser.Checked = false; return; }

            string data = key.GetValue("AllowEdgeSwipe")?.ToString();
            key.Close();

            if (data == null)
            { checkBoxSwipeEdgeUser.Checked = false; return; }

            checkBoxAutoStart.Checked = data == "0";
        }

        private void ReadSwipeEdgeUser()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegSwipeEdge, true);

            if (key == null)
            { checkBoxSwipeEdgeUser.Checked = false; return; }

            string data = key.GetValue(swipeRegValue)?.ToString();
            key.Close();

            if (data == null)
            { checkBoxSwipeEdgeUser.Checked = false; return; }

            checkBoxAutoStart.Checked = data == "0";
        }

        private void ReadAutostartReg()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegAutostart, true);
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
            { throw new Exception($@"Путь реестра не найден:\n'{RegAutostart}'\nvoid SetAutostartReg(bool isChecked)"); }
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
                Show(ex.Message);
            }
        }

        private void checkBoxSwipeEdgeUser_Click(object sender, EventArgs e)
        {
            try
            {
                SetRegDisableSwipeEdgeUser(checkBoxSwipeEdgeUser.Checked);
            }
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
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
    }
}
