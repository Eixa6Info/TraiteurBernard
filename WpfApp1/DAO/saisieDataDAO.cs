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

        /// <summary>
        /// Récupère l'intitulé par rapport a un ID par exemple l'ID 1 correspond à l'entrée du midi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static string GetIntituleFromId(int id)
        {
            switch (id)
            {

                case SaisieData.BAGUETTE:
                    return "Baguette";
                    break;;
                case SaisieData.POTAGE:
                    return "Potage"; 
                    break;
                case SaisieData.ENTREE_MIDI:
                    return "Entrée midi"; 
                    break;
                case SaisieData.PLAT_MIDI_1:
                    return "Plat midi 1";
                    break;
                case SaisieData.PLAT_MIDI_2:
                    return "Plat midi 2"; 
                    break;
                case SaisieData.PLAT_MIDI_3:
                    return "Plat midi 3"; 
                    break;
                case SaisieData.FROMAGE:
                    return "Fromage"; 
                    break;
                case SaisieData.DESSERT_MIDI:
                    return "Dessert midi"; 
                    break;
                case SaisieData.ENTREE_SOIR:
                    return "Entree"; 
                    break;
                case SaisieData.PLAT_SOIR_1:
                    return "Plat soir 1";
                    break;
                case SaisieData.DESSERT_SOIR:
                    return "Dessert soir"; 
                    break;
                default:
                    return "error";


            }
        }


    }
}
