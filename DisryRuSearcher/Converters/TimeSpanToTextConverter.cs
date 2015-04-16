using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DiaryRuSearcher.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                object parameter, CultureInfo culture)
        {
            var data = (TimeSpan)value;
            return data.TotalMilliseconds.ToString(culture);
        }

        public object ConvertBack(object value, Type targetType,
                                    object parameter, CultureInfo culture)
        {
            var data = value as string;
            double result;
            if(double.TryParse(data, NumberStyles.Any, culture, out result))
            {
                return TimeSpan.FromMilliseconds(double.Parse(data, culture));
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append("Только число, пожалуйста!")
                    .Append(Environment.NewLine)
                    .Append("Сейчас будут использованы значения по умолчанию!");
                MessageBox.Show(sb.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
            }   
            return TimeSpan.FromMilliseconds(2000);
        }
    }

}
