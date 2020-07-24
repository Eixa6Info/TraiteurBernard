using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using GregorianCalendar = System.Globalization.GregorianCalendar;
using Calendar = System.Globalization.Calendar;
using System.IO;
using TraiteurBernardWPF.Utils;
using TraiteurBernardWPF.DAO;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieCreerCalendrierWpf.xaml
    /// </summary>
    public partial class SaisieCreerCalendrierWpf : Window
    {
        private BaseContext db;
        private Saisie Edite { get; set; }
        private int[] IDs;
        CalenderBackground background = new CalenderBackground();
        private int semaineDisplay;
        private int anneeDisplay;
        private Personne personne;
        public SaisieCreerCalendrierWpf(Saisie edite, BaseContext db, int[] IDs)
        {
            if (edite == null) throw new ArgumentNullException(nameof(edite));

            InitializeComponent(); 
            //this.db = new BaseContext();
            this.db = db;
            this.Edite = edite;
            this.IDs = IDs;
            this.semaineDisplay = edite.Semaine;
            this.anneeDisplay = edite.Annee;
            this.personne = Edite.Personne;
        }

        private void calendar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            //Close();
            SaisieCreerWpf.infoCal = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                // https://stackoverflow.com/questions/53371091/displaying-dates-as-holiday-in-wpf-calendar
                // Voir ce lien pour la couleur background avec un converter

                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/d264d00b-b948-4862-908d-bc90bb6d9424/set-bold-dates-in-calendar?forum=wpf
                // https://www.codeproject.com/Articles/104081/Extending-the-WPF-Calendar-Control
                // Pour le bold

                calendar.IsTodayHighlighted = false;
                background = new CalenderBackground(calendar);

       

                var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                background.AddOverlay("trait", new Uri(Path.Combine(outPutDirectory, "Assets\\trait.png")).LocalPath);
                background.AddOverlay("circle", new Uri(Path.Combine(outPutDirectory, "Assets\\circle.png")).LocalPath);


                calendar.SelectedDates.Clear();
                background.ClearDates();
               
                string resJour = "";
                int resMois;

                IQueryable<Saisie> req = from t in db.Saisies
                                         where
                                         t.Personne.ID == personne.ID
                                         select t;                

                IQueryable<Livraison> reqLiv = from t in db.Livraisons
                                               select t;

                int newMois = 0;
                int oldMois = 0;
                foreach (Saisie p in req)
                {
                        
                    List<int> reqJourDeSaisie = SaisieDAO.SaisiePourUneJournee(db, personne, p.Annee, p.Semaine, p.Jour);
                   
                    // on calcule la sommes dans la liste
                    if (reqJourDeSaisie.Sum() > 0)
                    {

                        // Afficher sur le calendrier les jours de saisie.
                        resJour = GestionDeDateCalendrier.LeJourSuivantLeNuméro(p.Jour);
                        DateTime JourDeSaisie = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(p.Annee, p.Semaine, resJour);
                        resMois = GestionDeDateCalendrier.TrouverLeMoisAvecNumSemaine(semaineDisplay, anneeDisplay);
                        DateTime JourDAffichage = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(anneeDisplay, semaineDisplay, "lundi");
                        //calendar.SelectedDates.Add(JourDeSaisie);
                        oldMois = JourDeSaisie.Month;
                        if (newMois == 0)
                        {
                            newMois = oldMois;
                        }
                        if ((resJour == "Lundi" && JourDAffichage.Day == 1) || newMois == oldMois)
                        {
                            background.AddDate(JourDeSaisie, "trait", true);
                            Console.WriteLine("je suis a true");
                        }
                        else
                        {
                            background.AddDate(JourDeSaisie, "trait", false);
                            if (newMois != oldMois)
                            {
                                newMois = oldMois;
                            }
                            
                            Console.WriteLine("Je suis a false");
                        }
                        
                        Console.WriteLine("la date -> " + JourDeSaisie.ToString());
                        calendar.DisplayDate = new DateTime(JourDAffichage.Year, JourDAffichage.Month, JourDAffichage.Day);

                        DateTime leJourDeLivraison;

                        if (p.Tournee == null || p.Tournee.Nom == "")
                        {
                            throw new Exception("Tournée null impossible");
                           // leJourDeLivraison = LivraisonDAO.JourDeLivraisonCal(personne.Tournee.Nom, p.Annee, p.Semaine, JourDeSaisie);
                        }
                        else
                        {
                            // Afficher sur le calendrier les jours de livraison par rapport au saisie
                            leJourDeLivraison = LivraisonDAO.JourDeLivraisonCal(p.Tournee.Nom, p.Annee, p.Semaine, JourDeSaisie);
                        }

                        if (resJour == "lundi" && JourDAffichage.Day == 01)
                        {
                            background.AddDate(leJourDeLivraison, "circle", true);
                        }
                        else
                        {
                            background.AddDate(leJourDeLivraison, "circle",false);
                        }
                        

                        calendar.Background = background.GetBackground();

                        calendar.DisplayDateChanged += CalenderOnDisplayDateChanged;
                    }
                }
                calendar.SelectedDate = new DateTime(anneeDisplay, GestionDeDateCalendrier.TrouverLeMoisAvecNumSemaine(semaineDisplay, anneeDisplay), 1);

            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw;
            }
        }
        private void CalenderOnDisplayDateChanged(object sender, CalendarDateChangedEventArgs calendarDateChangedEventArgs)
        {
            calendar.Background = background.GetBackground();
        }
    }
}
