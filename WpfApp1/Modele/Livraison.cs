using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;


namespace TraiteurBernardWPF.Modele
{
    public class Livraison
    {
        public int ID { get; set; }
        public string JourLivraison { get; set; }
        public string JourRepas1 { get; set; }
        public string JourRepas2 { get; set; }
        public string JourRepas3 { get; set; }

        public int TypeTourneeID { get; set; }
        public override string ToString()
        {
            return $"{JourLivraison} : {JourRepas1} &  {JourRepas2} &  {JourRepas3}";
        }
    }
}
