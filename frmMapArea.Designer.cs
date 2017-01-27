namespace Editroid
{
    partial class frmMapArea
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMapArea));
            this.pnlMap = new System.Windows.Forms.Panel();
            this.lblSelection = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cboLevel = new System.Windows.Forms.ComboBox();
            this.cboScale = new System.Windows.Forms.ComboBox();
            this.chkFillEmptySpots = new System.Windows.Forms.CheckBox();
            this.chkPhysics = new System.Windows.Forms.CheckBox();
            this.chkEnemies = new System.Windows.Forms.CheckBox();
            this.chkQuality = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.pnlMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(527, 7);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(29, 13);
            label1.TabIndex = 4;
            label1.Text = "Filter";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(527, 49);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(34, 13);
            label2.TabIndex = 6;
            label2.Text = "Scale";
            // 
            // pnlMap
            // 
            this.pnlMap.BackColor = System.Drawing.Color.Black;
            this.pnlMap.Controls.Add(this.lblSelection);
            this.pnlMap.Location = new System.Drawing.Point(12, 12);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(512, 512);
            this.pnlMap.TabIndex = 0;
            this.pnlMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMap_MouseMove);
            this.pnlMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMap_MouseDown);
            // 
            // lblSelection
            // 
            this.lblSelection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblSelection.Location = new System.Drawing.Point(0, 0);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(8, 8);
            this.lblSelection.TabIndex = 0;
            this.lblSelection.LocationChanged += new System.EventHandler(this.lblSelection_SizeChanged);
            this.lblSelection.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseMove);
            this.lblSelection.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseDown);
            this.lblSelection.SizeChanged += new System.EventHandler(this.lblSelection_SizeChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(562, 472);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(563, 501);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cboLevel
            // 
            this.cboLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLevel.FormattingEnabled = true;
            this.cboLevel.Items.AddRange(new object[] {
            "(All levels)",
            "Brinstar",
            "Norfair",
            "Ridley",
            "Kraid",
            "Tourian"});
            this.cboLevel.Location = new System.Drawing.Point(530, 24);
            this.cboLevel.Name = "cboLevel";
            this.cboLevel.Size = new System.Drawing.Size(109, 21);
            this.cboLevel.TabIndex = 3;
            this.cboLevel.SelectedIndexChanged += new System.EventHandler(this.cboLevel_SelectedIndexChanged);
            // 
            // cboScale
            // 
            this.cboScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScale.FormattingEnabled = true;
            this.cboScale.Items.AddRange(new object[] {
            "1 / 1  (Enormous)",
            "1 / 2  (Large)",
            "1 / 4  (Normal)",
            "1 / 8  (Managable)",
            "1 / 16 (Small)"});
            this.cboScale.Location = new System.Drawing.Point(530, 66);
            this.cboScale.Name = "cboScale";
            this.cboScale.Size = new System.Drawing.Size(109, 21);
            this.cboScale.TabIndex = 5;
            this.cboScale.SelectedIndexChanged += new System.EventHandler(this.cboScale_SelectedIndexChanged);
            // 
            // chkFillEmptySpots
            // 
            this.chkFillEmptySpots.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkFillEmptySpots.AutoSize = true;
            this.chkFillEmptySpots.Location = new System.Drawing.Point(530, 92);
            this.chkFillEmptySpots.Name = "chkFillEmptySpots";
            this.chkFillEmptySpots.Size = new System.Drawing.Size(100, 17);
            this.chkFillEmptySpots.TabIndex = 7;
            this.chkFillEmptySpots.Text = "Fill Empty Spots";
            this.chkFillEmptySpots.UseVisualStyleBackColor = true;
            this.chkFillEmptySpots.Visible = false;
            this.chkFillEmptySpots.CheckedChanged += new System.EventHandler(this.chkFillEmptySpots_CheckedChanged);
            // 
            // chkPhysics
            // 
            this.chkPhysics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPhysics.AutoSize = true;
            this.chkPhysics.Location = new System.Drawing.Point(530, 106);
            this.chkPhysics.Name = "chkPhysics";
            this.chkPhysics.Size = new System.Drawing.Size(92, 17);
            this.chkPhysics.TabIndex = 8;
            this.chkPhysics.Text = "Show Physics";
            this.chkPhysics.UseVisualStyleBackColor = true;
            this.chkPhysics.CheckedChanged += new System.EventHandler(this.chkPhysics_CheckedChanged);
            // 
            // chkEnemies
            // 
            this.chkEnemies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkEnemies.AutoSize = true;
            this.chkEnemies.Checked = true;
            this.chkEnemies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnemies.Location = new System.Drawing.Point(530, 120);
            this.chkEnemies.Name = "chkEnemies";
            this.chkEnemies.Size = new System.Drawing.Size(96, 17);
            this.chkEnemies.TabIndex = 9;
            this.chkEnemies.Text = "Show Enemies";
            this.chkEnemies.UseVisualStyleBackColor = true;
            this.chkEnemies.CheckedChanged += new System.EventHandler(this.chkEnemies_CheckedChanged);
            // 
            // chkQuality
            // 
            this.chkQuality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkQuality.AutoSize = true;
            this.chkQuality.Checked = true;
            this.chkQuality.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQuality.Location = new System.Drawing.Point(530, 134);
            this.chkQuality.Name = "chkQuality";
            this.chkQuality.Size = new System.Drawing.Size(83, 17);
            this.chkQuality.TabIndex = 10;
            this.chkQuality.Text = "High Quality";
            this.chkQuality.UseVisualStyleBackColor = true;
            this.chkQuality.CheckedChanged += new System.EventHandler(this.chkQuality_CheckedChanged);
            // 
            // frmMapArea
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(649, 533);
            this.Controls.Add(this.chkQuality);
            this.Controls.Add(this.chkEnemies);
            this.Controls.Add(this.chkPhysics);
            this.Controls.Add(this.chkFillEmptySpots);
            this.Controls.Add(label2);
            this.Controls.Add(this.cboScale);
            this.Controls.Add(label1);
            this.Controls.Add(this.cboLevel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pnlMap);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMapArea";
            this.ShowInTaskbar = false;
            this.Text = "View Map";
            this.pnlMap.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMap;
        private System.Windows.Forms.Label lblSelection;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cboLevel;
        private System.Windows.Forms.ComboBox cboScale;
        private System.Windows.Forms.CheckBox chkFillEmptySpots;
        private System.Windows.Forms.CheckBox chkPhysics;
        private System.Windows.Forms.CheckBox chkEnemies;
        private System.Windows.Forms.CheckBox chkQuality;
    }
}