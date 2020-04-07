using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.DAO
{
    class SaisieDAO
    {

        /// <summary>
        /// Récupérer toutes les saisies d'une année
        /// </summary>
        /// <param name="annee"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<Saisie> getAllFromYear(int annee, BaseContext db)
        {


            IQueryable<Saisie> req = from s in db.Saisies where s.Annee == annee select s;

            List<Saisie> saisies = new List<Saisie>();

            foreach(Saisie saisie in req)
            {
                db.Entry(saisie).Reference(s => s.Personne).Load();
                db.Entry(saisie).Reference(s => s.Tournee).Load();
                db.Entry(saisie).Collection(s => s.data).Load();

                saisies.Add(saisie);
            }
          


            return saisies;
        }


        /// <summary>
        /// Retourne les ID des saisies en fonction de l'année, de la semaine et de la personne
        /// l'ID sera 0 si il n'y a pas de saisie sur un jour donné
        /// </summary>
        /// <param name="anneee"></param>
        /// <param name="semaine"></param>
        /// <param name="personne"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static int[] getIdsFromYearWeekPersonne(int anneee, int semaine, Personne personne, BaseContext db)
        {
            IQueryable<Saisie> req = from s in db.Saisies where s.Personne == personne && s.Annee == anneee && s.Semaine == semaine select s;
            int[] tabId = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            List<Saisie> saisies = new List<Saisie>();
            foreach(Saisie s in req)
            {
                saisies.Add(s);
            }

            if (req.Any())
            {
                for (int i = 0; i < 7; i++)
                {
                    if (saisies[i] != null) tabId[i] = saisies[i].ID;
                }
            }

            return tabId;

        }

        /// <summary>
        /// Retourne toutes les saisies en fonction de l'anée, de la semaine et du jour
        /// </summary>
        /// <param name="anneee"></param>
        /// <param name="semaine"></param>
        /// <param name="jour"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<Saisie> getAllFromYearWeekDay(int anneee, int semaine, int jour, BaseContext db)
        {
            IQueryable<Saisie> req = from s in db.Saisies where s.Jour == jour && s.Annee == anneee && s.Semaine == semaine select s;

            List<Saisie> saisies = new List<Saisie>();
            foreach(Saisie saisie in req)
            {
                db.Entry(saisie).Collection(s => s.data).Load();
                saisies.Add(saisie);
            }



            return saisies;

        }

        /// <summary>
        /// Retourne toutes les saisies en fonction de l'anée, de la semaine et du jour
        /// </summary>
        /// <param name="anneee"></param>
        /// <param name="semaine"></param>
        /// <param name="jour"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<Saisie> getAllFromYearWeekDayForTournee(string tournee1, string tournee2, int anneee, int semaine, int jour, BaseContext db)
        {
            IQueryable<Saisie> req = from s in db.Saisies 
                                     where s.Jour == jour && s.Annee == anneee && s.Semaine == semaine && (s.Tournee.Nom == tournee1 || s.Tournee.Nom == tournee2)
                                     select s;

            List<Saisie> saisies = new List<Saisie>();
            foreach (Saisie saisie in req)
            {
                db.Entry(saisie).Collection(s => s.data).Load();
                saisies.Add(saisie);
            }



            return saisies;

        }

        /// <summary>
        /// Récupérer toutes les saisies en fonction de l'année et de la semaine
        /// </summary>
        /// <param name="annee"></param>
        /// <param name="semaine"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<Saisie> getAllFromYearWeek(int annee, int semaine, BaseContext db)
        {


            IQueryable<Saisie> req = from s in db.Saisies where s.Annee == annee && s.Semaine == semaine select s;

            List<Saisie> saisies = new List<Saisie>();

            foreach(Saisie saisie in req)
            {
                db.Entry(saisie).Reference(s => s.Personne).Load();
                db.Entry(saisie).Reference(s => s.Tournee).Load();
                db.Entry(saisie).Collection(s => s.data).Load();

                saisies.Add(saisie);
            }
          


            return saisies;
        }

        /// <summary>
        /// Récupérer toutes les saison en fonction d'une personne
        /// </summary>
        /// <param name="personne"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<Saisie> getAllFromPersonne(Personne personne, BaseContext db)
        {


            IQueryable<Saisie> req = from s in db.Saisies where s.Personne == personne select s;

            List<Saisie> saisies = new List<Saisie>();

            foreach(Saisie saisie in req)
            {
                db.Entry(saisie).Reference(s => s.Personne).Load();
                db.Entry(saisie).Reference(s => s.Tournee).Load();
                db.Entry(saisie).Collection(s => s.data).Load();

                saisies.Add(saisie);
            }
          


            return saisies;
        }
        
        /// <summary>
        /// Récupérer toutes les saisones en fonction d'une année, d'une semaine et d'une personne
        /// </summary>
        /// <param name="anneee"></param>
        /// <param name="semaine"></param>
        /// <param name="personne"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<Saisie> getAllFromYearWeekPersonne(int anneee, int semaine, Personne personne, BaseContext db)
        {
            IQueryable<Saisie> req = from s in db.Saisies where s.Personne == personne && s.Annee == anneee && s.Semaine == semaine select s;

            List<Saisie> saisies = new List<Saisie>();

            foreach(Saisie saisie in req)
            {
                db.Entry(saisie).Reference(s => s.Personne).Load();
                db.Entry(saisie).Reference(s => s.Tournee).Load();
                db.Entry(saisie).Collection(s => s.data).Load();

                saisies.Add(saisie);
            }
          


            return saisies;
        }
        
      


    }
}
