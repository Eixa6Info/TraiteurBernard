using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TraiteurBernardWPF.Utils
{
    class GestionFichier
    {     
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

    }
}




   
