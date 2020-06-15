using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
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
        bool printSaisieBool;
        bool composition;

        /// <summary>
        /// Constructeur premier paramètre : saisie ou menu
        /// deuxieem paramètre : composition ou non
        /// </summary>
        /// <param name="printSaisieBool"></param>
        /// <param name="composition"></param>
        public PdfCreerWpf(int semaine, bool printSaisieBool, bool composition = false)
        {
            InitializeComponent();
            this.printSaisieBool = printSaisieBool;
            txtSemaine.Text = semaine.ToString();
            this.composition = composition;
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
            if (!this.composition)
            {
                var outputfile = CreatePDF.Start(595.27563F, 841.8898F, short.Parse(txtSemaine.Text), DateTime.Today.Year, this.printSaisieBool);
                if (!string.IsNullOrEmpty(outputfile))
                {
                    System.Diagnostics.Process.Start(outputfile);
                }
            }
            else
            {
                var outputfile = CreatePDF.StartComposition(595.27563F, 841.8898F, short.Parse(txtSemaine.Text), DateTime.Today.Year);
                if (!string.IsNullOrEmpty(outputfile)) System.Diagnostics.Process.Start(outputfile);
            }
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
