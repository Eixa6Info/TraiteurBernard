using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.DAO
{
    class LivraisonDAO
    {
        internal static DateTime JourDeLivraisonCal(string tournee, int annee, int semaine, DateTime jourDeSaisie)
        {
            
            String jourStr = jourDeSaisie.DayOfWeek.ToString();
            String jourDeLivraisonStr = "";
            DateTime jourDeLivraison;
            if (tournee == "Marennes")
            {
                if (jourStr == "Monday" || jourStr == "Tuesday")
                {
                    jourDeLivraisonStr = "Samedi";
                    jourDeLivraison = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, jourDeLivraisonStr);
                }
                else if (jourStr == "Wednesday" || jourStr == "Thursday")
                {
                    jourDeLivraisonStr = "Mardi";
                    jourDeLivraison = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, jourDeLivraisonStr);
                }
                else
                {
                    jourDeLivraisonStr = "Jeudi";
                    jourDeLivraison = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, jourDeLivraisonStr);
                }
            }
            else if (tournee == "contre-tournée")
            {
                if (jourStr == "Wednesday" || jourStr == "Tuesday")
                {
                    jourDeLivraisonStr = "Lundi";
                    jourDeLivraison = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, jourDeLivraisonStr);
                }
                else if (jourStr == "Friday" || jourStr == "Thursday")
                {
                    jourDeLivraisonStr = "Mercredi";
                    jourDeLivraison = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, jourDeLivraisonStr);
                }
                else
                {
                    jourDeLivraisonStr = "Vendredi";
                    jourDeLivraison = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, jourDeLivraisonStr);
                }
            }
            else
            {
                if (jourStr == "Monday")
                {
                    jourDeLivraison = jourDeSaisie;
                }
                else if (jourStr == "Tuesday")
                {
                    jourDeLivraison = jourDeSaisie;
                }
                else if (jourStr == "Wednesday")
                {
                    jourDeLivraison = jourDeSaisie;
                }
                else if (jourStr == "Thursday")
                {
                    jourDeLivraison = jourDeSaisie;
                }
                else
                {
                    jourDeLivraisonStr = "Vendredi";
                    jourDeLivraison = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, jourDeLivraisonStr);
                }
            }

            
            return jourDeLivraison;
        }
    }
}
