using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour MenuListerWpf.xaml
    /// </summary>
    public partial class MenuListerWpf : Window
    {

        private BaseContext db;

        /// <summary>
        /// Constructeur
        /// </summary>
        public MenuListerWpf()
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
            this.db.Dispose();
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

            // Liste d'objets anonymes qui vont etre utilisés pour faire une ligne dans la datagrid
            // forme : Jour, EntreeMidi, Plat1Midi, Plat2Midi, Plat3Midi, DessertMidi, EntreeSoir, PlatSoir, DessertSoir
            List<object> rowFormList = new List<object>();

            // Liste d'objets anonymes permettant de mettre en forme l'expander ainsi que la datagrid associée
            // Contient le Header ainsi que la liste des lignes (rowFormList)
            List<object> data = new List<object>();

            // Pour les 52 semaines de l'année
            for (int i = 1; i < 53; i++)
            {
                // On récupère tous les menus sur la semaine en itération
                List<TraiteurBernardWPF.Modele.Menu> listeMenus = MenuDao.getAllFromWeek(i);

                foreach(TraiteurBernardWPF.Modele.Menu menu in listeMenus)
                {

                    // Tableau qui représente les 8 plats hypothétique du jour
                    Plat[] tabPlats = new Plat[8];

                    // Conversion de l'hashset des plats en tableau puis copie dans le tableau précédent
                    menu.Plats.CopyTo(tabPlats);

                    // Objet anonyme qui correspond aux données qui vont être bind dans la datagrid
                    object rowForm = new
                    {
                        Jour = menu.Jour,
                        EntreeMidi = tabPlats[0] != null ? tabPlats[0].Name : "",
                        Plat1Midi = tabPlats[1] != null ? tabPlats[1].Name : "",
                        Plat2Midi = tabPlats[2] != null ? tabPlats[2].Name : "",
                        Plat3Midi = tabPlats[3] != null ? tabPlats[3].Name : "",
                        DessertMidi = tabPlats[4] != null ? tabPlats[4].Name : "",
                        EntreeSoir = tabPlats[5] != null ? tabPlats[5].Name : "",
                        PlatSoir = tabPlats[6] != null ? tabPlats[6].Name : "",
                        DessertSoir = tabPlats[7] !=null ? tabPlats[7].Name : "",
                    };

                    rowFormList.Add(rowForm);
                }

                // Finalement, pour chaque semaine on ajoute le header correspondant, la liste des menus correspondant
                // et on remet le tout à 0
                data.Add(new { Header = $"Semaine {i} | Menus {rowFormList.Count}", Plats = rowFormList });
                rowFormList = new List<object>();

            }

            dataGridSemaines.ItemsSource = data;

        }

        /// <summary>
        /// Suppression d'un menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supprimer(object sender, RoutedEventArgs e)
        {
            TraiteurBernardWPF.Modele.Menu menu = this.RecupererMenuSelectionne(sender);
            this.db.Remove(menu);
            this.db.SaveChanges();
            this.Window_Loaded(new object(), new RoutedEventArgs());
        }

        /// <summary>
        /// Recupérer le menu sur lequel un évènement est déclenché (nottament pour le clique
        /// des boutons
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private TraiteurBernardWPF.Modele.Menu RecupererMenuSelectionne(object sender)
        {
            // Forme : { Header = Semaine INT | Menus INT2, Plats = LIST<OBJECT> }
            string header = dataGridSemaines.SelectedItem.ToString();

            // Forme : { Jour = 1, EntreeMidi = STRING, Plat1Midi = STRING, 
            // Plat2Midi = STRING, Plat3Midi = STRING, DessertMidi = STRING, 
            // EntreeSoir = STRING, PlatSoir = STRING, DessertSoir = STRING }
            string dataContext = (sender as Button).DataContext.ToString();

            // Regex pour prendre le premier nombre
            Regex digits = new Regex(@"\d+");

            int semaineId = short.Parse(digits.Match(header).Value);
            int jourId = short.Parse(digits.Match(dataContext).Value);

            // On récupère le menu associé au clique du bouton
            TraiteurBernardWPF.Modele.Menu menu = MenuDao.getFirstFromWeekAndDay(semaineId, jourId, this.db);

            return menu;
        }

        /// <summary>
        /// Modification d'un menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modifier(object sender, RoutedEventArgs e)
        {

            TraiteurBernardWPF.Modele.Menu menu = this.RecupererMenuSelectionne(sender);

            MenuCreerWpf wpf = new MenuCreerWpf(menu, this.db);

            wpf.ShowDialog();


            // On recharge les données car il n'y a pas de Binding donc les données ne changent
            // pas automatiquement
            this.Window_Loaded(new Object(), new RoutedEventArgs());
        }

    }
}
