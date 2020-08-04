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
    class TourneeDAO
    {
        internal static string GetStringFromId(int id)
        {
            switch (id)
            {
                case 4:
                    return "Marennes";
                    break;
                case 3:
                    return "contre-tournée";
                    break;
                case 2:
                    return "ville 2";
                    break;
                case 1:
                    return "ville 1";
                    break;
                default:
                    return "erreur";
            }
        }
    }
}
