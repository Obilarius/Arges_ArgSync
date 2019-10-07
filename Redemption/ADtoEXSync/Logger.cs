using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADtoEXSync
{
    static class Logger
    {
        public static void writeLog(string logText)
        {
            string binaryPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
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
    }
}
