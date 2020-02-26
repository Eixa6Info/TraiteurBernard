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
        /// Place une fenêtre en haut à gauche d'une autre fenetre
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
