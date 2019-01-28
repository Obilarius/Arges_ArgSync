using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Redemption
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = ReadConfig();

            ExchangeSync.writeLog("##################################################################");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var m in config.mailboxes)
            {
                ExchangeSync.writeLog("---------- " + m.smtpAdresse.ToUpper() + " ----------");
                ExchangeService service = ExchangeConnect(config.username, config.password, config.domain, m.smtpAdresse, config.exUri);

                if (m.birthday || m.anniversary)
                {
                    ExchangeSync.writeLog("---------- SyncRun Start - Appointment ----------");
                    Stopwatch sWatch = new Stopwatch();
                    sWatch.Start();

                    var BASync = new AppointmentSync(service, m.smtpAdresse);

                    //BASync.runDelete();
                    //BASync.deleteWithCategorie("automatically added from PF");

                    if (m.birthday)
                    {
                        BASync.runBirthdaySync();
                    }
                    if (m.anniversary)
                    {
                        BASync.runAnniversarySync();
                    }

                    sWatch.Stop();
                    ExchangeSync.writeLog("---------- SyncRun End - " + stopWatch.Elapsed + " ----------");
                }


                foreach (var f in m.folder)
                {
                    var SyncRun = new ExchangeSync(service, m.smtpAdresse, f);
                    SyncRun.writePublicIdInExProp();
                    bool changes = SyncRun.Sync();

                    if (changes)
                    {
                        MatchingList.Create(service, m.smtpAdresse, f);
                        ExchangeSync.writeLog("Matching List created: " + m.smtpAdresse + " - " + f);
                    }
                }
            }

            stopWatch.Stop();
            ExchangeSync.writeLog("Complete Sync - Time: " + stopWatch.Elapsed);
        }

        public static ExchangeService ExchangeConnect(string username, string password, string domain, string smtpAdresse, string exUri)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(username, password, domain);

            service.Url = new Uri(exUri);

            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, smtpAdresse);

            return service;
        }

        static Config ReadConfig()
        {
            Config config = new Config();
            string file = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\config.cfg";
            //var file = "config.cfg";

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
                                List<string> folder = new List<string>();

                                var smtp = p[0].Trim();
                                var birthday = Convert.ToBoolean(p[1].Trim());
                                var anni = Convert.ToBoolean(p[2].Trim());

                                for (int k = 3; k < p.Length; k++)
                                {
                                    folder.Add(p[k].Trim());
                                }

                                config.AddMailbox(smtp, birthday, anni, folder);
                            }
                        }
                    }
                }

            }
            else
                throw new FileNotFoundException("Config not found");

            return config;
        }
    }
}
