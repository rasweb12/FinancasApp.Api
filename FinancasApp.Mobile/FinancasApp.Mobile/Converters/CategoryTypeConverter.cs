using FinancasApp.Mobile.Models.Local;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace FinancasApp.Mobile.Converters
{
    public class CategoryTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CategoryType type)
                return type == CategoryType.Income ? "Receita" : "Despesa";
            return "Despesa";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}