namespace Editroid
{
    partial class ItemEditor
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemEditor));
            this.LevelBar = new System.Windows.Forms.ToolStrip();
            this.btnBrinstar = new System.Windows.Forms.ToolStripButton();
            this.btnNorfair = new System.Windows.Forms.ToolStripButton();
            this.btnRidley = new System.Windows.Forms.ToolStripButton();
            this.btnKraid = new System.Windows.Forms.ToolStripButton();
            this.btnTourain = new System.Windows.Forms.ToolStripButton();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.lstSubtypes = new System.Windows.Forms.ListBox();
            this.chkEasy = new System.Windows.Forms.CheckBox();
            this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.pnlScreen = new System.Windows.Forms.Panel();
            this.picScreenLocation = new System.Windows.Forms.PictureBox();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.pnlEnemyEditor = new System.Windows.Forms.Panel();
            this.chkHard = new System.Windows.Forms.CheckBox();
            this.nudSpriteSlot = new System.Windows.Forms.NumericUpDown();
            this.nudEnemyType = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.LevelBar.SuspendLayout();
            this.pnlScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picScreenLocation)).BeginInit();
            this.pnlEnemyEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpriteSlot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnemyType)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(41, 2);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 13);
            label1.TabIndex = 1;
            label1.Text = "Enemy Type";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(42, 28);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(55, 13);
            label2.TabIndex = 3;
            label2.Text = "Sprite Slot";
            // 
            // LevelBar
            // 
            this.LevelBar.AutoSize = false;
            this.LevelBar.CanOverflow = false;
            this.LevelBar.Dock = System.Windows.Forms.DockStyle.None;
            this.LevelBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.LevelBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBrinstar,
            this.btnNorfair,
            this.btnRidley,
            this.btnKraid,
            this.btnTourain});
            this.LevelBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.LevelBar.Location = new System.Drawing.Point(277, 9);
            this.LevelBar.Name = "LevelBar";
            this.LevelBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.LevelBar.ShowItemToolTips = false;
            this.LevelBar.Size = new System.Drawing.Size(118, 118);
            this.LevelBar.TabIndex = 1;
            this.LevelBar.Text = "LevelSelector";
            // 
            // btnBrinstar
            // 
            this.btnBrinstar.AutoToolTip = false;
            this.btnBrinstar.Checked = true;
            this.btnBrinstar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnBrinstar.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrinstar.Image = global::Editroid.Properties.Resources.Missile;
            this.btnBrinstar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBrinstar.Name = "btnBrinstar";
            this.btnBrinstar.Size = new System.Drawing.Size(70, 20);
            this.btnBrinstar.Text = "Brinstar";
            this.btnBrinstar.Click += new System.EventHandler(this.LevelButton_Click);
            // 
            // btnNorfair
            // 
            this.btnNorfair.AutoToolTip = false;
            this.btnNorfair.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNorfair.Image = global::Editroid.Properties.Resources.Missile;
            this.btnNorfair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNorfair.Name = "btnNorfair";
            this.btnNorfair.Size = new System.Drawing.Size(65, 20);
            this.btnNorfair.Text = "Norfair";
            this.btnNorfair.Click += new System.EventHandler(this.LevelButton_Click);
            // 
            // btnRidley
            // 
            this.btnRidley.AutoToolTip = false;
            this.btnRidley.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRidley.Image = global::Editroid.Properties.Resources.Missile;
            this.btnRidley.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRidley.Name = "btnRidley";
            this.btnRidley.Size = new System.Drawing.Size(62, 20);
            this.btnRidley.Text = "Ridley";
            this.btnRidley.Click += new System.EventHandler(this.LevelButton_Click);
            // 
            // btnKraid
            // 
            this.btnKraid.AutoToolTip = false;
            this.btnKraid.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKraid.Image = global::Editroid.Properties.Resources.Missile;
            this.btnKraid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnKraid.Name = "btnKraid";
            this.btnKraid.Size = new System.Drawing.Size(56, 20);
            this.btnKraid.Text = "Kraid";
            this.btnKraid.Click += new System.EventHandler(this.LevelButton_Click);
            // 
            // btnTourain
            // 
            this.btnTourain.AutoToolTip = false;
            this.btnTourain.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTourain.Image = global::Editroid.Properties.Resources.Missile;
            this.btnTourain.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTourain.Name = "btnTourain";
            this.btnTourain.Size = new System.Drawing.Size(70, 20);
            this.btnTourain.Text = "Tourain";
            this.btnTourain.Click += new System.EventHandler(this.LevelButton_Click);
            // 
            // pnlMap
            // 
            this.pnlMap.Location = new System.Drawing.Point(12, 12);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(256, 256);
            this.pnlMap.TabIndex = 2;
            // 
            // lstSubtypes
            // 
            this.lstSubtypes.Enabled = false;
            this.lstSubtypes.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstSubtypes.FormattingEnabled = true;
            this.lstSubtypes.IntegralHeight = false;
            this.lstSubtypes.Location = new System.Drawing.Point(274, 330);
            this.lstSubtypes.Name = "lstSubtypes";
            this.lstSubtypes.Size = new System.Drawing.Size(118, 184);
            this.lstSubtypes.TabIndex = 3;
            this.lstSubtypes.SelectedIndexChanged += new System.EventHandler(this.lstSubtypes_SelectedIndexChanged);
            // 
            // chkEasy
            // 
            this.chkEasy.Checked = true;
            this.chkEasy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEasy.Location = new System.Drawing.Point(274, 232);
            this.chkEasy.Name = "chkEasy";
            this.chkEasy.Size = new System.Drawing.Size(121, 36);
            this.chkEasy.TabIndex = 4;
            this.chkEasy.Text = "Update password data";
            this.chkEasy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTips.SetToolTip(this.chkEasy, "Automatically update the password\r\nto reflect moved/changed items.");
            this.chkEasy.UseVisualStyleBackColor = true;
            // 
            // pnlScreen
            // 
            this.pnlScreen.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlScreen.Controls.Add(this.picScreenLocation);
            this.pnlScreen.Location = new System.Drawing.Point(12, 274);
            this.pnlScreen.Name = "pnlScreen";
            this.pnlScreen.Size = new System.Drawing.Size(256, 240);
            this.pnlScreen.TabIndex = 0;
            // 
            // picScreenLocation
            // 
            this.picScreenLocation.BackColor = System.Drawing.Color.Transparent;
            this.picScreenLocation.Image = global::Editroid.Properties.Resources.Missile;
            this.picScreenLocation.Location = new System.Drawing.Point(0, 0);
            this.picScreenLocation.Name = "picScreenLocation";
            this.picScreenLocation.Size = new System.Drawing.Size(16, 16);
            this.picScreenLocation.TabIndex = 5;
            this.picScreenLocation.TabStop = false;
            this.picScreenLocation.Visible = false;
            this.picScreenLocation.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picScreenLocation_MouseMove);
            this.picScreenLocation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picScreenLocation_MouseDown);
            this.picScreenLocation.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picScreenLocation_MouseUp);
            // 
            // lstItems
            // 
            this.lstItems.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstItems.FormattingEnabled = true;
            this.lstItems.IntegralHeight = false;
            this.lstItems.Location = new System.Drawing.Point(274, 274);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(118, 50);
            this.lstItems.TabIndex = 5;
            this.lstItems.SelectedIndexChanged += new System.EventHandler(this.lstItems_SelectedIndexChanged);
            // 
            // pnlEnemyEditor
            // 
            this.pnlEnemyEditor.Controls.Add(this.chkHard);
            this.pnlEnemyEditor.Controls.Add(label2);
            this.pnlEnemyEditor.Controls.Add(this.nudSpriteSlot);
            this.pnlEnemyEditor.Controls.Add(label1);
            this.pnlEnemyEditor.Controls.Add(this.nudEnemyType);
            this.pnlEnemyEditor.Enabled = false;
            this.pnlEnemyEditor.Location = new System.Drawing.Point(348, 213);
            this.pnlEnemyEditor.Name = "pnlEnemyEditor";
            this.pnlEnemyEditor.Size = new System.Drawing.Size(118, 184);
            this.pnlEnemyEditor.TabIndex = 6;
            this.pnlEnemyEditor.Visible = false;
            // 
            // chkHard
            // 
            this.chkHard.AutoSize = true;
            this.chkHard.Location = new System.Drawing.Point(0, 52);
            this.chkHard.Name = "chkHard";
            this.chkHard.Size = new System.Drawing.Size(61, 17);
            this.chkHard.TabIndex = 7;
            this.chkHard.Text = "Difficult";
            this.chkHard.UseVisualStyleBackColor = true;
            this.chkHard.CheckedChanged += new System.EventHandler(this.chkHard_CheckedChanged);
            // 
            // nudSpriteSlot
            // 
            this.nudSpriteSlot.Hexadecimal = true;
            this.nudSpriteSlot.Location = new System.Drawing.Point(0, 26);
            this.nudSpriteSlot.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudSpriteSlot.Name = "nudSpriteSlot";
            this.nudSpriteSlot.Size = new System.Drawing.Size(36, 20);
            this.nudSpriteSlot.TabIndex = 2;
            this.nudSpriteSlot.ValueChanged += new System.EventHandler(this.nudSpriteSlot_ValueChanged);
            // 
            // nudEnemyType
            // 
            this.nudEnemyType.Hexadecimal = true;
            this.nudEnemyType.Location = new System.Drawing.Point(-1, 0);
            this.nudEnemyType.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudEnemyType.Name = "nudEnemyType";
            this.nudEnemyType.Size = new System.Drawing.Size(36, 20);
            this.nudEnemyType.TabIndex = 0;
            this.nudEnemyType.ValueChanged += new System.EventHandler(this.nudEnemyType_ValueChanged);
            // 
            // ItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 526);
            this.Controls.Add(this.pnlEnemyEditor);
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.LevelBar);
            this.Controls.Add(this.pnlScreen);
            this.Controls.Add(this.chkEasy);
            this.Controls.Add(this.lstSubtypes);
            this.Controls.Add(this.pnlMap);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ItemEditor";
            this.Text = "Item Editor";
            this.LevelBar.ResumeLayout(false);
            this.LevelBar.PerformLayout();
            this.pnlScreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picScreenLocation)).EndInit();
            this.pnlEnemyEditor.ResumeLayout(false);
            this.pnlEnemyEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpriteSlot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnemyType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip LevelBar;
        private System.Windows.Forms.ToolStripButton btnBrinstar;
        private System.Windows.Forms.ToolStripButton btnNorfair;
        private System.Windows.Forms.ToolStripButton btnRidley;
        private System.Windows.Forms.ToolStripButton btnKraid;
        private System.Windows.Forms.ToolStripButton btnTourain;
        private System.Windows.Forms.Panel pnlMap;
        private System.Windows.Forms.ListBox lstSubtypes;
        private System.Windows.Forms.CheckBox chkEasy;
        private System.Windows.Forms.ToolTip ToolTips;
        private System.Windows.Forms.Panel pnlScreen;
        private System.Windows.Forms.PictureBox picScreenLocation;
        private System.Windows.Forms.ListBox lstItems;
        private System.Windows.Forms.Panel pnlEnemyEditor;
        private System.Windows.Forms.NumericUpDown nudEnemyType;
        private System.Windows.Forms.CheckBox chkHard;
        private System.Windows.Forms.NumericUpDown nudSpriteSlot;
    }
}