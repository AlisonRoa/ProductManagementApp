using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProductManagementApp.Common
{
    public class PageBgConverter : IMultiValueConverter
    {
        private static readonly Brush Normal = (Brush)new BrushConverter().ConvertFromString("#EEF2FF");
        private static readonly Brush Selected = (Brush)new BrushConverter().ConvertFromString("#C7D2FE");

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length >= 2 && values[0] != null && values[1] != null)
            {
                int cur, btn;
                if (int.TryParse(values[0].ToString(), out cur) &&
                    int.TryParse(values[1].ToString(), out btn))
                    return cur == btn ? Selected : Normal;
            }
            return Normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => new object[0];
    }
}
