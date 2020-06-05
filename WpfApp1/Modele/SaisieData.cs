using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace TraiteurBernardWPF.Modele
{
    public class SaisieData
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



        public int ID { get; set; }

        public int Type { get; set; }

        public int Quantite { get; set; }

        public string Libelle { get; set; }

        public bool Modifie { get; set; } = false;

        // Ajouter DELPRAT Bastien  04/06/2020
        public Saisie Saisie { get; set; }

    }
}
