namespace Editroid
{
    partial class frmPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPassword));
            this.pnlMap = new System.Windows.Forms.Panel();
            this.lblCurrentItem = new System.Windows.Forms.Label();
            this.lstSubtypes = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstEntries = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMap
            // 
            this.pnlMap.Controls.Add(this.lblCurrentItem);
            this.pnlMap.Location = new System.Drawing.Point(12, 12);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(256, 256);
            this.pnlMap.TabIndex = 3;
            this.pnlMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMap_MouseDown);
            // 
            // lblCurrentItem
            // 
            this.lblCurrentItem.BackColor = System.Drawing.Color.White;
            this.lblCurrentItem.Location = new System.Drawing.Point(0, 0);
            this.lblCurrentItem.Name = "lblCurrentItem";
            this.lblCurrentItem.Size = new System.Drawing.Size(8, 8);
            this.lblCurrentItem.TabIndex = 0;
            this.lblCurrentItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblCurrentItem_MouseMove);
            this.lblCurrentItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblCurrentItem_MouseDown);
            this.lblCurrentItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblCurrentItem_MouseUp);
            // 
            // lstSubtypes
            // 
            this.lstSubtypes.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstSubtypes.FormattingEnabled = true;
            this.lstSubtypes.IntegralHeight = false;
            this.lstSubtypes.Items.AddRange(new object[] {
            "Bomb",
            "High Jump",
            "Long Beam",
            "Screw Attack",
            "Maru Mari",
            "Varia",
            "Wave Beam",
            "Ice Beam",
            "Energy Tank",
            "Missile Expansion",
            "Door",
            "(Invalid)",
            "(Invalid)",
            "(Invalid)",
            "(Invalid)",
            "(Invalid)"});
            this.lstSubtypes.Location = new System.Drawing.Point(432, 24);
            this.lstSubtypes.Name = "lstSubtypes";
            this.lstSubtypes.Size = new System.Drawing.Size(118, 243);
            this.lstSubtypes.TabIndex = 4;
            this.lstSubtypes.SelectedIndexChanged += new System.EventHandler(this.lstSubtypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Entry";
            // 
            // lstEntries
            // 
            this.lstEntries.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstEntries.FormattingEnabled = true;
            this.lstEntries.IntegralHeight = false;
            this.lstEntries.Location = new System.Drawing.Point(277, 25);
            this.lstEntries.Name = "lstEntries";
            this.lstEntries.Size = new System.Drawing.Size(149, 243);
            this.lstEntries.TabIndex = 6;
            this.lstEntries.SelectedIndexChanged += new System.EventHandler(this.lstEntries_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(429, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Entry Type";
            // 
            // frmPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(562, 279);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstEntries);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstSubtypes);
            this.Controls.Add(this.pnlMap);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPassword";
            this.Text = "Password Tracking Editor";
            this.pnlMap.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMap;
        private System.Windows.Forms.ListBox lstSubtypes;
        private System.Windows.Forms.Label lblCurrentItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstEntries;
        private System.Windows.Forms.Label label2;
    }
}