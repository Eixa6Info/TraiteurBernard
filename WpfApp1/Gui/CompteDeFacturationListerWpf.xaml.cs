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
    /// Logique d'interaction pour CompteDeFacturationWpf.xaml
    /// </summary>
    public partial class CompteDeFacturationListerWpf : Window
    {

        private BaseContext db;

        public TypeCompteDeFacturation CompteAssocie { get; set; }

        /// <summary>
        /// Constructeur sans paramètres, donc aucun dépendance par rapport à d'autre fenêtres
        /// de l'utilisateur
        /// </summary>
        public CompteDeFacturationListerWpf()
        {
            InitializeComponent();
            this.db = new BaseContext();
            // On cache le bouton d'association car c'est le constructeur sans paramètres donc sans dépendances
            btnAssocier.Visibility = Visibility.Hidden;
            btnAssocier.IsEnabled = false;
        }

        /// <summary>
        /// Constructeur avec paramètres donc apellé depuis une autre fenêtre, on ne cache donc pas le bouton
        /// d'association car on pourrait en avoir besoin
        /// </summary>
        /// <param name="db"></param>
        public CompteDeFacturationListerWpf(BaseContext db)
        {
            InitializeComponent();
            this.db = db;
        }

        /// <summary>
        /// Fermer la fenêtre
        /// de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Associer le compte selectionné à la personne
        /// de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Associer(object sender, RoutedEventArgs e)
        {
            this.CompteAssocie = dataGridComptes.SelectedItem as TypeCompteDeFacturation ;
        }

        /// <summary>
        /// Modifier le compte choisi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modifier(object sender, RoutedEventArgs e)
        {

            TypeCompteDeFacturation t = dataGridComptes.SelectedItem as TypeCompteDeFacturation;
            CompteDeFacturationCreerWpf wpf = new CompteDeFacturationCreerWpf(t, db);
            wpf.ShowDialog();
           
        }

        /// <summary>
        /// Créer un nouveau compte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nouveau(object sender, RoutedEventArgs e)
        {
            CompteDeFacturationCreerWpf wpf = new CompteDeFacturationCreerWpf();
            wpf.ShowDialog();
            RafraichirDataGrid();
           
        }

        /// <summary>
        /// Au chargement de la fenêtre, on charge les comptes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RafraichirDataGrid();
        }

        /// <summary>
        /// Chargement des comptes dans la datagrid
        /// de l'utilisateur
        /// </summary>
        private void RafraichirDataGrid()
        {
            IQueryable<TypeCompteDeFacturation> req = from t in this.db.ComptesDeFacturation
                      select t;

            List<TypeCompteDeFacturation> data = new List<TypeCompteDeFacturation>();

            foreach (TypeCompteDeFacturation tt in req)
            {
                data.Add(tt);

            }

            dataGridComptes.ItemsSource = data;
        }
    }
}
