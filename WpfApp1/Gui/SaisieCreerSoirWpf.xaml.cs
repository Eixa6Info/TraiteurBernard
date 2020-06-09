using org.apache.pdfbox.pdmodel.common.function.type4;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieCreerWpf.xaml
    /// </summary>
    public partial class SaisieCreerSoirWpf : Window
    {

        private BaseContext db;
        private Saisie Edite { get; set; }

        private int colonneDepart = 1;
        private int ligneDepart = 3;
      
        private int[] IDs;

        int nombreDeChampsPourSoir = 3;

        int stateOfText = 0;

        private string[] itemNames = new string[3]
        {
            
            "EntreeSoirJour",
            "Plat1SoirJour",
            "DessertSoirJour",

        };

        private int[] types = new int[3]
        {
            SaisieData.ENTREE_SOIR,
            SaisieData.PLAT_SOIR_1,
            SaisieData.DESSERT_SOIR
        };
        class Coordonnees
        {
            internal int Ligne { get; set; }
            internal int Colonne { get; set; }
        }

        private List<Coordonnees> coordonneesModifiees = new List<Coordonnees>();

        /// <summary>
        /// Permet de générer tous les éléments (textboxs et comboboxs)
        /// </summary>
        private void GenererLinterface()
        {
            int jour = 1;
            int indexTxtNames = 0;

            // pour chaque jour, afficher la date
            var laDate = FirstDateOfWeekISO8601(this.Edite.Annee, this.Edite.Semaine);
            // date de contre tournee
            var laDateContreTournee = FirstDateOfWeekContreTourneeISO8601(this.Edite.Annee, this.Edite.Semaine);

            if (Edite.Tournee.ID == 3)
            {
                for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
                {
                    var control = (gridMain.FindName("date" + colonne) as Label);
                    control.Content = laDateContreTournee.ToShortDateString();
                    laDateContreTournee = laDateContreTournee.AddDays(1);
                }
            }
            else
            {
                for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
                {
                    var control = (gridMain.FindName("date" + colonne) as Label);
                    control.Content = laDate.ToShortDateString();
                    laDate = laDate.AddDays(1);
                }
            }

            // Pour chaques colonnes et chaque lignes, on génére un textbox et une combobox par cellules
            for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
            {
                for (int ligne = this.ligneDepart; ligne < this.ligneDepart + nombreDeChampsPourSoir; ligne++)
                {
                    // Textbox
                    TextBox txt = new TextBox
                    {
                        Width = 110,
                        Height = 240,
                        Margin = new Thickness(5, 5, 15, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Text = "",
                        TextWrapping = TextWrapping.Wrap
                    };
                    gridMain.RegisterName("txt" + this.itemNames[indexTxtNames] + jour, txt);
                    txt.SetValue(Grid.ColumnProperty, colonne);
                    txt.SetValue(Grid.RowProperty, ligne);

                    txt.TextChanged += Txt_TextChanged;
                    txt.Tag = new Coordonnees { Ligne = ligne, Colonne = colonne };

                    // Combobox
                    ComboBox cb = new ComboBox
                    {
                        Width = 27,
                        Height = 30,
                        Margin = new Thickness(0, 5, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        ItemsSource = new int[5] { 0, 1, 2, 3, 4 },
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

        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (stateOfText > 0)
            {
                var txt = sender as TextBox;
                var coord = txt.Tag as Coordonnees;

                txt.Background = Brushes.Pink;

                coordonneesModifiees.Add(coord);

            }
        }
        /// <summary>
        /// Constructeur avec en paramètre la saisie qui contient la semaine, le jour, la tournée, l'année et la personne
        /// </summary>
        /// <param name="edite"></param>
        public SaisieCreerSoirWpf(Saisie edite,  int[] IDs)
        {
            if (edite == null) throw new ArgumentNullException(nameof(edite));

            InitializeComponent();
            this.db = new BaseContext();
         
            this.Edite = edite;
           
            lblSemaine.Content =  this.Edite.Semaine;
          
            lblPersonne.Content = this.Edite.Personne;
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
           /* IEnumerable<Saisie> saisiesDejaExistantes = from s in this.db.Saisies
                      where
                        s.Annee == this.Edite.Annee &&
                        s.Personne == this.Edite.Personne &&
                        s.Semaine == this.Edite.Semaine
                      select s;*/

            List<Saisie> saisiesDejaExistantes = SaisieDAO.getAllFromYearWeekPersonne(this.Edite.Annee, this.Edite.Semaine, this.Edite.Personne, this.db);

            if (saisiesDejaExistantes.Any())
            {
                // les plats du soir sont ceux qui sont en position 8,9,et 10
                int nombrePlatsAMidi = 8; 

                List<Saisie> req = new List<Saisie>();

                foreach (Saisie saisie in saisiesDejaExistantes)
                {
                    this.db.Entry(saisie).Collection(s => s.data).Load();
                    req.Add(saisie);
                }

                // On va afficher les plats dans les textboxs et les quantité dans les combobox
                // Tableau des plats qui va servir plus tard
                SaisieData[] data = new SaisieData[3];

                // Sert si pas de saisie pour le soir
                List<TraiteurBernardWPF.Modele.Menu> menus = MenuDao.getAllFromWeek(this.Edite.Semaine);
                int indiceMenu = 0;

                foreach (Saisie saisie in req)
                {
                    
                    data = saisie.data.OrderBy(sd => sd.Type).ToArray();

                    if (data.Length > nombrePlatsAMidi)
                    {
                        for (int ligne = 0; ligne < nombreDeChampsPourSoir; ligne++)
                        {

                            if (data[ligne + nombrePlatsAMidi] != null)
                            {
                                var control = gridMain.FindName("txt" + this.itemNames[ligne] + saisie.Jour) as TextBox;
                                control.Text = data[ligne + nombrePlatsAMidi].Libelle;

                                (gridMain.FindName("cb" + this.itemNames[ligne] + saisie.Jour) as ComboBox).SelectedItem = data[ligne + nombrePlatsAMidi].Quantite;

                                var sd = data[ligne + nombrePlatsAMidi];
                                if (sd.Modifie)
                                {
                                    control.Background = Brushes.Pink;
                                    coordonneesModifiees.Add(control.Tag as Coordonnees);
                                }
                            }

                        }
                        indiceMenu++;
                    }
                    else
                    {
                        if (menus.Count <= indiceMenu) continue;

                        var plats = menus[indiceMenu].Plats.OrderBy(p => p != null ? p.Type : 9).ToArray();
                        // il n'y a que 8 plats dans les menus, on ajoutera potage, baguette et fromage

                        for (int i = 0; i < nombreDeChampsPourSoir; i++)
                        {
                            (gridMain.FindName("txt" + this.itemNames[i] + saisie.Jour) as TextBox).Text = plats[i + nombrePlatsAMidi-3].Name;
                            (gridMain.FindName("cb" + this.itemNames[i] + saisie.Jour) as ComboBox).SelectedItem = 0;
                        }
                    }
                    indiceMenu++;
                }
            }
            else
            {
                AfficherLesMenusPrevus();
            }

            stateOfText = 1;
        }

        private void AfficherLesMenusPrevus()
        {
            // On affiche les plats par défaut dans les checkbox
            // Liste des menus par rapport à la semaine en cours
            List<TraiteurBernardWPF.Modele.Menu> req = MenuDao.getAllFromWeek(this.Edite.Semaine);

            // Tableau des plats qui va servir plus tard
            Plat[] plats = new Plat[8];

            // Pour chaque menus, on affiche les plats dans les textbox associé
            foreach (TraiteurBernardWPF.Modele.Menu menu in req)
            {

                plats = menu.Plats.OrderBy(p => p != null ? p.Type : 9).ToArray();
                for (int i = 0; i < nombreDeChampsPourSoir; i++)
                {
                    if (plats[i] != null)
                        (gridMain.FindName("txt" + this.itemNames[i] + menu.Jour) as TextBox).Text = plats[i].Name;

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
            int jourDansLaSemaine = 0;



            // On recupère toutes les saisies de la semaine année et personne
            List<Saisie> saisies = SaisieDAO.getAllFromYearWeekPersonne(this.Edite.Annee, this.Edite.Semaine, this.Edite.Personne, this.db);

            // Pour chaque lignes et chaque colonnes, on récupère les valeur des textbos et des comboboxes pour les
            // assigner à une saisie et les enregistrer dans la bdd
            for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
            {

                Saisie saisie;

                // Si il y a déjà une saisie qui existe pour le jour donné, on la recupèren, sinn on en créer ne autre
                if(saisies.Count - 1 >= jourDansLaSemaine && saisies[jourDansLaSemaine] != null)
                {
                   saisie = saisies[jourDansLaSemaine];
                   // FBR mars 2020 ne pas perdre les saisies du repas de midi saisie.data = new HashSet<SaisieData>();
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

                // TODO : les saisies doivent être en position 8,9 et 10 -> 

                // Pour toutes les lignes (les repas entree, plat1, plat2 etc)
                for (int ligne = this.ligneDepart; ligne < this.ligneDepart + nombreDeChampsPourSoir; ligne++)
                {
                    // On recup_re le type (entree, plat1, dessert etc), le libelle (le menu) et la quantité
                    // puis on l'ajoute dans la liste des saisies
                    string txtValue = (gridMain.FindName("txt" + this.itemNames[indexTxtNames] + jour) as TextBox).Text;
                    string cbValue = (gridMain.FindName("cb" + this.itemNames[indexTxtNames] + jour) as ComboBox).SelectedItem.ToString();
                    int type = this.types[indexTxtNames++];
                    int qte = short.Parse(cbValue);

                    // y a t il deja des infos sur les plats du soir ?

                    var donnee = from d in saisie.data
                                 where d.Type == type
                                 select d;

                    if (!donnee.Any())
                    {
                        var modifie = ChercheSiTexteModifie(ligne, colonne);
                        var sd = new SaisieData { Quantite = qte, Libelle = txtValue, Type = type, Modifie = modifie };
                       
                        saisie.data.Add(sd);

                    }
                    else
                    {
                        var data = donnee.First(); 
                        data.Quantite = qte;
                        data.Libelle = txtValue;
                       
                    }


                }

                if (saisie.ID == 0) this.db.Add(saisie);
            

                indexTxtNames = 0;
                jour++;
                jourDansLaSemaine++;
            }


            var resultat = this.db.SaveChanges();
         

            this.db.Dispose();
            Close();

        }

        private bool ChercheSiTexteModifie(int ligne, int colonne)
        {

            foreach (var coord in coordonneesModifiees)
            {
                if (ligne == coord.Ligne && colonne == coord.Colonne)
                {
                    return true;
                }
            }

            return false;
        }

        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.db.Dispose();
            Close();
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

        public static DateTime FirstDateOfWeekContreTourneeISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Friday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }
    }
}
        


