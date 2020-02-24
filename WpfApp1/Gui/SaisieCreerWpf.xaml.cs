using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TraiteurBernardWPF.DAO;
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

        private string[] txts = new string[5]
        {
            "txtEntreeMidiJour",
            "txtPlat1MidiJour",
            "txtPlat2MidiJour",
            "txtPlat3MidiJour",
            "txtDessertMidiJour"

        };
        
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

            List<TraiteurBernardWPF.Modele.Menu> req = MenuDao.getAllFromWeek(this.Edite.Semaine);
            Plat[] plats = new Plat[8];

            foreach(TraiteurBernardWPF.Modele.Menu menu in req)
            {
                plats = menu.Plats.ToArray();
                for (int i = 0; i < 5; i++)
                {
                    if(plats[i] != null)
                        (this.FindName(this.txts[i] + menu.Jour) as TextBox).Text = plats[i].Name;
                }

            }

            ;
            

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
