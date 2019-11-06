using Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private ICommand someCommand;
        private Uri uriSoundClick = new Uri("/zapsplat_button_click_004.mp3", UriKind.Relative);
        private MediaElement soundClick = new MediaElement();
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            soundClick.UnloadedBehavior = MediaState.Manual;
            soundClick.LoadedBehavior = MediaState.Manual;
            soundClick.Source = uriSoundClick;
            soundClick.Position = new TimeSpan(0,0,0);
#if DEBUG
            WindowState = WindowState.Normal;
#else
            WindowState = WindowState.Maximized;
#endif
            SystemManager.isRe1ParamExist = false;
        }
        public MainWindow(bool re1param):this()
        {
            SystemManager.isRe1ParamExist = re1param;
            Ex.Log($"argumented ctor isRelaunched={SystemManager.isRe1ParamExist}");
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            ReloadGrid();
            SystemManager.OnWindowLoaded();
            SetEventSubscribes();
            SetBackground();
            SetHeadline();            
        }
        public ICommand SomeCommand
        {
            get
            {
                return someCommand
                    ?? (someCommand = new ActionCommand(() =>
                    {
                        var form = new InfoWindow();
                        form.Show();
                    }));
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
            Ex.Log("SetHeadline() called.");
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
            Ex.Log("SetBackground(): called");
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
            gamesGrid.Children.Add(imgControl);
        }
        private Image ImageCreateGameIcon(string incPathGame)
        {
            var imgControl = new Image();
            imgControl.Tag = incPathGame;
            ImageSource imageSource = null;
            imageSource = GameManager.FindLocalImg(incPathGame);

            try
            {
                var realExePath = GameManager.GetShortcutTarget(incPathGame);
                if(realExePath!=null && !File.Exists(realExePath))
                {
                    Ex.Throw($"Ярлык {incPathGame} ссылается на несуществующий файл {realExePath};\nMainWindow.ImageCreateGameIcon()");
                }
                imgControl.Source = imageSource ?? 
                    (realExePath == null ? Properties.Resources.game2.ToImageSource() 
                    : GameManager.GetHighResIcon(realExePath).ToImageSource() );
            }
            catch(CustException excust)
            { excust.Throw(); }
            catch(Exception ex)
            {
                ex.Throw($"Error incPathGame={incPathGame}\n{ex.Message}\nMainWindow.ImageCreateGameIcon()");
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
            box.FontSize = 20;            
            box.FontFamily = new FontFamily("Arial");            
            return box;
        }
        private void ReloadGrid()
        {
            Ex.Log("ReloadGrid(): called.");
            ClearGrid();
            FillGrid();
        }
        private void CreateFilledCell(int i, int j, string content)
        {
            var grid = new gridCellUser();
            try
            {
                Image imgControl = ImageCreateGameIcon(content);
                
                imgControl.Initialized += async (s, e) =>
                {
                    Trace.WriteLine($"{imgControl.Tag} Loaded;");
                    var rnd = new Random(i * 100 + j);
                    while (true)
                    { await SetAnimation(imgControl, rnd); Trace.WriteLine($"anim played {imgControl.Tag}"); }
                };
                Grid.SetRow(imgControl, 0);

                var lblControl = LabelCreateGameName(content);
                Grid.SetRow(lblControl, 2);

                grid.contentGrid.Children.Add(imgControl);
                grid.contentGrid.Children.Add(lblControl);

                ThicknessAnimation marginAnimation = MarginAnimation();
                DoubleAnimation fontAnimation = FontAnimation();

                grid.contentGrid.MouseDown += async (o, e) =>
                {
                    if (isClickable)
                    {
                        isClickable = false;
                        //soundClick.Position = new TimeSpan(0, 0, 0);
                        //soundClick.Play();
                        var task = GameManager.RunGame(content);
                        grid.contentGrid.BeginAnimation(MarginProperty, marginAnimation);
                        lblControl.BeginAnimation(Label.FontSizeProperty, fontAnimation);
                        try
                        {
                            await task;
                            await Task.Delay(1000);
                            isClickable = true;
                        }
                        catch (Exception ex)
                        {
                            Show(ex.Message);
                            isClickable = true;
                        }
                    }
                };
                grid.contentGrid.TouchDown += async (o, e) =>
                {
                    if (isClickable)
                    {
                        isClickable = false;
                        //soundClick.Position = new TimeSpan(0, 0, 0);
                        //soundClick.Play();
                        var task = GameManager.RunGame(content);
                        grid.contentGrid.BeginAnimation(MarginProperty, marginAnimation);
                        lblControl.BeginAnimation(Label.FontSizeProperty, fontAnimation);
                        try
                        {
                            await task;
                            await Task.Delay(1000);
                            isClickable = true;
                        }
                        catch (Exception ex)
                        {
                            Show(ex.Message);
                            isClickable = true;
                        }
                    }
                };

                Grid.SetRow(grid, i);
                Grid.SetColumn(grid, j);
                gamesGrid.Children.Add(grid);
            }
            catch(CustException ex1)
            { ex1.Throw(); }
            catch (Exception ex)
            {
                ex.Throw("MainWindow.CreateFilledCell()");
            }
        }

        private static async Task SetAnimation(Image imgControl, Random rnd)
        {
            var top = rnd.Next(5, 30);
            var left = rnd.Next(5, 40);
            var animationTime = rnd.Next(5, 12);
            var leftRight = rnd.Next(1, 3);
            int right = leftRight == 1 ? 0 : left;
            left = right == 0 ? left : 0;
            ThicknessAnimation anim = RandomMarginAnimation(left, top, right, animationTime * 100);
            //Timeline.SetDesiredFrameRate(anim, 30); // 60 FPS
            int interval = rnd.Next(7, 45);
            await Task.Delay(interval * 1000);            
            imgControl.BeginAnimation(MarginProperty, anim, HandoffBehavior.Compose);
            Trace.WriteLine($"{imgControl.Tag} {interval}");
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
        private void ControlsCircle(int i, int j, string content)
        {
            var ControlsList = new List<FrameworkElement>();
            ControlsList.Add(ImageCreateGameIcon(content));
            ControlsList.Add(LabelCreateGameName(content));

            foreach (var item in ControlsList)
            {
                Grid.SetRow(item, i);
                Grid.SetColumn(item, j);
                gamesGrid.Children.Add(item);
            }
        }
        private void FillGrid()
        {
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
            int numGame = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (numGame >= listGames.Length)
                    { break; }                    
                    try
                    {
                        CreateFilledCell(i, j, listGames[numGame++]);
                    }
                    catch(CustException ex1)
                    { ex1.Log(); j--; }
                    catch (Exception ex)
                    {
                        ex.Log(); j--;
                    }
                }
            }
        }
        private void ClearGrid()
        {
            gamesGrid.Children.Clear();
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
            gamesGrid.IsEnabled = false;
            label1.Content = "Пожалуйста подождите / Please Wait";
        }
    }
}
