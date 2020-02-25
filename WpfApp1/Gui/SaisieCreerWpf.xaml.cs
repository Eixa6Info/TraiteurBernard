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

        private int colonneDepart = 1;
        private int ligneDepart = 2;
        private int tailleColonne = 103;
        private int tailleLigne = 176;


        private string[] itemNames = new string[5]
        {
            "EntreeMidiJour",
            "Plat1MidiJour",
            "Plat2MidiJour",
            "Plat3MidiJour",
            "DessertMidiJour"

        };

        /// <summary>
        /// Permet de générer tous les éléments (textboxs et comboboxs)
        /// </summary>
        private void GenererLinterface()
        {
            int jour = 1;
            int indexTxtNames = 0;

            // Pour chaques colonnes et chaque lignes, on génére un textbox et une combobox par cellules
            for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
            {
                for (int ligne = this.ligneDepart; ligne < this.ligneDepart + 5; ligne++)
                {
                    // Textbox
                    TextBox txt = new TextBox
                    {
                        Width = 90,
                        Height = 40,
                        Margin = new Thickness(10, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = ""

                    };
                    gridMain.RegisterName("txt" + this.itemNames[indexTxtNames] + jour, txt);
                    txt.SetValue(Grid.ColumnProperty, colonne);
                    txt.SetValue(Grid.RowProperty, ligne);


                    // Combobox
                    ComboBox cb = new ComboBox
                    {
                        Width = 57,
                        Height = 30,
                        Margin = new Thickness(0, 42, 10, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        ItemsSource = new int[3] { 0, 1, 2 },
                        SelectedItem = 1
                    };
                    gridMain.RegisterName("cb" + this.itemNames[indexTxtNames++] + jour, cb);
                    cb.SetValue(Grid.ColumnProperty, colonne);
                    cb.SetValue(Grid.RowProperty, ligne);

                    // Ajout des éléments
                    gridMain.Children.Add(txt);
                    gridMain.Children.Add(cb);


                }
                jour++;
                indexTxtNames = 0;
            }


        }

        /// <summary>
        /// Constructeur avec en paramètre la saisie qui contient la semaine, le jour, la tournée, l'année et la personne
        /// </summary>
        /// <param name="edite"></param>
        public SaisieCreerWpf(Saisie edite, BaseContext db)
        {
            InitializeComponent();
            this.db = db;
            this.Edite = edite;
            //this.pffffff = edite.Tournee;
            lblSemaine.Content = "Semaine : " + this.Edite.Semaine;
            lblAnnee.Content = "Année : " + this.Edite.Annee;
            lblPersonne.Content = "Personne : " + this.Edite.Personne;
        }

        /// <summary>
        /// Au chargement de la page, on charge les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.GenererLinterface();

            // Liste des menus par rapport à la semaine en cours
            List<TraiteurBernardWPF.Modele.Menu> req = MenuDao.getAllFromWeek(this.Edite.Semaine);

            // Tableau des plats qui va servir plus tard
            Plat[] plats = new Plat[8];

            // Pour chaque menus, on affiche les plats dans les textbox associé
            foreach (TraiteurBernardWPF.Modele.Menu menu in req)
            {
                plats = menu.Plats.ToArray();
                for (int i = 0; i < 5; i++)
                {
                    if (plats[i] != null)
                        (gridMain.FindName("txt" + this.itemNames[i] + menu.Jour) as TextBox).Text = plats[i].Name;

                }

            }

            ;


        }

        /// <summary>
        /// Valider les saisies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            int jour = 1;
            int indexTxtNames = 0;

            // Pour chaque lignes et chaque colonnes, on récupère les valeur des textbos et des comboboxes pour les
            // assigner à une saisie et les enregistrer dans la bdd
            for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
               {
                   Saisie saisie = new Saisie
                   {
                       Annee = this.Edite.Annee,
                       Jour = this.Edite.Jour,
                       Personne = this.Edite.Personne,
                       Tournee = this.Edite.Tournee,
                       Semaine = this.Edite.Semaine,
                       data = new HashSet<SaisieData>()
                   };

                   for (int ligne = this.ligneDepart; ligne < this.ligneDepart + 5; ligne++)
                   {
                       string txtValue = (gridMain.FindName("txt" + this.itemNames[indexTxtNames] + jour) as TextBox).Text;
                       string cbValue = (gridMain.FindName("cb" + this.itemNames[indexTxtNames++] + jour) as ComboBox).SelectedItem.ToString();
                       saisie.data.Add(new SaisieData { Quantite = short.Parse(cbValue), Libelle = txtValue }); ;

                   }

                   this.db.Add(saisie);
                       
                   indexTxtNames = 0;
                   jour++;
               }
          
            this.db.SaveChanges();
            Close();

        }



    }
}
        


