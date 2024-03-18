using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls;

namespace SCREW_INSTALLER.Installer
{
    public static class AnimController
    {
        public static Task FadeIn(Image image)
        {
            var storyboard = new Storyboard();
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTarget(fadeInAnimation, image);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("(Opacity)"));
            storyboard.Children.Add(fadeInAnimation);

            var tcs = new TaskCompletionSource<bool>();
            EventHandler onAnimationCompleted = null;
            onAnimationCompleted = (sender, e) =>
            {
                storyboard.Completed -= onAnimationCompleted;
                tcs.SetResult(true);
            };
            storyboard.Completed += onAnimationCompleted;

            storyboard.Begin();
            return tcs.Task;
        }
        public static Task FadeOut(Image image)
        {
            var storyboard = new Storyboard();
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTarget(fadeOutAnimation, image);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("(Opacity)"));
            storyboard.Children.Add(fadeOutAnimation);

            var tcs = new TaskCompletionSource<bool>();
            EventHandler onAnimationCompleted = null;
            onAnimationCompleted = (sender, e) =>
            {
                storyboard.Completed -= onAnimationCompleted;
                tcs.SetResult(true);
            };
            storyboard.Completed += onAnimationCompleted;

            storyboard.Begin();
            return tcs.Task;
        }
    }
}
