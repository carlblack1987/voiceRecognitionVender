using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AutoSellGoodsMachineBusiness.AirBubble
{
    public partial class AirBubblePanel : UserControl
    {
        private static int DENSITY = 50;             
        private static double SWING_RADIUS_MIN = 10;    
        private static double SWING_RADIUS_MAX = 25;    
        private static double DOT_SIZE_MIN = 1.5;       
        private static double DOT_SIZE_MAX = 3.5;       
        private static double UP_SPEED_MIN = 1;         
        private static double UP_SPEED_MAX = 2.5;       
        private static double SWING_SPEED_MIN = 2;      
        private static double SWING_SPEED_MAX = 4;      
        private static double COLOR_OFFSET = 128;       

        private DispatcherTimer _timer;          
        private int _fps = 24;                   

        private List<AirBubbleItem> _magicDots = new List<AirBubbleItem>();

        public AirBubblePanel()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(AirBubblePanel_Loaded);
        }

        void AirBubblePanel_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / _fps);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();

            addMagicDots(true);
        }


        void _timer_Tick(object sender, EventArgs e)
        {
            moveMagicDots();
        }


        void moveMagicDots()
        {
            for (int i = _magicDots.Count - 1; i >= 0; i--)
            {
                AirBubbleItem magicDot = _magicDots[i];
                magicDot.Run();   

                if (magicDot.Y + DOT_SIZE_MAX < 0)
                {
                    LayoutRoot.Children.Remove(magicDot);
                    _magicDots.Remove(magicDot);
                }
            }
            addMagicDots(false);
        }


        public void addMagicDots(bool useRandom)
        {
            int seed = (int)DateTime.Now.Ticks;
            while (_magicDots.Count < DENSITY)
            {
                seed += (int)DateTime.Now.Ticks;
                Random r = new Random(seed);

                double size = DOT_SIZE_MIN + (DOT_SIZE_MAX - DOT_SIZE_MIN) * r.NextDouble();
                byte red = (byte)(COLOR_OFFSET + ((255 - COLOR_OFFSET) * r.NextDouble()));
                byte green = (byte)(COLOR_OFFSET + ((255 - COLOR_OFFSET) * r.NextDouble()));
                byte blue = (byte)(COLOR_OFFSET + ((255 - COLOR_OFFSET) * r.NextDouble()));

                AirBubbleItem magicDot = new AirBubbleItem(red, green, blue, size);
                magicDot.X = this.Width * r.NextDouble();
                magicDot.Y = useRandom ? this.Height * r.NextDouble() : this.Height;
                magicDot.CenterX = magicDot.X;
                magicDot.UpSpeed = UP_SPEED_MIN + (UP_SPEED_MAX - UP_SPEED_MIN) * r.NextDouble();
                magicDot.SwingRadius = SWING_RADIUS_MIN + (SWING_RADIUS_MAX - SWING_RADIUS_MIN) * r.NextDouble();
                magicDot.SwingSpeed = (int)(SWING_SPEED_MIN + (SWING_SPEED_MAX - SWING_SPEED_MIN) * r.NextDouble());
                magicDot.Counter = (int)(180 * r.NextDouble()); ;
                magicDot.Run();

                _magicDots.Add(magicDot);
                LayoutRoot.Children.Add(magicDot);
            }
        }
    }
}
