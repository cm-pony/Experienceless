using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
namespace Experienceless
{


    namespace Converters
    {

        public class CalcSizeConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                float mbsize = 0;
                
                if (values[0] != null && values[1] != null)
                {
                    Regex r = new Regex(@"^\d+$");
                    if (r.IsMatch(values[0].ToString()) && r.IsMatch(values[1].ToString())) {
                        mbsize = System.Convert.ToInt32(values[0]) * System.Convert.ToInt32(values[1]) * 60f / 8f / 1000f;
                    }
                }
                return string.Format("File size: {0:F} GB", mbsize);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
        [ValueConversion(typeof(bool), typeof(bool))]
        public class InverseBooleanConverter : IValueConverter
        {
  

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (targetType != typeof(bool))
                    throw new InvalidOperationException("The target must be a boolean");

                return !(bool)value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }

 
        }

    }
}