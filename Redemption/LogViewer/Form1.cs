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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btn_openFileDialog_Click(object sender, EventArgs e)
        {
            using (OpenLog openLogForm = new OpenLog())
            {
                if (openLogForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = openLogForm.logPath;

                    openLog();

                    lbl_openTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                    FileSystemWatcher watcher = new FileSystemWatcher();
                    watcher.Path = Path.GetDirectoryName(path);
                    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    watcher.Filter = Path.GetFileName(path);
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
            string line;

            // Read the file and display it line by line.  
            try
            {
                StreamReader file = new StreamReader(path);

                while ((line = file.ReadLine()) != null)
                {
                    var date = line.Substring(0, 19);
                    var text = line.Substring(22);
                    ListViewItem lvi;

                    switch (text)
                    {
                        case var someVal when new Regex(@"^[#]+$").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, "" });
                            lvi.BackColor = Color.Blue;
                            lvi.ForeColor = Color.White;
                            break;
                        case var someVal when new Regex(@"SyncRun (Start|End)").IsMatch(someVal):
                            lvi = new ListViewItem(new string[] { date, text });
                            lvi.BackColor = Color.Aquamarine;
                            break;
                        default:
                            lvi = new ListViewItem(new string[] { date, text });
                            break;
                    }

                    lst_logView.Items.Add(lvi);
                    lst_logView.Items[lst_logView.Items.Count - 1].EnsureVisible();
                }

                file.Close();
            }
            catch (Exception ex)
            {
                lbl_openTime.Text = ex.Message;
            }
        }
    }
}

