using org.apache.pdfbox.pdmodel.common;
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
using TraiteurBernardWPF.Utils;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using com.sun.tools.apt;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using javax.sound.midi;
using java.sql;
using System.Globalization;
using System.Windows.Threading;
using System.Reflection;

namespace TraiteurBernardWPF
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int i = 0;
        AnnivClient annivClient = new AnnivClient();
        public void CréationDuFichierXml()
        {
            XElement root = new XElement("nas",
            new XElement("chemin", "chemin du document DB dans le nas"),
            new XElement("dbFile", "chemin du fichier traiteur.db dans le nas"),
            new XElement("lock", "chemin du fichier lock dans le nas"),
            new XElement("oldDbFile", "chemin du fichier traiteur sans le db dans le nas")
            );
            root.Save(@"C:\eixa6\nas.xml");
        }

        GestionFichier gestionFichier = new GestionFichier();
        const string nasXml = @"C:\eixa6\nas.xml";

        public MainWindow()
        {
            gestionFichier.testVariable();
            if (!File.Exists(nasXml))
            {
                gestionFichier.FichierXMLExiste();
            }
            else
            {
                // On regarde si le fichier lock est present
                gestionFichier.LockFileExiste();

                WinFormWpf.CornerTopLeftToComputer(this);
                InitializeComponent();

                // Version de l'application
                //Title += "  |  v.1.1.0  |  25/03/2020";
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                Title = fvi.FileVersion;
                //On recupere le fichier traiteur.db du nas vers le fichier local si il existe
                gestionFichier.ExisteSurNas();
                // On créé la base de données si elle existe pas
                BaseContext db = new BaseContext();
                db.Database.EnsureCreated();
                db.Dispose();

                if (i == 0)
                {
                    this.DeleteTypeTournee();
                    this.InitialiserBdd();
                }

            }
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {

            List<Personne> listAnnivPersonne = annivClient.AnnvClientAll();
            String anniv = "";

            foreach (var p in listAnnivPersonne)
            {
                annivListBox.Items.Add(p.ToString());
            }

            if (annivListBox.Items.Count != 0)
            {
                annivLabel.Content = "Anniversaire de : ";
            }
            else
            {
                annivLabel.Content = "Il n'y a pas d'anniversaire ";
            }
            InitLiveTimeLabel();
        }

        private void InitLiveTimeLabel()
        {
            // Date
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            TimeLabel.Content = date.ToString("HH:mm", CultureInfo.CreateSpecificCulture("fr-FR"));
            DateLabel.Content = date.ToString("dddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("fr-FR"));
        }

        private void DeleteTypeTournee()
        {
            using (BaseContext db = new BaseContext())
            {
                var l = db.Livraisons;
                foreach (var ls in l)
                {
                    db.Entry(ls).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
        }
        private void InitialiserBdd()
        {
            using (BaseContext db = new BaseContext())
            {
                // insertion de la tournée ville1
                IList<Livraison> jours = new List<Livraison>();
                var l = new Livraison();
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
                l.JourRepas2 = "samedi";
                l.JourRepas3 = "dimanche";
                jours.Add(l);

                var ville1 = db.TypeTournee.First(a => a.ID == 1);
                ville1.Nom = "ville 1";
                ville1.JoursLivraisonsRepas = jours;
                db.SaveChanges();

                // insertion de la ville2
                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "vendredi";
                l.JourRepas2 = "samedi";
                l.JourRepas3 = "dimanche";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "jeudi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 = "mercredi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mardi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "lundi";
                jours.Add(l);

                var ville2 = db.TypeTournee.First(a => a.ID == 2);
                ville2.Nom = "ville 2";
                ville2.JoursLivraisonsRepas = jours;
                db.SaveChanges();

                // insertion de la contre-tournée
                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "samedi";
                l.JourRepas2 = "dimanche";
                l.JourRepas3 = "lundi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 = "jeudi";
                l.JourRepas2 = "vendredi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "mardi";
                l.JourRepas2 = "mercredi";
                jours.Add(l);

                var contre = db.TypeTournee.First(a => a.ID == 3);
                contre.Nom = "contre-tournée";
                contre.JoursLivraisonsRepas = jours;
                db.SaveChanges();


                // insertion de marennes
                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "vendredi";
                l.JourRepas2 = "samedi";
                l.JourRepas3 = "dimanche";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mercredi";
                l.JourRepas2 = "jeudi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "samedi";
                l.JourRepas1 = "lundi";
                l.JourRepas2 = "mardi";
                jours.Add(l);
                var marennes = db.TypeTournee.First(a => a.ID == 4);
                marennes.Nom = "Marennes";
                marennes.JoursLivraisonsRepas = jours;
                db.SaveChanges();

                i = 1;
            }

        }

        /// <summary>
        /// Initialisation, création des tournées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ReinitialiserBdd(object sender, RoutedEventArgs e)
        {

            MessageBoxWpf wpf = new MessageBoxWpf("Confirmation", "Vous êtes sur le point de remettre à zéro toutes les données, voulez-vous continuer ?", MessageBoxButton.YesNo);
            wpf.ShowDialog();
            if (!wpf.YesOrNo) return;

            Cursor = Cursors.Wait;


            using (BaseContext db = new BaseContext())
            {

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // insertion de la tournée ville1
                IList<Livraison> jours = new List<Livraison>();
                var l = new Livraison();
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
                l.JourRepas2 = "samedi";
                l.JourRepas3 = "dimanche";
                jours.Add(l);

                db.TypeTournee.Add(new TypeTournee { Nom = "ville 1", JoursLivraisonsRepas = jours });
                db.SaveChanges();

                // insertion de la ville2
                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "vendredi";
                l.JourRepas2 = "samedi";
                l.JourRepas3 = "dimanche";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "jeudi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 = "mercredi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mardi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "lundi";
                jours.Add(l);
                db.TypeTournee.Add(new TypeTournee { Nom = "ville 2", JoursLivraisonsRepas = jours });
                db.SaveChanges();

                // insertion de la contre-tournée
                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "vendredi";
                l.JourRepas1 = "samedi";
                l.JourRepas2 = "dimanche";
                l.JourRepas3 = "lundi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mercredi";
                l.JourRepas1 = "jeudi";
                l.JourRepas2 = "vendredi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "lundi";
                l.JourRepas1 = "mardi";
                l.JourRepas2 = "mercredi";
                jours.Add(l);
                db.TypeTournee.Add(new TypeTournee { Nom = "contre-tournée", JoursLivraisonsRepas = jours });
                db.SaveChanges();

                // insertion de marennes
                jours = new List<Livraison>();
                l = new Livraison();
                l.JourLivraison = "jeudi";
                l.JourRepas1 = "vendredi";
                l.JourRepas2 = "samedi";
                l.JourRepas3 = "dimanche";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "mardi";
                l.JourRepas1 = "mercredi";
                l.JourRepas2 = "jeudi";
                jours.Add(l);
                l = new Livraison();
                l.JourLivraison = "samedi";
                l.JourRepas1 = "lundi";
                l.JourRepas2 = "mardi";
                jours.Add(l);
                db.TypeTournee.Add(new TypeTournee { Nom = "Marennes", JoursLivraisonsRepas = jours });
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
            Cursor = Cursors.Arrow;
        }



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
            try
            {
                PersonneCreerWpf wpf = new PersonneCreerWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }

        }

        /// <summary>
        /// Ouverture de la fenêtre qui liste les personnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Personne_Lister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PersonneListerWpf wpf = new PersonneListerWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }

        }

        /// <summary>
        /// Ouverture de la fenêtre pour créer un compte de facturation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ComptesDeFacturation_Creer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CompteDeFacturationCreerWpf wpf = new CompteDeFacturationCreerWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Ouverture de la fenêtre qui liste les comptes de facturations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ComptesDeFacturation_Lister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CompteDeFacturationListerWpf wpf = new CompteDeFacturationListerWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Création d'un PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Menus_Pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfCreerWpf wpf = new PdfCreerWpf(1, 4);   // valeur 1 pour le numero de semaine valeur 4 pour dire que c'est un menu
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }


        /// <summary>
        /// Ouverture de la fenêtre qui liste les tournées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Tournees_Lister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TourneesListerWpf wpf = new TourneesListerWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }


        /// <summary>
        /// Ouverture de la fenêtre qui liste les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Menus_Lister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuListerWpf wpf = new MenuListerWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Ouverture de la fenêtre pour créer un menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Menus_Creer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuCreerWpf wpf = new MenuCreerWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Ouverture de la fenêtre pour créer une saisie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Saisies_Creer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaisieCreerPopupWpf wpfPopup = new SaisieCreerPopupWpf(1, DateTime.Now.Year);
                WinFormWpf.CornerTopLeftToParent(wpfPopup, this);
                wpfPopup.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }


        /// <summary>
        /// Création d'un PDF pour la cuisine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Saisies_Pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfCreerWpf wpf = new PdfCreerWpf(1, 1);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Création d'un PDF pour la cuisine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Saisies_Pdf5Feuilles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BoiteDialogueCompoWpf wpf = new BoiteDialogueCompoWpf(1);
                //PdfCreerWpf wpf = new PdfCreerWpf(1, 2);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Création d'un PDF pour la cuisine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Saisies_PdfJambon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfCreerWpf wpf = new PdfCreerWpf(1, 3);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Ouverture de la fenêtre d'imporation JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImporterJson(object sender, RoutedEventArgs e)
        {
            try
            {
                ImporterJson wpf = new ImporterJson();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Ouverture de la fenêtre d'exportation des personnes en JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExporterJson(object sender, RoutedEventArgs e)
        {
            try
            {
                ExporterJson wpf = new ExporterJson();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        /// <summary>
        /// Ouverture de la fenêtre 'A propos'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void APropos(object sender, RoutedEventArgs e)
        {
            try
            {
                AProposWpf wpf = new AProposWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        private void MenuItem_Saisies_PdfCompositions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfCreerWpf wpf = new PdfCreerWpf(1, 1, true);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gestionFichier.EnregistrerSurNasEtQuitteApp();
        }

        private void MenuItem_Saisies_Pdf_Client_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfCreerSaisieClient wpf = new PdfCreerSaisieClient();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        private void MenuItem_Saisies_PdfMarennes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfCreerWpf wpf = new PdfCreerWpf(1, 5);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }

        }

        private void MenuItem_Saisies_Pdf5FeuillesMarennse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BoiteDialogueCompoWpf wpf = new BoiteDialogueCompoWpf(2);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        private void MenuItem_Saisies_Creer_Tournee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaisiePopUpTourneeWpf wpf = new SaisiePopUpTourneeWpf(1, DateTime.Now.Year, null);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        private void MenuItem_Facture_ClientAPAMSA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfFacturationMoisCreeWpf wpf = new PdfFacturationMoisCreeWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        private void MenuItem_Facture_Client_Normal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfFacturationCreeWpf wpf = new PdfFacturationCreeWpf();
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }
    }
}
