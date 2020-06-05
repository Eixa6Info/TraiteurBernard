using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GregorianCalendar = System.Globalization.GregorianCalendar;
using Calendar = System.Globalization.Calendar;

namespace TraiteurBernardWPF.Utils
{
    class GestionDeDateCalendrier
    {
        internal static string LeJourSuivantLeNuméro(int jour)
        {
            string resJour = "";
            if (jour == 1)
            {
                resJour = "Lundi";
            }
            else if (jour == 2)
            {
                resJour = "Mardi";
            }
            else if (jour == 3)
            {
                resJour = "Mercredi";
            }
            else if (jour == 4)
            {
                resJour = "Jeudi";
            }
            else if (jour == 5)
            {
                resJour = "Vendredi";
            }
            else if (jour == 6)
            {
                resJour = "Samedi";
            }
            else if (jour == 7)
            {
                resJour = "Dimanche";
            }

            return resJour;
        }

        internal static int TrouverLeMoisAvecNumSemaine(int semaine, int année)
        {
            int resMois;
            Calendar toMoisCalendar = new GregorianCalendar();

            int searchedWeek = semaine; // N° de semaien à rechercher
            DateTime dtFirstDayFirstWeek = DateTime.MinValue; // pour éviter erreur de compil
            for (int iWeek = 0, iDay = 1; iWeek != 1; iDay++)
            {
                dtFirstDayFirstWeek = new DateTime(année, 1, iDay);
                iWeek = toMoisCalendar.GetWeekOfYear(dtFirstDayFirstWeek, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }
            DateTime firstDayOfGivenWeek = toMoisCalendar.AddWeeks(dtFirstDayFirstWeek, searchedWeek - 1);
            resMois = firstDayOfGivenWeek.Month;

            return resMois;
        }

        internal static DateTime TrouverDateAvecNumJourEtNumSemaine(int year, int weekNumber, string dayWeek)
        {
            Dictionary<string, DayOfWeek> dictDays =
            new Dictionary<string, DayOfWeek>();

            dictDays.Add("dimanche", DayOfWeek.Sunday);
            dictDays.Add("lundi", DayOfWeek.Monday);
            dictDays.Add("mardi", DayOfWeek.Tuesday);
            dictDays.Add("mercredi", DayOfWeek.Wednesday);
            dictDays.Add("jeudi", DayOfWeek.Thursday);
            dictDays.Add("vendredi", DayOfWeek.Friday);
            dictDays.Add("samedi", DayOfWeek.Saturday);

            DateTime value = new DateTime(year, 1, 1).AddDays(7 * weekNumber);

            int daysToAdd = 0;
            DayOfWeek day;

            if (dictDays.TryGetValue(dayWeek.ToLower(), out day).Equals(DayOfWeek.Sunday))
            {
                daysToAdd = 7 - Convert.ToInt32(value.DayOfWeek);
            }
            else
            {
                daysToAdd = Convert.ToInt32(day) - Convert.ToInt32(value.DayOfWeek);
            }

            if (Convert.ToInt32(new DateTime(value.Year, 1, 1).DayOfWeek) < 5)
            {
                daysToAdd -= 7;
            }

            return value.AddDays(daysToAdd);

        }
    }
}
