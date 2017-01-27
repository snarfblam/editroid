namespace Editroid
{
    partial class IpsForm
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
            if(disposing && (components != null)) {
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
            this.grpRom = new System.Windows.Forms.GroupBox();
            this.picRom = new System.Windows.Forms.PictureBox();
            this.btnRomApply = new System.Windows.Forms.Button();
            this.btnRomLoad = new System.Windows.Forms.Button();
            this.btnRomCurrent = new System.Windows.Forms.Button();
            this.lblRomPath = new System.Windows.Forms.Label();
            this.grpPatch = new System.Windows.Forms.GroupBox();
            this.picIps = new System.Windows.Forms.PictureBox();
            this.btnPatchLoad = new System.Windows.Forms.Button();
            this.lblPatchPath = new System.Windows.Forms.Label();
            this.btnPatchCreate = new System.Windows.Forms.Button();
            this.grpCompare = new System.Windows.Forms.GroupBox();
            this.picCompare = new System.Windows.Forms.PictureBox();
            this.btnCompareApply = new System.Windows.Forms.Button();
            this.btnCompareLoad = new System.Windows.Forms.Button();
            this.btnCompareCurrent = new System.Windows.Forms.Button();
            this.lblComparePath = new System.Windows.Forms.Label();
            this.RomOpener = new System.Windows.Forms.OpenFileDialog();
            this.RomSaver = new System.Windows.Forms.SaveFileDialog();
            this.PatchOpener = new System.Windows.Forms.OpenFileDialog();
            this.PatchSaver = new System.Windows.Forms.SaveFileDialog();
            this.grpRom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRom)).BeginInit();
            this.grpPatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIps)).BeginInit();
            this.grpCompare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCompare)).BeginInit();
            this.SuspendLayout();
            // 
            // grpRom
            // 
            this.grpRom.Controls.Add(this.picRom);
            this.grpRom.Controls.Add(this.btnRomApply);
            this.grpRom.Controls.Add(this.btnRomLoad);
            this.grpRom.Controls.Add(this.btnRomCurrent);
            this.grpRom.Controls.Add(this.lblRomPath);
            this.grpRom.ForeColor = System.Drawing.Color.Red;
            this.grpRom.Location = new System.Drawing.Point(12, 12);
            this.grpRom.Name = "grpRom";
            this.grpRom.Size = new System.Drawing.Size(268, 67);
            this.grpRom.TabIndex = 0;
            this.grpRom.TabStop = false;
            this.grpRom.Text = "ROM File";
            // 
            // picRom
            // 
            this.picRom.Location = new System.Drawing.Point(6, 16);
            this.picRom.Name = "picRom";
            this.picRom.Size = new System.Drawing.Size(16, 16);
            this.picRom.TabIndex = 6;
            this.picRom.TabStop = false;
            // 
            // btnRomApply
            // 
            this.btnRomApply.Enabled = false;
            this.btnRomApply.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRomApply.Location = new System.Drawing.Point(168, 36);
            this.btnRomApply.Name = "btnRomApply";
            this.btnRomApply.Size = new System.Drawing.Size(75, 23);
            this.btnRomApply.TabIndex = 5;
            this.btnRomApply.Text = "Apply Patch";
            this.btnRomApply.UseVisualStyleBackColor = true;
            this.btnRomApply.Click += new System.EventHandler(this.btnRomApply_Click);
            // 
            // btnRomLoad
            // 
            this.btnRomLoad.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRomLoad.Location = new System.Drawing.Point(87, 36);
            this.btnRomLoad.Name = "btnRomLoad";
            this.btnRomLoad.Size = new System.Drawing.Size(75, 23);
            this.btnRomLoad.TabIndex = 4;
            this.btnRomLoad.Text = "Load...";
            this.btnRomLoad.UseVisualStyleBackColor = true;
            this.btnRomLoad.Click += new System.EventHandler(this.btnRomLoad_Click);
            // 
            // btnRomCurrent
            // 
            this.btnRomCurrent.Enabled = false;
            this.btnRomCurrent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRomCurrent.Location = new System.Drawing.Point(6, 36);
            this.btnRomCurrent.Name = "btnRomCurrent";
            this.btnRomCurrent.Size = new System.Drawing.Size(75, 23);
            this.btnRomCurrent.TabIndex = 3;
            this.btnRomCurrent.Text = "Current";
            this.btnRomCurrent.UseVisualStyleBackColor = true;
            this.btnRomCurrent.Click += new System.EventHandler(this.btnRomCurrent_Click);
            // 
            // lblRomPath
            // 
            this.lblRomPath.AutoSize = true;
            this.lblRomPath.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblRomPath.Location = new System.Drawing.Point(28, 16);
            this.lblRomPath.Name = "lblRomPath";
            this.lblRomPath.Size = new System.Drawing.Size(39, 13);
            this.lblRomPath.TabIndex = 0;
            this.lblRomPath.Text = "(None)";
            // 
            // grpPatch
            // 
            this.grpPatch.Controls.Add(this.picIps);
            this.grpPatch.Controls.Add(this.btnPatchLoad);
            this.grpPatch.Controls.Add(this.lblPatchPath);
            this.grpPatch.Controls.Add(this.btnPatchCreate);
            this.grpPatch.ForeColor = System.Drawing.Color.Red;
            this.grpPatch.Location = new System.Drawing.Point(12, 158);
            this.grpPatch.Name = "grpPatch";
            this.grpPatch.Size = new System.Drawing.Size(268, 67);
            this.grpPatch.TabIndex = 2;
            this.grpPatch.TabStop = false;
            this.grpPatch.Text = "Patch File";
            // 
            // picIps
            // 
            this.picIps.Location = new System.Drawing.Point(6, 16);
            this.picIps.Name = "picIps";
            this.picIps.Size = new System.Drawing.Size(16, 16);
            this.picIps.TabIndex = 8;
            this.picIps.TabStop = false;
            // 
            // btnPatchLoad
            // 
            this.btnPatchLoad.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPatchLoad.Location = new System.Drawing.Point(87, 36);
            this.btnPatchLoad.Name = "btnPatchLoad";
            this.btnPatchLoad.Size = new System.Drawing.Size(75, 23);
            this.btnPatchLoad.TabIndex = 8;
            this.btnPatchLoad.Text = "Load...";
            this.btnPatchLoad.UseVisualStyleBackColor = true;
            this.btnPatchLoad.Click += new System.EventHandler(this.btnPatchLoad_Click);
            // 
            // lblPatchPath
            // 
            this.lblPatchPath.AutoSize = true;
            this.lblPatchPath.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPatchPath.Location = new System.Drawing.Point(28, 16);
            this.lblPatchPath.Name = "lblPatchPath";
            this.lblPatchPath.Size = new System.Drawing.Size(39, 13);
            this.lblPatchPath.TabIndex = 2;
            this.lblPatchPath.Text = "(None)";
            // 
            // btnPatchCreate
            // 
            this.btnPatchCreate.Enabled = false;
            this.btnPatchCreate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPatchCreate.Location = new System.Drawing.Point(6, 36);
            this.btnPatchCreate.Name = "btnPatchCreate";
            this.btnPatchCreate.Size = new System.Drawing.Size(75, 23);
            this.btnPatchCreate.TabIndex = 7;
            this.btnPatchCreate.Text = "Create";
            this.btnPatchCreate.UseVisualStyleBackColor = true;
            this.btnPatchCreate.Click += new System.EventHandler(this.btnPatchCreate_Click);
            // 
            // grpCompare
            // 
            this.grpCompare.Controls.Add(this.picCompare);
            this.grpCompare.Controls.Add(this.btnCompareApply);
            this.grpCompare.Controls.Add(this.btnCompareLoad);
            this.grpCompare.Controls.Add(this.btnCompareCurrent);
            this.grpCompare.Controls.Add(this.lblComparePath);
            this.grpCompare.ForeColor = System.Drawing.Color.Red;
            this.grpCompare.Location = new System.Drawing.Point(12, 85);
            this.grpCompare.Name = "grpCompare";
            this.grpCompare.Size = new System.Drawing.Size(268, 67);
            this.grpCompare.TabIndex = 6;
            this.grpCompare.TabStop = false;
            this.grpCompare.Text = "Original ROM File";
            // 
            // picCompare
            // 
            this.picCompare.Location = new System.Drawing.Point(6, 16);
            this.picCompare.Name = "picCompare";
            this.picCompare.Size = new System.Drawing.Size(16, 16);
            this.picCompare.TabIndex = 7;
            this.picCompare.TabStop = false;
            // 
            // btnCompareApply
            // 
            this.btnCompareApply.Enabled = false;
            this.btnCompareApply.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCompareApply.Location = new System.Drawing.Point(168, 36);
            this.btnCompareApply.Name = "btnCompareApply";
            this.btnCompareApply.Size = new System.Drawing.Size(75, 23);
            this.btnCompareApply.TabIndex = 5;
            this.btnCompareApply.Text = "Apply Patch";
            this.btnCompareApply.UseVisualStyleBackColor = true;
            this.btnCompareApply.Visible = false;
            this.btnCompareApply.Click += new System.EventHandler(this.btnCompareApply_Click);
            // 
            // btnCompareLoad
            // 
            this.btnCompareLoad.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCompareLoad.Location = new System.Drawing.Point(87, 36);
            this.btnCompareLoad.Name = "btnCompareLoad";
            this.btnCompareLoad.Size = new System.Drawing.Size(75, 23);
            this.btnCompareLoad.TabIndex = 4;
            this.btnCompareLoad.Text = "Load...";
            this.btnCompareLoad.UseVisualStyleBackColor = true;
            this.btnCompareLoad.Click += new System.EventHandler(this.btnCompareLoad_Click);
            // 
            // btnCompareCurrent
            // 
            this.btnCompareCurrent.Enabled = false;
            this.btnCompareCurrent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCompareCurrent.Location = new System.Drawing.Point(6, 36);
            this.btnCompareCurrent.Name = "btnCompareCurrent";
            this.btnCompareCurrent.Size = new System.Drawing.Size(75, 23);
            this.btnCompareCurrent.TabIndex = 3;
            this.btnCompareCurrent.Text = "Current";
            this.btnCompareCurrent.UseVisualStyleBackColor = true;
            this.btnCompareCurrent.Click += new System.EventHandler(this.btnCompareCurrent_Click);
            // 
            // lblComparePath
            // 
            this.lblComparePath.AutoSize = true;
            this.lblComparePath.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblComparePath.Location = new System.Drawing.Point(28, 16);
            this.lblComparePath.Name = "lblComparePath";
            this.lblComparePath.Size = new System.Drawing.Size(39, 13);
            this.lblComparePath.TabIndex = 0;
            this.lblComparePath.Text = "(None)";
            // 
            // RomOpener
            // 
            this.RomOpener.Filter = "NES Rom (*.nes)|*.nes|All Files|*.*";
            // 
            // RomSaver
            // 
            this.RomSaver.Filter = "NES Rom (*.nes)|*.nes|All Files|*.*";
            // 
            // PatchOpener
            // 
            this.PatchOpener.Filter = "IPS Patch (*.ips)|*.ips|All Files|*.*";
            // 
            // PatchSaver
            // 
            this.PatchSaver.Filter = "IPS Patch (*.ips)|*.ips|All Files|*.*";
            // 
            // IpsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(292, 250);
            this.Controls.Add(this.grpCompare);
            this.Controls.Add(this.grpPatch);
            this.Controls.Add(this.grpRom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IpsForm";
            this.Text = "IPS Utility";
            this.grpRom.ResumeLayout(false);
            this.grpRom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRom)).EndInit();
            this.grpPatch.ResumeLayout(false);
            this.grpPatch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIps)).EndInit();
            this.grpCompare.ResumeLayout(false);
            this.grpCompare.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCompare)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpRom;
        private System.Windows.Forms.Button btnRomApply;
        private System.Windows.Forms.Button btnRomLoad;
        private System.Windows.Forms.Button btnRomCurrent;
        private System.Windows.Forms.Label lblRomPath;
        private System.Windows.Forms.GroupBox grpPatch;
        private System.Windows.Forms.Button btnPatchLoad;
        private System.Windows.Forms.Label lblPatchPath;
        private System.Windows.Forms.Button btnPatchCreate;
        private System.Windows.Forms.GroupBox grpCompare;
        private System.Windows.Forms.Button btnCompareApply;
        private System.Windows.Forms.Button btnCompareLoad;
        private System.Windows.Forms.Button btnCompareCurrent;
        private System.Windows.Forms.Label lblComparePath;
        private System.Windows.Forms.OpenFileDialog RomOpener;
        private System.Windows.Forms.SaveFileDialog RomSaver;
        private System.Windows.Forms.OpenFileDialog PatchOpener;
        private System.Windows.Forms.SaveFileDialog PatchSaver;
        private System.Windows.Forms.PictureBox picRom;
        private System.Windows.Forms.PictureBox picIps;
        private System.Windows.Forms.PictureBox picCompare;
    }
}