namespace Editroid
{
    partial class frmChrSelect
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
            this.pnlScroller = new System.Windows.Forms.Panel();
            this.picTiles = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.bntCancel = new System.Windows.Forms.Button();
            this.pnlScroller.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTiles)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlScroller
            // 
            this.pnlScroller.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlScroller.AutoScroll = true;
            this.pnlScroller.Controls.Add(this.picTiles);
            this.pnlScroller.Location = new System.Drawing.Point(0, 0);
            this.pnlScroller.Name = "pnlScroller";
            this.pnlScroller.Size = new System.Drawing.Size(273, 494);
            this.pnlScroller.TabIndex = 2;
            // 
            // picTiles
            // 
            this.picTiles.BackColor = System.Drawing.Color.Black;
            this.picTiles.Location = new System.Drawing.Point(0, 0);
            this.picTiles.Name = "picTiles";
            this.picTiles.Size = new System.Drawing.Size(256, 519);
            this.picTiles.TabIndex = 2;
            this.picTiles.TabStop = false;
            this.picTiles.DoubleClick += new System.EventHandler(this.picTiles_DoubleClick);
            this.picTiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTiles_MouseDown);
            this.picTiles.Paint += new System.Windows.Forms.PaintEventHandler(this.picTiles_Paint);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(186, 500);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // bntCancel
            // 
            this.bntCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bntCancel.Location = new System.Drawing.Point(105, 500);
            this.bntCancel.Name = "bntCancel";
            this.bntCancel.Size = new System.Drawing.Size(75, 23);
            this.bntCancel.TabIndex = 4;
            this.bntCancel.Text = "Cancel";
            this.bntCancel.UseVisualStyleBackColor = true;
            this.bntCancel.Click += new System.EventHandler(this.bntCancel_Click);
            // 
            // frmChrSelect
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bntCancel;
            this.ClientSize = new System.Drawing.Size(273, 531);
            this.Controls.Add(this.bntCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlScroller);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChrSelect";
            this.Text = "CHR Select";
            this.pnlScroller.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlScroller;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button bntCancel;
        private System.Windows.Forms.PictureBox picTiles;

    }
}