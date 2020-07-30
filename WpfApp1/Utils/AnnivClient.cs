using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Utils
{
    class AnnivClient
    {
        BaseContext db = new BaseContext();
        public bool AnnvClientSaisie(Personne personne, int annee, int semaine, int jour)
        {
            string iDate = personne.DateNaissance;
            string[] Date = iDate.Split('/');
            int i = 0;
            int day = 0;
            int month = 0;
            String strJour = GestionDeDateCalendrier.LeJourSuivantLeNuméro(jour);
            DateTime dateTime = GestionDeDateCalendrier.TrouverDateAvecNumJourEtNumSemaine(annee, semaine, strJour);
           
            foreach (var d in Date)
            {
                if (i == 0)
                {
                    month = int.Parse(d);
                }
                else if (i == 1)
                {
                    day = int.Parse(d);
                }
                i++;
            }

            if (day == dateTime.Day && month == dateTime.Month)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public List<Personne> AnnvClientAll()
        {

            IQueryable<Personne> reqPersonne = from p in db.Personnes select p;

            List<Personne> data = new List<Personne>();

            foreach(Personne p in reqPersonne)
            {
                string iDate = p.DateNaissance;
                if (iDate != null)
                {
                    string[] Date = iDate.Split('/');
                    int i = 0;
                    int day = 0;
                    int month = 0;
                    foreach (var d in Date)
                    {
                        if (i == 0)
                        {
                            month = int.Parse(d);
                        }
                        else if (i == 1)
                        {
                            day = int.Parse(d);
                        }
                        i++;
                    }
                    if (day == DateTime.Now.Day && month == DateTime.Now.Month)
                    {
                        data.Add(p);
                        data.Sort((x, y) => string.Compare(x.Nom, y.Nom));
                    }
                }
            }
            return data;
        }
    }
}
