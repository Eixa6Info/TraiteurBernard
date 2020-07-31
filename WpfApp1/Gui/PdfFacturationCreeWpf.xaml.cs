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
    /// Logique d'interaction pour PdfFacturationCreeWpf.xaml
    /// </summary>
    public partial class PdfFacturationCreeWpf : Window
    {
        private BaseContext db = new BaseContext();
        public PdfFacturationCreeWpf()
        {
            InitializeComponent();
            txtSemaine.Text = GestionDeDateCalendrier.TrouverLeNumSemaineAvecMois(DateTime.Now).ToString();
            txtAnnee.Text = DateTime.Now.Year.ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IQueryable<TypeTournee> req = from t in this.db.TypeTournee
                                          select t;

            List<TypeTournee> data = new List<TypeTournee>();

            foreach (TypeTournee tt in req)
            {
                this.db.Entry(tt).Collection(s => s.JoursLivraisonsRepas).Load();
                data.Add(tt);
            }
            cbTournee.ItemsSource = data;
        }

        private void Valider(object sender, RoutedEventArgs e)
        {
            if (short.Parse(txtSemaine.Text) > 52)
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpSemaine, Properties.Resources.MessagePopUpErrorSemaineSup52, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
                // MessagePopUpErrorSemaineSup52
            }
            else
            {
                var outputfile = CreatePDFFacturation.Start(595.27563F, 841.8898F, short.Parse(txtSemaine.Text), short.Parse(txtAnnee.Text), cbTournee.SelectedItem as TypeTournee);
                if (!string.IsNullOrEmpty(outputfile))
                {
                    System.Diagnostics.Process.Start(outputfile);
                }
            }
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

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
