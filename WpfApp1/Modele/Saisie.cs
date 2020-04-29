using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace TraiteurBernardWPF.Modele
{
    public class Saisie
    {

        public int ID { get; set; }
        public int Jour { get; set; }
        public int Semaine { get; set; }
        public int Annee { get; set; }
        public TypeTournee Tournee { get; set; }
        public Personne Personne { get; set; }
        public HashSet<SaisieData> data { get; set; }
    }
}
