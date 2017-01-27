namespace Editroid
{
    partial class frmPointers
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.Splitter splitter2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPointers));
            this.PointerTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.PointerList = new System.Windows.Forms.ListView();
            this.ItemHeader = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolbar = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.HexDisplay = new System.Windows.Forms.TextBox();
            this.HexBoxHeader = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            splitter2 = new System.Windows.Forms.Splitter();
            this.toolbar.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Offset";
            columnHeader2.Width = 68;
            // 
            // splitter2
            // 
            splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitter2.Location = new System.Drawing.Point(0, 343);
            splitter2.Name = "splitter2";
            splitter2.Size = new System.Drawing.Size(683, 3);
            splitter2.TabIndex = 5;
            splitter2.TabStop = false;
            // 
            // PointerTree
            // 
            this.PointerTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.PointerTree.ImageIndex = 0;
            this.PointerTree.ImageList = this.imageList1;
            this.PointerTree.Location = new System.Drawing.Point(0, 25);
            this.PointerTree.Name = "PointerTree";
            this.PointerTree.SelectedImageIndex = 0;
            this.PointerTree.Size = new System.Drawing.Size(228, 318);
            this.PointerTree.TabIndex = 0;
            this.PointerTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.PointerTree_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList1.Images.SetKeyName(0, "ROM");
            this.imageList1.Images.SetKeyName(1, "Group.png");
            this.imageList1.Images.SetKeyName(2, "Palette");
            this.imageList1.Images.SetKeyName(3, "Patterns");
            this.imageList1.Images.SetKeyName(4, "Enemy");
            this.imageList1.Images.SetKeyName(5, "Doors");
            this.imageList1.Images.SetKeyName(6, "Struct");
            this.imageList1.Images.SetKeyName(7, "Misc");
            this.imageList1.Images.SetKeyName(8, "Screen");
            this.imageList1.Images.SetKeyName(9, "Combo");
            this.imageList1.Images.SetKeyName(10, "Level");
            this.imageList1.Images.SetKeyName(11, "Pointers");
            // 
            // PointerList
            // 
            this.PointerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ItemHeader,
            columnHeader2,
            this.columnHeader1,
            this.columnHeader3});
            this.PointerList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PointerList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PointerList.FullRowSelect = true;
            this.PointerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PointerList.HideSelection = false;
            this.PointerList.Location = new System.Drawing.Point(231, 25);
            this.PointerList.MultiSelect = false;
            this.PointerList.Name = "PointerList";
            this.PointerList.Size = new System.Drawing.Size(452, 318);
            this.PointerList.TabIndex = 1;
            this.PointerList.UseCompatibleStateImageBehavior = false;
            this.PointerList.View = System.Windows.Forms.View.Details;
            this.PointerList.SelectedIndexChanged += new System.EventHandler(this.PointerList_SelectedIndexChanged);
            // 
            // ItemHeader
            // 
            this.ItemHeader.Text = "Item";
            this.ItemHeader.Width = 167;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Size";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Data";
            this.columnHeader3.Width = 698;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(228, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 318);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // toolbar
            // 
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh});
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(683, 25);
            this.toolbar.TabIndex = 3;
            this.toolbar.Text = "toolStrip1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(65, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // HexDisplay
            // 
            this.HexDisplay.BackColor = System.Drawing.Color.White;
            this.HexDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HexDisplay.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HexDisplay.Location = new System.Drawing.Point(0, 20);
            this.HexDisplay.Multiline = true;
            this.HexDisplay.Name = "HexDisplay";
            this.HexDisplay.ReadOnly = true;
            this.HexDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.HexDisplay.Size = new System.Drawing.Size(683, 80);
            this.HexDisplay.TabIndex = 4;
            // 
            // HexBoxHeader
            // 
            this.HexBoxHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.HexBoxHeader.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HexBoxHeader.Location = new System.Drawing.Point(0, 0);
            this.HexBoxHeader.Name = "HexBoxHeader";
            this.HexBoxHeader.Size = new System.Drawing.Size(683, 20);
            this.HexBoxHeader.TabIndex = 6;
            this.HexBoxHeader.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.HexDisplay);
            this.panel1.Controls.Add(this.HexBoxHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 346);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(683, 100);
            this.panel1.TabIndex = 7;
            // 
            // frmPointers
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(683, 446);
            this.Controls.Add(this.PointerList);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.PointerTree);
            this.Controls.Add(this.toolbar);
            this.Controls.Add(splitter2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPointers";
            this.ShowInTaskbar = false;
            this.Text = "Pointer Explorer";
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView PointerTree;
        private System.Windows.Forms.ListView PointerList;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader ItemHeader;
        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox HexDisplay;
        private System.Windows.Forms.Label HexBoxHeader;
        private System.Windows.Forms.Panel panel1;
    }
}