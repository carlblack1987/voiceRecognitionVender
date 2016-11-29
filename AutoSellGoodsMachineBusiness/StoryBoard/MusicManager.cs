
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.Media;
using System.Windows.Media;

namespace AutoSellGoodsMachineBusiness.StoryBoard
{
    public class MusicManager
    {
        private DispatcherTimer playTime;

        private Grid layout;

        private int timeTotal = 0, j = 90;//110

        private int span = 0;

        private List<MediaElement> mediaCollection = new List<MediaElement>();

        public MusicManager(Grid grid)
        {
            this.layout = grid;
            playTime = new DispatcherTimer();
            playTime.Interval = new TimeSpan(0, 0, 0, 0, j);
            playTime.Tick += (_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (timeTotal > span)
            {
                 CreatePlayMedia();
            }
            else
            {
                timeTotal += j;
            }
        }

        public void CreatePlayMedia()
        {
            MediaElement element = new MediaElement()
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual,
                Source = new Uri("pack://siteoforigin:,,,/Sounds/BgMusic.mp3")
            };
            element.Position = new TimeSpan(0, 0, 0, 0, 0);
            element.SpeedRatio = 1;
            element.MediaEnded += (EndPlayMedia);
            layout.Children.Add(element);
            mediaCollection.Add(element);
            element.Play();
        }

        void EndPlayMedia(object sender, RoutedEventArgs e)
        {
            layout.Children.Remove(sender as MediaElement);
            mediaCollection.Remove(sender as MediaElement);
        }

        public void Start(int span)
        {
            this.span = span;
            CreatePlayMedia();
            playTime.Start();
        }

        public void Stop()
        {
            playTime.Stop();
            Clear();
        }

        private void Clear()
        {
            foreach (MediaElement element in mediaCollection)
            {
                element.Stop();
                this.layout.Children.Remove(element);
            }
            mediaCollection.Clear();
        }

        public void PlayMusic(MediaElement el)
        {
            el.Position = new TimeSpan(0, 0, 0, 0, 0);
            el.Play();
        }
    }
}
