using com.sun.xml.@internal.fastinfoset.vocab;
using java.io;
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
        public SaisieCreerWpf(Saisie edite, BaseContext db,  ImageBrush soirBackground, bool byClientOrTournee)
        {
            InitializeComponent();
            this.soirBackground = soirBackground;
            this.Edite = edite;
            lblSemaine.Content = this.Edite.Semaine;
            lblPersonne.Content = this.Edite.Personne;
            semaine = this.Edite.Semaine;
            per = this.Edite.Personne;
            this.db = db;
            if (byClientOrTournee == true)
            {
                // c'est par un client
                this.btnEnregistrerEtNouveau.Visibility = Visibility.Hidden;
                this.btnEnregistrerEtNouveau2.Visibility = Visibility.Visible;
            }
            else
            {
                // c'est par tournée
                this.btnEnregistrerEtNouveau2.Visibility = Visibility.Hidden;
                this.btnEnregistrerEtNouveau.Visibility = Visibility.Visible;
            }

            this.saisieHelper = new SaisieHelper(this.gridMain, this.types, this.typesBis, this.Edite, 1, 2, this.db);
        }





        /// <summary>
        /// Bouton pour le ouvrir la saisie du soir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Soir(object sender, RoutedEventArgs e)
        {
            try
            {
                BaseContext newDb = new BaseContext();
                // enregistrer les 8 premières infos afin que les plats du soir soient en position 8,9 et 10 dans les saisies
                this.saisieHelper.Save();
                
                var form = new SaisieCreerSoirWpf(Edite, null, newDb);
                form.gridMain.Background = this.soirBackground;
                form.Show();
                this.CalToTop();


            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }
            
        }

        /// <summary>
        /// Bouton Enregistrer la saisie et en ouvrir une autre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnregistrerEtNouveau(object sender, RoutedEventArgs e)
        {
            try
            {
                BaseContext newDb = new BaseContext();
                var persons = (from p in newDb.Personnes where p.Tournee == this.Edite.Personne.Tournee && p.Actif == true select p).ToList();
                persons = persons.OrderBy(p => p.Nom).ToList();
                int index = persons.FindIndex(i => i.ID == this.Edite.Personne.ID);
                if (index + 2 > persons.Count)
                {
                    MessageBoxWpf wpf = new MessageBoxWpf("Information", "Il n'y a pas d'autres personnes dans cette tournée.", MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                    return;
                }
                Personne nextPerson = persons[index + 1];

                newDb.Entry(nextPerson).Reference(s => s.Tournee).Load();
                newDb.Entry(nextPerson).Reference(s => s.CompteDeFacturation).Load();
                newDb.Entry(nextPerson).Reference(s => s.ContactDurgence).Load();

                Save();
                this.db.Dispose();
           
                this.Edite.Personne = nextPerson;
                this.Edite.Tournee = nextPerson.Tournee;

                SaisieCreerWpf saisieCreerWpf = new SaisieCreerWpf(this.Edite, newDb, this.soirBackground, false);
                saisieCreerWpf.gridMain.Background = this.gridMain.Background;
                WinFormWpf.CornerTopLeftToParent(saisieCreerWpf, this);
                this.Close();
                saisieCreerWpf.Show();
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }
            
        }

        private void EnregistrerEtNouveau2(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                int year = dateTime.Year;
                int week = int.Parse(lblSemaine.Content.ToString());
                Save();
                Close();
                SaisieCreerPopupWpf wpf = new SaisieCreerPopupWpf(week, year) ;
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }
        }

        /// <summary>
        /// Bouton enregistrer
        /// </summary>
        private void Save()
        {
            try
            {
                this.saisieHelper.Save();
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            } 
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
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
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }
        }

        public static bool infoCal = true;
        private SaisieCreerCalendrierWpf wpf1;

        private void CalToTop()
        {
            if (infoCal == true)
            {
                this.wpf1.Hide();
            }

            this.wpf1 = new SaisieCreerCalendrierWpf(this.Edite, this.db, null);
            wpf1.Show();
            
        }

        /// <summary>
        /// Ouvrir le calendrier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calendrier(object sender, EventArgs e)
        {
            try
            {
                this.wpf1 = new SaisieCreerCalendrierWpf(this.Edite, this.db, null);
                if (infoCal == false)
                {
                    wpf1.Show();
                }
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            } 
        }

        /// <summary>
        /// Au chargement de la page, on charge les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.wpf1 = new SaisieCreerCalendrierWpf(this.Edite, this.db, null);
                this.wpf1.Show();
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            } 
        }


        /// <summary>
        /// Quand on ferme la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fermer(object sender, EventArgs e)
        {
            try
            {
                this.wpf1.Close();
                this.db.Dispose();
                Close();
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }
        }

        private void MettreAZeroLundi(object sender, EventArgs e)
        {
            try
            {
                saisieHelper.SetDayToZero(1);
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }
        }
        private void MettreAZeroMardi(object sender, EventArgs e)
        {
            try
            {
                saisieHelper.SetDayToZero(2);
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }  
        }
        private void MettreAZeroMercredi(object sender, EventArgs e)
        {
            try
            {
                saisieHelper.SetDayToZero(3);
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }  
        }
        private void MettreAZeroJeudi(object sender, EventArgs e)
        {
            try
            {
                saisieHelper.SetDayToZero(4);
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }  
        }
        private void MettreAZeroVendredi(object sender, EventArgs e)
        {
            try
            {
                saisieHelper.SetDayToZero(5);
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }  
        }
        private void MettreAZeroSamedi(object sender, EventArgs e)
        {
            try
            {
                saisieHelper.SetDayToZero(6);
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            }  
        }
        private void MettreAZeroDimanche(object sender, EventArgs e)
        {
            try
            {
                saisieHelper.SetDayToZero(7);
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "SaisieCreerWpf.xaml.cs");
                throw a;
            } 
        }  
    }
}



