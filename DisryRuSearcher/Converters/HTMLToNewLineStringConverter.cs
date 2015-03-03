using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DiaryRuSearcher.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class HTMLToNewLineStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            var s = value as string;
            return s.Replace("<br>", Environment.NewLine).Replace("<br/>", Environment.NewLine);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return string.Empty;
        }
    }
}
