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

namespace WpfApp1.Gui
{
    /// <summary>
    /// Logique d'interaction pour PersonneWpf.xaml
    /// </summary>
    public partial class PersonneWpf : Window
    {
        Personne edite;
        BaseContext db = new BaseContext();

        public PersonneWpf()
        {
            InitializeComponent();
            edite = new Personne();
            edition.DataContext = edite;
            db = new BaseContext();
        }

        internal PersonneWpf(Personne edite, BaseContext db)
        {
            InitializeComponent();
            this.edite = edite;
            this.db = db;
            edition.DataContext = edite;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool VerifieDonneesIndispensables()
        {
            bool retval = false;

            if (txtNom.Text.Length != 0 && txtPrenom.Text.Length != 0 && cbTournee.SelectedItem != null)
            {
                retval = true;
            }

            return retval;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (VerifieDonneesIndispensables())
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

            if (edite.CompteDeFacturation != null)
                lblCompte.Content = edite.CompteDeFacturation.ToString();
            else
                lblCompte.Content = "Pas de compte";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var wpf = new CompteDeFacturationWpf(db);
            wpf.ShowDialog();
            edite.CompteDeFacturation = wpf.CompteAssocie;
            if (edite.CompteDeFacturation != null)  lblCompte.Content = edite.CompteDeFacturation.ToString();
        }
    }
}
