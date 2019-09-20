using Overlay.NET.Common;
using Overlay.NET.Wpf;
using Process.NET.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
//using OverlayWindow = Overlay.NET.Wpf.OverlayWindow;

namespace Logic
{
    //[RegisterPlugin("WpfOverlayDemo-1", "Jacob Kemple", "WpfOverlayDemo", "0.0", "A basic demo of the WPF overlay.")]
    public class OverlayLayer : WpfOverlayPlugin
    {
        public ISettings<OverlaySettings> Settings { get; } = new SerializableSettings<OverlaySettings>();
        // Used to limit update rates via timestamps 
        // This way we can avoid thread issues with wanting to delay updates
        private readonly TickEngine _tickEngine = new TickEngine();
        private bool _isDisposed;
        private bool _isSetup;
        private bool needToShow = true;

        public override void Enable()
        {
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        public override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Initialize(IWindow targetWindow)
        {
            // Set target window by calling the base method
            base.Initialize(targetWindow);

            OverlayWindow = new OverlayWindow(targetWindow);

            // For demo, show how to use settings
            var current = Settings.Current;
            var type = GetType();

            current.UpdateRate = 1000 / 5;
            current.Author = GetAuthor(type);
            current.Description = GetDescription(type);
            current.Identifier = GetIdentifier(type);
            current.Name = GetName(type);
            current.Version = GetVersion(type);

            // File is made from above info
            //Settings.Save();
            //Settings.Load();

            // Set up update interval and register events for the tick engine.
            _tickEngine.Interval = Settings.Current.UpdateRate.Milliseconds();
            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            // This will only be true if the target window is active
            // (or very recently has been, depends on your update rate)
            if (OverlayWindow.IsVisible)
            {
                OverlayWindow.Update();
            }
        }

        private void OnPreTick(object sender, EventArgs eventArgs)
        {
            // Only want to set them up once.
            if (!_isSetup)
            {
                SetUp();                
                _isSetup = true;
                return;
            }

            var isActive = TargetWindow.IsActivated;
            var isVisible = !OverlayWindow.IsVisible;

            //if (visible)
            //{
            //OverlayWindow.Show();
            //Ex.Log($"activ={isActive}; visible={isVisible};");
            if (isActive) //needToShow
            {
                if (true) //isActive
                {
                    OverlayWindow.Show();
                    //OverlayWindow.ActivateParent();
                    needToShow = false;
                }
            }
            if(!isActive)
            {
                OverlayWindow.Hide();
                needToShow = true;
            }
                //if (_firstShow) { OverlayWindow.ActivateParent(); _firstShow = false; }
            //}
            //else { OverlayWindow.Hide(); }


            //Ensure window is shown or hidden correctly prior to updating
            //if (activated && visible)
            //{
            //    OverlayWindow.Show();
            //    if (!_isSetup) { OverlayWindow.SetFocus(); }
            //}

            //else if (!activated && !visible)
            //{
            //    OverlayWindow.Hide();
            //}            
        }

        public override void Update() => _tickEngine.Pulse();
        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (IsEnabled)
            {
                Disable();
            }
            try
            {
                OverlayWindow?.Hide();
                OverlayWindow?.Close();
            }
            catch (Exception ex)
            {
                ex.Log("Ошибка в OverlayLayer.Dispose() на строке OverlayWindow.Hide() или Close()");
            }            
            OverlayWindow = null;
            _tickEngine.Stop();
            Settings.Save();

            base.Dispose();
            _isDisposed = true;
        }
        ~OverlayLayer()
        {
            Dispose();
        }
        private void SetUp()
        {
            var button = new Image
            {
                Margin = new Thickness(0, 0, 0, 0),
                Width = 50,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Source = Properties.Resources.close2.ToImageSource()
            };
            OverlayWindow.Add(button);
            #region shapes
            //var _polygon = new Polygon
            //{
            //    Points = new PointCollection(5) {
            //        new Point(100, 150),
            //        new Point(120, 130),
            //        new Point(140, 150),
            //        new Point(140, 200),
            //        new Point(100, 200)
            //    },
            //    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
            //    Fill =
            //        new RadialGradientBrush(
            //            Color.FromRgb(255, 255, 0),
            //            Color.FromRgb(255, 0, 255))
            //};

            //OverlayWindow.Add(_polygon);

            //// Create a line
            //_line = new Line
            //{
            //    X1 = 100,
            //    X2 = 300,
            //    Y1 = 200,
            //    Y2 = 200,
            //    Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0)),
            //    StrokeThickness = 2
            //};

            //OverlayWindow.Add(_line);

            //// Create an ellipse (circle)
            //_ellipse = new Ellipse
            //{
            //    Width = 15,
            //    Height = 15,
            //    Margin = new Thickness(300, 300, 0, 0),
            //    Stroke =
            //        new SolidColorBrush(Color.FromRgb(0, 255, 255))
            //};

            ////OverlayWindow.Add(_ellipse);

            //// Create a rectangle
            //_rectangle = new Rectangle
            //{
            //    RadiusX = 2,
            //    RadiusY = 2,
            //    Width = 50,
            //    Height = 100,
            //    Margin = new Thickness(400, 400, 0, 0),
            //    Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
            //    Fill =
            //        new SolidColorBrush(Color.FromArgb(100, 255, 255,
            //            255))
            //};

            ////OverlayWindow.Add(_rectangle);  
            #endregion
        }
    }
}