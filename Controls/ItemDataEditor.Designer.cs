namespace Editroid
{
    partial class ItemDataEditor
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Elevator");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Brinstar", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Norfair", 2);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Tourian", 1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Ridley", 12);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Kraid", "up.png");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("aoeu");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("uea");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("\',.u");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(",.pi");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("pyd");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("23p");
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("pyd");
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(",.p");
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("pyd");
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("23p");
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem(".p");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemDataEditor));
            this.lstItems = new System.Windows.Forms.ListView();
            this.itemValueList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.EnemyEditor = new System.Windows.Forms.Panel();
            this.DifficultCheck = new System.Windows.Forms.CheckBox();
            this.EnemySlotNud = new System.Windows.Forms.NumericUpDown();
            this.EnemyTypeNud = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.EnemyEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EnemySlotNud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EnemyTypeNud)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(2, 2);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(31, 13);
            label1.TabIndex = 0;
            label1.Text = "Type";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(76, 2);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(25, 13);
            label2.TabIndex = 1;
            label2.Text = "Slot";
            // 
            // lstItems
            // 
            this.lstItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstItems.BackColor = System.Drawing.Color.Black;
            this.lstItems.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstItems.ForeColor = System.Drawing.Color.White;
            this.lstItems.FullRowSelect = true;
            this.lstItems.HideSelection = false;
            this.lstItems.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lstItems.Location = new System.Drawing.Point(2, 2);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(109, 251);
            this.lstItems.TabIndex = 0;
            this.lstItems.UseCompatibleStateImageBehavior = false;
            this.lstItems.View = System.Windows.Forms.View.List;
            this.lstItems.SelectedIndexChanged += new System.EventHandler(this.lstItems_SelectedIndexChanged);
            // 
            // itemValueList
            // 
            this.itemValueList.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.itemValueList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.itemValueList.BackColor = System.Drawing.Color.Black;
            this.itemValueList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.itemValueList.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemValueList.ForeColor = System.Drawing.Color.White;
            this.itemValueList.FullRowSelect = true;
            this.itemValueList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.itemValueList.HideSelection = false;
            this.itemValueList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17});
            this.itemValueList.Location = new System.Drawing.Point(113, 2);
            this.itemValueList.MultiSelect = false;
            this.itemValueList.Name = "itemValueList";
            this.itemValueList.Size = new System.Drawing.Size(141, 251);
            this.itemValueList.SmallImageList = this.images;
            this.itemValueList.TabIndex = 1;
            this.itemValueList.UseCompatibleStateImageBehavior = false;
            this.itemValueList.View = System.Windows.Forms.View.Details;
            this.itemValueList.SelectedIndexChanged += new System.EventHandler(this.itemValueList_SelectedIndexChanged);
            this.itemValueList.Click += new System.EventHandler(this.itemValueList_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 119;
            // 
            // images
            // 
            this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "bomb");
            this.images.Images.SetKeyName(1, "down");
            this.images.Images.SetKeyName(2, "downPal");
            this.images.Images.SetKeyName(3, "energyTank");
            this.images.Images.SetKeyName(4, "highJump");
            this.images.Images.SetKeyName(5, "iceBeam");
            this.images.Images.SetKeyName(6, "invalid");
            this.images.Images.SetKeyName(7, "longBeam");
            this.images.Images.SetKeyName(8, "maruMari");
            this.images.Images.SetKeyName(9, "missiles");
            this.images.Images.SetKeyName(10, "screwAttack");
            this.images.Images.SetKeyName(11, "up");
            this.images.Images.SetKeyName(12, "upPal");
            this.images.Images.SetKeyName(13, "varia");
            this.images.Images.SetKeyName(14, "waveBeam");
            // 
            // EnemyEditor
            // 
            this.EnemyEditor.Controls.Add(this.DifficultCheck);
            this.EnemyEditor.Controls.Add(this.EnemySlotNud);
            this.EnemyEditor.Controls.Add(this.EnemyTypeNud);
            this.EnemyEditor.Controls.Add(label2);
            this.EnemyEditor.Controls.Add(label1);
            this.EnemyEditor.Location = new System.Drawing.Point(114, 3);
            this.EnemyEditor.Name = "EnemyEditor";
            this.EnemyEditor.Size = new System.Drawing.Size(139, 249);
            this.EnemyEditor.TabIndex = 2;
            this.EnemyEditor.Visible = false;
            this.EnemyEditor.Paint += new System.Windows.Forms.PaintEventHandler(this.EnemyEditor_Paint);
            // 
            // DifficultCheck
            // 
            this.DifficultCheck.AutoSize = true;
            this.DifficultCheck.Location = new System.Drawing.Point(5, 56);
            this.DifficultCheck.Name = "DifficultCheck";
            this.DifficultCheck.Size = new System.Drawing.Size(61, 17);
            this.DifficultCheck.TabIndex = 4;
            this.DifficultCheck.Text = "Difficult";
            this.DifficultCheck.UseVisualStyleBackColor = true;
            this.DifficultCheck.CheckedChanged += new System.EventHandler(this.DifficultCheck_CheckedChanged);
            // 
            // EnemySlotNud
            // 
            this.EnemySlotNud.Location = new System.Drawing.Point(79, 18);
            this.EnemySlotNud.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.EnemySlotNud.Name = "EnemySlotNud";
            this.EnemySlotNud.Size = new System.Drawing.Size(57, 20);
            this.EnemySlotNud.TabIndex = 3;
            this.EnemySlotNud.ValueChanged += new System.EventHandler(this.EnemySlotNud_ValueChanged);
            // 
            // EnemyTypeNud
            // 
            this.EnemyTypeNud.Hexadecimal = true;
            this.EnemyTypeNud.Location = new System.Drawing.Point(4, 18);
            this.EnemyTypeNud.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.EnemyTypeNud.Name = "EnemyTypeNud";
            this.EnemyTypeNud.Size = new System.Drawing.Size(58, 20);
            this.EnemyTypeNud.TabIndex = 2;
            this.EnemyTypeNud.ValueChanged += new System.EventHandler(this.EnemyTypeNud_ValueChanged);
            // 
            // ItemDataEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.itemValueList);
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.EnemyEditor);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ItemDataEditor";
            this.Size = new System.Drawing.Size(256, 256);
            this.EnemyEditor.ResumeLayout(false);
            this.EnemyEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EnemySlotNud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EnemyTypeNud)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstItems;
        private System.Windows.Forms.ListView itemValueList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel EnemyEditor;
        private System.Windows.Forms.CheckBox DifficultCheck;
        private System.Windows.Forms.NumericUpDown EnemySlotNud;
        private System.Windows.Forms.NumericUpDown EnemyTypeNud;
        public System.Windows.Forms.ImageList images;

    }
}
