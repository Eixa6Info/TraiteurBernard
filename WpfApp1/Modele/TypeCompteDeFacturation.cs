using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraiteurBernardWPF.Modele;
using System.Data.Entity;

namespace TraiteurBernardWPF.Modele
{
    public class TypeCompteDeFacturation
    {
        public int ID { get; set; }

        public string Nom { get; set; }

        public IList<Personne> Personnes { get; set; }

        public override string ToString()
        {
            return $"{Nom}";
        }
    }
}
