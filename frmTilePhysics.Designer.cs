namespace Editroid
{
    partial class frmTilePhysics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTilePhysics));
            this.picTiles = new System.Windows.Forms.PictureBox();
            this.radSolid = new System.Windows.Forms.RadioButton();
            this.radAir = new System.Windows.Forms.RadioButton();
            this.readCustom = new System.Windows.Forms.RadioButton();
            this.radBreakable = new System.Windows.Forms.RadioButton();
            this.radDoorcap = new System.Windows.Forms.RadioButton();
            this.radDoorV = new System.Windows.Forms.RadioButton();
            this.radDoorH = new System.Windows.Forms.RadioButton();
            this.nudCustom = new System.Windows.Forms.NumericUpDown();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radRevert = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.picTiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCustom)).BeginInit();
            this.SuspendLayout();
            // 
            // picTiles
            // 
            this.picTiles.BackColor = System.Drawing.Color.Black;
            this.picTiles.Location = new System.Drawing.Point(12, 12);
            this.picTiles.Name = "picTiles";
            this.picTiles.Size = new System.Drawing.Size(256, 256);
            this.picTiles.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTiles.TabIndex = 0;
            this.picTiles.TabStop = false;
            this.picTiles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTiles_MouseMove);
            this.picTiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTiles_MouseDown);
            this.picTiles.Paint += new System.Windows.Forms.PaintEventHandler(this.picTiles_Paint);
            // 
            // radSolid
            // 
            this.radSolid.AutoSize = true;
            this.radSolid.Checked = true;
            this.radSolid.Location = new System.Drawing.Point(287, 56);
            this.radSolid.Name = "radSolid";
            this.radSolid.Size = new System.Drawing.Size(48, 17);
            this.radSolid.TabIndex = 1;
            this.radSolid.TabStop = true;
            this.radSolid.Text = "Solid";
            this.radSolid.UseVisualStyleBackColor = true;
            this.radSolid.CheckedChanged += new System.EventHandler(this.radSolid_CheckedChanged);
            // 
            // radAir
            // 
            this.radAir.AutoSize = true;
            this.radAir.Location = new System.Drawing.Point(287, 79);
            this.radAir.Name = "radAir";
            this.radAir.Size = new System.Drawing.Size(37, 17);
            this.radAir.TabIndex = 2;
            this.radAir.Text = "Air";
            this.radAir.UseVisualStyleBackColor = true;
            this.radAir.CheckedChanged += new System.EventHandler(this.radAir_CheckedChanged);
            // 
            // readCustom
            // 
            this.readCustom.AutoSize = true;
            this.readCustom.Location = new System.Drawing.Point(287, 194);
            this.readCustom.Name = "readCustom";
            this.readCustom.Size = new System.Drawing.Size(60, 17);
            this.readCustom.TabIndex = 3;
            this.readCustom.Text = "Custom";
            this.readCustom.UseVisualStyleBackColor = true;
            this.readCustom.CheckedChanged += new System.EventHandler(this.readCustom_CheckedChanged);
            // 
            // radBreakable
            // 
            this.radBreakable.AutoSize = true;
            this.radBreakable.Enabled = false;
            this.radBreakable.Location = new System.Drawing.Point(287, 171);
            this.radBreakable.Name = "radBreakable";
            this.radBreakable.Size = new System.Drawing.Size(73, 17);
            this.radBreakable.TabIndex = 4;
            this.radBreakable.Text = "Breakable";
            this.radBreakable.UseVisualStyleBackColor = true;
            this.radBreakable.CheckedChanged += new System.EventHandler(this.radBreakable_CheckedChanged);
            // 
            // radDoorcap
            // 
            this.radDoorcap.AutoSize = true;
            this.radDoorcap.Location = new System.Drawing.Point(287, 148);
            this.radDoorcap.Name = "radDoorcap";
            this.radDoorcap.Size = new System.Drawing.Size(70, 17);
            this.radDoorcap.TabIndex = 5;
            this.radDoorcap.Text = "Door Cap";
            this.radDoorcap.UseVisualStyleBackColor = true;
            this.radDoorcap.CheckedChanged += new System.EventHandler(this.radDoorcap_CheckedChanged);
            // 
            // radDoorV
            // 
            this.radDoorV.AutoSize = true;
            this.radDoorV.Location = new System.Drawing.Point(287, 125);
            this.radDoorV.Name = "radDoorV";
            this.radDoorV.Size = new System.Drawing.Size(58, 17);
            this.radDoorV.TabIndex = 6;
            this.radDoorV.Text = "Door V";
            this.radDoorV.UseVisualStyleBackColor = true;
            this.radDoorV.CheckedChanged += new System.EventHandler(this.radDoorV_CheckedChanged);
            // 
            // radDoorH
            // 
            this.radDoorH.AutoSize = true;
            this.radDoorH.Location = new System.Drawing.Point(287, 102);
            this.radDoorH.Name = "radDoorH";
            this.radDoorH.Size = new System.Drawing.Size(59, 17);
            this.radDoorH.TabIndex = 7;
            this.radDoorH.Text = "Door H";
            this.radDoorH.UseVisualStyleBackColor = true;
            this.radDoorH.CheckedChanged += new System.EventHandler(this.radDoorH_CheckedChanged);
            // 
            // nudCustom
            // 
            this.nudCustom.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCustom.Hexadecimal = true;
            this.nudCustom.Location = new System.Drawing.Point(303, 217);
            this.nudCustom.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCustom.Name = "nudCustom";
            this.nudCustom.Size = new System.Drawing.Size(57, 20);
            this.nudCustom.TabIndex = 8;
            // 
            // btnAccept
            // 
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAccept.Location = new System.Drawing.Point(491, 245);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 9;
            this.btnAccept.Text = "Apply";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(410, 245);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(329, 245);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 11;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(271, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Change to...";
            // 
            // radRevert
            // 
            this.radRevert.AutoSize = true;
            this.radRevert.Location = new System.Drawing.Point(284, 33);
            this.radRevert.Name = "radRevert";
            this.radRevert.Size = new System.Drawing.Size(63, 17);
            this.radRevert.TabIndex = 13;
            this.radRevert.Text = "(Revert)";
            this.radRevert.UseVisualStyleBackColor = true;
            this.radRevert.CheckedChanged += new System.EventHandler(this.radRevert_CheckedChanged);
            // 
            // frmTilePhysics
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(577, 280);
            this.Controls.Add(this.radRevert);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.nudCustom);
            this.Controls.Add(this.radDoorH);
            this.Controls.Add(this.radDoorV);
            this.Controls.Add(this.radDoorcap);
            this.Controls.Add(this.radBreakable);
            this.Controls.Add(this.readCustom);
            this.Controls.Add(this.radAir);
            this.Controls.Add(this.radSolid);
            this.Controls.Add(this.picTiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTilePhysics";
            this.Text = "Tile Physics";
            ((System.ComponentModel.ISupportInitialize)(this.picTiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCustom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picTiles;
        private System.Windows.Forms.RadioButton radSolid;
        private System.Windows.Forms.RadioButton radAir;
        private System.Windows.Forms.RadioButton readCustom;
        private System.Windows.Forms.RadioButton radBreakable;
        private System.Windows.Forms.RadioButton radDoorcap;
        private System.Windows.Forms.RadioButton radDoorV;
        private System.Windows.Forms.RadioButton radDoorH;
        private System.Windows.Forms.NumericUpDown nudCustom;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radRevert;
    }
}