using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TraiteurBernardWPF.Utils
{
    class RenameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            bool modifie = (bool)values[0];
            int quantite = int.Parse(values[1].ToString());

            Console.WriteLine(
                ("Quantite =  " + quantite).PadRight(20) + " | " +
                ("Modifie = " + modifie).PadRight(20) + " | "
                );

            if (quantite == 0) return Brushes.Transparent;
            else if (quantite >= 10) return Brushes.LightBlue;
            else if (modifie) return Brushes.Pink;
            else return Brushes.LightGreen;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
