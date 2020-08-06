using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class PdfFacturationMoisCreeWpf : Window
    {
        private BaseContext db = new BaseContext();
        public PdfFacturationMoisCreeWpf()
        {
            InitializeComponent();
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

            var dateFormatInfo = CultureInfo.GetCultureInfo("fr-FR").DateTimeFormat;
            cbMois.ItemsSource = dateFormatInfo.MonthNames.Take(12).ToArray();
        }

        private void Valider(object sender, RoutedEventArgs e)
        {
            if (txtAnnee.Text == "")
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensableIncorrecte, Properties.Resources.MessagePopUpInfoIndispensableIncorrecte, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
                return;
            }

            if (cbMois.SelectedItem == null)
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensable, Properties.Resources.MessagePopUpMois, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
                return;
            }

            if (cbTournee.SelectedItem as TypeTournee == null)
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensable, Properties.Resources.MessagePopUpTournee, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
                return;
                
            }
            
            
            Close();

            var outputfile = CreatePDFFacturationMSAAPA.Start(595.27563F, 841.8898F, cbTournee.SelectedItem as TypeTournee, cbMois.SelectedIndex ,short.Parse(txtAnnee.Text), checkMsa.IsChecked ?? false, checkApa.IsChecked ?? false);
            if (!string.IsNullOrEmpty(outputfile))
            {
                if(outputfile == "pas de données")
                {
                    MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpErrorDonnes, Properties.Resources.MessagePopUpDonnes, MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                    return;
                }
                System.Diagnostics.Process.Start(outputfile);
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
