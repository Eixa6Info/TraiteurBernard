﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Utils
{
    class SaisieHelper
    {
        private BaseContext db;
        private Saisie Edite
        {
            get;
            set;
        }

        public static Personne per;
        public static int semaine;

        private List<Saisie> saisieList;

        private List<ComboData> quantiteAvecSoirCombobox; // ComboBox pour le potage et l'entrée

        public List<ComboData> quantiteSansSoirCombobox; // ComboBox normales

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


        private int[] types;

        private int[] typesBis;

        int colonneDepart;
        int ligneDepart;

        private Grid grid;
        public SaisieHelper(Grid grid, int[] types, int[] typesBis, Saisie edite, int colonneDepart, int ligneDepart, BaseContext db)
        {
            this.grid = grid;
            this.types = types;
            this.typesBis = typesBis;
            this.Edite = edite;
            this.db = db;

            this.colonneDepart = colonneDepart;
            this.ligneDepart = ligneDepart;
            this.PopulateComboDataLists();
            this.PopulateSaisieList();
            this.GenerateControls();


        }




        /// <summary>
        /// Remplit les listes d'objets ComboData
        /// </summary>
        public void PopulateComboDataLists()
        {

            List<ComboData> temp = new List<ComboData>();

            int[] ids = new int[] { 0, 1, 2, 3, 4, 5, 10, 20, 30 };
            string[] values = new string[] { "0", "1", "2", "3", "4", "5", "Soir 1", "Soir 2", "Soir 3" };

            if (ids.Length == values.Length)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    temp.Add(new ComboData
                    {
                        Id = ids[i],
                        Value = values[i]
                    });
                }
            }

            // ComboBox avec soir, on prend tout
            quantiteAvecSoirCombobox = temp.Where(t => t.Id >= 0).ToList();

            // ComboBox sans soir, on prend juste les ID inférieur à 10
            quantiteSansSoirCombobox = temp.Where(t => t.Id < 10).ToList();
        }

        /// <summary>
        /// On ajoute les saisiedata qui correspondent au tableau types
        /// </summary>
        /// <param name="saisie"></param>
        private void PopulateSaiseData(Saisie saisie)
        {
            // Pour tous les types
            for (int i = 0; i < this.types.Length; i++)
            {
                string libelle = MenuDao.GetPlatFromTypeWeekAndDay(this.typesBis[i], saisie.Semaine, saisie.Jour);
                if (this.types[i] == SaisieData.FROMAGE) libelle = "Fromage";
                SaisieData saisieData = new SaisieData
                {
                    Libelle = libelle,
                    Modifie = false,
                    Quantite = new int[] { SaisieData.PLAT_MIDI_1, SaisieData.PLAT_MIDI_2, SaisieData.PLAT_MIDI_3 }.Contains(this.types[i]) ? 0 : 1,
                    Sauce = false,
                    Mixe = false,
                    Nature = false,
                    Saisie = saisie,
                    Type = this.types[i]
                };
                saisie.data.Add(saisieData);
            }
        }

        /// <summary>
        /// On récupère les saisies si elles existent, sinon on les créait
        /// </summary>
        private void PopulateSaisieList()
        {
            this.saisieList = new List<Saisie>();

            List<Saisie> saisieList = SaisieDAO.getAllFromYearWeekPersonne(this.Edite.Annee, this.Edite.Semaine, this.Edite.Personne, this.db);

            // Si on a déjà des saisies d'enregistrer on verifie que les saisie contiennent bien les données voulues
            if (saisieList.Any())
            {
                // On regarde si les types sont contenues dans les données des saisies (si une saisie du soir a déjà ete créé par ex)
                // Pour toutes les saisies
                foreach (Saisie saisie in saisieList)
                    // On regarde si ID dans 'types' sont présents dans la saisiedata
                    if (this.types.Intersect(saisie.data.Select(sd => sd.Type).ToArray()).ToArray().Length != this.types.Length)
                        this.PopulateSaiseData(saisie);

                this.saisieList = saisieList;
            }
            // Si non on génère des saisies avec les plats du menu
            else
                // Pour chaque jours on créait un nouvel objet Saisie
                for (int i = 1; i < 8; i++)
                {
                    Saisie saisie = new Saisie
                    {
                        Annee = this.Edite.Annee,
                        Jour = i,
                        Personne = this.Edite.Personne,
                        Semaine = this.Edite.Semaine,
                        Tournee = this.Edite.Tournee,
                        data = new HashSet<SaisieData>()
                    };

                    this.PopulateSaiseData(saisie);
                    this.saisieList.Add(saisie);
                }
        }

        /// <summary>
        /// Génerer tous les controles WPF
        /// </summary>
        public void GenerateControls()
        {
            int ligne = 0 + this.ligneDepart, colonne = 0 + this.colonneDepart, jour = 0;

            // Pour toutes les saisies
            foreach (Saisie saisie in this.saisieList)
            {
                ++jour;
                // Pour tous les types (de saisiedata)
                for (int i = 0; i < this.types.Length; i++)
                {
                    // On récupère la saisiedata associée au type
                    SaisieData saisieData = saisie.data.First(s => s.Type == this.types[i]);

                    // ##### GENERATION DES COMBOBOXS POUR LA QUANTITE
                    ComboBox comboBox = new ComboBox
                    {
                        Width = 25,
                        Height = 30,
                        Margin = new Thickness(0, 5, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        DisplayMemberPath = "Value",
                        SelectedValuePath = "Id",
                    };

                    // Si le type est potage ou entree midi ou dessert midi on met les combobox avec option soir
                    if (new int[] { SaisieData.POTAGE, SaisieData.ENTREE_MIDI, SaisieData.DESSERT_MIDI }.Contains(this.types[i]))
                        comboBox.ItemsSource = quantiteAvecSoirCombobox;
                    // Si non on met les combobox normals
                    else
                        comboBox.ItemsSource = quantiteSansSoirCombobox;

                    // On positionne la combobox dans la grille
                    comboBox.SetValue(Grid.ColumnProperty, colonne);
                    comboBox.SetValue(Grid.RowProperty, ligne);

                    // On lie la valeur séléctionné à la propriété 'Quantite' de la saisiedata courante
                    comboBox.SetBinding(ComboBox.SelectedValueProperty, new Binding("Quantite")
                    {
                        Source = saisieData,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });

                    // Ajout de la combobox à la grille
                    this.grid.Children.Add(comboBox);

                    if (new int[] { SaisieData.ENTREE_MIDI, SaisieData.ENTREE_SOIR }.Contains(this.types[i]))
                    {
                        // ##### GENERATION DES CHECKBOX POUR L'OPTION NATURE
                        CheckBox checkBox1 = new CheckBox
                        {
                            Width = 25,
                            Height = 30,
                            Margin = new Thickness(0, 5, 5, 0),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Content = "",
                            Background = Brushes.CadetBlue,
                            ToolTip = "Nature"
                        };


                        // On positionne la combobox dans la grille
                        checkBox1.SetValue(Grid.ColumnProperty, colonne);
                        checkBox1.SetValue(Grid.RowProperty, ligne);

                        // On lie la valeur de la checkbox à la propriété (le flag) 'Nature'
                        checkBox1.SetBinding(CheckBox.IsCheckedProperty, new Binding("Nature")
                        {
                            Source = saisieData,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });

                        // Ajout de la combobox à la grille
                        this.grid.Children.Add(checkBox1);
                    }

                    // Si on est sur un plat du midi on met les checkboxs option sauce et option mixé
                    if (new int[] { SaisieData.PLAT_MIDI_1, SaisieData.PLAT_MIDI_2, SaisieData.PLAT_MIDI_3, SaisieData.PLAT_SOIR_1 }.Contains(this.types[i]))
                    {
                        // ##### GENERATION DES CHECKBOX POUR L'OPTION SANS SAUCE
                        CheckBox checkBox1 = new CheckBox
                        {
                            Width = 25,
                            Height = 30,
                            Margin = new Thickness(0, 5, 5, 0),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Content = "",
                            Background = Brushes.DarkOrchid,
                            ToolTip = "Sans sauce"
                        };


                        // On positionne la combobox dans la grille
                        checkBox1.SetValue(Grid.ColumnProperty, colonne);
                        checkBox1.SetValue(Grid.RowProperty, ligne);

                        // On lie la valeur de la checkbox à la propriété (le flag) 'Sauce'
                        checkBox1.SetBinding(CheckBox.IsCheckedProperty, new Binding("Sauce")
                        {
                            Source = saisieData,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });

                        // Ajout de la combobox à la grille
                        this.grid.Children.Add(checkBox1);


                        // ##### GENERATION DES CHECKBOX POUR L'OPTION MIXE
                        CheckBox checkBox2 = new CheckBox
                        {
                            Width = 25,
                            Height = 30,
                            Margin = new Thickness(0, 5, 5, 0),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Content = "",
                            Background = Brushes.DarkOrange,
                            ToolTip = "Mixer le repas"
                        };

                        // On positionne la combobox dans la grille
                        checkBox2.SetValue(Grid.ColumnProperty, colonne);
                        checkBox2.SetValue(Grid.RowProperty, ligne);

                        // On lie la valeur séléctionné à la propriété 'Quantite' de la saisiedata courante
                        checkBox2.SetBinding(CheckBox.IsCheckedProperty, new Binding("Mixe")
                        {
                            Source = saisieData,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });

                        // Ajout de la combobox à la grille
                        this.grid.Children.Add(checkBox2);

                    }


                    // ##### GENERATION DES TEXTBOXS POUR LE LIBELLE
                    TextBox textBox = new TextBox()
                    {
                        Width = 105,
                        Height = 80,
                        Margin = new Thickness(5, 5, 15, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        TextWrapping = TextWrapping.Wrap,
                        IsTabStop = false
                    };

                    // On positionne la textbox dans la grille
                    textBox.SetValue(Grid.ColumnProperty, colonne);
                    textBox.SetValue(Grid.RowProperty, ligne);

                    textBox.SetBinding(TextBox.TextProperty, new Binding("Libelle")
                    {
                        Source = saisieData,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });

                    // On créait un multibinding pour envoyer des informations au converter qui va s'occuper de générer
                    // une couleur on fonction des propriétés de la saisiedata
                    MultiBinding multiBindingBackground = new MultiBinding();
                    multiBindingBackground.Converter = new RenameConverter();

                    // On bind le flag 'Modifie' au converter
                    Binding bindingBackgroundModifie = new Binding("Modifie")
                    {
                        Source = saisieData,
                        Mode = BindingMode.OneWay
                    };

                    // On bind la propriété 'Quantite' au converter
                    Binding bindingBackgroundQuantite = new Binding("Quantite")
                    {
                        Source = saisieData,
                        Mode = BindingMode.OneWay
                    };

                    // On ajoute les bindings au mutlbinding
                    multiBindingBackground.Bindings.Add(bindingBackgroundModifie);
                    multiBindingBackground.Bindings.Add(bindingBackgroundQuantite);
                    textBox.SetBinding(TextBox.BackgroundProperty, multiBindingBackground);


                    // Ajout de la combobox à la grille
                    this.grid.Children.Add(textBox);

                    ligne++;

                }
                colonne++;
                ligne = 0 + this.ligneDepart;
            }
        }

        /// <summary>
        /// Enre
        /// </summary>
        public void Save()
        {


            foreach (Saisie saisie in this.saisieList)
            {
                // this.db.Saisies.Attach(saisie);

                if (saisie.ID == 0) this.db.Add(saisie);
            }
            this.db.SaveChanges();
        }
    }
}
