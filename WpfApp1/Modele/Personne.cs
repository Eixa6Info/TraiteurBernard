using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TraiteurBernardWPF.Modele;
using System.Data.Entity;

namespace TraiteurBernardWPF.Modele
{

    public class Personne
    {

        public int ID { get; set; }

        public String Nom { get; set; }


        public String Prenom { get; set; }


        public String Adresse { get; set; }


        public String Ville { get; set; }


        public String CodePostal { get; set; }


        public String ComplementAdresse { get; set; }


        public String Telephone { get; set; }


        public String Mail { get; set; }


        public String DateNaissance { get; set; }

        public String Civilite { get; set; }


        public String Comment { get; set; }

        public bool Actif { get; set; } = true;

        public bool MSA { get; set; }

        public bool APA { get; set; }

        public TypeCompteDeFacturation CompteDeFacturation { get; set; }

        public TypeTournee Tournee { get; set; }

        public string Couleur { get; set; }

        public ContactDurgence ContactDurgence { get; set; }

        public float APAMontantMax { get; set; }
        public float APALivraisonMax { get; set; }

        public float? MSAMontantMax { get; set; }
        public float? MSALivraisonMax { get; set; }
        public string APADateDebut { get; set; }
        public string APADateFin { get; set; }
        public string MSADateDebut { get; set; }
        public string MSADateFin { get; set; }
        public float? APALivraisonPrix { get; set; }
        public float? APATauxClient { get; set; }

        public override string ToString()
        {
            return $"{Nom} {Prenom} {Tournee}";
        }

    }
}
