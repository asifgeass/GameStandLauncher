using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Overlay.NET.Common;
using Process.NET;
using Process.NET.Memory;
using System.Threading;
using System.Threading.Tasks;

namespace Overlay.NET.Demo.Wpf {
    /// <summary>
    /// </summary>
    public class WpfOverlayExampleDemo
    {
        /// <summary>
        ///     The overlay
        /// </summary>
        private static OverlayPlugin _overlay;

        /// <summary>
        ///     The process sharp
        /// </summary>
        private static ProcessSharp _processSharp;

        /// <summary>
        ///     The work
        /// </summary>
        private static bool _work;

        /// <summary>
        ///     Starts the demo.
        /// </summary>        
        public void StartDemo() {
            Log.Debug(@"Please type the process name of the window you want to attach to, e.g 'notepad.");
            Log.Debug("Note: If there is more than one process found, the first will be used.");

            //Set up objects / overlay
            //var processName = Console.ReadLine();
            //var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            //if (process == null)
            //{
            //    Log.Warn($"No process by the name of {processName} was found.");
            //    Log.Warn("Please open one or use a different name and restart the demo.");
            //    Console.ReadLine();
            //    return;
            //}

            var process = System.Diagnostics.Process.Start("notepad.exe");
            Thread.Sleep(1000);

            //await Task.Delay(500);
            _processSharp = new ProcessSharp(process, MemoryType.Remote);
            _overlay = new WpfOverlayDemoExample();
            var wpfOverlay = (WpfOverlayDemoExample) _overlay;
            wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow);
            wpfOverlay.Enable();
            _work = true;
            var info = wpfOverlay.Settings.Current;
            wpfOverlay.OverlayWindow.Draw += OnDraw;                

            var task = new Task(() =>
            {
                while (_work)
                {
                    _overlay.Update();
                }
            });
            task.Start();            
            Log.Debug("Demo complete.");
        }

        private static void OnDraw(object sender, DrawingContext context) {
            // Draw a formatted text string into the DrawingContext.
            //context.DrawText(
            //    new FormattedText("Click Me!", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
            //        new Typeface("Verdana"), 36, Brushes.BlueViolet), new Point(200, 116));

            //context.DrawLine(new Pen(Brushes.Blue, 10), new Point(100, 100), new Point(10, 10));
        }
    }
}