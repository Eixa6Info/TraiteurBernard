using System.Collections.Generic;
using System.Data.Entity;



namespace TraiteurBernardWPF.Modele
{
    public class Menu
    {

        public int ID { get; set; }

        public int Semaine { get; set; }

        public int Jour { get; set; }

        public HashSet<Plat> Plats { get; set; }

        public override string ToString()
        {
            return $"Semaine {Semaine} | Jour {Jour}";
        }

    }
}
