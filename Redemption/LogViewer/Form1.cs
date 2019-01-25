using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace LogViewer
{
    public partial class Form1 : Form
    {
        string path;
        int linesCount;
        FileSystemWatcher watcher;
        DateTime lastOpenLog;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_openFileDialog_Click(object sender, EventArgs e)
        {
            using (OpenLog openLogForm = new OpenLog())
            {
                if (openLogForm.ShowDialog() == DialogResult.OK)
                {
                    path = openLogForm.logPath;

                    linesCount = 0;
                    lst_logView.Items.Clear();

                    openLog();

                    lbl_openTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                    watcher = new FileSystemWatcher();
                    watcher.Path = Path.GetDirectoryName(path);
                    watcher.Filter = Path.GetFileName(path);
                    watcher.NotifyFilter = NotifyFilters.LastWrite; //NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    watcher.SynchronizingObject = this;
                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    watcher.EnableRaisingEvents = true;

                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            openLog();

            lbl_openTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private void openLog ()
        {
            TimeSpan ts = lastOpenLog - DateTime.Now;
            if (ts.Milliseconds < 500)
            {
                //return;
            }

            int currentIndex = 0;

            IEnumerable<string> Lines = ReadLines(path);            

            foreach (var l in Lines)
            {
                if (currentIndex >= linesCount)
                {
                    linesCount++;

                    var date = l.Substring(0, 19);
                    var text = l.Substring(22);
                    ListViewItem lvi;

                    switch (text)
                    {
                        case var someVal when new Regex(@"^[#]+$").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, "START COMPLETE RUN" });
                            lvi.BackColor = Color.Blue;
                            lvi.ForeColor = Color.White;
                            break;
                        case var someVal when new Regex(@"Complete Sync - Time:").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, "FINISH COMPLETE RUN - " + text });
                            lvi.BackColor = Color.Blue;
                            lvi.ForeColor = Color.White;
                            break;
                        case var someVal when new Regex(@"SyncRun (Start|End)").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, text });
                            lvi.BackColor = Color.Aquamarine;
                            break;
                        case var someVal when new Regex(@"ERROR:").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, text });
                            lvi.BackColor = Color.Red;
                            break;
                        case var someVal when new Regex(@" - \d* changes ").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, text });
                            lvi.ForeColor = Color.SaddleBrown;
                            break;
                        case var someVal when new Regex(@"-+ [A-Z]+@[A-Z]+.[A-Z]+ -+").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, text });
                            lvi.BackColor = Color.Peru;
                            break;
                        default:
                            lvi = new ListViewItem(new string[] { date, text });
                            break;
                    }

                    lst_logView.Items.Add(lvi);
                    
                }

                currentIndex++;
            }
            //lst_logView.Refresh();
            lst_logView.Items[lst_logView.Items.Count - 1].EnsureVisible();
            lbl_lines.Text = linesCount.ToString() + " Zeilen";

            lastOpenLog = DateTime.Now;
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}

