namespace Editroid
{
	partial class PaletteEditor
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
			this.table3 = new Editroid.PalTableEditor();
			this.table2 = new Editroid.PalTableEditor();
			this.table1 = new Editroid.PalTableEditor();
			this.table0 = new Editroid.PalTableEditor();
			this.SuspendLayout();
			// 
			// table3
			// 
			this.table3.Location = new System.Drawing.Point(0, 51);
			this.table3.Name = "table3";
			this.table3.Size = new System.Drawing.Size(64, 16);
			this.table3.TabIndex = 3;
			// 
			// table2
			// 
			this.table2.Location = new System.Drawing.Point(0, 34);
			this.table2.Name = "table2";
			this.table2.Size = new System.Drawing.Size(64, 16);
			this.table2.TabIndex = 2;
			// 
			// table1
			// 
			this.table1.Location = new System.Drawing.Point(0, 17);
			this.table1.Name = "table1";
			this.table1.Size = new System.Drawing.Size(64, 16);
			this.table1.TabIndex = 1;
			// 
			// table0
			// 
			this.table0.Location = new System.Drawing.Point(0, 0);
			this.table0.Name = "table0";
			this.table0.Size = new System.Drawing.Size(64, 16);
			this.table0.TabIndex = 0;
			// 
			// PaletteEditor
			// 
			this.Controls.Add(this.table3);
			this.Controls.Add(this.table2);
			this.Controls.Add(this.table1);
			this.Controls.Add(this.table0);
			this.Name = "PaletteEditor";
			this.Size = new System.Drawing.Size(64, 68);
			this.ResumeLayout(false);

		}

		#endregion

		private PalTableEditor table0;
		private PalTableEditor table1;
		private PalTableEditor table3;
		private PalTableEditor table2;
	}
}
