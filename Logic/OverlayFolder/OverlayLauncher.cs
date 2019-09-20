using Overlay.NET;
using Process.NET;
using Process.NET.Memory;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Logic
{
    /// <summary>
    /// </summary>
    public class OverlayLauncher
    {
        /// <summary>
        ///     The overlay
        /// </summary>
        private OverlayPlugin _overlay;

        /// <summary>
        ///     The process sharp
        /// </summary>
        private ProcessSharp _processSharp;

        /// <summary>
        ///     The work
        /// </summary>
        public bool _work;

        public void Start(System.Diagnostics.Process process)
        {
            if (process == null)
            {
                return;
            }
            Thread.Sleep(3000);
            var update = new Thread(() =>
            {
                try
                {
                    _processSharp = new ProcessSharp(process, MemoryType.Remote);
                    _overlay = new OverlayLayer();
                    var wpfOverlay = (OverlayLayer)_overlay;
                    wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow);
                    wpfOverlay.Enable();

                    _work = true;
                    var info = wpfOverlay.Settings.Current;
                    wpfOverlay.OverlayWindow.Draw += OnDraw;
                    Ex.Log($"Overlay Started Successfully = {process.MainWindowTitle}({process.ProcessName})");
                    while (_work)
                    {
                        _overlay.Update();
                        Thread.Sleep(5);
                        _work = wpfOverlay.OverlayWindow.isExist;
                    }
                    Ex.Log($"Overlay Finished = {process.MainWindowTitle}({process.ProcessName})");
                }
                catch (Exception ex)
                {
                    ex.Log("Overlay Finished with Ex");
                }
            });
            update.SetApartmentState(ApartmentState.STA);
            update.Start();
        }

        private void OnDraw(object sender, DrawingContext context)
        {
            // Draw a formatted text string into the DrawingContext.
            //context.DrawText(
            //    new FormattedText("Click Me!", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
            //        new Typeface("Verdana"), 36, Brushes.BlueViolet), new Point(200, 116));

            //context.DrawLine(new Pen(Brushes.Blue, 10), new Point(100, 100), new Point(10, 10));
        }
    }
}