using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Redemption
{
    class Program
    {
        static void Main(string[] args)
        {
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

                            var SyncRun = new ExchangeSync(smtp, folder);
                            SyncRun.writePublicIdInExProp();
                            bool changes = SyncRun.Sync();

                            if (changes)
                            {
                                SyncRun.createMatchingList();
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
    }
}
