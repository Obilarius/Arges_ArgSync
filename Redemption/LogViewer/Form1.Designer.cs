namespace LogViewer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_openFileDialog = new System.Windows.Forms.Button();
            this.lst_logView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbl_openTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_openFileDialog
            // 
            this.btn_openFileDialog.Location = new System.Drawing.Point(13, 13);
            this.btn_openFileDialog.Name = "btn_openFileDialog";
            this.btn_openFileDialog.Size = new System.Drawing.Size(75, 23);
            this.btn_openFileDialog.TabIndex = 1;
            this.btn_openFileDialog.Text = "Open Log";
            this.btn_openFileDialog.UseVisualStyleBackColor = true;
            this.btn_openFileDialog.Click += new System.EventHandler(this.btn_openFileDialog_Click);
            // 
            // lst_logView
            // 
            this.lst_logView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lst_logView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lst_logView.Location = new System.Drawing.Point(12, 42);
            this.lst_logView.Name = "lst_logView";
            this.lst_logView.Size = new System.Drawing.Size(776, 396);
            this.lst_logView.TabIndex = 0;
            this.lst_logView.UseCompatibleStateImageBehavior = false;
            this.lst_logView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Datum - Uhrzeit";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Text";
            this.columnHeader2.Width = 1000;
            // 
            // lbl_openTime
            // 
            this.lbl_openTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_openTime.Location = new System.Drawing.Point(488, 18);
            this.lbl_openTime.Name = "lbl_openTime";
            this.lbl_openTime.Size = new System.Drawing.Size(300, 13);
            this.lbl_openTime.TabIndex = 2;
            this.lbl_openTime.Text = "No Log Opened";
            this.lbl_openTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbl_openTime);
            this.Controls.Add(this.btn_openFileDialog);
            this.Controls.Add(this.lst_logView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(816, 400);
            this.Name = "Form1";
            this.Text = "LogViewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_openFileDialog;
        private System.Windows.Forms.ListView lst_logView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label lbl_openTime;
    }
}

