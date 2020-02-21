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

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour CompteDeFacturationCreerWpf.xaml
    /// </summary>
    public partial class CompteDeFacturationCreerWpf : Window
    {

        private BaseContext db;

        public TypeCompteDeFacturation Edite { get; set; }

        /// <summary>
        /// Constructeur sans paramètres donc sans dépendance donc création 
        /// </summary>
        public CompteDeFacturationCreerWpf()
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new TypeCompteDeFacturation();
            edition.DataContext = this.Edite;
        }

        /// <summary>
        /// Construction avec paramètres edite et db donc avec dépendances donc modification
        /// </summary>
        /// <param name="edite"></param>
        /// <param name="db"></param>
        public CompteDeFacturationCreerWpf(TypeCompteDeFacturation edite, BaseContext db)
        {
            InitializeComponent();
            this.db = db;
            this.Edite = edite;
            edition.DataContext = this.Edite;
        }

        /// <summary>
        /// Au chargement de la fenêtre on affiche toutes les personnes de la bdd dans la
        /// liste prévue pour cela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            IQueryable<Personne> req = from t in this.db.Personnes
                      select t;

            List<Personne> data = new List<Personne>();

            foreach (Personne p in req)
            {
                this.db.Entry(p).Reference(s => s.Tournee).Load();
                data.Add(p);
            }

            lstPersonnes.ItemsSource = data;

            // Si il y a déjà des personnes dans le groupe (cas de modification), on les affiche
            // dans le label prévue pour cela
            if(this.Edite.Personnes!=null) lblListe.Content = string.Join<Personne>("\n", (from p in this.Edite.Personnes select p).ToArray());
        }

        /// <summary>
        /// Fonction de vérification des données avant création / édition
        /// </summary>
        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtNom.Text.Length != 0)
            {
                retval = true;
            }

            return retval;
        }

        /// <summary>
        /// Valider les changements / la création
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            if (VerifierDonnees())
            {
                if (this.Edite.ID == 0) this.db.Add(Edite);
                this.db.SaveChanges();
                Close();
            }
            else
            {
                MessageBox.Show("Les informations de nom, prénom et tournée sont indispensables",
                    "Informations indispensables",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Fonction qui permet de mettre à jour la liste des personnes en fonction des choix
        /// de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstPersonnes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Personne selected = lstPersonnes.SelectedItem as Personne;

            if (this.Edite.Personnes == null) this.Edite.Personnes = new List<Personne>();

            Personne found = (from p in Edite.Personnes
                        where p.ID == selected.ID
                        select p).FirstOrDefault();

            if(found != null)
            {
                this.Edite.Personnes.Remove(found);
            }
            else
            {
                this.Edite.Personnes.Add(selected);
                lblListe.Content = string.Join<Personne>("\n", (from p in this.Edite.Personnes select p).ToArray());

            }
        }
    }
}
