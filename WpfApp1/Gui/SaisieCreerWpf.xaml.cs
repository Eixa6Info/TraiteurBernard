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
        // Hauteur entre les lignes
        private int tailleColonne = 5;
        // Largeur entre les colonnes
        private int tailleLigne = 176;
        private int[] IDs;
            
        private string[] itemNames = new string[8]
        {
            "EntreeMidiJour",
            "Plat1MidiJour",
            "Plat2MidiJour",
            "Plat3MidiJour",
            "DessertMidiJour",
            "EntreeSoirJour",
            "Plat1SoirJour",
            "DessertSoirJour",

        };

        private int[] types = new int[8]
        {
            SaisieData.ENTREE_MIDI,
            SaisieData.PLAT_MIDI_1,
            SaisieData.PLAT_MIDI_2,
            SaisieData.PLAT_MIDI_3,
            SaisieData.DESSERT_MIDI,
            SaisieData.ENTREE_SOIR,
            SaisieData.PLAT_SOIR_1,
            SaisieData.DESSERT_SOIR
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
                for (int ligne = this.ligneDepart; ligne < this.ligneDepart + 8; ligne++)
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
        public SaisieCreerWpf(Saisie edite, BaseContext db, int[] IDs)
        {
            InitializeComponent();
            this.db = db;
            this.Edite = edite;
            //this.pffffff = edite.Tournee;
            lblSemaine.Content = "Semaine : " + this.Edite.Semaine;
            lblAnnee.Content = "Année : " + this.Edite.Annee;
            lblPersonne.Content = "Personne : " + this.Edite.Personne;
            this.IDs = IDs;
        }

        /// <summary>
        /// Au chargement de la page, on charge les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.GenererLinterface();

            // On regarde si il y a déjà une saisie existante pour cette personne a cette semaine
            // et cette année
            IEnumerable<Saisie> saisiesDejaExistantes = from s in this.db.Saisies
                      where
                        s.Annee == this.Edite.Annee &&
                        s.Personne == this.Edite.Personne &&
                        s.Semaine == this.Edite.Semaine
                      select s;

            

            if(saisiesDejaExistantes.Any())
            {

                List<Saisie> req = new List<Saisie>();

                foreach (Saisie saisie in saisiesDejaExistantes)
                {
                    this.db.Entry(saisie).Collection(s => s.data).Load();
                    req.Add(saisie);
                }

                // On va afficher les plats dans les textboxs et les quantité dans les combobox
                // Tableau des plats qui va servir plus tard
                SaisieData[] data = new SaisieData[8];

                foreach(Saisie saisie in req)
                {
                    data = saisie.data.OrderBy(sd => sd.Type).ToArray();
                    
                    for(int i = 0; i < 8; i++)
                    {
                        if (data[i] != null)
                        {
                            (gridMain.FindName("txt" + this.itemNames[i] + saisie.Jour) as TextBox).Text = data[i].Libelle;
                            (gridMain.FindName("cb" + this.itemNames[i] + saisie.Jour) as ComboBox).SelectedItem = data[i].Quantite;
                        }
                            
                    }
                }
            }
            else
            {
                // On affiche les plats par défaut dans les checkbox
                // Liste des menus par rapport à la semaine en cours
                List<TraiteurBernardWPF.Modele.Menu> req = MenuDao.getAllFromWeek(this.Edite.Semaine);

                // Tableau des plats qui va servir plus tard
                Plat[] plats = new Plat[8];

                // Pour chaque menus, on affiche les plats dans les textbox associé
                foreach (TraiteurBernardWPF.Modele.Menu menu in req)
                {
                    plats = menu.Plats.OrderBy(p => p.Type).ToArray();
                    for (int i = 0; i < 8; i++)
                    {
                        if (plats[i] != null)
                            (gridMain.FindName("txt" + this.itemNames[i] + menu.Jour) as TextBox).Text = plats[i].Name;

                    }

                }
            }

            

            


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
            int indexSaisies = 0;

            // On recupère toutes les saisies de la semaine année et personne
            List<Saisie> req = SaisieDAO.getAllFromYearWeekPersonne(this.Edite.Annee, this.Edite.Semaine, this.Edite.Personne, this.db);

            // Conversion en array pour la manipulation par index
            Saisie[] saisies = req.ToArray();

            // Pour chaque lignes et chaque colonnes, on récupère les valeur des textbos et des comboboxes pour les
            // assigner à une saisie et les enregistrer dans la bdd
            for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
            {

                Saisie saisie;

                // Si il y a déjà une saisie qui existe pour le jour donné, on la recupèren, sinn on en créer ne autre
                if(saisies.Length - 1 >= indexSaisies && saisies[indexSaisies] != null)
                {
                   saisie = saisies[indexSaisies];
                   saisie.data = new HashSet<SaisieData>();
                }
                else
                {
                    saisie = new Saisie
                    {
                        ID = this.IDs[indexTxtNames],
                        Annee = this.Edite.Annee,
                        Jour = jour,
                        Personne = this.Edite.Personne,
                        Tournee = this.Edite.Tournee,
                        Semaine = this.Edite.Semaine,
                        data = new HashSet<SaisieData>()
                    };
                }

                // Pour toutes les lignes (les repas entree, plat1, plat2 etc)
                for (int ligne = this.ligneDepart; ligne < this.ligneDepart + 8; ligne++)
                {
                    // On recup_re le type (entree, plat1, dessert etc), le libelle (le menu) et la quantité
                    // puis on l'ajoute dans la liste des saisies
                    string txtValue = (gridMain.FindName("txt" + this.itemNames[indexTxtNames] + jour) as TextBox).Text;
                    string cbValue = (gridMain.FindName("cb" + this.itemNames[indexTxtNames] + jour) as ComboBox).SelectedItem.ToString();
                    int type = this.types[indexTxtNames++];
                    saisie.data.Add(new SaisieData { Quantite = short.Parse(cbValue), Libelle = txtValue, Type = type }); ;

                }

                if(saisie.ID == 0) this.db.Add(saisie);

                indexTxtNames = 0;
                jour++;
                indexSaisies++;
            }

            this.db.SaveChanges();
            this.db.Dispose();
            Close();

        }



    }
}
        


