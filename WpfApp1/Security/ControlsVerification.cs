using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TraiteurBernardWPF.Security
{
    class ControlsVerification
    {
        /// <summary>
        /// Verifie que la saisie tappée dans le textboxest un chiffre (0-9)
        /// Si c'est le cas, le chiffre est écrit dans le textbox
        /// dans le cas contraire, rien n'est ecrit dans le textbox
        /// </summary>
        /// <param name="wpf"></param>
        /// <param name="parent"></param>
        internal static void DigitsOnly(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
