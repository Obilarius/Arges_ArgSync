namespace ConfigEditor
{
    partial class ConfigEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigEditor));
            this.lbl_serverSettings = new System.Windows.Forms.Label();
            this.lbl_syncMailboxes = new System.Windows.Forms.Label();
            this.lbl_username = new System.Windows.Forms.Label();
            this.lbl_password = new System.Windows.Forms.Label();
            this.lbl_exUri = new System.Windows.Forms.Label();
            this.lbl_domain = new System.Windows.Forms.Label();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.txt_exUri = new System.Windows.Forms.TextBox();
            this.txt_domain = new System.Windows.Forms.TextBox();
            this.btn_changeServerSettings = new System.Windows.Forms.Button();
            this.lst_syncMailboxes = new System.Windows.Forms.ListView();
            this.SMTP_Adresse = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Folder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.birthday = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.anniversary = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.txt_smtp = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.check_ArgesKontakte = new System.Windows.Forms.CheckBox();
            this.check_ArgesIntern = new System.Windows.Forms.CheckBox();
            this.btn_deleteSyncMailbox = new System.Windows.Forms.Button();
            this.btn_newSyncMailbox = new System.Windows.Forms.Button();
            this.btn_saveConfig = new System.Windows.Forms.Button();
            this.lbl_note = new System.Windows.Forms.Label();
            this.check_disable = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.check_anniversaries = new System.Windows.Forms.CheckBox();
            this.check_birthdays = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_serverSettings
            // 
            this.lbl_serverSettings.AutoSize = true;
            this.lbl_serverSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_serverSettings.Location = new System.Drawing.Point(12, 9);
            this.lbl_serverSettings.Name = "lbl_serverSettings";
            this.lbl_serverSettings.Size = new System.Drawing.Size(114, 16);
            this.lbl_serverSettings.TabIndex = 0;
            this.lbl_serverSettings.Text = "Server Settings";
            // 
            // lbl_syncMailboxes
            // 
            this.lbl_syncMailboxes.AutoSize = true;
            this.lbl_syncMailboxes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_syncMailboxes.Location = new System.Drawing.Point(12, 123);
            this.lbl_syncMailboxes.Name = "lbl_syncMailboxes";
            this.lbl_syncMailboxes.Size = new System.Drawing.Size(168, 16);
            this.lbl_syncMailboxes.TabIndex = 1;
            this.lbl_syncMailboxes.Text = "Syncronized Mailboxes";
            // 
            // lbl_username
            // 
            this.lbl_username.AutoSize = true;
            this.lbl_username.Location = new System.Drawing.Point(12, 35);
            this.lbl_username.Name = "lbl_username";
            this.lbl_username.Size = new System.Drawing.Size(58, 13);
            this.lbl_username.TabIndex = 2;
            this.lbl_username.Text = "Username:";
            // 
            // lbl_password
            // 
            this.lbl_password.AutoSize = true;
            this.lbl_password.Location = new System.Drawing.Point(12, 58);
            this.lbl_password.Name = "lbl_password";
            this.lbl_password.Size = new System.Drawing.Size(56, 13);
            this.lbl_password.TabIndex = 3;
            this.lbl_password.Text = "Password:";
            // 
            // lbl_exUri
            // 
            this.lbl_exUri.AutoSize = true;
            this.lbl_exUri.Location = new System.Drawing.Point(259, 58);
            this.lbl_exUri.Name = "lbl_exUri";
            this.lbl_exUri.Size = new System.Drawing.Size(80, 13);
            this.lbl_exUri.TabIndex = 5;
            this.lbl_exUri.Text = "Exchange URI:";
            // 
            // lbl_domain
            // 
            this.lbl_domain.AutoSize = true;
            this.lbl_domain.Location = new System.Drawing.Point(259, 35);
            this.lbl_domain.Name = "lbl_domain";
            this.lbl_domain.Size = new System.Drawing.Size(46, 13);
            this.lbl_domain.TabIndex = 4;
            this.lbl_domain.Text = "Domain:";
            // 
            // txt_username
            // 
            this.txt_username.Location = new System.Drawing.Point(77, 32);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(153, 20);
            this.txt_username.TabIndex = 6;
            // 
            // txt_password
            // 
            this.txt_password.Location = new System.Drawing.Point(77, 55);
            this.txt_password.Name = "txt_password";
            this.txt_password.Size = new System.Drawing.Size(153, 20);
            this.txt_password.TabIndex = 7;
            // 
            // txt_exUri
            // 
            this.txt_exUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_exUri.Location = new System.Drawing.Point(343, 55);
            this.txt_exUri.Name = "txt_exUri";
            this.txt_exUri.Size = new System.Drawing.Size(366, 20);
            this.txt_exUri.TabIndex = 9;
            // 
            // txt_domain
            // 
            this.txt_domain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_domain.Location = new System.Drawing.Point(343, 32);
            this.txt_domain.Name = "txt_domain";
            this.txt_domain.Size = new System.Drawing.Size(366, 20);
            this.txt_domain.TabIndex = 8;
            // 
            // btn_changeServerSettings
            // 
            this.btn_changeServerSettings.Location = new System.Drawing.Point(15, 86);
            this.btn_changeServerSettings.Name = "btn_changeServerSettings";
            this.btn_changeServerSettings.Size = new System.Drawing.Size(75, 23);
            this.btn_changeServerSettings.TabIndex = 10;
            this.btn_changeServerSettings.Text = "Change";
            this.btn_changeServerSettings.UseVisualStyleBackColor = true;
            this.btn_changeServerSettings.Click += new System.EventHandler(this.btn_changeServerSettings_Click);
            // 
            // lst_syncMailboxes
            // 
            this.lst_syncMailboxes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lst_syncMailboxes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SMTP_Adresse,
            this.Folder,
            this.birthday,
            this.anniversary});
            this.lst_syncMailboxes.FullRowSelect = true;
            this.lst_syncMailboxes.GridLines = true;
            this.lst_syncMailboxes.Location = new System.Drawing.Point(15, 148);
            this.lst_syncMailboxes.Name = "lst_syncMailboxes";
            this.lst_syncMailboxes.Size = new System.Drawing.Size(441, 472);
            this.lst_syncMailboxes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lst_syncMailboxes.TabIndex = 12;
            this.lst_syncMailboxes.UseCompatibleStateImageBehavior = false;
            this.lst_syncMailboxes.View = System.Windows.Forms.View.Details;
            this.lst_syncMailboxes.SelectedIndexChanged += new System.EventHandler(this.lst_syncMailboxes_SelectedIndexChanged);
            // 
            // SMTP_Adresse
            // 
            this.SMTP_Adresse.Text = "SMTP Adresse";
            this.SMTP_Adresse.Width = 150;
            // 
            // Folder
            // 
            this.Folder.Text = "Folder";
            this.Folder.Width = 180;
            // 
            // birthday
            // 
            this.birthday.Text = "Birthday";
            this.birthday.Width = 51;
            // 
            // anniversary
            // 
            this.anniversary.Text = "Anniversary";
            this.anniversary.Width = 55;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(469, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "SMTP Adresse:";
            // 
            // txt_smtp
            // 
            this.txt_smtp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_smtp.Location = new System.Drawing.Point(472, 165);
            this.txt_smtp.Name = "txt_smtp";
            this.txt_smtp.Size = new System.Drawing.Size(236, 20);
            this.txt_smtp.TabIndex = 15;
            this.txt_smtp.TextChanged += new System.EventHandler(this.saveSyncMailbox_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.check_ArgesKontakte);
            this.groupBox1.Controls.Add(this.check_ArgesIntern);
            this.groupBox1.Location = new System.Drawing.Point(473, 191);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(117, 78);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Folder";
            // 
            // check_ArgesKontakte
            // 
            this.check_ArgesKontakte.AutoSize = true;
            this.check_ArgesKontakte.Location = new System.Drawing.Point(6, 42);
            this.check_ArgesKontakte.Name = "check_ArgesKontakte";
            this.check_ArgesKontakte.Size = new System.Drawing.Size(99, 17);
            this.check_ArgesKontakte.TabIndex = 26;
            this.check_ArgesKontakte.Text = "Arges Kontakte";
            this.check_ArgesKontakte.UseVisualStyleBackColor = true;
            this.check_ArgesKontakte.CheckedChanged += new System.EventHandler(this.saveSyncMailbox_Click);
            // 
            // check_ArgesIntern
            // 
            this.check_ArgesIntern.AutoSize = true;
            this.check_ArgesIntern.Location = new System.Drawing.Point(6, 19);
            this.check_ArgesIntern.Name = "check_ArgesIntern";
            this.check_ArgesIntern.Size = new System.Drawing.Size(83, 17);
            this.check_ArgesIntern.TabIndex = 25;
            this.check_ArgesIntern.Text = "Arges Intern";
            this.check_ArgesIntern.UseVisualStyleBackColor = true;
            this.check_ArgesIntern.CheckedChanged += new System.EventHandler(this.saveSyncMailbox_Click);
            // 
            // btn_deleteSyncMailbox
            // 
            this.btn_deleteSyncMailbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_deleteSyncMailbox.Location = new System.Drawing.Point(472, 498);
            this.btn_deleteSyncMailbox.Name = "btn_deleteSyncMailbox";
            this.btn_deleteSyncMailbox.Size = new System.Drawing.Size(118, 52);
            this.btn_deleteSyncMailbox.TabIndex = 20;
            this.btn_deleteSyncMailbox.Text = "Delete";
            this.btn_deleteSyncMailbox.UseVisualStyleBackColor = true;
            this.btn_deleteSyncMailbox.Click += new System.EventHandler(this.btn_deleteSyncMailbox_Click);
            // 
            // btn_newSyncMailbox
            // 
            this.btn_newSyncMailbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_newSyncMailbox.Location = new System.Drawing.Point(592, 498);
            this.btn_newSyncMailbox.Name = "btn_newSyncMailbox";
            this.btn_newSyncMailbox.Size = new System.Drawing.Size(115, 52);
            this.btn_newSyncMailbox.TabIndex = 21;
            this.btn_newSyncMailbox.Text = "New";
            this.btn_newSyncMailbox.UseVisualStyleBackColor = true;
            this.btn_newSyncMailbox.Click += new System.EventHandler(this.btn_newSyncMailbox_Click);
            // 
            // btn_saveConfig
            // 
            this.btn_saveConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_saveConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_saveConfig.Location = new System.Drawing.Point(472, 556);
            this.btn_saveConfig.Name = "btn_saveConfig";
            this.btn_saveConfig.Size = new System.Drawing.Size(236, 64);
            this.btn_saveConfig.TabIndex = 22;
            this.btn_saveConfig.Text = "Save Config";
            this.btn_saveConfig.UseVisualStyleBackColor = true;
            this.btn_saveConfig.Click += new System.EventHandler(this.btn_saveConfig_Click);
            // 
            // lbl_note
            // 
            this.lbl_note.AutoSize = true;
            this.lbl_note.ForeColor = System.Drawing.Color.Red;
            this.lbl_note.Location = new System.Drawing.Point(145, 91);
            this.lbl_note.Name = "lbl_note";
            this.lbl_note.Size = new System.Drawing.Size(0, 13);
            this.lbl_note.TabIndex = 23;
            // 
            // check_disable
            // 
            this.check_disable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.check_disable.AutoSize = true;
            this.check_disable.Location = new System.Drawing.Point(472, 276);
            this.check_disable.Name = "check_disable";
            this.check_disable.Size = new System.Drawing.Size(61, 17);
            this.check_disable.TabIndex = 24;
            this.check_disable.Text = "Disable";
            this.check_disable.UseVisualStyleBackColor = true;
            this.check_disable.CheckedChanged += new System.EventHandler(this.saveSyncMailbox_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.check_anniversaries);
            this.groupBox2.Controls.Add(this.check_birthdays);
            this.groupBox2.Location = new System.Drawing.Point(592, 191);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(117, 78);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Appointment";
            // 
            // check_anniversaries
            // 
            this.check_anniversaries.AutoSize = true;
            this.check_anniversaries.Location = new System.Drawing.Point(6, 42);
            this.check_anniversaries.Name = "check_anniversaries";
            this.check_anniversaries.Size = new System.Drawing.Size(89, 17);
            this.check_anniversaries.TabIndex = 26;
            this.check_anniversaries.Text = "Anniversaries";
            this.check_anniversaries.UseVisualStyleBackColor = true;
            this.check_anniversaries.CheckedChanged += new System.EventHandler(this.saveSyncMailbox_Click);
            // 
            // check_birthdays
            // 
            this.check_birthdays.AutoSize = true;
            this.check_birthdays.Location = new System.Drawing.Point(6, 19);
            this.check_birthdays.Name = "check_birthdays";
            this.check_birthdays.Size = new System.Drawing.Size(69, 17);
            this.check_birthdays.TabIndex = 25;
            this.check_birthdays.Text = "Birthdays";
            this.check_birthdays.UseVisualStyleBackColor = true;
            this.check_birthdays.CheckedChanged += new System.EventHandler(this.saveSyncMailbox_Click);
            // 
            // ConfigEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 632);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.check_disable);
            this.Controls.Add(this.lbl_note);
            this.Controls.Add(this.btn_saveConfig);
            this.Controls.Add(this.btn_newSyncMailbox);
            this.Controls.Add(this.btn_deleteSyncMailbox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_smtp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lst_syncMailboxes);
            this.Controls.Add(this.btn_changeServerSettings);
            this.Controls.Add(this.txt_exUri);
            this.Controls.Add(this.txt_domain);
            this.Controls.Add(this.txt_password);
            this.Controls.Add(this.txt_username);
            this.Controls.Add(this.lbl_exUri);
            this.Controls.Add(this.lbl_domain);
            this.Controls.Add(this.lbl_password);
            this.Controls.Add(this.lbl_username);
            this.Controls.Add(this.lbl_syncMailboxes);
            this.Controls.Add(this.lbl_serverSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigEditor";
            this.Text = "ARGES Sync Config Editor";
            this.Load += new System.EventHandler(this.ConfigEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_serverSettings;
        private System.Windows.Forms.Label lbl_syncMailboxes;
        private System.Windows.Forms.Label lbl_username;
        private System.Windows.Forms.Label lbl_password;
        private System.Windows.Forms.Label lbl_exUri;
        private System.Windows.Forms.Label lbl_domain;
        private System.Windows.Forms.TextBox txt_username;
        private System.Windows.Forms.TextBox txt_password;
        private System.Windows.Forms.TextBox txt_exUri;
        private System.Windows.Forms.TextBox txt_domain;
        private System.Windows.Forms.Button btn_changeServerSettings;
        private System.Windows.Forms.ListView lst_syncMailboxes;
        private System.Windows.Forms.ColumnHeader SMTP_Adresse;
        private System.Windows.Forms.ColumnHeader Folder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_smtp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_deleteSyncMailbox;
        private System.Windows.Forms.Button btn_newSyncMailbox;
        private System.Windows.Forms.Button btn_saveConfig;
        private System.Windows.Forms.Label lbl_note;
        private System.Windows.Forms.CheckBox check_disable;
        private System.Windows.Forms.ColumnHeader anniversary;
        private System.Windows.Forms.ColumnHeader birthday;
        private System.Windows.Forms.CheckBox check_ArgesKontakte;
        private System.Windows.Forms.CheckBox check_ArgesIntern;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox check_anniversaries;
        private System.Windows.Forms.CheckBox check_birthdays;
    }
}

