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
    class CreatePDFFacturationMSA
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

        private static PersonnesHelper personnesHelper = new PersonnesHelper();

        /**
                 * Function d'enter pour créer le PDF
                 *
                 * @param format  PDRectangle A3 ou A4
                 * @param semaine Integer Numéros de semaine
                 * @return boolean
                 * @throws Exception ...
                 */
        public static string Start(float width, float height, int annee)
        {

            //Récuperation du format de la page en fonction de A3 ou A4
            maxX = height - margin;
            maxY = width - margin;

            //Récupération des variable en relatison avec des pourcentage
            menuYTop = getY(100);
            menuYTopNoLivraison = getY(100);
            menuYTopNoDay = getY(99);
            menuYBottom = getY(1);
            Comment = null;
            Annee = annee;
            namePdf = "Facturation_MSA_" + annee + ".pdf";

            //Demande a l'utilisateur de choisir ou enregistrer    
            if (!getPath())
            {
                return "";
            }

            //Création du document
            document = new PDDocument();

            // print


            PrintPage();

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

        private static void PrintPages()
        {

           /* var data = GetData(nbrSemaines, Annee);
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
            }*/

        }

        private static void PrintPage()
        {
            getDocument();

            double y = 100;
            double x = 0;

            double debutHeader = 90;

            double espaceTexte = 2;

            y = debutHeader;
            PrintTextBetweenTowPoint("CONSEIL DEPARTEMENTAL DE LA CHARENTE MARITIME", getX(50), getX(100), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            PrintTextBetweenTowPoint("Service GSP - Direction de l'automonie", getX(50), getX(100), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            PrintTextBetweenTowPoint("85 Bouleward de la République CS 60003", getX(50), getX(100), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            PrintTextBetweenTowPoint("17076 LA ROCHELLE CEDEX 9", getX(50), getX(100), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            drawText(NORMAL, 10, getX(0), getY(y), "49 route de Meursac", 0, 0, 0);
            //PrintTextBetweenTowPoint("49 route de Meursac", getX(0), getX(30), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            drawText(NORMAL, 10, getX(0), getY(y), "17600 SABLONCEAUX - Tél. 05.46.02.83.62", 0, 0, 0);
            //PrintTextBetweenTowPoint("17600 SABLONCEAUX - Tél. 05.46.02.83.62", getX(0), getX(30), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            drawText(NORMAL, 10, getX(0), getY(y), "SIRET : 878 657 361 00018", 0, 0, 0);
            //PrintTextBetweenTowPoint("SIRET : 878 657 361 00018", getX(0), getX(30), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            drawText(NORMAL, 10, getX(0), getY(y), "APE : 5621Z", 0, 0, 0);
            //PrintTextBetweenTowPoint("APE : 5621Z", getX(0), getX(30), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            drawText(NORMAL, 10, getX(0), getY(y), "E-mail : eric.benard17@orange.fr", 0, 0, 0);
            y -= espaceTexte;
            //PrintTextBetweenTowPoint("E-mail : eric.benard17@orange.fr", getX(0), getX(30), getY(y), 10, NORMAL, 0, 0, 0);
            drawText(NORMAL, 10, getX(0), getY(y), "Internet : www.traiteur-ericbenard.com", 0, 0, 0);
            y -= espaceTexte;
            //PrintTextBetweenTowPoint("Internet : www.traiteur-ericbenard.com", getX(0), getX(30), getY(y), 10, NORMAL, 0, 0, 0);
            y -= espaceTexte;
            PrintTextBetweenTowPoint("FACTURE APA PORTAGE DE REPAS", getX(25), getX(75), getY(y), 10, BOLD, 0, 0, 0);
            y -= espaceTexte;
            PrintTextBetweenTowPoint("Juillet 2020 (variable)", getX(25), getX(75), getY(y), 10, BOLD, 0, 0, 0);
            drawText(NORMAL, 10, getX(0), getY(y), "Dat facture ! 30/06/2020", 0, 0, 0);
            //PrintTextBetweenTowPoint("Dat facture ! 30/06/2020", getX(25), getX(75), getY(y), 10, BOLD, 0, 0, 0);
            y -= espaceTexte;

            // horizontales
            drawLine(getX(0), getX(100), getY(y), getY(y));
            drawLine(getX(0), getX(100), getY(0), getY(0));

            //verticales
            drawLine(getX(0), getX(0), getY(y), getY(0));
            drawLine(getX(100), getX(100), getY(y), getY(0));

            // header

            double hauteurHeaderTableau = 4;
            double largeurBeneficiaire = 100 / 3;
            double largeurDroitMsa = 100 / 3;
            double largeurCalculMontant = 100 / 3;
            double hauteurDeuxiemeHeaderTableau = 12;
            double largeurNom = largeurBeneficiaire / 2;
            double largeurPrenom = largeurBeneficiaire / 2;
            double largeurNbrPortage = largeurBeneficiaire / 4;
            double largeurMontantMensuel = largeurBeneficiaire / 4;
            double largeurRepasMidi = largeurBeneficiaire / 4;
            double largeurRepasSoir = largeurBeneficiaire / 4;
            double largeurBag = largeurCalculMontant / 3;
            double largeurNbrPortageEff = largeurCalculMontant / 3;
            double largeurMontantMSADepar = largeurCalculMontant / 3;

            double hauteurClient = 3;
            // PREMIER HEADER BENEFICIAIRES / DROITS MSA / CALCUL DU MONTANT

            // verticales
            x = largeurBeneficiaire;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("BENEFICIAIRES", getX(0), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurDroitMsa;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("DROITS MSA", getX(x - largeurBeneficiaire), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurCalculMontant;
            PrintTextBetweenTowPoint("CALCUL DU MONTANT", getX(x - largeurBeneficiaire), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);

            //horizontale
            y -= hauteurHeaderTableau;
            drawLine(getX(0), getX(100), getY(y), getY(y));

            // DEUXIEME HEADER
            
            // verticales
            x = largeurNom;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("Nom", getX(0), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurPrenom;
            PrintTextBetweenTowPoint("Prénom", getX(x - largeurPrenom), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);

            x += largeurNbrPortage;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("Nombre de portage maximum mensuel", getX(x - largeurNbrPortage), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurMontantMensuel;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("Monstant mensuel", getX(x - largeurMontantMensuel), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurRepasMidi;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("Repas midi", getX(x - largeurRepasMidi), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurRepasSoir;
            PrintTextBetweenTowPoint("Repas soir", getX(x - largeurRepasSoir), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
           
            x += largeurBag;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("Bag", getX(x - largeurBag), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurNbrPortageEff;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("Nombre de portage effectué", getX(x - largeurNbrPortageEff), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurMontantMSADepar;
            drawLine(getX(x), getX(x), getY(y), getY(0));
            PrintTextBetweenTowPoint("Montant APA dû par le département", getX(x - largeurMontantMSADepar), getX(x), getMiddelofYBetweenTowPoint(y, y - hauteurDeuxiemeHeaderTableau, BOLD, 10), 10, BOLD, 0, 0, 0);
            //horizontale
            y -= hauteurDeuxiemeHeaderTableau;
            drawLine(getX(0), getX(100), getY(y), getY(y));

            // DONNEES
            // horizontale
            y -= hauteurClient;
            drawLine(getX(0), getX(100), getY(y), getY(y));
            // verticale
            x = largeurNom;
            PrintTextBetweenTowPoint("ARLUISON", getX(x - largeurNom), getX(x), getMiddelofYBetweenTowPoint(y, y + hauteurClient, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurPrenom;
            PrintTextBetweenTowPoint("Georges", getX(x - largeurPrenom), getX(x), getMiddelofYBetweenTowPoint(y, y + hauteurClient, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurNbrPortage;
            PrintTextBetweenTowPoint("19", getX(x - largeurNbrPortage), getX(x), getMiddelofYBetweenTowPoint(y, y + hauteurClient, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurMontantMensuel;
            PrintTextBetweenTowPoint("", getX(x - largeurMontantMensuel), getX(x), getMiddelofYBetweenTowPoint(y, y + hauteurClient, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurRepasMidi;
            PrintTextBetweenTowPoint("31", getX(x - largeurRepasMidi), getX(x), getMiddelofYBetweenTowPoint(y, y + hauteurClient, BOLD, 10), 10, BOLD, 0, 0, 0);
            x += largeurRepasSoir;
            PrintTextBetweenTowPoint("26", getX(x - largeurRepasSoir), getX(x), getMiddelofYBetweenTowPoint(y, y + hauteurClient, BOLD, 10), 10, BOLD, 0, 0, 0);





            // ligne horizontal sub header
            /*  y -= hauteurSubHeader;
              drawLine(getX(0), getX(customMaxX), getY(y), getY(y));
              drawText(NORMAL, 10, getMiddelofXBetweenTowPoint(0, x, BOLD, "NOMS", 10), getMiddelofYBetweenTowPoint(y + hauteurSubHeader, y, NORMAL, 10), "NOMS", 0, 0, 0);*/


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
            List<Personne> personnes = PersonneDAO.GetPersonnesWithTourneeNotAPANotMSA(Tournee.ID, db);

            foreach (var p in personnes)
            {

                if (p.Couleur != null)
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

                        for (int repas = -1; repas < 9; repas++)
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

     

        private static void PrintTextBetweenTowPoint(String str, double x, double maxX, double y, double fontSize, PDType1Font font, int R, int G, int B)
        {
            double width = (font.getStringWidth(str) / 1000 * fontSize);

            if (x + width < maxX)
            {
                drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFFacturationMSA.maxX * 100), (maxX / CreatePDFFacturationMSA.maxX * 100), font, str, fontSize), y, str, R, G, B);
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

                    drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFFacturationMSA.maxX * 100), (maxX / CreatePDFFacturationMSA.maxX * 100), font, maxLength, fontSize), getY((y / maxY * 100)) + height, strings, R, G, B);
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