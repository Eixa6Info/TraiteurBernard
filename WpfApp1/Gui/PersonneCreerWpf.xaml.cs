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
using TraiteurBernardWPF.Gui;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PersonneWpf.xaml
    /// </summary>
    public partial class PersonneCreerWpf : Window
    {
        Personne edite;
        BaseContext db;

        public PersonneCreerWpf()
        {
            InitializeComponent();
            edite = new Personne();
            edition.DataContext = edite;
            db = new BaseContext();
        }

        internal PersonneCreerWpf(Personne edite, BaseContext db)
        {
            InitializeComponent();
            this.edite = edite;
            this.db = db;
            edition.DataContext = edite;
        }

        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtNom.Text.Length != 0 && txtPrenom.Text.Length != 0 && cbTournee.SelectedItem != null)
            {
                retval = true;
            }

            return retval;
        }

        private void Valider(object sender, RoutedEventArgs e)
        {
            if (VerifierDonnees())
            {
              
                if (edite.ID == 0) db.Add(edite);

                db.SaveChanges();
               
            }
            else
            {
                MessageBox.Show("Les informations de nom, prénom et tournée sont indispensables",
                    "Informations indispensables",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var req = from t in db.TypeTournee
                      select t;

            var data = new List<TypeTournee>();

            foreach (var tt in req)
            {
                db.Entry(tt).Collection(s => s.JoursLivraisonsRepas).Load();
                data.Add(tt);
            }
            cbTournee.ItemsSource = data;

  
            UpdateStatus(lblContactDurgence, edite.ContactDurgence, "Pas de contact d'urgence"); 
            UpdateStatus(lblCompte, edite.CompteDeFacturation, "Pas de compte de facturation"); 
        }

        private void CompteDeFacturationCreer(object sender, RoutedEventArgs e)
        {
            var wpf = new CompteDeFacturationListerWpf(db);
            wpf.ShowDialog();
            edite.CompteDeFacturation = wpf.CompteAssocie;
            UpdateStatus(lblCompte, edite.CompteDeFacturation, "Pas de compte de facturation");

        }

        private void UpdateStatus(Label label, Object obj, String textIfNull)
        {
            label.Content = obj != null ? obj.ToString() : textIfNull;
        }

        private void ContactDurgenceCreer(object sender, RoutedEventArgs e)
        {
            ContactDurgenceCreer wpf = new ContactDurgenceCreer(edite.ContactDurgence, db);
            wpf.ShowDialog();
            edite.ContactDurgence = wpf.Edite;
            UpdateStatus(lblContactDurgence, edite.ContactDurgence, "Pas de contact d'urgence");



    }

        private void APA_Checked(object sender, RoutedEventArgs e)
        {
            txtAPALivraisonMax.Visibility = Visibility.Visible;
            txtAPAMontantMax.Visibility = Visibility.Visible;
        }

        private void APA_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAPALivraisonMax.Text = "0";
            txtAPAMontantMax.Text = "0";
            txtAPALivraisonMax.Visibility = Visibility.Hidden;
            txtAPAMontantMax.Visibility = Visibility.Hidden;
            edite.APALivraisonMax = 0.0F;
            edite.APAMontantMax = 0.0F;
        }
    }
}
