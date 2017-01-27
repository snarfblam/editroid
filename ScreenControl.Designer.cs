namespace Editroid
{
	partial class ScreenControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.PictureBox picBottomBorder;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenControl));
            System.Windows.Forms.PictureBox picTopBorder;
            this.pnlScreen = new System.Windows.Forms.PictureBox();
            this.lblType = new System.Windows.Forms.Label();
            this.lblPal = new System.Windows.Forms.Label();
            this.lblSlot = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblScreen = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            picBottomBorder = new System.Windows.Forms.PictureBox();
            picTopBorder = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(picBottomBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(picTopBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // picBottomBorder
            // 
            picBottomBorder.Image = ((System.Drawing.Image)(resources.GetObject("picBottomBorder.Image")));
            picBottomBorder.Location = new System.Drawing.Point(0, 248);
            picBottomBorder.Name = "picBottomBorder";
            picBottomBorder.Size = new System.Drawing.Size(256, 8);
            picBottomBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            picBottomBorder.TabIndex = 7;
            picBottomBorder.TabStop = false;
            picBottomBorder.Visible = false;
            // 
            // picTopBorder
            // 
            picTopBorder.Image = ((System.Drawing.Image)(resources.GetObject("picTopBorder.Image")));
            picTopBorder.Location = new System.Drawing.Point(0, 240);
            picTopBorder.Name = "picTopBorder";
            picTopBorder.Size = new System.Drawing.Size(256, 8);
            picTopBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            picTopBorder.TabIndex = 6;
            picTopBorder.TabStop = false;
            picTopBorder.Visible = false;
            // 
            // pnlScreen
            // 
            this.pnlScreen.BackColor = System.Drawing.Color.Black;
            this.pnlScreen.Location = new System.Drawing.Point(0, 0);
            this.pnlScreen.Name = "pnlScreen";
            this.pnlScreen.Size = new System.Drawing.Size(256, 240);
            this.pnlScreen.TabIndex = 5;
            this.pnlScreen.TabStop = false;
            // 
            // lblType
            // 
            this.lblType.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblType.Image = global::Editroid.Properties.Resources.VariableArray;
            this.lblType.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblType.Location = new System.Drawing.Point(3, 240);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(42, 16);
            this.lblType.TabIndex = 8;
            this.lblType.Text = "00";
            this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblType, "The selected item\'s type");
            // 
            // lblPal
            // 
            this.lblPal.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPal.Image = global::Editroid.Properties.Resources.Structure;
            this.lblPal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPal.Location = new System.Drawing.Point(51, 240);
            this.lblPal.Name = "lblPal";
            this.lblPal.Size = new System.Drawing.Size(35, 16);
            this.lblPal.TabIndex = 9;
            this.lblPal.Text = "0";
            this.lblPal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblPal, "The selected item\'s palette");
            // 
            // lblSlot
            // 
            this.lblSlot.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSlot.Image = ((System.Drawing.Image)(resources.GetObject("lblSlot.Image")));
            this.lblSlot.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSlot.Location = new System.Drawing.Point(92, 240);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.Size = new System.Drawing.Size(35, 16);
            this.lblSlot.TabIndex = 10;
            this.lblSlot.Text = "0";
            this.lblSlot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblSlot, "Enemy sprite slot");
            this.lblSlot.DoubleClick += new System.EventHandler(this.lblSlot_Click);
            this.lblSlot.Click += new System.EventHandler(this.lblSlot_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 50000000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ShowAlways = true;
            // 
            // lblScreen
            // 
            this.lblScreen.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScreen.Image = global::Editroid.Properties.Resources.Image;
            this.lblScreen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblScreen.Location = new System.Drawing.Point(162, 240);
            this.lblScreen.Name = "lblScreen";
            this.lblScreen.Size = new System.Drawing.Size(42, 16);
            this.lblScreen.TabIndex = 11;
            this.lblScreen.Text = "00";
            this.lblScreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblScreen, "Index of the showing screen");
            // 
            // lblMemory
            // 
            this.lblMemory.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemory.Image = global::Editroid.Properties.Resources.Binary16;
            this.lblMemory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMemory.Location = new System.Drawing.Point(210, 240);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(45, 16);
            this.lblMemory.TabIndex = 12;
            this.lblMemory.Text = "256";
            this.lblMemory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblMemory, "Free bytes of memory for this level");
            // 
            // ScreenControl
            // 
            this.Controls.Add(this.lblMemory);
            this.Controls.Add(this.lblScreen);
            this.Controls.Add(this.lblSlot);
            this.Controls.Add(this.lblPal);
            this.Controls.Add(this.lblType);
            this.Controls.Add(picBottomBorder);
            this.Controls.Add(picTopBorder);
            this.Controls.Add(this.pnlScreen);
            this.Name = "ScreenControl";
            this.Size = new System.Drawing.Size(256, 256);
            ((System.ComponentModel.ISupportInitialize)(picBottomBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(picTopBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.PictureBox pnlScreen;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblPal;
        private System.Windows.Forms.Label lblSlot;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblScreen;
        private System.Windows.Forms.Label lblMemory;
	}
}
