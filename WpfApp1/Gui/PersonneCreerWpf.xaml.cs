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
        Personne edite;
        BaseContext db;

        /// <summary>
        /// Constructeur sans paramètres donc pour la création d'une personne
        /// </summary>
        public PersonneCreerWpf()
        {
            InitializeComponent();
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
                if (edite.ID == 0) db.Add(edite);
                db.SaveChanges();
            }
            else
            {
                MessageBox.Show("Les informations de nom, prénom et tournée sont indispensables",
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
            var req = from t in db.TypeTournee
                      select t;

            var data = new List<TypeTournee>();

            foreach (var tt in req)
            {
                db.Entry(tt).Collection(s => s.JoursLivraisonsRepas).Load();
                data.Add(tt);
            }
            cbTournee.ItemsSource = data;

  
            UpdateStatus(lblContactDurgence, edite.ContactDurgence, "Pas de contact d'urgence"); 
            UpdateStatus(lblCompte, edite.CompteDeFacturation, "Pas de compte de facturation"); 
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
            edite.CompteDeFacturation = wpf.CompteAssocie;
            UpdateStatus(lblCompte, edite.CompteDeFacturation, "Pas de compte de facturation");

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
            ContactDurgenceCreer wpf = new ContactDurgenceCreer(edite.ContactDurgence, db);
            wpf.ShowDialog();
            edite.ContactDurgence = wpf.Edite;
            UpdateStatus(lblContactDurgence, edite.ContactDurgence, "Pas de contact d'urgence");
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
            edite.APALivraisonMax = 0.0F;
            edite.APAMontantMax = 0.0F;
        }
    }
}
