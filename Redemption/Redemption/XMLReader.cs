using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Redemption
{
    /// <summary>
    /// Klasse die System.Xml.Linq benutzt um XML Objekte zu erstellen und diese im Filesystem zu speichern oder zu laden.
    /// </summary>
    class XMLReader
    {
        /// <summary>
        /// Bekommt eine Liste von Matchings und übersetzt sie in XML Elemente. Speichert danach die Datei unter dem übergebenen Pfad.
        /// </summary>
        /// <param name="path">Der Pfad in dem die XML Datei gespeichert werden soll.</param>
        /// <param name="list">Die Liste an Matchings die gespeichert werden soll</param>
        public static void saveToXml (string path, List<Matching> list)
        {
            try
            {
                //WRITE XML
                var doc = new XDocument();

                //doc.Add(new XElement("MatchingList", list.Select(x => new XElement("MatchingEntry",
                //        new XAttribute("Subject", x.Subject),
                //        new XElement("PublicId", x.PublicId),
                //        new XElement("MailboxId", x.MailboxId)))));

                XElement xml = new XElement("MatchingList",
                                from p in list
                                select new XElement("MatchingEntry",
                                            new XElement("Subject", p.Subject),
                                            new XElement("PublicId", p.PublicId),
                                            new XElement("MailboxId", p.MailboxId)));

                doc.Add(xml);
                doc.Save(path);

            }
            catch (Exception ex)
            {
                //Console.WriteLine("XMLReader.cs - 38: "+ex.Message);
                ExchangeSync.writeLog("XMLReader.cs - 38: " + ex.Message);
            }
        }

        /// <summary>
        /// Eine XML Datei wird eingelesen und in Matchings übersetzt.
        /// </summary>
        /// <param name="path">Der Pfad der XML Datei die eingelesen werden soll.</param>
        /// <returns>Eine Liste von Matchings</returns>
        public static List<Matching> readFromXml (string path)
        {
            List<Matching> _list = null;

            try
            {
                _list = (
                        from e in XDocument.Load(path).Root.Elements("MatchingEntry")
                        select new Matching
                        {
                            MailboxId = (string)e.Element("MailboxId"),
                            PublicId = (string)e.Element("PublicId"),
                            Subject = (string)e.Element("Subject")
                        }
                    ).ToList();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("ERROR: " + ex.Message);
                ExchangeSync.writeLog("XMLReader.cs - 62: " + ex.Message);
            }

            return _list;
        }
    }

    
}
