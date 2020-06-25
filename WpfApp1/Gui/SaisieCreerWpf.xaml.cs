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
using TraiteurBernardWPF.PDF;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieCreerWpf.xaml
    /// </summary>
    public partial class SaisieCreerWpf : Window
    {

        private BaseContext db;
        private SaisieCreerCalendrierWpf wpf1;
        private Saisie Edite { get; set; }
        private int cal = 0;
        private ImageBrush soirBackground;
        private int colonneDepart = 1;
        private int ligneDepart = 2;
        public static bool infoCal = true;
        public static Personne personneCal;
        private bool txtChange = false;
        List<TextBox> ListTxt = new List<TextBox>();
        TextBox txt;
        Label rectangle;
        Dictionary<int,string> EntreeSoir = new Dictionary<int, string>();
        Dictionary<int, string> DessertSoir = new Dictionary<int, string>();
        private int[] IDs;
        public static Personne per;
        public static int semaine;
      
        private static string txtBoxOld = "";
        private static string txtBoxChangeOld = "";
        TextBox textBoxChange;
        private static int ColOld = 0;
        private static int LigOld = 0;
        private static int cpt = 0;

        int nombreDeChampsPourMidi = 8;

        int stateOfText = 0;

        const int LIGNE_BAGUETTE = 2;
        const int LIGNE_POTAGE = 3;
        const int LIGNE_ENTREE = 4;
        const int LIGNE_PLAT1 = 5;
        const int LIGNE_PLAT2 = 6;
        const int LIGNE_PLAT3 = 7;
        const int LIGNE_FROMAGE = 8;
        const int LIGNE_DESSERT = 9;

        private string[] itemNames = new string[8]
        {
            "Baguettes",
            "Potage",
            "EntreeMidiJour",
            "Plat1MidiJour",
            "Plat2MidiJour",
            "Plat3MidiJour",
            "FromageMidi",
            "DessertMidiJour"
        };

        private int[] types = new int[8]
        {
            SaisieData.BAGUETTE,
            SaisieData.POTAGE,
            SaisieData.ENTREE_MIDI,
            SaisieData.PLAT_MIDI_1,
            SaisieData.PLAT_MIDI_2,
            SaisieData.PLAT_MIDI_3,
            SaisieData.FROMAGE,
            SaisieData.DESSERT_MIDI
        };

        class Coordonnees
        {
            internal int Ligne { get; set; }
            internal int Colonne { get; set; }
        }

        /// <summary>
        /// Classe pour map les combos box pour la valeur (ce que l'on voit) et l'ID (la valeur qui rentre en bdd)
        /// </summary>
        class ComboData
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        private List<Coordonnees> coordonneesModifiees = new List<Coordonnees>();


        /// <summary>
        /// Permet de générer tous les éléments (textboxs et comboboxs)
        /// </summary>
        private void GenererLinterface()
        {
            IQueryable<TypeTournee> req = from t in db.TypeTournee
                                          select t;

            int jour = 1;
            int indexTxtNames = 0;
            //int keyTxt = 0;
            
          
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

            int tabindex = 0;
            
            // Pour chaques colonnes et chaque lignes, on génére un textbox et une combobox par cellules
            for (int colonne = this.colonneDepart; colonne < this.colonneDepart + 7; colonne++)
            {
                if (tabindex == 50)
                {
                    tabindex = 2;
                }
                else if (tabindex == 51)
                {
                    tabindex = 3;
                }
                else if (tabindex == 52)
                {
                    tabindex = 4;
                }
                else if (tabindex == 53)
                {
                    tabindex = 5;
                }
                else if (tabindex == 54)
                {
                    tabindex = 6;
                }else if (tabindex == 55)
                {
                    tabindex = 7;
                }
                else
                {
                    tabindex = 1;
                }
                
                for (int ligne = this.ligneDepart; ligne < this.ligneDepart + nombreDeChampsPourMidi; ligne++)
                {
             
                    if (ligne != LIGNE_BAGUETTE && ligne != LIGNE_FROMAGE)
                    {
                        // Textbox
                        this.txt = new TextBox()
                        {
                            Width = 105,
                            Height = 80,
                            Margin = new Thickness(5, 5, 15, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Text = "",
                            TextWrapping = TextWrapping.Wrap,
                            IsTabStop = false
                        };
                        gridMain.RegisterName("txt" + this.itemNames[indexTxtNames] + jour, txt);
                        txt.SetValue(Grid.ColumnProperty, colonne);
                        txt.SetValue(Grid.RowProperty, ligne);

                        txt.TextChanged += Txt_TextChanged;
                        txt.Tag = new Coordonnees { Ligne = ligne, Colonne = colonne };

                        ListTxt.Add(this.txt);
                        gridMain.Children.Add(txt);

                    }

                    List<ComboData> ListData = new List<ComboData>();
                    if (ligne == LIGNE_ENTREE || ligne== LIGNE_DESSERT || ligne == LIGNE_POTAGE)
                    {
                        ListData.Add(new ComboData { Id = 0, Value = "0" });
                        ListData.Add(new ComboData { Id = 1, Value = "1" });
                        ListData.Add(new ComboData { Id = 2, Value = "2" });
                        ListData.Add(new ComboData { Id = 3, Value = "3" });
                        ListData.Add(new ComboData { Id = 4, Value = "4" });
                        ListData.Add(new ComboData { Id = 5, Value = "5" });
                        ListData.Add(new ComboData { Id = 10, Value = "Soir" });
                    }
                    else
                    {
                        ListData.Add(new ComboData { Id = 0, Value = "0" });
                        ListData.Add(new ComboData { Id = 1, Value = "1" });
                        ListData.Add(new ComboData { Id = 2, Value = "2" });
                        ListData.Add(new ComboData { Id = 3, Value = "3" });
                        ListData.Add(new ComboData { Id = 4, Value = "4" });
                        ListData.Add(new ComboData { Id = 5, Value = "5" });
                    }
       
                    //ComboBox

                    ComboBox cb = new ComboBox
                    {
                        Width = 25,
                        Height = 30,
                        Margin = new Thickness(0, 5, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                    };

                    cb.ItemsSource = ListData;
                    cb.DisplayMemberPath = "Value";
                    cb.SelectedValuePath = "Id";

                    cb.SelectionChanged += cb_ValChanged;
                    cb.Tag = new Coordonnees { Ligne = ligne, Colonne = colonne};

                    if (ligne == LIGNE_BAGUETTE || ligne == LIGNE_PLAT1 || ligne == LIGNE_PLAT2 || ligne == LIGNE_PLAT3)
                    {
                        cb.SelectedValue = 0;
                    }
                    else
                    {
                        cb.SelectedValue = 1;
                        
                    }

                    cb.TabIndex = tabindex;
                    gridMain.RegisterName("cb" + this.itemNames[indexTxtNames++] + jour, cb);
                    cb.SetValue(Grid.ColumnProperty, colonne);
                    cb.SetValue(Grid.RowProperty, ligne);
                    gridMain.Children.Add(cb);
                    tabindex = tabindex + 7;
                    
                }
                jour++;
                indexTxtNames = 0;
            }
            
        }

        private void ColorChanged(ComboBox cb, TextBox txt, string qt, Coordonnees coord)
        {

            if (txt != null)
            {
                int ligne = coord.Ligne;
                int colonne = coord.Colonne;
                if (txt.Background != Brushes.Pink)
                {
                    if (qt == "10")
                    {
                        txt.Background = Brushes.LightBlue;
                        if (ligne == LIGNE_ENTREE || ligne == LIGNE_POTAGE)
                        {
                            foreach (KeyValuePair<int, string> k in EntreeSoir)
                            {
                                if (k.Key == colonne)
                                {
                                    stateOfText = 0;
                                    txtBoxOld = txt.Text;
                                    Console.WriteLine("txtBoxOld: " + txtBoxOld);
                                    txt.Text = k.Value;
                                }
                            }
                        }
                        else if (ligne == LIGNE_DESSERT)
                        {
                            foreach (KeyValuePair<int, string> k in DessertSoir)
                            {
                                if (k.Key == colonne)
                                {
                                    stateOfText = 0;
                                    txt.Text = k.Value;
                                }
                            }
                        }
                    }
                    else if (qt == "0")
                    {
                        if (ligne == LIGNE_POTAGE)
                        {
                            stateOfText = 0;
                            txt.Background = Brushes.Transparent;
                            txt.Text = "";
                        }
                        else
                        {
                            txt.Background = Brushes.Transparent;
                        }
                    }
                    else if (qt == "txt")
                    {
                        if (stateOfText > 0)
                        {
                            txtBoxChangeOld = txt.Text;
                            textBoxChange = txt;
                            txt.Background = Brushes.Pink;
                            this.txtChange = true;
                            coordonneesModifiees.Add(coord);
                        }
                    }
                    else
                    {
                        if ((ligne == LIGNE_ENTREE || ligne == LIGNE_DESSERT) && txtBoxOld != "")
                        {
                            stateOfText = 0;
                            txt.Background = Brushes.LightGreen;
                            txt.Text = txtBoxOld;
                        }
                        else if (ligne == LIGNE_POTAGE)
                        {
                            stateOfText = 0;
                            txt.Background = Brushes.LightGreen;
                            txt.Text = "";
                        }
                        else
                        {
                            txt.Background = Brushes.LightGreen;
                        }
                    }
                }
                else
                {
                    if (qt == "0")
                    {
                        stateOfText = 0;
                        txt.Background = Brushes.Transparent;
                        txt.Text = "";
                    }
                    else if (qt == "txt")
                    {
                        if (stateOfText > 0)
                        {
                            txtBoxChangeOld = txt.Text;
                            textBoxChange = txt;
                            txt.Background = Brushes.Pink;
                            this.txtChange = true;
                            coordonneesModifiees.Add(coord);
                        }
                    }
                    else
                    {
                        if ((ligne == LIGNE_ENTREE || ligne == LIGNE_DESSERT) && txtBoxOld != "")
                        {
                            stateOfText = 0;
                            txt.Text = txtBoxOld;
                        }
                        else if (ligne == LIGNE_POTAGE)
                        {
                            stateOfText = 0;
                            txt.Text = "";
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Quand on modifie le texte d'une textBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            var coord = txt.Tag as Coordonnees;
            ColorChanged(null, txt, "txt", coord);
        }

        /// <summary>
        /// Quand un comboBox change de valeur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_ValChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            var coordCb = cb.Tag as Coordonnees;
            int col = 0;
            int lig = 0;
            TextBox textBox = new TextBox();
            var coord = Tag as Coordonnees;

            foreach (var l in ListTxt)
            {
                var coordTxt = l.Tag as Coordonnees;
                if (coordTxt.Colonne == coordCb.Colonne && coordTxt.Ligne == coordCb.Ligne)
                {
                    col = coordTxt.Colonne;
                    lig = coordTxt.Ligne;
                    textBox = l as TextBox;
                    coord = coordTxt;
                }
            }

            ColorChanged(cb, textBox, cb.SelectedValue.ToString(), coordCb);
        }
    

        /// <summary>
        /// Constructeur avec en paramètre la saisie qui contient la semaine, le jour, la tournée, l'année et la personne
        /// </summary>
        /// <param name="edite"></param>
        public SaisieCreerWpf(Saisie edite, BaseContext db, int[] IDs, ImageBrush soirBackground)
        {
            InitializeComponent();
            this.db = db;
            this.Edite = edite;
            this.soirBackground = soirBackground;
            lblSemaine.Content = this.Edite.Semaine;
            lblPersonne.Content = this.Edite.Personne;
            this.IDs = IDs;
            semaine = this.Edite.Semaine;
            per = this.Edite.Personne;
        }

        /// <summary>
        /// Au chargement de la page, on charge les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Modification demo calendrier
            this.wpf1 = new SaisieCreerCalendrierWpf(this.Edite, this.db, IDs);
            this.wpf1.Show();
            cal = 1;
            this.GenererLinterface();

            // On regarde si il y a déjà une saisie existante pour cette personne a cette semaine
            // et cette année
            IEnumerable<Saisie> saisiesDejaExistantes = from s in this.db.Saisies
                                                        where
                                                          s.Annee == this.Edite.Annee &&
                                                          s.Personne == this.Edite.Personne &&
                                                          s.Semaine == this.Edite.Semaine
                                                        select s;

            List<TraiteurBernardWPF.Modele.Menu> reqMenu = MenuDao.getAllFromWeek(this.Edite.Semaine);

            // Tableau des plats qui va servir plus tard
            Plat[] plats1 = new Plat[nombreDeChampsPourMidi];

            // Dans une saisie, on doit avoir 11 champs, y compris baguette, potages, et fromage (sur le midi)
            int valE = 1;
            int valD = 1;
            if (saisiesDejaExistantes.Any())
            {
                List<Saisie> req = new List<Saisie>();

                foreach (Saisie saisie in saisiesDejaExistantes)
                {
                    this.db.Entry(saisie).Collection(s => s.data).Load();
                    req.Add(saisie);
                }

                // On va afficher les plats dans les textboxs et les quantité dans les combobox
                // Tableau des plats qui va servir plus tard
                SaisieData[] data = new SaisieData[nombreDeChampsPourMidi];
                
                foreach (Saisie saisie in req)
                {
                    data = saisie.data.OrderBy(sd => sd.Type).ToArray();

                    foreach (TraiteurBernardWPF.Modele.Menu menu in reqMenu)
                    {
                        plats1 = menu.Plats.OrderBy(p => p != null ? p.Type : 9).ToArray();
                        EntreeSoir.Add(valE++, plats1[5].Name);
                        DessertSoir.Add(valD++, plats1[7].Name);
                    }

                    for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
                    {
                        if (data[ligne] != null)
                        {
                            
                            if (ligne != LIGNE_BAGUETTE - 2 && ligne != LIGNE_FROMAGE - 2 && ligne != LIGNE_POTAGE - 2)
                            {
                                var control = (gridMain.FindName("txt" + this.itemNames[ligne] + saisie.Jour) as TextBox);
                                control.Text = data[ligne].Libelle;
                                
                                var sd = data[ligne];
                                
                                ColorChanged(null, control, sd.Quantite.ToString(), control.Tag as Coordonnees);

                                Console.WriteLine("sdtexte " + sd.Libelle);
                                Console.WriteLine("sd.Modifier: " + sd.Modifie);

                                if (sd.Modifie)
                                {
                                    control.Background = Brushes.Pink;
                                    coordonneesModifiees.Add(control.Tag as Coordonnees);
                                    var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + saisie.Jour) as ComboBox;
                                    controlCB.SelectedValue = 1;
                                }
                                else
                                {
                                    var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + saisie.Jour) as ComboBox;
                                    controlCB.SelectedValue = data[ligne].Quantite;
                                }
                            }
                            else
                            {
                                var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + saisie.Jour) as ComboBox;
                                controlCB.SelectedValue = data[ligne].Quantite;
                            }
                        }
                        else
                        {
                            throw new Exception("je ne comprends pas comment arriver ici !!");
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
                Plat[] plats = new Plat[nombreDeChampsPourMidi];

                // Baguette et Potage ne sont pas dans les menus, mais peuvent être dans la saisie

                // Pour chaque menus, on affiche les plats dans les textbox associé
                foreach (TraiteurBernardWPF.Modele.Menu menu in req)
                {

                    plats = menu.Plats.OrderBy(p => p != null ? p.Type : 9).ToArray();
                    EntreeSoir.Add(valE++, plats[5].Name);
                    DessertSoir.Add(valD++, plats[7].Name);
                    int numeroPlatCourant = 0;

                    for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
                    {
                        // dans les menus, n'apparaissent pas les baguettes et potages, ni le fromage
                        if (ligne != LIGNE_BAGUETTE - 2 && ligne != LIGNE_FROMAGE - 2 && ligne != LIGNE_POTAGE - 2)
                        {
                            if (plats[numeroPlatCourant] != null)
                            {
                                var control = gridMain.FindName("txt" + this.itemNames[ligne] + menu.Jour) as TextBox;
                                control.Text = plats[numeroPlatCourant].Name;
                                control.IsTabStop = false;
                                numeroPlatCourant++;

                            }
                        }

                    }

                }
            }
            stateOfText = 1;
        }

        /// <summary>
        /// Valider les saisies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            Enregistrer();
            MessageBoxResult res = MessageBox.Show("Voulez-vous créer le pdf ?", "PDF", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                PdfCreerSaisieClient.PrintClient(per, semaine);
                Close();
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Bouton Enregistrer la saisie et en ouvrir une autre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnregistrerEtNouveau(object sender, RoutedEventArgs e)
        {
            Enregistrer();
            Close();
            SaisieCreerPopupWpf popupWpf = new SaisieCreerPopupWpf(this.Edite.Semaine, this.Edite.Annee);
            popupWpf.Show();
        }

        /// <summary>
        /// Bouton enregistrer
        /// </summary>
        private void Enregistrer()
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

                // Si il y a déjà une saisie qui existe pour le jour donné, on la recupère, sinn on en créer une autre
                if (saisies.Length - 1 >= indexSaisies && saisies[indexSaisies] != null)
                {
                    saisie = saisies[indexSaisies];
                    // FBR éviter ça de façon systématique, sinon on multiplie les SaisieData dans la base te on perd l'existant du soir
                    //saisie.data = new HashSet<SaisieData>();
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
                for (int ligne = this.ligneDepart; ligne < this.ligneDepart + nombreDeChampsPourMidi; ligne++)
                {
                    string txtValue = " ";
                    string cbValueVal0;
                    string cbValue;
                    int qte;
                    if (ligne != LIGNE_BAGUETTE && ligne != LIGNE_FROMAGE)
                    {
                        txtValue = (gridMain.FindName("txt" + this.itemNames[indexTxtNames] + jour) as TextBox).Text;
                    }

                   
                    cbValue = (gridMain.FindName("cb" + this.itemNames[indexTxtNames] + jour) as ComboBox).SelectedValue.ToString();
                    qte = short.Parse(cbValue);
                   
                    int type = this.types[indexTxtNames++];

                    var donnee = from d in saisie.data
                                    where d.Type == type
                                    select d;

                    if (!donnee.Any())
                    {
                        var modifie = ChercheSiTexteModifie(ligne, colonne);
                        Console.WriteLine("On enregistre que c'est modifier");
                        saisie.data.Add(new SaisieData { Quantite = qte, Libelle = txtValue, Type = type, Modifie = modifie });
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
                indexSaisies++;
            }
            
            this.db.SaveChanges();
            this.db.Dispose();       
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

        /// <summary>
        /// Bouton pour le ouvrir la saisie du soir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Soir(object sender, RoutedEventArgs e)
        {
            // enregistrer les 8 premières infos afin que les plats du soir soient en position 8,9 et 10 dans les saisies
            Enregistrer();

            var form = new SaisieCreerSoirWpf(Edite, IDs);
            form.gridMain.Background = soirBackground;
            form.ShowDialog();

            this.db = new BaseContext();
        }

        /// <summary>
        /// Annuler la saisie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Ouvrir le calendrier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calendrier(object sender, EventArgs e)
        {
         
            this.wpf1 = new SaisieCreerCalendrierWpf(this.Edite, this.db, IDs);
            if (infoCal == false) {
                wpf1.Show();
            }
        }

        /// <summary>
        /// Quand on ferme la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fermer(object sender, EventArgs e)
        {
            // fermeture du calendrier
            this.wpf1.Close();
            cal = 1;
            // fermetur de la fenetre
            Close();
        }

        private void MettreAZeroLundi(object sender, EventArgs e)
        { 
            for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
            {
                    var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + 1) as ComboBox;
                    controlCB.SelectedValue = 0;
            }
            mettreMenuSaisie(1);   
        }
        private void MettreAZeroMardi(object sender, EventArgs e)
        {
            for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
            {
                var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + 2) as ComboBox;
                controlCB.SelectedValue = 0;
            }
            mettreMenuSaisie(2);
        }
        private void MettreAZeroMercredi(object sender, EventArgs e)
        {
            for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
            {
                var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + 3) as ComboBox;
                controlCB.SelectedValue = 0;
            }
            mettreMenuSaisie(3);
        }
        private void MettreAZeroJeudi(object sender, EventArgs e)
        {
            for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
            {
                var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + 4) as ComboBox;
                controlCB.SelectedValue = 0;
            }
            mettreMenuSaisie(4);
        }
        private void MettreAZeroVendredi(object sender, EventArgs e)
        {
            for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
            {
                var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + 5) as ComboBox;
                controlCB.SelectedValue = 0;
            }
            mettreMenuSaisie(5);
        }
        private void MettreAZeroSamedi(object sender, EventArgs e)
        {
            for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
            {
                var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + 6) as ComboBox;
                controlCB.SelectedValue = 0;
            }
            mettreMenuSaisie(6);
        }
        private void MettreAZeroDimanche(object sender, EventArgs e)
        {
            for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
            {
                var controlCB = gridMain.FindName("cb" + this.itemNames[ligne] + 7) as ComboBox;
                controlCB.SelectedValue = 0;
            }
            mettreMenuSaisie(7);
        }

        private void mettreMenuSaisie(int jour)
        {
            IEnumerable<Saisie> saisiesDejaExistantes = from s in this.db.Saisies
                                                        where
                                                          s.Annee == this.Edite.Annee &&
                                                          s.Personne == this.Edite.Personne &&
                                                          s.Semaine == this.Edite.Semaine &&
                                                          s.Jour == jour
                                                          
                                                        select s;

            // On affiche les plats par défaut dans les checkbox
            // Liste des menus par rapport à la semaine en cours
            List<TraiteurBernardWPF.Modele.Menu> req = MenuDao.getAllFromDay(this.Edite.Semaine, jour);

            // Tableau des plats qui va servir plus tard
            Plat[] plats = new Plat[nombreDeChampsPourMidi];

            // Baguette et Potage ne sont pas dans les menus, mais peuvent être dans la saisie

            // Pour chaque menus, on affiche les plats dans les textbox associé
            foreach (TraiteurBernardWPF.Modele.Menu menu in req)
            {

                 plats = menu.Plats.OrderBy(p => p != null ? p.Type : 9).ToArray();
                 /*EntreeSoir.Add(valE++, plats[5].Name);
                 DessertSoir.Add(valD++, plats[7].Name);*/
                int numeroPlatCourant = 0;

                for (int ligne = 0; ligne < nombreDeChampsPourMidi; ligne++)
                {
                    // dans les menus, n'apparaissent pas les baguettes et potages, ni le fromage
                    if (ligne != LIGNE_BAGUETTE - 2 && ligne != LIGNE_FROMAGE - 2 && ligne != LIGNE_POTAGE - 2)
                    {                    
                        var control = gridMain.FindName("txt" + this.itemNames[ligne] + menu.Jour) as TextBox;
                        control.Text = plats[numeroPlatCourant].Name;
                        control.IsTabStop = false;
                        numeroPlatCourant++; 
                    }

                }

            }
            stateOfText = 1;
        }
    }
}



