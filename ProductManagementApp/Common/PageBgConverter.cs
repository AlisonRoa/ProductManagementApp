using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProductManagementApp.Common
{
    public class PageBgConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return Brushes.Transparent;

            try
            {
                int current = System.Convert.ToInt32(values[0]);
                int item = System.Convert.ToInt32(values[1]);
                return current == item ? (Brush)new SolidColorBrush(Color.FromRgb(224, 234, 255))   // #E0EAFF
                    : (Brush)new SolidColorBrush(Color.FromRgb(238, 242, 255)); // #EEF2FF
            }
            catch { return Brushes.Transparent; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}