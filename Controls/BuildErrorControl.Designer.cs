namespace Editroid.Controls
{
    partial class BuildErrorControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.errorList = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // errorList
            // 
            this.errorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3});
            this.errorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorList.FullRowSelect = true;
            this.errorList.GridLines = true;
            this.errorList.Location = new System.Drawing.Point(0, 23);
            this.errorList.Name = "errorList";
            this.errorList.Size = new System.Drawing.Size(427, 127);
            this.errorList.TabIndex = 0;
            this.errorList.UseCompatibleStateImageBehavior = false;
            this.errorList.View = System.Windows.Forms.View.Details;
            this.errorList.ItemActivate += new System.EventHandler(this.errorList_ItemActivate);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.Width = 31;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Line";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Code";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Error";
            this.columnHeader3.Width = 366;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(427, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Build Errors";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel1
            // 
            this.linkLabel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.linkLabel1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabel1.Location = new System.Drawing.Point(327, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(100, 23);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Close";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel1.VisitedLinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            // 
            // BuildErrorControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.errorList);
            this.Controls.Add(this.label1);
            this.Name = "BuildErrorControl";
            this.Size = new System.Drawing.Size(427, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView errorList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}
