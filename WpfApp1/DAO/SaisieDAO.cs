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
