using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AutoSellGoodsMachineBusiness.AirBubble
{
    public class AirBubbleItem : Canvas
    {
        private static int ELLIPSE_COUNT = 3;
        private static double OPACITY = 0.6;
        private static double OPACITY_INC = -0.15;

        public double SwingRadius;
        public int Counter = 0;
        public int SwingSpeed = 5;
        public double UpSpeed = 1;
        public double CenterX;

        public AirBubbleItem(byte red, byte green, byte blue, double size)
        {
            double opac = OPACITY;

            for (int i = 0; i < ELLIPSE_COUNT; i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = size;
                ellipse.Height = size;
                //if (i == 0)
                //{
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                //}
                //else
                //{
                //ellipse.Fill = new SolidColorBrush(Color.FromArgb(red, green, blue, 255));
                ellipse.Opacity = opac;
                opac += OPACITY_INC;
                size += size;
                //}
                ellipse.SetValue(Canvas.LeftProperty, -ellipse.Width / 2);
                ellipse.SetValue(Canvas.TopProperty, -ellipse.Height / 2);
                this.Children.Add(ellipse);
            }
        }

        public void Run()
        {
            double angle = Counter / 180.0 * Math.PI;
            Y = Y - UpSpeed;
            X = CenterX + Math.Cos(angle) * SwingRadius;
            Counter += SwingSpeed;
        }

        public double X
        {
            get
            {
                return (double)(GetValue(Canvas.LeftProperty));
            }
            set
            {
                SetValue(Canvas.LeftProperty, value);
            }
        }

        public double Y
        {
            get
            {
                return (double)(GetValue(Canvas.TopProperty));
            }
            set
            {
                SetValue(Canvas.TopProperty, value);
            }
        }
    }
}
