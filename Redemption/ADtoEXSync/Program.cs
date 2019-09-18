using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
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

            ExchangeService exService = EXWorker.ExchangeConnect("redemption", "redemption", "arges", "walzenbach@arges.de", "https://helios.arges.local/EWS/Exchange.asmx");

            Folder folder = EXWorker.GetMailboxFolder(exService, "ADSyncTest");

            foreach (var user in allADUsers)
            {
                //if(user.Surname == "Walzenbach")
                    EXWorker.CreateContact(exService, folder, user);
            }
            

        }
    }
}
