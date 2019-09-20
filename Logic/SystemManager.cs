using DisableDevice;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProcessSD = System.Diagnostics.Process;

namespace Logic
{
    public class SystemManager
    {
        public static event Action OnSensorFound = DefaultOnSensorFound; //delegate { };
        public static event Action OnBeforeRelaunchApp = delegate { };

        #region Fields
        private static bool isSensorActiveOnLaunch = false;
        private static bool isShowTaskbarOnExit = true;
        #endregion
        #region DLLImport
        [DllImport("user32.dll")]
        static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;
        #endregion

        #region Properties
        public static bool isRelaunched { get; set; }
        #endregion

        #region Public Methods
        public static void OnWindowLoaded()
        {
            Ex.Catch("Попытка скрыть панель задач", () =>
            {
#pragma warning disable CS0162 // Unreachable code detected
                if (false) Taskbar.Hide();
#pragma warning restore CS0162 // Unreachable code detected
#if DEBUG
#else
                Taskbar.Hide();
#endif
            });
            
            Ex.Log("Taskbar.Hide() passed.");
            isSensorActiveOnLaunch = DeviceManagerApi.IsSensorExist();
            GameManager.KillAllGames().RunParallel();
            CheckSensor().RunParallel();
            //mh = new MouseHookAdapter();
            if (isSensorActiveOnLaunch == false)
            {
                OnSensorFound += RelaunchApp;
            }
            ComPort.PortReader().RunParallel();
            try
            {
                //GameManager.DisableWPFTabletSupport();
            }
            catch (Exception ex)
            { ex.Log("Попытка отключить стилус"); }
            Ex.Log($"isRelaunched={isRelaunched}");
            Ex.Log($"isShowTaskbarOnExit={isShowTaskbarOnExit}");
            SetRegDisableSwipeEdgeMachine();
        }
        public static void OnWindowClosed()
        {
            if (isShowTaskbarOnExit)
            {
                Ex.Catch("Попытка вернуть видимость панели задач", () =>
                {
                    Taskbar.Show();
                });
            }
            //mh?.UnHook();
            try
            {
                ComPort.port.Close();
            }
            catch { }
            Environment.Exit(0);
        }
        public static void CheckInactive()
        {

        }

        public static async void RelaunchApp()
        {
            if (isRelaunched == false)
            {
                OnBeforeRelaunchApp();
                var path = Application.ResourceAssembly.Location;
                Ex.Log($"рестарт программы={path}");
                System.Diagnostics.Process.Start(path, "re1");
                isShowTaskbarOnExit = false;
                await Task.Delay(1500);
                Application.Current.Shutdown();
            }
        }
        public static void WakeMonitor()
        {
            Ex.Log("Wake Monitor");
            WakeUpScreenSaver();
            //MouseMove();            
        }

        public static async Task CheckSensor()
        {
            Task checking = Task.CompletedTask;
            CancellationTokenSource cancelTokenSource = null;
            short count = 0;
            while (true)
            {
                await Task.Delay(2000);
                Ex.TryLog("Попытка скрыть панель задач", () =>
                {
#pragma warning disable CS0162 // Unreachable code detected
                    if (false) Taskbar.Hide();
#pragma warning restore CS0162 // Unreachable code detected
#if DEBUG
#else
                    Taskbar.Hide();
#endif
                });
                cancelTokenSource = cancelTokenSource ?? new CancellationTokenSource();
                bool isTaskComplete = checking.IsCompleted;
                //Ex.Log($"isTaskComplete={checking.Status}({checking.IsCompleted})");
                bool isFound = DeviceManagerApi.IsSensorExist();
                //bool isFound = boolTest; //TEST
                if (isFound)
                {
                    if (count < 3) { Ex.Log("сенсор найден"); }
                    if (count < 5) { count++; }
                    if (isTaskComplete)
                    {
                        OnSensorFound();
                        Ex.Log("сенсор подключился");
                        CancellationToken token = cancelTokenSource.Token;
                        checking = CheckBrowser(token);
                        Ex.Log($"cheking browser task status={checking.Status}({checking.IsCompleted})");
                    }
                }
                else
                {
                    //Ex.Log("не найден сенсор. посылаем токен отмены");
                    cancelTokenSource.Cancel();
                    cancelTokenSource = null;
                }
            }
        }

        public static async Task CheckBrowser(CancellationToken token)
        {
            bool isWork = true;
            while (isWork)
            {
                KillBrowser();
                await Task.Delay(330);
                //Ex.Log($"token.IsCancellationRequested={token.IsCancellationRequested}");
                if (token.IsCancellationRequested)
                {
                    Ex.Log("получен токен отмены");
                    isWork = false;
                }
            }
            Ex.Log("закончился таск чекбраузер");
        }

        public static void KillBrowser()
        {
            var bunchProcesses = System.Diagnostics.Process.GetProcessesByName("chrome");//.Where(x => x.MainWindowTitle != "");
            Parallel.ForEach(bunchProcesses, item =>
            {
                Ex.Try(false, () => item.Kill());
            });
        }

        public static async Task KillExplorer()
        {
            //return; //TEST
            RegistryKey ourKey = null;
            try
            {
                ourKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                ourKey?.SetValue("AutoRestartShell", 0);
            }
            catch (Exception ex)
            {
                ex.Log("Ошибка при попытке влезть в реестр и убить explorer");
            }
            var explorer = ProcessSD.GetProcessesByName("explorer");
            await Task.Delay(500);
            foreach (var item in explorer)
            {
                Ex.Try(() =>
                {
                    item.Kill();
                    item.WaitForExit();
                });
            }
            //await Task.Delay(1000);
            //ourKey?.SetValue("AutoRestartShell", 1);
        }

        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this window.  
            System.Windows.Input.TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, System.Windows.Input.InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the device class.
                    Type devicesType = devices.GetType();

                    // Loop until there are no more devices to remove.
                    int count = devices.Count + 1;

                    while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.
                        devicesType.InvokeMember("HandleTabletRemoved", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, devices, new object[] { (uint)0 });

                        count--;

                        if (devices.Count != count)
                        {
                            throw new Exception("Unable to remove real-time stylus support.");
                        }
                    }
                }
            }
        }

        public static void SetRegDisableSwipeEdgeMachine()
        {
            try
            {
                RegistryKey key = RegPath.GetCreatePath(RegPath.RegSwipeEdge, 1, true);
                key.SetValue(RegPath.swipeRegValue, 0, RegistryValueKind.DWord);
                key.Close();
            }
            catch
            {
            }
        }
        #endregion

        #region Private Methods
        private static void WakeUpScreenSaver()
        {
            if (ScreenSaverController.GetScreenSaverRunning())
            {
                Ex.Log("GetScreenSaverRunning==true");
                ScreenSaverController.KillScreenSaver();
            }
        }

        private static void MouseMove()
        {
            Ex.Log("Try to wake up (move mouse)");
            mouse_event(MOUSEEVENTF_MOVE, 40, 0, 0, UIntPtr.Zero);
        }

        private static void DefaultOnSensorFound()
        {
            WakeMonitor();
        }
        #endregion
    }
}
