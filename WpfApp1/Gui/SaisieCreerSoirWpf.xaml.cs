﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieCreerWpf.xaml
    /// </summary>
    public partial class SaisieCreerSoirWpf : Window
    {
        private Saisie Edite { get; set; }

        private int colonneDepart = 1;
        private int ligneDepart = 3;


        private int[] types = new int[3] 
        {
           SaisieData.ENTREE_SOIR,
           SaisieData.PLAT_SOIR_1,
           SaisieData.DESSERT_SOIR
        };

        private int[] typesBis = new int[3] 
        {
           Plat.ENTREE_SOIR,
           Plat.PLAT_SOIR_1,
           Plat.DESSERT_SOIR
        };

        private BaseContext db;

        private SaisieHelper saisieHelper;


        /// <summary>
        /// Constructeur avec en paramètre la saisie qui contient la semaine, le jour, la tournée, l'année et la personne
        /// </summary>
        /// <param name="edite"></param>
        public SaisieCreerSoirWpf(Saisie edite, int[] IDs, BaseContext db)
        {
            if (edite == null) throw new ArgumentNullException(nameof(edite));

            InitializeComponent();

            this.Edite = edite;
            this.db = db;

            this.saisieHelper = new SaisieHelper(this.gridMain, this.types, this.typesBis, this.Edite, 1, 3, this.db);

            //saisieCreerWpfTest.GenerateControls();
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

            Close();

        }
        /// <summary>
        /// Au chargement de la page, on charge les menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.db.Dispose();
            Close();
        }
    }
}
        


