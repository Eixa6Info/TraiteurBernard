
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
using System.Text;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;


namespace TraiteurBernardWPF.PDF
{
    public static class CreatePDFJambon
    {
        /**
         * Variable pour le placement
         */
        private static float margin = 10;

        private static double minX = margin;
        private static double minY = margin;
        private static double columnSpace = 12.5;//12,5%
        private static double choiceSize = 25;//25Px

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

            namePdf = "saisies_Cuisine_Jambon_" + semaine + "_" + annee + ".pdf";

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
            PrintJambon(annee, semaine);

        }

      
        private static void PrintJambon(int annee, int semaine)
        {
            //Création d'une nouvelle page
            newPageAndPrintAllJambon(annee, semaine);

            //Ajout de toute les infirmation
            printJours();

            //Close de la page
            contentStream.close();
        }

        /**
        * Function identique a toute les page du midi
        * si l'argument est true, on print les saies
        * sinon on print les menu
        * @throws IOException ...
        */
        private static void newPageAndPrintAllJambon(int annee, int semaine)
        {
            //Récup du document
            getDocument();

            //création du cadre
            printCadre();

            //Création de toute les ligne
            PrintLines();

            //Création de toute les column
            printColumn();

            //Ajout des description a gauche du tableaux
            printDescLine();

            //Ajout des menus ou des saisies dans les case
            printSaisieJambon(annee, semaine);

        }

        /**
         * Print sur le PDF des jours pour la page 1 (Tourner 1)
         *
         * @throws IOException ...
         */
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Wednesday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }
        public static DateTime FirstDateOfWeekContreTourneeISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

        private static void PrintCenter(printJourOuSoir fct)
        {
           
            //variavle réutiliser
            String text;
            int column;
            float fontSize = 10;

            text = "VILLE 1";
            column = 1;           
            //Print de l'information au centre de la case
            fct(text, fontSize, column);

            text = "VILLE 2";
            column = 2;            
            fct(text, fontSize, column);

            text = "CT";
            column = 3;            
            fct(text, fontSize, column);

            text = "MARENNES";
            column = 4;
            fct( text, fontSize, column);

            text = "";
            column = 5;            
            fct(text, fontSize, column);

        }

        private static void printJours()
        {
            PrintCenter(printCenterDay);
        }


        delegate void printJourOuSoir(String text, float fontSize, int column);


        /**
         * Print des date de consomation des plats
         *
         * @param cal      Calendar
         * @param sdf      SimpleDateFormat
         * @param text     String
         * @param fontSize float
         * @param column   int
         * @throws IOException ...
         */
        private static void printCenterDay( String text, float fontSize, int column)
        {
            fontSize = 10;
            double max = ((columnSpace * column) + (columnSpace - choiceSize / maxX * 100));
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column + 5, max, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(94, 97, BOLD, fontSize), text);
            //drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column, max, NORMAL, str, fontSize), getMiddelofYBetweenTowPoint(93, 96, NORMAL, fontSize), str);
        }

        /**
         * Fonction, pout ptiny les plats et quantités de toutes les saises
        */
        private static void printSaisieJambon(int annee, int semaine, bool composition = false)
        {
            BaseContext db = new BaseContext();
            int jour = 1;
            int a = 92;
            int b = 0;
            int c = 102;
            int d = 0;
            int e = 102;
            var perOld = "";
            var newPer = "";
            int qtt = 0;
            int qttct = 0;
            int leJour = 0;
            // Pour tous les jours on récupère toutes les saisies et toutes les saisies data 
            // de ce même jour
            for (jour = 1; jour < 8; jour++)
            {
                double column = columnSpace * jour;
                List<Saisie> saisiesList = new List<Saisie>();
                List<Saisie> saisiesListVille1;
                List<Saisie> saisiesListVille2;
                List<Saisie> saisiesListCT;
                List<Saisie> saisiesListMarennes;
                

                saisiesListVille1 = SaisieDAO.getAllFromYearWeekDayForTournee("ville 1", "", annee, semaine, jour, db);
                saisiesListVille2 = SaisieDAO.getAllFromYearWeekDayForTournee("ville 2", "", annee, semaine, jour, db);
                saisiesListCT = SaisieDAO.getAllFromYearWeekDayForTournee("contre-tournée", "", annee, semaine, jour, db);
                saisiesListMarennes = SaisieDAO.getAllFromYearWeekDayForTournee("Marennes", "", annee, semaine, jour, db);


                foreach (var v1 in saisiesListVille1)
                {
                    saisiesList.Add(v1);
                }
                foreach (var v2 in saisiesListVille2)
                {
                    saisiesList.Add(v2);
                }
                foreach (var ct in saisiesListCT)
                {
                    saisiesList.Add(ct);
                }
                foreach (var mn in saisiesListMarennes)
                {
                    saisiesList.Add(mn);
                }

                List<Saisie> personnes = new List<Saisie>();
                // Les données des saisies
                List<SaisieData> saisiesDatas = new List<SaisieData>();

                // On remplit les données des saisies
                foreach (Saisie saisie in saisiesList)
                {
                    foreach (SaisieData saisieData in saisie.data)
                    {
                        if (saisieData.Type == 4) 
                        {
                            saisiesDatas.Add(saisieData);

                            if (saisieData.Quantite != 0)
                            {  
                                personnes.Add(saisie);
                            }
                        }
                    }
                }
                                               
                // Dictionnaire des formules (ex 2 * frites, 1 * salade, etc)
                Dictionary<string, int> repasIntituleQuantite = new Dictionary<string, int>();

                // Pour toutes les données des saisies du jours et par repas 
                
                foreach (SaisieData sd in SaisieDataDAO.SortByTypeFromList(4, saisiesDatas))
                {
                  
                    string libelle = sd.Libelle;
                    int quantite = sd.Quantite;
                    int tournee = sd.Saisie.Tournee.ID;
                    // On additionne les quantité des repas déjà existant, sinon on l'ajoute dans le dictionnaire
                    if (repasIntituleQuantite.ContainsKey(libelle))
                    {
                        repasIntituleQuantite[libelle] += quantite;
                    }
                    else
                    {
                        repasIntituleQuantite.Add(libelle, quantite);
                    }
                    
                }
                
                string txt = "";

                // Ecriture des repas sur le PDF              
                foreach (KeyValuePair<string, int> entry in repasIntituleQuantite)
                {
                    if (!String.IsNullOrEmpty(entry.Key))
                    {     
                        txt = entry.Key;
                    }
                }
                float fontSize = 9;
                c = e - 12;
                e = e - 12;
                qtt = 0;
                qtt += qttct;
                qttct = 0;
                foreach (Saisie s in personnes)
                {
                    
                    var per = s.Personne.Nom;
                    newPer = s.Personne.Tournee.Nom;

                    if (perOld == newPer && leJour >= jour)
                    {
                        c = d;
                    }
                    else
                    {
                        c = e;
                    }
                    leJour = jour;
                    if (s.Personne.Tournee.Nom == Properties.Resources.Ville1)
                    {
                        d = c - 2;
                        drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(25, columnSpace, NORMAL, per, fontSize), getMiddelofYBetweenTowPoint(d, c, NORMAL, fontSize), per);
                        qtt++;
                    }
                    else if (s.Personne.Tournee.Nom == Properties.Resources.Ville2)
                    {
                        d = c - 2;
                        drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(50, columnSpace, NORMAL, per, fontSize), getMiddelofYBetweenTowPoint(d, c, NORMAL, fontSize), per);
                        qtt++;
                    }
                    else if (s.Personne.Tournee.Nom == Properties.Resources.CT)
                    {
                        d = c - 2;
                        drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(75, columnSpace, NORMAL, per, fontSize), getMiddelofYBetweenTowPoint(d - 12 , c - 12 , NORMAL, fontSize), per); 
                        qttct++;
                    }
                    else if (s.Personne.Tournee.Nom == Properties.Resources.Marennes)
                    {
                        d = c - 2;
                        drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(100.5, columnSpace, NORMAL, per, fontSize), getMiddelofYBetweenTowPoint(d, c, NORMAL, fontSize), per);            
                        qtt++;
                    }
                    perOld = newPer;
                }
                
                b = a - 12;
               
                drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, txt, fontSize), getMiddelofYBetweenTowPoint(b, a, BOLD, fontSize), txt);
                drawText(BOLD, 16, getMiddelofXBetweenTowPoint(175, columnSpace, BOLD, qtt.ToString(), 16), getMiddelofYBetweenTowPoint(b, a, BOLD, 16), qtt.ToString());

                a = b;
            }
            db.Dispose();
        }

        private static void PrintTextBetweenTowPoint(String str, double x, double maxX, double y, double fontSize, PDType1Font font)
        {
            double width = (font.getStringWidth(str) / 1000 * fontSize);

            if (x + width < maxX)
            {
                drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFJambon.maxX * 100), (maxX / CreatePDFJambon.maxX * 100), font, str, fontSize), y, str);
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

                    drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFJambon.maxX * 100), (maxX / CreatePDFJambon.maxX * 100), font, maxLength, fontSize), getY((y / maxY * 100)) + height, strings);
                }
            }
        }


        private static void PrintLines()
        {
            //Bottom Lundi
            drawLine(getX(0), getX(100), getY(92), getY(92));

            //Bottom Mardi
            drawLine(getX(0), getX(100), getY(80), getY(80));

            //Bottom Mercredi
            drawLine(getX(0), getX(100), getY(68), getY(68));

            //Bottom Jeudi
            drawLine(getX(0), getX(100), getY(56), getY(56));

            //Bottom Vendredi
            drawLine(getX(0), getX(100), getY(44), getY(44));

            //Bottom Samedi
            drawLine(getX(0), getX(100), getY(32), getY(32));

            //Bottom Dimanche
            drawLine(getX(0), getX(100), getY(20), getY(20));

            //Bottom Dimanche/Lundi ct
            drawLine(getX(37.5), getX(50), getY(6.5), getY(6.5));
            drawLine(getX(0), getX(12.5), getY(6.5), getY(6.5));

        }



        /**
         * Print de toute les column
         *
         * @throws IOException ...
         */
        private static void printColumn()
        {
            for (int i = 0; i < 7; i++)
            {
                //Left bar Jour 1
                drawLine(getX(columnSpace * (1 + i)), getX(columnSpace * (1 + i)), menuYTopNoLivraison, menuYBottom);

                //Left Bar Jour 1 choice
               // drawLine(getX(columnSpace * (2 + i)) - choiceSize, getX(columnSpace * (2 + i)) - choiceSize, menuYTopNoLivraison, menuYBottom);
            }

            //drawLine(getX(columnSpace / 2), getX(columnSpace / 2), getY(80), getY(44));
        }



        /**
         * Print des description et du fromage a gauche du tableaux
         *
         * @throws IOException ...
         */
        private static void printDescLine()
        {
            //Baguettes
            String text;
            float fontSize = 9;




            text = "Lundi".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(83, 95, BOLD, fontSize), text);

            text = "Mardi".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(71, 83, BOLD, fontSize), text);

            text = "Mercredi".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(59, 71, BOLD, fontSize), text);

            text = "Jeudi".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(47, 59, BOLD, fontSize), text);

            text = "Vendredi".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(35, 47, BOLD, fontSize), text);

            text = "Samedi".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(23, 35, BOLD, fontSize), text);

            text = "Dimanche".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(11, 23, BOLD, fontSize), text);

            text = "Lundi CT".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(1, 9, BOLD, fontSize), text);

        }


        /**
         * Function de print pour le cadre (quatre ligne)
         *
         * @throws IOException ...
         */
        private static void printCadre()
        {
            //Left Line
            drawLine(getX(1), getX(1), getY(100), getY(1));

            //Right Line
            drawLine(getX(100), getX(100), getY(1), getY(100));

            //Top Line
            //  drawLine(getX(0), getX(100), getY(100), getY(100));

            //Bottom Line
            drawLine(getX(0), getX(100), getY(1), getY(1));
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
        private static void drawText(PDFont font, double fontSize, double x, double y, String text)
        {

            //Begin the Content stream
            contentStream.beginText();

            //Setting the font to the Content stream
            contentStream.setFont(font, (float)fontSize);

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
        private static void drawText(PDFont font, double fontSize, double x, double y, List<String> text)
        {
            int count = 0;
            foreach (String s in text)
            {
                drawText(font, fontSize, x, (y - ((fontSize + 2) * count)), s);
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
