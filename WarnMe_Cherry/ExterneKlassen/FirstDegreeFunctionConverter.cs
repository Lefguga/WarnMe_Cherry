using System;
using System.Windows.Data;

namespace WarnMe_Cherry.ExterneKlassen
{
    /// <summary>
    /// Will return a*value + b
    /// </summary>
    public class FirstDegreeFunctionConverter : IValueConverter
    {
        public double A { get; set; }
        public double B { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double a = GetDoubleValue(parameter, A);
            double x = GetDoubleValue(value, 0.0);

            return (a * x) + B;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double a = GetDoubleValue(parameter, A);
            double y = GetDoubleValue(value, 0.0);

            return (y - B) / a;
        }

        #endregion


        private double GetDoubleValue(object parameter, double defaultValue)
        {
            double a = parameter != null ? System.Convert.ToDouble(parameter) : defaultValue;

            return a;
        }
    }
}
