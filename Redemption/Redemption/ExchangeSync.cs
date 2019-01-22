﻿using System;
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

        public static String binaryPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); // + @"\config.cfg";


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

                //makeChangeKey(SMTPAdresse, MailboxContactFolder.Id, "Anfang"); // DEBUG
                var changeKeys = "";
                do
                {
                    ChangeCollection<ItemChange> icc_mailbox = service.SyncFolderItems(MailboxContactFolder.Id, PropertySet.FirstClassProperties, null, 512, SyncFolderItemsScope.NormalItems, localSyncState);

                    if (icc_mailbox.Count != 0)
                    {
                        changeValue = true;
                        writeLog(SMTPAdresse + " - " + icc_mailbox.Count + " changes in own mailbox folder");

                        foreach (ItemChange ic_mailbox in icc_mailbox)
                        {
                            Console.WriteLine(ic_mailbox.ChangeType);

                            if (ic_mailbox.ChangeType == ChangeType.Create)
                            {
                                try
                                {
                                    Contact contacts = Contact.Bind(service, ic_mailbox.ItemId);
                                    //writeLog(contacts.Subject + " - Create");
                                    contacts.Delete(DeleteMode.HardDelete);
                                }
                                catch (Exception ex)
                                {
                                    writeLog("ERROR: LocalSync Create: " + ex.Message);
                                }
                                

                                //Console.WriteLine(SMTPAdresse + " - LocalChange " + contacts.Subject + " was created locally and removed automatically");
                            }
                            else if (ic_mailbox.ChangeType == ChangeType.Update)
                            {
                                // Wird nicht benutzt da der ChacheModus von Outlook die ChangeKeys ändert und damit ca alle 30min ein InitialerSync stattfindet
                                // Evtl eigenen Hash über Felder sobalt die Kontakte aus SAP kommen alternativ anderes Feld mit dem letzten Änderungsdatum.
                                // Lokale Änderungen würden sowieso nicht zurück in den Öffentlichen Ordner geschrieben, sondern nur mit den Daten von dort ersetzt werden.
                                // Damit werden lokale Änderungen (z.B Name, Tel ... ) vom Benutzer nicht mehr berücksichtigt, sondern nur noch Create und Delete. 
                                try
                                {
                                    Contact contacts = Contact.Bind(service, ic_mailbox.ItemId);
                                    //writeLog(contacts.Subject + " - Update");
                                    changeKeys += contacts.Subject + " -- " + contacts.Id.ChangeKey + System.Environment.NewLine;

                                    contacts.Delete(DeleteMode.HardDelete);

                                    var MailboxId = ic_mailbox.ItemId.UniqueId;
                                    List<Matching> matchingList = MatchingList.GetList(SMTPAdresse, ContactFolderName);
                                    Matching result = matchingList.Find(x => x.MailboxId == MailboxId);

                                    Contact PublicContacts = Contact.Bind(service, result.PublicId);
                                    PublicContacts.Copy(MailboxContactFolder.Id);
                                }
                                catch (Exception ex)
                                {
                                    writeLog("ERROR: LocalSync Update: " + ex.Message);
                                }

                            }
                            else if (ic_mailbox.ChangeType == ChangeType.Delete)
                            {
                                var MailboxId = ic_mailbox.ItemId.UniqueId;

                                try
                                {
                                    List<Matching> matchingList = MatchingList.GetList(SMTPAdresse, ContactFolderName);
                                    Matching result = matchingList.Find(x => x.MailboxId == MailboxId);

                                    if (result == null)
                                    {
                                        writeLog("ERROR: No match in MatchingList for: " + MailboxId);
                                    }
                                    else
                                    {
                                        Contact contacts = Contact.Bind(service, result.PublicId);
                                        //writeLog(contacts.Subject + " - Delete");
                                        contacts.Copy(MailboxContactFolder.Id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    writeLog("ERROR: LocalSync Delete: " + ex.Message);
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

                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                File.WriteAllText(binaryPath + @"\changeKeys\ChangeKeys_" + ContactFolderName + SMTPAdresse + "_" + unixTimestamp, changeKeys);

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
                                try
                                {
                                    List<Matching> matchingList = MatchingList.GetList(SMTPAdresse, ContactFolderName);
                                    Matching result = matchingList.Find(x => x.PublicId == ic.ItemId.UniqueId);

                                    Contact LocalContact = Contact.Bind(service, result.MailboxId);
                                    LocalContact.Delete(DeleteMode.HardDelete);

                                    Contact PublicContact = Contact.Bind(service, ic.ItemId);
                                    PublicContact.Copy(MailboxContactFolder.Id);
                                }
                                catch (Exception ex)
                                {
                                    writeLog("ERROR: ExchangeSync.cs - 179: " + ex.Message);
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


                //makeChangeKey(SMTPAdresse, MailboxContactFolder.Id, "Ende"); // DEBUG
                
                stopWatch.Stop();
                writeLog("---------- SyncRun End - "+ stopWatch.Elapsed +" ----------");


                
            }
            return changeValue;
        }

        public void makeChangeKey(string SMTPAdresse, FolderId Id, string fileName = "")
        {
            ItemView itemView = new ItemView(int.MaxValue);
            FindItemsResults<Item> searchResults = service.FindItems(Id, itemView);

            var changeKeys = "<?xml version=\"1.0\" encoding=\"utf-8\"?><changeKeysList>";

            foreach (Item item in searchResults)
            {
                changeKeys += "<ChangeKeyEntry>";
                changeKeys += "<Subject>" + item.Subject + "</Subject>";
                changeKeys += "<UniqueId>" + item.Id.UniqueId + "</UniqueId>";
                changeKeys += "<ChangeKey>" + item.Id.ChangeKey + "</ChangeKey>";
                changeKeys += "</ChangeKeyEntry>";
            }

            changeKeys += "</changeKeysList>";
            var time = System.DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
            File.WriteAllText("ChangeKeys/" + SMTPAdresse + "-" + time +"-"+ fileName + ".xml", changeKeys);
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
                    try
                    {
                        FindMailboxContactFolder.Folders[0].Delete(DeleteMode.HardDelete);
                    }
                    catch (Exception)
                    {
                        throw new Exception(ContactFolderName + " wurde nicht gelöscht");
                    }
                    

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
            String path = binaryPath + @"\SyncStates\" + smtpAdresse + @"\" + ContactFolderName;
            if (isPublic)
            {
                path += "_public";
            }
            path += ".dat";

            if (File.Exists(path))
            {
                //StreamReader SyncStateReader = new StreamReader(path);
                //SyncState = SyncStateReader.ReadLine();
                //SyncStateReader.Close();

                SyncState = File.ReadAllText(path);
            }
            return SyncState;
        }

        public void writeSyncState(string syncState, bool isPublic, string smtpAdresse)
        {
            String path = binaryPath + @"\SyncStates\" + smtpAdresse + @"\" + ContactFolderName;
            if (isPublic)
            {
                path += "_public";
            }
            path += ".dat";

            if (!Directory.Exists(binaryPath + @"\SyncStates"))
            {
                Directory.CreateDirectory(binaryPath + @"\SyncStates");
            }
            if (!Directory.Exists(binaryPath + @"\SyncStates\" + smtpAdresse))
            {
                Directory.CreateDirectory(binaryPath + @"\SyncStates\" + smtpAdresse);
            }
            //StreamWriter SyncStateWriter = new StreamWriter(path);
            //SyncStateWriter.Write(syncState);
            //SyncStateWriter.Close();

            FileInfo file = new FileInfo(path);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            File.WriteAllText(file.FullName, syncState);
        }

        public static void writeLog(string logText)
        {
            var path = binaryPath + @"\log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var log = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToLongTimeString() + " - " + logText;
            var filename = path + @"\" + DateTime.Now.ToString("yyyy-MM") + "_Log.txt";
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
