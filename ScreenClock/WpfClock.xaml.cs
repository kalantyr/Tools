using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ScreenClock
{
    /// <summary>
    /// Interaction logic for WpfClock.xaml
    /// </summary>
    public partial class WpfClock
    {
        public WpfClock()
        {
            InitializeComponent();

            CreateDivisions();

            var fromSecond = 360d * (DateTime.Now.Second / 60d);
            var secondArrowAnimation = new DoubleAnimation(fromSecond, fromSecond + 360, TimeSpan.FromMinutes(1)) { RepeatBehavior = RepeatBehavior.Forever };
            SecondArrowRotate.BeginAnimation(RotateTransform.AngleProperty, secondArrowAnimation);

            var fromMinute = 360d * ((DateTime.Now.Minute + DateTime.Now.Second / 60d) / 60d);
            var minuteArrowAnimation = new DoubleAnimation(fromMinute, fromMinute + 360, TimeSpan.FromHours(1)) { RepeatBehavior = RepeatBehavior.Forever };
            MinuteArrowRotate.BeginAnimation(RotateTransform.AngleProperty, minuteArrowAnimation);

            var fromHour = 360d * ((DateTime.Now.Hour + DateTime.Now.Minute / 60d + DateTime.Now.Second / 3600d ) / 12d);
            var hourArrowAnimation = new DoubleAnimation(fromHour, fromHour + 360, TimeSpan.FromHours(12)) { RepeatBehavior = RepeatBehavior.Forever };
            HourArrowRotate.BeginAnimation(RotateTransform.AngleProperty, hourArrowAnimation);
        }

        private void CreateDivisions()
        {
            for (var i = 0; i < 60; i++)
            {
                if (i % 5 == 0)
                    continue;

                var canvas = new Canvas
                                 {
                                     Width = 0,
                                     Height = 0,
                                     RenderTransform = new RotateTransform { Angle = 360 * (i / 60d) }
                                 };
                canvas.Children.Add(new SmallDivision());
                Divisions.Children.Add(canvas);
            }

            for (var i = 0; i < 12; i++)
            {
                var canvas = new Canvas
                                 {
                                     Width = 0,
                                     Height = 0,
                                     RenderTransform = new RotateTransform { Angle = 360 * (i / 12d) }
                                 };
                canvas.Children.Add(new Division());
                Divisions.Children.Add(canvas);
            }
        }
    }
}
