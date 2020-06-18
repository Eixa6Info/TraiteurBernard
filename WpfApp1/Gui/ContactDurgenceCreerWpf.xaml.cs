using System.Windows;
using System.Windows.Input;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Security;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour ContactDurgenceCreerWpf.xaml
    /// </summary>
    public partial class ContactDurgenceCreerWpf : Window
    {

        private BaseContext db;

        private ContactDurgence sauvegardeEdite;

        public ContactDurgence Edite { get; set; }

        /// <summary>
        /// Un seul constructeur, on peut passer un objet null ou un objet initialisé 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public ContactDurgenceCreerWpf(ContactDurgence edite, BaseContext db)
        {
            InitializeComponent();
            this.db = db;

            if (edite != null)
            {
                // On sauvegarde l'objet en paramètre comme si l'utilisateur ferme la fenêtre sans valider,
                // on pourra reutiliser la version original
                this.sauvegardeEdite = new ContactDurgence
                {
                    ID = -1,
                    Prenom = edite.Prenom,
                    Nom = edite.Nom,
                    Telephone = edite.Telephone
                };
                this.Edite = edite;
            }
            else
            {
                this.sauvegardeEdite = null;
                this.Edite = new ContactDurgence();
            }

            edition.DataContext = this.Edite;

        }

        /// <summary>
        /// Fonction de vérification des données avant sauvgarde
        /// </summary>
        private bool VerifierDonnees()
        {
            bool isValid = false;

            if (txtNom.Text.Length != 0 && 
                txtPrenom.Text.Length != 0 && 
                txtTelephone.Text.Length != 0)
            {
                isValid = true;
            }
            
           
            return isValid;
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
                if (this.Edite.ID == 0) this.db.Add(this.Edite);
                Close();
            }
            else
            {
                MessageBoxWpf wpf = new MessageBoxWpf(Properties.Resources.MessagePopUpErrorIndispensable, Properties.Resources.MessagePopUpErrorIndispensable3, MessageBoxButton.OK);
                WinFormWpf.CenterToParent(wpf, this);
                wpf.ShowDialog();
            }



        }

        /// <summary>
        /// Suppression du contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supprimer(object sender, RoutedEventArgs e)
        {
            if (this.Edite.ID != 0) this.db.Remove(this.Edite);
            this.Edite = null;
            Close();
        }

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            // Si le contact n'étai pas null, on renvoit l'original sinon on renvoit un contact null
            if(this.sauvegardeEdite != null)
            {
                this.Edite.Nom = this.sauvegardeEdite.Nom;
                this.Edite.Prenom = this.sauvegardeEdite.Prenom;
                this.Edite.Telephone = this.sauvegardeEdite.Telephone;
            }
            else
                this.Edite = null;
            
            Close();
        }

        private void VerifierNumeroTelephone(object sender, TextCompositionEventArgs e)
        {
            ControlsVerification.DigitsOnly(e);

        }
    }
}
