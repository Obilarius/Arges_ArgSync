using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Redemption
{

    public class ExchangeSync
    {
        ExchangeService service = null;

        String SMTPAdresse;
        String ContactFolderName;


        public ExchangeSync(ExchangeService _service, string _SMTPAdresse, string _ContactFolderName)
        {
            SMTPAdresse = _SMTPAdresse;
            ContactFolderName = _ContactFolderName;
            service = _service;
        }

        public bool Sync()
        {
            var changeValue = false;

            if (service != null)
            {
                var PublicContactFolder = getPublicFolder();

                writeLog("---------- SyncRun Start - "+ ContactFolderName +" ----------");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();


                #region LOCAL SYNC
                bool isEndOfChanges = false;
                var localSyncState = getSyncState(false, SMTPAdresse); // Ist null falls Initialer SyncRun

                var MailboxContactFolder = getMailboxFolder(localSyncState == null);

                do
                {
                    ChangeCollection<ItemChange> icc_mailbox = service.SyncFolderItems(MailboxContactFolder.Id, PropertySet.FirstClassProperties, null, 512, SyncFolderItemsScope.NormalItems, localSyncState);

                    if (icc_mailbox.Count != 0)
                    {
                        changeValue = true;
                        writeLog(SMTPAdresse + " - " + icc_mailbox.Count + " changes in own mailbox folder");

                        foreach (ItemChange ic_mailbox in icc_mailbox)
                        {
                            if (ic_mailbox.ChangeType == ChangeType.Create)
                            {
                                Contact contacts = Contact.Bind(service, ic_mailbox.ItemId);
                                contacts.Delete(DeleteMode.HardDelete);

                                //Console.WriteLine(SMTPAdresse + " - LocalChange " + contacts.Subject + " was created locally and removed automatically");
                            }
                            else if (ic_mailbox.ChangeType == ChangeType.Update)
                            {
                                Contact contacts = Contact.Bind(service, ic_mailbox.ItemId);

                                SearchFilter.IsEqualTo filter2 = new SearchFilter.IsEqualTo(ItemSchema.Subject, contacts.Subject);

                                try
                                {
                                    FindItemsResults<Item> findResults = service.FindItems(PublicContactFolder.Id, filter2, new ItemView(1));
                                    contacts.Delete(DeleteMode.HardDelete);
                                    foreach (Contact item in findResults.Items)
                                    {
                                        item.Copy(MailboxContactFolder.Id);
                                    }

                                    //Console.WriteLine(SMTPAdresse + " - LocalChange " + contacts.Subject + " was updated locally and removed automatically");
                                }
                                catch (Exception ex)
                                {
                                    writeLog("ERROR: ExchangeSync.cs - 85: "+ex.Message);
                                }
                                
                            }
                            else if (ic_mailbox.ChangeType == ChangeType.Delete)
                            {
                                var MailboxId = ic_mailbox.ItemId.UniqueId;

                                try
                                {
                                    List<Matching> matchingList = MatchingList.GetList(SMTPAdresse, ContactFolderName);
                                    Matching result = matchingList.Find(x => x.MailboxId == MailboxId);

                                    Contact contacts = Contact.Bind(service, result.PublicId);
                                    contacts.Copy(MailboxContactFolder.Id);
                                }
                                catch (Exception ex)
                                {
                                    writeLog("ERROR: ExchangeSync.cs - 103: " + ex.Message);
                                }
                            }
                        }
                        //Console.WriteLine(icc_mailbox.Count + " changes in own mailbox folder");
                    }

                    localSyncState = icc_mailbox.SyncState;

                    if (!icc_mailbox.MoreChangesAvailable)
                    {
                        isEndOfChanges = true;
                    }

                } while (!isEndOfChanges);

                writeSyncState(localSyncState, false, SMTPAdresse);
                #endregion

                #region PUBLIC SYNC
                bool isEndOfChangesPublic = false;
                var sSyncState = getSyncState(true, SMTPAdresse);
                var index = 0;


                do
                {
                    ChangeCollection<ItemChange> icc = service.SyncFolderItems(PublicContactFolder.Id, PropertySet.FirstClassProperties, null, 512, SyncFolderItemsScope.NormalItems, sSyncState);

                    if (icc.Count == 0)
                    {
                        writeLog(SMTPAdresse + " - There are no item changes to synchronize.");
                    }
                    else
                    {
                        changeValue = true;
                        writeLog(SMTPAdresse + " - " + icc.Count + " changes in public folder");

                        foreach (ItemChange ic in icc)
                        {
                            if (ic.ChangeType == ChangeType.Create)
                            {
                                try
                                {
                                    Contact contacts = Contact.Bind(service, ic.ItemId);
                                    contacts.Copy(MailboxContactFolder.Id);
                                }
                                catch (Exception ex)
                                {
                                    writeLog("ERROR: ExchangeSync.cs - 152: " + ic.Item.Subject + " - " + ex.Message);
                                }
                                
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
                                var PublicId = ic.ItemId.UniqueId;

                                List<Matching> matchingList = MatchingList.GetList(SMTPAdresse, ContactFolderName);

                                if (matchingList != null)
                                {
                                    try
                                    {
                                        Matching result = matchingList.Find(x => x.PublicId == PublicId);

                                        Contact contacts = Contact.Bind(service, result.MailboxId);
                                        contacts.Delete(DeleteMode.HardDelete);
                                    }
                                    catch (Exception ex)
                                    {
                                        writeLog("ERROR: ExchangeSync.cs - 187: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    writeLog(SMTPAdresse + " - Mailbox MatchingListe konnte nicht geladen werden");
                                }


                                //Contact contacts = Contact.Bind(service, ic.ItemId);

                                //SearchFilter.IsEqualTo filter2 = new SearchFilter.IsEqualTo(ItemSchema.Subject, contacts.Subject);
                                //FindItemsResults<Item> findResults = service.FindItems(MailboxContactFolder.Id, filter2, new ItemView(1));
                                //foreach (Contact item in findResults.Items)
                                //{
                                //    item.Delete(DeleteMode.HardDelete);
                                //}
                            }

                            //var OutputText = index + " - " + ic.ChangeType.ToString() + " - ";
                            //if (ic.Item != null) { OutputText += ic.Item.Subject; }
                            //Console.WriteLine(OutputText);

                            if (index % 50 == 0)
                            {
                                Console.WriteLine("SyncIndex: " + index);
                            }

                            index++;
                        }
                    }

                    
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

                //writeSyncState(localSyncState, false, SMTPAdresse);
                #endregion

                stopWatch.Stop();
                writeLog("---------- SyncRun End - "+ stopWatch.Elapsed +" ----------");


                
            }
            return changeValue;
        }

        public Folder getPublicFolder()
        {
            var PublicRoot = Folder.Bind(service, WellKnownFolderName.PublicFoldersRoot);
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, ContactFolderName);
            FindFoldersResults FindPublicContactFolder = service.FindFolders(PublicRoot.Id, filter, new FolderView(1));
            return FindPublicContactFolder.Folders[0];
        }

        public Folder getMailboxFolder(bool init = false)
        {
            var MailboxContactRoot = Folder.Bind(service, WellKnownFolderName.Contacts);
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, ContactFolderName);
            FindFoldersResults FindMailboxContactFolder = service.FindFolders(MailboxContactRoot.Id, filter, new FolderView(1));

            Folder MailboxContactFolder;
            if (FindMailboxContactFolder.TotalCount != 0)
            {
                //löscht den Kontakt Ordner falls er beim Initialen SyncRun vorhanden ist
                if (init)
                {
                    FindMailboxContactFolder.Folders[0].Delete(DeleteMode.HardDelete);

                    ContactsFolder folder = new ContactsFolder(service);
                    folder.DisplayName = ContactFolderName;
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
                folder.DisplayName = ContactFolderName;
                folder.Save(MailboxContactRoot.Id);
                MailboxContactFolder = folder;
            }

            return MailboxContactFolder;
        }

        public string getSyncState(bool isPublic, string smtpAdresse)
        {
            String SyncState = null;
            String path = "SyncStates/" + smtpAdresse + "/" + ContactFolderName;
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
            String path = "SyncStates/" + smtpAdresse + "/" + ContactFolderName;
            if (isPublic)
            {
                path += "_public";
            }
            path += ".dat";

            if (!Directory.Exists("SyncStates"))
            {
                Directory.CreateDirectory("SyncStates");
            }
            if (!Directory.Exists("SyncStates/" + smtpAdresse))
            {
                Directory.CreateDirectory("SyncStates/" + smtpAdresse);
            }
            StreamWriter SyncStateWriter = new StreamWriter(path);
            SyncStateWriter.Write(syncState);
            SyncStateWriter.Close();
        }

        public static void writeLog(string logText)
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

        public void writePublicIdInExProp()
        {
            if (service != null)
            {
                var PublicRoot = Folder.Bind(service, WellKnownFolderName.PublicFoldersRoot);
                SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, ContactFolderName);
                FindFoldersResults FindPublicContactFolder = service.FindFolders(PublicRoot.Id, filter, new FolderView(1));
                var ContactFolder = FindPublicContactFolder.Folders[0];

                Guid MyPropertySetId = new Guid("{57616c7a-656e-6261-6368-536173636861}");
                ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(MyPropertySetId, "PublicID", MapiPropertyType.String);

                // EXTENDED PROP READ
                ItemView view = new ItemView(int.MaxValue);
                view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, extendedPropertyDefinition);
                FindItemsResults<Item> findResults;

                var index = 0;
                do
                {
                    findResults = service.FindItems(ContactFolder.Id, view);

                    foreach (Item item in findResults.Items)
                    {
                        string PublicID;
                        if (item.ExtendedProperties.Count > 0)
                        {
                            item.TryGetProperty(extendedPropertyDefinition, out PublicID);
                        }
                        else
                        {
                            try
                            {
                                Contact contact = Contact.Bind(service, item.Id);
                                contact.SetExtendedProperty(extendedPropertyDefinition, item.Id.UniqueId);
                                contact.Update(ConflictResolutionMode.AlwaysOverwrite);

                                //Console.WriteLine(index + " - " + item.Subject + " - UniqueId in extendedProp geschrieben.");
                            }
                            catch (Exception)
                            {
                            }
                        }

                        index++;
                    }

                    view.Offset += findResults.Items.Count;
                } while (findResults.MoreAvailable == true);

            }
        }

    }
}
