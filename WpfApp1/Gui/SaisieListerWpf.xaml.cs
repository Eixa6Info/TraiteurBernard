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
using System.Windows.Shapes;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour SaisieListerWpf.xaml
    /// </summary>
    public partial class SaisieListerWpf : Window
    {

        BaseContext db;
        public SaisieListerWpf()
        {
            InitializeComponent();
            this.db = new BaseContext();
        }

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            this.db.Dispose();
            Close();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            IQueryable<Personne> personnes = from p in this.db.Personnes select p;

            List < Saisie > saisies2020 = SaisieDAO.getAllFromYear(2020, this.db);

            // Liste d'objets anonymes qui vont etre utilisés pour faire une ligne dans la datagrid
            // forme : Jour, EntreeMidi, Plat1Midi, Plat2Midi, Plat3Midi, DessertMidi, EntreeSoir, PlatSoir, DessertSoir
            List<object> rowFormList = new List<object>();

            // Liste d'objets anonymes permettant de mettre en forme l'expander ainsi que la datagrid associée
            // Contient le Header ainsi que la liste des lignes (rowFormList)
            List<object> data = new List<object>();

       

           // Dictionary<int, Dictionary<int, List<object>>> defze = new Dictionary<int, Dictionary<int, List<object>>> ();



            List<object> semaines = new List<object>();

            for(int semaine = 1; semaine < 53; semaine++)
            {

                List<object> personnas = new List<object>();

                foreach(Personne pers in personnes)
                {
                    personnas.Add(new
                    {
                        HeaderPersonne = pers.ToString(),
                        Data = new List<object>
                                {
                                    new
                                    {
                                        Jour = 1,
                                        EntreeMidi  = "salade"
                                    }
                                }
                    });
                }
               
                semaines.Add(new
                {
                    HeaderSemaine = "Semaine " + semaine,
                    Personne = personnas
                });
                    
      


            }
            data.Add(new
            {
                HeaderAnnee = "Année " + "2020",
                Semaine = semaines
            });
            

           /* List<object> final = new List<object> 
            { 
                new 
                { 
                    HeaderAnnee = "Année 2020", 
                    Semaine = new List<object>
                    {
                        new
                        {
                            HeaderSemaine = "Semaine 1",
                            Personne = new List<object>
                            {
                                new
                                {
                                    HeaderPersonne = "willy tardy",
                                    Data = new List<object>
                                    {
                                        new
                                        {
                                            Jour = 1,
                                            EntreeMidi  = "salade"
                                        }
                                    }
                                }
                                
                            }
                        }
                    } 
                
                } 
            
            
            };*/
        

            //dataGridAnnee.ItemsSource = data;

        }
    }
}
