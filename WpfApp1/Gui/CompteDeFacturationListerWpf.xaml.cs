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

        public CompteDeFacturationListerWpf()
        {
            InitializeComponent();
            this.db = new BaseContext();
        }

        public CompteDeFacturationListerWpf(BaseContext db)
        {
            InitializeComponent();
            this.db = db;
        }

        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Associer(object sender, RoutedEventArgs e)
        {
            CompteAssocie = dataGridComptes.SelectedItem as TypeCompteDeFacturation ;
           
        }

        private void Nouveau(object sender, RoutedEventArgs e)
        {
            var wpf = new CompteDeFacturationCreerWpf();
            wpf.ShowDialog();
            RafraichirDataGrid();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RafraichirDataGrid();
        }

        private void RafraichirDataGrid()
        {
            var req = from t in db.ComptesDeFacturation
                      select t;

            var data = new List<TypeCompteDeFacturation>();

            foreach (var tt in req)
            {
                data.Add(tt);
            }

            dataGridComptes.ItemsSource = data;
        }
    }
}
