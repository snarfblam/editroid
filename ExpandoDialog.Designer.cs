namespace Editroid
{
    partial class ExpandoDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpandoDialog));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOX = new System.Windows.Forms.Button();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.chkUpdateFixed = new System.Windows.Forms.CheckBox();
            this.chkUpdateExp = new System.Windows.Forms.CheckBox();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Black;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(313, 283);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "words";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(183, 301);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOX
            // 
            this.btnOX.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOX.Location = new System.Drawing.Point(78, 301);
            this.btnOX.Name = "btnOX";
            this.btnOX.Size = new System.Drawing.Size(99, 23);
            this.btnOX.TabIndex = 2;
            this.btnOX.Text = "Expand ROM";
            this.btnOX.UseVisualStyleBackColor = true;
            this.btnOX.Click += new System.EventHandler(this.btnOX_Click);
            // 
            // pnlOptions
            // 
            this.pnlOptions.Controls.Add(this.chkUpdateFixed);
            this.pnlOptions.Controls.Add(this.chkUpdateExp);
            this.pnlOptions.ForeColor = System.Drawing.Color.White;
            this.pnlOptions.Location = new System.Drawing.Point(12, 330);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(313, 51);
            this.pnlOptions.TabIndex = 3;
            // 
            // chkUpdateFixed
            // 
            this.chkUpdateFixed.AutoSize = true;
            this.chkUpdateFixed.Checked = true;
            this.chkUpdateFixed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateFixed.Location = new System.Drawing.Point(3, 26);
            this.chkUpdateFixed.Name = "chkUpdateFixed";
            this.chkUpdateFixed.Size = new System.Drawing.Size(241, 17);
            this.chkUpdateFixed.TabIndex = 1;
            this.chkUpdateFixed.Text = "Update references to fixed bank (.PATCH 0F)";
            this.chkUpdateFixed.UseVisualStyleBackColor = true;
            // 
            // chkUpdateExp
            // 
            this.chkUpdateExp.AutoSize = true;
            this.chkUpdateExp.Checked = true;
            this.chkUpdateExp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateExp.Location = new System.Drawing.Point(3, 3);
            this.chkUpdateExp.Name = "chkUpdateExp";
            this.chkUpdateExp.Size = new System.Drawing.Size(132, 17);
            this.chkUpdateExp.TabIndex = 0;
            this.chkUpdateExp.Text = "Update Expansion File";
            this.chkUpdateExp.UseVisualStyleBackColor = true;
            // 
            // ExpandoDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.CancelButton = this.btnOX;
            this.ClientSize = new System.Drawing.Size(337, 389);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.btnOX);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExpandoDialog";
            this.ShowInTaskbar = false;
            this.Text = "Expanding ROM";
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOX;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.CheckBox chkUpdateFixed;
        private System.Windows.Forms.CheckBox chkUpdateExp;
    }
}