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

namespace WpfApp1.Gui
{
    /// <summary>
    /// Logique d'interaction pour PersonneListe.xaml
    /// </summary>
    public partial class PersonneListerWpf : Window
    {
        BaseContext db = new BaseContext();

        public PersonneListerWpf()
        {
            InitializeComponent();
            dataGridPersonnes.CanUserAddRows = false;
        }

    

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                  
            var req = from t in db.Personnes
                        select t;

            List<Personne> data = new List<Personne>();
            foreach(var p in req)
            {
                //Chargement préalable des données liées, sinon "lazy loading"
                db.Entry(p).Reference(s => s.Tournee).Load();
                db.Entry(p).Reference(s => s.CompteDeFacturation).Load();

                data.Add(p);
            }

            dataGridPersonnes.ItemsSource = data;
   
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            db.Dispose();
            Close();
        }

        /// <summary>
        /// Suppression d'une personne en cliquant sur le bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_personne_delete(object sender, RoutedEventArgs e)
        {
           
        }
        /// <summary>
        /// Modification d'une personne en cliquant sur le bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_personne_modify(object sender, RoutedEventArgs e)
        {
            var p = dataGridPersonnes.SelectedItem as Personne;

            var wpf = new PersonneCreerWpf(p,db);

            wpf.ShowDialog();
        }

        private void dataGridPersonnes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = dataGridPersonnes.SelectedItem as Personne;

            var wpf = new PersonneCreerWpf(p, db);

            wpf.ShowDialog();
        }
    }
}
