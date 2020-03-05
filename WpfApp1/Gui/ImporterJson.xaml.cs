using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour ImporterJson.xaml
    /// </summary>
    public partial class ImporterJson : Window
    {

        /// <summary>
        /// Constructeur
        /// </summary>
        public ImporterJson()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fonction pour ouvrir le fichier JSON au clique sur bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OuvrirUnFichier(object sender, RoutedEventArgs e)
        {
            // Ouverture de la fenêtre d'exploration (api win32)
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Filtre des fichiers autorisés
            openFileDialog.Filter = "Fichier JSON |*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                // On récupère le nom du fichier et on affiche le contenu du fichier
                string chemin = openFileDialog.FileName;
                string[] cheminDivise = chemin.Split('\\');
                string nomDuFichier = cheminDivise[cheminDivise.Length - 1];
                lblNomDuFichier.Content = nomDuFichier;

                string contenuDuFichier = File.ReadAllText(openFileDialog.FileName);
                txtContent.Text = contenuDuFichier;
                
                // Object principal JSON
                JObject main =  JObject.Parse(contenuDuFichier);

                // Tableau des personnes
                JToken personnes = main.GetValue("personnes");

                BaseContext db = new BaseContext();

                int nbrPersonnes = 0;

                // Pour toutesles personnes ont incrémente le nombre de personne, on crée un objet personne
                // et on l'ajoute à la base de données
                foreach (JToken personne in personnes)
                {
                    nbrPersonnes++;
                    Personne personneObject = new Personne
                    {
                        Civilite = personne["civilite"].ToString(),
                        Nom = personne["nom"].ToString(),
                        Prenom = personne["prenom"].ToString(),
                        Ville = personne["ville"].ToString(),
                        ComplementAdresse = personne["complementAdresse"].ToString(),
                        Adresse = personne["adresse"].ToString(),
                        Comment = personne["commentaire"].ToString(),
                        ContactDurgence = new ContactDurgence
                        {
                            Nom = personne["contactUrgenceNom"].ToString(),
                            Prenom = personne["contactUrgencePrenom"].ToString(),
                            Telephone = personne["contactUrgenceTelephone"].ToString()
                        },
                        Tournee = (from t in db.TypeTournee
                                   where t.Nom.ToLower() == personne["tournee"].ToString().ToLower()
                                   select t).FirstOrDefault()
                    };

                    db.Add(personneObject);

                }

                // On affiche le nombre de personnes chargées
                lblPersonnesChargees.Content = nbrPersonnes + " personnes ont été chargées";
                db.SaveChanges();
                db.Dispose();
                
            }
        }
    }
}
