using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogViewer
{
    public partial class OpenLog : Form
    {
        public string logPath { get; set; }

        public OpenLog()
        {
            InitializeComponent();
        }

        private void OpenLog_Load(object sender, EventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\log\");

            try
            {
                FileInfo[] Files = d.GetFiles("*Log.txt");
                foreach (FileInfo file in Files)
                {
                    //str = str + ", " + file.Name;
                    var logDate = file.Name.Split('-');
                    var month = logDate[1].Split('_');
                    lst_showLogs.Items.Add(new ListViewItem(new string[] { logDate[0], month[0] }));
                }
            }
            catch (Exception)
            {
                lst_showLogs.Items.Add(new ListViewItem(new string[] { "No logs found!" }));
            }
            
        }

        private void lst_showLogs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem selectedLog = lst_showLogs.SelectedItems[0];
            logPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\log\" + selectedLog.SubItems[0].Text + "-" + selectedLog.SubItems[1].Text + "_Log.txt";

        }

        private void lst_showLogs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem selectedLog = lst_showLogs.SelectedItems[0];
            logPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\log\" + selectedLog.SubItems[0].Text + "-" + selectedLog.SubItems[1].Text + "_Log.txt";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
