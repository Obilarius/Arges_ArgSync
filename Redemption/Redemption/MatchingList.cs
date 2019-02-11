using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    /// <summary>
    /// Die Klasse wird verwendet um eine Matchingliste zu speichern oder einzulesen.
    /// </summary>
    public class MatchingList
    {
        /// <summary>
        /// Läuft mit einer Schleife über alle Kontakte im Ordner (zb. "Arges Intern") des Postfaches.
        /// Es wird das Subject, UniqueId und das externe Feld "PublicID" ausgelesen und als Matching in einer Liste gespeichert.
        /// Nach der Schleife wird überprüft ob der Ordner "MatchingList" vorhanden ist, falls nicht wird er erstellt,
        /// und die Liste mit Matchings wird über die Klasse XMLReader in eine Datei gespeichert. <br/>
        /// <i>Die Listen werden verwendet um zu jedem Kontakt in jedem Postfach sein Pendant aus dem öffentlichen Ordner zu finden.</i>
        /// </summary>
        /// <param name="service">Das ExchangeService Objekt</param>
        /// <param name="SMTPAdresse">Die SMTP Adresse</param>
        /// <param name="ContactFolderName">Der Name der Ordners (zb. "Arges Intern")</param>
        public static void Create (ExchangeService service, string SMTPAdresse, string ContactFolderName)
        {
            if (service != null)
            {
                var path = ExchangeSync.binaryPath + @"\MatchingList\" + SMTPAdresse + "_" + ContactFolderName + "_matchingList.xml";

                var PublicRoot = Folder.Bind(service, WellKnownFolderName.Contacts);
                SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, ContactFolderName);
                FindFoldersResults FindPublicContactFolder = service.FindFolders(PublicRoot.Id, filter, new FolderView(1));
                var ContactFolder = FindPublicContactFolder.Folders[0];

                Guid MyPropertySetId = new Guid("{57616c7a-656e-6261-6368-536173636861}");
                ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(MyPropertySetId, "PublicID", MapiPropertyType.String);

                // EXTENDED PROP READ
                ItemView view = new ItemView(int.MaxValue);
                view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, extendedPropertyDefinition);
                FindItemsResults<Item> findResults;

                var matchingList = new List<Matching>();

                do
                {
                    findResults = service.FindItems(ContactFolder.Id, view);

                    foreach (Item item in findResults.Items)
                    {
                        string PublicID;
                        if (item.ExtendedProperties.Count > 0)
                        {
                            // Display the extended name and value of the extended property.
                            item.TryGetProperty(extendedPropertyDefinition, out PublicID);

                            var entry = new Matching(item.Subject, PublicID, item.Id.UniqueId);
                            matchingList.Add(entry);
                        }
                    }

                    view.Offset += findResults.Items.Count;
                } while (findResults.MoreAvailable == true);



                if (!Directory.Exists(ExchangeSync.binaryPath + @"\MatchingList"))
                {
                    Directory.CreateDirectory(ExchangeSync.binaryPath + @"\MatchingList");
                }

                XMLReader.saveToXml(path, matchingList);
            }
        }

        /// <summary>
        /// Liest über die Klasse XMLReader die Matchingliste ein.
        /// </summary>
        /// <param name="SMTPAdresse">Die SMTP Adresse</param>
        /// <param name="ContactFolderName">Der Name der Ordners (zb. "Arges Intern")</param>
        /// <returns></returns>
        public static List<Matching> GetList (string SMTPAdresse, string ContactFolderName)
        {
            var path = ExchangeSync.binaryPath + @"\MatchingList\" + SMTPAdresse + "_" + ContactFolderName + "_matchingList.xml";

            List<Matching> _list = null;
            _list = XMLReader.readFromXml(path);
            return _list;
        }
    }

    /// <summary>
    /// Stellt die einzelnen Einträge einer MatchinListe dar.
    /// </summary>
    public class Matching
    {
        /// <summary>
        /// Die UniqueId die der Kontakt im Postfach hat.
        /// </summary>
        public string MailboxId { get; set; }
        /// <summary>
        /// Dei UniqueId die der Kontakt im öffentlichen Ordner hat.
        /// </summary>
        public string PublicId { get; set; }
        /// <summary>
        /// Subject des Kontaktes.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Kontruktor
        /// </summary>
        public Matching() { }

        /// <summary>
        /// Kontruktor
        /// </summary>
        /// <param name="subject">Subject des Kontaktes.</param>
        /// <param name="publicId">Dei UniqueId die der Kontakt im öffentlichen Ordner hat.</param>
        /// <param name="mailboxId">Die UniqueId die der Kontakt im Postfach hat.</param>
        public Matching(string subject, string publicId, string mailboxId)
        {
            this.Subject = subject;
            this.PublicId = publicId;
            this.MailboxId = mailboxId;
        }
    }
}
