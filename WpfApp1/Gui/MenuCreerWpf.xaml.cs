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

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour MenuCreerWpf.xaml
    /// </summary>
    public partial class MenuCreerWpf : Window
    {

        private BaseContext db;

        // La classe Menu existe déjà donc on utilise le namespace pour distinguer
        // notre classe métier de la classe déjà existante
        private TraiteurBernardWPF.Modele.Menu Edite;

        /// <summary>
        /// Constructeur sans paramètres donc sans dépendance donc création 
        /// </summary>
        public MenuCreerWpf()
        {
            InitializeComponent();
            this.db = new BaseContext();
            this.Edite = new TraiteurBernardWPF.Modele.Menu { Plats = new HashSet<Plat>() };
            edition.DataContext = this.Edite;     
        }

        /// <summary>
        /// Fonction de vérification des données avant création / édition
        /// </summary>
        private bool VerifierDonnees()
        {
            bool retval = false;

            /*if (txtNom.Text.Length != 0)
            {
                retval = true;
            }

            return retval;*/
            return true;
        }

        /// <summary>
        /// Ajouter UN plat dans la hashset des plats
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="type"></param>
        private void AjouterPlat(TextBox txt, int type)
        {
            if (!String.IsNullOrEmpty(txt.Text)) this.Edite.Plats.Add(new Plat { Type = type, Name = txt.Text });
        }

        /// <summary>
        /// Ajouter DES plats dans la hastset des plats
        /// </summary>
        /// <param name="txt_type"></param>
        private void AjouterPlat(Dictionary<TextBox, int> txt_type)
        {
            foreach (KeyValuePair<TextBox, int> pair in txt_type)
                if (!String.IsNullOrEmpty(pair.Key.Text)) this.Edite.Plats.Add(new Plat { Type = pair.Value, Name = pair.Key.Text });
        }

        /// <summary>
        /// Valider les changements / la création
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider(object sender, RoutedEventArgs e)
        {
            Dictionary<TextBox, int> txt_type = new Dictionary<TextBox, int>
            {
                { txtEntreeMidi, Plat.ENTREE_MIDI},
                { txtPlat1Midi, Plat.PLAT_MIDI_1},
                { txtPlat2Midi, Plat.PLAT_MIDI_2},
                { txtPlat3Midi, Plat.PLAT_MIDI_3},
                { txtDessertMidi, Plat.DESSERT_MIDI},
                { txtEntreeSoir, Plat.ENTREE_SOIR},
                { txtPlatSoir, Plat.PLAT_SOIR_1},
                { txtDessertSoir, Plat.DESSERT_SOIR},
            };

            this.AjouterPlat(txt_type);

            // Quelles données sont obligatoires ici ?? demander à Fabien
            if (VerifierDonnees())
            {
                if (this.Edite.ID == 0) this.db.Add(Edite);
                this.db.SaveChanges();
                Close();
            }
            else
            {
                MessageBox.Show("Les informations de nom, prénom et tournée sont indispensables",
                    "Informations indispensables",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



    }
}
