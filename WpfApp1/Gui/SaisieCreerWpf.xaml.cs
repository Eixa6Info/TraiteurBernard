using org.hamcrest.core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.PDF;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieCreerWpf.xaml
    /// </summary>
    public partial class SaisieCreerWpf : Window
    {

        private ImageBrush soirBackground;
        private Saisie Edite
        {
            get;
            set;
        }

        public static Personne per;
        public static int semaine;

        private List<Saisie> saisieList;

        private List<ComboData> quantiteAvecSoirCombobox; // ComboBox pour le potage et l'entrée

        public List<ComboData> quantiteSansSoirCombobox; // ComboBox pour le potage et l'entrée

        /// <summary>
        /// Classe pour map les combos box pour la valeur (ce que l'on voit) et l'ID (la valeur qui rentre en bdd)
        /// </summary>
        public class ComboData
        {
            public int Id
            {
                get;
                set;
            }
            public string Value
            {
                get;
                set;
            }
        }

        private SaisieHelper saisieHelper;

        private int[] types = new int[8] {
           SaisieData.BAGUETTE,
            SaisieData.POTAGE,
            SaisieData.ENTREE_MIDI,
            SaisieData.PLAT_MIDI_1,
            SaisieData.PLAT_MIDI_2,
            SaisieData.PLAT_MIDI_3,
            SaisieData.FROMAGE,
            SaisieData.DESSERT_MIDI
        };

        private int[] typesBis = new int[8] {
           99,
           99,
           Plat.ENTREE_MIDI,
           Plat.PLAT_MIDI_1,
           Plat.PLAT_MIDI_2,
           Plat.PLAT_MIDI_3,
           99, // fromage
           Plat.DESSERT_MIDI
        };

        private BaseContext db;


        /// <summary>
        /// Constructeur avec en paramètre la saisie qui contient la semaine, le jour, la tournée, l'année et la personne
        /// </summary>
        /// <param name="edite"></param>
        public SaisieCreerWpf(Saisie edite, BaseContext db, int[] IDs, ImageBrush soirBackground)
        {
            InitializeComponent();
            this.soirBackground = soirBackground;
            this.Edite = edite;
            lblSemaine.Content = this.Edite.Semaine;
            lblPersonne.Content = this.Edite.Personne;
            semaine = this.Edite.Semaine;
            per = this.Edite.Personne;
            this.db = db;


            this.saisieHelper = new SaisieHelper(this.gridMain, this.types, this.typesBis, this.Edite, 1, 2, this.db);
        }





        /// <summary>
        /// Bouton pour le ouvrir la saisie du soir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Soir(object sender, RoutedEventArgs e)
        {
            // enregistrer les 8 premières infos afin que les plats du soir soient en position 8,9 et 10 dans les saisies
            this.saisieHelper.Save();

            var form = new SaisieCreerSoirWpf(Edite, null, this.db);
            form.gridMain.Background = this.soirBackground;
            form.ShowDialog();


        }

        /// <summary>
        /// Bouton Enregistrer la saisie et en ouvrir une autre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnregistrerEtNouveau(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
            SaisieCreerPopupWpf popupWpf = new SaisieCreerPopupWpf(this.Edite.Semaine, this.Edite.Annee);
            popupWpf.Show();
        }


        /// <summary>
        /// Bouton enregistrer
        /// </summary>
        private void Save()
        {
            this.saisieHelper.Save();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.saisieHelper.Save();
            this.db.Dispose();

            MessageBoxResult res = MessageBox.Show("Voulez-vous créer le pdf ?", "PDF", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                PdfCreerSaisieClient.PrintClient(per, semaine);
                Close();
            }
            else
            {
                Close();
            }

        }

        private int cal = 0;
        public static bool infoCal = true;
        private SaisieCreerCalendrierWpf wpf1;

        /// <summary>
        /// Ouvrir le calendrier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calendrier(object sender, EventArgs e)
        {

            this.wpf1 = new SaisieCreerCalendrierWpf(this.Edite, this.db, null);
            if (infoCal == false)
            {
                wpf1.Show();
            }
        }

        /// <summary>
        /// Au chargement de la page, on charge les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.wpf1 = new SaisieCreerCalendrierWpf(this.Edite, this.db, null);
            this.wpf1.Show();
            cal = 1;
        }


        /// <summary>
        /// Quand on ferme la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fermer(object sender, EventArgs e)
        {
            this.wpf1.Close();
            cal = 1;
            // fermetur de la fenetre
            Close();

        }
    }
}



