using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Utils;
using System.Globalization;
using System.IO;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PersonneWpf.xaml
    /// </summary>
    public partial class PersonneCreerWpf : Window
    {
        DateTime thisDate = new DateTime(1950, 01, 01);
        private Personne edite;
        private BaseContext db;
        private float res = 0.0F;
        private float liv = 0.00F;
        private float tau = 0.0F;

        public MessageBoxWpf MessageBoxWpf { get; private set; }

        /// <summary>
        /// Constructeur sans paramètres donc pour la création d'une personne
        /// </summary>
        public PersonneCreerWpf()
        {
            InitializeComponent();
            Title += "Création d'une personne";
            this.edite = new Personne();
            this.db = new BaseContext();
            edition.DataContext = this.edite;
            datePicker.DisplayDate = thisDate;
        }

        /// <summary>
        /// Constructeure avec paramètres donc pour la modification d'une personne
        /// </summary>
        /// <param name="edite"></param>
        /// <param name="db"></param>
        public PersonneCreerWpf(Personne edite, BaseContext db)
        {
            InitializeComponent();
            Title += "Modification d'une personne";
            this.edite = edite;
            this.db = db;
            edition.DataContext = this.edite;
        }

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Verifier les données avant de valider l'opération
        /// </summary>
        private bool VerifierDonnees()
        {
            bool retval = false;

            if (txtNom.Text.Length != 0 && txtPrenom.Text.Length != 0 && cbTournee.SelectedItem != null)
            {
                retval = true;
            }

            return retval;
        }

        /// <summary>
        /// Valider l'opération de création / modification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerifierDonnees())
                {
                    if (this.edite.ID == 0) this.db.Add(this.edite);
                    this.db.SaveChanges();
                    Close();
                }
                else
                {
                    MessageBoxWpf wpf = new MessageBoxWpf("Informations indispensables", "Le nom, le prénom et la tournée sont indisensables", MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                } 
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
        }

        /// <summary>
        /// Au chargement de la fenêtre, on charge les tournées dans la combobox prévue pour cela
        /// et on met à jour les status de Contact et de Compte de facturation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IQueryable<TypeTournee> req = from t in this.db.TypeTournee
                                          select t;

            List<TypeTournee> data = new List<TypeTournee>();

            foreach (TypeTournee tt in req)
            {
                this.db.Entry(tt).Collection(s => s.JoursLivraisonsRepas).Load();
                data.Add(tt);
            }
            String[] couleur = {"Vert", "Rose", "Jaune","Gris"};
            cbTournee.ItemsSource = data;
            cbTourneeCouleur.ItemsSource = couleur;
            this.UpdateStatus(lblContactDurgence, this.edite.ContactDurgence, "Pas de contact d'urgence"); 
            this.UpdateStatus(lblCompte, this.edite.CompteDeFacturation, "Pas de compte de facturation"); 
        }

        /// <summary>
        /// Créer un compte de facturation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompteDeFacturationCreer(object sender, RoutedEventArgs e)
        { 
            try
            {
                CompteDeFacturationListerWpf wpf = new CompteDeFacturationListerWpf(db);
                wpf.ShowDialog();
                if(wpf.CompteAssocie != null) this.edite.CompteDeFacturation = wpf.CompteAssocie;
                this.UpdateStatus(lblCompte, this.edite.CompteDeFacturation, "Pas de compte de facturation");
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
        }

        /// <summary>
        /// Mettre à jour le status sur un label, par rapport à un objet et en fournissant un
        /// string au cas ou l'objet est null
        /// </summary>
        /// <param name="label"></param>
        /// <param name="obj"></param>
        /// <param name="textIfNull"></param>
        private void UpdateStatus(Label label, Object obj, String textIfNull)
        {
            label.Content = obj != null ? obj.ToString() : textIfNull;
        }

        /// <summary>
        /// // Créer un contact d'urgence
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContactDurgenceCreer(object sender, RoutedEventArgs e)
        {  
            try
            {
                ContactDurgenceCreerWpf wpf = new ContactDurgenceCreerWpf(edite.ContactDurgence, db);
                wpf.ShowDialog();
                this.edite.ContactDurgence = wpf.Edite;
                this.UpdateStatus(lblContactDurgence, this.edite.ContactDurgence, "Pas de contact d'urgence");
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
        }

        /// <summary>
        /// Si la cache APA est coché, on affiche les champs pour rentrer les informations
        /// associées (livraison max  et montant max)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void APA_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtAPALivraisonMax.Visibility = Visibility.Visible;
                txtAPAMontantMax.Visibility = Visibility.Visible;
                txtAPADateDebut.Visibility = Visibility.Visible;
                txtAPADateFin.Visibility = Visibility.Visible;
                txtAPALivraisonPrix.Text = "0";
                txtAPATauxClient.Text = "0";
          
                txtAPATauxClient.Visibility = Visibility.Visible;
                txtAPALivraisonPrix.Visibility = Visibility.Visible;
                btnVerifier.Visibility = Visibility.Visible;

                Console.Out.WriteLine(txtAPACovid.Text);

                txtAPACovid.Visibility = Visibility.Visible;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
        }

        /// <summary>
        /// Si la cache APA est décoché, on cache les champs pour rentrer les informations 
        /// associées et on les remet à 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void APA_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtAPALivraisonMax.Text = "0";
                txtAPAMontantMax.Text = "0";
                txtAPADateDebut.Text = "00/00/00";
                txtAPADateFin.Text = "00/00/00";
                txtAPATauxClient.Text = "0";
                txtAPALivraisonPrix.Text = "0";
                txtAPACovid.Text = "0";

                txtAPALivraisonMax.Visibility = Visibility.Hidden;
                txtAPAMontantMax.Visibility = Visibility.Hidden;
                txtAPADateDebut.Visibility = Visibility.Hidden;
                txtAPADateFin.Visibility = Visibility.Hidden;
                txtAPATauxClient.Visibility = Visibility.Hidden;
                txtAPALivraisonPrix.Visibility = Visibility.Hidden;
                txtAPACovid.Visibility = Visibility.Hidden;
                btnVerifier.Visibility = Visibility.Hidden;
            
                this.edite.APALivraisonMax = 0.0F;
                this.edite.APAMontantMax = 0.0F;
                this.edite.APADateDebut = "00/00/00";
                this.edite.APADateFin = "00/00/00";
                this.edite.APATauxClient = 0.0F;
                this.edite.APALivraisonPrix = 0.0F;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
            
        }
        
        private void MSA_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMSALivraisonMax.Visibility = Visibility.Visible;
                txtMSAMontantMax.Visibility = Visibility.Visible;
                txtMSADateDebut.Visibility = Visibility.Visible;
                txtMSADateFin.Visibility = Visibility.Visible;
                txtAPATauxClient.Visibility = Visibility.Visible;
                txtAPALivraisonPrix.Visibility = Visibility.Visible;
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
            
        }

        /// <summary>
        /// Si la cache APA est décoché, on cache les champs pour rentrer les informations 
        /// associées et on les remet à 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MSA_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMSALivraisonMax.Text = "0";
                txtMSAMontantMax.Text = "0";
                txtMSADateDebut.Text = "00/00/00";
                txtMSADateFin.Text = "00/00/00";
                txtMSALivraisonMax.Visibility = Visibility.Hidden;
                txtMSAMontantMax.Visibility = Visibility.Hidden;
                txtMSADateDebut.Visibility = Visibility.Hidden;
                txtMSADateFin.Visibility = Visibility.Hidden;
                this.edite.MSALivraisonMax = 0.0F;
                this.edite.MSAMontantMax = 0.0F;
                this.edite.MSADateDebut = "00/00/00";
                this.edite.MSADateFin = "00/00/00";
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
        }


        private void Verifier(object sender, RoutedEventArgs e)
        {
            try
            {
                string l;
                tau = float.Parse(txtAPATauxClient.Text);
                l = String.Format("{0:0.##}", txtAPALivraisonPrix.Text);
                liv = float.Parse(l, CultureInfo.InvariantCulture.NumberFormat);
                res = liv - (liv * (tau / 100));
                txtAPACovid.Text = res.ToString();
                
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
        }

        private void cbTournee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTournee.SelectedIndex == 2)
            {
                cbTourneeCouleur.Visibility = Visibility.Visible;
            }
            else
            {
                cbTourneeCouleur.Visibility = Visibility.Hidden;
            }

            if (cbTournee.SelectedIndex == 0)
            {
                cbTourneeCouleur.SelectedValue = "Vert";
            }
            else if (cbTournee.SelectedIndex == 1)
            {
                cbTourneeCouleur.SelectedValue = "Jaune";
            }
        }

        private void cbTourneeCouleur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ValiderEtNew(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerifierDonnees())
                {
                    if (this.edite.ID == 0) this.db.Add(this.edite);
                    this.db.SaveChanges();
                    Close();
                }
                else
                {
                    MessageBoxWpf wpf = new MessageBoxWpf("Informations indispensables", "Le nom, le prénom et la tournée sont indisensables", MessageBoxButton.OK);
                    WinFormWpf.CenterToParent(wpf, this);
                    wpf.ShowDialog();
                }
                PersonneCreerWpf wpfNew = new PersonneCreerWpf();
                WinFormWpf.CornerTopLeftToParent(wpfNew, this);
                wpfNew.ShowDialog();
            }
            catch (IOException a)
            {
                LogHelper.WriteToFile(a.Message, "PersonneCreerWpf.xaml.cs");
                throw a;
            }
        }
    }
}
