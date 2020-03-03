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
