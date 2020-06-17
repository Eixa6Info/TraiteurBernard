using com.ibm.icu.util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Utils;
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Globalization;
using GregorianCalendar = System.Globalization.GregorianCalendar;
using Calendar = System.Globalization.Calendar;
using System.IO;
using java.sql;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Properties;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PersonneListe.xaml
    /// </summary>
    public partial class PersonneListerWpf : Window
    {

        private BaseContext db;
        String recherche = "";
        public static int jourDeSaisie;
        public static string jourDeLivraison;
        CalenderBackground background;
        public static Personne row_selected;
        private Saisie Edite { get; set; }

        /// <summary>
        /// On bloque la possibilité à l'utilisateur d'ajouter des lignes (note : on peut
        /// aussi mettre cette propriété directement dans le xaml
        /// </summary>
        public PersonneListerWpf()
        {
            InitializeComponent();
            dataGridPersonnes.CanUserAddRows = false;
            this.db = new BaseContext();
        }

        /// <summary>
        /// Au chargement de la fenêtres, on charge les personnes et leur objets de référence puis
        /// on les affiche dans la datagrig
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                  
            IQueryable<Personne> req = from t in db.Personnes
                        select t;

            List<Personne> data = new List<Personne>();

            foreach(Personne p in req)
            {
                //Chargement préalable des données liées, sinon "lazy loading"
                // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                // voir pour plus de détails 
                this.db.Entry(p).Reference(s => s.Tournee).Load();
                this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                this.db.Entry(p).Reference(s => s.ContactDurgence).Load();

                if (p.Actif == true)
                {
                    data.Add(p);
                    data.Sort((x, y) => string.Compare(x.Nom, y.Nom));
                }
            }

            dataGridPersonnes.ItemsSource = data;

            IQueryable<TypeTournee> reqT = from t in this.db.TypeTournee
                                          select t;

            List<string> allDataT = new List<string>();
            List<string> dataCbActif = new List<string>();

            foreach (TypeTournee tt in reqT)
            {
                this.db.Entry(tt).Collection(s => s.JoursLivraisonsRepas).Load();
                allDataT.Add(tt.Nom.ToString());
            }
            allDataT.Add("Toutes les tournées");
            cbTournee.ItemsSource = allDataT;

            dataCbActif.Add("Actif");
            dataCbActif.Add("Inactif");
            dataCbActif.Add("Actif & Inactif");
            cbActif.ItemsSource = dataCbActif;
            
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
        /// Suppression d'une personne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supprimer(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxWpf wpf = new MessageBoxWpf("Confirmation", "Vous êtes sur le point de supprimer cette personne, voulez vous continuer ?", MessageBoxButton.YesNo);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
                if (!wpf.YesOrNo) return;

                Personne p = dataGridPersonnes.SelectedItem as Personne;

                // Suppression des saisies liés à la personne
                IEnumerable<Saisie> req = from s in this.db.Saisies where s.Personne == p select s;
                foreach(Saisie saisie in req)
                {
                    this.db.Entry(saisie).Collection(s => s.data).Load();

                    // On supprime les saisies data associées
                    if(saisie.data != null)
                        foreach (SaisieData saisieData in saisie.data)
                            if (saisieData != null)
                                this.db.Remove(saisieData);

                    this.db.Remove(saisie);
                }

                // On enlève la personne des comptes de facturations
               IEnumerable<TypeCompteDeFacturation> req2 = from t in this.db.ComptesDeFacturation select t;
                foreach(TypeCompteDeFacturation typeCompteFacturation in req2)
                {
                    this.db.Entry(typeCompteFacturation).Collection(tc => tc.Personnes).Load();

                    // On supprime la personne d'eventuels comptes de facturations
                    if (typeCompteFacturation.Personnes != null)
                        if (typeCompteFacturation.Personnes.Contains(p)) typeCompteFacturation.Personnes.Remove(p);
                   
                
               

                }

                // Enfin on enllève la personne et on sauvegarde le tout 
                // puis on reaffiche la nouvelle liste de personnes
                this.db.Remove(p);
                this.db.SaveChanges();
                this.Window_Loaded(new object(), new RoutedEventArgs());
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
            

            
        }

        /// <summary>
        /// Modification d'une personne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modifier(object sender, RoutedEventArgs e)
        {
            
            try
            {
                Personne p = dataGridPersonnes.SelectedItem as Personne;

                PersonneCreerWpf wpf = new PersonneCreerWpf(p, this.db);

                wpf.ShowDialog();
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }

        private void textChangedRechercheClient(object sender, TextChangedEventArgs e)
        {
            try
            {
                String lastNameWordP;
                String wordPNameSplit = "";
                this.recherche = txtRecherche.Text;
                IQueryable<Personne> req = from t in db.Personnes
                                           select t;

                List<Personne> data = new List<Personne>();
                foreach (Personne p in req)
                {
                    //Chargement préalable des données liées, sinon "lazy loading"
                    // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                    // voir pour plus de détails 
                    this.db.Entry(p).Reference(s => s.Tournee).Load();
                    this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                    this.db.Entry(p).Reference(s => s.ContactDurgence).Load();
               
                    lastNameWordP = p.Nom.ToLower();

                    // Si il y a rien dans la barre de recherche on affiche tous
                    if (recherche == "")
                    {
                        data.Add(p);
                    }

                    // compte de le nombre de lettre dans la barre de recherche et réitère à charque nouvelle lettre
                    for (int i = 0; i < recherche.Length; i++)
                    {
                        if (recherche.Length <= lastNameWordP.Length)
                        {
                           wordPNameSplit = lastNameWordP.Substring(0, i + 1);
                        }
                    
                        // Si la lettre du nom = a la lettre de la barre de recherche on ajoute a la liste
                        if (wordPNameSplit.ToLower() == recherche.ToLower())
                        {
                            data.Add(p);
                        }
                    } 
                }
                dataGridPersonnes.ItemsSource = data;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                IQueryable<Personne> req = from t in db.Personnes
                                           select t;

                List<Personne> data = new List<Personne>();

                foreach (Personne p in req)
                {
                    //Chargement préalable des données liées, sinon "lazy loading"
                    // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                    // voir pour plus de détails 
                    this.db.Entry(p).Reference(s => s.Tournee).Load();
                    this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                    this.db.Entry(p).Reference(s => s.ContactDurgence).Load();


                    data.Add(p);

                }
                cbTournee.Text = "Toutes les tournées";
                cbActif.Text = "Actif & Inactif";
                cbTournee.SelectedIndex = 5;
                cbActif.SelectedIndex = 3;
                dataGridPersonnes.ItemsSource = data;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }

        private void cbActif_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            try
            {
                IQueryable<Personne> req = from t in db.Personnes
                                           select t;

                List<Personne> data = new List<Personne>();

                foreach (Personne p in req)
                {
                    //Chargement préalable des données liées, sinon "lazy loading"
                    // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                    // voir pour plus de détails 
                    this.db.Entry(p).Reference(s => s.Tournee).Load();
                    this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                    this.db.Entry(p).Reference(s => s.ContactDurgence).Load();

                    if (cbActif.SelectedItem.ToString() == Properties.Resources.Actif)
                    {
                        if (p.Actif == true)
                        {
                            data.Add(p);
                        }
                    }
                    else if (cbActif.SelectedItem.ToString() == Properties.Resources.Inactif)
                    {
                        if (p.Actif == false)
                        {
                            data.Add(p);
                        }
                    }
                    else
                    {
                        data.Add(p);
                    }
                }
                dataGridPersonnes.ItemsSource = data;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }

        private void cbTournee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                IQueryable<Personne> req = from t in db.Personnes
                                           select t;

                List<Personne> data = new List<Personne>();

                foreach (Personne p in req)
                {
                    //Chargement préalable des données liées, sinon "lazy loading"
                    // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                    // voir pour plus de détails 
                    this.db.Entry(p).Reference(s => s.Tournee).Load();
                    this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                    this.db.Entry(p).Reference(s => s.ContactDurgence).Load();               

                    if (cbTournee.SelectedItem.ToString() == "ville 1")
                    {
                        if (p.Tournee.Nom == "ville 1")
                        {
                            if (cbActif.SelectedItem.ToString() == "Actif")
                            {
                                if (p.Actif == true)
                                {
                                    data.Add(p);
                                }
                            }
                            else if (cbActif.SelectedItem.ToString() == "Inactif")
                            {
                                if (p.Actif == false)
                                {
                                    data.Add(p);
                                }
                            }
                            else
                            {
                                data.Add(p);
                            }
                        }
                    }

                    if (cbTournee.SelectedItem.ToString() == "ville 2")
                    {
                        if (p.Tournee.Nom == "ville 2")
                        {
                            if (cbActif.SelectedItem.ToString() == "Actif")
                            {
                                if (p.Actif == true)
                                {
                                    data.Add(p);
                                }
                            }
                            else if (cbActif.SelectedItem.ToString() == "Inactif")
                            {
                                if (p.Actif == false)
                                {
                                    data.Add(p);
                                }
                            }
                            else
                            {
                                data.Add(p);
                            }
                        }
                    }
                    else if (cbTournee.SelectedItem.ToString() == "Marennes")
                    {
                        if (p.Tournee.Nom == "Marennes")
                        {
                            if (cbActif.SelectedItem.ToString() == "Actif")
                            {
                                if (p.Actif == true)
                                {
                                    data.Add(p);
                                }
                            }
                            else if (cbActif.SelectedItem.ToString() == "Inactif")
                            {
                                if (p.Actif == false)
                                {
                                    data.Add(p);
                                }
                            }
                            else
                            {
                                data.Add(p);
                            }
                        }
                    }
                    else if (cbTournee.SelectedItem.ToString() == "contre-tournée")
                    {
                        if (p.Tournee.Nom == "contre-tournée")
                        {
                            if (cbActif.SelectedItem.ToString() == "Actif")
                            {
                                if (p.Actif == true)
                                {
                                    data.Add(p);
                                }
                            }
                            else if (cbActif.SelectedItem.ToString() == "Inactif")
                            {
                                if (p.Actif == false)
                                {
                                    data.Add(p);
                                }
                            }
                            else
                            {
                                data.Add(p);
                            }
                        }
                    }
                    else if (cbTournee.SelectedItem.ToString() == "Toutes les tournées")
                    {
                        if (cbActif.SelectedItem.ToString() == "Actif")
                        {
                            if (p.Actif == true)
                            {
                                data.Add(p);
                            }
                        }
                        if (cbActif.SelectedItem.ToString() == "Inactif")
                        {
                            if (p.Actif == false)
                            {
                                data.Add(p);
                            }
                        }
                        if (cbActif.SelectedItem.ToString() == "Actif & Inactif")
                        {
                            data.Add(p);
                        }
                    }
                    else
                    {
                        data.Add(p);
                    }
                    
                }
                dataGridPersonnes.ItemsSource = data;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            } 
        }


        private void dataGridPersonnes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                background = new CalenderBackground(calendar);
                background.AddOverlay("circle", Properties.Resources.imgCircle);
                calendar.SelectedDates.Clear();
                DataGrid gd = (DataGrid)sender;
                row_selected = gd.SelectedItem as Personne;
                
                List<string> jourLivraison = new List<string>();
                List<string> jourRepas1 = new List<string>();
                List<string> jourRepas2 = new List<string>();
                List<string> jourRepas3 = new List<string>();
                string resJour = "";
                int resMois;
                IQueryable<Saisie> req = from t in db.Saisies
                                         where
                                         t.Personne.ID == row_selected.ID
                                         select t;

                IQueryable<Livraison> reqLiv = from t in db.Livraisons
                                               select t;

                foreach (Livraison t in reqLiv)
                {
                    jourLivraison.Add(t.JourLivraison);
                    jourRepas1.Add(t.JourRepas1);
                    jourRepas2.Add(t.JourRepas2);
                    jourRepas3.Add(t.JourRepas3);
                } 
                if (req.Count() != 0)
                {
                    foreach (Saisie p in req)
                    {
                        List<int> reqJourDeSaisie = SaisieDAO.SaisiePourUneJournee(db, row_selected, p.Annee, p.Semaine, p.Jour);
                        // on calcule la sommes dans la liste
                        if (reqJourDeSaisie.Sum() > 0)
                        {

                            // Afficher sur le calendrier les jours de saisie.
                            resJour = GestionDeDateCalendrier.LeJourSuivantLeNuméro(p.Jour);
                            DateTime JourDeSaisie = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(p.Annee, p.Semaine, resJour);
                            resMois = GestionDeDateCalendrier.TrouverLeMoisAvecNumSemaine(p.Semaine, p.Annee);

                            calendar.SelectedDates.Add(JourDeSaisie);
                            //calendar.SelectedDates.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                            calendar.DisplayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                            // Afficher sur le calendrier les jours de livraison par rapport au saisie
                            DateTime leJourDeLivraison = LivraisonDAO.JourDeLivraisonCal(p.Tournee.Nom, p.Annee, p.Semaine, JourDeSaisie);

                            int j = leJourDeLivraison.Day;
                            int m = leJourDeLivraison.Month;
                            int y = leJourDeLivraison.Year;

                            background.AddDate(new DateTime(y, m, j), "circle");

                            calendar.Background = background.GetBackground();

                            calendar.DisplayDateChanged += CalenderOnDisplayDateChanged;
                        }
                        else
                        {
                            Console.WriteLine("il y a pas de jour de saisie");
                        }
                    }
                }
                else
                {
                    background.ClearDates();
                }
                
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }

        private void CalenderOnDisplayDateChanged(object sender, CalendarDateChangedEventArgs calendarDateChangedEventArgs)
        {
            calendar.Background = background.GetBackground();
        }

        private void cal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
       /*     System.Windows.Controls.Primitives.CalendarDayButton button = sender as System.Windows.Controls.Primitives.CalendarDayButton;
            DateTime clickedDate = (DateTime)button.DataContext;
            if (calendar.SelectedDates.Contains(clickedDate))
            {
                var Date = sender;
                string date = Date.ToString().Substring(0, 10);
                DateTime dateTime = Convert.ToDateTime(date);
                var semaine = CultureInfo.CurrentUICulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                var annee = dateTime.Year;
                Console.WriteLine("row_selected.ID : " + row_selected.ID);

                this.Edite = new Saisie { Semaine = semaine, Annee = annee };
                this.Edite.Personne = row_selected;
                this.Edite.Semaine = semaine;
                this.Edite.Annee = annee;
                this.Edite.Tournee = row_selected.Tournee;

                IQueryable<Saisie> req = from t in db.Saisies
                                         where
                                         t.Personne.ID == row_selected.ID &&
                                         t.Semaine == semaine &&
                                         t.Annee == annee

                                         select t;

                foreach (Saisie s in req)
                {
                    this.Edite = s;
                }

                // suivant la tournée, ouvrir une saisir ou une autre
                if (this.Edite.Tournee.Nom == "ville 1" || this.Edite.Tournee.Nom == "ville 2")
                {
                    Close();
                    int[] ID = SaisieDAO.getIdsFromYearWeekPersonne(annee, semaine, this.Edite.Personne, this.db);
                    var soirBackground = new ImageBrush(new BitmapImage(new Uri("/eixa6/TourneeSoirVille.png", UriKind.RelativeOrAbsolute)));
                    SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, ID, soirBackground);
                    wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri("/eixa6/TourneeMidiVille.png", UriKind.RelativeOrAbsolute)));
                    WinFormWpf.CornerTopLeftToParent(wpf, this);
                    wpf.ShowDialog();

                }
                else if (this.Edite.Tournee.Nom == "contre-tournée")
                {
                    Close();
                    int[] ID = SaisieDAO.getIdsFromYearWeekPersonne(annee, semaine, this.Edite.Personne, this.db);
                    var soirBackground = new ImageBrush(new BitmapImage(new Uri("/eixa6/TourneeSoirContre.png", UriKind.RelativeOrAbsolute)));
                    SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, ID, soirBackground);
                    wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri("/eixa6/TourneeMidiContre.png", UriKind.RelativeOrAbsolute)));
                    WinFormWpf.CornerTopLeftToParent(wpf, this);
                    wpf.ShowDialog();
                }
                else if (this.Edite.Tournee.Nom == "Marennes")
                {
                    Close();
                    int[] ID = SaisieDAO.getIdsFromYearWeekPersonne(annee, semaine, this.Edite.Personne, this.db);
                    var soirBackground = new ImageBrush(new BitmapImage(new Uri("/eixa6/TourneeSoirMarennes.png", UriKind.RelativeOrAbsolute)));
                    SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, ID, soirBackground);
                    wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri("/eixa6/TourneeMidiMarennes.png", UriKind.RelativeOrAbsolute)));
                    WinFormWpf.CornerTopLeftToParent(wpf, this);
                    wpf.ShowDialog();
                }
                else
                {
                    var wpf = new MessageBoxWpf("Tournée manquante", $"La saisie pour cette tournée {this.Edite.Tournee.Nom} n'est pas disponible", MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                }
            }*/
        }
    }
}
