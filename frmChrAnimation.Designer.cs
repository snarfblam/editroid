namespace Editroid
{
    partial class frmChrAnimation
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChrAnimation));
            this.cboLevel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudSpr0 = new System.Windows.Forms.NumericUpDown();
            this.btnSpr0 = new System.Windows.Forms.Button();
            this.lblJustinBailey = new System.Windows.Forms.Label();
            this.btnSpr1 = new System.Windows.Forms.Button();
            this.nudSpr1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboAnimation = new System.Windows.Forms.ComboBox();
            this.txtAnimationName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lstFrames = new System.Windows.Forms.ListBox();
            this.btnBg0 = new System.Windows.Forms.Button();
            this.nudBg0 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.btnBg1 = new System.Windows.Forms.Button();
            this.nudBg1 = new System.Windows.Forms.NumericUpDown();
            this.btnBg2 = new System.Windows.Forms.Button();
            this.nudBg2 = new System.Windows.Forms.NumericUpDown();
            this.btnBg3 = new System.Windows.Forms.Button();
            this.nudBg3 = new System.Windows.Forms.NumericUpDown();
            this.picBg = new System.Windows.Forms.PictureBox();
            this.picSpr = new System.Windows.Forms.PictureBox();
            this.chkAnimate = new System.Windows.Forms.CheckBox();
            this.chkJustinBailey = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nudFrametime = new System.Windows.Forms.NumericUpDown();
            this.btnDeleteAnimation = new System.Windows.Forms.Button();
            this.btnRemoveFrame = new System.Windows.Forms.Button();
            this.btnAddFrame = new System.Windows.Forms.Button();
            this.tmrAnimate = new System.Windows.Forms.Timer(this.components);
            this.lblFreeSpace = new System.Windows.Forms.Label();
            this.btnAddAnimation = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudSpr0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpr1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrametime)).BeginInit();
            this.SuspendLayout();
            // 
            // cboLevel
            // 
            this.cboLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLevel.FormattingEnabled = true;
            this.cboLevel.Items.AddRange(new object[] {
            "Title",
            "Brinstar",
            "Norfair",
            "Tourian",
            "Kraid",
            "Ridley"});
            this.cboLevel.Location = new System.Drawing.Point(12, 12);
            this.cboLevel.Name = "cboLevel";
            this.cboLevel.Size = new System.Drawing.Size(139, 21);
            this.cboLevel.TabIndex = 0;
            this.toolTip1.SetToolTip(this.cboLevel, "Areas");
            this.cboLevel.SelectedIndexChanged += new System.EventHandler(this.cboLevel_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(157, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sprite CHR";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // nudSpr0
            // 
            this.nudSpr0.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSpr0.Hexadecimal = true;
            this.nudSpr0.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudSpr0.Location = new System.Drawing.Point(250, 13);
            this.nudSpr0.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSpr0.Name = "nudSpr0";
            this.nudSpr0.Size = new System.Drawing.Size(53, 20);
            this.nudSpr0.TabIndex = 2;
            this.nudSpr0.ValueChanged += new System.EventHandler(this.nudSpr0_ValueChanged);
            // 
            // btnSpr0
            // 
            this.btnSpr0.Location = new System.Drawing.Point(309, 12);
            this.btnSpr0.Name = "btnSpr0";
            this.btnSpr0.Size = new System.Drawing.Size(20, 21);
            this.btnSpr0.TabIndex = 3;
            this.btnSpr0.Text = "●";
            this.toolTip1.SetToolTip(this.btnSpr0, "Browse");
            this.btnSpr0.UseVisualStyleBackColor = true;
            this.btnSpr0.Click += new System.EventHandler(this.btnSpr0_Click);
            // 
            // lblJustinBailey
            // 
            this.lblJustinBailey.AutoSize = true;
            this.lblJustinBailey.Location = new System.Drawing.Point(531, 16);
            this.lblJustinBailey.Name = "lblJustinBailey";
            this.lblJustinBailey.Size = new System.Drawing.Size(113, 13);
            this.lblJustinBailey.TabIndex = 4;
            this.lblJustinBailey.Text = "Justin Bailey Bank: XX";
            // 
            // btnSpr1
            // 
            this.btnSpr1.Location = new System.Drawing.Point(505, 12);
            this.btnSpr1.Name = "btnSpr1";
            this.btnSpr1.Size = new System.Drawing.Size(20, 21);
            this.btnSpr1.TabIndex = 7;
            this.btnSpr1.Text = "●";
            this.toolTip1.SetToolTip(this.btnSpr1, "Browse");
            this.btnSpr1.UseVisualStyleBackColor = true;
            this.btnSpr1.Click += new System.EventHandler(this.btnSpr1_Click);
            // 
            // nudSpr1
            // 
            this.nudSpr1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSpr1.Hexadecimal = true;
            this.nudSpr1.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudSpr1.Location = new System.Drawing.Point(446, 13);
            this.nudSpr1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSpr1.Name = "nudSpr1";
            this.nudSpr1.Size = new System.Drawing.Size(53, 20);
            this.nudSpr1.TabIndex = 6;
            this.nudSpr1.ValueChanged += new System.EventHandler(this.nudSpr1_ValueChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(335, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Enemy CHR";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(21, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(746, 1);
            this.label4.TabIndex = 8;
            // 
            // cboAnimation
            // 
            this.cboAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnimation.FormattingEnabled = true;
            this.cboAnimation.Items.AddRange(new object[] {
            "Brinstar",
            "Norfair",
            "Tourian",
            "Kraid",
            "Ridley"});
            this.cboAnimation.Location = new System.Drawing.Point(12, 65);
            this.cboAnimation.Name = "cboAnimation";
            this.cboAnimation.Size = new System.Drawing.Size(139, 21);
            this.cboAnimation.TabIndex = 9;
            this.toolTip1.SetToolTip(this.cboAnimation, "Animations");
            this.cboAnimation.SelectedIndexChanged += new System.EventHandler(this.cboAnimation_SelectedIndexChanged);
            // 
            // txtAnimationName
            // 
            this.txtAnimationName.Location = new System.Drawing.Point(160, 65);
            this.txtAnimationName.Name = "txtAnimationName";
            this.txtAnimationName.Size = new System.Drawing.Size(143, 20);
            this.txtAnimationName.TabIndex = 10;
            this.txtAnimationName.TextChanged += new System.EventHandler(this.txtAnimationName_TextChanged);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(21, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(746, 1);
            this.label5.TabIndex = 11;
            // 
            // lstFrames
            // 
            this.lstFrames.FormattingEnabled = true;
            this.lstFrames.IntegralHeight = false;
            this.lstFrames.Location = new System.Drawing.Point(12, 119);
            this.lstFrames.Name = "lstFrames";
            this.lstFrames.Size = new System.Drawing.Size(139, 279);
            this.lstFrames.TabIndex = 12;
            this.lstFrames.SelectedIndexChanged += new System.EventHandler(this.lstFrames_SelectedIndexChanged);
            // 
            // btnBg0
            // 
            this.btnBg0.Location = new System.Drawing.Point(219, 139);
            this.btnBg0.Name = "btnBg0";
            this.btnBg0.Size = new System.Drawing.Size(20, 21);
            this.btnBg0.TabIndex = 15;
            this.btnBg0.Text = "●";
            this.toolTip1.SetToolTip(this.btnBg0, "Browse");
            this.btnBg0.UseVisualStyleBackColor = true;
            this.btnBg0.Click += new System.EventHandler(this.btnBg0_Click);
            // 
            // nudBg0
            // 
            this.nudBg0.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBg0.Hexadecimal = true;
            this.nudBg0.Location = new System.Drawing.Point(160, 140);
            this.nudBg0.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBg0.Name = "nudBg0";
            this.nudBg0.Size = new System.Drawing.Size(53, 20);
            this.nudBg0.TabIndex = 14;
            this.nudBg0.ValueChanged += new System.EventHandler(this.nudBg0_ValueChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(157, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 18);
            this.label6.TabIndex = 13;
            this.label6.Text = "Background CHR";
            // 
            // btnBg1
            // 
            this.btnBg1.Location = new System.Drawing.Point(219, 165);
            this.btnBg1.Name = "btnBg1";
            this.btnBg1.Size = new System.Drawing.Size(20, 21);
            this.btnBg1.TabIndex = 17;
            this.btnBg1.Text = "●";
            this.toolTip1.SetToolTip(this.btnBg1, "Browse");
            this.btnBg1.UseVisualStyleBackColor = true;
            this.btnBg1.Click += new System.EventHandler(this.btnBg1_Click);
            // 
            // nudBg1
            // 
            this.nudBg1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBg1.Hexadecimal = true;
            this.nudBg1.Location = new System.Drawing.Point(160, 166);
            this.nudBg1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBg1.Name = "nudBg1";
            this.nudBg1.Size = new System.Drawing.Size(53, 20);
            this.nudBg1.TabIndex = 16;
            this.nudBg1.ValueChanged += new System.EventHandler(this.nudBg1_ValueChanged);
            // 
            // btnBg2
            // 
            this.btnBg2.Location = new System.Drawing.Point(219, 191);
            this.btnBg2.Name = "btnBg2";
            this.btnBg2.Size = new System.Drawing.Size(20, 21);
            this.btnBg2.TabIndex = 19;
            this.btnBg2.Text = "●";
            this.toolTip1.SetToolTip(this.btnBg2, "Browse");
            this.btnBg2.UseVisualStyleBackColor = true;
            this.btnBg2.Click += new System.EventHandler(this.btnBg2_Click);
            // 
            // nudBg2
            // 
            this.nudBg2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBg2.Hexadecimal = true;
            this.nudBg2.Location = new System.Drawing.Point(160, 192);
            this.nudBg2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBg2.Name = "nudBg2";
            this.nudBg2.Size = new System.Drawing.Size(53, 20);
            this.nudBg2.TabIndex = 18;
            this.nudBg2.ValueChanged += new System.EventHandler(this.nudBg2_ValueChanged);
            // 
            // btnBg3
            // 
            this.btnBg3.Location = new System.Drawing.Point(219, 217);
            this.btnBg3.Name = "btnBg3";
            this.btnBg3.Size = new System.Drawing.Size(20, 21);
            this.btnBg3.TabIndex = 21;
            this.btnBg3.Text = "●";
            this.toolTip1.SetToolTip(this.btnBg3, "Browse");
            this.btnBg3.UseVisualStyleBackColor = true;
            this.btnBg3.Click += new System.EventHandler(this.btnBg3_Click);
            // 
            // nudBg3
            // 
            this.nudBg3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBg3.Hexadecimal = true;
            this.nudBg3.Location = new System.Drawing.Point(160, 218);
            this.nudBg3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBg3.Name = "nudBg3";
            this.nudBg3.Size = new System.Drawing.Size(53, 20);
            this.nudBg3.TabIndex = 20;
            this.nudBg3.ValueChanged += new System.EventHandler(this.nudBg3_ValueChanged);
            // 
            // picBg
            // 
            this.picBg.BackColor = System.Drawing.Color.Black;
            this.picBg.Location = new System.Drawing.Point(261, 119);
            this.picBg.Name = "picBg";
            this.picBg.Size = new System.Drawing.Size(256, 256);
            this.picBg.TabIndex = 22;
            this.picBg.TabStop = false;
            this.picBg.Paint += new System.Windows.Forms.PaintEventHandler(this.picBg_Paint);
            // 
            // picSpr
            // 
            this.picSpr.BackColor = System.Drawing.Color.Black;
            this.picSpr.Location = new System.Drawing.Point(523, 119);
            this.picSpr.Name = "picSpr";
            this.picSpr.Size = new System.Drawing.Size(256, 256);
            this.picSpr.TabIndex = 23;
            this.picSpr.TabStop = false;
            this.picSpr.Paint += new System.Windows.Forms.PaintEventHandler(this.picSpr_Paint);
            // 
            // chkAnimate
            // 
            this.chkAnimate.AutoSize = true;
            this.chkAnimate.Location = new System.Drawing.Point(261, 381);
            this.chkAnimate.Name = "chkAnimate";
            this.chkAnimate.Size = new System.Drawing.Size(64, 17);
            this.chkAnimate.TabIndex = 24;
            this.chkAnimate.Text = "Animate";
            this.chkAnimate.UseVisualStyleBackColor = true;
            this.chkAnimate.CheckedChanged += new System.EventHandler(this.chkAnimate_CheckedChanged);
            // 
            // chkJustinBailey
            // 
            this.chkJustinBailey.AutoSize = true;
            this.chkJustinBailey.Location = new System.Drawing.Point(523, 381);
            this.chkJustinBailey.Name = "chkJustinBailey";
            this.chkJustinBailey.Size = new System.Drawing.Size(84, 17);
            this.chkJustinBailey.TabIndex = 25;
            this.chkJustinBailey.Text = "Justin Bailey";
            this.chkJustinBailey.UseVisualStyleBackColor = true;
            this.chkJustinBailey.CheckedChanged += new System.EventHandler(this.chkJustinBailey_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(704, 408);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(157, 241);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 18);
            this.label2.TabIndex = 27;
            this.label2.Text = "Frame Time";
            // 
            // nudFrametime
            // 
            this.nudFrametime.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudFrametime.Hexadecimal = true;
            this.nudFrametime.Location = new System.Drawing.Point(160, 262);
            this.nudFrametime.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudFrametime.Name = "nudFrametime";
            this.nudFrametime.Size = new System.Drawing.Size(53, 20);
            this.nudFrametime.TabIndex = 28;
            this.nudFrametime.ValueChanged += new System.EventHandler(this.nudFrametime_ValueChanged);
            // 
            // btnDeleteAnimation
            // 
            this.btnDeleteAnimation.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteAnimation.Image")));
            this.btnDeleteAnimation.Location = new System.Drawing.Point(336, 64);
            this.btnDeleteAnimation.Name = "btnDeleteAnimation";
            this.btnDeleteAnimation.Size = new System.Drawing.Size(21, 21);
            this.btnDeleteAnimation.TabIndex = 29;
            this.toolTip1.SetToolTip(this.btnDeleteAnimation, "Delete current animation");
            this.btnDeleteAnimation.UseVisualStyleBackColor = true;
            this.btnDeleteAnimation.Click += new System.EventHandler(this.btnDeleteAnimation_Click);
            // 
            // btnRemoveFrame
            // 
            this.btnRemoveFrame.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveFrame.Image")));
            this.btnRemoveFrame.Location = new System.Drawing.Point(39, 404);
            this.btnRemoveFrame.Name = "btnRemoveFrame";
            this.btnRemoveFrame.Size = new System.Drawing.Size(21, 21);
            this.btnRemoveFrame.TabIndex = 30;
            this.toolTip1.SetToolTip(this.btnRemoveFrame, "Remove selected frame from animation");
            this.btnRemoveFrame.UseVisualStyleBackColor = true;
            this.btnRemoveFrame.Click += new System.EventHandler(this.btnRemoveFrame_Click);
            // 
            // btnAddFrame
            // 
            this.btnAddFrame.Image = ((System.Drawing.Image)(resources.GetObject("btnAddFrame.Image")));
            this.btnAddFrame.Location = new System.Drawing.Point(12, 404);
            this.btnAddFrame.Name = "btnAddFrame";
            this.btnAddFrame.Size = new System.Drawing.Size(21, 21);
            this.btnAddFrame.TabIndex = 31;
            this.toolTip1.SetToolTip(this.btnAddFrame, "Add frame to animation");
            this.btnAddFrame.UseVisualStyleBackColor = true;
            this.btnAddFrame.Click += new System.EventHandler(this.btnAddFrame_Click);
            // 
            // tmrAnimate
            // 
            this.tmrAnimate.Tick += new System.EventHandler(this.tmrAnimate_Tick);
            // 
            // lblFreeSpace
            // 
            this.lblFreeSpace.Location = new System.Drawing.Point(66, 408);
            this.lblFreeSpace.Name = "lblFreeSpace";
            this.lblFreeSpace.Size = new System.Drawing.Size(138, 23);
            this.lblFreeSpace.TabIndex = 32;
            this.lblFreeSpace.Text = "Free space:";
            // 
            // btnAddAnimation
            // 
            this.btnAddAnimation.Image = ((System.Drawing.Image)(resources.GetObject("btnAddAnimation.Image")));
            this.btnAddAnimation.Location = new System.Drawing.Point(309, 64);
            this.btnAddAnimation.Name = "btnAddAnimation";
            this.btnAddAnimation.Size = new System.Drawing.Size(21, 21);
            this.btnAddAnimation.TabIndex = 33;
            this.toolTip1.SetToolTip(this.btnAddAnimation, "Add new animation");
            this.btnAddAnimation.UseVisualStyleBackColor = true;
            this.btnAddAnimation.Click += new System.EventHandler(this.btnAddAnimation_Click);
            // 
            // frmChrAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 440);
            this.Controls.Add(this.btnAddAnimation);
            this.Controls.Add(this.lblFreeSpace);
            this.Controls.Add(this.btnAddFrame);
            this.Controls.Add(this.btnRemoveFrame);
            this.Controls.Add(this.btnDeleteAnimation);
            this.Controls.Add(this.nudFrametime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkJustinBailey);
            this.Controls.Add(this.chkAnimate);
            this.Controls.Add(this.picSpr);
            this.Controls.Add(this.picBg);
            this.Controls.Add(this.btnBg3);
            this.Controls.Add(this.nudBg3);
            this.Controls.Add(this.btnBg2);
            this.Controls.Add(this.nudBg2);
            this.Controls.Add(this.btnBg1);
            this.Controls.Add(this.nudBg1);
            this.Controls.Add(this.btnBg0);
            this.Controls.Add(this.nudBg0);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lstFrames);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtAnimationName);
            this.Controls.Add(this.cboAnimation);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSpr1);
            this.Controls.Add(this.nudSpr1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblJustinBailey);
            this.Controls.Add(this.btnSpr0);
            this.Controls.Add(this.nudSpr0);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboLevel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmChrAnimation";
            this.Text = "CHR Animation";
            ((System.ComponentModel.ISupportInitialize)(this.nudSpr0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpr1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBg3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrametime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudSpr0;
        private System.Windows.Forms.Button btnSpr0;
        private System.Windows.Forms.Label lblJustinBailey;
        private System.Windows.Forms.Button btnSpr1;
        private System.Windows.Forms.NumericUpDown nudSpr1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboAnimation;
        private System.Windows.Forms.TextBox txtAnimationName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lstFrames;
        private System.Windows.Forms.Button btnBg0;
        private System.Windows.Forms.NumericUpDown nudBg0;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnBg1;
        private System.Windows.Forms.NumericUpDown nudBg1;
        private System.Windows.Forms.Button btnBg2;
        private System.Windows.Forms.NumericUpDown nudBg2;
        private System.Windows.Forms.Button btnBg3;
        private System.Windows.Forms.NumericUpDown nudBg3;
        private System.Windows.Forms.PictureBox picBg;
        private System.Windows.Forms.PictureBox picSpr;
        private System.Windows.Forms.CheckBox chkAnimate;
        private System.Windows.Forms.CheckBox chkJustinBailey;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudFrametime;
        private System.Windows.Forms.Button btnDeleteAnimation;
        private System.Windows.Forms.Button btnRemoveFrame;
        private System.Windows.Forms.Button btnAddFrame;
        private System.Windows.Forms.Timer tmrAnimate;
        private System.Windows.Forms.Label lblFreeSpace;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnAddAnimation;
    }
}