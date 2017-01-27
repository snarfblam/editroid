namespace Editroid
{
	partial class PalTableEditor
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
			this.pal1 = new System.Windows.Forms.PictureBox();
			this.pal0 = new System.Windows.Forms.PictureBox();
			this.pal3 = new System.Windows.Forms.PictureBox();
			this.pal2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pal1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pal0)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pal3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pal2)).BeginInit();
			this.SuspendLayout();
			// 
			// pal1
			// 
			this.pal1.Image = global::Editroid.Properties.Resources.PaletteOverlay;
			this.pal1.Location = new System.Drawing.Point(16, 0);
			this.pal1.Name = "pal1";
			this.pal1.Size = new System.Drawing.Size(16, 16);
			this.pal1.TabIndex = 1;
			this.pal1.TabStop = false;
			// 
			// pal0
			// 
			this.pal0.Image = global::Editroid.Properties.Resources.PaletteOverlay;
			this.pal0.Location = new System.Drawing.Point(0, 0);
			this.pal0.Name = "pal0";
			this.pal0.Size = new System.Drawing.Size(16, 16);
			this.pal0.TabIndex = 0;
			this.pal0.TabStop = false;
			// 
			// pal3
			// 
			this.pal3.Image = global::Editroid.Properties.Resources.PaletteOverlay;
			this.pal3.Location = new System.Drawing.Point(48, 0);
			this.pal3.Name = "pal3";
			this.pal3.Size = new System.Drawing.Size(16, 16);
			this.pal3.TabIndex = 3;
			this.pal3.TabStop = false;
			// 
			// pal2
			// 
			this.pal2.Image = global::Editroid.Properties.Resources.PaletteOverlay;
			this.pal2.Location = new System.Drawing.Point(32, 0);
			this.pal2.Name = "pal2";
			this.pal2.Size = new System.Drawing.Size(16, 16);
			this.pal2.TabIndex = 2;
			this.pal2.TabStop = false;
			// 
			// PalTableEditor
			// 
			this.Controls.Add(this.pal3);
			this.Controls.Add(this.pal2);
			this.Controls.Add(this.pal1);
			this.Controls.Add(this.pal0);
			this.Name = "PalTableEditor";
			this.Size = new System.Drawing.Size(64, 16);
			((System.ComponentModel.ISupportInitialize)(this.pal1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pal0)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pal3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pal2)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pal0;
		private System.Windows.Forms.PictureBox pal1;
		private System.Windows.Forms.PictureBox pal3;
		private System.Windows.Forms.PictureBox pal2;
	}
}
