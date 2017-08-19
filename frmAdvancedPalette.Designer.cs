namespace Editroid
{
    partial class frmAdvancedPalette
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdvancedPalette));
            this.LevelLabel = new System.Windows.Forms.Label();
            this.PaletteList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPointer = new Editroid.NumericTextBox();
            this.txtMacroBytes = new Editroid.NumericTextBox();
            this.txtPpuPointer = new Editroid.NumericTextBox();
            this.PaletteControl = new Editroid.Controls.AdvancedPalControl();
            this.colorPicker = new Editroid.ColorSelector();
            this.SuspendLayout();
            // 
            // LevelLabel
            // 
            this.LevelLabel.AutoSize = true;
            this.LevelLabel.Location = new System.Drawing.Point(12, 9);
            this.LevelLabel.Name = "LevelLabel";
            this.LevelLabel.Size = new System.Drawing.Size(106, 13);
            this.LevelLabel.TabIndex = 0;
            this.LevelLabel.Tag = "Showing palettes for: *";
            this.LevelLabel.Text = "Showing palettes for:";
            // 
            // PaletteList
            // 
            this.PaletteList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PaletteList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader6});
            this.PaletteList.FullRowSelect = true;
            this.PaletteList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PaletteList.Location = new System.Drawing.Point(15, 25);
            this.PaletteList.MultiSelect = false;
            this.PaletteList.Name = "PaletteList";
            this.PaletteList.Size = new System.Drawing.Size(673, 206);
            this.PaletteList.TabIndex = 1;
            this.PaletteList.UseCompatibleStateImageBehavior = false;
            this.PaletteList.View = System.Windows.Forms.View.Details;
            this.PaletteList.SelectedIndexChanged += new System.EventHandler(this.PaletteList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "Index";
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 58;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Offset";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Ppu Address";
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Size";
            this.columnHeader3.Width = 46;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 5;
            this.columnHeader4.Text = "Data";
            this.columnHeader4.Width = 293;
            // 
            // columnHeader6
            // 
            this.columnHeader6.DisplayIndex = 4;
            this.columnHeader6.Text = "Desc.";
            this.columnHeader6.Width = 132;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(497, 237);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "PPU Address:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(587, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Byte Count:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(407, 237);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Pointer:";
            // 
            // txtPointer
            // 
            this.txtPointer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPointer.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPointer.Hexadecimal = true;
            this.txtPointer.Location = new System.Drawing.Point(407, 253);
            this.txtPointer.Maximum = 65535;
            this.txtPointer.Name = "txtPointer";
            this.txtPointer.Size = new System.Drawing.Size(84, 20);
            this.txtPointer.TabIndex = 18;
            this.txtPointer.Text = "FFFF";
            this.txtPointer.Value = 65535;
            this.txtPointer.TextChanged += new System.EventHandler(this.txtPointer_TextChanged);
            // 
            // txtMacroBytes
            // 
            this.txtMacroBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMacroBytes.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMacroBytes.Location = new System.Drawing.Point(587, 253);
            this.txtMacroBytes.Maximum = 255;
            this.txtMacroBytes.Name = "txtMacroBytes";
            this.txtMacroBytes.Size = new System.Drawing.Size(83, 20);
            this.txtMacroBytes.TabIndex = 16;
            this.txtMacroBytes.Text = "0";
            this.txtMacroBytes.Value = 0;
            this.txtMacroBytes.TextChanged += new System.EventHandler(this.txtMacroBytes_TextChanged);
            // 
            // txtPpuPointer
            // 
            this.txtPpuPointer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPpuPointer.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPpuPointer.Hexadecimal = true;
            this.txtPpuPointer.Location = new System.Drawing.Point(497, 253);
            this.txtPpuPointer.Maximum = 65535;
            this.txtPpuPointer.Name = "txtPpuPointer";
            this.txtPpuPointer.Size = new System.Drawing.Size(84, 20);
            this.txtPpuPointer.TabIndex = 14;
            this.txtPpuPointer.Text = "FFFF";
            this.txtPpuPointer.Value = 65535;
            this.txtPpuPointer.TextChanged += new System.EventHandler(this.txtPpuPointer_TextChanged);
            // 
            // PaletteControl
            // 
            this.PaletteControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PaletteControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PaletteControl.Location = new System.Drawing.Point(12, 307);
            this.PaletteControl.Name = "PaletteControl";
            this.PaletteControl.Size = new System.Drawing.Size(673, 16);
            this.PaletteControl.TabIndex = 12;
            // 
            // colorPicker
            // 
            this.colorPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.colorPicker.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("colorPicker.BackgroundImage")));
            this.colorPicker.Location = new System.Drawing.Point(15, 237);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Selection = 0;
            this.colorPicker.Size = new System.Drawing.Size(256, 64);
            this.colorPicker.TabIndex = 11;
            this.colorPicker.Text = "colorSelector1";
            // 
            // frmAdvancedPalette
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(700, 335);
            this.Controls.Add(this.txtPointer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMacroBytes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPpuPointer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PaletteControl);
            this.Controls.Add(this.colorPicker);
            this.Controls.Add(this.PaletteList);
            this.Controls.Add(this.LevelLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAdvancedPalette";
            this.Text = "Advanced Palette Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LevelLabel;
        private System.Windows.Forms.ListView PaletteList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private ColorSelector colorPicker;
        private Editroid.Controls.AdvancedPalControl PaletteControl;
        private System.Windows.Forms.Label label2;
        private NumericTextBox txtPpuPointer;
        private NumericTextBox txtMacroBytes;
        private System.Windows.Forms.Label label3;
        private NumericTextBox txtPointer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}