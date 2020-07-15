
using com.sun.org.apache.xerces.@internal.impl.dtd.models;
using Microsoft.Win32;
using org.apache.pdfbox;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.common;
using org.apache.pdfbox.pdmodel.edit;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.pdmodel.graphics;
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
    public static class CreatePDFCuisine 
    {
        /**
         * Variable pour le placement
         */
        private static float margin = 10;

        private static double minX = margin;
        private static double minY = margin;
        private static double columnSpace = 12.5;//12,5%
        private static double choiceSize = 45;//25Px

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
                
            namePdf = "saisies_Cuisine_" + semaine + "_" + annee + ".pdf";
            
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
            PrintCuisine(annee, semaine);

        }

        private static void PrintCuisine(int annee, int semaine)
        {
            //Création d'une nouvelle page
            newPageAndPrintAll(annee, semaine);

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
        private static void newPageAndPrintAll(int annee, int semaine)
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
            printSaisie(annee, semaine, false);

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
            DateTime today = DateTime.Today;
            DateTime laDate;
     
            laDate = FirstDateOfWeekISO8601(today.Year, Semaine);

            var sdf = new DateTimeFormatInfo();

            //variavle réutiliser
            String text;
            int column;
            float fontSize = 10;

            //Print du Lundi
            //Text a afficher
            text = "LUNDI";

            //Column n°
            column = 1;
            laDate = laDate.AddDays(1);
            //Print de l'information au centre de la case
            fct(laDate, sdf, text, fontSize, column);

            //Print du MARDI
            text = "MARDI";
            column = 2;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            //Print du MERCREDI
            text = "MERCREDI";
            column = 3;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            //Print du JEUDI
            text = "JEUDI";
            column = 4;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            //Print du VENDREDI
            text = "VENDREDI";
            column = 5;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            //Print du SAMEDI
            text = "SAMEDI";
            column = 6;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            //Print du DIMANCHE
            text = "DIMANCHE";
            column = 7;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);
        }

        private static void printJours()
        {
            PrintCenter(printCenterDay);
        }


       delegate void printJourOuSoir(DateTime dt, DateTimeFormatInfo sdf, String text, float fontSize, int column);


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
        private static void printCenterDay(DateTime dt, DateTimeFormatInfo sdf, String text, float fontSize, int column)
        {
            var str = dt.ToShortDateString().ToUpper();
            double max = ((columnSpace * column) + (columnSpace - choiceSize / maxX * 100));
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column, max, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(96, 99, BOLD, fontSize), text);
            drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column, max, NORMAL, str, fontSize), getMiddelofYBetweenTowPoint(93, 96, NORMAL, fontSize), str);
        }

        
        /**
         * Fonction, pout ptiny les plats et quantités de toutes les saises
        */
        private static void printSaisie(int annee, int semaine, bool composition = false)
        {
            BaseContext db = new BaseContext();
            int jour = 1;
            // Pour tous les jours on récupère toutes les saisies et toutes les saisies data 
            // de ce même jour
            for (jour = 1; jour < 8; jour++)
            {
                double column = columnSpace * jour;
                List<Saisie> saisiesList = new List<Saisie>();
                List<Saisie> saisiesListVille1;
                List<Saisie> saisiesListVille2;
                List<Saisie> saisiesListCT;
                //List<Saisie> saisiesListMarennes;

                saisiesListVille1 = SaisieDAO.getAllFromYearWeekDayForTournee("ville 1", "", annee, semaine, jour, db);
                saisiesListVille2 = SaisieDAO.getAllFromYearWeekDayForTournee("ville 2", "", annee, semaine, jour, db);
                saisiesListCT = SaisieDAO.getAllFromYearWeekDayForTournee("contre-tournée", "", annee, semaine, jour, db);
                //saisiesListMarennes = SaisieDAO.getAllFromYearWeekDayForTournee("Marennes", "", annee, semaine, jour, db);
               
                if (jour == 1 || jour == 3 || jour == 5 || jour == 6 || jour == 7)
                {
                    foreach(var v1 in saisiesListVille1)
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
                }
                else if (jour == 2 || jour == 4)
                {
                    foreach (var v1 in saisiesListVille1)
                    {
                        saisiesList.Add(v1);
                    }
                    foreach (var v2 in saisiesListVille2)
                    {
                        saisiesList.Add(v2);
                    }
                }

                // Les données des saisies
                List<SaisieData> saisiesDatas = new List<SaisieData>();

                // On remplit les données des saisies
                foreach (Saisie saisie in saisiesList)
                {
                    foreach (SaisieData saisieData in saisie.data)
                    {
                        saisiesDatas.Add(saisieData);
                    }
                }


                // Pour tous les repas (entrée, plat1, plat2 etc)
                for (int repas = -1; repas <10 ; repas++)
                {
                    // Dictionnaire des formules (ex 2 * frites, 1 * salade, etc)
                    Dictionary<string, double> repasIntituleQuantite = new Dictionary<string, double>();
                    
                    // Pour toutes les données des saisies du jours et par repas 
                    foreach (SaisieData sd in SaisieDataDAO.SortByTypeFromList(repas, saisiesDatas))
                    {
                        //Console.WriteLine(sd.Libelle);
                        string startLibelle = "";
                        string libelle;
                        
                        if (sd.Modifie == false && (sd.Sauce || sd.Mixe || sd.Nature))
                        {
                            startLibelle = "ùùù ";
                            libelle = startLibelle + (sd.Sauce ? " SANS SAUCE " : "") + (sd.Mixe ? " MIXE " : "") + (sd.Nature ? " NATURE " : "");
                        }
                        else
                        {
                            startLibelle = "";
                            libelle = startLibelle + sd.Libelle + (sd.Sauce ? " SANS SAUCE " : "") + (sd.Mixe ? " MIXE " : "") + (sd.Nature ? " NATURE " : "");
                        }
                        
                        double quantite = sd.Quantite == -1 ? 0.5 : sd.Quantite;

                        // On additionne les quantité des repas déjà existant, sinon on l'ajoute dans le dictionnaire
                        if (repasIntituleQuantite.ContainsKey(libelle))
                        {
                            repasIntituleQuantite[libelle] += quantite ;
                        }
                        else
                        {
                            repasIntituleQuantite.Add(libelle, quantite);
                        }

                    }


                    double line = 0;

                    switch (repas)
                    {
                    
                        case SaisieData.BAGUETTE:
                            line = getMiddelofYBetweenTowPoint(96, 99, NORMAL, 9);
                            break;
                        case SaisieData.POTAGE:
                            line = getMiddelofYBetweenTowPoint(90, 93, NORMAL, 9);
                            break;
                        case SaisieData.ENTREE_MIDI:
                            line = getMiddelofYBetweenTowPoint(83, 90, NORMAL, 9);
                            break;
                       case SaisieData.PLAT_MIDI_1:
                            line = getMiddelofYBetweenTowPoint(71, 83, NORMAL, 9);
                            break;
                        case SaisieData.PLAT_MIDI_2:
                            line = getMiddelofYBetweenTowPoint(59, 71, NORMAL, 9);
                            break;
                        case SaisieData.PLAT_MIDI_3:
                            line = getMiddelofYBetweenTowPoint(47, 59, NORMAL, 9);
                            break;
                        case SaisieData.FROMAGE:
                            line = getMiddelofYBetweenTowPoint(44, 47, NORMAL, 9);
                            break;
                        case SaisieData.DESSERT_MIDI:
                            line = getMiddelofYBetweenTowPoint(34, 44, NORMAL, 9);
                            break;
                        case SaisieData.ENTREE_SOIR:
                            line = getMiddelofYBetweenTowPoint(24, 34, NORMAL, 9);
                            break;
                        case SaisieData.PLAT_SOIR_1:
                            line = getMiddelofYBetweenTowPoint(14, 24, NORMAL, 9);
                            break;
                        case SaisieData.DESSERT_SOIR:
                            line = getMiddelofYBetweenTowPoint(1, 11, NORMAL, 9);
                            break;

                    }

                    // Ecriture des repas sur le PDF
                    if (line != 0)
                    {

                        foreach (KeyValuePair<string, double> entry in repasIntituleQuantite)
                        {
                            var txt = entry.Key;
                            bool compoMixe = false;

                            if (!String.IsNullOrEmpty(entry.Key))
                            {
                                PDType1Font font = NORMAL;
                                int R = 0;
                                int G = 0;
                                int B = 0;

                                List<String> plats;
                               
                                plats = MenuDao.getPlatsNameFromWeekDay(semaine, jour);

                                if (entry.Key.Length > 3)
                                {
                                    var test = entry.Key.Substring(0, 3);

                                    if (test == "ùùù")
                                    {
                                        compoMixe = true;
                                        R = 0;
                                        G = 155;
                                        B = 0;
                                        txt = txt.Remove(0, 3);
                                    }   
                                }

                                if (plats.Contains(txt))
                                {
                                    if (entry.Value == 10)
                                    { 
                                        var txtQ = "1";
                                        R = 30;
                                        G = 127;
                                        B = 203;
                                        PrintTextBetweenTowPoint(txt, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                        PrintTextBetweenTowPoint(txtQ, getX(column) + 50 + 50, getX(column + columnSpace) + 50 - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                        line -= 20;
                                    }
                                    else if (entry.Value == 20)
                                    {
                                        var txtQ = "2";
                                        R = 30;
                                        G = 127;
                                        B = 203;
                                        PrintTextBetweenTowPoint(txt, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                        PrintTextBetweenTowPoint(txtQ, getX(column) + 50 + 50, getX(column + columnSpace) + 50 - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                        line -= 20;
                                    }
                                    else if (entry.Value == 30)
                                    {
                                        var txtQ = "3";
                                        R = 30;
                                        G = 127;
                                        B = 203;
                                        PrintTextBetweenTowPoint(txt, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                        PrintTextBetweenTowPoint(txtQ, getX(column) + 50 + 50, getX(column + columnSpace) + 50 - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                        line -= 20;
                                    }
                                    else
                                    {                                       
                                        var txtQ = entry.Value.ToString();
                                        PrintTextBetweenTowPoint(txt, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 8, font, R, G, B);
                                        PrintTextBetweenTowPoint(txtQ, getX(column) + 50 + 50, getX(column + columnSpace) + 50 - (choiceSize + 5), line, 8, font, R, G, B);
                                        line -= 20;
                                    }
                                }
                                else if (compoMixe == true)
                                {
                                    var txtQ = entry.Value.ToString();
                                    PrintTextBetweenTowPoint(txt, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                    PrintTextBetweenTowPoint(txtQ, getX(column) + 50 + 50, getX(column + columnSpace) + 50 - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                    line -= 20;
                                }
                                else if(txt == "Solene")
                                {
                                    var txtQ = entry.Value.ToString();
                                    PrintTextBetweenTowPoint("S", getX(column) + +50 + 50, getX(column + columnSpace) + 25 - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                    PrintTextBetweenTowPoint(txtQ, getX(column) + 50 + 75, getX(column + columnSpace) + 50 - (choiceSize + 5), line, 8, NORMAL, R, G, B);
                                } 
                                else if (txt == "Blanche")
                                {
                                    var txtQ = entry.Value.ToString();
                                    PrintTextBetweenTowPoint("B", getX(column) + +50 + 50, getX(column + columnSpace) + 25 - (choiceSize + 5), line - 20, 8, NORMAL, R, G, B);
                                    PrintTextBetweenTowPoint(txtQ, getX(column) + 50 + 75, getX(column + columnSpace) + 50 - (choiceSize + 5), line - 20, 8, NORMAL, R, G, B);

                                }
                            }
                        }
                    }
                }
            }
            db.Dispose();
        }

        private static void PrintTextBetweenTowPoint(String str, double x, double maxX, double y, double fontSize, PDType1Font font, int R, int G, int B)
        {
            double width = (font.getStringWidth(str) / 1000 * fontSize);

            if (x + width < maxX)
            {
                drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFCuisine.maxX * 100), (maxX / CreatePDFCuisine.maxX * 100), font, str, fontSize), y, str, R, G, B);
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

              
                    strings.Add(line.ToString());
                  

                    double height = ((font.getFontDescriptor().getCapHeight()) / 1000 * fontSize / 2) * strings.Count;
                
                    string maxLength = "";

                    foreach (var s in strings)
                    {
                        if (s.Length > maxLength.Length) maxLength = s;
                    }

                    drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDFCuisine.maxX * 100), (maxX / CreatePDFCuisine.maxX * 100), font, maxLength, fontSize), getY((y / maxY * 100)) + height, strings, R, G, B);
                }
            }
        }


        private static void PrintLines()
        {
          
            //Jours Bottom Line
            drawLine(getX(0), getX(columnSpace), getY(96), getY(96));

            //Baguette Bottom Line
            drawLine(getX(0), getX(100), getY(96), getY(96));

            //Semaine Bottom Line
            drawLine(getX(0), getX(100), getY(93), getY(93));

            //Potages Bottom Line
            drawLine(getX(0), getX(100), getY(88), getY(88));

            //Entrees Line
            drawLine(getX(0), getX(100), getY(80), getY(80));

            //Plat 1 Bottom Line
            drawLine(getX(columnSpace / 2), getX(100), getY(68), getY(68));

            //Plat 2 Bottom Line
            drawLine(getX(columnSpace / 2), getX(100), getY(56), getY(56));

            //Plat 3 Bottom Line Line
            drawLine(getX(0), getX(100), getY(44), getY(44));

            //Formage Bottom Line
            drawLine(getX(0), getX(100), getY(41), getY(41));

            //Dessert Bottom Line
            drawLine(getX(0), getX(100), getY(31), getY(31));
            drawLine(getX(0), getX(100), getY(30.8), getY(30.8));

            //Entrée Bottom Line Soir
            drawLine(getX(0), getX(100), getY(21), getY(21));
            
            //Plat Bottom Line Soir
            drawLine(getX(0), getX(100), getY(11), getY(11));
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
                drawLine(getX(columnSpace * (2 + i)) - choiceSize, getX(columnSpace * (2 + i)) - choiceSize, menuYTopNoLivraison, menuYBottom);
            }

            drawLine(getX(columnSpace / 2), getX(columnSpace / 2), getY(80), getY(44));
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
            int R = 0;
            int G = 0;
            int B = 0;

            text = "Baguettes".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(96, 99, BOLD, fontSize), text, R, G, B);

            text = "Semaine "+ Semaine;
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(93, 96, BOLD, fontSize), text, R, G, B);

            text = "Potages".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(90, 93, BOLD, fontSize), text, R, G, B);

            text = "Entrées".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(80, 90, BOLD, fontSize), text, R, G, B);


            text = "Plats".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace / 2, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(68, 80, BOLD, fontSize), text, R, G, B);

            text = "Au".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace / 2, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(56, 68, BOLD, fontSize), text, R, G, B);

            text = "Choix".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace / 2, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(44, 56, BOLD, fontSize), text, R, G, B);

            text = "Plat 1".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace / 2, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(68, 80, BOLD, fontSize), text, R, G, B);

            text = "Plat 2".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace / 2, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(56, 68, BOLD, fontSize), text, R, G, B);

            text = "Plat 3".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace / 2, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(44, 56, BOLD, fontSize), text, R, G, B);

            for (int i = 0; i < 7; i++)
            {
                text = "Fromage";
                drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(columnSpace * (1 + i), columnSpace * (1 + i + 1) - (choiceSize / maxX * 100), NORMAL, text, fontSize), getMiddelofYBetweenTowPoint(41, 44, NORMAL, fontSize), text, R, G, B);
            }

            text = "Desserts".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(31, 41, BOLD, fontSize), text, R, G, B);

            text = "Entrées du soir".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(21, 31, BOLD, fontSize), text, R, G, B);

            text = "PLAT du SOIR".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(11, 21, BOLD, fontSize), text, R, G, B);

            text = "Desserts du soir".ToUpper();
            drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(1, 11, BOLD, fontSize), text, R, G, B);
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
                drawText(font, fontSize, x, (y - ((fontSize + 2) * count)), s,0,0,0);
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
