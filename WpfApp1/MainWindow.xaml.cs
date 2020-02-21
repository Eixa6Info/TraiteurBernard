﻿using org.apache.pdfbox.pdmodel.common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.PDF;
using TraiteurBernardWPF.Gui;

namespace TraiteurBernardWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialisation, création des tournées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (BaseContext db = new BaseContext())
            {

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // insertion de la tournée ville1
                IList<Livraison> jours = new List<Livraison>();
                var l = new Livraison();
                l.JourLivraison = "samedi";
                l.JourRepas1 = "samedi";

                l.JourRepas2  =  "dimanche" ;
                jours.Add(l);

                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "lundi" ;
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mardi" ;
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 =  "mercredi" ;
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "jeudi" ;
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "vendredi" ;
                jours.Add(l);


                db.TypeTournee.Add(new TypeTournee { Nom = "ville1", JoursLivraisonsRepas = jours });
                db.SaveChanges();

                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "samedi";
                l.JourRepas1 = "samedi";
                l.JourRepas2 = "dimanche";
                jours.Add(l);

                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "lundi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mardi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 = "mercredi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "jeudi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "vendredi";
                jours.Add(l);

                db.TypeTournee.Add(new TypeTournee { Nom = "ville", JoursLivraisonsRepas = jours });
                db.SaveChanges();

                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "samedi";
                l.JourRepas1 = "samedi";
                l.JourRepas2 = "dimanche";
                jours.Add(l);

                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "lundi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mardi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 = "mercredi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "jeudi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "vendredi";
                jours.Add(l);


                db.TypeTournee.Add(new TypeTournee { Nom = "contre-tournée", JoursLivraisonsRepas = jours });
                db.SaveChanges();

                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "samedi";
                l.JourRepas1 = "samedi";
                l.JourRepas2 = "dimanche";
                jours.Add(l);

                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "lundi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mardi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 = "mercredi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "jeudi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "vendredi";
                jours.Add(l);


                db.TypeTournee.Add(new TypeTournee { Nom = "marennes", JoursLivraisonsRepas = jours });
                db.SaveChanges();

                /*     var req = from t in db.TypeTournee
                              select t;

                    var data = new List<TypeTournee>();

                    foreach(var tt in req)
                    {
                        data.Add(tt);
                    }
                */

            }
        }

        /// <summary>
        /// Fermeture de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Quitter_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Ouverture de la fenêtre pour créer une personne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Personne_Creer_Click(object sender, RoutedEventArgs e)
        {
            PersonneCreerWpf wpf = new PersonneCreerWpf();
            wpf.ShowDialog();
        }

        /// <summary>
        /// Ouverture de la fenêtre qui liste les personnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Personne_Lister_Click(object sender, RoutedEventArgs e)
        {
            PersonneListerWpf wpf = new PersonneListerWpf();
            wpf.ShowDialog();
        }

        /// <summary>
        /// Ouverture de la fenêtre pour créer un compte de facturation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ComptesDeFacturation_Creer_Click(object sender, RoutedEventArgs e)
        {
            CompteDeFacturationCreerWpf wpf = new CompteDeFacturationCreerWpf();
            wpf.ShowDialog();
        }

        /// <summary>
        /// Ouverture de la fenêtre qui liste les comptes de facturations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ComptesDeFacturation_Lister_Click(object sender, RoutedEventArgs e)
        {
            CompteDeFacturationListerWpf wpf = new CompteDeFacturationListerWpf();
            wpf.ShowDialog();
        }

        /// <summary>
        /// Création d'un PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreatePDFSemaine1_Click(object sender, RoutedEventArgs e)
        {
            CreatePDF.Start(595.27563F, 841.8898F, 1);
        }
        
        /// <summary>
        /// Ouverture de la fenêtre qui liste les tournées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Tournees_Lister_Click(object sender, RoutedEventArgs e)
        {
            TourneesListerWpf wpf = new TourneesListerWpf();
            wpf.ShowDialog();
        }
        
        
        /// <summary>
        /// Ouverture de la fenêtre qui liste les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Menus_Lister_Click(object sender, RoutedEventArgs e)
        {
            MenuListerWpf wpf = new MenuListerWpf();
            wpf.ShowDialog();
        }
       
        /// <summary>
        /// Ouverture de la fenêtre pour créer un menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Menus_Creer_Click(object sender, RoutedEventArgs e)
        {
            MenuCreerWpf wpf = new MenuCreerWpf();
            wpf.ShowDialog();
        }


        

    }
}
