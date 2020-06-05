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
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using GregorianCalendar = System.Globalization.GregorianCalendar;
using Calendar = System.Globalization.Calendar;
using System.IO;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieCreerCalendrierWpf.xaml
    /// </summary>
    public partial class SaisieCreerCalendrierWpf : Window
    {
        private BaseContext db;
        private Saisie Edite { get; set; }
        private int[] IDs;

        public SaisieCreerCalendrierWpf(Saisie edite, BaseContext db, int[] IDs)
        {
            InitializeComponent(); 
            //this.db = new BaseContext();
            this.db = db;
            this.Edite = edite;
            this.IDs = IDs;
        }

        private void calendar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("la date");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("avant visibility.Hidden");
            this.Visibility = Visibility.Hidden;
            Console.WriteLine("apres visibility.Hidden");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
               
                List<int> jour = new List<int>();
                List<int> numSemaine = new List<int>();
                List<int> annee = new List<int>();
                List<string> jourLivraison = new List<string>();
                List<string> jourRepas1 = new List<string>();
                List<string> jourRepas2 = new List<string>();
                List<string> jourRepas3 = new List<string>();
                int intAnnee = 0;
                int intMois = 0;
                int resMois = 0;
                int intJour = 0;


                IQueryable<Saisie> req = from t in db.Saisies
                                         select t;

                IQueryable<Livraison> reqLiv = from t in db.Livraisons
                                               select t;

                foreach (Livraison t in reqLiv)
                {
                    jourLivraison.Add(t.JourLivraison);
                    jourRepas1.Add(t.JourRepas1);
                    jourRepas2.Add(t.JourRepas2);
                    jourRepas3.Add(t.JourRepas3);
                }



                foreach (Saisie p in req)
                {
                    if (p.Personne.ID == Edite.ID)
                    {
                        jour.Add(p.Jour);
                        annee.Add(p.Annee);
                        numSemaine.Add(p.Semaine);
                        // jourLivraison.Add(Int32.Parse(p.Personne.Tournee.JoursLivraisonsRepas.ToString()));

                        foreach (int aAnnee in annee)
                        {
                            intAnnee = aAnnee;

                            foreach (int aSemaine in numSemaine)
                            {
                                intMois = aSemaine;
                                Calendar toMoisCalendar = new GregorianCalendar();

                                int searchedWeek = intMois; // N° de semaien à rechercher
                                DateTime dtFirstDayFirstWeek = DateTime.MinValue; // pour éviter erreur de compil
                                for (int iWeek = 0, iDay = 1; iWeek != 1; iDay++)
                                {
                                    dtFirstDayFirstWeek = new DateTime(intAnnee, 1, iDay);
                                    iWeek = toMoisCalendar.GetWeekOfYear(dtFirstDayFirstWeek, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                                }
                                DateTime firstDayOfGivenWeek = toMoisCalendar.AddWeeks(dtFirstDayFirstWeek, searchedWeek - 1);
                                resMois = firstDayOfGivenWeek.Month;

                                foreach (int aJour in jour)
                                {
                                    if (aJour == 1)
                                    {
                                        intJour = firstDayOfGivenWeek.Day;
                                    }
                                    if (aJour == 2)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 1;
                                    }
                                    if (aJour == 3)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 2;
                                    }
                                    if (aJour == 4)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 3;
                                    }
                                    if (aJour == 5)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 4;
                                    }
                                    if (aJour == 6)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 5;
                                    }
                                    if (aJour == 7)
                                    {
                                        intJour = firstDayOfGivenWeek.Day + 6;
                                    }
                                }
                            }
                        }
                        if (intAnnee != 0 && resMois != 0 && intJour != 0)
                        {

                            Console.WriteLine("annee: " + intAnnee);
                            Console.WriteLine("mois: " + resMois);
                            Console.WriteLine("jour: " + intJour);
                            Console.WriteLine("jour de livraison: ");
                            Console.WriteLine("JoursDeLivraisonsrepas" + p.Tournee.JoursLivraisonsRepas);
                            //calendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(intAnnee, resMois, intJour)));
                            calendar.SelectedDates.Add(new DateTime(intAnnee, resMois, intJour));
                            calendar.BlackoutDates.Add(new CalendarDateRange(new DateTime()));
                            calendar.DisplayDate = new DateTime(intAnnee, resMois, intJour);


                        }

                    }
                }
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }
    }
}
