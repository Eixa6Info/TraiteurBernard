using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieCreerCalendrierWpf.xaml
    /// </summary>
    public partial class SaisieCreerCalendrierWpf : Window
    {
        public SaisieCreerCalendrierWpf()
        {
            InitializeComponent();
        }

        private void calendar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("la date");
        }
    }
}
