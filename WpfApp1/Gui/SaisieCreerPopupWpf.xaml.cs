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
using System.Windows.Shapes;
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
        public SaisieCreerPopupWpf()
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new Saisie();
            edition.DataContext = this.Edite;
        }

        /// <summary>
        /// On charge les tournées et les personnes pour les mettre dans des combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Chargement des tournées et assignation à la combobox
           IQueryable<TypeTournee> req1 = from t in this.db.TypeTournee
                                          select t;

            List<TypeTournee> data1 = new List<TypeTournee>();

            foreach (TypeTournee tt in req1)
            {
                this.db.Entry(tt).Collection(s => s.JoursLivraisonsRepas).Load();
                data1.Add(tt);
            }

            cbTournee.ItemsSource = data1;

            // Chargement des personnes et assignation à la combobox
            IQueryable<Personne> req2 = from t in this.db.Personnes
                                          select t;

            List<Personne> data2 = new List<Personne>();

            foreach (Personne p in req2)
            {
                this.db.Entry(p).Reference(s => s.Tournee).Load();
                this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                this.db.Entry(p).Reference(s => s.ContactDurgence).Load();
                data2.Add(p);
            }

            cbPersonne.ItemsSource = data2;
        }

        /// <summary>
        /// Verifier les données avant de valider l'opération
        /// </summary>
        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtSemaine.Text.Length != 0 && txtAnnee.Text.Length != 0 && cbTournee.SelectedItem != null && cbPersonne.SelectedItem != null)
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
            if (this.VerifierDonnees()){
                Close();
                SaisieCreerWpf wpf = new SaisieCreerWpf(this.Edite, this.db);
                WinFormWpf.CornerTopLeftToParent(wpf, this);
                wpf.ShowDialog();
            }
            else
            {
                MessageBox.Show("L'année, la semaine, la tournée et la personne sont indispensables",
                    "Informations indispensables",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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