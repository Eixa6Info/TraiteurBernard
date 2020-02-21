using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.DAO
{
    class MenuDao
    {
        internal static List<Menu> getAllFromWeek(int semaine)
        {
            BaseContext bd = new BaseContext();
            //this.db.Entry(p).Reference(s => s.Tournee).Load();
            IQueryable<Menu> req = from t in bd.Menu
                                   where t.Semaine == semaine
                                       select t;

            List<Menu> data = new List<Menu>();

            foreach (Menu p in req)
            {
                //Chargement préalable des données liées, sinon "lazy loading"
                // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                // voir pour plus de détails 
                bd.Entry(p).Collection(s => s.Plats).Load();


                data.Add(p);
            }
            return data;
        }
    }
}
