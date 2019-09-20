using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADtoEXSync
{
    class Program
    {
        static void Main(string[] args)
        {
            List<UserPrincipal> allADUsers = ADWorker.GetAllRelevantADUsers("ARGES", "OU=Arges_Intern,DC=arges,DC=local");

            ExchangeService exService = EXWorker.ExchangeConnect("redemption", "redemption", "arges", "administrator@arges.de", "https://helios.arges.local/EWS/Exchange.asmx");

            //Folder folder = EXWorker.GetMailboxFolder(exService, "ADSyncTest");
            Folder folder = EXWorker.getPublicFolder(exService, "Arges Intern");

            List<string> ADSIDsFromPublicFolder = EXWorker.GetAllADSIDsInFolderWhereAutogenarated(exService, folder);

            // Kontakte kopieren
            foreach (var user in allADUsers)
            {
                if (!ADSIDsFromPublicFolder.Contains(user.Sid.ToString()))
                    EXWorker.CreateContact(exService, folder, user);

                ADSIDsFromPublicFolder.Remove(user.Sid.ToString());
            }

            // Löscht Kontakte die im AD Deaktiviert worden sind
            foreach (var sid in ADSIDsFromPublicFolder)
            {
                EXWorker.DeleteContactWithADSID(exService, folder, sid);
            }

        }
    }
}
