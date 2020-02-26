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
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Gui;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour PersonneWpf.xaml
    /// </summary>
    public partial class PersonneCreerWpf : Window
    {

        private Personne edite;
        private BaseContext db;

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
            if (VerifierDonnees())
            {
                if (this.edite.ID == 0) this.db.Add(this.edite);
                this.db.SaveChanges();
                Close();
            }
            else
            {
                MessageBox.Show("Le nom, le prénom et la tournée sont indisensables",
                    "Informations indispensables",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

            cbTournee.ItemsSource = data;
  
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
            CompteDeFacturationListerWpf wpf = new CompteDeFacturationListerWpf(db);
            wpf.ShowDialog();
            this.edite.CompteDeFacturation = wpf.CompteAssocie;
            this.UpdateStatus(lblCompte, this.edite.CompteDeFacturation, "Pas de compte de facturation");

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
            ContactDurgenceCreerWpf wpf = new ContactDurgenceCreerWpf(edite.ContactDurgence, db);
            wpf.ShowDialog();
            this.edite.ContactDurgence = wpf.Edite;
            this.UpdateStatus(lblContactDurgence, this.edite.ContactDurgence, "Pas de contact d'urgence");
        }

        /// <summary>
        /// Si la cache APA est coché, on affiche les champs pour rentrer les informations
        /// associées (livraison max  et montant max)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void APA_Checked(object sender, RoutedEventArgs e)
        {
            txtAPALivraisonMax.Visibility = Visibility.Visible;
            txtAPAMontantMax.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Si la cache APA est décoché, on cache les champs pour rentrer les informations 
        /// associées et on les remet à 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void APA_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAPALivraisonMax.Text = "0";
            txtAPAMontantMax.Text = "0";
            txtAPALivraisonMax.Visibility = Visibility.Hidden;
            txtAPAMontantMax.Visibility = Visibility.Hidden;
            this.edite.APALivraisonMax = 0.0F;
            this.edite.APAMontantMax = 0.0F;
        }
    }
}
