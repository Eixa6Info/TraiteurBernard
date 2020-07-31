using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Gui;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.DAO
{
    class PersonneDAO
    {
       
        internal static List<Personne> GetPersonneMSA()
        {
            BaseContext db = new BaseContext();

            List<Personne> req = (from p in db.Personnes
                                   where p.MSA == true
                                   select p).ToList();
            db.Dispose();
            return req;
        }

        internal static List<Personne> GetPersonneAPA()
        {
            BaseContext db = new BaseContext();

            List<Personne> req = (from p in db.Personnes
                                  where p.APA == true
                                  select p).ToList();
            db.Dispose();
            return req;
        }

        internal static List<Personne> GetPersonneNotAPANotMSA()
        {
            BaseContext db = new BaseContext();

            List<Personne> req = (from p in db.Personnes
                                  where p.APA == false && p.MSA == false
                                  select p).ToList();
            db.Dispose();
            return req;
        }
        internal static List<Personne> GetPersonnesWithTourneeNotAPANotMSA(int tournee, BaseContext db)
        {
            List<Personne> req = (from p in db.Personnes
                                  where p.Tournee.ID == tournee && p.APA == false && p.MSA == false
                                  select p).ToList();
            return req;
        }

    }
}
