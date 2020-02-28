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

            // Suppression des saisies liés à la personne
            IEnumerable<Saisie> req = from s in this.db.Saisies where s.Personne == p select s;
            foreach(Saisie saisie in req)
            {
                this.db.Entry(saisie).Collection(s => s.data).Load();

                // On supprime les saisies data associées
                if(saisie.data != null)
                    foreach (SaisieData saisieData in saisie.data)
                        if (saisieData != null)
                            this.db.Remove(saisieData);

                this.db.Remove(saisie);
            }

            // On enlève la personne des comptes de facturations
           IEnumerable<TypeCompteDeFacturation> req2 = from t in this.db.ComptesDeFacturation select t;
            foreach(TypeCompteDeFacturation typeCompteFacturation in req2)
            {
                this.db.Entry(typeCompteFacturation).Collection(tc => tc.Personnes).Load();

                // On supprime la personne d'eventuels comptes de facturations
                if(typeCompteFacturation.Personnes != null)
                    foreach (Personne personne in typeCompteFacturation.Personnes)
                        if (personne == p) 
                            typeCompteFacturation.Personnes.Remove(p);
                    
                
               

            }

            // Enfin on enllève la personne et on sauvegarde le tout 
            // puis on reaffiche la nouvelle liste de personnes
            this.db.Remove(p);
            this.db.SaveChanges();
            this.Window_Loaded(new object(), new RoutedEventArgs());

            
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
