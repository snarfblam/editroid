namespace Editroid
{
    partial class frmDump
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDump));
            this.pnlDumpEasy = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.lnkUnselectAll = new System.Windows.Forms.LinkLabel();
            this.lnkSelectAll = new System.Windows.Forms.LinkLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkAsm = new System.Windows.Forms.CheckBox();
            this.chkTitleChrAnim = new System.Windows.Forms.CheckBox();
            this.chkTitleChr = new System.Windows.Forms.CheckBox();
            this.chkPassword = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkAreaTilePhysics = new System.Windows.Forms.CheckBox();
            this.chkAreaAsm = new System.Windows.Forms.CheckBox();
            this.chkAreaScreens = new System.Windows.Forms.CheckBox();
            this.chkAreaStructures = new System.Windows.Forms.CheckBox();
            this.chkAreaCombos = new System.Windows.Forms.CheckBox();
            this.chkAreaChr = new System.Windows.Forms.CheckBox();
            this.chkAreaAltMusic = new System.Windows.Forms.CheckBox();
            this.chkAreaItems = new System.Windows.Forms.CheckBox();
            this.chkAreaPalette = new System.Windows.Forms.CheckBox();
            this.chkAreaChrAnim = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkRidley = new System.Windows.Forms.CheckBox();
            this.chkKraid = new System.Windows.Forms.CheckBox();
            this.chkTourian = new System.Windows.Forms.CheckBox();
            this.chkNorfair = new System.Windows.Forms.CheckBox();
            this.chkBrinstar = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlDumpEasy.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDumpEasy
            // 
            this.pnlDumpEasy.Controls.Add(this.btnCancel);
            this.pnlDumpEasy.Controls.Add(this.btnSave);
            this.pnlDumpEasy.Controls.Add(this.lnkUnselectAll);
            this.pnlDumpEasy.Controls.Add(this.lnkSelectAll);
            this.pnlDumpEasy.Controls.Add(this.groupBox3);
            this.pnlDumpEasy.Controls.Add(this.groupBox2);
            this.pnlDumpEasy.Controls.Add(this.groupBox1);
            this.pnlDumpEasy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDumpEasy.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlDumpEasy.Location = new System.Drawing.Point(0, 0);
            this.pnlDumpEasy.Name = "pnlDumpEasy";
            this.pnlDumpEasy.Size = new System.Drawing.Size(402, 333);
            this.pnlDumpEasy.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(315, 298);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // lnkUnselectAll
            // 
            this.lnkUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkUnselectAll.AutoSize = true;
            this.lnkUnselectAll.Location = new System.Drawing.Point(77, 306);
            this.lnkUnselectAll.Name = "lnkUnselectAll";
            this.lnkUnselectAll.Size = new System.Drawing.Size(73, 18);
            this.lnkUnselectAll.TabIndex = 15;
            this.lnkUnselectAll.TabStop = true;
            this.lnkUnselectAll.Text = "Unselect All";
            this.lnkUnselectAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUnselectAll_LinkClicked);
            // 
            // lnkSelectAll
            // 
            this.lnkSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkSelectAll.AutoSize = true;
            this.lnkSelectAll.Location = new System.Drawing.Point(12, 306);
            this.lnkSelectAll.Name = "lnkSelectAll";
            this.lnkSelectAll.Size = new System.Drawing.Size(59, 18);
            this.lnkSelectAll.TabIndex = 13;
            this.lnkSelectAll.TabStop = true;
            this.lnkSelectAll.Text = "Select All";
            this.lnkSelectAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSelectAll_LinkClicked);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.chkAsm);
            this.groupBox3.Controls.Add(this.chkTitleChrAnim);
            this.groupBox3.Controls.Add(this.chkTitleChr);
            this.groupBox3.Controls.Add(this.chkPassword);
            this.groupBox3.Location = new System.Drawing.Point(265, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(124, 280);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Global Data";
            // 
            // chkAsm
            // 
            this.chkAsm.AutoSize = true;
            this.chkAsm.Location = new System.Drawing.Point(6, 88);
            this.chkAsm.Name = "chkAsm";
            this.chkAsm.Size = new System.Drawing.Size(94, 22);
            this.chkAsm.TabIndex = 12;
            this.chkAsm.Text = "Project ASM";
            this.toolTip1.SetToolTip(this.chkAsm, "All ASM excluding screen load ASM.");
            this.chkAsm.UseVisualStyleBackColor = true;
            // 
            // chkTitleChrAnim
            // 
            this.chkTitleChrAnim.AutoSize = true;
            this.chkTitleChrAnim.Location = new System.Drawing.Point(6, 65);
            this.chkTitleChrAnim.Name = "chkTitleChrAnim";
            this.chkTitleChrAnim.Size = new System.Drawing.Size(109, 22);
            this.chkTitleChrAnim.TabIndex = 11;
            this.chkTitleChrAnim.Text = "Title Animation";
            this.toolTip1.SetToolTip(this.chkTitleChrAnim, "CHR animation data (for animated tiles) for title screen.");
            this.chkTitleChrAnim.UseVisualStyleBackColor = true;
            // 
            // chkTitleChr
            // 
            this.chkTitleChr.AutoSize = true;
            this.chkTitleChr.Location = new System.Drawing.Point(6, 42);
            this.chkTitleChr.Name = "chkTitleChr";
            this.chkTitleChr.Size = new System.Drawing.Size(104, 22);
            this.chkTitleChr.TabIndex = 10;
            this.chkTitleChr.Text = "Title Graphics";
            this.toolTip1.SetToolTip(this.chkTitleChr, "Graphics (CHR) for title screen.");
            this.chkTitleChr.UseVisualStyleBackColor = true;
            // 
            // chkPassword
            // 
            this.chkPassword.AutoSize = true;
            this.chkPassword.Location = new System.Drawing.Point(6, 19);
            this.chkPassword.Name = "chkPassword";
            this.chkPassword.Size = new System.Drawing.Size(105, 22);
            this.chkPassword.TabIndex = 8;
            this.chkPassword.Text = "Password Data";
            this.toolTip1.SetToolTip(this.chkPassword, "Password tracking data. Associates password data with items on the map.");
            this.chkPassword.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.chkAreaTilePhysics);
            this.groupBox2.Controls.Add(this.chkAreaAsm);
            this.groupBox2.Controls.Add(this.chkAreaScreens);
            this.groupBox2.Controls.Add(this.chkAreaStructures);
            this.groupBox2.Controls.Add(this.chkAreaCombos);
            this.groupBox2.Controls.Add(this.chkAreaChr);
            this.groupBox2.Controls.Add(this.chkAreaAltMusic);
            this.groupBox2.Controls.Add(this.chkAreaItems);
            this.groupBox2.Controls.Add(this.chkAreaPalette);
            this.groupBox2.Controls.Add(this.chkAreaChrAnim);
            this.groupBox2.Location = new System.Drawing.Point(124, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(135, 280);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Area Data";
            // 
            // chkAreaTilePhysics
            // 
            this.chkAreaTilePhysics.AutoSize = true;
            this.chkAreaTilePhysics.Location = new System.Drawing.Point(6, 226);
            this.chkAreaTilePhysics.Name = "chkAreaTilePhysics";
            this.chkAreaTilePhysics.Size = new System.Drawing.Size(90, 22);
            this.chkAreaTilePhysics.TabIndex = 14;
            this.chkAreaTilePhysics.Text = "Tile Physics";
            this.toolTip1.SetToolTip(this.chkAreaTilePhysics, "Custom tile physics. Applies to MMC3 (\"Rogue Edition\") ROMs only.");
            this.chkAreaTilePhysics.UseVisualStyleBackColor = true;
            // 
            // chkAreaAsm
            // 
            this.chkAreaAsm.AutoSize = true;
            this.chkAreaAsm.Location = new System.Drawing.Point(6, 203);
            this.chkAreaAsm.Name = "chkAreaAsm";
            this.chkAreaAsm.Size = new System.Drawing.Size(122, 22);
            this.chkAreaAsm.TabIndex = 13;
            this.chkAreaAsm.Text = "Screen Load ASM";
            this.toolTip1.SetToolTip(this.chkAreaAsm, "ASM associated with area screens.");
            this.chkAreaAsm.UseVisualStyleBackColor = true;
            // 
            // chkAreaScreens
            // 
            this.chkAreaScreens.AutoSize = true;
            this.chkAreaScreens.Location = new System.Drawing.Point(6, 157);
            this.chkAreaScreens.Name = "chkAreaScreens";
            this.chkAreaScreens.Size = new System.Drawing.Size(71, 22);
            this.chkAreaScreens.TabIndex = 13;
            this.chkAreaScreens.Text = "Screens";
            this.chkAreaScreens.UseVisualStyleBackColor = true;
            // 
            // chkAreaStructures
            // 
            this.chkAreaStructures.AutoSize = true;
            this.chkAreaStructures.Location = new System.Drawing.Point(6, 134);
            this.chkAreaStructures.Name = "chkAreaStructures";
            this.chkAreaStructures.Size = new System.Drawing.Size(86, 22);
            this.chkAreaStructures.TabIndex = 12;
            this.chkAreaStructures.Text = "Structures";
            this.chkAreaStructures.UseVisualStyleBackColor = true;
            // 
            // chkAreaCombos
            // 
            this.chkAreaCombos.AutoSize = true;
            this.chkAreaCombos.Location = new System.Drawing.Point(6, 111);
            this.chkAreaCombos.Name = "chkAreaCombos";
            this.chkAreaCombos.Size = new System.Drawing.Size(69, 22);
            this.chkAreaCombos.TabIndex = 11;
            this.chkAreaCombos.Text = "Combos";
            this.chkAreaCombos.UseVisualStyleBackColor = true;
            // 
            // chkAreaChr
            // 
            this.chkAreaChr.AutoSize = true;
            this.chkAreaChr.Location = new System.Drawing.Point(6, 88);
            this.chkAreaChr.Name = "chkAreaChr";
            this.chkAreaChr.Size = new System.Drawing.Size(75, 22);
            this.chkAreaChr.TabIndex = 10;
            this.chkAreaChr.Text = "Graphics";
            this.toolTip1.SetToolTip(this.chkAreaChr, "Graphics (CHR)");
            this.chkAreaChr.UseVisualStyleBackColor = true;
            // 
            // chkAreaAltMusic
            // 
            this.chkAreaAltMusic.AutoSize = true;
            this.chkAreaAltMusic.Location = new System.Drawing.Point(6, 180);
            this.chkAreaAltMusic.Name = "chkAreaAltMusic";
            this.chkAreaAltMusic.Size = new System.Drawing.Size(108, 22);
            this.chkAreaAltMusic.TabIndex = 9;
            this.chkAreaAltMusic.Text = "Alt Music Flags";
            this.toolTip1.SetToolTip(this.chkAreaAltMusic, "List of which screen layouts play the alternate music.");
            this.chkAreaAltMusic.UseVisualStyleBackColor = true;
            // 
            // chkAreaItems
            // 
            this.chkAreaItems.AutoSize = true;
            this.chkAreaItems.Location = new System.Drawing.Point(6, 65);
            this.chkAreaItems.Name = "chkAreaItems";
            this.chkAreaItems.Size = new System.Drawing.Size(79, 22);
            this.chkAreaItems.TabIndex = 7;
            this.chkAreaItems.Text = "Item Data";
            this.toolTip1.SetToolTip(this.chkAreaItems, "May include some enemies and door bubbles as well.");
            this.chkAreaItems.UseVisualStyleBackColor = true;
            // 
            // chkAreaPalette
            // 
            this.chkAreaPalette.AutoSize = true;
            this.chkAreaPalette.Location = new System.Drawing.Point(6, 42);
            this.chkAreaPalette.Name = "chkAreaPalette";
            this.chkAreaPalette.Size = new System.Drawing.Size(71, 22);
            this.chkAreaPalette.TabIndex = 6;
            this.chkAreaPalette.Text = "Palettes";
            this.toolTip1.SetToolTip(this.chkAreaPalette, "Includes all area palettes.");
            this.chkAreaPalette.UseVisualStyleBackColor = true;
            // 
            // chkAreaChrAnim
            // 
            this.chkAreaChrAnim.AutoSize = true;
            this.chkAreaChrAnim.Location = new System.Drawing.Point(6, 19);
            this.chkAreaChrAnim.Name = "chkAreaChrAnim";
            this.chkAreaChrAnim.Size = new System.Drawing.Size(106, 22);
            this.chkAreaChrAnim.TabIndex = 5;
            this.chkAreaChrAnim.Text = "CHR Animation";
            this.toolTip1.SetToolTip(this.chkAreaChrAnim, "CHR animation sequence (for animated tiles)");
            this.chkAreaChrAnim.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.chkRidley);
            this.groupBox1.Controls.Add(this.chkKraid);
            this.groupBox1.Controls.Add(this.chkTourian);
            this.groupBox1.Controls.Add(this.chkNorfair);
            this.groupBox1.Controls.Add(this.chkBrinstar);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(106, 280);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Areas";
            // 
            // chkRidley
            // 
            this.chkRidley.AutoSize = true;
            this.chkRidley.Location = new System.Drawing.Point(6, 111);
            this.chkRidley.Name = "chkRidley";
            this.chkRidley.Size = new System.Drawing.Size(60, 22);
            this.chkRidley.TabIndex = 9;
            this.chkRidley.Text = "Ridley";
            this.toolTip1.SetToolTip(this.chkRidley, "Include Ridley\'s hideout in area data.");
            this.chkRidley.UseVisualStyleBackColor = true;
            // 
            // chkKraid
            // 
            this.chkKraid.AutoSize = true;
            this.chkKraid.Location = new System.Drawing.Point(6, 88);
            this.chkKraid.Name = "chkKraid";
            this.chkKraid.Size = new System.Drawing.Size(55, 22);
            this.chkKraid.TabIndex = 8;
            this.chkKraid.Text = "Kraid";
            this.toolTip1.SetToolTip(this.chkKraid, "Include Kraid\'s hideout in area data.");
            this.chkKraid.UseVisualStyleBackColor = true;
            // 
            // chkTourian
            // 
            this.chkTourian.AutoSize = true;
            this.chkTourian.Location = new System.Drawing.Point(6, 65);
            this.chkTourian.Name = "chkTourian";
            this.chkTourian.Size = new System.Drawing.Size(68, 22);
            this.chkTourian.TabIndex = 7;
            this.chkTourian.Text = "Tourian";
            this.toolTip1.SetToolTip(this.chkTourian, "Include Torian in area data.");
            this.chkTourian.UseVisualStyleBackColor = true;
            // 
            // chkNorfair
            // 
            this.chkNorfair.AutoSize = true;
            this.chkNorfair.Location = new System.Drawing.Point(6, 42);
            this.chkNorfair.Name = "chkNorfair";
            this.chkNorfair.Size = new System.Drawing.Size(65, 22);
            this.chkNorfair.TabIndex = 6;
            this.chkNorfair.Text = "Norfair";
            this.toolTip1.SetToolTip(this.chkNorfair, "Include Norfair in area data.");
            this.chkNorfair.UseVisualStyleBackColor = true;
            // 
            // chkBrinstar
            // 
            this.chkBrinstar.AutoSize = true;
            this.chkBrinstar.Location = new System.Drawing.Point(6, 19);
            this.chkBrinstar.Name = "chkBrinstar";
            this.chkBrinstar.Size = new System.Drawing.Size(70, 22);
            this.chkBrinstar.TabIndex = 5;
            this.chkBrinstar.Text = "Brinstar";
            this.toolTip1.SetToolTip(this.chkBrinstar, "Include Brinstar in area data.");
            this.chkBrinstar.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(234, 298);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmDump
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 333);
            this.Controls.Add(this.pnlDumpEasy);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "frmDump";
            this.ShowInTaskbar = false;
            this.Text = "Dump Data";
            this.pnlDumpEasy.ResumeLayout(false);
            this.pnlDumpEasy.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlDumpEasy;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkAreaAltMusic;
        private System.Windows.Forms.CheckBox chkAreaItems;
        private System.Windows.Forms.CheckBox chkAreaPalette;
        private System.Windows.Forms.CheckBox chkAreaChrAnim;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkRidley;
        private System.Windows.Forms.CheckBox chkKraid;
        private System.Windows.Forms.CheckBox chkTourian;
        private System.Windows.Forms.CheckBox chkNorfair;
        private System.Windows.Forms.CheckBox chkBrinstar;
        private System.Windows.Forms.CheckBox chkAreaScreens;
        private System.Windows.Forms.CheckBox chkAreaStructures;
        private System.Windows.Forms.CheckBox chkAreaCombos;
        private System.Windows.Forms.CheckBox chkAreaChr;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkAsm;
        private System.Windows.Forms.CheckBox chkTitleChrAnim;
        private System.Windows.Forms.CheckBox chkTitleChr;
        private System.Windows.Forms.CheckBox chkPassword;
        private System.Windows.Forms.CheckBox chkAreaTilePhysics;
        private System.Windows.Forms.CheckBox chkAreaAsm;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel lnkUnselectAll;
        private System.Windows.Forms.LinkLabel lnkSelectAll;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}