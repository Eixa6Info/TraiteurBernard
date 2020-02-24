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
    /// Logique d'interaction pour SaisieCreerWpf.xaml
    /// </summary>
    public partial class SaisieCreerWpf : Window
    {

        private BaseContext db;
        private Saisie Edite { get; set; }
        
        /// <summary>
        /// Constructeur avec en paramètre la saisie qui contient la semaine, le jour, la tournée, l'année et la personne
        /// </summary>
        /// <param name="edite"></param>
        public SaisieCreerWpf(Saisie edite)
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = edite;
            lblSemaine.Content = "Semaine : " + this.Edite.Semaine;
            lblJour.Content = "Jour : " + this.Edite.Jour;
            lblAnnee.Content = "Année : " + this.Edite.Annee;
            lblPersonne.Content = "Personne : " + this.Edite.Personne;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Saisie nede = new Saisie
            {
                Annee = 2020,
                Jour = 21,
                Personne = new Personne(),
                Semaine = 7,
                Tournee = 1,
                data = new HashSet<SaisieData>{
                    new SaisieData
                {
                    Libelle="cc",
                    Quantite=1
                },
                    new SaisieData
                {
                    Libelle="aa",
                    Quantite=2
                }

                }
            };

            this.db.Add(nede);
            this.db.SaveChanges();

        }

    }
}
