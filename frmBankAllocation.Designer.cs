namespace Editroid
{
    partial class frmBankAllocation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBankAllocation));
            this.lstBanks = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReserve = new System.Windows.Forms.Button();
            this.lblDetails = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstBanks
            // 
            this.lstBanks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBanks.ColumnWidth = 240;
            this.lstBanks.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstBanks.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstBanks.FormattingEnabled = true;
            this.lstBanks.IntegralHeight = false;
            this.lstBanks.ItemHeight = 20;
            this.lstBanks.Items.AddRange(new object[] {
            "Bank 00",
            "Bank 01",
            "Bank 02",
            "Bank 03",
            "Bank 04",
            "Bank 05",
            "Bank 06",
            "Bank 07",
            "Bank 08",
            "Bank 09",
            "Bank 0A",
            "Bank 0B",
            "Bank 0C",
            "Bank 0D",
            "Bank 0E",
            "Bank 0F",
            "Bank 10",
            "Bank 11",
            "Bank 12",
            "Bank 13",
            "Bank 14",
            "Bank 15",
            "Bank 16",
            "Bank 17",
            "Bank 18",
            "Bank 19",
            "Bank 1A",
            "Bank 1B",
            "Bank 1C",
            "Bank 1D",
            "Bank 1E",
            "Bank 1F",
            "Bank 00",
            "Bank 01",
            "Bank 02",
            "Bank 03",
            "Bank 04",
            "Bank 05",
            "Bank 06",
            "Bank 07",
            "Bank 08",
            "Bank 09",
            "Bank 0A",
            "Bank 0B",
            "Bank 0C",
            "Bank 0D",
            "Bank 0E",
            "Bank 0F",
            "Bank 10",
            "Bank 11",
            "Bank 12",
            "Bank 13",
            "Bank 14",
            "Bank 15",
            "Bank 16",
            "Bank 17",
            "Bank 18",
            "Bank 19",
            "Bank 1A",
            "Bank 1B",
            "Bank 1C",
            "Bank 1D",
            "Bank 1E",
            "Bank 1F"});
            this.lstBanks.Location = new System.Drawing.Point(12, 69);
            this.lstBanks.MultiColumn = true;
            this.lstBanks.Name = "lstBanks";
            this.lstBanks.Size = new System.Drawing.Size(515, 379);
            this.lstBanks.TabIndex = 0;
            this.lstBanks.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstBanks_DrawItem);
            this.lstBanks.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lstBanks_MeasureItem);
            this.lstBanks.SelectedIndexChanged += new System.EventHandler(this.lstBanks_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(515, 57);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnReserve
            // 
            this.btnReserve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReserve.Enabled = false;
            this.btnReserve.Location = new System.Drawing.Point(12, 454);
            this.btnReserve.Name = "btnReserve";
            this.btnReserve.Size = new System.Drawing.Size(95, 23);
            this.btnReserve.TabIndex = 2;
            this.btnReserve.Text = "Reserve";
            this.btnReserve.UseVisualStyleBackColor = true;
            this.btnReserve.Click += new System.EventHandler(this.btnReserve_Click);
            // 
            // lblDetails
            // 
            this.lblDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDetails.Location = new System.Drawing.Point(113, 454);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(256, 23);
            this.lblDetails.TabIndex = 3;
            this.lblDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(432, 454);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(95, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "OK";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmBankAllocation
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 489);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.btnReserve);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstBanks);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 240);
            this.Name = "frmBankAllocation";
            this.Text = "Bank Allocation";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstBanks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReserve;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Button btnClose;
    }
}