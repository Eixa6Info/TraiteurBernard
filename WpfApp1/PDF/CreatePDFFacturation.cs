﻿using com.sun.org.apache.regexp.@internal;
using com.sun.xml.@internal.bind.v2.runtime;
using java.util;
using javax.xml.soap;
using Microsoft.Win32;
using org.apache.pdfbox;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.common;
using org.apache.pdfbox.pdmodel.edit;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.pdmodel.graphics.color;
using sun.tools.tree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Gui;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.PDF
{
    class CreatePDFFacturation
    {
        /**
            * Variable pour le placement
            */
        private static float margin = 10;

        private static double minX = margin;
        private static double minY = margin;
        private static double columnSpace = 32; //12,5%
        private static double choiceSize = 51; //25Px

        private static PDType1Font BOLD = PDType1Font.HELVETICA_BOLD;
        private static PDType1Font NORMAL = PDType1Font.HELVETICA;
        private static PDType1Font OBLIQUE = PDType1Font.HELVETICA_OBLIQUE;
        private static double maxX;
        private static double maxY;
        private static double menuYTop;
        private static double menuYTopNoLivraison;
        private static double menuYTopNoDay;
        private static double menuYBottom;

        private static int Annee;
        private static TypeTournee Tournee;
        private static int Semaine;
        private static Dictionary<int,
        string> Comment;
        private static bool calculSemaine = true;
        private static string namePdf;
        private static string output;
        private static PDDocument document;
        private static PDPage blankPage;
        private static PDPageContentStream contentStream;

        private static int nombreClientParListe = 29;
        private static bool Msa;
        private static bool Apa;


        private static PersonnesHelper personnesHelper = new PersonnesHelper();

        /**
                 * Function d'enter pour créer le PDF
                 *
                 * @param format  PDRectangle A3 ou A4
                 * @param semaine Integer Numéros de semaine
                 * @return boolean
                 * @throws Exception ...
                 */
        public static string Start(float width, float height, int semaine, int annee, TypeTournee tournee, bool msa, bool apa)
        {

            //Récuperation du format de la page en fonction de A3 ou A4
            maxX = height - margin;
            maxY = width - margin;

            //Récupération des variable en relatison avec des pourcentage
            menuYTop = getY(100);
            menuYTopNoLivraison = getY(100);
            menuYTopNoDay = getY(99);
            menuYBottom = getY(1);
            Semaine = semaine;
            Comment = null;
            Annee = annee;
            Tournee = tournee;
            namePdf = "Facturation_" + tournee + "_" + annee + ".pdf";
            Msa = msa;
            Apa = apa;

            var semaineStart = semaine;
            var semaineEnd = semaine + 5;
            int nbrSemaine = semaineEnd - semaineStart;
            var data = GetData(nbrSemaine, Annee);

            if (data.Count == 0)
            {
                return "pas de données";
            }

            //Demande a l'utilisateur de choisir ou enregistrer    
            if (!getPath())
            {
                return "";
            }

            //Création du document
            document = new PDDocument();

            PrintPages(data, nbrSemaine);

            //Saving the document
            document.save(output);

            //Closing the document
            document.close();

            return output;
        }

        private static int GetNombreClient()
        {
            var db = new BaseContext();
            var nb = PersonneDAO.GetPersonnesWithTourneeNotAPANotMSA(Tournee.ID, db).Count;
            db.Dispose();
            return nb;
        }

        private static void PrintPages(List<dynamic> data, int nbrSemaines)
        {

            data = data.OrderBy(d => d.Couleur).ThenBy(d => d.Nom).ToList();

            int nombreDeClients = data.Count;
            int nombreClientsParPage = nombreClientParListe;

            for (int i = 0; i < nombreDeClients; i += nombreClientsParPage)
            {
                // methode qui coupe liste
                if (nombreDeClients - i >= nombreClientsParPage)
                {
                    PrintPage(nbrSemaines, i, i + nombreClientsParPage - 1, data);
                }
                else
                {
                    PrintPage(nbrSemaines, i, nombreDeClients - 1, data);

                }
            }

        }

        private static void PrintPage(int nombreDeSemaines, int start, int end, List<dynamic> data)
        {
            getDocument();

            double y = 100;
            double x = 0;

            //var db = new BaseContext();

            int largeurNom = 14;

            double largeurSemaine = (100 - 15) / 5;

            int hauteurHeader = 5;
            int hauteurSubHeader = 4;

            double hauteurClient = (100 - hauteurHeader - hauteurSubHeader) / nombreClientParListe;
            var customMaxX = nombreDeSemaines * largeurSemaine + largeurNom;
            var customMaxY = (100 - hauteurHeader - hauteurSubHeader) - nombreClientParListe * hauteurClient;

            int fontSize = 8;

            var largeurSupp = largeurSemaine * 0.40;
            var largeurMidi = largeurSemaine * 0.20;
            var largeurSoir = largeurSemaine * 0.20;
            var largeurBag = largeurSemaine * 0.20;

            //var data = GetData(nbrSemaines, Annee, Semaine);

            var clients = new List<dynamic>();

            y = y - hauteurHeader - hauteurSubHeader;

            for (int i = 0; i < nombreDeSemaines; i++)
            {
                y = 100 - hauteurHeader - hauteurSubHeader;

                int count = 0;
                for (int j = start; j <= end; j++)
                {
                    var client = data[j];

                    x = 0;
                    y -= hauteurClient;
                    x += largeurNom;
                    x += i * largeurSemaine;

                    if (i == 0)
                    {
                        contentStream.setNonStrokingColor(client.Couleur.Item1, client.Couleur.Item2, client.Couleur.Item3);
                        contentStream.fillRect((float)getX(0), (float)getY(y), (float)getX(largeurNom - 1.2), (float)getY(hauteurClient));
                        // remplir 'NOMS'
                        PrintTextBetweenTowPoint(client.Nom.ToString(), getX(0), getX(largeurNom), getY(y + hauteurClient / 3), fontSize, NORMAL, 0, 0, 0);
                    }

                    if (((List<dynamic>)client.Semaines).Any(c => c.Semaine == Semaine + i))
                    {
                        var detail = ((List<dynamic>)client.Semaines).Find(c => c.Semaine == Semaine + i);
                        // remplir 'SUPP'
                        x += largeurSupp;
                        PrintTextBetweenTowPoint(detail.Supp.ToString(), getX(x - largeurSupp), getX(x), getY(y + hauteurClient / 3), fontSize, NORMAL, 0, 0, 0);

                        // remplir 'MIDI'
                        x += largeurMidi;
                        // surlignage du midi
                        contentStream.setNonStrokingColor(96, 163, 188);
                        contentStream.fillRect((float)getX(x - largeurMidi), (float)getY(y), (float)getX(largeurMidi), (float)getY(hauteurClient));
                        PrintTextBetweenTowPoint(detail.Midi.ToString(), getX(x - largeurMidi), getX(x), getY(y + hauteurClient / 3), fontSize, NORMAL, 0, 0, 0);

                        // remplir 'SOIR'
                        x += largeurSoir;
                        // surlignage du soir
                        contentStream.setNonStrokingColor(164, 176, 190);
                        contentStream.fillRect((float)getX(x - largeurSoir), (float)getY(y), (float)getX(largeurSoir), (float)getY(hauteurClient));
                        PrintTextBetweenTowPoint(detail.Soir.ToString(), getX(x - largeurSoir), getX(x), getY(y + hauteurClient / 3), fontSize, NORMAL, 0, 0, 0);

                        // remplir 'BAG'
                        x += largeurBag;
                        PrintTextBetweenTowPoint(detail.Bag.ToString(), getX(x - largeurBag), getX(x), getY(y + hauteurClient / 3), fontSize, NORMAL, 0, 0, 0);
                    }

                }
            }

            // On dessigne ls traits des clients (masque les couleurs qui debordent
            y = 100 - hauteurSubHeader - hauteurHeader;
            for (int i = start; i <= end; i++)
            {
                y -= hauteurClient;
                // ligne horizontal du client
                drawLine(getX(0), getX(customMaxX), getY(y), getY(y));
            }

            y = 100;
            x = 0;

            // cadre horizontal haut
            drawLine(getX(0), getX(customMaxX), getY(y), getY(y));
            // cadre horizontal bas
            drawLine(getX(0), getX(customMaxX), getY(customMaxY), getY(customMaxY));
            // cadre vertical gauche
            drawLine(getX(0), getX(0), getY(y), getY(customMaxY));
            // cadre vertical droite
            // drawLine(getX(100), getX(100), getY(y), getY(customMaxY));

            y -= hauteurHeader;
            // ligne horizontal header
            drawLine(getX(0), getX(customMaxX), getY(y), getY(y));
            // ligne vertical colonne noms
            x += largeurNom;
            // gras
            drawLine(getX(x), getX(x), getY(100), getY(customMaxY));
            drawLine(getX(x + 0.1), getX(x + 0.1), getY(100), getY(customMaxY));

            //drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(0, x, BOLD, "Orange = APA", 10), getMiddelofYBetweenTowPoint(y + hauteurHeader, y, NORMAL, 10), "Orange = APA", 0, 0, 0);

            // ligne horizontal sub header
            y -= hauteurSubHeader;
            drawLine(getX(0), getX(customMaxX), getY(y), getY(y));
            drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(0, x, BOLD, "NOMS", 10), getMiddelofYBetweenTowPoint(y + hauteurSubHeader, y, NORMAL, 10), "NOMS", 0, 0, 0);

            // écriture des mots statiques
            for (int i = 0; i < nombreDeSemaines; i++)
            {
                // header
                drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(x, x + largeurSemaine, BOLD, "Semaine " + ((int)Semaine + i), 10), getMiddelofYBetweenTowPoint(y + hauteurSubHeader + hauteurHeader, y + hauteurSubHeader, NORMAL, 10), "Semaine " + ((int)Semaine + i), 0, 0, 0);

                // colonne SUPP
                x += largeurSupp;
                drawLine(getX(x), getX(x), getY(y + hauteurSubHeader), getY(customMaxY));
                drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(x - largeurSupp, x, BOLD, "SUPP", 10), getMiddelofYBetweenTowPoint(y + hauteurSubHeader, y, NORMAL, 10), "SUPP", 0, 0, 0);

                // colonne MIDI
                x += largeurMidi;
                drawLine(getX(x), getX(x), getY(y + hauteurSubHeader), getY(customMaxY));
                drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(x - largeurMidi, x, BOLD, "MIDI", 10), getMiddelofYBetweenTowPoint(y + hauteurSubHeader, y, NORMAL, 10), "MIDI", 0, 0, 0);

                // colonne SOIR
                x += largeurSoir;
                drawLine(getX(x), getX(x), getY(y + hauteurSubHeader), getY(customMaxY));
                drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(x - largeurSoir, x, BOLD, "SOIR", 10), getMiddelofYBetweenTowPoint(y + hauteurSubHeader, y, NORMAL, 10), "SOIR", 0, 0, 0);

                // colonne BAG
                x += largeurBag;
                // gras
                if (i != nombreDeSemaines)
                {
                    drawLine(getX(x), getX(x), getY(y + hauteurSubHeader + hauteurHeader), getY(customMaxY));
                    drawLine(getX(x + 0.1), getX(x + 0.1), getY(y + hauteurSubHeader + hauteurHeader), getY(customMaxY));
                }

                drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(x - largeurBag, x, BOLD, "BAG", 10), getMiddelofYBetweenTowPoint(y + hauteurSubHeader, y, NORMAL, 10), "BAG", 0, 0, 0);
            }

            //Close de la page
            contentStream.close();
        }

        static List<dynamic> GetData(int nbrSemaines, int annee)
        {
            List<dynamic> data = new List<dynamic>();

            /*
                  - List<dynamic>
                      |- NomClient : string
                      |- Couleur : 
                      |- Semaines : List<dynamic>
                          |- Semaine : int
                          |- Supp : int
                          |- Midi : int
                          |- Soir : int
                          |- Bag : int
                  */

            BaseContext db = new BaseContext();
            var couleur = (255, 255, 255);
            int semaine = 0;
            int nbMidi = 0;
            int nbSoir = 0;
            int nbBag = 0;
            int nbSupp = 0;
            int calculeTypeMidi = 0;
            int calculeTypeSoir = 0;
            
            List<Personne> personnes = null;

            if(!Msa && !Apa)
            {
                personnes = PersonneDAO.GetPersonnesWithTourneeNotAPANotMSA(Tournee.ID, db);
            }
            else if(Msa && Apa){
                personnes = PersonneDAO.GetPersonneWithTourneeMSAAPA(Tournee.ID);
            } else if (Msa && !Apa)
            {
                personnes = PersonneDAO.GetPersonneWithTourneeMSA(Tournee.ID);
            } else
            {
                personnes = PersonneDAO.GetPersonneWithTourneeAPA(Tournee.ID);
            }

            foreach (var p in personnes)
            {

                if (p.Couleur != null && Tournee.ID == 3)
                {
                    switch (p.Couleur)
                    {
                        case "Jaune":
                            couleur = (255, 202, 0);
                            break;
                        case "Rose":
                            couleur = (229, 113, 210);
                            break;
                        case "Gris":
                            couleur = (194, 192, 193);
                            break;
                        case "Vert":
                            couleur = (20, 227, 210);
                            break;
                        default:
                            couleur = (255, 255, 255);
                            break;

                    }
                }

                dynamic detailPersonne = new
                {
                    Nom = p.Nom,
                    Couleur = couleur,
                    Semaines = new List<dynamic>()
                };

                for (int i = 0; i < nbrSemaines; i++)
                {
                    semaine = Semaine + i;
                    for (int jour = 1; jour < 8; jour++)
                    {
                        List<Saisie> saisiesDeLaPersonneListe = SaisieDAO.getAllFromYearWeekDayForTourneeForPersonne(p, TourneeDAO.GetStringFromId(Tournee.ID), null, annee, semaine, jour, db);
                        List<SaisieData> saisieDatasDeLaPersonne = new List<SaisieData>();
                        foreach (Saisie saisie in saisiesDeLaPersonneListe)
                        {
                            foreach (SaisieData sdp in saisie.data)
                            {
                                saisieDatasDeLaPersonne.Add(sdp);
                            }
                        }

                        for (int repas = -1; repas <= 9; repas++)
                        {
                            foreach (SaisieData sd in SaisieDataDAO.SortByTypeFromList(repas, saisieDatasDeLaPersonne))
                            {
                                if (sd.Type == 1 || sd.Type == 2 || sd.Type == 3 || sd.Type == 4 || sd.Type == 6)
                                {
                                    if (sd.Quantite == 1 || sd.Quantite == 10)
                                    {
                                        calculeTypeMidi++;
                                    }
                                    else if (sd.Quantite == 2 || sd.Quantite == 20)
                                    {
                                        nbSupp++;
                                        calculeTypeMidi++;
                                    }
                                    else if (sd.Quantite == 3 || sd.Quantite == 30)
                                    {
                                        nbSupp++;
                                        nbSupp++;
                                        calculeTypeMidi++;
                                    }
                                    else if (sd.Quantite == 4)
                                    {
                                        nbSupp++;
                                        nbSupp++;
                                        nbSupp++;
                                        calculeTypeMidi++;
                                    }
                                    else if (sd.Quantite == 5)
                                    {
                                        nbSupp++;
                                        nbSupp++;
                                        nbSupp++;
                                        nbSupp++;
                                        calculeTypeMidi++;
                                    }
                                }
                                if (sd.Type == 7 || sd.Type == 8 || sd.Type == 9)
                                {
                                    if (sd.Quantite == 1)
                                    {
                                        calculeTypeSoir++;
                                    }
                                    else if (sd.Quantite == 2)
                                    {
                                        nbSupp++;
                                        calculeTypeSoir++;
                                    }
                                    else if (sd.Quantite == 3)
                                    {
                                        nbSupp++;
                                        nbSupp++;
                                        calculeTypeSoir++;
                                    }
                                }
                                if (sd.Type == -1)
                                {
                                    if (sd.Quantite == 1 || sd.Quantite == -1)
                                    {
                                        nbBag++;
                                    }
                                }
                            }
                        }

                        if (calculeTypeMidi == 3)
                        {
                            nbMidi++;
                        }
                        if (calculeTypeSoir == 3)
                        {
                            nbSoir++;
                        }
                        calculeTypeMidi = 0;
                        calculeTypeSoir = 0;
                    }

                    if (nbSupp > 0 || nbMidi > 0 || nbSoir > 0 || nbBag > 0)
                    {
                        dynamic detailSemaine = new
                        {
                            Semaine = semaine,
                            Supp = nbSupp,
                            Midi = nbMidi,
                            Soir = nbSoir,
                            Bag = nbBag,
                        };
                        detailPersonne.Semaines.Add(detailSemaine);
                    }

                    nbMidi = 0;
                    nbSoir = 0;
                    nbBag = 0;
                    nbSupp = 0;
                    couleur = (255, 255, 255);

                }
                if (detailPersonne.Semaines.Count > 0) data.Add(detailPersonne);
            }
            return data;
        }

        private static void PrintPage()
        {
            getDocument();

            // Echelle de la partie des compositions (100 par défaut)
            var echelle = 100;

            // Valeurs de Y
            var Y_AU_PLUS_HAUT = 100;
            var Y_AU_PLUS_BAS = 0;

            // Valeurs de X
            var X_AU_PLUS_A_GAUCHE = 0;
            var X_AU_PLUS_A_DROITE = 100;

            // Décalage x et y de la partie des compositions (0 par défaut)
            var xDecalage = 0;
            var yDecalage = 0;

            // Paramètres pour le header

            double y = 100;
            double x = 0;

            // #### CADRE ####
            // droite et gauche
            drawLine(getX(0), getX(0), getY(100), getY(0));
            drawLine(getX(100), getX(100), getY(100), getY(0));
            // haut et bas
            drawLine(getX(100), getX(0), getY(y), getY(y));
            drawLine(getX(100), getX(0), getY(0), getY(0));

            // #### HEADER ####
            var hauteurDuHeader = 6;
            y -= hauteurDuHeader;
            drawLine(getX(0), getX(100), getY(y), getY(y));
            drawText(BOLD, 10, getMiddelofXBetweenTowPoint(0, 100, BOLD, "Header", 10), getMiddelofYBetweenTowPoint(100, y, BOLD, 10), "Header", 0, 0, 0);

            // #### SUBHEADER ####
            var hauteurDuSubHeader = 4;
            y -= hauteurDuSubHeader;
            drawLine(getX(0), getX(100), getY(y), getY(y));

            // #### COLONNE CLIENT ####
            var largeurDuClient = 40;
            x += largeurDuClient;
            drawLine(getX(x), getX(x), getY(y + hauteurDuSubHeader), getY(0));
            drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(0, x, BOLD, "Client", 10), getMiddelofYBetweenTowPoint(y + hauteurDuSubHeader, y, NORMAL, 10), "Client", 0, 0, 0);

            // #### COLONNE AUTRE ####
            var largeurDeAutre = 40;
            x += largeurDeAutre;
            drawLine(getX(x), getX(x), getY(y + hauteurDuSubHeader), getY(0));
            drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(x - largeurDeAutre, x, BOLD, "Autre", 10), getMiddelofYBetweenTowPoint(y + hauteurDuSubHeader, y, NORMAL, 10), "Autre", 0, 0, 0);

            // #### COLONNE TOTAL ####
            var largeurTotaux = 20;
            x += largeurTotaux;
            drawLine(getX(x), getX(x), getY(y + hauteurDuSubHeader), getY(0));
            drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(x - largeurTotaux, x, BOLD, "Totaux", 10), getMiddelofYBetweenTowPoint(y + hauteurDuSubHeader, y, NORMAL, 10), "Totaux", 0, 0, 0);

            var nombreLignesMax = 10;
            double hauteurLigne = y / nombreLignesMax;

            List<string> listOfStrings = new List<string> {
        "Bonjour",
        "a",
        "tous",
        "je",
        "suis",
        "un",
        "simple",
        "client"
      };
            // #### On remplit les cases
            foreach (string theString in listOfStrings)
            {
                x = 0;
                y -= hauteurLigne;
                drawLine(getX(X_AU_PLUS_A_DROITE), getX(X_AU_PLUS_A_GAUCHE), getY(y), getY(y));
                x += largeurDuClient;
                // Nom client
                PrintTextBetweenTowPoint(theString, getX(0), getX(x), getY(y + hauteurLigne / 2), 10, NORMAL, 0, 0, 0);
                x += largeurDeAutre;
                // Autre case
                PrintTextBetweenTowPoint("autre", getX(x - largeurDeAutre), getX(x), getY(y + hauteurLigne / 2), 10, NORMAL, 0, 0, 0);
                x += largeurTotaux;
                // Totaux client
                PrintTextBetweenTowPoint("totaux", getX(x - largeurTotaux), getX(x), getY(y + hauteurLigne / 2), 10, NORMAL, 0, 0, 0);
            }

            //Close de la page
            contentStream.close();
        }

        private static void PrintTextBetweenTowPoint(String str, double x, double maxX, double y, double fontSize, PDType1Font font, int R, int G, int B)
        {
            double width = (font.getStringWidth(str) / 1000 * fontSize);

            if (x + width < maxX)
            {
                drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFFacturation.maxX * 100), (maxX / CreatePDFFacturation.maxX * 100), font, str, fontSize), y, str, R, G, B);
            }
            else
            {
                StringBuilder line = new StringBuilder();
                String[] words = str.Split(' ');
                var strings = new List<string>();

                if (words.Length > 1)
                {
                    foreach (String word in words)
                    {
                        if (word == null)
                        {
                            return;
                        }
                        if (word.Length < 1)
                        {
                            continue;
                        }

                        double wordSize;

                        if (line.Length > 0)
                        {
                            wordSize = font.getStringWidth(line + " " + word) / 1000 * fontSize;
                        }
                        else
                        {
                            wordSize = font.getStringWidth(word) / 1000 * fontSize;
                        }

                        if (x + wordSize < maxX)
                        {
                            if (line.Length > 0)
                            {
                                line.Append(" ").Append(word);
                            }
                            else
                            {
                                line = new StringBuilder(word);
                            }
                        }
                        else
                        {
                            if (line.Length > 0)
                            {
                                strings.Add(line.ToString());
                            }
                            line = new StringBuilder(word);
                        }
                    }

                    if (!strings[strings.Count - 1].Equals(line.ToString()))
                    {
                        strings.Add(line.ToString());
                    }

                    double height = ((font.getFontDescriptor().getCapHeight()) / 1000 * fontSize / 2) * strings.Count;

                    string maxLength = "";

                    foreach (var s in strings)
                    {
                        if (s.Length > maxLength.Length) maxLength = s;
                    }

                    drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFFacturation.maxX * 100), (maxX / CreatePDFFacturation.maxX * 100), font, maxLength, fontSize), getY((y / maxY * 100)) + height, strings, R, G, B);
                }
            }
        }

        /**
                 * Function pour créer une ligne entre quatre point
                 *
                 * @param startX double
                 * @param endX   double
                 * @param startY double
                 * @param endY   double
                 * @throws IOException ...
                 */
        private static void drawLine(double startX, double endX, double startY, double endY)
        {
            contentStream.moveTo((float)startX, (float)startY);
            contentStream.lineTo((float)endX, (float)endY);
            contentStream.stroke();
        }

        /**
                 * Function pour ecrire un text a une position
                 *
                 * @param font     PDFont
                 * @param fontSize double
                 * @param x        double
                 * @param y        double
                 * @param text     String
                 * @throws IOException ...
                 */
        private static void drawText(PDFont font, double fontSize, double x, double y, String text, int R, int G, int B)
        {

            //Begin the Content stream
            contentStream.beginText();

            //Setting the font to the Content stream
            contentStream.setFont(font, (float)fontSize);

            //Setting Color
            contentStream.setNonStrokingColor(R, G, B);

            //Setting the position for the line
            //TODO contentStream.newLineAtOffset((float) x, (float) y);
            contentStream.moveTextPositionByAmount((float)x, (float)y);

            //Adding text in the form of string
            //TODO contentStream.showText(text);
            contentStream.drawString(text);

            //Ending the content stream
            contentStream.endText();
        }

        /**
                 * Function pour ecrire un text a plusieur ligne a une position
                 *
                 * @param font     PDFont
                 * @param fontSize double
                 * @param x        double
                 * @param y        double
                 * @param text     String...
                 * @throws IOException ...
                 */
        private static void drawText(PDFont font, double fontSize, double x, double y, params String[] text)
        {
            int count = 0;
            foreach (var s in text)
            {
                drawText(font, fontSize, x, (y - ((fontSize + 2) * count)), s);
                count = count + 1;
            }
        }

        /**
                 * Function pour ecrire un text a plusieur ligne a une position
                 *
                 * @param font     PDFont
                 * @param fontSize double
                 * @param x        double
                 * @param y        double
                 * @param text     ArrayList Of String
                 * @throws IOException ...
                 */
        private static void drawText(PDFont font, double fontSize, double x, double y, List<String> text, int R, int G, int B)
        {
            int count = 0;
            foreach (String s in text)
            {
                drawText(font, fontSize, x, (y - ((fontSize + 2) * count)), s, R, G, B);
                count = count + 1;
            }
        }

        /**
                 * Récupération de Y en fonction d'un pourcentage
                 *
                 * @param percent double
                 * @return double
                 */
        private static double getY(double percent)
        {
            if (percent == 100)
            {
                return maxY - margin;
            }
            if (percent == 0)
            {
                return minY;
            }
            double retour = ((percent / 100) * maxY);
            return retour;
        }

        /**
                 * Récupération de X en fonction d'un pourcentage
                 *
                 * @param percent double
                 * @return double
                 */
        private static double getX(double percent)
        {
            if (percent == 100)
            {
                return maxX - margin;
            }
            if (percent == 0)
            {
                return minX;
            }
            return ((percent / 100) * maxX);
        }

        /**
                 * Demande a l'utilisateur le chemin d'enregistrement
                 *
                 * @return boolean si cela est valid
                 */
        private static bool getPath()
        {
            bool retval = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = namePdf;
            var res = fileDialog.ShowDialog();

            if (!res.HasValue) retval = false;
            else retval = res.Value;

            if (retval == true)
            {
                output = fileDialog.FileName;
            }
            return retval;
        }

        /**
                 * Création d'une page et définition du stream de la page
                 *
                 * @throws IOException ...
                 */
        private static void getDocument()
        {
            blankPage = new PDPage(new PDRectangle((float)maxX, (float)maxY));
            document.addPage(blankPage);

            contentStream = new PDPageContentStream(document, blankPage);
        }

        /**
                 * Récuperation du centre en pixel du text
                 *
                 * @param font     PDType1Font
                 * @param text     String
                 * @param fontSize float
                 * @return double
                 * @throws IOException ...
                 */
        private static double getMiddleOfText(PDType1Font font, String text, float fontSize)
        {
            return (font.getStringWidth(text) / 1000 * fontSize) / 2;
        }

        /**
                 * Récuperation du centre en pourcentage du text en fonction d'une position
                 *
                 * @param font     PDType1Font
                 * @param text     String
                 * @param fontSize float
                 * @return double
                 * @throws IOException ...
                 */
        private static double getMiddelOfX(double positionInPercent, PDType1Font font, String text, float fontSize)
        {
            return getX(positionInPercent) - getMiddleOfText(font, text, fontSize);
        }

        /**
                 * Récuperation de la largeur en pourcentage du text en fonction de deux position
                 *
                 * @param positionOneInPercent double
                 * @param positionTowInPercent double
                 * @param font                 PDType1Font
                 * @param text                 String
                 * @param fontSize             double
                 * @return double
                 * @throws IOException ...
                 */
        private static double getMiddelofXBetweenTowPoint(double positionOneInPercent, double positionTowInPercent, PDType1Font font, String text, double fontSize)
        {
            double start = getX(positionOneInPercent);
            double end = getX(positionTowInPercent);
            double center = (end - start) / 2;
            return (start + center) - getMiddleOfText(font, text, (float)fontSize);
        }

        /**
                 * Récuperation de la hauteur en pourcentage du text en fonction de deux position
                 *
                 * @param positionOneInPercent double
                 * @param positionTowInPercent double
                 * @param font                 PDType1Font
                 * @param fontSize             double
                 * @return double
                 */
        private static double getMiddelofYBetweenTowPoint(double positionOneInPercent, double positionTowInPercent, PDType1Font font, double fontSize)
        {
            double start = getY(positionOneInPercent);
            double end = getY(positionTowInPercent);
            double center = (end - start) / 2;
            return (start + center) - ((font.getFontDescriptor().getCapHeight()) / 1000 * fontSize / 2);
        }
    }
}