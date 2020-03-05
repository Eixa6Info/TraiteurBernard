using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.DAO
{
    class SaisieDataDAO
    {

        /// <summary>
        /// Récupère toutes les saisies data avec le type passé en argument 
        /// exemple si on veut toutes les entrées du midi, on passe l'argument 1 et 
        /// ca retournera toutes les saises data de l'entrée du midi
        /// </summary>
        /// <param name="type"></param>
        /// <param name="saisieDatas"></param>
        /// <returns></returns>
        internal static List<SaisieData> SortByTypeFromList(int type, List<SaisieData> saisieDatas)
        {


            var req = from sd in saisieDatas where sd.Type == type select sd;

            List < SaisieData > returnList = new List<SaisieData>();

            foreach(SaisieData saisie in req)
            {

                returnList.Add(saisie);
            }
          


            return returnList;
        }




    }
}
