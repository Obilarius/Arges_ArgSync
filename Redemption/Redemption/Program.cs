using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Exchange.WebServices.Data;

namespace Redemption
{
    class Program
    {
        

        static void Main(string[] args)
        {
            String password = "redemption";
            String username = "redemption";
            String domain = "arges";
            String exUri = "https://helios.arges.local/EWS/Exchange.asmx";

            var file = "config.cfg";
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                string[][] parts = new string[lines.Length][];

                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i])) // Leere Zeile in Config
                    {
                        if (lines[i].Substring(0, 1) != "#") // Kommentar in der Config
                        {
                            parts[i] = lines[i].Split(';');

                            var smtp = parts[i][0].Trim(' ');
                            var folder = parts[i][1].Trim(' ');
                            ExchangeService service = ExchangeConnect(username, password, domain, smtp, exUri);

                            var SyncRun = new ExchangeSync(service, smtp, folder);
                            SyncRun.writePublicIdInExProp();
                            bool changes = SyncRun.Sync();

                            if (changes)
                            {
                                MatchingList.Create(service, smtp, folder);
                            }
                        }
                    }
                }

            }
            else
                throw new FileNotFoundException();

            //Console.WriteLine("Press Key to Exit");
            //Console.ReadKey();
        }

        public static ExchangeService ExchangeConnect(string username, string password, string domain, string smtpAdresse, string exUri)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(username, password, domain);

            //service.AutodiscoverUrl("walzenbach@arges.de");
            service.Url = new Uri(exUri);

            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, smtpAdresse);

            return service;
        }
    }
}
