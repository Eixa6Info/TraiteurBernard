
using Microsoft.Win32;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.common;
using org.apache.pdfbox.pdmodel.edit;
using org.apache.pdfbox.pdmodel.font;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TraiteurBernardWPF.DAO;
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.PDF
{
    public static class CreatePDF
    {

        /**
         * Variable pour le placement
         */
        private static  float margin = 10;

        private static  double minX = margin;
        private static  double minY = margin;
        private static  double columnSpace = 12.5;//12,5%
        private static  double choiceSize = 25;//25Px

        private static  PDType1Font BOLD = PDType1Font.HELVETICA_BOLD;
        private static  PDType1Font NORMAL = PDType1Font.HELVETICA;
        private static  PDType1Font OBLIQUE = PDType1Font.HELVETICA_OBLIQUE;

        private static double maxX;
        private static double maxY;
        private static double menuYTop;
        private static double menuYTopNoLivraison;
        private static double menuYTopNoDay;
        private static double menuYBottom;

        private static int semaine;
        private static string output;
        private static PDDocument document;
        private static PDPage blankPage;
        private static PDPageContentStream contentStream;
        private static String jour1Livraison, jour2Livraison, jour3Livraison;

        /**
         * Function d'enter pour créer le PDF
         *
         * @param format  PDRectangle A3 ou A4
         * @param semaine Integer Numéros de semaine
         * @return boolean
         * @throws Exception ...
         */
        public static bool Start(float width, float height, int semaine, bool printSaisieBool)
        {

            //Récuperation du format de la page en fonction de A3 ou A4
            maxX = height - margin;
            maxY = width - margin;

            //Récupération des variable en relation avec des pourcentage
            menuYTop = getY(85);
            menuYTopNoLivraison = getY(82);
            menuYTopNoDay = getY(79);
            menuYBottom = getY(14);

            //Définition de la semaine
            CreatePDF.semaine = semaine;

            //Demande a l'utilisateur de choisir ou enregistrer
            if (!getPath()) {
                return false;
        }

        //Création du document
        document = new PDDocument();

        //Print de tout les page du midi
        printMidi(printSaisieBool);

        //Print de tout les page du soir
        printSoir(printSaisieBool);

        //Saving the document
        document.save(output);

        //Closing the document
        document.close();

        //ouverture du document
        //Desktop.getDesktop().open(output);
            
          

            //Function ok !
            return true;
    }

    /**
     * Function pour créer les 3 pages du midi
     *
     * @throws IOException ...
     */
    private static void printMidi(bool printSaisieBool) 
    {

        //Création d'une nouvelle page
        newPageAndPrintAllForMidi(printSaisieBool);

        //Ajout de toute les infirmation
        printJoursT1();

        //Close de la page
        contentStream.close();


        //Création d'une nouvelle page
        newPageAndPrintAllForMidi(printSaisieBool);

        //Définition des jour de livraison
        jour1Livraison = "lundi après midi";
        jour2Livraison = "mercredi après midi";
        jour3Livraison = "vendredi après midi";

        //Print du header de livraison
        printLivraison();

        //Print des jours de livraison
        printJoursT2();

        //Close de la page
        contentStream.close();


        //Création d'une nouvelle page
        newPageAndPrintAllForMidi(printSaisieBool);

        //Définition des jour de livraison
        jour1Livraison = "samedi";
        jour2Livraison = "mardi";
        jour3Livraison = "jeudi";

        //Print du header de livraison
        printLivraison();

        //Print des jours de livraison
        printJoursT3();

        //Close de la page
        contentStream.close();
    }

    /**
     * Function pour créer les 3 pages du midi
     *
     * @throws IOException ...
     */
    private static void printSoir(bool printSaisieBool)
    {

        //Création d'une nouvelle page
        newPageAndPrintAllForSoir(printSaisieBool);

        //Ajout de toute les infirmation
        printSoirT1();

        //Close de la page
        contentStream.close();


        //Création d'une nouvelle page
        newPageAndPrintAllForSoir(printSaisieBool);

        //Définition des jour de livraison
        jour1Livraison = "lundi après midi";
        jour2Livraison = "mercredi après midi";
        jour3Livraison = "vendredi après midi";

        //Print du header de livraison
        printLivraisonSoir();

        //Print des jours de livraison
        printSoirT2();

        //Close de la page
        contentStream.close();


        //Création d'une nouvelle page
        newPageAndPrintAllForSoir(printSaisieBool);

        //Définition des jour de livraison
        jour1Livraison = "samedi";
        jour2Livraison = "mardi";
        jour3Livraison = "jeudi";

        //Print du header de livraison
        printLivraisonSoir();

        //Print des jours de livraison
        printSoirT3();

        //Close de la page
        contentStream.close();
    }

    /**
     * Function identique a toute les page du midi
     * si l'argument est true, on print les saies
     * sinon on print les menu
     * @throws IOException ...
     */
    private static void newPageAndPrintAllForMidi(bool printSaisieBool) 
    {
        //Récup du document
        getDocument();

        //création du cadre
        printCadre();

        //Création de toute les ligne
        PrintLines();

        //Création de toute les column
        printColumn();

        //Ajout du header
        printHeader();

        //Ajout des mention de reserve
        printReserve();

        //ajout de la signature
        printSignature();

        //ajout des infromation pour la saisie
        printNB();

        //Ajout des description a gauche du tableaux
        printDescLine();

        //Ajout des menus ou des saisies dans les case
        if (printSaisieBool) printSaisie();
        else printMenu();

        // On enleve le 'a remettre avant le 
        //Ajout de la date en haut
        //printDateOnTop();
    }

    /**
     * Function identique a toute les page du midi
     *
     * @throws IOException ...
     */
    private static void newPageAndPrintAllForSoir(bool printSaisieBool) 
    {
        //Récup du document
        getDocument();

        //création du cadre
        printCadre();

        //Création de toute les ligne
        printLinesSoir();

        //Création de toute les column
        printColumnSoir();

        //Ajout du header
        printHeader();

        //Ajout des mention de reserve
        printReserveSoir();

        //ajout de la signature
        printSignature();

        //ajout des infromation pour la saisie
        printNB();

        //Ajout des description a gauche du tableaux
        printDescLineSoir();

        //Ajout des menus ou des saisies dans les case
        if (printSaisieBool) printSaisieSoir();
        else printMenuSoir();

        // On enleve le 'a remettre avant le 
        //Ajout de la date en haut
        //printDateOnTop();
    }

        /**
         * Print sur le PDF des jours pour la page 1 (Tourner 1)
         *
         * @throws IOException ...
         */
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
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

        private static void PrintCenter1(printJourOuSoir fct)
        {
            DateTime today = DateTime.Today;

            DateTime laDate = FirstDateOfWeekISO8601(today.Year, semaine);

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
        private static void printJoursT1() 
        {
            PrintCenter1(printCenterDay);
        }

        /**
         * Print sur le PDF des jours pour la page 2 (Tourner 2)
         *
         * @throws IOException ...
         */
        private static void PrintCenter2(printJourOuSoir fct)
        {
            DateTime today = DateTime.Today;

            DateTime laDate = FirstDateOfWeekISO8601(today.Year, semaine);

            var sdf = new DateTimeFormatInfo();

            //variavle réutiliser
            String text;
            int column;
            float fontSize = 10;

            text = "MARDI";
            column = 1;

            laDate = laDate.AddDays(1);

            fct(laDate, sdf, text, fontSize, column);

            text = "MERCREDI";
            column = 2;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            text = "JEUDI";
            column = 3;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            text = "VENDREDI";
            column = 4;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            text = "SAMEDI";
            column = 5;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            text = "DIMANCHE";
            column = 6;
            laDate = laDate.AddDays(1);
            fct(laDate, sdf, text, fontSize, column);

            text = "LUNDI";
            column = 7;
            laDate = laDate.AddDays(1);

            fct(laDate, sdf, text, fontSize, column);
        }
        private static void printJoursT2() 
        {

            PrintCenter2(printCenterDay);

        }

        /**
         * Print sur le PDF des jours pour la page 3 (Tourner 3)
         *
         * @throws IOException ...
         */
        private static void printJoursT3() 
        {
            printJoursT1();
        }

        /**
         * Print sur le PDF des Soir pour la page 1 (Tourner 1)
         *
         * @throws IOException ...
         */
        private static void printSoirT1()
        {
            PrintCenter1(printCenterDaySoir);
        }

    /**
     * Print sur le PDF des Soir pour la page 2 (Tourner 2)
     *
     * @throws IOException ...
     */
    private static void printSoirT2()
        {
            PrintCenter2(printCenterDaySoir);

        }

    /**
     * Print sur le PDF des Soir pour la page 3 (Tourner 3)
     *
     * @throws IOException ...
     */
    private static void printSoirT3() 
    {
        printSoirT1();
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
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column, max, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(79, 82, BOLD, fontSize), text);
        drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column, max, NORMAL, str, fontSize), getMiddelofYBetweenTowPoint(76, 79, NORMAL, fontSize), str);
    }

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
    private static void printCenterDaySoir(DateTime dt, DateTimeFormatInfo sdf, String text, float fontSize, int column)
    {
        var str = dt.ToShortDateString().ToUpper();

        double max = ((columnSpace * column) + (columnSpace - choiceSize / maxX * 100));
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column, max, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(76, 79, BOLD, fontSize), text);
        drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(columnSpace * column, max, NORMAL, str, fontSize), getMiddelofYBetweenTowPoint(73, 76, NORMAL, fontSize), str);
    }

    /**
     * Function pour print les Plats de tout les Menus
     */
    private static void printMenu()
    {
        List<Menu> menus = MenuDao.getAllFromWeek(semaine);

            foreach(var menu in menus)
     {

            if (menu == null)
            {
                return;
            }
            if (menu.Plats.Count == 0)
            {
                return;
            }

            var plats = new List<Plat>(menu.Plats);

            double column = columnSpace * menu.Jour;

            foreach(var plat in plats)
            {

                if (plat == null)
                {
                    return;
                }
                if (plat.Name == null)
                {
                    return;
                }

                double line = 0;

                switch (plat.Type)
                {
                    case Plat.ENTREE_MIDI:
                        line = getMiddelofYBetweenTowPoint(63, 73, NORMAL, 11);
                        break;
                    case Plat.PLAT_MIDI_1:
                        line = getMiddelofYBetweenTowPoint(51, 63, NORMAL, 11);
                        break;
                    case Plat.PLAT_MIDI_2:
                        line = getMiddelofYBetweenTowPoint(39, 51, NORMAL, 11);
                        break;
                    case Plat.PLAT_MIDI_3:
                        line = getMiddelofYBetweenTowPoint(27, 39, NORMAL, 11);
                        break;
                    case Plat.DESSERT_MIDI:
                        line = getMiddelofYBetweenTowPoint(14, 24, NORMAL, 11);
                        break;
                }

                if (line != 0)
                {
                    try
                    {
                            String platString = plat.Name;//.toLowerCase();

                            platString = platString.Substring(0, 1).ToUpper() + platString.Substring(1);//.toLowerCase();
                        PrintTextBetweenTowPoint(platString, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 10, NORMAL);
                    }
                    catch (IOException e)
                    {
                            throw e;
                    }
                }

            };

        };
    }

        /**
         * Fonctio, pout ptiny les plats et quantités de toutes les saises
        */
        private static void printSaisie()
        {
            BaseContext db = new BaseContext();
               
            // Pour tous les jours on récupère toutes les saisies et toutes les saisies data 
            // de ce même jour
            for(int jour = 1; jour < 8; jour++)
            {
                double column = columnSpace * jour;
                
                // Les saisies
                List<Saisie> saisiesList = SaisieDAO.getAllFromYearWeekDay(2020, semaine, jour, db);

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
            for (int repas = 1; repas < 6;repas++)
            {
                // Dictionnaire des formules (ex 2 * frites, 1 * salade, etc)
                Dictionary<string, int> repasIntituleQuantite = new Dictionary<string, int>();

                // Pour toutes les données des saisies du jours et par repas 
                foreach (SaisieData sd in SaisieDataDAO.SortByTypeFromList(repas, saisiesDatas))
                {
                    string libelle = sd.Libelle;
                    int quantite = sd.Quantite;

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


                double line = 0;

                switch (repas)
                {
                    case Plat.ENTREE_MIDI:
                        line = getMiddelofYBetweenTowPoint(63, 73, NORMAL, 11);
                        break;
                    case Plat.PLAT_MIDI_1:
                        line = getMiddelofYBetweenTowPoint(51, 63, NORMAL, 11);
                        break;
                    case Plat.PLAT_MIDI_2:
                        line = getMiddelofYBetweenTowPoint(39, 51, NORMAL, 11);
                        break;
                    case Plat.PLAT_MIDI_3:
                        line = getMiddelofYBetweenTowPoint(27, 39, NORMAL, 11);
                        break;
                    case Plat.DESSERT_MIDI:
                        line = getMiddelofYBetweenTowPoint(14, 24, NORMAL, 11);
                        break;


                }

                // Ecriture des repas sur le PDF
                if (line != 0)
                {
                    string platString = "";
                    foreach (KeyValuePair<string, int> entry in repasIntituleQuantite)
                    {
                        if (!String.IsNullOrEmpty(entry.Key))
                        {
                            platString += " " + entry.Value + "*" + entry.Key + " ";

                        }

                    }
                    PrintTextBetweenTowPoint(platString, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 10, NORMAL);
                }
            }
   
        }

        db.Dispose();
  
    }

        /**
         * Function pour print les Plats de tout les Menus
         */
        private static void printMenuSoir()
        {
            List<Menu> menus = MenuDao.getAllFromWeek(semaine);

            foreach(Menu menu in menus)
            {
                if(menu == null)
                {
                    return;
                }
                if(menu.Plats.Count == 0)
                {
                    return;
                }

                List<Plat> plats = new List<Plat>(menu.Plats);

                double column = columnSpace * menu.Jour;

                foreach(Plat plat in plats)
                {
                    if(plat == null)
                    {
                        return;
                    }
                    if(plat.Name == null)
                    {
                        return;
                    }

                    double line = 0;

                    switch (plat.Type)
                    {
                        case Plat.ENTREE_SOIR:
                            line = getMiddelofYBetweenTowPoint(63, 73, NORMAL, 11);
                            break;

                        case Plat.PLAT_SOIR_1:
                            line = getMiddelofYBetweenTowPoint(27, 63, NORMAL, 11);
                            break;

                        case Plat.DESSERT_SOIR:
                            line = getMiddelofYBetweenTowPoint(18, 27, NORMAL, 11);
                            break;

                    }

                    if (line != 0)
                    {
                        try
                        {
                            String platString = plat.Name;//.toLowerCase();

                            platString = platString.Substring(0, 1).ToUpper() + platString.Substring(1);//.toLowerCase();
                            PrintTextBetweenTowPoint(platString, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 10, NORMAL);
                        }
                        catch (IOException e)
                        {
                            throw e;
                        }
                    }
                }
            }

        }

        /**
         * Function pour print les Plats de tout les Menus
         */
        private static void printSaisieSoir()
        {

            BaseContext db = new BaseContext();

            // Pour tous les jours on récupère toutes les saisies et toutes les saisies data 
            // de ce même jour
            for (int jour = 1; jour < 8; jour++)
            {
                double column = columnSpace * jour;

                // Les saisies
                List<Saisie> saisiesList = SaisieDAO.getAllFromYearWeekDay(2020, semaine, jour, db);

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
                for (int repas = 6; repas < 9; repas++)
                {
                    // Dictionnaire des formules (ex 2 * frites, 1 * salade, etc)
                    Dictionary<string, int> repasIntituleQuantite = new Dictionary<string, int>();

                    // Pour toutes les données des saisies du jours et par repas 
                    foreach (SaisieData sd in SaisieDataDAO.SortByTypeFromList(repas, saisiesDatas))
                    {
                        string libelle = sd.Libelle;
                        int quantite = sd.Quantite;

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


                    double line = 0;

                    switch (repas)
                    {
                        case Plat.ENTREE_SOIR:
                            line = getMiddelofYBetweenTowPoint(63, 73, NORMAL, 11);
                            break;
                        case Plat.PLAT_SOIR_1:
                            line = getMiddelofYBetweenTowPoint(27, 63, NORMAL, 11);
                            break;
                        case Plat.DESSERT_SOIR:
                            line = getMiddelofYBetweenTowPoint(18, 27, NORMAL, 11);
                            break;


                    }

                    // Ecriture des repas sur le PDF
                    if (line != 0)
                    {
                        string platString = "";
                        foreach (KeyValuePair<string, int> entry in repasIntituleQuantite)
                        {
                            if (!String.IsNullOrEmpty(entry.Key))
                            {
                                platString += " " + entry.Value + "*" + entry.Key + " ";

                            }

                        }
                        PrintTextBetweenTowPoint(platString, getX(column) + 5, getX(column + columnSpace) - (choiceSize + 5), line, 10, NORMAL);
                    }
                }

            }

            db.Dispose();




        }


        private static void PrintTextBetweenTowPoint(String str, double x, double maxX, double y, double fontSize, PDType1Font font)
    {
        double width = (font.getStringWidth(str) / 1000 * fontSize);

        if (x + width < maxX) {
            drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDF.maxX * 100), (maxX / CreatePDF.maxX * 100), font, str, fontSize), y, str);
        } else {
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
                /*AtomicReference<String> maxLength = new AtomicReference<>("");


                strings.forEach(s-> {
                    if (s.length() > maxLength.get().length())
                    {
                        maxLength.set(s);
                    }
                });*/
                string maxLength = "";

                foreach (var s in strings)
                {
                    if (s.Length > maxLength.Length) maxLength = s;
                }

                drawText(font, fontSize, getMiddelofXBetweenTowPoint((x / CreatePDF.maxX * 100), (maxX / CreatePDF.maxX * 100), font, maxLength, fontSize), getY((y / maxY * 100)) + height, strings);
            }
        }
    }

   
    private static void PrintLines() 
    {
        //Livraison Bottom Line
        drawLine(getX(0), getX(100), getY(82), getY(82));

        //Jours Bottom Line
        drawLine(getX(0), getX(columnSpace), getY(79), getY(79));

        for (int i = 0; i < 500; i++) {
            if (getX(columnSpace) + (i * 5) >= getX(100))
            {
                break;
            }
            drawLine(getX(columnSpace) + (i * 5), getX(columnSpace) + (i * 5 + 2), getY(79), getY(79));
        }

        //Baguette Bottom Line
        drawLine(getX(0), getX(100), getY(76), getY(76));

        //Potages Bottom Line
        drawLine(getX(0), getX(100), getY(73), getY(73));

        //Entrees Line
        drawLine(getX(0), getX(100), getY(63), getY(63));

        //Plat 1 Bottom Line
        drawLine(getX(columnSpace / 2), getX(100), getY(51), getY(51));

        //Plat 2 Bottom Line
        drawLine(getX(columnSpace / 2), getX(100), getY(39), getY(39));

        //Plat 3 Bottom Line Line
        drawLine(getX(0), getX(100), getY(27), getY(27));

        //Formage Bottom Line
        drawLine(getX(0), getX(100), getY(24), getY(24));
    }

    /**
     * Function pour print toute les lignes
     *
     * @throws IOException ...
     */
    private static void printLinesSoir() 
    {
        //Livraison Bottom Line
        drawLine(getX(0), getX(100), getY(79), getY(79));

        for (int i = 0; i < 500; i++) {
            if (getX(columnSpace) + (i * 5) >= getX(100))
            {
                break;
            }
            drawLine(getX(columnSpace) + (i * 5), getX(columnSpace) + (i * 5 + 2), getY(76), getY(76));
        }

        //Jour Bottom Line
        drawLine(getX(0), getX(100), getY(73), getY(73));

        //Entrees Bottom Line
        drawLine(getX(0), getX(100), getY(63), getY(63));

        //Plat 3 Bottom Line Line
        drawLine(getX(0), getX(100), getY(27), getY(27));
    }

    /**
     * Print des jour de livraison en haut du tableux
     *
     * @throws IOException ...
     */
    private static void printLivraison() 
    {
        //Top Bar Livraison
        drawLine(getX(columnSpace), getX(columnSpace * 8), menuYTop, menuYTop);
        drawLine(getX(columnSpace), getX(columnSpace), menuYTop, menuYTopNoLivraison);
        drawLine(getX(columnSpace * 3), getX(columnSpace * 3), menuYTop, menuYTopNoLivraison);
        drawLine(getX(columnSpace * 5), getX(columnSpace * 5), menuYTop, menuYTopNoLivraison);

        String baseText = "Livraison le ";
        String text1 = baseText + jour1Livraison;
        String text2 = baseText + jour2Livraison;
        String text3 = baseText + jour3Livraison;
        double fontSize = 12;

        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * 1, columnSpace * 3, BOLD, text1, fontSize), getMiddelofYBetweenTowPoint(82, 85, BOLD, fontSize), text1);
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * 3, columnSpace * 5, BOLD, text2, fontSize), getMiddelofYBetweenTowPoint(82, 85, BOLD, fontSize), text2);
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * 5, columnSpace * 8, BOLD, text3, fontSize), getMiddelofYBetweenTowPoint(82, 85, BOLD, fontSize), text3);
    }

    /**
     * Print des jour de livraison en haut du tableux
     *
     * @throws IOException ...
     */
    private static void printLivraisonSoir() 
    {
        //Top Bar Livraison
        drawLine(getX(columnSpace), getX(columnSpace * 8), getY(82), getY(82));
        drawLine(getX(columnSpace), getX(columnSpace), getY(79), menuYTopNoLivraison);
        drawLine(getX(columnSpace * 3), getX(columnSpace * 3), getY(79), menuYTopNoLivraison);
        drawLine(getX(columnSpace * 5), getX(columnSpace * 5), getY(79), menuYTopNoLivraison);

        String baseText = "Livraison le ";
        String text1 = baseText + jour1Livraison;
        String text2 = baseText + jour2Livraison;
        String text3 = baseText + jour3Livraison;
        double fontSize = 12;

        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * 1, columnSpace * 3, BOLD, text1, fontSize), getMiddelofYBetweenTowPoint(79, 82, BOLD, fontSize), text1);
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * 3, columnSpace * 5, BOLD, text2, fontSize), getMiddelofYBetweenTowPoint(79, 82, BOLD, fontSize), text2);
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace * 5, columnSpace * 8, BOLD, text3, fontSize), getMiddelofYBetweenTowPoint(79, 82, BOLD, fontSize), text3);
    }

    /**
     * Print de toute les column
     *
     * @throws IOException ...
     */
    private static void printColumn()
    {
        for (int i = 0; i < 7; i++) {
            //Left bar Jour 1
            drawLine(getX(columnSpace * (1 + i)), getX(columnSpace * (1 + i)), menuYTopNoLivraison, menuYBottom);

            //Left Bar Jour 1 choice
            drawLine(getX(columnSpace * (2 + i)) - choiceSize, getX(columnSpace * (2 + i)) - choiceSize, menuYTopNoLivraison, menuYBottom);
        }

        //Plat Column
        drawLine(getX(columnSpace / 2), getX(columnSpace / 2), getY(63), getY(27));
    }

    /**
     * Print de toute les column
     *
     * @throws IOException ...
     */
    private static void printColumnSoir() 
    {
        for (int i = 0; i < 7; i++) {
            //Left bar Jour 1
            drawLine(getX(columnSpace * (1 + i)), getX(columnSpace * (1 + i)), getY(79), getY(18));

            //Left Bar Jour 1 choice
            drawLine(getX(columnSpace * (2 + i)) - choiceSize, getX(columnSpace * (2 + i)) - choiceSize, getY(79), getY(18));
        }
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
        float fontSize = 10;

        text = "Baguettes".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(76, 79, BOLD, fontSize), text);

        text = "Potages".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(73, 76, BOLD, fontSize), text);

        text = "Entrées".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(63, 73, BOLD, fontSize), text);


        text = "Plats".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace / 2, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(51, 63, BOLD, fontSize), text);

        text = "Au".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace / 2, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(39, 51, BOLD, fontSize), text);

        text = "Choix".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace / 2, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(27, 39, BOLD, fontSize), text);

        text = "Plat 1".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace / 2, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(51, 63, BOLD, fontSize), text);

        text = "Plat 2".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace / 2, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(39, 51, BOLD, fontSize), text);

        text = "Plat 3".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(columnSpace / 2, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(27, 39, BOLD, fontSize), text);

        for (int i = 0; i < 7; i++) {
            text = "Fromage".ToUpper();
            drawText(NORMAL, fontSize, getMiddelofXBetweenTowPoint(columnSpace * (1 + i), columnSpace * (1 + i + 1) - (choiceSize / maxX * 100), NORMAL, text, fontSize), getMiddelofYBetweenTowPoint(24, 27, NORMAL, fontSize), text);
        }

        text = "Desserts".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(14, 24, BOLD, fontSize), text);
    }

    /**
     * Print des description et du fromage a gauche du tableaux
     *
     * @throws IOException ...
     */
    private static void printDescLineSoir()
    {
        //Baguettes
        String text;
        float fontSize = 10;

        text = "Entrées".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(63, 73, BOLD, fontSize), text);

        text = "PLAT du SOIR".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(27, 63, BOLD, fontSize), text);

        text = "Desserts".ToUpper();
        drawText(BOLD, fontSize, getMiddelofXBetweenTowPoint(0, columnSpace, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(18, 27, BOLD, fontSize), text);
    }

    /**
     * Print du header (nom, tel, email...)
     *
     * @throws IOException ...
     */
    private static void printHeader() 
    {
        //Ligne de changement
        drawText(BOLD, 12, getX(0) + 2, getY(93),
                "Maison BERNARD Traiteur - 49 Route de Meursac 17600 SABLONCEAUX - 05 46 02 83 62 - eric.bernard17@orange.fr",
                "NOM et ADRESSE : ..........................................          TELEPHONE : ..........................................          PAIN : OUI / NON               SEMAINE : " + CreatePDF.semaine);
        drawText(OBLIQUE, 11, getX(0) + 2, getY(93) - ((12 * 2) + 2), "Supplément Baguette : 0,50€ la demi baguette / 1€ la baguette");
        drawLine(getX(0), getX(100), getY(88), getY(88));
    }

    /**
     * Function de print pour le cadre (quatre ligne)
     *
     * @throws IOException ...
     */
    private static void printCadre() { 
        //Left Line
        drawLine(getX(0), getX(0), getY(10), getY(95));

        //Right Line
        drawLine(getX(100), getX(100), getY(10), getY(95));

        //Top Line
        drawLine(getX(0), getX(100), getY(95), getY(95));

        //Bottom Line
        drawLine(getX(0), getX(100), getY(10), getY(10));
    }

    /**
     * Print des infromation en bas du tableuax
     *
     * @throws IOException ...
     */
    private static void printNB() 
    {
        //NB Bottom
        drawText(NORMAL, 10, getX(0) + 2, getY(8) + 2,
                "NB / Mettre une croix dans la case prévue à cet effet à droite du composant choisi pour chaque jour de la semaine souhaitée. Rayer les jours sans repas.",
                "Pour le bon fonctionnement du service, veuillez impérativement rendre votre feuille de menu remplie au plus tard le mercredi matin.",
                "Toute annulation sera acceptée, si vous prévenez par téléphone 72 heures à l'avance pendant les jours ouvrables.");
    }

    /**
     * Print du bon pour accord et de signature
     *
     * @throws IOException ...
     */
    private static void printSignature() 
    {
        //Signature client
        String text;
        float fontSize = 12;

        text = "Bon pour Accord :";
        drawText(BOLD, fontSize, getMiddelOfX(26, BOLD, text, fontSize), getY(2), text);

        text = "Date :";
        drawText(BOLD, fontSize, getMiddelOfX(50, BOLD, text, fontSize), getY(2), text);

        text = "Signature :";
        drawText(BOLD, fontSize, getMiddelOfX(78, BOLD, text, fontSize), getY(2), text);
    }

    /**
     * Print de la reserve de modification en fonction des arivage
     *
     * @throws IOException ...
     */
    private static void printReserve() 
    {
        //Ligne de changement
        drawLine(getX(0), getX(100), getY(14), getY(14));

        String text = "Nous nous réservons le droit de modifier la composition des menus en fonction des arrivages";
        float fontSize = 12;

        drawText(BOLD, fontSize, getMiddelOfX(50, BOLD, text, fontSize), getMiddelofYBetweenTowPoint(10, 14, BOLD, fontSize), text);
    }

    /**
     * Print de la reserve de modification en fonction des arivage
     *
     * @throws IOException ...
     */
    private static void printReserveSoir() { 
        //Ligne de changement
        drawLine(getX(0), getX(100), getY(14), getY(14));
        drawLine(getX(0), getX(100), getY(18), getY(18));

        String text1 = "Nous nous réservons le droit de modifier la composition des menus en fonction des arrivages";
        String text2 = "Pas de possibilité de changement des plats du soir";
        String text3 = "6,80 euros TTC le Repas du soir UNIQUEMENT livré avec le reps du midi";
        float fontSize = 12;

        drawText(BOLD, fontSize, getMiddelOfX(50, BOLD, text2, fontSize), getMiddelofYBetweenTowPoint(14, 18, BOLD, fontSize), text2);
        drawText(NORMAL, fontSize, getMiddelOfX(50, NORMAL, text1, fontSize), getMiddelofYBetweenTowPoint(10, 14, NORMAL, fontSize), text1);
        drawText(BOLD, fontSize + 2, getMiddelOfX(50, BOLD, text3, fontSize + 2), getMiddelofYBetweenTowPoint(83, 85, BOLD, fontSize + 2), text3);
    }

    /**
     * Print de la date : A remettre avant
     *
     * @throws IOException ...
     */
    private static void printDateOnTop() 
    {
        //Add Date On Top
     /*   Calendar cal = Calendar.getInstance();
        cal.set(Calendar.WEEK_OF_YEAR, semaine - 1);
        cal.set(Calendar.DAY_OF_WEEK, Calendar.THURSDAY);
          */  
        var dateRemise = FirstDateOfWeekISO8601(DateTime.Today.Year, semaine - 1);

        String text = "A Remettre avant le : " + dateRemise.ToShortDateString();
        double middleText = getMiddelOfX(50, BOLD, text, 17);
        drawText(BOLD, 17, middleText, getY(96), text);
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
        contentStream.moveTo((float) startX, (float) startY);
        contentStream.lineTo((float) endX, (float) endY);
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
            contentStream.setFont(font, (float) fontSize);

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
        foreach (var s in text) {
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
        foreach (String s in text) {
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

            var fileDialog = new SaveFileDialog();
            var res  = fileDialog.ShowDialog();

            if (!res.HasValue) retval = false;
            else retval = res.Value;
           
            if(retval==true)
            {
                output = fileDialog.FileName;
            }

           

            return retval ;
            
            /*
        DirectoryChooser fileChooser = new DirectoryChooser();

        File selectedDir = fileChooser.showDialog(Main.getMainStage());

        File file;

        try
        {
            file = new File(selectedDir.toString() + File.separator + "Tournée semaine " + semaine + ".pdf");
        }
        catch (Exception e)
        {
            return false;
        }

        output = file;

        if (output.exists())
        {
            Optional<ButtonType> result = FxHelper.showMessage(-1, Alert.AlertType.CONFIRMATION, Resource.WORDS.pdf.pdfExist, Resource.WORDS.error.removeAllertMessage, null, true);

            if (result.get() == FxHelper.yesButton)
            {
                output.delete();
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return true;
        }*/
    }

    /**
     * Création d'une page et définition du stream de la page
     *
     * @throws IOException ...
     */
    private static void getDocument() 
    {
        blankPage = new PDPage(new PDRectangle((float) maxX, (float) maxY));
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
        return (start + center) - getMiddleOfText(font, text, (float) fontSize);
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
