﻿
using System.Collections.Generic;


namespace TraiteurBernardWPF.Modele
{
    public class Menu
    {

        public int ID { get; set; }

        public int Semaine { get; set; }

        public int Jour { get; set; }


        public HashSet<Plat> Plats { get; set; }


    }
}