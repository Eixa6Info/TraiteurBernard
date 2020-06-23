using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.PDF;
using TraiteurBernardWPF.Security;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PdfCreerSaisieClient.xaml
    /// </summary>
    public partial class PdfCreerSaisieClient : Window
    {

        BaseContext db;
        public Saisie Edite { get; set; }
        public Personne per;
        public IList<Personne> SaisiePersonneList { get; set; }
        String recherche = "";

        public PdfCreerSaisieClient()
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new Saisie { Semaine = 1, Annee = DateTime.Now.Year };
            SaisiePersonneList = new List<Personne>();
            edition.DataContext = this.Edite;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IQueryable<Personne> req2 = from t in this.db.Personnes
                                        select t;

            List<Personne> data2 = new List<Personne>();

            foreach (Personne p in req2)
            {
                this.db.Entry(p).Reference(s => s.Tournee).Load();
                this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                this.db.Entry(p).Reference(s => s.ContactDurgence).Load();


                if (p.Actif == true)
                {
                    data2.Add(p);
                    data2.Sort((x, y) => string.Compare(x.Nom, y.Nom));
                }
            }
            ListBoxPersonne.ItemsSource = data2;
            int laDate = GestionDeDateCalendrier.TrouverLeNumSemaineAvecMois(DateTime.Now);
            
            if (laDate % 2 == 0)
            {
                // pair
                laDate = laDate - 1;
            }
          
            txtSemaine.Text = laDate.ToString();
        }

        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtSemaine.Text.Length != 0 && ListBoxPersonne.SelectedItem != null)
            {
                if (Int32.Parse(txtSemaine.Text) % 2 == 0)
                {
                    retval = false;
                }
                else
                {
                    retval = true;
                }
                
            }

            return retval;
        }

        /// <summary>
        /// Valider selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.VerifierDonnees())
                {
                    foreach(var personne in SaisiePersonneList)
                    {
                        per = personne as Personne;
                        var outputfile = CreatePDFClient.Start(595.27563F, 841.8898F, short.Parse(txtSemaine.Text), DateTime.Today.Year, true, per);
                        if (!string.IsNullOrEmpty(outputfile))
                        {
                            System.Diagnostics.Process.Start(outputfile);
                        }  
                    }      
                }
                else
                {
                    MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpInfoIndispensableIncorrecte, Properties.Resources.MessagePopUpErrorIndiIncorrecte, MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                }
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PdfCreerSaisieClient.xaml.cs");
                throw;
            }
        }
        public static void PrintClient(Personne personne, int semaine)
        {
                var outputfile = CreatePDFClient.Start(595.27563F, 841.8898F, semaine, DateTime.Today.Year, true, personne);
                if (!string.IsNullOrEmpty(outputfile))
                {
                    System.Diagnostics.Process.Start(outputfile);
                }
            
        }

        /// <summary>
        /// fermer fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void VerifierFormatSemaine(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);
        }

        /*private void textChangedRechercheClient(object sender, TextChangedEventArgs e)
        {
            try
            {
                String lastNameWordP;
                String wordPNameSplit = "";
                this.recherche = txtRecherche.Text;
                IQueryable<Personne> req = from t in db.Personnes
                                           select t;

                List<Personne> data = new List<Personne>();
                foreach (Personne p in req)
                {
                    this.db.Entry(p).Reference(s => s.Tournee).Load();
                    this.db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                    this.db.Entry(p).Reference(s => s.ContactDurgence).Load();

                    lastNameWordP = p.Nom.ToLower();

                    // Si il y a rien dans la barre de recherche on affiche tous
                    if (recherche == "")
                    {
                        data.Add(p);
                    }

                    // compte de le nombre de lettre dans la barre de recherche et réitère à charque nouvelle lettre
                    for (int i = 0; i < recherche.Length; i++)
                    {
                        if (recherche.Length <= lastNameWordP.Length)
                        {
                            wordPNameSplit = lastNameWordP.Substring(0, i + 1);
                        }

                        // Si la lettre du nom = a la lettre de la barre de recherche on ajoute a la liste
                        if (wordPNameSplit.ToLower() == recherche.ToLower())
                        {
                            data.Add(p);
                        }
                    }
                }
                ListBoxPersonne.ItemsSource = data;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneListerWpf.xaml.cs");
                throw a;
            }
        }*/
       
            /// <summary>
            /// Basculer les personnes de la liste box au label
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void ListBoxPersonne_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Personne selected = ListBoxPersonne.SelectedItem as Personne;
                if (SaisiePersonneList == null) SaisiePersonneList = new List<Personne>();
                Personne found = (from p in SaisiePersonneList
                                    where p.ID == selected.ID
                                    select p).FirstOrDefault();

                if (found != null)
                {
                    SaisiePersonneList.Remove(found);
                }
                else
                {
                    SaisiePersonneList.Add(selected);
                }
                lblListe.Content = string.Join<Personne>("\n", (from p in SaisiePersonneList select p).ToArray()); 
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "CompteDeFacturationCréeWpf.xaml.cs");
                throw a;
            }
        }
    }
}
