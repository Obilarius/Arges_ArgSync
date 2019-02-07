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
    /// <summary>
    /// Hier wird der Sync Status jedes eizelnen Postfachen überprüft und bei änderungen werden die Kontakte gelöscht, geupdatet oder hinzugefügt
    /// </summary>
    public class ExchangeSync
    {
        ExchangeService service = null;

        String SMTPAdresse;
        String ContactFolderName;

        /// <summary>
        /// Die Variable beinhalten den Pfad zur Binary. (zb. um das einlesen der Config Datei relativ zu machen)
        /// </summary>
        public static string binaryPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); // + @"\config.cfg";

        /// <summary>
        /// Konstruktor für die Klasse
        /// </summary>
        /// <param name="_service">Das ExchangeService Objekt</param>
        /// <param name="_SMTPAdresse">Die SMTP Adresse des Postfachs</param>
        /// <param name="_ContactFolderName">Der Name des Ordners (zb. "Arges Intern")</param>
        public ExchangeSync(ExchangeService _service, string _SMTPAdresse, string _ContactFolderName)
        {
            SMTPAdresse = _SMTPAdresse;
            ContactFolderName = _ContactFolderName;
            service = _service;
        }


        /// <summary>
        /// <h2>Hauptfunktion</h2>
        /// Hier werden die SyncStates der einzelnen Ordner verglichen und verarbeitet.
        /// Das alles wird umspannt von einer Stopuhr um im Log eine Ausgabe zu haben wie viel Zeit für jeden SyncRun gebraucht wird.
        /// <h3>LOCAL SYNC</h3>
        /// Zuerst wird der Kontakt Ordner im Postfach bearbeitet.
        /// Dazu wird der gespeicherte SyncStatus vom letzten Durchgang gelesen und mit dem aktuellen Status verglichen.
        /// In einer Schleife wird dann über jede erfasste änderung gegangen und in verschiedenen if Abfragen verarbeitet.
        /// Da die SyncStatus Abfrage nur 512 Änderungen zurückgibt, muss die Schleife so lange wiederholt werden bis es keine Änderungen mehr gibt.
        /// <ul>
        /// <li>Create : Kontakt wird wieder gelöscht</li>
        /// <li>Update : Werden aktuell ignoriert da der ChacheModus von Outlook auch immer wieder Änderungen vornimmt.</li>
        /// <li>Delete : Vom gelöschten Kontakt wird das public Pendant in der MatchingListe gesucht. Danach wird der Kontakt aus dem öffentlichen Ordner wieder kopiert.</li>
        /// </ul>
        /// <h3>PUBLIC SYNC</h3>
        /// Als Zweites wird der öffentliche Ordner bearbeitet.
        /// <i>siehe LOCAL SYNC</i>
        /// <ul>
        /// <li>Create : Kontakt wird in das Postfach kopiert.</li>
        /// <li>Update : Kontakt wird in der MatchingListe gesucht. Pendant im Postfach wird gelöscht und aus dem öffentlichen Ordner wieder kopiert.</li>
        /// <li>Delete : Kontakt wird in der MatchingListe gesucht. Pendant wird im Postfach gesucht und gelöscht. Es wird mit dem AppointmentSync der Jahrestag und Geburtstag aus dem Kalender gelöscht.</li>
        /// </ul>
        /// <h3>GET AND SET LOCAL SYNC</h3>
        /// Hier wird noch mal über die Änderungen im Postfach Ordner gegangen aber nichts bearbeitet. Nach der Schleife wird der neue SyncStatus für das Postfach geschrieben.
        /// Das muss im nach dem PUBLIC SYNC nochmal passieren das dur den PUBLIC SYNC änderungen am Postfach Ordner vorgenommen werden können.
        /// </summary>
        /// <returns></returns>
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
                do
                {
                    ChangeCollection<ItemChange> icc_mailbox = service.SyncFolderItems(MailboxContactFolder.Id, PropertySet.FirstClassProperties, null, 512, SyncFolderItemsScope.NormalItems, localSyncState);

                    if (icc_mailbox.Count != 0)
                    {
                        changeValue = true;
                        var c = 0;
                        var u = 0;
                        var d = 0;
                        //writeLog(SMTPAdresse + " - " + icc_mailbox.Count + " changes in own mailbox folder");

                        foreach (ItemChange ic_mailbox in icc_mailbox)
                        {
                            Console.WriteLine(ic_mailbox.ChangeType);
                            try
                            {
                                //changeKeys += ic_mailbox.ChangeType + "_" + ic_mailbox.Item.Subject + "_" + ic_mailbox.ItemId.ChangeKey + System.Environment.NewLine; //DEBUG
                            }
                            catch (Exception) {}
                            

                            if (ic_mailbox.ChangeType == ChangeType.Create)
                            {
                                c++;
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
                                u++;
                                // Wird nicht benutzt da der ChacheModus von Outlook die ChangeKeys ändert und damit ca alle 30min ein InitialerSync stattfindet
                                // Evtl eigenen Hash über Felder sobalt die Kontakte aus SAP kommen alternativ anderes Feld mit dem letzten Änderungsdatum.
                                // Lokale Änderungen würden sowieso nicht zurück in den Öffentlichen Ordner geschrieben, sondern nur mit den Daten von dort ersetzt werden.
                                // Damit werden lokale Änderungen (z.B Name, Tel ... ) vom Benutzer nicht mehr berücksichtigt, sondern nur noch Create und Delete. 

                                //try
                                //{
                                //    Contact contacts = Contact.Bind(service, ic_mailbox.ItemId);
                                //    //writeLog(contacts.Subject + " - Update");

                                //    contacts.Delete(DeleteMode.HardDelete);

                                //    var MailboxId = ic_mailbox.ItemId.UniqueId;
                                //    List<Matching> matchingList = MatchingList.GetList(SMTPAdresse, ContactFolderName);
                                //    Matching result = matchingList.Find(x => x.MailboxId == MailboxId);

                                //    Contact PublicContacts = Contact.Bind(service, result.PublicId);
                                //    PublicContacts.Copy(MailboxContactFolder.Id);
                                //}
                                //catch (Exception ex)
                                //{
                                //    writeLog("ERROR: LocalSync Update: " + ex.Message);
                                //}

                            }
                            else if (ic_mailbox.ChangeType == ChangeType.Delete)
                            {
                                d++;
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

                        writeLog(SMTPAdresse + " - " + icc_mailbox.Count + " changes ("+c+" created, "+u+" updated, "+d+" deleted) in own mailbox folder");
                        writeLog("LOCAL UPDATES ARE IGNORED!");
                    }

                    localSyncState = icc_mailbox.SyncState;

                    if (!icc_mailbox.MoreChangesAvailable)
                    {
                        isEndOfChanges = true;
                    }

                } while (!isEndOfChanges);

                // DEBUG
                //Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                //File.WriteAllText(binaryPath + @"\changeKeys\ChangeKeys_" + ContactFolderName + SMTPAdresse + "_" + unixTimestamp, changeKeys);

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
                        var c = 0;
                        var u = 0;
                        var d = 0;
                        changeValue = true;
                        //writeLog(SMTPAdresse + " - " + icc.Count + " changes in public folder");

                        foreach (ItemChange ic in icc)
                        {
                            if (ic.ChangeType == ChangeType.Create)
                            {
                                c++;
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
                                u++;
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
                                d++;
                                var PublicId = ic.ItemId.UniqueId;

                                

                                List<Matching> matchingList = MatchingList.GetList(SMTPAdresse, ContactFolderName);

                                if (matchingList != null)
                                {
                                    try
                                    {
                                        Matching result = matchingList.Find(x => x.PublicId == PublicId);

                                        // Löscht nur die Geburtstage und Jahrestage bei Arges Intern Kontakten
                                        if (ContactFolderName == "Arges Intern")
                                        {
                                            var BASync = new AppointmentSync(service, SMTPAdresse);
                                            BASync.deleteName(result.Subject);
                                        }

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

                        writeLog(SMTPAdresse + " - " + icc.Count + " changes (" + c + " created, " + u + " updated, " + d + " deleted) in public folder");
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

        /// <summary>
        /// <b>(nicht benutzt)</b> Eine Hilfsfunktion die für alle Kontakte eine XML Datei erstellt hat mit Subject, UniqueId und ChankeKey 
        /// </summary>
        /// <param name="SMTPAdresse"></param>
        /// <param name="Id"></param>
        /// <param name="fileName"></param>
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

        /// <summary>
        /// Sucht im PublicRoot einen Ordner mit dem Namen aus der Config Datei (zb. "Arges Intern").
        /// </summary>
        /// <returns>Das Ordnerobjekt des Ordners mit dem richtigen Namen. Wird benutzt um den Ordner per Id zu binden.</returns>
        public Folder getPublicFolder()
        {
            var PublicRoot = Folder.Bind(service, WellKnownFolderName.PublicFoldersRoot);
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, ContactFolderName);
            FindFoldersResults FindPublicContactFolder = service.FindFolders(PublicRoot.Id, filter, new FolderView(1));
            return FindPublicContactFolder.Folders[0];
        }

        /// <summary>
        /// Sucht im Kontakte Ordner des Postfaches einen Ordner mit dem Namen aus der Config Datei (zb. "Arges Intern"). Wird der Name nicht gefunden wird ein Ordner mit dem Namen erstellt.
        /// </summary>
        /// <param name="init">true wenn es der Initiale SyncRun ist.</param>
        /// <returns>Das Ordnerobjekt des Ordners mit dem richtigen Namen. Wird benutzt um den Ordner per Id zu binden.</returns>
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

        /// <summary>
        /// Liest die SyncStates Datei zu jedem Pärchen aus Postfach und Ordner ein.
        /// </summary>
        /// <param name="isPublic">true = Die SyncState Dateien der öffentlichen Ordner erhalten ein Schlüsselwort in den Dateinamen</param>
        /// <param name="smtpAdresse">Die SMTP Adresse des Postfachs</param>
        /// <returns>Den gespeicherten SyncState vom letzten SyncRun</returns>
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

        /// <summary>
        /// Schreibt die SyncStates Datei zu jedem Pärchen aus Postfach und Ordner in den Unterordner SyncStates. Falls der Ordner nicht vorhanden ist wird er erstellt.
        /// </summary>
        /// <param name="syncState">Der SyncState als String der gespeichert werden soll.</param>
        /// <param name="isPublic">true = Die SyncState Dateien der öffentlichen Ordner erhalten ein Schlüsselwort in den Dateinamen</param>
        /// <param name="smtpAdresse">Die SMTP Adresse des Postfachs</param>
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


        /// <summary>
        /// Erstellt für jeden neuen Monat eine extra Log Datei und hängt den übergebenen Text an.
        /// </summary>
        /// <param name="logText">Text der an die Log Datei angehängt werden soll</param>
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

        /// <summary>
        /// Mappt eine Zahl von einem Zahlenblock in einen anderen (zb. 0-255 => 0-100) 
        /// </summary>
        /// <param name="value">Zahl die in den neuen Zahlenblock umgerechnet werden soll</param>
        /// <param name="from1">Start Zahlenblock Quelle</param>
        /// <param name="to1">Ende Zahlenblock Quelle</param>
        /// <param name="from2">Start Zahlenblock Ziel</param>
        /// <param name="to2">Ende Zahlenblock Ziel</param>
        /// <returns>Berechnete Zahl im neuen Zahlenblock</returns>
        public float Remap(float value, float from1, float to1, float from2, float to2)
        {
            var val = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            return val;
        }

        /// <summary>
        /// <b>(nicht benutzt)</b> Zeigt einen Fortschrittsbalken auf der Console an.
        /// </summary>
        /// <param name="percent">Prozent die die Dargestellt werden soll</param>
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

        /// <summary>
        /// Geht über alle Kontakte im öffentlichen Ordner und schreibt ihre UniqueId in ein neu angelegtes Feld "PublicID".
        /// Beim Kopieren in das User Postfach bekommt jeder Kontakt eine neue UniqueId. Somit geht die Referenz zum Public Kontakt verloren. 
        /// Um das zu verhindern wird dieses Feld benutzt. Somit hat man in jedem Kontakt des Postfaches immer die UniqueId des dazugehörigen Public Kontaktes.
        /// </summary>
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
