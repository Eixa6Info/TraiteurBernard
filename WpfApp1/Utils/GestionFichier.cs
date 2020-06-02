using org.apache.commons.logging;
using sun.tools.tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Resources;
using TraiteurBernardWPF.Properties;

namespace TraiteurBernardWPF.Utils
{
    class GestionFichier
    {
        Resources Resources = new Resources();
        TextWriterTraceListener log = new TextWriterTraceListener(@"C:\eixa6\Log\traceLancementEtFermetureLogiciel.log");
        static XmlDocument monFichier = new XmlDocument();
        private static string xmlNas = @"C:\eixa6\nas.xml";
        const string local = @"C:\eixa6\traiteur.db";
        private string nomFichier = "GestionFichier.cs";
        private string nas;
        private string nasNew;
        private string fileName;
        private string lockFile;
        string[] res;
        string nasOld = "";
        string messageBoxText = "";
        string caption = "";
        int erreur = 0;
        MessageBoxButton button = MessageBoxButton.OK;
        MessageBoxImage iconW = MessageBoxImage.Error;
        MessageBoxImage iconI = MessageBoxImage.Information;
        MessageBoxImage iconA = MessageBoxImage.Warning;

        public void testVariable()
        {
            if (File.Exists(xmlNas))
            {
                nas = RechercheCheminDansFichierXmlDbFichier(monFichier);
                nasNew = RechercheCheminDansFichierXmlVieuFichierDb(monFichier);
                fileName = RechercheCheminDansFichierXmlCheminNas(monFichier);
                lockFile = RechercheCheminDansFichierXmlLock(monFichier);
            }
            else
            {
                erreur = 1; // le fichier de nas est pas present on met l'erreur à 1
            }
        }
        public void CréationDuFichierXml()
        {
            XElement root = new XElement("nas",
            new XElement("chemin", "chemin du document DB dans le nas"),
            new XElement("dbFile", "chemin du fichier traiteur.db dans le nas"),
            new XElement("lock", "chemin du fichier lock dans le nas"),
            new XElement("oldDbFile", "chemin du fichier traiteur sans le db dans le nas")
            );
            root.Save(@"C:\eixa6\nas.xml");
        }

        public string RechercheCheminDansFichierXml(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("dbFile");
            string nas = "";
            foreach (XmlNode n in Nas)
            {
                nas = n.InnerText;
            }
            return nas;
        }

        public static string RechercheCheminDansFichierXmlLock(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("lock");
            string nas = "";
            foreach (XmlNode n in Nas)
            {
                nas = n.InnerText;
            }
            return nas;
        }

        public bool LeFichierTraiteurdbExisteSurNas(string nas)
        {
            return System.IO.File.Exists(nas);
        }

        public static string RechercheCheminDansFichierXmlCheminNas(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("chemin");
            string fileName = "";
            foreach (XmlNode n in Nas)
            {
                fileName = n.InnerText;
            }
            return fileName;
        }
        public static string RechercheCheminDansFichierXmlDbFichier(XmlDocument monFichier)
        {
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("dbFile");
            string nas = "";
            foreach (XmlNode n in Nas)
            {
                nas = n.InnerText;
            }
            return nas;
        }
        public static string RechercheCheminDansFichierXmlVieuFichierDb(XmlDocument monFichier)
        {
            DateTime date = DateTime.Now;
            string dateStr = "";
            string jour = date.Day.ToString();
            string mois = date.Month.ToString();
            string annee = date.Year.ToString();
            if (jour.Length == 1)
            {
                jour = "0" + date.Day;
            }
            if (mois.Length == 1)
            {
                mois = "0" + date.Month;
            }
            dateStr = jour + mois + annee;
            monFichier.Load(xmlNas);
            XmlNodeList Nas = monFichier.GetElementsByTagName("oldDbFile");
            string nasNew = "";
            foreach (XmlNode n in Nas)
            {
                nasNew = n.InnerText + dateStr + ".db";
            }
            return nasNew;
        }

        public bool LeNombreDeFichierEstSuppAUn(string file)
        {
            return file.Length > 1;
        }

        public bool LeFichierSelectonnéEstPasTraiteurdb(string nas, string file)
        {
            return file != nas;
        }

        public bool DeuxiemeSauvegardeSurNas(string[] res)
        {
            return res.Length == 1;
        }

        public bool PremiereSauvegardeSurNas(string local, string nas)
        {
            return System.IO.File.Exists(local) && !System.IO.File.Exists(nas);
        }

        public void FichierXMLExiste()
        {
            messageBoxText = Resources.MessagePopUpXml;
            caption = Resources.TitrePopUpAlerte;
            var res = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNo, iconA);
            switch (res)
            {
                case MessageBoxResult.Yes:
                    erreur = 3; //le fichier du nas est pas present mais on travail en local
                    LogHelper.WriteToFile(Resources.MessageLogXmlYesLocal, nomFichier);
                    break;
                case MessageBoxResult.No:
                    CréationDuFichierXml();
                    messageBoxText = Resources.MessagePopUpXmlNoLocal;
                    caption = Resources.TitrePopUpInfo;
                    MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, iconI);
                    LogHelper.WriteToFile(Resources.MessageLogXmlNoLocal, nomFichier);
                    Application.Current.Shutdown();
                    break;
            }
        }

        public void LockFileExiste()
        {
            if (File.Exists(lockFile))
            {
                LogHelper.WriteToFile(Resources.MessageLogFichierLockExiste, nomFichier);
                messageBoxText = Resources.MessagePopUpFicherLockExiste;
                caption = Resources.TitrePopUpAlerte;
                var res = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNo, iconA);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        File.Delete(lockFile);
                        LogHelper.WriteToFile(Resources.MessageLogFichierLockOk , nomFichier);
                        break;
                    case MessageBoxResult.No:
                        LogHelper.WriteToFile(Resources.MessageLogFichierLockKo , nomFichier);
                        Application.Current.Shutdown(); 
                        break;    
                }
            }
            else
            {
                File.Create(lockFile);
                LogHelper.WriteToFile(Resources.MessageLogFichierLockExistePas, nomFichier);
            }
        }

        public void ExisteSurNas()
        {
            if (LeFichierTraiteurdbExisteSurNas(nas))
            {
                try
                {
                    if (File.GetLastWriteTime(RechercheCheminDansFichierXmlDbFichier(monFichier)) >= File.GetLastWriteTime(local))
                    {
                        System.IO.File.Delete(local);
                        System.IO.File.Copy(nas, local);
                        LogHelper.WriteToFile(Resources.MessageLogCopyNasToLocal, nomFichier);
                        messageBoxText = Resources.MessagePopUpCopyNasToLocal;
                        caption = Resources.TitrePopUpInfo;
                        MessageBox.Show(messageBoxText, caption, button, iconI);
                    }
                    else
                    {
                        System.IO.File.Delete(nas);
                        System.IO.File.Copy(local, nas);
                        LogHelper.WriteToFile(Resources.MessageLogCopyLocalToNas, nomFichier);
                        messageBoxText = Resources.MessagePopUpCopyLocalToNas;
                        caption = Resources.TitrePopUpInfo;
                        MessageBox.Show(messageBoxText, caption, button, iconI);
                    }
                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = Resources.MessagePopUpEixa6Alerte;
                    caption = Resources.TitrePopUpAlerte;
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }
            }
            else
            {
                messageBoxText = Resources.MessagePopUpNasExistePas;
                caption = Resources.TitrePopUpInfo;
                var res = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNo, iconW);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        LogHelper.WriteToFile(Resources.MessageLogFichierDbPasSurNasTravailEnLocal, nomFichier);
                        return;
                        break;
                    case MessageBoxResult.No:
                        LogHelper.WriteToFile(Resources.MessageLogFichierDbPasSurNasIlCoupe, nomFichier);
                        Application.Current.Shutdown();
                        break;
                }
                return;
            }
        }

        public void EnregistrerSurNasEtQuitteApp()
        {
            if (System.IO.File.Exists(nas))
            {
                try
                {
                    res = System.IO.Directory.GetFiles(fileName);
                    foreach (string file in res)
                    {
                        if (LeFichierSelectonnéEstPasTraiteurdb(nas, file) && LeNombreDeFichierEstSuppAUn(file))
                        {
                            nasNew = file;
                        }
                    }
                    if (DeuxiemeSauvegardeSurNas(res))
                    {
                        // On copie le fichier traiteur.db au 
                        System.IO.File.Copy(nas, nasNew);
                        System.IO.File.Delete(nas);
                        System.IO.File.Copy(local, nas);
                        LogHelper.WriteToFile(Resources.MessageLogDeuxiemeSauvegarde, nomFichier);
                    }
                    else
                    {
                        nasOld = nasNew;
                        // On supprime le fichier traiteurdate.db
                        System.IO.File.Delete(nasNew);
                        // la fichier traiteur.db passe en traiteurdate.db
                        System.IO.File.Copy(nas, nasNew);
                        // On supprime le fichier traiteur.db
                        System.IO.File.Delete(nas);
                        // On copie le fichier traiteur.db en local sur le nas
                        System.IO.File.Copy(local, nas);

                        LogHelper.WriteToFile(Resources.MessageLogSauvegardeLocalToNas, nomFichier);
                    }

                    messageBoxText = Resources.MessagePopUpSauvegardeLocalToNas ;
                    caption = Resources.TitrePopUpInfo;
                    MessageBox.Show(messageBoxText, caption, button, iconI);
                    File.Delete(RechercheCheminDansFichierXmlLock(monFichier));

                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = Resources.MessagePopUpEixa6Alerte;
                    caption = Resources.TitrePopUpAlerte;
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }
            }
            else if (PremiereSauvegardeSurNas(local, nas) && erreur == 0)
            {
                try
                {
                    //On Copie le fichier traiteur.db de local au nas
                    System.IO.File.Copy(local, nas);
                    LogHelper.WriteToFile(Resources.MessageLogCopyFichierDbLocalToNas, nomFichier);
                    messageBoxText = Resources.MessagePopUpCopyFichierDbLocalToNas;
                    caption = Resources.TitrePopUpInfo;
                    MessageBox.Show(messageBoxText, caption, button, iconI);
                    File.Delete(RechercheCheminDansFichierXmlLock(monFichier));
                }
                catch (System.IO.IOException a)
                {
                    messageBoxText = Resources.MessagePopUpEixa6Alerte;
                    caption = Resources.TitrePopUpAlerte;
                    MessageBox.Show(messageBoxText, caption, button, iconW);
                    LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                    Console.WriteLine(a.Message);
                    return;
                }
            }
            else
            {
                LogHelper.WriteToFile(Resources.MessageLogFichierDbSaveLocalPasNas, nomFichier);
            }
        }
    }
}




   
