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
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.PDF;
using TraiteurBernardWPF.Security;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour FacturationClientPopup.xaml
    /// </summary>
    public partial class FacturationClientPopup : Window
    {

        BaseContext db;


        public FacturationClientPopup()
        {
            InitializeComponent();
            this.db = new BaseContext();

        }

        /// <summary>
        /// Bouton valider, ferme la fenetre et ouvre la fenetre de creation principal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            TypeTournee typeTournee = (TypeTournee)this.cbTournee.SelectedItem;
            if(!String.IsNullOrEmpty(txtSemaineStart.Text)  &&  !String.IsNullOrEmpty(txtSemaineEnd.Text))
            {
                int semaineStart = int.Parse(this.txtSemaineStart.Text);
                int semaineEnd = int.Parse(this.txtSemaineEnd.Text);
                // message d'erreur ici
                if (semaineEnd <= semaineStart)
                {
                    MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensableIncorrecte, Properties.Resources.MessagePopUpS2SuperieurS1, MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                    return;
                }

                if (typeTournee == null)
                {
                    MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensableIncorrecte, Properties.Resources.MessagePopUpTournee, MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                    return;
                }

                var outputfile = CreatePDFFacturation.Start(595.27563F, 841.8898F, typeTournee, semaineStart, semaineEnd);
                if (!string.IsNullOrEmpty(outputfile))
                {
                    System.Diagnostics.Process.Start(outputfile);
                }
                Close();
            }
            else
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensableIncorrecte, Properties.Resources.MessagePopUpSemaines, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
                return;
            }
            

           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Chargement des personnes et assignation à la combobox
            List<TypeTournee> typeTournees = (from t in this.db.TypeTournee
                                           select t).ToList();



            cbTournee.ItemsSource = typeTournees;
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
        /// Verifier que le format entré est bien une semaine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifierFormatSemaine(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);
        }

    }
}
