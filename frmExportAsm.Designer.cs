namespace Editroid
{
    partial class frmExportAsm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.FileList = new System.Windows.Forms.CheckedListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblBadFolderError = new System.Windows.Forms.Label();
            this.explorerTreeView1 = new WilsonProgramming.ExplorerTreeView();
            this.chkAsmExtension = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // FileList
            // 
            this.FileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FileList.FormattingEnabled = true;
            this.FileList.IntegralHeight = false;
            this.FileList.Location = new System.Drawing.Point(290, 33);
            this.FileList.MultiColumn = true;
            this.FileList.Name = "FileList";
            this.FileList.Size = new System.Drawing.Size(204, 288);
            this.FileList.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(419, 327);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(338, 327);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(290, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "Files To Export:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 21);
            this.label2.TabIndex = 5;
            this.label2.Text = "Export To:";
            // 
            // lblBadFolderError
            // 
            this.lblBadFolderError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBadFolderError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblBadFolderError.Location = new System.Drawing.Point(12, 9);
            this.lblBadFolderError.Name = "lblBadFolderError";
            this.lblBadFolderError.Size = new System.Drawing.Size(267, 21);
            this.lblBadFolderError.TabIndex = 6;
            this.lblBadFolderError.Text = "Can not save to the specified location";
            this.lblBadFolderError.Visible = false;
            // 
            // explorerTreeView1
            // 
            this.explorerTreeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.explorerTreeView1.Location = new System.Drawing.Point(12, 33);
            this.explorerTreeView1.Name = "explorerTreeView1";
            this.explorerTreeView1.Size = new System.Drawing.Size(267, 288);
            this.explorerTreeView1.TabIndex = 4;
            // 
            // chkAsmExtension
            // 
            this.chkAsmExtension.AutoSize = true;
            this.chkAsmExtension.Checked = true;
            this.chkAsmExtension.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAsmExtension.Location = new System.Drawing.Point(15, 331);
            this.chkAsmExtension.Name = "chkAsmExtension";
            this.chkAsmExtension.Size = new System.Drawing.Size(125, 17);
            this.chkAsmExtension.TabIndex = 7;
            this.chkAsmExtension.Text = "Apply .asm extension";
            this.chkAsmExtension.UseVisualStyleBackColor = true;
            // 
            // frmExportAsm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(506, 362);
            this.Controls.Add(this.chkAsmExtension);
            this.Controls.Add(this.lblBadFolderError);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.explorerTreeView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.FileList);
            this.MinimizeBox = false;
            this.Name = "frmExportAsm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Export ASM";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox FileList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private WilsonProgramming.ExplorerTreeView explorerTreeView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblBadFolderError;
        private System.Windows.Forms.CheckBox chkAsmExtension;
    }
}