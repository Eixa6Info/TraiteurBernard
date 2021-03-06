﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Security;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour MenuCreerWpf.xaml
    /// </summary>
    public partial class MenuCreerWpf : Window
    {

        private BaseContext db;
        private bool modif = false;

        // La classe Menu existe déjà donc on utilise le namespace pour distinguer
        // notre classe métier de la classe déjà existante
        private TraiteurBernardWPF.Modele.Menu Edite;

        /// <summary>
        /// Constructeur sans paramètres donc sans dépendance donc création 
        /// </summary>
        public MenuCreerWpf()
        {
            InitializeComponent();
            Title += "Création d'un menu";
            this.db = new BaseContext();
            this.Edite = new TraiteurBernardWPF.Modele.Menu { Plats = new HashSet<Plat>(), Jour = 1, Semaine = 1};
            edition.DataContext = this.Edite;     
        }
        
        /// <summary>
        /// Constructeur avec paramètres pour modification
        /// </summary>
        public MenuCreerWpf(TraiteurBernardWPF.Modele.Menu edite, BaseContext db, bool modifie)
        {
            InitializeComponent();
            Title += "Modification d'un menu";
            this.db = db;
            this.Edite = edite;
            edition.DataContext = this.Edite;
            this.modif = modifie;
            this.BinderLesDonnees();
            
        }

        /// <summary>
        /// Fonction pour assigner les données aux textbox (dans le cas d'une mofication)
        /// </summary>
        private void BinderLesDonnees()
        {
            // Tableau qui représente les 8 plats hypothétique du jour
            Plat[] tabPlats = new Plat[8];

            // Conversion de l'hashset des plats en tableau puis copie dans le tableau précédent
            this.Edite.Plats.CopyTo(tabPlats, 0);

            // Trie du tableau par ordre des plat
            tabPlats = tabPlats.OrderBy(p => p != null ? p.Type : 9).ToArray();

            txtEntreeMidi.Text = tabPlats[0] != null ? tabPlats[0].Name : "";
            txtPlat1Midi.Text = tabPlats[1] != null ? tabPlats[1].Name : "";
            txtPlat2Midi.Text = tabPlats[2] != null ? tabPlats[2].Name : "";
            txtPlat3Midi.Text = tabPlats[3] != null ? tabPlats[3].Name : "";
            txtDessertMidi.Text = tabPlats[4] != null ? tabPlats[4].Name : "";
            txtEntreeSoir.Text = tabPlats[5] != null ? tabPlats[5].Name : "";
            txtPlatSoir.Text = tabPlats[6] != null ? tabPlats[6].Name : "";
            txtDessertSoir.Text = tabPlats[7] != null ? tabPlats[7].Name : "";
           
            if (modif == true)
            {
                this.txtNumJour.IsEnabled = false;
                this.txtNumSemaine.IsEnabled = false;
                this.btnValiderEtSuivant.Visibility = Visibility.Hidden;
            }

        }
        /// <summary>
        /// Fonction de vérification des données avant création / édition
        /// </summary>
        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtNumSemaine.Text.Length != 0 && txtNumJour.Text.Length != 0)
            {
                retval = true;
            }

            return retval;
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
        /// Ajouter UN plat dans la hashset des plats
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="type"></param>
        private void AjouterPlat(TextBox txt, int type)
        {
            if (!String.IsNullOrEmpty(txt.Text)) this.Edite.Plats.Add(new Plat { Type = type, Name = txt.Text });
            else this.Edite.Plats.Add(new Plat { Type = type, Name = "" });
        }

        /// <summary>
        /// Ajouter DES plats dans la hastset des plats
        /// </summary>
        /// <param name="txt_type"></param>
        private void AjouterPlat(Dictionary<TextBox, int> txt_type)
        {
            foreach (KeyValuePair<TextBox, int> pair in txt_type)
                if (!String.IsNullOrEmpty(pair.Key.Text)) this.Edite.Plats.Add(new Plat { Type = pair.Value, Name = pair.Key.Text });
                else this.Edite.Plats.Add(new Plat { Type = pair.Value, Name = "" });
        }

        /// <summary>
        /// Valider les changements / la création
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            // On reintialise les plats pour ne pas faire de doublons lors de l'ajout / modification
            // des plats sur le formulaire
            this.Edite.Plats = new HashSet<Plat>();

            Dictionary<TextBox, int> txt_type = new Dictionary<TextBox, int>
            {
                { txtEntreeMidi, Plat.ENTREE_MIDI},
                { txtPlat1Midi, Plat.PLAT_MIDI_1},
                { txtPlat2Midi, Plat.PLAT_MIDI_2},
                { txtPlat3Midi, Plat.PLAT_MIDI_3},
                { txtDessertMidi, Plat.DESSERT_MIDI},
                { txtEntreeSoir, Plat.ENTREE_SOIR},
                { txtPlatSoir, Plat.PLAT_SOIR_1},
                { txtDessertSoir, Plat.DESSERT_SOIR},
            };

            this.AjouterPlat(txt_type);

            // Quelles données sont obligatoires ici ?? demander à Fabien
            if (VerifierDonnees())
            {
                if (this.Edite.ID == 0) this.db.Add(this.Edite);
                this.db.SaveChanges();
                Close();    
            }
            else
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensable, Properties.Resources.MessagePopUpErrorIndispensable2, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
            }
                

        }


        private void ValiderEtSuivant(object sender, RoutedEventArgs e)
        {
            // On reintialise les plats pour ne pas faire de doublons lors de l'ajout / modification
            // des plats sur le formulaire
            this.Edite.Plats = new HashSet<Plat>();

            Dictionary<TextBox, int> txt_type = new Dictionary<TextBox, int>
            {
                { txtEntreeMidi, Plat.ENTREE_MIDI},
                { txtPlat1Midi, Plat.PLAT_MIDI_1},
                { txtPlat2Midi, Plat.PLAT_MIDI_2},
                { txtPlat3Midi, Plat.PLAT_MIDI_3},
                { txtDessertMidi, Plat.DESSERT_MIDI},
                { txtEntreeSoir, Plat.ENTREE_SOIR},
                { txtPlatSoir, Plat.PLAT_SOIR_1},
                { txtDessertSoir, Plat.DESSERT_SOIR},
            };

            this.AjouterPlat(txt_type);

            // Quelles données sont obligatoires ici ?? demander à Fabien
            if (VerifierDonnees())
            {
                int semaineId = 0;
                int jourId = 0;

                if (this.Edite.ID == 0) this.db.Add(this.Edite);
                this.db.SaveChanges();
                semaineId = short.Parse(txtNumSemaine.Text);
                jourId = short.Parse(txtNumJour.Text);

                if (jourId > 6)
                {
                    jourId = 1;
                    semaineId = semaineId + 1;
                }
                else
                {
                    jourId = jourId + 1;
                }

                InitializeComponent();
                Title += "Création d'un menu";
                this.db = new BaseContext();
                this.Edite = new TraiteurBernardWPF.Modele.Menu { Plats = new HashSet<Plat>(), Jour= jourId , Semaine= semaineId};
                edition.DataContext = this.Edite;
            }
            else
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensable, Properties.Resources.MessagePopUpErrorIndispensable2, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
            }


        }

        /// <summary>
        /// Fonction qui sert à verifier si la semaine et le jour indiqués appartiennent déjà a un menu.
        /// Si c'est le cas, on bind toutes les valeurs du menu déjà existant en offrant la possibilité
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChargerSiExistant(object sender, TextChangedEventArgs e)
        {
            int semaineId = 0;
            int jourId = 0;
            if(!String.IsNullOrEmpty(txtNumJour.Text) && !String.IsNullOrEmpty(txtNumSemaine.Text))
            {
                semaineId = short.Parse(txtNumSemaine.Text);

                jourId = short.Parse(txtNumJour.Text);
            }


            TraiteurBernardWPF.Modele.Menu menu = MenuDao.getFirstFromWeekAndDay(semaineId, jourId, this.db);

            if (menu != null)
            {
                //this.db.Entry(menu).Collection(m => m.Plats).Load();
                this.Edite = menu;
            }
            else
            {
                this.Edite = new TraiteurBernardWPF.Modele.Menu
                {
                    Jour = jourId,
                    Semaine = semaineId,
                    Plats = new HashSet<Plat>()
                };
            }

            this.BinderLesDonnees();
        }

        /// <summary>
        /// Verifier que la saisie est bien un nombre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifierFormatSemaine(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);
        }

        /// <summary>
        ///  Verifier que la saisie est bien un nombre 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifierFormatJour(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
