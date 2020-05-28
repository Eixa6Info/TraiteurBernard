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

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PersonneListe.xaml
    /// </summary>
    public partial class PersonneListerWpf : Window
    {

        private BaseContext db;
        String recherche = "";

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
                    }
                
                    if (cbTournee.SelectedItem.ToString() == "Marennes")
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
                    }
                
                    if (cbTournee.SelectedItem.ToString() == "contre-tournée")
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
                    }

                    if (cbTournee.SelectedItem.ToString() == "Toutes les tournées")
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
            
                DataGrid gd = (DataGrid)sender;
                var row_selected = gd.SelectedItem as Personne;
                List<int> jour = new List<int>();
                List<int> numSemaine = new List<int>();
                List<int> annee = new List<int>();
                List<int> jourLivraison = new List<int>();
                int intAnnee = 0;
                int intMois = 0;
                int resMois = 0;
                int intJour = 0;
                int intJourLivraison = 0;


                IQueryable<Saisie> req = from t in db.Saisies
                                         select t;

                foreach (Saisie p in req)
                {
                    if (p.Personne.ID == row_selected.ID)
                    {
                        jour.Add(p.Jour);
                        annee.Add(p.Annee);
                        numSemaine.Add(p.Semaine);
                       // jourLivraison.Add(Int32.Parse(p.Personne.Tournee.JoursLivraisonsRepas.ToString()));

                        foreach (int aJourLivraison in jourLivraison)
                        {
                            intJourLivraison = aJourLivraison;
                            Console.WriteLine("Jour de Livraison = " + intJourLivraison);
                        }
                        foreach (int aAnnee in annee)
                        {
                            intAnnee = aAnnee;

                            foreach (int aSemaine in numSemaine)
                            {
                                intMois = aSemaine;
                                Calendar toMoisCalendar = new GregorianCalendar();

                                int searchedWeek = intMois; // N° de semaien à rechercher
                                DateTime dtFirstDayFirstWeek = DateTime.MinValue; // pour éviter erreur de compil
                                for (int iWeek = 0, iDay = 1; iWeek != 1; iDay++)
                                {
                                    dtFirstDayFirstWeek = new DateTime(intAnnee, 1, iDay);
                                    iWeek = toMoisCalendar.GetWeekOfYear(dtFirstDayFirstWeek, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                                }
                                DateTime firstDayOfGivenWeek = toMoisCalendar.AddWeeks(dtFirstDayFirstWeek, searchedWeek - 1);
                                resMois = firstDayOfGivenWeek.Month;

                                foreach (int aJour in jour)
                                {
                                    if (aJour == 1)
                                    {
                                        intJour = firstDayOfGivenWeek.Day;
                                    }
                                    if (aJour == 2)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 1;
                                    }
                                    if (aJour == 3)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 2;
                                    }
                                    if (aJour == 4)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 3;
                                    }
                                    if (aJour == 5)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 4;
                                    }
                                    if (aJour == 6)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 5;
                                    }
                                    if (aJour == 7)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 6;
                                    }
                                }
                            }
                        }
                        if (intAnnee != 0 && resMois != 0 && intJour != 0)
                        {
               
                            Console.WriteLine("annee: " + intAnnee);
                            Console.WriteLine("mois: " + resMois);
                            Console.WriteLine("jour: " + intJour);
                            Console.WriteLine("jour de livraison: " + intJourLivraison);
                            //calendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(intAnnee, resMois, intJour)));
                            calendar.SelectedDates.Add(new DateTime(intAnnee, resMois, intJour));
                            calendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(intJourLivraison)));
                            calendar.DisplayDate = new DateTime(intAnnee, resMois, intJour);
                            
                            
                        }

                    }
                }
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }
    }
}
