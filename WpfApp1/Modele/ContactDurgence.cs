using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TraiteurBernardWPF.Modele
{

    public class ContactDurgence
    {
        public int ID { get; set; }

        public String Nom { get; set; }

        public String Prenom { get; set; }

        public String Telephone { get; set; }

        public override string ToString()
        {
            return $"{Prenom} {Nom} {Telephone}";
        }
    }
}