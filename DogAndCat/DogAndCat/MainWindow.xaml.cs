using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DogAndCat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();

        bool goLeft, goRight, goDown, goUp;
        bool noLeft, noRight, noDown, noUp;

        int speed = 6;

        Rect catHitBox;

        int dogSpeed = 8;
        int dogMoveStep = 130;
        int currentDogStep;
        int score = 0;

        public MainWindow()
        {
            InitializeComponent();

            GameSetUp(); 
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left && noLeft == false)
            {
                goRight = goUp = goDown = false;
                noRight = noUp = noDown = false;

                goLeft = true;
                cat.RenderTransform=new RotateTransform(-90,cat.Width /2,cat.Height /2);
            }

            if (e.Key == Key.Right && noRight == false)
            {
                goLeft = goUp = goDown = false;
                noLeft = noUp = noDown = false;

                goRight = true;
                cat.RenderTransform = new RotateTransform(90, cat.Width / 2, cat.Height / 2);
            }

            if (e.Key == Key.Up && noUp == false)
            {
                goLeft = goRight = goDown = false;
                noLeft = noRight = noDown = false;

                goUp = true;
                cat.RenderTransform = new RotateTransform(0, cat.Width / 2, cat.Height / 2);
            }

            if (e.Key == Key.Down && noDown == false)
            {
                goLeft = goUp = goRight = false;
                noLeft = noUp = noRight = false;

                goDown = true;
                cat.RenderTransform = new RotateTransform(-180, cat.Width / 2, cat.Height / 2);
            }


        }
        
        private void GameSetUp()
        {
            MyCanvas.Focus();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentDogStep = dogMoveStep;

            ImageBrush catImage = new ImageBrush();
            catImage.ImageSource = new BitmapImage(new Uri("D://DogAndCat/img/happy.png"));
            cat.Fill = catImage;

            ImageBrush angryDog = new ImageBrush();
            angryDog.ImageSource = new BitmapImage(new Uri("D://DogAndCat/img/angry.png"));
            angrydog.Fill = angryDog;

            ImageBrush coolDog = new ImageBrush();
            coolDog.ImageSource = new BitmapImage(new Uri("D://DogAndCat/img/cool.png"));
            cooldog.Fill = coolDog;

            ImageBrush madDog = new ImageBrush();
            madDog.ImageSource = new BitmapImage(new Uri("D://DogAndCat/img/mad.png"));
            maddog.Fill = madDog;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score:" + score;

            if(goRight)
            {
                Canvas.SetLeft(cat, Canvas.GetLeft(cat) + speed);
            }

            if (goLeft)
            {
                Canvas.SetLeft(cat, Canvas.GetLeft(cat) - speed);
            }

            if (goUp)
            {
                Canvas.SetTop(cat, Canvas.GetTop(cat) - speed);
            }

            if (goDown)
            {
                Canvas.SetTop(cat, Canvas.GetTop(cat) + speed);
            }

            if(goDown && Canvas.GetTop(cat) +80 > Application.Current.MainWindow.Height)
            {
                noDown = true;
                goDown=false;
            }

            if(goUp && Canvas.GetTop(cat) < 1)
            {
                goUp=false;
                noUp=true;
            }

            if (goLeft && Canvas.GetLeft(cat) -10 < 1)
            {
                goLeft = false;
                noLeft = true;
            }

            if (goRight && Canvas.GetLeft(cat) +70 > Application.Current.MainWindow.Width)
            {
                noRight = true;
                goRight = false;
            }

            catHitBox = new Rect(Canvas.GetLeft(cat), Canvas.GetTop(cat), cat.Width, cat.Height); 


            foreach(var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if((string)x.Tag == "wall")
                {
                    if(goLeft == true && catHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(cat, Canvas.GetLeft(cat) + 10);
                        noLeft = true;
                        goLeft = false;
                    }

                    if (goRight == true && catHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(cat, Canvas.GetLeft(cat) - 10);
                        noRight = true;
                        goRight = false;
                    }

                    if (goDown == true && catHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(cat, Canvas.GetTop(cat) - 10);
                        noDown = true;
                        goDown = false;
                    }

                    if (goUp == true && catHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(cat, Canvas.GetTop(cat) + 10);
                        noUp = true;
                        goUp = false;
                    }
                }

                if ((string) x.Tag == "coin")
                {
                    if(catHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                        // 108 coins
                    }
                }

                if((string) x.Tag == "dog")
                {
                    if(catHitBox.IntersectsWith(hitBox))
                    {
                        GameOver("Dogs caught you, click ok to try one more time");
                    }

                    if (x.Name.ToString() == "angrydog")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - dogSpeed);
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + dogSpeed);
                    }

                    currentDogStep--;

                    if(currentDogStep < 1)
                    {
                        currentDogStep = dogMoveStep;
                        dogSpeed = -dogSpeed;
                    }

                }
            }

            if(score==108)
            {
                GameOver("You Win!");
            }

        }

        private void GameOver(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "Dogs And Cat");
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
