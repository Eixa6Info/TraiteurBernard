using System;

namespace TraiteurBernardWPF.Modele
{
    public class Plat
    {

        public const int ENTREE_MIDI = 1;
        public const int PLAT_MIDI_1 = 2;
        public const int PLAT_MIDI_2 = 3;
        public const int PLAT_MIDI_3 = 4;
        public const int DESSERT_MIDI = 5;

        public const int ENTREE_SOIR = 6;
        public const int PLAT_SOIR_1 = 7;
        public const int DESSERT_SOIR = 8;



        public int ID { get; set; }


        public int Type { get; set; }

        public String Name { get; set; }
    }
}
