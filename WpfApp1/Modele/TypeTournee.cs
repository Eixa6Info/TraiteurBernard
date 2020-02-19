using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TraiteurBernardWPF.Modele
{
    public class TypeTournee
    {
        public int ID { get; set; }
        public string Nom { get; set; }

        // Exemple : 4 types de tournée
        // Tournée : <lundi,[lundi]>, ..., <samedi,[samedi, dimanche]>
        // ContreTournée : <lundi,[mardi, mercredi]>, ...
        // Marennes
        // Saujon

    
        public IList<Livraison> JoursLivraisonsRepas { get; set; }

     
        public override string ToString()
        {
            return $"{Nom}";
        }
    }
}
