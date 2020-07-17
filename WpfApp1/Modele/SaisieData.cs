using com.sun.org.apache.xpath.@internal.functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using TraiteurBernardWPF.DAO;

namespace TraiteurBernardWPF.Modele
{
    public class SaisieData : INotifyPropertyChanged
    {
        public const int BAGUETTE = -1;
        public const int POTAGE = 0;
        public const int ENTREE_MIDI = 1;
        public const int PLAT_MIDI_1 = 2;
        public const int PLAT_MIDI_2 = 3;
        public const int PLAT_MIDI_3 = 4;
        public const int FROMAGE = 5;
        public const int DESSERT_MIDI = 6;

        public const int ENTREE_SOIR = 7;
        public const int PLAT_SOIR_1 = 8;
        public const int DESSERT_SOIR = 9;




        public int ID
        {
            get;
            set;
        }

        private String CommentValue;
        [NotMapped]
        public String Comment
        {
            get
            {
                return this.CommentValue;
            }
            set
            {
                if (value != this.CommentValue)
                {
                    this.CommentValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int typeValue;
        public int Type
        {
            get
            {
                return this.typeValue;
            }
            set
            {
                if (value != this.typeValue)
                {
                    this.typeValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int quantiteValue;
        public int Quantite
        {
            get
            {
                return this.quantiteValue;
            }
            set
            {
                if (value != this.quantiteValue)
                {
                    this.quantiteValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string libelleValue;
        public string Libelle
        {
            get
            {
                return this.libelleValue;
            }
            set
            {
                if (value != this.libelleValue)
                {
                    this.libelleValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool modifieValue;
        public bool Modifie
        {
            get
            {
                return this.modifieValue;
            }
            set
            {
                if (value != this.modifieValue)
                {
                    this.modifieValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Ajout DELPRAT Bastien  04/06/2020
        private Saisie saisieValue;
        public Saisie Saisie
        {
            get
            {
                return this.saisieValue;
            }
            set
            {
                if (value != this.saisieValue)
                {
                    this.saisieValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool sauceValue;

        public bool Sauce
        {
            get
            {
                return this.sauceValue;
            }
            set
            {
                if (value != this.sauceValue)
                {
                    this.sauceValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool mixeValue;

        public bool Mixe
        {
            get
            {
                return this.mixeValue;
            }
            set
            {
                if (value != this.mixeValue)
                {
                    this.mixeValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool natureValue;

        public bool Nature
        {
            get
            {
                return this.natureValue;
            }
            set
            {
                if (value != this.natureValue)
                {
                    this.natureValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool changementSoir = false;

        private bool forceChange = false;

        public event PropertyChangedEventHandler PropertyChanged;


        private void ResetLibelleWithDefaultValue(int defaultValueType = int.MinValue)
        {
            bool isCustomDefaultValueType = false;

            if (defaultValueType == int.MinValue) defaultValueType = this.Type;
            else isCustomDefaultValueType = true;

            string defaultLibelle = GetDefaultValue(defaultValueType, isCustomDefaultValueType);

            if (this.Libelle != defaultLibelle) (this.forceChange, this.Libelle) = (true, defaultLibelle);

        }

        private string GetDefaultValue(int type, bool customType = false)
        {
            switch (this.Type)
            {
                case BAGUETTE:
                case POTAGE:
                case ENTREE_MIDI:
                case PLAT_MIDI_1:
                case PLAT_MIDI_2:
                case PLAT_MIDI_3:
                    return MenuDao.GetPlatFromTypeWeekAndDay(type, this.Saisie.Semaine, this.Saisie.Jour);
                case FROMAGE:
                    return "Fromage";
                case DESSERT_MIDI:
                case ENTREE_SOIR:
                case PLAT_SOIR_1:
                case DESSERT_SOIR:
                    // Si c'est pas un type customisé alors c'est les types de bases, on fait donc (-1) pour eviter le décalage des ID
                    return MenuDao.GetPlatFromTypeWeekAndDay(customType ? type : type - 1, this.Saisie.Semaine, this.Saisie.Jour);
                default:
                    return "erreur";
            }
        }


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

                Console.WriteLine(propertyName + " vient de changer | Libelle : " + this.Libelle + " | Modifie : " + this.Modifie + " | Quantite : " + this.Quantite + " | Mixé : " + this.Mixe + " | Avec saucé " + this.Sauce + " | Nature : " + this.Nature);

                switch (propertyName)
                {
                    // Le libelle change (on ne sait pas par qui, l'utilisateur ou le code ?) c'est pour ça qu'on utilise
                    // le flag 'forceHange' pour savoir si la modification vient du code ou de l'utilisateur
                    case "Libelle":

                        // Si on écrit un mot clé pour reset, on reset
                        if (new string[] { "reset", "0" }.Contains(this.Libelle))
                        {
                            // Si la quantité vaut 0 on reset juste le libelle
                            if (this.Quantite == 0) ResetLibelleWithDefaultValue();
                            // Si non on met la quantité à 0 ce qui va egalement reset le libelle
                            else this.Quantite = 0;
                            break;
                        }

                        // Si le flag forceChange est utilisé ça veut dire que c'est un modification par le code donc on
                        // met le flag Modifie à false
                        if (this.forceChange)
                        {
                            this.Modifie = this.forceChange = false;
                            break;
                        }

                        // Si le libelle est égale au plat normal, on met le flag Modifie à false
                        if (this.Libelle == GetDefaultValue(this.Type))
                        {
                            this.Modifie = false;
                            break;
                        }

                        // Si la quantité est >= 10, ça veut dire qu'on est sur un plat du soir et qu'on essaye de changer la valeur,
                        // on met donc la quantité à 1 et le flag Modifie à true
                        if (this.Quantite >= 10)
                        {
                            (this.Quantite, this.Modifie) = (1, true);
                            break;
                        }

                        // Si la quantité est 0 on la met à 1
                        if (this.Quantite == 0) this.Quantite = 1;

                        // Si on change le type de baguette on laisse le flag Modifie à false
                        if (this.Type == BAGUETTE)
                        {
                            this.Modifie = false;
                            break;
                        }

                        this.Modifie = true;
                        
                        break;

                    // La quantité change
                    case "Quantite":

                        // Si la quantité est mise à zéro, on remet le plat d'origine
                        if (this.Quantite == 0)
                        {
                            ResetLibelleWithDefaultValue();
                            (this.Nature, this.Mixe, this.Sauce) = (false, false, false);
                            
                            break;
                        }

                        // Si la quantité est plus grande ou égale à 10 (option spécial soir)
                        if (this.Quantite >= 10)
                        {
                            // On regarde le type de plat concerné
                            switch (this.Type)
                            {
                                // Dans le cas du potage et de l'entree du midi, on affiche l'entrée du soir dans le libelle
                                case POTAGE:
                                case ENTREE_MIDI:
                                    (this.forceChange, this.Libelle) = (true, MenuDao.GetPlatFromTypeWeekAndDay(Plat.ENTREE_SOIR, this.Saisie.Semaine, this.Saisie.Jour));
                                    break;
                                // Dans le cas du dessert du midi on affiche le dessert du soir dans le libelle
                                case DESSERT_MIDI:
                                    (this.forceChange, this.Libelle) = (true, MenuDao.GetPlatFromTypeWeekAndDay(Plat.DESSERT_SOIR, this.Saisie.Semaine, this.Saisie.Jour));
                                    break;
                            }
                        }

                        // Si la quantité est inférieur à 10 (selection normale)
                        if (this.Quantite < 10)
                        {
                            // On regarde le type de plat concerné
                            switch (this.Type)
                            {
                                // Sur le potage on remet le plat d'o
                                case POTAGE:
                                case ENTREE_MIDI:

                                    // Si de l'option du soir on revient à une option normale, on reprend le menu d'avant
                                    if (this.Libelle == MenuDao.GetPlatFromTypeWeekAndDay(Plat.ENTREE_SOIR, this.Saisie.Semaine, this.Saisie.Jour))
                                    {
                                        this.forceChange = true;
                                        // On peut passer this.Type en paramètre car sur POTAGE et ENTREE_MIDI il n'y a pas de décalage d'ID
                                        this.Libelle = MenuDao.GetPlatFromTypeWeekAndDay(this.Type, this.Saisie.Semaine, this.Saisie.Jour);
                                    }

                                    break;

                                case DESSERT_MIDI:
                                    if (this.Libelle == MenuDao.GetPlatFromTypeWeekAndDay(Plat.DESSERT_SOIR, this.Saisie.Semaine, this.Saisie.Jour))
                                    {
                                        this.forceChange = true;
                                        // On doit passer this.Type - 1 car il y a un décalage après le plat 3 du midi
                                        this.Libelle = MenuDao.GetPlatFromTypeWeekAndDay(this.Type - 1, this.Saisie.Semaine, this.Saisie.Jour);
                                    }
                                    break;
                            }

                        }
                        break;
                }

            }
        }

    }
}
