using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Utils
{
    class PersonnesHelper
    {
        public void CouleurPersonne(String couleur, ref int r, ref int g, ref int b)
        {
            if (couleur == "Vert")
            {
                r = 0;
                g = 155;
                b = 0;
            }
            else if (couleur == "Jaune")
            {
                r = 200;
                g = 220;
                b = 0;
            }
            else if (couleur == "Rose")
            {
                r = 255;
                g = 108;
                b = 158;
            }
            else if (couleur == "Jaune")
            {
                r = 255;
                g = 255;
                b = 0;
            }
            else if (couleur == "Gris")
            {
                r = 118;
                g = 118;
                b = 115;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }
        }
    }
}
