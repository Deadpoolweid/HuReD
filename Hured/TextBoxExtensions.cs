using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hured
{
    public static class TextboxExtensions
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.RegisterAttached(
            "Placeholder", typeof(string), typeof(TextboxExtensions), new PropertyMetadata(default(string), propertyChangedCallback: PlaceholderChanged));

        public static readonly DependencyProperty HavePlaceHolderProperty = DependencyProperty.RegisterAttached(
            "HavePlaceHolder", typeof(bool), typeof(TextboxExtensions),new PropertyMetadata(default(bool)));

        public static bool IsHavePlaceholder(this TextBox tb)
        {
            return GetHavePlaceHolder(tb);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetHavePlaceHolder(DependencyObject element, bool value)
        {
            element.SetValue(HavePlaceHolderProperty,value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetHavePlaceHolder(DependencyObject element)
        {
            return (bool) element.GetValue(HavePlaceHolderProperty);
        }

        private static void PlaceholderChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var tb = dependencyObject as TextBox;
            if (tb == null)
                return;

            tb.LostFocus -= OnLostFocus;
            tb.GotFocus -= OnGotFocus;

            if (args.NewValue != null)
            {
                tb.GotFocus += OnGotFocus;
                tb.LostFocus += OnLostFocus;

                SetHavePlaceHolder(tb, !GetHavePlaceHolder(tb));
                OnLostFocus(tb,null);

            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var tb = sender as TextBox;
            if (string.IsNullOrEmpty(tb.Text) || string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = GetPlaceholder(tb);
                SetHavePlaceHolder(tb,true);
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var tb = sender as TextBox;
            var ph = GetPlaceholder(tb);
            if (tb.Text == ph)
            {
                tb.Text = string.Empty;
                SetHavePlaceHolder(tb,false);
            }
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetPlaceholder(DependencyObject element, string value)
        {
            element.SetValue(PlaceholderProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static string GetPlaceholder(DependencyObject element)
        {
            return (string)element.GetValue(PlaceholderProperty);
        }
    }
}
