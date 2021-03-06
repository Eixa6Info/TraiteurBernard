﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TraiteurBernardWPF.Utils
{
    class WinFormWpf
    {
        /// <summary>
        /// Place une fenêtre en haut à gauche d'une autre fenetre
        /// </summary>
        /// <param name="wpf"></param>
        /// <param name="parent"></param>
        internal static void CornerTopLeftToParent(Window wpf, Window parent)
        {
            wpf.Top = parent.Top;
            wpf.Left = parent.Left;
        }

        /// <summary>
        /// Place une fenêtre en haut à gauche de l'écran de l'ordinateur
        /// </summary>
        /// <param name="wpf"></param>
        internal static void CornerTopLeftToComputer(Window wpf)
        {
            wpf.Top = 0;
            wpf.Left = 0;
        }
        
        /// <summary>
        /// Place une fenêtre enfant au milieu du parent
        /// </summary>
        /// <param name="wpf"></param>
        internal static void CenterToParent(Window wpf, Window parent)
        {
            wpf.Top = parent.Top + (parent.Height / 2) - wpf.Height / 2;
            wpf.Left = parent.Left + (parent.Width / 2) - wpf.Width / 2;
        }


    }
}
