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

namespace TraiteurBernardWPF
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlDocument monFichier = new XmlDocument();
        private static string xmlNas = @"C:\eixa6\nas.xml";
        public MainWindow()
        {

            
            string nas = RechercheCheminDansFichierXml(monFichier);
            string lockFile = RechercheCheminDansFichierXmlLock(monFichier);
            string messageBoxText = "";
            string caption = "";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage iconW = MessageBoxImage.Error;
            MessageBoxImage iconI = MessageBoxImage.Information;
            MessageBoxImage iconA = MessageBoxImage.Warning;

            if (File.Exists(lockFile))
            {
                messageBoxText = "Un autre utilisateur travail sur le fichier de base de données. Voulez-vous quand même travailler sur le même fichier ? Un risque de tous perdre est envisageable";
                caption = "Alerte";
                var res = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNo, iconA);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        File.Delete(lockFile);
                        break;
                    case MessageBoxResult.No:
                        Close();
                        break; 
                }
            }
            else
            {
                File.Create(lockFile);
            }

            WinFormWpf.CornerTopLeftToComputer(this);
            InitializeComponent();

            // Version de l'application
            //Title += "  |  v.1.1.0  |  25/03/2020";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Title = fvi.FileVersion;
            //On recupere le fichier traiteur.db du nas vers le fichier local
            string local = @"C:\eixa6\traiteur.db";
            
            

            if (LeFichierTraiteurdbExisteSurNas(nas))
            {
                try
                {
                    if (File.GetLastWriteTime(MainWindow.RechercheCheminDansFichierXmlDbFichier(monFichier)) >= File.GetLastWriteTime(local))
                    {
                        System.IO.File.Delete(local);
                        System.IO.File.Copy(nas, local);
                        messageBoxText = "Le fichier de données a été récupéré sur le NAS.";
                        caption = "Récupération des données";
                        MessageBox.Show(messageBoxText, caption, button, iconI);
                    }
                    else
                    {
                        System.IO.File.Delete(nas);
                        System.IO.File.Copy(local, nas);
                        messageBoxText = "Le fichier de données a été récupéré sur le NAS.";
                        caption = "Récupération des données";
                        MessageBox.Show(messageBoxText, caption, button, iconI);
                    }
                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = "Problème avec le fichier de données! Contacter EIXA6 Informatique";
                    caption = "Alerte";
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }
            }
            else
            {
                messageBoxText = "Vous n'avez pas acces au NAS voulez-vous travailer en local ?";
                caption = "Alerte";
                var res = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNo, iconW);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        return;
                        break;
                    case MessageBoxResult.No:
                        Close();
                        break;
                }
                LogHelper.WriteToFile("Probleme de fichier db dans le nas verifier le nas et le XML c:/eixa6/nas.xml", "MainWindow.xaml.cs");
                return;
            }

            // On créé la base de données si elle existe pas
            BaseContext db = new BaseContext();
            db.Database.EnsureCreated();
            db.Dispose();
        }

        private static string RechercheCheminDansFichierXml(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("dbFile");
            string nas = "";
            foreach (XmlNode n in Nas)
            {
                nas = n.InnerText;
            }
            return nas;
        }

        private static string RechercheCheminDansFichierXmlLock(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("lock");
            string nas = "";
            foreach (XmlNode n in Nas)
            {
                nas = n.InnerText;
            }
            return nas;
        }

        private static bool LeFichierTraiteurdbExisteSurNas(string nas)
        {
            return System.IO.File.Exists(nas);
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

        private static string RechercheCheminDansFichierXmlCheminNas(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("chemin");
            string fileName = "";
            foreach (XmlNode n in Nas)
            {
                fileName = n.InnerText;
            }
            return fileName;
        }
        private static string RechercheCheminDansFichierXmlDbFichier(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("dbFile");
            string nas = "";
            foreach (XmlNode n in Nas)
            {
                nas = n.InnerText;
            }
            return nas;
        }
        private static string RechercheCheminDansFichierXmlVieuFichierDb(XmlDocument monFichier)
        {
            DateTime date = DateTime.Now;
            string dateStr = "";
            string jour = date.Day.ToString();
            string mois = date.Month.ToString();
            string annee = date.Year.ToString();
            if (jour.Length == 1)
            {
                jour = "0" + date.Day;
            }
            if (mois.Length == 1)
            {
                mois = "0" + date.Month;
            }
            dateStr = jour + mois + annee;
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("oldDbFile");
            string nasNew = "";
            foreach (XmlNode n in Nas)
            {
                nasNew = n.InnerText + dateStr + ".db";
            }
            return nasNew;
        }
        
        private void MenuItem_Quitter_Click(object sender, RoutedEventArgs e)
        {
            //Close();
        }
        /// <summary>
        /// Fermeture de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument monFichier = new XmlDocument();
            
            string local = @"C:\eixa6\traiteur.db";
            string nas = RechercheCheminDansFichierXmlDbFichier(monFichier);
            string nasNew = RechercheCheminDansFichierXmlVieuFichierDb(monFichier);
            string fileName = RechercheCheminDansFichierXmlCheminNas(monFichier);
            string[] res;
            string nasOld = "";
            string messageBoxText = "";
            string caption = "";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage iconW = MessageBoxImage.Error;
            MessageBoxImage iconI = MessageBoxImage.Information;

            if (System.IO.File.Exists(nas))
            {
                try
                {
                    res = System.IO.Directory.GetFiles(fileName);
                    foreach (string file in res)
                    {
                        if (LeFichierSelectonnéEstPasTraiteurdb(nas, file) && LeNombreDeFichierEstSuppAUn(file))
                        {
                            nasNew = file;
                        }
                    }
                    if (DeuxiemeSauvegardeSurNas(res))
                    {
                        // On copie le fichier traiteur.db au 
                        System.IO.File.Copy(nas, nasNew);
                        System.IO.File.Delete(nas);
                        System.IO.File.Copy(local, nas);
                    }
                    else
                    {
                        nasOld = nasNew;
                        // On supprime le fichier traiteurdate.db
                        System.IO.File.Delete(nasNew);
                        // la fichier traiteur.db passe en traiteurdate.db
                        System.IO.File.Copy(nas, nasNew);
                        // On supprime le fichier traiteur.db
                        System.IO.File.Delete(nas);
                        // On copie le fichier traiteur.db en local sur le nas
                        System.IO.File.Copy(local, nas);
                    }

                    messageBoxText = "Le fichier de données est enregistré sur le NAS.";
                    caption = "Enregistrement de données";
                    MessageBox.Show(messageBoxText, caption, button, iconI);

                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = "Problème avec le fichier de données! Contacter EIXA6 Informatique";
                    caption = "Alerte";
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }
            }
            else if (PremiereSauvegardeSurNas(local, nas))
            {
                try
                {
                    //On Copie le fichier traiteur.db de local au nas
                    System.IO.File.Copy(local, nas);

                    messageBoxText = "Le fichier de données est enregistré sur le NAS.";
                    caption = "Enregistrement de données";
                    MessageBox.Show(messageBoxText, caption, button, iconI);
                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = "Problème avec le fichier de données! Contacter EIXA6 Informatique";
                    caption = "Alerte";
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }

            }
        }

        private static bool LeNombreDeFichierEstSuppAUn(string file)
        {
            return file.Length > 1;
        }

        private static bool LeFichierSelectonnéEstPasTraiteurdb(string nas, string file)
        {
            return file != nas;
        }

        private static bool DeuxiemeSauvegardeSurNas(string[] res)
        {
            return res.Length == 1;
        }

        private static bool PremiereSauvegardeSurNas(string local, string nas)
        {
            return System.IO.File.Exists(local) && !System.IO.File.Exists(nas);
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
                PdfCreerWpf wpf = new PdfCreerWpf(false);
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
                SaisieCreerPopupWpf wpfPopup = new SaisieCreerPopupWpf();
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
        /// Création d'un PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Saisies_Pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfCreerWpf wpf = new PdfCreerWpf(true);
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
                PdfCreerWpf wpf = new PdfCreerWpf(true, true);
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
            XmlDocument monFichier = new XmlDocument();

            string local = @"C:\eixa6\traiteur.db";
            string nas = RechercheCheminDansFichierXmlDbFichier(monFichier);
            string nasNew = RechercheCheminDansFichierXmlVieuFichierDb(monFichier);
            string fileName = RechercheCheminDansFichierXmlCheminNas(monFichier);
            string[] res;
            string nasOld = "";
            string messageBoxText = "";
            string caption = "";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage iconW = MessageBoxImage.Error;
            MessageBoxImage iconI = MessageBoxImage.Information;

            if (System.IO.File.Exists(nas))
            {
                try
                {
                    res = System.IO.Directory.GetFiles(fileName);
                    foreach (string file in res)
                    {
                        if (LeFichierSelectonnéEstPasTraiteurdb(nas, file) && LeNombreDeFichierEstSuppAUn(file))
                        {
                            nasNew = file;
                        }
                    }
                    if (DeuxiemeSauvegardeSurNas(res))
                    {
                        // On copie le fichier traiteur.db au 
                        System.IO.File.Copy(nas, nasNew);
                        System.IO.File.Delete(nas);
                        System.IO.File.Copy(local, nas);
                    }
                    else
                    {
                        nasOld = nasNew;
                        // On supprime le fichier traiteurdate.db
                        System.IO.File.Delete(nasNew);
                        // la fichier traiteur.db passe en traiteurdate.db
                        System.IO.File.Copy(nas, nasNew);
                        // On supprime le fichier traiteur.db
                        System.IO.File.Delete(nas);
                        // On copie le fichier traiteur.db en local sur le nas
                        System.IO.File.Copy(local, nas);
                    }

                    messageBoxText = "Le fichier de données est enregistré sur le NAS.";
                    caption = "Enregistrement de données";
                    MessageBox.Show(messageBoxText, caption, button, iconI);

                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = "Problème avec le fichier de données! Contacter EIXA6 Informatique";
                    caption = "Alerte";
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }
            }
            else if (PremiereSauvegardeSurNas(local, nas))
            {
                try
                {
                    //On Copie le fichier traiteur.db de local au nas
                    System.IO.File.Copy(local, nas);

                    messageBoxText = "Le fichier de données est enregistré sur le NAS.";
                    caption = "Enregistrement de données";
                    MessageBox.Show(messageBoxText, caption, button, iconI);
                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = "Problème avec le fichier de données! Contacter EIXA6 Informatique";
                    caption = "Alerte";
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }     
            }
            File.Delete(MainWindow.RechercheCheminDansFichierXmlLock(monFichier));
        }
    }
}
