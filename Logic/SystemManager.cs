using DisableDevice;
using ExceptionManager;
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
        #region events
        public static event Action OnSensorFound = delegate { };
        public static event Action OnBeforeRelaunchApp = delegate { };
        public static event Action OnScreenSaverDetected = delegate { };
        public static event Action OnSwipesEnabledWarning = delegate { };
        #endregion

        #region Fields
        private static bool isSensorWasActiveOnLaunch = false;
        private static bool isShowTaskbarOnExit = true;
        #endregion

        #region DLLImport
        [DllImport("user32.dll")]
        static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;
        #endregion

        static SystemManager()
        {
            OnSensorFound += WakeMonitor;
            //OnScreenSaverDetected += async () => GameManager.KillAllGames().RunParallel();
        }

        #region Properties
        public static bool isRe1ParamExist { get; set; }
        #endregion

        #region Public Methods
        public static async Task OnWindowLoadedAsync()
        {
            Ex.Log("SystemManager.OnWindowLoadedAsync()");
            Ex.Catch("SystemManager: Попытка скрыть панель задач", () =>
            {
#pragma warning disable CS0162 // Unreachable code detected
                if (false) Taskbar.Hide();
#pragma warning restore CS0162 // Unreachable code detected
#if DEBUG
#else
                Taskbar.Hide();
#endif
            });
            
            Ex.Log("SystemManager.Taskbar.Hide() passed.");

            if (!isRe1ParamExist) isSensorWasActiveOnLaunch = await DeviceManagerApi.IsSensorExistAsync();
            else isSensorWasActiveOnLaunch = true;

            Ex.Log($"SystemManager.isSensorWasActiveOnLaunch={isSensorWasActiveOnLaunch}");
            GameManager.KillAllGames().RunParallel();
            CheckSensor().RunParallel();
            //mh = new MouseHookAdapter();
            if (isSensorWasActiveOnLaunch == false)
            {
                OnSensorFound += RelaunchApp;
            }
            ComPort.PortReader().RunParallel();
            Ex.Log($"SystemManager.isRelaunched={isRe1ParamExist}");
            Ex.Log($"SystemManager.isShowTaskbarOnExit={isShowTaskbarOnExit}");
            SetRegDisableSwipeEdgeMachine();
            OnScreenSaverDetected += async () => await GameManager.KillAllGames();
            CheckScreenSaver().RunParallel();
            WarningSwipe();
        }

        public static async void RelaunchApp()
        {
            if (isRe1ParamExist == false) //Change/reverse relaunch logic HERE. false: relaunch without param
            {
                Ex.Log("SystemManager.RelaunchApp()");
                OnBeforeRelaunchApp();
                var path = Application.ResourceAssembly.Location; //AppDomain.CurrentDomain.BaseDirectory           
                Ex.Log($"SystemManager.RelaunchApp(): path:\n{path}");                
                System.Diagnostics.Process.Start(path, "re1"); //if (isRe1ParamExist == false)
                //System.Diagnostics.Process.Start(path); //if (isRe1ParamExist == true)
                isShowTaskbarOnExit = false;
                await Task.Delay(1500);
                Application.Current.Shutdown();
            }
        }

        private static void WarningSwipe()
        {
            Ex.Log("SystemManager.WarningSwipe()");
            StringBuilder msg = new StringBuilder("ВНИМАНИЕ! Не отключены свайпы границ экрана в Windows, что нарушает безопасность.\n\n");
            msg.AppendLine("Воспользуйтесь TuningGameStand.exe от имени администратора для отключения свайпов.\n");
            msg.AppendLine("Для отключения этого сообщения (не рекомендуется) в файле settings.ini выставьте параметр DisableSwipeWarning=1.\n");
            OnSwipesEnabledWarning += () => Ex.Show(msg.ToString());

            RegPath.ReadSwipeEdgeMachine();
            var set = new SavingManager(Where.local);
            bool isForceDisable = set.Key(Setting.DisableSwipeWarning).ValueBool;
            set.Save();
            if (RegPath.isDisabledSwipes==false && isForceDisable==false)
            {
                Ex.Log("SystemManager.OnSwipesEnabledWarning()");
                OnSwipesEnabledWarning();
            }
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
        public static async Task CheckScreenSaver()
        {
            Ex.Log("SystemManager.CheckScreenSaver()");
            bool isScreensaverRuning;
            bool isDoneOnce = false;
            while (true)
            {
                isScreensaverRuning = ScreenSaverController.GetScreenSaverRunning();
                if(!isScreensaverRuning)
                {
                    isDoneOnce = false;
                }
                if (isScreensaverRuning && !isDoneOnce)
                {
                    OnScreenSaverDetected();
                    isDoneOnce = true;
                    await Task.Delay(30000);
                }
                await Task.Delay(2000);
            }
            Ex.Log("CheckScreenSaver() while(true) finished.");
        }

        public static void WakeMonitor()
        {
            //Ex.Log("Wake Monitor");
            WakeUpScreenSaver();
            //MouseMove();            
        }

        public static async Task CheckSensor()
        {
            Ex.Log("SystemManager.CheckSensor()");
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
                bool isFound = await DeviceManagerApi.IsSensorExistAsync();
                //bool isFound = boolTest; //TEST
                if (isFound)
                {
                    if (count < 3) { Ex.Log("Task CheckSensor(): Sensor Found"); }
                    if (count < 5) { count++; }
                    if (isTaskComplete)
                    {
                        Ex.Log("Task CheckSensor(): Sensor Connected");
                        OnSensorFound();                        
                        CancellationToken token = cancelTokenSource.Token;
                        checking = CheckBrowser(token);
                        checking.Start();
                        Ex.Log($"Task CheckSensor(): Task CheckBrowser IsCompleted={checking.IsCompleted}; Status={checking.Status}");
                    }
                }
                else
                {
                    //Ex.Log("не найден сенсор. посылаем токен отмены");
                    cancelTokenSource.Cancel();
                    cancelTokenSource = null;
                }
            }
            Ex.Log("Task CheckSensor(): while(true) finished");
        }

        public static async Task CheckBrowser(CancellationToken token)
        {
            bool isWork = true;
            Ex.Log("SystemManager.CheckBrowser()");
            while (isWork)
            {
                KillBrowser();
                await Task.Delay(600);
                //Ex.Log($"token.IsCancellationRequested={token.IsCancellationRequested}");
                if (token.IsCancellationRequested)
                {
                    Ex.Log("SystemManager.CheckBrowser(): Got Cancel Token");
                    isWork = false;
                }
            }
            Ex.Log("SystemManager.CheckBrowser() Finished.");
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
            Ex.Log("SystemManager.SetRegDisableSwipeEdgeMachine()");
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
                Ex.Log("WakeUpScreenSaver() ScreenSaverRunning==true");
                ScreenSaverController.KillScreenSaver();
            }
        }

        private static void MouseMove()
        {
            Ex.Log("Try to wake up (move mouse)");
            mouse_event(MOUSEEVENTF_MOVE, 40, 0, 0, UIntPtr.Zero);
        }
        #endregion
    }
}
