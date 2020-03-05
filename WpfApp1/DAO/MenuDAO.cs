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

        /// <summary>
        /// Recupérer un menu en fonction de la semaine et du jour
        /// </summary>
        /// <param name="semaine"></param>
        /// <param name="jour"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static Menu getFirstFromWeekAndDay(int semaine, int jour, BaseContext db)
        {


            TraiteurBernardWPF.Modele.Menu menu = (from t in db.Menu where t.Jour == jour && t.Semaine == semaine select t).FirstOrDefault();
            if(menu != null) db.Entry(menu).Collection(m => m.Plats).Load();

            //db.Dispose();

            return menu;
        }

        /// <summary>
        /// Récupérer des menus en fonctionde la semaine
        /// </summary>
        /// <param name="semaine"></param>
        /// <returns></returns>
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

                // Liste de plats
                List<Plat> tabPlats = new List<Plat>();

                // Tableau de plats de taille 8 (contient que des 'null' pour l'instant)
                Plat[] arrPlats = new Plat[8];

                // Copy des plats dans le tableau (contient donc des plat et potentiellemetn des
                // null si jamais il n'y a pas tous les plats d'inscrits
                p.Plats.CopyTo(arrPlats, 0);

                // Conversion du tableau en liste pour la manipulation des données plus facile
                tabPlats = arrPlats.ToList();
                
                // On trie les plats (ex si on a 4 plats ( type 1, 3, 4 et 5) la liste vaudra :
                // 1, 3, 4, 5, null, null, null, null
                tabPlats = tabPlats.OrderBy(x => x != null ? x.Type : 9).ToList();

                for(int i = 0; i < 8 ; i++)
                {
                    // Si le plat actuel est inscrit
                    if (tabPlats[i] != null)
                        // Si il est a la bonne place (entrée avant des plats, plats avant
                        // le dessert etc
                        if (tabPlats[i].Type == i + 1)
                            continue;
                        else
                        {
                            // Sinon on enlève le dernier plat (forcement un null) et on 
                            // insert le null au bon index
                            tabPlats.RemoveAt(7);
                            tabPlats.Insert(i, null);
                        }
                    
                }

                // Au final de tableau de plats derrangé : 1, 3, 4, 5, null, null, null, null
                // devant : 1, null, 3, 4, 5, null, null, null
                // Et on reconvertit le tout en hashset
                p.Plats = tabPlats.ToHashSet();

                data.Add(p);
            }

            db.Dispose();

            return data;
        }

        internal static List<String> getPlatsNameFromWeekDay(int semaine, int jour)
        {
            BaseContext db = new BaseContext();

            IQueryable<Menu> req = from t in db.Menu
                                   where t.Semaine == semaine && t.Jour == jour
                                   select t;

            List<String> data = new List<String>();

            foreach(Menu menu in req)
            {
                db.Entry(menu).Collection(m => m.Plats).Load();

                foreach(Plat plat in menu.Plats)
                {
                    data.Add(plat.Name);
                }
            }

            return data;

        }
    }
}
