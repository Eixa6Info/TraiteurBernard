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
    /// Logique d'interaction pour TourneesLister.xaml
    /// </summary>
    public partial class TourneesLister : Window
    {

        BaseContext db;

        /// <summary>
        /// Constructeur
        /// </summary>
        public TourneesLister()
        {
            InitializeComponent();
            this.db = new BaseContext();
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
        /// Au chargement de la fenêtres, on charge les personnes et leur objets de référence puis
        /// on les affiche dans la datagrig
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            IQueryable<TypeTournee> req = from t in this.db.TypeTournee
                      select t;

            List<TypeTournee> data = new List<TypeTournee>();

            foreach (var p in req)
            {
                //Chargement préalable des données liées, sinon "lazy loading"
                // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                // voir pour plus de détails 
                db.Entry(p).Collection(s => s.JoursLivraisonsRepas).Load();
                data.Add(p);
            }

            dataGridTournees.ItemsSource = data;
            
        }

    }
}
