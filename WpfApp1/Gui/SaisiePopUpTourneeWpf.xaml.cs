using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Security;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisiePopUpTourneeWpf.xaml
    /// </summary>
    public partial class SaisiePopUpTourneeWpf : Window
    {
      
        BaseContext db;
        Saisie p1t1, p1t2, p1t3, p1t4;

        public Saisie Edite { get; set; }

        public SaisiePopUpTourneeWpf(int semaine, int annee, Personne personne)
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new Saisie { Semaine = semaine, Annee = annee, Personne = personne };
            edition.DataContext = this.Edite;
        }
        /*
        public void SaisieCreerPopupSaveAndNewWpf(int semaine, int annee)
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new Saisie { Semaine = semaine, Annee = annee };
            edition.DataContext = this.Edite;
        }*/

        /// <summary>
        /// On charge les tournées et les personnes pour les mettre dans des combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Chargement des personnes et assignation à la combobox
            IQueryable<TypeTournee> req2 = from t in this.db.TypeTournee
                                        select t;
            IQueryable<Saisie> req3 = from t in this.db.Saisies
                                        select t;

           
            List<TypeTournee> data2 = new List<TypeTournee>();
            List<Saisie> t1 = new List<Saisie>(); //ville 1
            List<Saisie> t2 = new List<Saisie>(); //ville 2
            List<Saisie> t3 = new List<Saisie>(); // ct
            List<Saisie> t4 = new List<Saisie>(); // marennes

            foreach (TypeTournee t in req2)
            {
                data2.Add(t);
            }
            cbTournee.ItemsSource = data2;

            foreach (Saisie sa in req3)
            {
                this.db.Entry(sa).Reference(s => s.Tournee).Load();
                this.db.Entry(sa).Reference(s => s.Personne).Load();
                //this.db.Entry(sa).Reference(s => s.Personne.ContactDurgence).Load();

                if (sa.Personne.Actif == true)
                {
                    if(sa.Personne.Tournee.ID == 1)
                    {
                        t1.Add(sa);
                        t1.Sort((x, y) => string.Compare(x.Personne.Nom, y.Personne.Nom));
                    }
                    else if (sa.Personne.Tournee.ID == 2)
                    {
                        t2.Add(sa);
                        t2.Sort((x, y) => string.Compare(x.Personne.Nom, y.Personne.Nom));
                    }
                    else if (sa.Personne.Tournee.ID == 3)
                    {
                        t3.Add(sa);
                        t3.Sort((x, y) => string.Compare(x.Personne.Nom, y.Personne.Nom));
                    }
                    else if (sa.Personne.Tournee.ID == 4)
                    {
                        t4.Add(sa);
                        t4.Sort((x, y) => string.Compare(x.Personne.Nom, y.Personne.Nom));
                    }
                }
            }
            
            p1t1 = t1.First();
            p1t2 = t2.First();
            p1t3 = t3.First();
            p1t4 = t4.First();
        }

        /// <summary>
        /// Verifier les données avant de valider l'opération
        /// </summary>
        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtSemaine.Text.Length != 0 && txtAnnee.Text.Length != 0 && cbTournee.SelectedItem != null)
            {
                retval = true;
            }

            return retval;
        }

        /// <summary>
        /// Bouton valider, ferme la fenetre et ouvre la fenetre de creation principal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {

            try
            {
                if (this.VerifierDonnees())
                {
                    int semaine = Int16.Parse(txtSemaine.Text);
                    

                    if (semaine < 53)
                    {
                     
                        if (cbTournee.Text == "ville 1") 
                        {
                            this.Edite = p1t1;
                            this.Edite.Semaine = semaine;
                        }
                        else if (cbTournee.Text == "ville 2")
                        {
                            this.Edite = p1t2;
                            this.Edite.Semaine = semaine;
                        }
                        else if (cbTournee.Text == "contre-tournée")
                        {
                            this.Edite = p1t3;
                            this.Edite.Semaine = semaine;
                        }
                        else if (cbTournee.Text == "Marennes")
                        {
                            this.Edite = p1t4;
                            this.Edite.Semaine = semaine;
                        }

                        // Objet pour récupérer les images
                        var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

                        // suivant la tournée, ouvrir une saisir ou une autre
                        if (this.Edite.Tournee.Nom == Properties.Resources.Ville1 || this.Edite.Tournee.Nom == Properties.Resources.Ville2)
                        {
                            Close();
                            var soirBackground = new ImageBrush(new BitmapImage(new Uri(Path.Combine(outPutDirectory, Properties.Resources.ImgTourneeVilleSoir))));
                            SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, soirBackground, false);
                            wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri(Path.Combine(outPutDirectory, Properties.Resources.ImgTourneeVille))));
                            WinFormWpf.CornerTopLeftToParent(wpf, this);
                            wpf.ShowDialog();

                        }
                        else if (this.Edite.Tournee.Nom == Properties.Resources.CT)
                        {
                            Close();
                            var soirBackground = new ImageBrush(new BitmapImage(new Uri(Path.Combine(outPutDirectory, Properties.Resources.ImgTourneeCTSoir))));
                            SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, soirBackground, false);
                            wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri(Path.Combine(outPutDirectory, Properties.Resources.ImgTourneeCT))));
                            WinFormWpf.CornerTopLeftToParent(wpf, this);
                            wpf.ShowDialog();
                        }
                        else if (this.Edite.Tournee.Nom == Properties.Resources.Marennes)
                        {
                            Close();
                            var soirBackground = new ImageBrush(new BitmapImage(new Uri(Path.Combine(outPutDirectory, Properties.Resources.ImgTourneeMarennesSoir))));
                            SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, soirBackground, false);
                            wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri(Path.Combine(outPutDirectory, Properties.Resources.ImgTourneeMarennes))));
                            WinFormWpf.CornerTopLeftToParent(wpf, this);
                            wpf.ShowDialog();
                        }
                        else
                        {
                            var wpf = new MessageBoxWpf("Tournée manquante", $"La saisie pour cette tournée {this.Edite.Tournee.Nom} n'est pas disponible", MessageBoxButton.OK);
                            WinFormWpf.CenterToParent(wpf, this);
                            wpf.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpSemaine, Properties.Resources.MessagePopUpSemaineHorsAnnee, MessageBoxButton.OK);
                        WinFormWpf.CenterToParent(wpf, this);
                        wpf.ShowDialog();
                    }

                }
                else
                {
                    MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensable, Properties.Resources.MessagePopUpErrorIndispensable, MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                }

            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerPopupWpf.xaml.cs");
                throw a;
            }
        }

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Verifier que le format entré est bien une semaine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifierFormatSemaine(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);
        }

        /// <summary>
        /// Verifier que la saisie correspond bien a une année
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifierFormatAnnee(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);
        }
    }
}
