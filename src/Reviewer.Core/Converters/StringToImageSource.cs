using System;
using System.Globalization;
using Xamarin.Forms;

namespace Reviewer.Core
{
    public class StringToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string url))
                return null;

            return ImageSource.FromUri(new Uri((url)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
