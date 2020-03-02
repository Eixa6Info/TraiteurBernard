using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TraiteurBernardWPF.PDF;
using TraiteurBernardWPF.Security;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PdfCreerWpf.xaml
    /// </summary>
    public partial class PdfCreerWpf : Window
    {
        public PdfCreerWpf()
        {
            InitializeComponent();
            txtSemaine.Text = "1";
        }

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Valider la creationde pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            CreatePDF.Start(595.27563F, 841.8898F, short.Parse(txtSemaine.Text));
            Close();
        }

        /// <summary>
        /// Verifier le format de la semaine (que des nombres)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifierFormatSemaine(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);
        }
    }
}
