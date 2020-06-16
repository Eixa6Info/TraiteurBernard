﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using GregorianCalendar = System.Globalization.GregorianCalendar;
using Calendar = System.Globalization.Calendar;
using System.IO;
using TraiteurBernardWPF.Utils;
using TraiteurBernardWPF.DAO;
using System.Data.Entity;
using java.lang;
using System.Collections.Immutable;

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
        CalenderBackground background;
        private int semaineDisplay;
        private int anneeDisplay;

        public SaisieCreerCalendrierWpf(Saisie edite, BaseContext db, int[] IDs)
        {
            InitializeComponent(); 
            //this.db = new BaseContext();
            this.db = db;
            this.Edite = edite;
            this.IDs = IDs;
            this.semaineDisplay = edite.Semaine;
            this.anneeDisplay = edite.Annee;
        }

        private void calendar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            SaisieCreerWpf.infoCal = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Personne personne = Edite.Personne;
                calendar.IsTodayHighlighted = false;
                background = new CalenderBackground(calendar);
                background.AddOverlay("circle", @"C:\\eixa6\\circle.png");
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
                        calendar.SelectedDates.Add(JourDeSaisie);


                        calendar.DisplayDate = new DateTime(JourDAffichage.Year, JourDAffichage.Month, JourDAffichage.Day);
                        DateTime leJourDeLivraison;

                        if (p.Tournee.Nom == "")
                        {
                            leJourDeLivraison = LivraisonDAO.JourDeLivraisonCal(db, personne.Tournee.Nom, p.Annee, p.Semaine, JourDeSaisie);
                        }
                        else
                        {
                            // Afficher sur le calendrier les jours de livraison par rapport au saisie
                            leJourDeLivraison = LivraisonDAO.JourDeLivraisonCal(db, p.Tournee.Nom, p.Annee, p.Semaine, JourDeSaisie);
                        }
                            
                        int j = leJourDeLivraison.Day;
                        int m = leJourDeLivraison.Month;
                        int y = leJourDeLivraison.Year;

                        background.AddDate(new DateTime(y, m, j), "circle");

                        calendar.Background = background.GetBackground();

                        calendar.DisplayDateChanged += CalenderOnDisplayDateChanged;
                    }
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
    }
}
