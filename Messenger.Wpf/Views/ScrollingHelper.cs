﻿using System.Windows;
using System.Windows.Controls;

namespace VkMessageWpf.Views
{
    public static class ScrollingHelper
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof (bool), typeof (ScrollingHelper),
                new PropertyMetadata(false, AutoScrollPropertyChanged));

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool) obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;

            if (scrollViewer != null && (bool) e.NewValue)
            {
                scrollViewer.ScrollToBottom();
            }
        }
    }
}