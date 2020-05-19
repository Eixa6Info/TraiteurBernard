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
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour CompteDeFacturationWpf.xaml
    /// </summary>
    public partial class CompteDeFacturationListerWpf : Window
    {

        private BaseContext db;
        String rechercheFacturation = "";

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
            try 
            {
                this.CompteAssocie = dataGridComptes.SelectedItem as TypeCompteDeFacturation;
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "CompteDeFacturationListerWpf.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Modifier le compte choisi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modifier(object sender, RoutedEventArgs e)
        {
            try
            {
                TypeCompteDeFacturation t = dataGridComptes.SelectedItem as TypeCompteDeFacturation;
                CompteDeFacturationCreerWpf wpf = new CompteDeFacturationCreerWpf(t, db);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "CompteDeFacturationListerWpf.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Créer un nouveau compte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nouveau(object sender, RoutedEventArgs e)
        { 
            try
            {
                CompteDeFacturationCreerWpf wpf = new CompteDeFacturationCreerWpf();
                wpf.ShowDialog();
                RafraichirDataGrid();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "CompteDeFacturationListerWpf.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
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

        /// <summary>
        /// Supprimer le compte de facturation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supprimer(object sender, RoutedEventArgs e)
        {

            try
            {
                MessageBoxWpf wpf = new MessageBoxWpf("Confirmation", "Vous êtes sur le point de supprimer ce compte de facturation, voulez vous continuer ?", MessageBoxButton.YesNo);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
                if (!wpf.YesOrNo) return;

                TypeCompteDeFacturation t = dataGridComptes.SelectedItem as TypeCompteDeFacturation;

                IQueryable<Personne> personnesDansLeCompte = from p in this.db.Personnes where p.CompteDeFacturation == t select p;

                // On enlève les personnes du compte de facturation
                foreach (Personne p in personnesDansLeCompte)
                {
                    p.CompteDeFacturation = null;
                }

                // On enlève le compte de facturation
                this.db.Remove(t);
                this.db.SaveChanges();

                this.Window_Loaded(new object(), new RoutedEventArgs());
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "CompteDeFacturationListerWpf.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            } 
        }

        

        private void textChangedRechercheFacturation(object sender, TextChangedEventArgs e)
        {
            try
            {
                IQueryable<TypeCompteDeFacturation> req = from t in this.db.ComptesDeFacturation
                                                          select t;

                List<TypeCompteDeFacturation> data = new List<TypeCompteDeFacturation>();
                String lastWordTt;
                String wordTtSplit = "";

                this.rechercheFacturation = this.txtRecherche.Text;

                foreach (TypeCompteDeFacturation tt in req)
                {
                    lastWordTt = tt.Nom.ToLower();

                    // Si il y a rien dans la barre de recherche on affiche tous
                    if (rechercheFacturation == "")
                    {
                        data.Add(tt);
                    }

                    // compte de le nombre de lettre dans la barre de recherche et réitère à charque nouvelle lettre
                    for (int i = 0; i < rechercheFacturation.Length; i++)
                    {
                        // On prend le nom du groupe et on met le meme nombre de lettre que la saisie dans la barre de recherche
                        wordTtSplit = lastWordTt.Substring(0, i + 1);
                        // Si la lettre du nom = a la lettre de la barre de recherche on ajoute a la liste
                        if (wordTtSplit.ToLower() == rechercheFacturation.ToLower())
                        {
                            data.Add(tt);
                        }
                    }
                }
                // on affiche
                dataGridComptes.ItemsSource = data;
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "CompteDeFacturationListerWpf.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
            
        }
    }
}
