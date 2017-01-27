namespace Editroid
{
    partial class frmAdvancedItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdvancedItems));
            this.label3 = new System.Windows.Forms.Label();
            this.ItemList = new System.Windows.Forms.ListView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.btnDeleteItem = new System.Windows.Forms.Button();
            this.ItemMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.enemyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mellaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elevatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turretToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.motherBrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zebetiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rinkasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doorwayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paletteSwapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.picMap = new System.Windows.Forms.Panel();
            this.DataSizeLabel = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ScreenWarningLabel = new System.Windows.Forms.Label();
            this.btnAddScreen = new System.Windows.Forms.Button();
            this.btnRemoveScreen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddItem = new Microsoft.Samples.SplitButton();
            this.ItemMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(526, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Items";
            // 
            // ItemList
            // 
            this.ItemList.FullRowSelect = true;
            this.ItemList.HideSelection = false;
            this.ItemList.Location = new System.Drawing.Point(526, 93);
            this.ItemList.MultiSelect = false;
            this.ItemList.Name = "ItemList";
            this.ItemList.Size = new System.Drawing.Size(152, 157);
            this.ItemList.TabIndex = 5;
            this.ItemList.UseCompatibleStateImageBehavior = false;
            this.ItemList.View = System.Windows.Forms.View.List;
            this.ItemList.SelectedIndexChanged += new System.EventHandler(this.ItemList_SelectedIndexChanged);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CommandsVisibleIfAvailable = false;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(526, 277);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid1.Size = new System.Drawing.Size(152, 161);
            this.propertyGrid1.TabIndex = 7;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // btnDeleteItem
            // 
            this.btnDeleteItem.Enabled = false;
            this.btnDeleteItem.Image = global::Editroid.Properties.Resources.X;
            this.btnDeleteItem.Location = new System.Drawing.Point(654, 63);
            this.btnDeleteItem.Name = "btnDeleteItem";
            this.btnDeleteItem.Size = new System.Drawing.Size(24, 24);
            this.btnDeleteItem.TabIndex = 12;
            this.btnDeleteItem.UseVisualStyleBackColor = true;
            this.btnDeleteItem.Click += new System.EventHandler(this.btnDeleteItem_Click);
            // 
            // ItemMenu
            // 
            this.ItemMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enemyToolStripMenuItem,
            this.powerupToolStripMenuItem,
            this.mellaToolStripMenuItem,
            this.elevatorToolStripMenuItem,
            this.turretToolStripMenuItem,
            this.motherBrainToolStripMenuItem,
            this.zebetiteToolStripMenuItem,
            this.rinkasToolStripMenuItem,
            this.doorwayToolStripMenuItem,
            this.paletteSwapToolStripMenuItem});
            this.ItemMenu.Name = "contextMenuStrip1";
            this.ItemMenu.Size = new System.Drawing.Size(148, 224);
            // 
            // enemyToolStripMenuItem
            // 
            this.enemyToolStripMenuItem.Name = "enemyToolStripMenuItem";
            this.enemyToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.enemyToolStripMenuItem.Text = "Enemy";
            this.enemyToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // powerupToolStripMenuItem
            // 
            this.powerupToolStripMenuItem.Name = "powerupToolStripMenuItem";
            this.powerupToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.powerupToolStripMenuItem.Text = "Power-up";
            this.powerupToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // mellaToolStripMenuItem
            // 
            this.mellaToolStripMenuItem.Name = "mellaToolStripMenuItem";
            this.mellaToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.mellaToolStripMenuItem.Text = "Mella";
            this.mellaToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // elevatorToolStripMenuItem
            // 
            this.elevatorToolStripMenuItem.Name = "elevatorToolStripMenuItem";
            this.elevatorToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.elevatorToolStripMenuItem.Text = "Elevator";
            this.elevatorToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // turretToolStripMenuItem
            // 
            this.turretToolStripMenuItem.Name = "turretToolStripMenuItem";
            this.turretToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.turretToolStripMenuItem.Text = "Turret";
            this.turretToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // motherBrainToolStripMenuItem
            // 
            this.motherBrainToolStripMenuItem.Name = "motherBrainToolStripMenuItem";
            this.motherBrainToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.motherBrainToolStripMenuItem.Text = "Mother Brain";
            this.motherBrainToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // zebetiteToolStripMenuItem
            // 
            this.zebetiteToolStripMenuItem.Name = "zebetiteToolStripMenuItem";
            this.zebetiteToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.zebetiteToolStripMenuItem.Text = "Zebetite";
            this.zebetiteToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // rinkasToolStripMenuItem
            // 
            this.rinkasToolStripMenuItem.Name = "rinkasToolStripMenuItem";
            this.rinkasToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.rinkasToolStripMenuItem.Text = "Rinkas";
            this.rinkasToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // doorwayToolStripMenuItem
            // 
            this.doorwayToolStripMenuItem.Name = "doorwayToolStripMenuItem";
            this.doorwayToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.doorwayToolStripMenuItem.Text = "Doorway";
            this.doorwayToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // paletteSwapToolStripMenuItem
            // 
            this.paletteSwapToolStripMenuItem.Name = "paletteSwapToolStripMenuItem";
            this.paletteSwapToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.paletteSwapToolStripMenuItem.Text = "Palette swap";
            this.paletteSwapToolStripMenuItem.Click += new System.EventHandler(this.OnItemMenuClick);
            // 
            // picMap
            // 
            this.picMap.Location = new System.Drawing.Point(8, 46);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(512, 512);
            this.picMap.TabIndex = 18;
            // 
            // DataSizeLabel
            // 
            this.DataSizeLabel.AutoSize = true;
            this.DataSizeLabel.Location = new System.Drawing.Point(529, 492);
            this.DataSizeLabel.Name = "DataSizeLabel";
            this.DataSizeLabel.Size = new System.Drawing.Size(99, 26);
            this.DataSizeLabel.TabIndex = 19;
            this.DataSizeLabel.Tag = "Using {1} of {3} bytes (original: {2} bytes)";
            this.DataSizeLabel.Text = "{1} Bytes | {2}\r\n{3} Bytes Available";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(607, 535);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 20;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(526, 535);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ScreenWarningLabel
            // 
            this.ScreenWarningLabel.AutoSize = true;
            this.ScreenWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.ScreenWarningLabel.Location = new System.Drawing.Point(526, 473);
            this.ScreenWarningLabel.Name = "ScreenWarningLabel";
            this.ScreenWarningLabel.Size = new System.Drawing.Size(113, 13);
            this.ScreenWarningLabel.TabIndex = 22;
            this.ScreenWarningLabel.Text = "* Screens overlapping";
            // 
            // btnAddScreen
            // 
            this.btnAddScreen.Location = new System.Drawing.Point(8, 12);
            this.btnAddScreen.Name = "btnAddScreen";
            this.btnAddScreen.Size = new System.Drawing.Size(75, 23);
            this.btnAddScreen.TabIndex = 23;
            this.btnAddScreen.Text = "Add Screen";
            this.btnAddScreen.UseVisualStyleBackColor = true;
            this.btnAddScreen.Click += new System.EventHandler(this.btnAddScreen_Click);
            // 
            // btnRemoveScreen
            // 
            this.btnRemoveScreen.Enabled = false;
            this.btnRemoveScreen.Location = new System.Drawing.Point(89, 12);
            this.btnRemoveScreen.Name = "btnRemoveScreen";
            this.btnRemoveScreen.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveScreen.TabIndex = 24;
            this.btnRemoveScreen.Text = "Remove Screen";
            this.btnRemoveScreen.UseVisualStyleBackColor = true;
            this.btnRemoveScreen.Click += new System.EventHandler(this.btnRemoveScreen_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(526, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Selected Item";
            // 
            // btnAddItem
            // 
            this.btnAddItem.AutoSize = true;
            this.btnAddItem.ContextMenuStrip = this.ItemMenu;
            this.btnAddItem.Enabled = false;
            this.btnAddItem.Image = ((System.Drawing.Image)(resources.GetObject("btnAddItem.Image")));
            this.btnAddItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddItem.Location = new System.Drawing.Point(526, 62);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(122, 25);
            this.btnAddItem.TabIndex = 14;
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // frmAdvancedItems
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(685, 567);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemoveScreen);
            this.Controls.Add(this.btnAddScreen);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.ScreenWarningLabel);
            this.Controls.Add(this.picMap);
            this.Controls.Add(this.DataSizeLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.btnDeleteItem);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.ItemList);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAdvancedItems";
            this.Text = "Item Editor";
            this.ItemMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView ItemList;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button btnDeleteItem;
        private Microsoft.Samples.SplitButton btnAddItem;
        private System.Windows.Forms.ContextMenuStrip ItemMenu;
        private System.Windows.Forms.ToolStripMenuItem enemyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem powerupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mellaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elevatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem turretToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motherBrainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zebetiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rinkasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doorwayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paletteSwapToolStripMenuItem;
        private System.Windows.Forms.Panel picMap;
        private System.Windows.Forms.Label DataSizeLabel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label ScreenWarningLabel;
        private System.Windows.Forms.Button btnAddScreen;
        private System.Windows.Forms.Button btnRemoveScreen;
        private System.Windows.Forms.Label label1;
    }
}