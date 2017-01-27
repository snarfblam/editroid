namespace Editroid
{
    partial class PasswordDetailEditor
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Bombs", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("High Jump", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Long Beam", 2);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Screw Attack", 3);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Maru Mari", 4);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Varia", 5);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Wave Beam", 6);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Ice Beam", 7);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Energy Tank", 8);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Missiles", 9);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Door", 10);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Invalid", 11);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("Invalid", 11);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("Invalid", 11);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("Invalid", 11);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("Invalid", 11);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordDetailEditor));
            this.valueList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.entryList = new System.Windows.Forms.ListBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // valueList
            // 
            this.valueList.BackColor = System.Drawing.Color.Black;
            this.valueList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.valueList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.valueList.ForeColor = System.Drawing.Color.White;
            this.valueList.FullRowSelect = true;
            this.valueList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.valueList.HideSelection = false;
            this.valueList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
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
            listViewItem16});
            this.valueList.Location = new System.Drawing.Point(123, 1);
            this.valueList.MultiSelect = false;
            this.valueList.Name = "valueList";
            this.valueList.Size = new System.Drawing.Size(132, 254);
            this.valueList.SmallImageList = this.images;
            this.valueList.TabIndex = 1;
            this.valueList.UseCompatibleStateImageBehavior = false;
            this.valueList.View = System.Windows.Forms.View.Details;
            this.valueList.SelectedIndexChanged += new System.EventHandler(this.valueList_SelectedIndexChanged);
            this.valueList.SizeChanged += new System.EventHandler(this.valueList_SizeChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 100;
            // 
            // images
            // 
            this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "bomb.png");
            this.images.Images.SetKeyName(1, "highJump.png");
            this.images.Images.SetKeyName(2, "longBeam.png");
            this.images.Images.SetKeyName(3, "screwAttack.png");
            this.images.Images.SetKeyName(4, "maruMari.png");
            this.images.Images.SetKeyName(5, "varia.png");
            this.images.Images.SetKeyName(6, "waveBeam.png");
            this.images.Images.SetKeyName(7, "iceBeam.png");
            this.images.Images.SetKeyName(8, "energyTank.png");
            this.images.Images.SetKeyName(9, "missiles.png");
            this.images.Images.SetKeyName(10, "Door.png");
            this.images.Images.SetKeyName(11, "invalid.png");
            this.images.Images.SetKeyName(12, "invalid.png");
            this.images.Images.SetKeyName(13, "invalid.png");
            this.images.Images.SetKeyName(14, "invalid.png");
            this.images.Images.SetKeyName(15, "invalid.png");
            this.images.Images.SetKeyName(16, "invalid.png");
            // 
            // entryList
            // 
            this.entryList.BackColor = System.Drawing.Color.Black;
            this.entryList.Dock = System.Windows.Forms.DockStyle.Left;
            this.entryList.ForeColor = System.Drawing.Color.White;
            this.entryList.FormattingEnabled = true;
            this.entryList.IntegralHeight = false;
            this.entryList.Location = new System.Drawing.Point(1, 1);
            this.entryList.Name = "entryList";
            this.entryList.Size = new System.Drawing.Size(119, 254);
            this.entryList.TabIndex = 2;
            this.entryList.SelectedIndexChanged += new System.EventHandler(this.entryList_SelectedIndexChanged);
            // 
            // splitter1
            // 
            this.splitter1.Enabled = false;
            this.splitter1.Location = new System.Drawing.Point(120, 1);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 254);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // PasswordDetailEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.valueList);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.entryList);
            this.Name = "PasswordDetailEditor";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(256, 256);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView valueList;
        private System.Windows.Forms.ImageList images;
        private System.Windows.Forms.ListBox entryList;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}
