namespace Editroid
{
	partial class frmPointer
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPointer));
			this.cboLevel = new System.Windows.Forms.ComboBox();
			this.txtPointer = new System.Windows.Forms.TextBox();
			this.txtROM = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cboLevel
			// 
			this.cboLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLevel.FormattingEnabled = true;
			this.cboLevel.Items.AddRange(new object[] {
            "Brinstar",
            "Norfair",
            "Tourian",
            "Kraid",
            "Ridley",
            "(No Level)"});
			this.cboLevel.Location = new System.Drawing.Point(12, 29);
			this.cboLevel.Name = "cboLevel";
			this.cboLevel.Size = new System.Drawing.Size(131, 21);
			this.cboLevel.TabIndex = 0;
			this.cboLevel.SelectedIndexChanged += new System.EventHandler(this.cboLevel_SelectedIndexChanged);
			// 
			// label1
			// 
			label1.Location = new System.Drawing.Point(12, 9);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(100, 17);
			label1.TabIndex = 1;
			label1.Text = "Level:";
			// 
			// txtPointer
			// 
			this.txtPointer.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPointer.Location = new System.Drawing.Point(15, 149);
			this.txtPointer.Name = "txtPointer";
			this.txtPointer.Size = new System.Drawing.Size(128, 20);
			this.txtPointer.TabIndex = 2;
			this.txtPointer.Text = "0";
			this.txtPointer.TextChanged += new System.EventHandler(this.txtPointer_TextChanged);
			// 
			// txtROM
			// 
			this.txtROM.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtROM.Location = new System.Drawing.Point(15, 104);
			this.txtROM.Name = "txtROM";
			this.txtROM.Size = new System.Drawing.Size(128, 20);
			this.txtROM.TabIndex = 3;
			this.txtROM.Text = "0";
			this.txtROM.TextChanged += new System.EventHandler(this.txtROM_TextChanged);
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(15, 84);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(128, 17);
			label2.TabIndex = 4;
			label2.Text = "ROM Offset";
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(15, 129);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(128, 17);
			label3.TabIndex = 5;
			label3.Text = "Pointer Value";
			// 
			// frmPointer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(155, 189);
			this.Controls.Add(label3);
			this.Controls.Add(label2);
			this.Controls.Add(this.txtROM);
			this.Controls.Add(this.txtPointer);
			this.Controls.Add(label1);
			this.Controls.Add(this.cboLevel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPointer";
			this.Text = "Pointer Converter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cboLevel;
		private System.Windows.Forms.TextBox txtPointer;
		private System.Windows.Forms.TextBox txtROM;
	}
}