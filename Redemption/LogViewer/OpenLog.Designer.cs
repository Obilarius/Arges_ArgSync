namespace LogViewer
{
    partial class OpenLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenLog));
            this.lst_showLogs = new System.Windows.Forms.ListView();
            this.Jahr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Monat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lst_showLogs
            // 
            this.lst_showLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lst_showLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Jahr,
            this.Monat,
            this.path});
            this.lst_showLogs.FullRowSelect = true;
            this.lst_showLogs.GridLines = true;
            this.lst_showLogs.Location = new System.Drawing.Point(13, 13);
            this.lst_showLogs.Name = "lst_showLogs";
            this.lst_showLogs.Size = new System.Drawing.Size(205, 313);
            this.lst_showLogs.TabIndex = 0;
            this.lst_showLogs.UseCompatibleStateImageBehavior = false;
            this.lst_showLogs.View = System.Windows.Forms.View.Details;
            this.lst_showLogs.SelectedIndexChanged += new System.EventHandler(this.lst_showLogs_SelectedIndexChanged);
            this.lst_showLogs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lst_showLogs_MouseDoubleClick);
            // 
            // Jahr
            // 
            this.Jahr.Text = "Jahr";
            this.Jahr.Width = 100;
            // 
            // Monat
            // 
            this.Monat.Text = "Monat";
            this.Monat.Width = 95;
            // 
            // path
            // 
            this.path.Text = "path";
            this.path.Width = 0;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(13, 333);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(205, 62);
            this.button1.TabIndex = 1;
            this.button1.Text = "Open Log";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // OpenLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 407);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lst_showLogs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(246, 446);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(246, 446);
            this.Name = "OpenLog";
            this.Text = "OpenLog";
            this.Load += new System.EventHandler(this.OpenLog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lst_showLogs;
        private System.Windows.Forms.ColumnHeader Jahr;
        private System.Windows.Forms.ColumnHeader Monat;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColumnHeader path;
    }
}