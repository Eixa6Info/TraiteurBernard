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
using WpfApp1.Gui;

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

        // Initialisation, création des tournées
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new BaseContext())
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

        private void MenuItem_Quitter_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Personne_Creer_Click(object sender, RoutedEventArgs e)
        {
            // ouvrir une boîte de création des personnes
            var p = new PersonneCreerWpf();
            p.ShowDialog();
        }

        private void MenuItem_Personne_Lister_Click(object sender, RoutedEventArgs e)
        {
            // ouvrir une boîte de création des personnes
            var p = new PersonneListerWpf();
            p.ShowDialog();
        }

        private void btnCreatePDFSemaine1_Click(object sender, RoutedEventArgs e)
        {
            CreatePDF.Start(595.27563F, 841.8898F, 1);
        }
    }
}
