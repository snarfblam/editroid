namespace Editroid
{
	partial class ComboEditor
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
            this.lblSelection = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSelection
            // 
            this.lblSelection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblSelection.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSelection.Location = new System.Drawing.Point(0, 0);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(16, 16);
            this.lblSelection.TabIndex = 0;
            this.lblSelection.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseMove);
            this.lblSelection.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseDown);
            this.lblSelection.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseUp);
            // 
            // ComboEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.lblSelection);
            this.Name = "ComboEditor";
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Label lblSelection;
	}
}
