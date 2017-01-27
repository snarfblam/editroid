namespace Editroid
{
    partial class frmBuildErrors
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
            this.errorControl = new Editroid.Controls.BuildErrorControl();
            this.SuspendLayout();
            // 
            // errorControl
            // 
            this.errorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorControl.Location = new System.Drawing.Point(0, 0);
            this.errorControl.Name = "errorControl";
            this.errorControl.ShowCaption = false;
            this.errorControl.Size = new System.Drawing.Size(622, 266);
            this.errorControl.TabIndex = 0;
            // 
            // frmBuildErrors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(622, 266);
            this.Controls.Add(this.errorControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmBuildErrors";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Build Errors";
            this.ResumeLayout(false);

        }

        #endregion

        private Editroid.Controls.BuildErrorControl errorControl;
    }
}