namespace Editroid
{
	partial class frmPalette
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
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPalette));
            this.lblSpritePal2 = new System.Windows.Forms.Label();
            this.lblBg2 = new System.Windows.Forms.Label();
            this.spritePal = new Editroid.LevelPaletteEditor();
            this.bgPal2 = new Editroid.LevelPaletteEditor();
            this.bgPal = new Editroid.LevelPaletteEditor();
            this.colorPicker = new Editroid.ColorSelector();
            this.spritePal2 = new Editroid.LevelPaletteEditor();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(94, 15);
            label1.TabIndex = 3;
            label1.Text = "Background";
            label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label3.Location = new System.Drawing.Point(212, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(94, 15);
            label3.TabIndex = 5;
            label3.Text = "Sprites";
            label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            label4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            label4.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label4.Location = new System.Drawing.Point(84, 106);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(256, 15);
            label4.TabIndex = 8;
            label4.Text = "Colors";
            label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lblSpritePal2
            // 
            this.lblSpritePal2.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpritePal2.Location = new System.Drawing.Point(312, 9);
            this.lblSpritePal2.Name = "lblSpritePal2";
            this.lblSpritePal2.Size = new System.Drawing.Size(94, 15);
            this.lblSpritePal2.TabIndex = 12;
            this.lblSpritePal2.Text = "Sprites 2";
            this.lblSpritePal2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lblBg2
            // 
            this.lblBg2.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBg2.Location = new System.Drawing.Point(112, 9);
            this.lblBg2.Name = "lblBg2";
            this.lblBg2.Size = new System.Drawing.Size(94, 15);
            this.lblBg2.TabIndex = 4;
            this.lblBg2.Text = "Background 2";
            this.lblBg2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // spritePal
            // 
            this.spritePal.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.spritePal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.spritePal.LevelData = null;
            this.spritePal.Location = new System.Drawing.Point(225, 27);
            this.spritePal.Name = "spritePal";
            this.spritePal.SelectedColor = 0;
            this.spritePal.Size = new System.Drawing.Size(64, 64);
            this.spritePal.TabIndex = 11;
            // 
            // bgPal2
            // 
            this.bgPal2.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.bgPal2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bgPal2.LevelData = null;
            this.bgPal2.Location = new System.Drawing.Point(125, 27);
            this.bgPal2.Name = "bgPal2";
            this.bgPal2.SelectedColor = 0;
            this.bgPal2.Size = new System.Drawing.Size(64, 64);
            this.bgPal2.TabIndex = 11;
            // 
            // bgPal
            // 
            this.bgPal.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.bgPal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bgPal.LevelData = null;
            this.bgPal.Location = new System.Drawing.Point(24, 27);
            this.bgPal.Name = "bgPal";
            this.bgPal.PalType = Editroid.PaletteType.Background;
            this.bgPal.SelectedColor = 0;
            this.bgPal.Size = new System.Drawing.Size(64, 64);
            this.bgPal.TabIndex = 10;
            // 
            // colorPicker
            // 
            this.colorPicker.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.colorPicker.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("colorPicker.BackgroundImage")));
            this.colorPicker.Location = new System.Drawing.Point(84, 121);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Selection = 0;
            this.colorPicker.Size = new System.Drawing.Size(256, 64);
            this.colorPicker.TabIndex = 9;
            this.colorPicker.Text = "colorSelector1";
            this.colorPicker.ColorSelected += new System.EventHandler(this.colorSelector1_ColorSelected);
            // 
            // spritePal2
            // 
            this.spritePal2.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.spritePal2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.spritePal2.LevelData = null;
            this.spritePal2.Location = new System.Drawing.Point(325, 27);
            this.spritePal2.Name = "spritePal2";
            this.spritePal2.PalType = Editroid.PaletteType.Background;
            this.spritePal2.SelectedColor = 0;
            this.spritePal2.Size = new System.Drawing.Size(64, 64);
            this.spritePal2.TabIndex = 13;
            // 
            // frmPalette
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(422, 196);
            this.Controls.Add(this.spritePal2);
            this.Controls.Add(this.lblSpritePal2);
            this.Controls.Add(this.spritePal);
            this.Controls.Add(this.bgPal2);
            this.Controls.Add(this.bgPal);
            this.Controls.Add(this.colorPicker);
            this.Controls.Add(label4);
            this.Controls.Add(label3);
            this.Controls.Add(this.lblBg2);
            this.Controls.Add(label1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPalette";
            this.ShowInTaskbar = false;
            this.Text = "Palettes";
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Label lblBg2;
        private ColorSelector colorPicker;
        private LevelPaletteEditor bgPal;
        private LevelPaletteEditor bgPal2;
        private LevelPaletteEditor spritePal;
        private LevelPaletteEditor spritePal2;
        private System.Windows.Forms.Label lblSpritePal2;
	}
}