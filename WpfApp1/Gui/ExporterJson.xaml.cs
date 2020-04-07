using Microsoft.Win32;
using Newtonsoft.Json;
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
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour ImporterJson.xaml
    /// </summary>
    public partial class ExporterJson : Window
    {

        /// <summary>
        /// Constructeur
        /// </summary>
        public ExporterJson()
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
            SaveFileDialog openFileDialog = new SaveFileDialog();

            // Filtre des fichiers autorisés
            openFileDialog.Filter = "Fichier JSON |*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                // On récupère le nom du fichier et on affiche le contenu du fichier
                string chemin = openFileDialog.FileName;
                string[] cheminDivise = chemin.Split('\\');
                string nomDuFichier = cheminDivise[cheminDivise.Length - 1];

                lblNomDuFichier.Content = nomDuFichier;

                

                BaseContext db = new BaseContext();

                IQueryable<Personne> req = from t in db.Personnes
                                           select t;

                List<Personne> laListe = new List<Personne>();

                foreach (Personne p in req)
                {
                    //Chargement préalable des données liées, sinon "lazy loading"
                    // https://docs.microsoft.com/fr-fr/ef/ef6/querying/related-data
                    // voir pour plus de détails 
                    db.Entry(p).Reference(s => s.Tournee).Load();
                    //db.Entry(p).Reference(s => s.CompteDeFacturation).Load();
                    db.Entry(p).Reference(s => s.ContactDurgence).Load();

                    laListe.Add(p);
                }

                db.Dispose();



                int nbrPersonnes = 0;


                var strbuf = new StringBuilder();

                string output = JsonConvert.SerializeObject(laListe);//, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                strbuf.Append(output);

               

                txtContent.Text = strbuf.ToString();

                using (var fichier = new StreamWriter(openFileDialog.FileName))
                {
                    fichier.Write(strbuf.ToString());
                }
                

                /*
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
                */

                lblPersonnesChargees.Content = nbrPersonnes;
               


            }
        }
    }
}
