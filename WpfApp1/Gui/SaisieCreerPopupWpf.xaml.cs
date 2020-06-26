using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Logique d'interaction pour SaisieCreerPopupWpf.xaml
    /// </summary>
    public partial class SaisieCreerPopupWpf : Window
    {

        BaseContext db;

        public Saisie Edite { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public SaisieCreerPopupWpf(int semaine, int annee)
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new Saisie { Semaine = semaine , Annee = annee };
            edition.DataContext = this.Edite;
        }

        public void SaisieCreerPopupSaveAndNewWpf(int semaine, int annee)
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new Saisie { Semaine = semaine, Annee = annee };
            edition.DataContext = this.Edite;
        }

        /// <summary>
        /// On charge les tournées et les personnes pour les mettre dans des combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Chargement des personnes et assignation à la combobox
            IQueryable<Personne> req2 = from t in this.db.Personnes
                                          select t;

            List<Personne> data2 = new List<Personne>();
           
            foreach (Personne p in req2)
            {
                this.db.Entry(p).Reference(s => s.Tournee).Load();
                this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                this.db.Entry(p).Reference(s => s.ContactDurgence).Load();
             
               
                if (p.Actif == true)
                {
                    data2.Add(p);
                    data2.Sort((x, y) => string.Compare(x.Nom, y.Nom));
                } 
            }
            cbPersonne.ItemsSource = data2;
        }

        /// <summary>
        /// Verifier les données avant de valider l'opération
        /// </summary>
        private bool VerifierDonnees()
        {
            bool retval = false;
            
            if (txtSemaine.Text.Length != 0 && txtAnnee.Text.Length != 0 && cbPersonne.SelectedItem != null)
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
                        this.Edite.Tournee = this.Edite.Personne.Tournee;

                        // suivant la tournée, ouvrir une saisir ou une autre
                        if (this.Edite.Tournee.Nom == Properties.Resources.Ville1 || this.Edite.Tournee.Nom == Properties.Resources.Ville2)
                        {
                            Close();
                            int[] ID = SaisieDAO.getIdsFromYearWeekPersonne(this.Edite.Annee, this.Edite.Semaine, this.Edite.Personne, this.db);
                            var soirBackground = new ImageBrush(new BitmapImage(new Uri(Properties.Resources.ImgTourneeVilleSoir, UriKind.RelativeOrAbsolute)));
                            SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, ID, soirBackground);
                            wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri(Properties.Resources.ImgTourneeVille, UriKind.RelativeOrAbsolute)));
                            WinFormWpf.CornerTopLeftToParent(wpf, this);
                            wpf.ShowDialog();

                        }
                        else if (this.Edite.Tournee.Nom == Properties.Resources.CT)
                        {
                            Close();
                            int[] ID = SaisieDAO.getIdsFromYearWeekPersonne(this.Edite.Annee, this.Edite.Semaine, this.Edite.Personne, this.db);
                            var soirBackground = new ImageBrush(new BitmapImage(new Uri(Properties.Resources.ImgTourneeCTSoir, UriKind.RelativeOrAbsolute)));
                            SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, ID, soirBackground);
                            wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri(Properties.Resources.ImgTourneeCT, UriKind.RelativeOrAbsolute)));
                            WinFormWpf.CornerTopLeftToParent(wpf, this);
                            wpf.ShowDialog();
                        }
                        else if (this.Edite.Tournee.Nom == Properties.Resources.Marennes)
                        {
                            Close();
                            int[] ID = SaisieDAO.getIdsFromYearWeekPersonne(this.Edite.Annee, this.Edite.Semaine, this.Edite.Personne, this.db);
                            var soirBackground = new ImageBrush(new BitmapImage(new Uri(Properties.Resources.ImgTourneeMarennesSoir, UriKind.RelativeOrAbsolute)));
                            SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db, ID, soirBackground);
                            wpf.gridMain.Background = new ImageBrush(new BitmapImage(new Uri(Properties.Resources.ImgTourneeMarennes, UriKind.RelativeOrAbsolute)));
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
