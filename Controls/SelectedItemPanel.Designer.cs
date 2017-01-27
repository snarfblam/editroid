namespace Editroid
{
    partial class SelectedItemPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectedItemPanel));
            this.IndexLabel = new System.Windows.Forms.Label();
            this.PaletteLabel = new System.Windows.Forms.Label();
            this.LayoutLabel = new System.Windows.Forms.Label();
            this.FreespaceLabel = new System.Windows.Forms.Label();
            this.SlotImage = new System.Windows.Forms.PictureBox();
            this.SlotLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.PasswordDataToggle = new Editroid.ToggleBox();
            this.musicButton = new Editroid.ToggleBox();
            this.lblDefaultPal = new System.Windows.Forms.Label();
            this.lblMapLocation = new System.Windows.Forms.Label();
            this.DefaultPalClicker = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SlotImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordDataToggle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicButton)).BeginInit();
            this.SuspendLayout();
            // 
            // IndexLabel
            // 
            this.IndexLabel.AutoSize = true;
            this.IndexLabel.ForeColor = System.Drawing.Color.White;
            this.IndexLabel.Location = new System.Drawing.Point(20, 5);
            this.IndexLabel.Name = "IndexLabel";
            this.IndexLabel.Size = new System.Drawing.Size(19, 13);
            this.IndexLabel.TabIndex = 0;
            this.IndexLabel.Text = "00";
            // 
            // PaletteLabel
            // 
            this.PaletteLabel.AutoSize = true;
            this.PaletteLabel.ForeColor = System.Drawing.Color.White;
            this.PaletteLabel.Location = new System.Drawing.Point(20, 21);
            this.PaletteLabel.Name = "PaletteLabel";
            this.PaletteLabel.Size = new System.Drawing.Size(19, 13);
            this.PaletteLabel.TabIndex = 1;
            this.PaletteLabel.Text = "00";
            // 
            // LayoutLabel
            // 
            this.LayoutLabel.AutoSize = true;
            this.LayoutLabel.ForeColor = System.Drawing.Color.White;
            this.LayoutLabel.Location = new System.Drawing.Point(133, 5);
            this.LayoutLabel.Name = "LayoutLabel";
            this.LayoutLabel.Size = new System.Drawing.Size(19, 13);
            this.LayoutLabel.TabIndex = 2;
            this.LayoutLabel.Text = "00";
            // 
            // FreespaceLabel
            // 
            this.FreespaceLabel.ForeColor = System.Drawing.Color.White;
            this.FreespaceLabel.Location = new System.Drawing.Point(180, 5);
            this.FreespaceLabel.Name = "FreespaceLabel";
            this.FreespaceLabel.Size = new System.Drawing.Size(62, 13);
            this.FreespaceLabel.TabIndex = 3;
            this.FreespaceLabel.Text = "00";
            this.FreespaceLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SlotImage
            // 
            this.SlotImage.Image = ((System.Drawing.Image)(resources.GetObject("SlotImage.Image")));
            this.SlotImage.Location = new System.Drawing.Point(42, 20);
            this.SlotImage.Name = "SlotImage";
            this.SlotImage.Size = new System.Drawing.Size(16, 16);
            this.SlotImage.TabIndex = 4;
            this.SlotImage.TabStop = false;
            // 
            // SlotLabel
            // 
            this.SlotLabel.AutoSize = true;
            this.SlotLabel.ForeColor = System.Drawing.Color.White;
            this.SlotLabel.Location = new System.Drawing.Point(57, 21);
            this.SlotLabel.Name = "SlotLabel";
            this.SlotLabel.Size = new System.Drawing.Size(19, 13);
            this.SlotLabel.TabIndex = 5;
            this.SlotLabel.Text = "00";
            // 
            // PasswordDataToggle
            // 
            this.PasswordDataToggle.Image = null;
            this.PasswordDataToggle.Location = new System.Drawing.Point(97, 20);
            this.PasswordDataToggle.Name = "PasswordDataToggle";
            this.PasswordDataToggle.OffImage = null;
            this.PasswordDataToggle.OnImage = ((System.Drawing.Image)(resources.GetObject("PasswordDataToggle.OnImage")));
            this.PasswordDataToggle.Size = new System.Drawing.Size(16, 16);
            this.PasswordDataToggle.TabIndex = 7;
            this.PasswordDataToggle.TabStop = false;
            this.PasswordDataToggle.ToggleOnClick = false;
            this.toolTip1.SetToolTip(this.PasswordDataToggle, "Password Data");
            this.PasswordDataToggle.Click += new System.EventHandler(this.PasswordDataToggle_Click);
            // 
            // musicButton
            // 
            this.musicButton.Image = null;
            this.musicButton.Location = new System.Drawing.Point(97, 4);
            this.musicButton.Name = "musicButton";
            this.musicButton.OffImage = ((System.Drawing.Image)(resources.GetObject("musicButton.OffImage")));
            this.musicButton.OnImage = ((System.Drawing.Image)(resources.GetObject("musicButton.OnImage")));
            this.musicButton.Size = new System.Drawing.Size(16, 16);
            this.musicButton.TabIndex = 6;
            this.musicButton.TabStop = false;
            this.musicButton.ToggleOnClick = true;
            this.toolTip1.SetToolTip(this.musicButton, "Play Creepy Music");
            this.musicButton.TryToggle += new System.ComponentModel.CancelEventHandler(this.musicButton_TryClick);
            this.musicButton.Toggled += new System.EventHandler(this.musicButton_Toggled);
            // 
            // lblDefaultPal
            // 
            this.lblDefaultPal.AutoSize = true;
            this.lblDefaultPal.ForeColor = System.Drawing.Color.White;
            this.lblDefaultPal.Location = new System.Drawing.Point(133, 21);
            this.lblDefaultPal.Name = "lblDefaultPal";
            this.lblDefaultPal.Size = new System.Drawing.Size(19, 13);
            this.lblDefaultPal.TabIndex = 8;
            this.lblDefaultPal.Text = "00";
            this.lblDefaultPal.DoubleClick += new System.EventHandler(this.DefaultPalClicker_Click);
            this.lblDefaultPal.Click += new System.EventHandler(this.DefaultPalClicker_Click);
            // 
            // lblMapLocation
            // 
            this.lblMapLocation.ForeColor = System.Drawing.Color.White;
            this.lblMapLocation.Location = new System.Drawing.Point(177, 22);
            this.lblMapLocation.Name = "lblMapLocation";
            this.lblMapLocation.Size = new System.Drawing.Size(78, 16);
            this.lblMapLocation.TabIndex = 9;
            this.lblMapLocation.Text = "1a, 1a";
            this.lblMapLocation.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // DefaultPalClicker
            // 
            this.DefaultPalClicker.Location = new System.Drawing.Point(115, 19);
            this.DefaultPalClicker.Name = "DefaultPalClicker";
            this.DefaultPalClicker.Size = new System.Drawing.Size(16, 16);
            this.DefaultPalClicker.TabIndex = 10;
            this.DefaultPalClicker.DoubleClick += new System.EventHandler(this.DefaultPalClicker_Click);
            this.DefaultPalClicker.Click += new System.EventHandler(this.DefaultPalClicker_Click);
            // 
            // SelectedItemPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.DefaultPalClicker);
            this.Controls.Add(this.lblMapLocation);
            this.Controls.Add(this.lblDefaultPal);
            this.Controls.Add(this.PasswordDataToggle);
            this.Controls.Add(this.musicButton);
            this.Controls.Add(this.SlotLabel);
            this.Controls.Add(this.SlotImage);
            this.Controls.Add(this.FreespaceLabel);
            this.Controls.Add(this.LayoutLabel);
            this.Controls.Add(this.PaletteLabel);
            this.Controls.Add(this.IndexLabel);
            this.Name = "SelectedItemPanel";
            this.Size = new System.Drawing.Size(258, 40);
            ((System.ComponentModel.ISupportInitialize)(this.SlotImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordDataToggle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IndexLabel;
        private System.Windows.Forms.Label PaletteLabel;
        private System.Windows.Forms.Label LayoutLabel;
        private System.Windows.Forms.Label FreespaceLabel;
        private System.Windows.Forms.PictureBox SlotImage;
        private System.Windows.Forms.Label SlotLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        internal ToggleBox musicButton;
        internal ToggleBox PasswordDataToggle;
        private System.Windows.Forms.Label lblDefaultPal;
        private System.Windows.Forms.Label lblMapLocation;
        private System.Windows.Forms.Label DefaultPalClicker;
    }
}
