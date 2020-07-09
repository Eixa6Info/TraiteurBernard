
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
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.PDF
{
    class CreatePDF5FeuillesMarennes
    {
             /**
         * Variable pour le placement
         */
        private static float margin = 10;

        private static double minX = margin;
        private static double minY = margin;
        private static double columnSpace = 32;//12,5%
        private static double choiceSize = 51;//25Px

        private static PDType1Font BOLD = PDType1Font.HELVETICA_BOLD;
        private static PDType1Font NORMAL = PDType1Font.HELVETICA;
        private static PDType1Font OBLIQUE = PDType1Font.HELVETICA_OBLIQUE;
        private static double maxX;
        private static double maxY;
        private static double menuYTop;
        private static double menuYTopNoLivraison;
        private static double menuYTopNoDay;
        private static double menuYBottom;

        private static int Semaine;
        private static bool calculSemaine = true;
        private static string namePdf;
        private static string output;
        private static PDDocument document;
        private static PDPage blankPage;
        private static PDPageContentStream contentStream;

        /**
         * Function d'enter pour créer le PDF
         *
         * @param format  PDRectangle A3 ou A4
         * @param semaine Integer Numéros de semaine
         * @return boolean
         * @throws Exception ...
         */
        public static string Start(float width, float height, int semaine, int annee)
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

            namePdf = "saisies_Cuisine_5_feuilles_Marennes_" + semaine + "_" + annee + ".pdf";

            //Demande a l'utilisateur de choisir ou enregistrer    
            if (!getPath())
            {
                return "";
            }

            //Création du document
            document = new PDDocument();



            // print
            print(annee, semaine);

            //Saving the document
            document.save(output);

            //Closing the document
            document.close();

            return output;
        }

        /**
         * Function pour créer les 3 pages du midi
         *
         * @throws IOException ...
         */
        private static void print(int annee, int semaine)
        {
            // On créer les 5 pages (jour 1,2,3,4,5)
            for(int i = 1; i < 6; i++)
            {
                PrintPage(annee, semaine,i);

            }
        }

        private static void PrintPage(int annee, int semaine, int jour)
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
            var hauteurDuHeader = 4;

            // #### CADRE ####
            drawLine(getX(X_AU_PLUS_A_GAUCHE), getX(X_AU_PLUS_A_GAUCHE), getY(Y_AU_PLUS_HAUT - hauteurDuHeader), getY(Y_AU_PLUS_BAS));
            drawLine(getX(X_AU_PLUS_A_DROITE), getX(X_AU_PLUS_A_DROITE), getY(Y_AU_PLUS_HAUT - hauteurDuHeader), getY(Y_AU_PLUS_BAS));
            drawLine(getX(X_AU_PLUS_A_DROITE), getX(X_AU_PLUS_A_GAUCHE), getY(Y_AU_PLUS_BAS), getY(Y_AU_PLUS_BAS));

            // #### HEADER ####
            // - les deux lignes horizontales
            drawLine(getX(X_AU_PLUS_A_GAUCHE), getX(X_AU_PLUS_A_DROITE), getY(Y_AU_PLUS_HAUT), getY(Y_AU_PLUS_HAUT));
            drawLine(getX(X_AU_PLUS_A_GAUCHE), getX(X_AU_PLUS_A_DROITE), getY(Y_AU_PLUS_HAUT - hauteurDuHeader), getY(Y_AU_PLUS_HAUT - hauteurDuHeader));
            // - les deux lignes verticales
            drawLine(getX(X_AU_PLUS_A_GAUCHE), getX(X_AU_PLUS_A_GAUCHE), getY(Y_AU_PLUS_HAUT), getY(Y_AU_PLUS_HAUT - hauteurDuHeader));
            drawLine(getX(X_AU_PLUS_A_DROITE), getX(X_AU_PLUS_A_DROITE), getY(Y_AU_PLUS_HAUT), getY(Y_AU_PLUS_HAUT - hauteurDuHeader));
            // - le texte
            var ge = "jour - " + jour + " - semaine " + semaine + " - année " + annee + "    MARENNES";
            drawText(BOLD, 10, getMiddelofXBetweenTowPoint(X_AU_PLUS_A_GAUCHE, X_AU_PLUS_A_DROITE, BOLD, ge, 10), getMiddelofYBetweenTowPoint(Y_AU_PLUS_HAUT, Y_AU_PLUS_HAUT - hauteurDuHeader, BOLD, 10), ge, 0, 0, 0);

            // #### COLONNE DES QUANTITES ####
            // - les deux lignes verticales
            drawLine(getX(50 * echelle / 100 + xDecalage), getX(50 * echelle / 100 + xDecalage), getY(Y_AU_PLUS_HAUT - hauteurDuHeader), getY(Y_AU_PLUS_BAS));
            drawLine(getX(60 * echelle / 100 + xDecalage), getX(60 * echelle / 100 + xDecalage), getY(Y_AU_PLUS_HAUT - hauteurDuHeader), getY(Y_AU_PLUS_BAS));

            // Les données des saisies
            BaseContext db = new BaseContext();
         
            // Récupération de toutes les informations pour générer les compositions
            List<Saisie> saisiesList = new List<Saisie>();
            List<Saisie> saisiesListCompo = new List<Saisie>();
            int nomDeJoursDans1Jour = jour == 5 ? 3 : 1;
            for(int i = jour; i < jour + nomDeJoursDans1Jour; i++)
            {
               /* List<Saisie> saisiesListVille1 = SaisieDAO.getAllFromYearWeekDayForTournee("ville 1", "", annee, semaine, i, db);
                List<Saisie> saisiesListVille2 = SaisieDAO.getAllFromYearWeekDayForTournee("ville 2", "", annee, semaine, i, db);
                List<Saisie> saisiesListCT = SaisieDAO.getAllFromYearWeekDayForTournee("contre-tournée", "", annee, semaine, i, db);*/
                List<Saisie> saisiesListMarennes = SaisieDAO.getAllFromYearWeekDayForTournee("Marennes", "", annee, semaine, i, db);
              /*  saisiesList.AddRange(saisiesListVille1);
                saisiesList.AddRange(saisiesListVille2);
                saisiesList.AddRange(saisiesListCT);*/
                saisiesList.AddRange(saisiesListMarennes);
            }
            

            Random random = new Random();

            var espacementEntreTexte = 2;
            var yDesTextes = 100 - hauteurDuHeader - espacementEntreTexte;
            
            // Affichage des compositions
            foreach (Saisie saisie in saisiesList)
            {
                foreach (SaisieData saisieData in saisie.data)
                {
                    int index = 0;
                    if (saisieData.Modifie == true)
                    {
                        var r = random.Next(190);
                        var g = random.Next(190);
                        var b = random.Next(190);
                        var nomPrenom = saisieData.Saisie.Personne.Nom + " " + saisieData.Saisie.Personne.Prenom;
                        // Composition
                        drawText(BOLD, 10, getMiddelofXBetweenTowPoint(X_AU_PLUS_A_GAUCHE * echelle / 100 + xDecalage, 50 * echelle / 100 + xDecalage, BOLD, saisieData.Libelle, 10), getMiddelofYBetweenTowPoint(yDesTextes, yDesTextes, BOLD, 10), saisieData.Libelle, r, g, b);
                        // Quantité
                        drawText(BOLD, 10, getMiddelofXBetweenTowPoint(50 * echelle / 100 + xDecalage, 60 * echelle / 100 + xDecalage, BOLD, saisieData.Quantite.ToString(), 10), getMiddelofYBetweenTowPoint(yDesTextes, yDesTextes, BOLD, 10), saisieData.Quantite.ToString(), r, g, b);
                        // Personne
                        drawText(BOLD, 10, getMiddelofXBetweenTowPoint(60 * echelle / 100 + xDecalage, X_AU_PLUS_A_DROITE * echelle / 100 + xDecalage, BOLD, nomPrenom, 10), getMiddelofYBetweenTowPoint(yDesTextes, yDesTextes, BOLD, 10), nomPrenom, r, g, b);
                        yDesTextes -= espacementEntreTexte;
                    }

                }
            }
          

            //Close de la page
            contentStream.close();
        }


        delegate void printJourOuSoir(DateTime dt, DateTimeFormatInfo sdf, String text, float fontSize, int column);


        private static void PrintTextBetweenTowPoint(String str, double x, double maxX, double y, double fontSize, PDType1Font font ,int R, int G, int B)
        {
            double width = (font.getStringWidth(str) / 1000 * fontSize);

            if (x + width < maxX)
            {
                drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDF5FeuillesMarennes.maxX * 100), (maxX / CreatePDF5FeuillesMarennes.maxX * 100), font, str, fontSize), y, str, R, G, B);
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

                    drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDF5FeuillesMarennes.maxX * 100), (maxX / CreatePDF5FeuillesMarennes.maxX * 100), font, maxLength, fontSize), getY((y / maxY * 100)) + height, strings, R, G, B);
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
                drawText(font, fontSize, x, (y - ((fontSize + 2) * count)), s,R,G,B);
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
