﻿using System;
using System.Windows;
using System.Windows.Input;
using TraiteurBernardWPF.PDF;
using TraiteurBernardWPF.Security;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PdfCreerWpf.xaml
    /// </summary>
    public partial class PdfCreerWpf : Window
    {
        int printSaisieInt;
        bool composition;

        /// <summary>
        /// Constructeur premier paramètre : saisie ou menu
        /// deuxieem paramètre : composition ou non
        /// </summary>
        /// <param name="printSaisieInt"></param>
        /// <param name="composition"></param>
        public PdfCreerWpf(int semaine, int printSaisieInt, bool composition = false)
        {
            InitializeComponent();
            this.printSaisieInt = printSaisieInt;
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
                if (printSaisieInt == 1)
                {
                    var outputfile = CreatePDFCuisine.Start(841.8898F, 1190.55126F, short.Parse(txtSemaine.Text), DateTime.Today.Year);
                    if (!string.IsNullOrEmpty(outputfile))
                    {
                        System.Diagnostics.Process.Start(outputfile);
                    }
                }
                else if ( printSaisieInt == 2)
                {
                    var outputfile = CreatePDF5Feuilles.Start(595.27563F, 841.8898F, short.Parse(txtSemaine.Text), DateTime.Today.Year);
                    if (!string.IsNullOrEmpty(outputfile))
                    {
                        System.Diagnostics.Process.Start(outputfile);
                    }
                }
                else if (printSaisieInt == 3)
                {
                    var outputfile = CreatePDFJambon.Start(841.8898F, 595.27563F, short.Parse(txtSemaine.Text), DateTime.Today.Year);
                    if (!string.IsNullOrEmpty(outputfile))
                    {
                        System.Diagnostics.Process.Start(outputfile);
                    }
                }
                else if (printSaisieInt == 4)
                {
                    var outputfile = CreatePDF.Start(595.27563F, 841.8898F, short.Parse(txtSemaine.Text), DateTime.Today.Year, this.printSaisieInt);
                    if (!string.IsNullOrEmpty(outputfile))
                    {
                        System.Diagnostics.Process.Start(outputfile);
                    }
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
