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

        internal static Menu getFirstFromWeekAndDay(int semaine, int jour, BaseContext db)
        {


            TraiteurBernardWPF.Modele.Menu menu = (from t in db.Menu where t.Jour == jour && t.Semaine == semaine select t).FirstOrDefault();
            if(menu != null) db.Entry(menu).Collection(m => m.Plats).Load();

            //db.Dispose();

            return menu;
        }

        internal static List<Menu> getAllFromWeek(int semaine)
        {
            BaseContext db = new BaseContext();

            IQueryable<Menu> req = from t in db.Menu
                                   where t.Semaine == semaine
                                       select t;

            List<Menu> data = new List<Menu>();

            foreach (Menu p in req)
            {
                //Chargement préalable des données liées, sinon "lazy loading"
                // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                // voir pour plus de détails 
                db.Entry(p).Collection(s => s.Plats).Load();


                data.Add(p);
            }

            db.Dispose();

            return data;
        }
    }
}
