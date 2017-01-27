namespace Editroid
{
    partial class TileSourceEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileSourceEditor));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LenBox = new Editroid.NumericTextBox();
            this.DestBox = new Editroid.NumericTextBox();
            this.TypeBox = new System.Windows.Forms.ComboBox();
            this.SourceBox = new Editroid.NumericTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PpuAddressLabel = new System.Windows.Forms.Label();
            this.PatternGroupsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnGlobal = new System.Windows.Forms.ToolStripButton();
            this.btnBrinstar = new System.Windows.Forms.ToolStripButton();
            this.btnNorfair = new System.Windows.Forms.ToolStripButton();
            this.btnRidley = new System.Windows.Forms.ToolStripButton();
            this.btnKraid = new System.Windows.Forms.ToolStripButton();
            this.btnTourian = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SpritePatternBox = new System.Windows.Forms.PictureBox();
            this.BackgroundPatternBox = new System.Windows.Forms.PictureBox();
            this.PatternGroupIndexTableList = new System.Windows.Forms.ListBox();
            this.numericTextBox3 = new Editroid.NumericTextBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SpritePatternBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPatternBox)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.PatternGroupsList);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(580, 243);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pattern Groups";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.LenBox);
            this.panel1.Controls.Add(this.DestBox);
            this.panel1.Controls.Add(this.TypeBox);
            this.panel1.Controls.Add(this.SourceBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.PpuAddressLabel);
            this.panel1.Location = new System.Drawing.Point(6, 195);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(569, 42);
            this.panel1.TabIndex = 15;
            // 
            // LenBox
            // 
            this.LenBox.Hexadecimal = true;
            this.LenBox.Location = new System.Drawing.Point(221, 14);
            this.LenBox.Maximum = 255;
            this.LenBox.Name = "LenBox";
            this.LenBox.Size = new System.Drawing.Size(86, 20);
            this.LenBox.TabIndex = 14;
            this.LenBox.Text = "0";
            this.LenBox.Value = 0;
            this.LenBox.Leave += new System.EventHandler(this.LenBox_Leave);
            // 
            // DestBox
            // 
            this.DestBox.Hexadecimal = true;
            this.DestBox.Location = new System.Drawing.Point(129, 14);
            this.DestBox.Maximum = 255;
            this.DestBox.Name = "DestBox";
            this.DestBox.Size = new System.Drawing.Size(86, 20);
            this.DestBox.TabIndex = 12;
            this.DestBox.Text = "0";
            this.DestBox.Value = 0;
            this.DestBox.Leave += new System.EventHandler(this.DestBox_Leave);
            // 
            // TypeBox
            // 
            this.TypeBox.FormattingEnabled = true;
            this.TypeBox.IntegralHeight = false;
            this.TypeBox.Items.AddRange(new object[] {
            "Sprite",
            "Background"});
            this.TypeBox.Location = new System.Drawing.Point(313, 14);
            this.TypeBox.Name = "TypeBox";
            this.TypeBox.Size = new System.Drawing.Size(103, 24);
            this.TypeBox.TabIndex = 9;
            this.TypeBox.SelectedIndexChanged += new System.EventHandler(this.TypeBox_SelectedIndexChanged);
            // 
            // SourceBox
            // 
            this.SourceBox.Hexadecimal = true;
            this.SourceBox.Location = new System.Drawing.Point(3, 14);
            this.SourceBox.Maximum = 131072;
            this.SourceBox.Name = "SourceBox";
            this.SourceBox.Size = new System.Drawing.Size(120, 20);
            this.SourceBox.TabIndex = 11;
            this.SourceBox.Text = "0";
            this.SourceBox.Value = 0;
            this.SourceBox.TextChanged += new System.EventHandler(this.SourceBox_TextChanged);
            this.SourceBox.Leave += new System.EventHandler(this.SourceBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Source ROM Offset:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(126, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "PPU Tile Index:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(218, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Tile Count:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(310, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Pattern Table:";
            // 
            // PpuAddressLabel
            // 
            this.PpuAddressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PpuAddressLabel.Location = new System.Drawing.Point(422, 0);
            this.PpuAddressLabel.Name = "PpuAddressLabel";
            this.PpuAddressLabel.Size = new System.Drawing.Size(149, 34);
            this.PpuAddressLabel.TabIndex = 10;
            this.PpuAddressLabel.Text = "PPU Address:\r\n0x0000";
            // 
            // PatternGroupsList
            // 
            this.PatternGroupsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PatternGroupsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader5,
            this.columnHeader4});
            this.PatternGroupsList.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PatternGroupsList.FullRowSelect = true;
            this.PatternGroupsList.HideSelection = false;
            this.PatternGroupsList.Location = new System.Drawing.Point(6, 19);
            this.PatternGroupsList.MultiSelect = false;
            this.PatternGroupsList.Name = "PatternGroupsList";
            this.PatternGroupsList.Size = new System.Drawing.Size(569, 173);
            this.PatternGroupsList.TabIndex = 0;
            this.PatternGroupsList.UseCompatibleStateImageBehavior = false;
            this.PatternGroupsList.View = System.Windows.Forms.View.Details;
            this.PatternGroupsList.SelectedIndexChanged += new System.EventHandler(this.PatternGroupsList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Index";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Source Offset";
            this.columnHeader2.Width = 94;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "PPU Tile Index";
            this.columnHeader3.Width = 97;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Tile Count";
            this.columnHeader5.Width = 74;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Pattern Table";
            this.columnHeader4.Width = 110;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnGlobal,
            this.btnBrinstar,
            this.btnNorfair,
            this.btnRidley,
            this.btnKraid,
            this.btnTourian});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(9, 23);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(256, 23);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnGlobal
            // 
            this.btnGlobal.AutoSize = false;
            this.btnGlobal.Checked = true;
            this.btnGlobal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnGlobal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGlobal.Image = ((System.Drawing.Image)(resources.GetObject("btnGlobal.Image")));
            this.btnGlobal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGlobal.Name = "btnGlobal";
            this.btnGlobal.Size = new System.Drawing.Size(41, 20);
            this.btnGlobal.Text = "Global";
            this.btnGlobal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGlobal.Click += new System.EventHandler(this.btnGlobal_Click);
            // 
            // btnBrinstar
            // 
            this.btnBrinstar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBrinstar.Image = ((System.Drawing.Image)(resources.GetObject("btnBrinstar.Image")));
            this.btnBrinstar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBrinstar.Name = "btnBrinstar";
            this.btnBrinstar.Size = new System.Drawing.Size(48, 17);
            this.btnBrinstar.Text = "Brinstar";
            this.btnBrinstar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrinstar.Click += new System.EventHandler(this.btnBrinstar_Click);
            // 
            // btnNorfair
            // 
            this.btnNorfair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNorfair.Image = ((System.Drawing.Image)(resources.GetObject("btnNorfair.Image")));
            this.btnNorfair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNorfair.Name = "btnNorfair";
            this.btnNorfair.Size = new System.Drawing.Size(44, 17);
            this.btnNorfair.Text = "Norfair";
            this.btnNorfair.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNorfair.Click += new System.EventHandler(this.btnNorfair_Click);
            // 
            // btnRidley
            // 
            this.btnRidley.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRidley.Image = ((System.Drawing.Image)(resources.GetObject("btnRidley.Image")));
            this.btnRidley.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRidley.Name = "btnRidley";
            this.btnRidley.Size = new System.Drawing.Size(40, 17);
            this.btnRidley.Text = "Ridley";
            this.btnRidley.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRidley.Click += new System.EventHandler(this.btnRidley_Click);
            // 
            // btnKraid
            // 
            this.btnKraid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnKraid.Image = ((System.Drawing.Image)(resources.GetObject("btnKraid.Image")));
            this.btnKraid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnKraid.Name = "btnKraid";
            this.btnKraid.Size = new System.Drawing.Size(35, 17);
            this.btnKraid.Text = "Kraid";
            this.btnKraid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnKraid.Click += new System.EventHandler(this.btnKraid_Click);
            // 
            // btnTourian
            // 
            this.btnTourian.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTourian.Image = ((System.Drawing.Image)(resources.GetObject("btnTourian.Image")));
            this.btnTourian.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTourian.Name = "btnTourian";
            this.btnTourian.Size = new System.Drawing.Size(47, 17);
            this.btnTourian.Text = "Tourian";
            this.btnTourian.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTourian.Click += new System.EventHandler(this.btnTourian_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.SpritePatternBox);
            this.groupBox2.Controls.Add(this.BackgroundPatternBox);
            this.groupBox2.Controls.Add(this.PatternGroupIndexTableList);
            this.groupBox2.Controls.Add(this.toolStrip1);
            this.groupBox2.Controls.Add(this.numericTextBox3);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(12, 261);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(580, 186);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Level Pattern Tables";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(443, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Background";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(309, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Sprites";
            // 
            // SpritePatternBox
            // 
            this.SpritePatternBox.BackColor = System.Drawing.Color.Black;
            this.SpritePatternBox.Location = new System.Drawing.Point(312, 46);
            this.SpritePatternBox.Name = "SpritePatternBox";
            this.SpritePatternBox.Size = new System.Drawing.Size(128, 128);
            this.SpritePatternBox.TabIndex = 2;
            this.SpritePatternBox.TabStop = false;
            // 
            // BackgroundPatternBox
            // 
            this.BackgroundPatternBox.BackColor = System.Drawing.Color.Black;
            this.BackgroundPatternBox.Location = new System.Drawing.Point(446, 46);
            this.BackgroundPatternBox.Name = "BackgroundPatternBox";
            this.BackgroundPatternBox.Size = new System.Drawing.Size(128, 128);
            this.BackgroundPatternBox.TabIndex = 3;
            this.BackgroundPatternBox.TabStop = false;
            // 
            // PatternGroupIndexTableList
            // 
            this.PatternGroupIndexTableList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PatternGroupIndexTableList.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PatternGroupIndexTableList.FormattingEnabled = true;
            this.PatternGroupIndexTableList.IntegralHeight = false;
            this.PatternGroupIndexTableList.ItemHeight = 16;
            this.PatternGroupIndexTableList.Location = new System.Drawing.Point(9, 46);
            this.PatternGroupIndexTableList.Name = "PatternGroupIndexTableList";
            this.PatternGroupIndexTableList.Size = new System.Drawing.Size(290, 128);
            this.PatternGroupIndexTableList.TabIndex = 4;
            this.PatternGroupIndexTableList.SelectedIndexChanged += new System.EventHandler(this.LevelGroupsList_SelectedIndexChanged);
            // 
            // numericTextBox3
            // 
            this.numericTextBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericTextBox3.Hexadecimal = true;
            this.numericTextBox3.Location = new System.Drawing.Point(106, 145);
            this.numericTextBox3.Maximum = 29;
            this.numericTextBox3.Name = "numericTextBox3";
            this.numericTextBox3.Size = new System.Drawing.Size(290, 20);
            this.numericTextBox3.TabIndex = 12;
            this.numericTextBox3.Text = "0";
            this.numericTextBox3.Value = 0;
            this.numericTextBox3.Visible = false;
            // 
            // TileSourceEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(604, 457);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TileSourceEditor";
            this.Text = "Pattern Source Editor";
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SpritePatternBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPatternBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView PatternGroupsList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ComboBox TypeBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private NumericTextBox DestBox;
        private NumericTextBox SourceBox;
        private System.Windows.Forms.Label PpuAddressLabel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnGlobal;
        private System.Windows.Forms.ToolStripButton btnBrinstar;
        private System.Windows.Forms.ToolStripButton btnNorfair;
        private System.Windows.Forms.ToolStripButton btnRidley;
        private System.Windows.Forms.ToolStripButton btnKraid;
        private System.Windows.Forms.ToolStripButton btnTourian;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox SpritePatternBox;
        private NumericTextBox numericTextBox3;
        private System.Windows.Forms.ListBox PatternGroupIndexTableList;
        private System.Windows.Forms.PictureBox BackgroundPatternBox;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private NumericTextBox LenBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
    }
}