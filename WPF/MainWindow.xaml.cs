using ExceptionManager;
using Logic;
using Logic.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class MainWindow : Window
    {
        //MouseHookAdapter mh;
        //bool isShowTaskbarOnExit = true;
        //readonly bool isRelaunched;
        bool isClickable = true;
        bool isDown = false;
        IntPtr hwnd;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(bool re1param):this()
        {            
            SystemManager.isRe1ParamExist = false;
#if DEBUG
            WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.ThreeDBorderWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Width = 1280;
            this.Height = 720;
#else
            WindowState = WindowState.Maximized;
#endif
            SystemManager.isRe1ParamExist = re1param;
            Ex.Log($"argumented ctor isRelaunched={SystemManager.isRe1ParamExist}");
        }

        private void Window_Initialized(object sender, EventArgs e)
        {            
            SystemManager.OnWindowLoadedAsync();
            SetEventSubscribes();
            SetBackground();
            SetHeadline();
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            try
            {
                Window window = Window.GetWindow(this);
                var wih = new WindowInteropHelper(window);
                hwnd = wih.Handle;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }
        private void SetEventSubscribes()
        {
            SystemManager.OnBeforeRelaunchApp += DisableUI;
        }
        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            //===TEST BUTTON===
            //RelaunchApp();
            await Task.Delay(5000);
            DisableUI();
            //await Task.Delay(90000);
            //SystemManager.WakeMonitor();
        }

        private void SetHeadline()
        {
            Ex.Log("MainWindow.SetHeadline()");
            try
            {
                var local = new SavingManager(Where.local);
                label1.Content = local.Key(Setting.HeadlineLauncherText).Value;
                local.Save();
            }
            catch (Exception ex)
            {
                ex.Log("Error at SetHeadline()");
            }
        }

        private void SetBackground()
        {
            Ex.Log("MainWindow.SetBackground()");
            try
            {
                if (File.Exists("background.png"))
                {
                    Ex.Log("SetBackground(): background.png exists (найден).");
                    ImageBrush myBrush = new ImageBrush();
                    var img = new BitmapImage(new Uri("background.png", UriKind.Relative));
                    try
                    {
                        var temp = img.Format;
                        myBrush.ImageSource = img;
                        this.Background = myBrush;
                    }
                    catch(Exception ex)
                    {
                        ex.Log("SetBackground()");
                    }
                }
                else
                {
                    Ex.Log("SetBackground(): background.png not found.");
                }
            }
            catch (Exception ex)
            {
                ex.Log("SetBackground()");
            }
        }

        private static void Show(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //ReloadGrid();
        }
        private void TestImageControl()
        {
            string pathImg = @"D:\Work Horizont\img\Ph03nyx-Super-Mario-Mushroom-1UP.ico";
            Image imgControl = ImageCreateGameIcon(pathImg);
            Grid.SetRow(imgControl, 4);
            Grid.SetColumn(imgControl, 1);
            //gamesGrid.Children.Add(imgControl);
        }
        private Image ImageCreateGameIcon(string incPathGame)
        {
            Ex.Log($"MainWindow.ImageCreateGameIcon() {incPathGame}");
            var imgControl = new Image();
            ImageSource imageSource = null;
            imageSource = GameManager.FindLocalImg(incPathGame);

            try
            {
                var realExePath = GameManager.GetShortcutTarget(incPathGame);
                if(realExePath != null && !File.Exists(realExePath))
                { Ex.Throw($"Ярлык {incPathGame}\nссылается на несуществующий файл:\n{realExePath}\n\nMainWindow.ImageCreateGameIcon()"); }

                imgControl.Source = imageSource ?? 
                    (realExePath == null ? Properties.Resources.game2.ToImageSource() 
                    : GameManager.GetHighResIcon(realExePath).ToImageSource() );
            }
            catch(Exception ex)
            {
                ex.Throw($"Error at {incPathGame}:\n{ex.Message}\n\nMainWindow.ImageCreateGameIcon()");
                //Show($"Ошибка с ярлыком {incPathGame};\n\n{ex.Message}\n\n MainWindow.ImageCreateGameIcon()");
            }
            imgControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            imgControl.VerticalAlignment = VerticalAlignment.Stretch;
            return imgControl;
        }
        private TextBlock LabelCreateGameName(string incPathGame)
        {
            var box = new TextBlock();
            var nameFile = new FileInfo(incPathGame).Name;
            box.Text = GameManager.CleanAfterDot(nameFile);

            box.HorizontalAlignment = HorizontalAlignment.Stretch;
            box.VerticalAlignment = VerticalAlignment.Stretch;
            box.TextAlignment = TextAlignment.Center;
            box.TextWrapping = TextWrapping.Wrap;

            //box.HorizontalContentAlignment = HorizontalAlignment.Center;
            //box.VerticalContentAlignment = VerticalAlignment.Top;
            box.FontSize = 19;            
            box.FontFamily = new FontFamily("Arial");            
            return box;
        }
        private void ReloadGrid()
        {
            Ex.Log("MainWindow.ReloadGrid()");
            ClearGrid();
            FillGrid();
        }
        private async void FillGrid()
        {
            Ex.Log("MainWindow.FillGrid()");
            string[] listGames = null;
            try
            {
                listGames = GameManager.GetAllGames();
            }
            catch (Exception ex)
            {
                ex.Log("Error at 'GameManager.GetAllGames()'");
                Show($"Error at 'GameManager.GetAllGames()'\n{ex.Message}");
            }

            int rows = 4;
            int columns = 9;
            wrapPanelGame.ItemHeight = ((1080-218) / rows) - 1;
            wrapPanelGame.ItemWidth = (1920 / columns) - 1;

            foreach (var item in listGames)
            {
                try
                {
                    wrapPanelGame.Children.Add(CreateFilledCell(item));
                }
                catch (Exception ex)
                {
                    Task.Run( () => ex.InnerGetLast().Show() );
                }                
            }

            //int gameNumber = 0;
            //for (int i = 0; i < gamesGrid.RowDefinitions.Count; i++)
            //{
            //    if (gameNumber >= listGames.Length)
            //    { break; }
            //    for (int j = 0; j < gamesGrid.ColumnDefinitions.Count; j++)
            //    {
            //        if (gameNumber >= listGames.Length)
            //        { break; }
            //        try
            //        {
            //            var cell = CreateFilledCell(listGames[gameNumber]);
            //            Grid.SetRow(cell, i);
            //            Grid.SetColumn(cell, j);                        
            //        }
            //        catch (Exception ex)
            //        {
            //            j--;
            //            Task.Run( () => ex.InnerGetLast().Show() );
            //        }
            //        gameNumber++;
            //    }
            //}
        }
        private ContentControl CreateFilledCell(string content)
        {
            if (content == null) throw new NullReferenceException("MainWindow.CreateFilledCell(arg): arg=null");
            var grid = new gridCellUser();
            var button = new Button();
            button.Content = grid;

            Image imgControl = ImageCreateGameIcon(content);
            imgControl.Initialized += async (s, e) =>
            {                
                var fileName = new Regex(@"[/\\][^/\\]*$").Match(content)?.Value;
                int seed = fileName?.Length*10 ?? 0 + content.Length;
                var rnd = new Random(seed);
                while (true)
                { await PlayIconGameAnimation(imgControl, rnd); }
            };
            Grid.SetRow(imgControl, 0);

            var lblControl = LabelCreateGameName(content);
            Grid.SetRow(lblControl, 2);

            grid.contentGrid.Children.Add(imgControl);
            grid.contentGrid.Children.Add(lblControl);

            button.Click += async (o, e) => OnClickGame(content, grid, lblControl);
            isDown = false;
            button.TouchDown += async (o, e) => 
            { Ex.Log($"TouchDown: isDown={isDown}"); isDown = true; };
            button.TouchLeave += async (o, e) => 
            { Ex.Log($"TouchLeave: isDown={isDown}"); isDown = false; };
            button.TouchUp += async (o, e) =>
            {
                Ex.Log($"TouchUp: isDown={isDown}");
                if(isDown)
                {
                    MessageBox.Show("MouseLeftButtonUp");
                    //OnClickGame(content, grid, lblControl);
                    isDown = false;
                    e.Handled = true;
                }                
            };
            //grid.contentGrid.ManipulationDelta += ContentGrid_ManipulationDelta;

            //gamesGrid.Children.Add(grid);
            button.Background = null;
            button.BorderBrush = null;
            button.Height = double.NaN;
            
            return button;
        }

        private void ContentGrid_ManipulationDelta(object sender, ManipulationDeltaEventArgs e) => throw new NotImplementedException();

        private static async Task PlayIconGameAnimation(Image imgControl, Random rnd)
        {
            var interval = rnd.Next(10, 50);
            await Task.Delay(interval * 1000);
            var top = rnd.Next(5, 30);
            var left = rnd.Next(5, 40);
            var animationTime = rnd.Next(5, 10);
            var leftRight = rnd.Next(1, 3);
            int right = leftRight == 1 ? 0 : left;
            left = right == 0 ? left : 0;
            ThicknessAnimation anim = RandomMarginAnimation(left, top, right, animationTime * 100);
            //Timeline.SetDesiredFrameRate(anim, 30); // 60 FPS
            imgControl.BeginAnimation(MarginProperty, anim);
        }

        private async Task OnClickGame(string content, gridCellUser grid, TextBlock lblControl)
        {
            if(false)WindowAPI.SetWindowExTransparent(hwnd);

            ThicknessAnimation marginAnimation = MarginAnimation();
            DoubleAnimation fontAnimation = FontAnimation();

            grid.contentGrid.IsEnabled = false;
            grid.contentGrid.BeginAnimation(MarginProperty, marginAnimation);
            lblControl.BeginAnimation(Label.FontSizeProperty, fontAnimation);
            var task = GameManager.RunGame(content);

            await Task.Delay(3000);
            grid.contentGrid.IsEnabled = true;
            //isClickable = true;
            if (false)WindowAPI.RemoveWindowExTransparent(hwnd);
        }

        private static ThicknessAnimation RandomMarginAnimation(double left, double top, double right, double length)
        {
            var animation = new ThicknessAnimation();
            animation.From = new Thickness(0);
            animation.To = new Thickness(left, top, right, 0);
            animation.Duration = TimeSpan.FromMilliseconds(length);
            animation.AutoReverse = true;
            var ease = new SineEase();
            ease.EasingMode = EasingMode.EaseInOut;
            animation.EasingFunction = ease;
            return animation;
        }
        private static ThicknessAnimation MarginAnimation()
        {
            var animation = new ThicknessAnimation();
            //animation.From = new Thickness(40);
            animation.To = new Thickness(40);
            animation.Duration = TimeSpan.FromMilliseconds(150);
            animation.AutoReverse = true;
            var ease = new CircleEase();
            //ease.Power = 3;
            ease.EasingMode = EasingMode.EaseOut;
            //animation.EasingFunction = ease;
            return animation;
        }
        private static DoubleAnimation FontAnimation()
        {
            var animation = new DoubleAnimation();
            //animation.From = 6;
            animation.To = 18;
            animation.Duration = TimeSpan.FromMilliseconds(100);
            animation.AutoReverse = true;
            var ease = new CircleEase();
            //ease.Power = 3;
            ease.EasingMode = EasingMode.EaseIn;
            animation.EasingFunction = ease;
            return animation;
        }

        private void ClearGrid()
        {
            wrapPanelGame.Children.Clear();
        }
        private void imgTest_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }        
        private void ImgTest_TouchDown(object sender, TouchEventArgs e)
        {

        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            GameManager.boolTest = true;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            GameManager.boolTest = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SystemManager.OnWindowClosed();
        }

        private void DisableUI()
        {
            Ex.Log("DisableUI(): called.");
            wrapPanelGame.IsEnabled = false;
            label1.Content = "Пожалуйста подождите / Please Wait";
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ReloadGrid();
        }
    }
}
