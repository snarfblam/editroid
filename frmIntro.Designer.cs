namespace Editroid
{
    partial class frmIntro
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
            this.TipView = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ExitButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TipView
            // 
            this.TipView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TipView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TipView.Location = new System.Drawing.Point(8, 8);
            this.TipView.Name = "TipView";
            this.TipView.ReadOnly = true;
            this.TipView.Size = new System.Drawing.Size(302, 388);
            this.TipView.TabIndex = 1;
            this.TipView.Text = "TipView";
            this.TipView.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.TipView_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ExitButton);
            this.panel1.Controls.Add(this.NextButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(8, 396);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(302, 40);
            this.panel1.TabIndex = 0;
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.Location = new System.Drawing.Point(114, 9);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 0;
            this.ExitButton.Text = "&OK";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // NextButton
            // 
            this.NextButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.NextButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NextButton.Location = new System.Drawing.Point(234, 9);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(64, 23);
            this.NextButton.TabIndex = 0;
            this.NextButton.Text = "Next &Tip...";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Visible = false;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmIntro
            // 
            this.AcceptButton = this.NextButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.ExitButton;
            this.ClientSize = new System.Drawing.Size(318, 436);
            this.Controls.Add(this.TipView);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmIntro";
            this.Padding = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox TipView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}