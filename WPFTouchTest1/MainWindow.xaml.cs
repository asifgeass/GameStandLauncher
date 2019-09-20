using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace WPFTouchTest1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr _hook;
        IntPtr _hwnd;
        NativeMethods.CBTProc _callback;
        TextBox _txtStatus;
        CheckBox _chkBox;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            // create an instance of the delegate that
            // won't be garbage collected to avoid:
            //   Managed Debugging Assistant 'CallbackOnCollectedDelegate' :** 
            //   'A callback was made on a garbage collected delegate of type 
            //   'WpfApp1!WpfApp1.MainWindow+NativeMethods+CBTProc::Invoke'. 
            //   This may cause application crashes, corruption and data loss. 
            //   When passing delegates to unmanaged code, they must be 
            //   kept alive by the managed application until it is guaranteed 
            //   that they will never be called.'
            _callback = this.CallBack;
            _hook = NativeMethods.SetWindowsHookEx(
                NativeMethods.HookType.WH_MOUSE,
                _callback,
                instancePtr: IntPtr.Zero,
                threadID: NativeMethods.GetCurrentThreadId());
            this.Closed += MainWindow_Closed;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _txtStatus = new TextBox()
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    MaxLines = 60,
                    IsReadOnly = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                _chkBox = new CheckBox()
                {
                    Content = "Divert mouse messages",
                    ToolTip = "try to move the mouse around and click on the window controls\r" +
                    "You can reenable the mouse by using the keyboard\r" +
                    "(spacebar when the chkbox has focus) to uncheck this option\r" +
                    "Notice that tooltips don't show when checkbox is checked"
                };
                var btn = new Button()
                {
                    Content = "Click Me",
                    ToolTip = "Now you see me",
                    Width = 150,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                btn.Click += (ob, eb) =>
                {
                    AddStatusMsg("Thanks, I needed that");
                };
                var sp = new StackPanel()
                {
                    Orientation = Orientation.Vertical
                };
                sp.Children.Add(
                  new Label()
                  {
                      Content = "Move the mouse around"
                  });
                sp.Children.Add(_chkBox);
                sp.Children.Add(btn);
                sp.Children.Add(_txtStatus);
                this.Content = sp;
                AddStatusMsg("Starting");
            }
            catch (Exception ex)
            {
                this.Content = ex.ToString();
            }
        }
        public void AddStatusMsg(string msg)
        {
            Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    var txt = DateTime.Now.ToString("MM/dd/yy HH:mm:ss:fff ") + msg;
                    _txtStatus.AppendText(txt + "\r\n");
                    _txtStatus.ScrollToEnd();
                }
                ));
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (_hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(this._hook);
            }
        }

        private IntPtr CallBack(int code, IntPtr wParam, IntPtr lParam)
        {
            var wind = Window.GetWindow(this);
            if (_hwnd == null)
            {
                _hwnd = (new WindowInteropHelper(wind)).EnsureHandle();
            }
            var mouseInfo = Marshal.PtrToStructure<NativeMethods.MOUSEHOOKSTRUCT>(lParam);
            AddStatusMsg($"{code} wParam {wParam} lParam {lParam} {mouseInfo}");
            if (_chkBox.IsChecked == true)
            {
                return new IntPtr(1); // non-zero indicates don't pass to target
            }
            return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }
    }
}
