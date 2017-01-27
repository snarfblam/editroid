namespace Editroid
{
    partial class PasswordDataGenerator
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Doors");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Brinstar Items");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Norfair Items");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Ridley Items");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Kraid Items");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Tourian Items");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Omitted Data");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordDataGenerator));
            this.dataList = new System.Windows.Forms.TreeView();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ItemLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataList
            // 
            this.dataList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataList.ImageIndex = 17;
            this.dataList.ImageList = this.images;
            this.dataList.Location = new System.Drawing.Point(4, 17);
            this.dataList.Name = "dataList";
            treeNode1.Name = "DoorNode";
            treeNode1.Text = "Doors";
            treeNode2.Name = "Brinstar";
            treeNode2.Text = "Brinstar Items";
            treeNode3.Name = "Norfair";
            treeNode3.Text = "Norfair Items";
            treeNode4.Name = "Ridley";
            treeNode4.Text = "Ridley Items";
            treeNode5.Name = "Kraid";
            treeNode5.Text = "Kraid Items";
            treeNode6.Name = "Tourian";
            treeNode6.Text = "Tourian Items";
            treeNode7.ForeColor = System.Drawing.SystemColors.GrayText;
            treeNode7.Name = "OmittedNode";
            treeNode7.Text = "Omitted Data";
            this.dataList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7});
            this.dataList.SelectedImageIndex = 17;
            this.dataList.Size = new System.Drawing.Size(274, 314);
            this.dataList.TabIndex = 0;
            this.dataList.DoubleClick += new System.EventHandler(this.dataList_DoubleClick);
            this.dataList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
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
            this.images.Images.SetKeyName(17, "VariableObject.png");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGenerate);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 331);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(274, 35);
            this.panel1.TabIndex = 1;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(161, 6);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(110, 23);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Generate Data";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(80, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ItemLabel
            // 
            this.ItemLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ItemLabel.Location = new System.Drawing.Point(4, 4);
            this.ItemLabel.Name = "ItemLabel";
            this.ItemLabel.Size = new System.Drawing.Size(274, 13);
            this.ItemLabel.TabIndex = 2;
            this.ItemLabel.Text = "32 Items";
            // 
            // PasswordDataGenerator
            // 
            this.AcceptButton = this.btnGenerate;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(282, 370);
            this.Controls.Add(this.dataList);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ItemLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PasswordDataGenerator";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "Password Data Generator";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView dataList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label ItemLabel;
        private System.Windows.Forms.ImageList images;
    }
}