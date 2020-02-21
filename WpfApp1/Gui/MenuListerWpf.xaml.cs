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
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;

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
            // Utilisation du namespace car la classe Menu existe déjà nativement en C#
            IQueryable<TraiteurBernardWPF.Modele.Menu> req = from t in this.db.Menu
                                          select t;

            List<List<TraiteurBernardWPF.Modele.Menu>> data = new List<List<TraiteurBernardWPF.Modele.Menu>>();

            for (int i = 1; i < 10; i++)
            {
                data.Add(MenuDao.getAllFromWeek(i));
            }

            dataGridTournees.ItemsSource = data;

        }

        private void DataGridTemplateColumn_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
