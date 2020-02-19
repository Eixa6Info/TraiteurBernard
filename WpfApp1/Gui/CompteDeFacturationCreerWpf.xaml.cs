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
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour CompteDeFacturationCreerWpf.xaml
    /// </summary>
    public partial class CompteDeFacturationCreerWpf : Window
    {
        BaseContext db;
      

        public TypeCompteDeFacturation Edite { get; set; }

        public CompteDeFacturationCreerWpf()
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new TypeCompteDeFacturation();
        }

        public CompteDeFacturationCreerWpf(TypeCompteDeFacturation edite, BaseContext db)
        {
            InitializeComponent();
            this.db = db;
            this.Edite = edite;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var req = from t in db.Personnes
                      select t;

            List<Personne> data = new List<Personne>();

            foreach (var p in req)
            {
                db.Entry(p).Reference(s => s.Tournee).Load();
                data.Add(p);
            }

            lstPersonnes.ItemsSource = data;

            if(Edite.Personnes!=null) lblListe.Content = string.Join<Personne>("\n", (from p in Edite.Personnes select p).ToArray());
        }

        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtNom.Text.Length != 0)
            {
                retval = true;
            }

            return retval;
        }
        private void Fermer(object sender, RoutedEventArgs e)
        {

            if (VerifierDonnees())
            {
                Edite.Nom = txtNom.Text;

                if (Edite.ID == 0) db.Add(Edite);

                db.SaveChanges();

                Close();
            }
            else
            {
                MessageBox.Show("Les informations de nom, prénom et tournée sont indispensables",
                    "Informations indispensables",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void lstPersonnes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lstPersonnes.SelectedItem as Personne;

            if (Edite.Personnes == null) Edite.Personnes = new List<Personne>();

            var found = (from p in Edite.Personnes
                        where p.ID == selected.ID
                        select p).FirstOrDefault();

            if(found != null)
            {
                Edite.Personnes.Remove(found);
            }
            else
            {
                Edite.Personnes.Add(selected);
                lblListe.Content = string.Join<Personne>("\n", (from p in Edite.Personnes select p).ToArray());

            }
        }
    }
}
