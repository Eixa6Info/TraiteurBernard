using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour PersonneListe.xaml
    /// </summary>
    public partial class PersonneListerWpf : Window
    {

        private BaseContext db;

        /// <summary>
        /// On bloque la possibilité à l'utilisateur d'ajouter des lignes (note : on peut
        /// aussi mettre cette propriété directement dans le xaml
        /// </summary>
        public PersonneListerWpf()
        {
            InitializeComponent();
            dataGridPersonnes.CanUserAddRows = false;
            this.db = new BaseContext();
        }

        /// <summary>
        /// Au chargement de la fenêtres, on charge les personnes et leur objets de référence puis
        /// on les affiche dans la datagrig
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                  
            IQueryable<Personne> req = from t in db.Personnes
                        select t;

            List<Personne> data = new List<Personne>();

            foreach(Personne p in req)
            {
                //Chargement préalable des données liées, sinon "lazy loading"
                // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                // voir pour plus de détails 
                this.db.Entry(p).Reference(s => s.Tournee).Load();
                this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                this.db.Entry(p).Reference(s => s.ContactDurgence).Load();

                data.Add(p);
            }

            dataGridPersonnes.ItemsSource = data;
        }

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            this.db.Dispose();
            Close();
        }

        /// <summary>
        /// Suppression d'une personne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supprimer(object sender, RoutedEventArgs e)
        {
            Personne p = dataGridPersonnes.SelectedItem as Personne;
            
            this.db.Remove(p);

            
        }

        /// <summary>
        /// Modification d'une personne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modifier(object sender, RoutedEventArgs e)
        {
            Personne p = dataGridPersonnes.SelectedItem as Personne;

            PersonneCreerWpf wpf = new PersonneCreerWpf(p, this.db);

            wpf.ShowDialog();
        }


    }
}
