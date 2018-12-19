using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using System.IO;

namespace Redemption
{
    class ExchangeSync
    {
        ExchangeService service = null;

        String password = "redemption";
        String username = "redemption";
        String domain = "arges";
        String exUri = "https://helios.arges.local/EWS/Exchange.asmx";
        String SMTPAdresse;
        String ContactFolderName;


        public ExchangeSync(string _SMTPAdresse, string _ContactFolderName)
        {
            SMTPAdresse = _SMTPAdresse;
            ContactFolderName = _ContactFolderName;
        }

        public void Sync()
        {
            service = ExchangeConnect(username, password, domain, SMTPAdresse, exUri);
            if (service != null)
            {
                var PublicContactFolder = getPublicFolder(ContactFolderName);
                var MailboxContactFolder = getMailboxFolder(ContactFolderName);

                #region LOCAL SYNC
                bool isEndOfChanges = false;
                var localSyncState = getSyncState(false, SMTPAdresse); // Ist null falls Initialer SyncRun

                do
                {
                    ChangeCollection<ItemChange> icc_mailbox = service.SyncFolderItems(MailboxContactFolder.Id, PropertySet.FirstClassProperties, null, 512, SyncFolderItemsScope.NormalItems, localSyncState);

                    if (icc_mailbox.Count != 0)
                    {
                        //Folder MailboxFolder = Folder.Bind(service, MailboxContactFolder.Id);
                        foreach (ItemChange ic_mailbox in icc_mailbox)
                        {
                            if (ic_mailbox.ChangeType == ChangeType.Create)
                            {
                                Contact contacts = Contact.Bind(service, ic_mailbox.ItemId);
                                contacts.Delete(DeleteMode.HardDelete);

                                Console.WriteLine(SMTPAdresse + " - LocalChange " + contacts.Subject + " was created locally and removed automatically");
                            }
                            else if (ic_mailbox.ChangeType == ChangeType.Update)
                            {
                                Contact contacts = Contact.Bind(service, ic_mailbox.ItemId);

                                SearchFilter.IsEqualTo filter2 = new SearchFilter.IsEqualTo(ItemSchema.Subject, contacts.Subject);
                                FindItemsResults<Item> findResults = service.FindItems(PublicContactFolder.Id, filter2, new ItemView(1));
                                contacts.Delete(DeleteMode.HardDelete);
                                foreach (Contact item in findResults.Items)
                                {
                                    item.Copy(MailboxContactFolder.Id);
                                }

                                Console.WriteLine(SMTPAdresse + " - LocalChange " + contacts.Subject + " was updated locally and removed automatically");
                            }
                            else if (ic_mailbox.ChangeType == ChangeType.Delete)
                            {
                                //Contact contacts = Contact.Bind(service, ic_mailbox.ItemId.UniqueId);

                                //SearchFilter.IsEqualTo filter2 = new SearchFilter.IsEqualTo(ItemSchema.Subject, contacts.Subject);
                                //FindItemsResults<Item> findResults = service.FindItems(PublicContactFolder.Id, filter2, new ItemView(1));
                                //foreach (Contact item in findResults.Items)
                                //{
                                //    item.Copy(MailboxContactFolder.Id);
                                //}
                            }

                            //var OutputText = ic_mailbox.ChangeType.ToString() + " - ";
                            //if (ic_mailbox.Item != null) { OutputText += ic_mailbox.Item.Subject; }
                            //Console.WriteLine(OutputText);
                        }

                        Console.WriteLine(icc_mailbox.Count + " changes in own mailbox folder");
                        writeLog(SMTPAdresse + " - " + icc_mailbox.Count + " changes in own mailbox folder");
                    }

                    if (!icc_mailbox.MoreChangesAvailable)
                    {
                        isEndOfChanges = true;
                    }

                } while (!isEndOfChanges);
                #endregion

                #region PUBLIC SYNC
                bool isEndOfChangesPublic = false;
                var sSyncState = getSyncState(true, SMTPAdresse);
                var index = 0;

                if (index > 513 && sSyncState == null)
                    throw new NullReferenceException();

                do
                {
                    ChangeCollection<ItemChange> icc = service.SyncFolderItems(PublicContactFolder.Id, PropertySet.FirstClassProperties, null, 512, SyncFolderItemsScope.NormalItems, sSyncState);

                    if (icc.Count == 0)
                    {
                        writeLog(SMTPAdresse + " - There are no item changes to synchronize.");
                    }
                    else
                    {

                        writeLog(SMTPAdresse + " - " + icc.Count + " changes in public folder");

                        foreach (ItemChange ic in icc)
                        {
                            if (ic.ChangeType == ChangeType.Create)
                            {
                                Contact contacts = Contact.Bind(service, ic.ItemId);
                                contacts.Copy(MailboxContactFolder.Id);


                                //// Retrieve a collection of all the mail (limited to 10 items) in the Inbox. View the extended property.
                                //ItemView view = new ItemView(10000);
                                //PropertySet PropertySet = new PropertySet(BasePropertySet.FirstClassProperties);

                                //Guid MyPropertySetId = new Guid("{00062004-0000-0000-C000-000000000046}");
                                //ExtendedPropertyDefinition extendedPropertyDefinitionUser4 = new ExtendedPropertyDefinition(MyPropertySetId, 0x8051, MapiPropertyType.String);
                                //PropertySet.Add(extendedPropertyDefinitionUser4);
                                //view.PropertySet = PropertySet;
                                //FindItemsResults<Item> findResults = service.FindItems(MailboxContactFolder.Id, view);

                                //foreach (Item item in findResults.Items)
                                //{
                                //    var User4 = string.Empty;
                                //    item.TryGetProperty(extendedPropertyDefinitionUser4, out User4);

                                //    if (User4 != "")
                                //    {
                                //        item.SetExtendedProperty(extendedPropertyDefinitionUser4, contacts.Id.ChangeKey);
                                //        item.Update(ConflictResolutionMode.AlwaysOverwrite);
                                //    }
                                //}

                                //Console.WriteLine(SMTPAdresse + " -" + index + " - PublicChange " + contacts.Subject + " was created in public and copied to the mailbox");
                            }
                            else if (ic.ChangeType == ChangeType.Update)
                            {
                                Contact contacts = Contact.Bind(service, ic.ItemId);

                                SearchFilter.IsEqualTo filter2 = new SearchFilter.IsEqualTo(ItemSchema.Subject, contacts.Subject);
                                FindItemsResults<Item> findResults = service.FindItems(MailboxContactFolder.Id, filter2, new ItemView(1));
                                foreach (Contact item in findResults.Items)
                                {
                                    item.Delete(DeleteMode.HardDelete);
                                    contacts.Copy(MailboxContactFolder.Id);
                                }

                                //Console.WriteLine(SMTPAdresse + " - " + index + " - PublicChange " + contacts.Subject + " was updated in public and updated in the mailbox");
                            }
                            else if (ic.ChangeType == ChangeType.Delete)
                            {



                                //Contact contacts = Contact.Bind(service, ic.ItemId);

                                //SearchFilter.IsEqualTo filter2 = new SearchFilter.IsEqualTo(ItemSchema.Subject, contacts.Subject);
                                //FindItemsResults<Item> findResults = service.FindItems(MailboxContactFolder.Id, filter2, new ItemView(1));
                                //foreach (Contact item in findResults.Items)
                                //{
                                //    item.Delete(DeleteMode.HardDelete);
                                //}
                            }
                            else if (ic.ChangeType == ChangeType.ReadFlagChange)
                            {
                                //TODO: Update the item's read flag on the client.
                            }


                            // AUSGABE IN CONSOLE
                            //Console.CursorTop = 1;
                            //var percent = Math.Floor(Remap(index, 0, icc.Count, 1, 100));
                            //showProgressBar((int)percent);

                            var OutputText = index + " - " + ic.ChangeType.ToString() + " - ";
                            if (ic.Item != null) { OutputText += ic.Item.Subject; }
                            Console.WriteLine(OutputText);

                            index++;
                        }
                    }

                    // TODO Könnte sein das ich den Hash erst nach der Do Schleife schreiben darf  // Dateischreibzugriffe in Variable verschieben
                    sSyncState = icc.SyncState;

                    if (!icc.MoreChangesAvailable)
                    {
                        isEndOfChangesPublic = true;
                    }
                } while (!isEndOfChangesPublic);

                writeSyncState(sSyncState, true, SMTPAdresse);
                #endregion

                #region GET AND SET LOCAL SYNC
                bool isEndOfChangesLocal = false;
                var sSyncStateLocal = getSyncState(false, SMTPAdresse);

                do
                {
                    ChangeCollection<ItemChange> icc_mailbox2 = service.SyncFolderItems(MailboxContactFolder.Id, PropertySet.FirstClassProperties, null, 512, SyncFolderItemsScope.NormalItems, sSyncStateLocal);
                    sSyncStateLocal = icc_mailbox2.SyncState;

                    if (!icc_mailbox2.MoreChangesAvailable)
                    {
                        isEndOfChangesLocal = true;
                    }
                } while (!isEndOfChangesLocal);

                writeSyncState(sSyncStateLocal, false, SMTPAdresse);
                #endregion

            }
        }

        public Folder getPublicFolder(string FolderName)
        {
            var PublicRoot = Folder.Bind(service, WellKnownFolderName.PublicFoldersRoot);
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, FolderName);
            FindFoldersResults FindPublicContactFolder = service.FindFolders(PublicRoot.Id, filter, new FolderView(1));
            return FindPublicContactFolder.Folders[0];
        }

        public Folder getMailboxFolder(string FolderName, bool init = false)
        {
            var MailboxContactRoot = Folder.Bind(service, WellKnownFolderName.Contacts);
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, FolderName);
            FindFoldersResults FindMailboxContactFolder = service.FindFolders(MailboxContactRoot.Id, filter, new FolderView(1));

            Folder MailboxContactFolder;
            if (FindMailboxContactFolder.TotalCount != 0)
            {
                // löscht den Kontakt Ordner falls er beim Initialen SyncRun vorhanden ist
                //if (init)
                //{
                //    FindMailboxContactFolder.Folders[0].Delete(DeleteMode.HardDelete);
                //}
                MailboxContactFolder = FindMailboxContactFolder.Folders[0];
            }
            else
            {
                ContactsFolder folder = new ContactsFolder(service);
                folder.DisplayName = FolderName;
                folder.Save(MailboxContactRoot.Id);
                MailboxContactFolder = folder;
            }

            return MailboxContactFolder;
        }

        public ExchangeService ExchangeConnect(string username, string password, string domain, string smtpAdresse, string exUri)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(username, password, domain);

            //service.AutodiscoverUrl("walzenbach@arges.de");
            service.Url = new Uri(exUri);

            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, smtpAdresse);

            return service;
        }

        public string getSyncState(bool isPublic, string smtpAdresse)
        {
            String SyncState = null;
            String path = "SyncStates/" + smtpAdresse + "_" + ContactFolderName;
            if (isPublic)
            {
                path += "_public";
            }
            path += ".dat";

            if (File.Exists(path))
            {
                StreamReader SyncStateReader = new StreamReader(path);
                SyncState = SyncStateReader.ReadLine();
                SyncStateReader.Close();
            }
            return SyncState;
        }

        public void writeSyncState(string syncState, bool isPublic, string smtpAdresse)
        {
            String path = "SyncStates/" + smtpAdresse + "_" + ContactFolderName;
            if (isPublic)
            {
                path += "_public";
            }
            path += ".dat";

            if (!Directory.Exists("SyncStates"))
            {
                Directory.CreateDirectory("SyncStates");
            }
            StreamWriter SyncStateWriter = new StreamWriter(path);
            SyncStateWriter.Write(syncState);
            SyncStateWriter.Close();
        }

        public void writeLog(string logText)
        {
            if (!Directory.Exists("log"))
            {
                Directory.CreateDirectory("log");
            }
            var log = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToLongTimeString() + " - " + logText;
            var filename = "log/" + DateTime.Now.ToString("yyyy-MM") + "_Log.txt";
            StreamWriter SyncStateWriter = new StreamWriter(filename, true);
            SyncStateWriter.WriteLine(log);
            SyncStateWriter.Close();
        }

        public float Remap(float value, float from1, float to1, float from2, float to2)
        {
            var val = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            return val;
        }

        public void showProgressBar(int percent)
        {
            Console.SetCursorPosition(0, 1);
            Console.Write("[");
            for (int i = 0; i < percent / 2; i++)
            {
                Console.Write("#");
            }

            Console.CursorLeft = 51;
            Console.Write("] " + percent + "%");
            Console.WriteLine();
        }
    }
}
