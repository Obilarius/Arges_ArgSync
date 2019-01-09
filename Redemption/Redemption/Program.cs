using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using System.Text.RegularExpressions;

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

            service.Url = new Uri(exUri);

            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, smtpAdresse);

            return service;
        }

        static config ReadConfig()
        {
            config config = new config();
            var file = "config.cfg";

            int index = 0;

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
                            if (lines[i].StartsWith("[DEFAULT SETTINGS]"))
                            {
                                index = 1;
                            }
                            else if (lines[i].StartsWith("[SYNCRONIZED MAILBOXES]")) 
                            {
                                index = 2;
                            }
                            else if (index == 1)
                            {
                                string pattern = @"(?<="")(.*)(?="")";
                                if (lines[i].StartsWith("password")) config.password = Regex.Match(lines[i], pattern).Value;
                                if (lines[i].StartsWith("username")) config.username = Regex.Match(lines[i], pattern).Value;
                                if (lines[i].StartsWith("domain")) config.domain = Regex.Match(lines[i], pattern).Value;
                                if (lines[i].StartsWith("exUri")) config.exUri = Regex.Match(lines[i], pattern).Value;
                            }
                            else if (index == 2)
                            {
                                var p = lines[i].Split(';');
                                config.AddMailbox(p[0].Trim(' '), p[1].Trim(' '));
                            }
                        }
                    }
                }

            }
            else
                throw new FileNotFoundException();

            return config;
        }
    }

    public class config
    {
        public string password, username, domain, exUri;
        public List<syncMailboxes> mailboxes = new List<syncMailboxes>();

        public void AddMailbox(string v1, string v2)
        {
            mailboxes.Add(new syncMailboxes(v1, v2));
        }
    }

    public class syncMailboxes
    {
        public string smtpAdresse, folder;

        public syncMailboxes (string smtpAdresse, string folder)
        {
            this.smtpAdresse = smtpAdresse;
            this.folder = folder;
        }
    }
}
