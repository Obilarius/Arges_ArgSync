using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADtoEXSync
{
    public static class EXWorker
    {
        #region Property Definitions
        static ExtendedPropertyDefinition extendedPropertyDefinitionADSID = new ExtendedPropertyDefinition(new Guid("53617363-6861-5761-6c7a-656e62616368"), "ADSID", MapiPropertyType.String);
        #endregion

        public static ExchangeService ExchangeConnect(string username, string password, string domain, string smtpAdresse, string exUri)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(username, password, domain);

            service.Url = new Uri(exUri);

            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, smtpAdresse);

            return service;
        }


        public static Folder GetMailboxFolder(ExchangeService service, string contactFolderName, bool init = false)
        {
            var MailboxContactRoot = Folder.Bind(service, WellKnownFolderName.Contacts);
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, contactFolderName);
            FindFoldersResults FindMailboxContactFolder = service.FindFolders(MailboxContactRoot.Id, filter, new FolderView(1));

            Folder MailboxContactFolder;
            if (FindMailboxContactFolder.TotalCount != 0)
            {
                //löscht den Kontakt Ordner falls er beim Initialen SyncRun vorhanden ist
                if (init)
                {
                    try
                    {
                        FindMailboxContactFolder.Folders[0].Delete(DeleteMode.HardDelete);
                    }
                    catch (Exception)
                    {
                        throw new Exception(contactFolderName + " wurde nicht gelöscht");
                    }


                    ContactsFolder folder = new ContactsFolder(service);
                    folder.DisplayName = contactFolderName;
                    folder.Save(MailboxContactRoot.Id);
                    MailboxContactFolder = folder;
                }
                else
                {
                    MailboxContactFolder = FindMailboxContactFolder.Folders[0];
                }
            }
            else
            {
                ContactsFolder folder = new ContactsFolder(service);
                folder.DisplayName = contactFolderName;
                folder.Save(MailboxContactRoot.Id);
                MailboxContactFolder = folder;
            }

            return MailboxContactFolder;
        }

        public static Folder getPublicFolder(ExchangeService service, string contactFolderName)
        {
            var PublicRoot = Folder.Bind(service, WellKnownFolderName.PublicFoldersRoot);
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, contactFolderName);
            FindFoldersResults FindPublicContactFolder = service.FindFolders(PublicRoot.Id, filter, new FolderView(1));
            return FindPublicContactFolder.Folders[0];
        }


        public static void CreateContact(ExchangeService service, Folder folder, UserPrincipal user)
        {
            // Create the contact.
            Contact contact = new Contact(service);

            contact.GivenName = user.GivenName;
            contact.MiddleName = user.MiddleName;
            contact.Surname = user.Surname;
            contact.FileAsMapping = FileAsMapping.SurnameCommaGivenName;

            contact.EmailAddresses[EmailAddressKey.EmailAddress1] = (user.EmailAddress != null) ? new EmailAddress(user.EmailAddress) : null;

            contact.PhoneNumbers[PhoneNumberKey.BusinessPhone] = user.GetProperty("telephoneNumber");
            contact.PhoneNumbers[PhoneNumberKey.HomePhone] = user.GetProperty("homePhone");
            contact.PhoneNumbers[PhoneNumberKey.MobilePhone] = user.GetProperty("mobile");
            contact.PhoneNumbers[PhoneNumberKey.BusinessFax] = user.GetProperty("facsimileTelephoneNumber");

      
            PhysicalAddressEntry paEntry1 = new PhysicalAddressEntry();
            paEntry1.Street = user.GetProperty("streetAddress");
            paEntry1.City = user.GetProperty("l");
            paEntry1.State = user.GetProperty("st");
            paEntry1.PostalCode = user.GetProperty("postalCode");
            paEntry1.CountryOrRegion = user.GetProperty("co");
            contact.PhysicalAddresses[PhysicalAddressKey.Business] = paEntry1;


            contact.Department = user.GetProperty("department");
            contact.OfficeLocation = user.GetProperty("physicalDeliveryOfficeName");
            contact.Profession = user.GetProperty("title");
            contact.JobTitle = user.GetProperty("title");
            contact.NickName = user.GetProperty("initials");

            var manager = user.GetProperty("manager");
            manager = (manager != "") ? manager.Split(',')[0].Substring(3) : "";

            contact.Manager = manager;
            contact.CompanyName = user.GetProperty("company");

            var thumbnail = user.GetThumbnail();
            if (thumbnail != null)
                contact.SetContactPicture(thumbnail);


            contact.Categories.Add("autogenerated by Arges IT");


            
            contact.SetExtendedProperty(extendedPropertyDefinitionADSID, user.Sid.ToString());

            contact.Save(folder.Id);
        }

        public static List<string> GetAllADSIDsInFolderWhereAutogenarated(ExchangeService service, Folder folder)
        {
            ItemView view = new ItemView(int.MaxValue);
            view.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties, extendedPropertyDefinitionADSID);
            FindItemsResults<Item> findResults = service.FindItems(folder.Id, view);


            List<string> retList = new List<string>();
            foreach (Item item in findResults.Items)
            {
                if(item.Categories.Contains("autogenerated by Arges IT"))
                {
                    string ADSID;
                    if (item.ExtendedProperties.Count > 0)
                    {
                        item.TryGetProperty(extendedPropertyDefinitionADSID, out ADSID);
                        retList.Add(ADSID);
                    }
                }
            }

            return retList;
        }

        public static void DeleteContactWithADSID(ExchangeService service, Folder folder, string sid)
        {
            ItemView view = new ItemView(int.MaxValue);
            view.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties, extendedPropertyDefinitionADSID);
            FindItemsResults<Item> findResults = service.FindItems(folder.Id, view);

            foreach (Item item in findResults.Items)
            {
                if (item.Categories.Contains("autogenerated by Arges IT"))
                {
                    string ADSID = "";
                    if (item.ExtendedProperties.Count > 0)
                    {
                        item.TryGetProperty(extendedPropertyDefinitionADSID, out ADSID);

                        if (ADSID == sid)
                        {
                            item.Delete(DeleteMode.HardDelete);
                        }
                    }
                }
            }
        }



        #region Helper
        static String GetProperty(this Principal principal, String property)
        {
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            if (directoryEntry.Properties.Contains(property))
                return directoryEntry.Properties[property].Value.ToString();
            else
                return String.Empty;
        }

        static byte[] GetThumbnail(this Principal principal)
        {
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            if (directoryEntry.Properties.Contains("thumbnailPhoto"))
                return directoryEntry.Properties["thumbnailPhoto"].Value as byte[];
            else
                return null;
        }
        #endregion
    }
}
