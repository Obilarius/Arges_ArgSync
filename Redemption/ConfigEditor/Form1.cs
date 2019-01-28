using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigEditor
{
    public partial class ConfigEditor : Form
    {
        Config config = null;
        int selectedMailbox = -1;
        string[] FolderArray = new string[] { "Arges Intern", "Arges Kontakte" };

        public ConfigEditor()
        {
            InitializeComponent();
            config = ReadConfig();

            txt_username.Text = config.username;
            txt_username.Enabled = false;
            txt_password.Text = config.password;
            txt_password.Enabled = false;
            txt_domain.Text = config.domain;
            txt_domain.Enabled = false;
            txt_exUri.Text = config.exUri;
            txt_exUri.Enabled = false;
        }

        Config ReadConfig()
        {
            Config config = new Config();
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
                                var smtp = p[0].Trim();
                                var birthday = Convert.ToBoolean(p[1].Trim());
                                var anniversary = Convert.ToBoolean(p[2].Trim());
                                var folder = new List<string>();

                                for (int j = 3; j < p.Length; j++)
                                {
                                    folder.Add(p[j].Trim());
                                }

                                config.AddMailbox(smtp, birthday, anniversary, folder);
                            }
                        }
                        else
                        {
                            if (index == 2)
                            {
                                var p = lines[i].Split(';');
                                var smtp = p[0].Trim().Substring(1);
                                var birthday = Convert.ToBoolean(p[1].Trim());
                                var anniversary = Convert.ToBoolean(p[2].Trim());
                                var folder = new List<string>();

                                for (int j = 3; j < p.Length; j++)
                                {
                                    folder.Add(p[j].Trim());
                                }

                                config.AddMailbox(smtp, birthday, anniversary, folder, true);
                            }
                        }
                    }
                }

            }
            else
                throw new FileNotFoundException();

            return config;
        }

        private void btn_changeServerSettings_Click(object sender, EventArgs e)
        {
            txt_username.Enabled = true;
            txt_password.Enabled = true;
            txt_domain.Enabled = true;
            txt_exUri.Enabled = true;
        }

        private void ConfigEditor_Load(object sender, EventArgs e)
        {
            
            foreach (var item in config.mailboxes)
            {
                string folderStr = "";
                for (int i = 0; i < item.folder.Count(); i++)
                {
                    folderStr += item.folder[i];
                    if (i != item.folder.Count() - 1)
                    {
                        folderStr += ";";
                    }
                }


                ListViewItem lvitem = new ListViewItem(new string[] { item.smtpAdresse, folderStr, item.birthday.ToString() , item.anniversary.ToString() });

                if (item.disable)
                {
                    lvitem.BackColor = Color.DarkGray;
                    lvitem.ForeColor = Color.DimGray;
                }

                lst_syncMailboxes.Items.Add(lvitem);
            }
        }

        private void lst_syncMailboxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txt_smtp.Text = "";
                check_anniversaries.Checked = false;
                check_birthdays.Checked = false;
                check_ArgesIntern.Checked = false;
                check_ArgesKontakte.Checked = false;

                var selectedItem = lst_syncMailboxes.SelectedItems[0];
                selectedMailbox = selectedItem.Index;
                var smtp = selectedItem.SubItems[0].Text;
                var folder = selectedItem.SubItems[1].Text.Split(';');

                txt_smtp.Text = smtp;


                if (Array.IndexOf(folder, FolderArray[0]) > -1)
                {
                    check_ArgesIntern.Checked = true;
                }
                if (Array.IndexOf(folder, FolderArray[1]) > -1)
                {
                    check_ArgesKontakte.Checked = true;
                }

                if (selectedItem.SubItems[2].Text == "True")
                {
                    check_birthdays.Checked = true;
                }
                if (selectedItem.SubItems[3].Text == "True")
                {
                    check_anniversaries.Checked = true;
                }

                if (lst_syncMailboxes.Items[selectedMailbox].BackColor == Color.DarkGray)
                {
                    check_disable.Checked = true;
                } 
                else
                {
                    check_disable.Checked = false;
                }
                
            }
            catch (Exception)
            {

                //throw;
            }
        }

        private void btn_newSyncMailbox_Click(object sender, EventArgs e)
        {
            string folder = null;
            bool AI = check_ArgesIntern.Checked;
            bool AK = check_ArgesKontakte.Checked;

            if ( AI && !AK )     { folder = FolderArray[0]; }
            else if (!AI && AK)  { folder = FolderArray[1]; }
            else if (AI && AK)   { folder = FolderArray[0] + ";" + FolderArray[1]; }
            else if (!AI && !AK) { folder = ""; }

            lst_syncMailboxes.Items.Add(new ListViewItem(new string[] { txt_smtp.Text, folder, check_birthdays.Checked.ToString(), check_anniversaries.Checked.ToString() }));
            lst_syncMailboxes.Refresh();

            txt_smtp.Text = "";
            check_ArgesIntern.Checked = false;
            check_ArgesKontakte.Checked = false;
            check_birthdays.Checked = false;
            check_anniversaries.Checked = false;
            selectedMailbox = -10;
        }

        private void btn_deleteSyncMailbox_Click(object sender, EventArgs e)
        {
            switch (selectedMailbox)
            {
                // new Item
                case -10:
                    txt_smtp.Text = "";
                    check_ArgesIntern.Checked = false;
                    check_ArgesKontakte.Checked = false;
                    check_birthdays.Checked = false;
                    check_anniversaries.Checked = false;
                    selectedMailbox = -1;
                    break;
                // no Item selected
                case -1:
                    break;
                // Item selected
                default:
                    lst_syncMailboxes.Items[selectedMailbox].Remove();
                    txt_smtp.Text = "";
                    check_ArgesIntern.Checked = false;
                    check_ArgesKontakte.Checked = false;
                    check_birthdays.Checked = false;
                    check_anniversaries.Checked = false;
                    check_disable.Checked = false;
                    selectedMailbox = -1;
                    break;
            }
        }

        private void saveSyncMailbox_Click(object sender, EventArgs e)
        {
            string folder = null;
            bool AI = check_ArgesIntern.Checked;
            bool AK = check_ArgesKontakte.Checked;

            if (AI && !AK) { folder = FolderArray[0]; }
            else if (!AI && AK) { folder = FolderArray[1]; }
            else if (AI && AK) { folder = FolderArray[0] + ";" + FolderArray[1]; }
            else if (!AI && !AK) { folder = ""; }

            lst_syncMailboxes.Items[selectedMailbox].SubItems[0].Text = txt_smtp.Text;
            lst_syncMailboxes.Items[selectedMailbox].SubItems[1].Text = folder;
            lst_syncMailboxes.Items[selectedMailbox].SubItems[2].Text = check_birthdays.Checked.ToString();
            lst_syncMailboxes.Items[selectedMailbox].SubItems[3].Text = check_anniversaries.Checked.ToString();

            if (check_disable.Checked)
            {
                lst_syncMailboxes.Items[selectedMailbox].BackColor = Color.DarkGray;
                lst_syncMailboxes.Items[selectedMailbox].ForeColor = Color.DimGray;
            }
            else
            {
                lst_syncMailboxes.Items[selectedMailbox].BackColor = SystemColors.Window;
                lst_syncMailboxes.Items[selectedMailbox].ForeColor = SystemColors.WindowText;
            }

            lst_syncMailboxes.Refresh();
        }

        private void btn_saveConfig_Click(object sender, EventArgs e)
        {
            string cs = "";
            cs += "[DEFAULT SETTINGS]\n";
            cs += "password = \"" + txt_password.Text + "\"\n";
            cs += "username = \"" + txt_username.Text + "\"\n";
            cs += "domain = \"" + txt_domain.Text + "\"\n";
            cs += "exUri = \"" + txt_exUri.Text + "\"\n\n";
            cs += "[SYNCRONIZED MAILBOXES]\n";

            foreach (ListViewItem item in lst_syncMailboxes.Items)
            {

                if (item.BackColor == Color.DarkGray)
                {
                    cs += "#";
                }
                cs += item.SubItems[0].Text + ";" + item.SubItems[2].Text + ";" + item.SubItems[3].Text + ";" + item.SubItems[1].Text + "\n";
            }

            File.WriteAllText("config.cfg", cs);
            lbl_note.Text = "Config saved!";
        }

        private void txt_smextChanged(object sender, EventArgs e)
        {

        }
    }

}
