using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class MatchingList
    {
        public static void Create (ExchangeService service, string SMTPAdresse, string ContactFolderName)
        {
            if (service != null)
            {

                var path = "MatchingList/" + SMTPAdresse + "_" + ContactFolderName + "_matchingList.xml";

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



                if (!Directory.Exists("MatchingList"))
                {
                    Directory.CreateDirectory("MatchingList");
                }

                XMLReader.saveToXml(path, matchingList);
            }
        }

        public static List<Matching> GetList (string SMTPAdresse, string ContactFolderName)
        {
            var path = "MatchingList/" + SMTPAdresse + "_" + ContactFolderName + "_matchingList.xml";

            List<Matching> _list = null;
            _list = XMLReader.readFromXml(path);
            return _list;
        }
    }

    public class Matching
    {
        public string MailboxId { get; set; }
        public string PublicId { get; set; }
        public string Subject { get; set; }

        // Konstruktoren 
        public Matching() { }

        public Matching(string subject, string publicId, string mailboxId)
        {
            this.Subject = subject;
            this.PublicId = publicId;
            this.MailboxId = mailboxId;
        }
    }
}
