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
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour ContactDurgenceCreer.xaml
    /// </summary>
    public partial class ContactDurgenceCreer : Window
    {
        BaseContext db;

        public ContactDurgence Edite { get; set; }

        public ContactDurgenceCreer(ContactDurgence edite, BaseContext db)
        {
            InitializeComponent();
            this.db = db;
            this.Edite = edite != null ? edite : new ContactDurgence();
            edition.DataContext = Edite;

        }

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

        private void Valider(object sender, RoutedEventArgs e)
        {
            if (VerifierDonnees())
            {
                if (Edite.ID == 0) db.Add(Edite);
                db.SaveChanges();
                Close();
            }
            else MessageBox.Show("Les informations de nom, prénom et téléphone sont indispensables",
                    "Informations indispensables",
                    MessageBoxButton.OK, MessageBoxImage.Error);
           
        }

        private void Supprimer(object sender, RoutedEventArgs e)
        {
            if (Edite.ID == 0) return;
            db.Remove(Edite);
            Edite = null;
            Close();
        }
    }
}
