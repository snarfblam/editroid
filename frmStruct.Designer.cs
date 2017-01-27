namespace Editroid
{
    internal partial class frmStruct
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStruct));
            this.panel1 = new System.Windows.Forms.Panel();
            this.picStruct = new System.Windows.Forms.PictureBox();
            this.cmnStructContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblComboData = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuShiftLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShiftRight = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTile = new System.Windows.Forms.Label();
            this.lblFreeMem = new System.Windows.Forms.Label();
            this.NeededBytesLabel = new System.Windows.Forms.Label();
            this.tsTiles = new Editroid.TileSelector();
            this.comboEditor = new Editroid.ComboEditor();
            this.tileEditor1 = new Editroid.TileEditor();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStruct)).BeginInit();
            this.cmnStructContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tsTiles)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.picStruct);
            this.panel1.Location = new System.Drawing.Point(540, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(256, 240);
            this.panel1.TabIndex = 2;
            // 
            // picStruct
            // 
            this.picStruct.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picStruct.BackgroundImage")));
            this.picStruct.ContextMenuStrip = this.cmnStructContext;
            this.picStruct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picStruct.Image = global::Editroid.Properties.Resources.StructBG;
            this.picStruct.Location = new System.Drawing.Point(0, 0);
            this.picStruct.Name = "picStruct";
            this.picStruct.Size = new System.Drawing.Size(256, 240);
            this.picStruct.TabIndex = 0;
            this.picStruct.TabStop = false;
            this.picStruct.MouseLeave += new System.EventHandler(this.picStruct_MouseLeave);
            this.picStruct.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picStruct_MouseMove);
            this.picStruct.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picStruct_MouseDown);
            // 
            // cmnStructContext
            // 
            this.cmnStructContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblComboData,
            this.btnDelete,
            this.toolStripSeparator4,
            this.mnuShiftLeft,
            this.mnuShiftRight});
            this.cmnStructContext.Name = "contextMenuStrip1";
            this.cmnStructContext.Size = new System.Drawing.Size(157, 98);
            this.cmnStructContext.Opening += new System.ComponentModel.CancelEventHandler(this.cmnStructContext_Opening);
            // 
            // lblComboData
            // 
            this.lblComboData.Name = "lblComboData";
            this.lblComboData.Size = new System.Drawing.Size(156, 22);
            this.lblComboData.Text = "[00] Intangible";
            this.lblComboData.Click += new System.EventHandler(this.lblComboData_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::Editroid.Properties.Resources.X;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(156, 22);
            this.btnDelete.Text = "Delete Tile";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(153, 6);
            this.toolStripSeparator4.Visible = false;
            // 
            // mnuShiftLeft
            // 
            this.mnuShiftLeft.Enabled = false;
            this.mnuShiftLeft.Image = ((System.Drawing.Image)(resources.GetObject("mnuShiftLeft.Image")));
            this.mnuShiftLeft.Name = "mnuShiftLeft";
            this.mnuShiftLeft.Size = new System.Drawing.Size(156, 22);
            this.mnuShiftLeft.Text = "Shift Left";
            this.mnuShiftLeft.Visible = false;
            // 
            // mnuShiftRight
            // 
            this.mnuShiftRight.Enabled = false;
            this.mnuShiftRight.Image = ((System.Drawing.Image)(resources.GetObject("mnuShiftRight.Image")));
            this.mnuShiftRight.Name = "mnuShiftRight";
            this.mnuShiftRight.Size = new System.Drawing.Size(156, 22);
            this.mnuShiftRight.Text = "Shift Right";
            this.mnuShiftRight.Visible = false;
            // 
            // lblTile
            // 
            this.lblTile.AutoSize = true;
            this.lblTile.ForeColor = System.Drawing.Color.White;
            this.lblTile.Location = new System.Drawing.Point(-3, 259);
            this.lblTile.Name = "lblTile";
            this.lblTile.Size = new System.Drawing.Size(45, 13);
            this.lblTile.TabIndex = 22;
            this.lblTile.Text = "00 Solid";
            // 
            // lblFreeMem
            // 
            this.lblFreeMem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFreeMem.ForeColor = System.Drawing.Color.White;
            this.lblFreeMem.Image = global::Editroid.Properties.Resources.Binary16;
            this.lblFreeMem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFreeMem.Location = new System.Drawing.Point(537, 242);
            this.lblFreeMem.Name = "lblFreeMem";
            this.lblFreeMem.Size = new System.Drawing.Size(111, 23);
            this.lblFreeMem.TabIndex = 23;
            this.lblFreeMem.Text = "0 free bytes";
            this.lblFreeMem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NeededBytesLabel
            // 
            this.NeededBytesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NeededBytesLabel.ForeColor = System.Drawing.Color.White;
            this.NeededBytesLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.NeededBytesLabel.Location = new System.Drawing.Point(654, 242);
            this.NeededBytesLabel.Name = "NeededBytesLabel";
            this.NeededBytesLabel.Size = new System.Drawing.Size(142, 23);
            this.NeededBytesLabel.TabIndex = 25;
            this.NeededBytesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tsTiles
            // 
            this.tsTiles.Location = new System.Drawing.Point(0, 0);
            this.tsTiles.Name = "tsTiles";
            this.tsTiles.Size = new System.Drawing.Size(256, 256);
            this.tsTiles.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tsTiles.TabIndex = 20;
            this.tsTiles.TabStop = false;
            this.tsTiles.Paint += new System.Windows.Forms.PaintEventHandler(this.tsTiles_Paint);
            this.tsTiles.TileSelected += new Editroid.TileSelector.IndexEvent(this.tsTiles_TileSelected);
            // 
            // comboEditor
            // 
            this.comboEditor.BackColor = System.Drawing.Color.Black;
            this.comboEditor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("comboEditor.BackgroundImage")));
            this.comboEditor.CurrentTile = ((byte)(0));
            this.comboEditor.Level = Editroid.LevelIndex.Brinstar;
            this.comboEditor.Location = new System.Drawing.Point(270, 0);
            this.comboEditor.Name = "comboEditor";
            this.comboEditor.PaletteTable = 0;
            this.comboEditor.ScrollPosition = 0;
            this.comboEditor.SelectedCombo = 0;
            this.comboEditor.Size = new System.Drawing.Size(256, 256);
            this.comboEditor.TabIndex = 21;
            this.comboEditor.UserGrabbedCombo += new System.EventHandler(this.comboEditor_UserGrabbedCombo);
            this.comboEditor.ComboEdited += new System.EventHandler(this.comboEditor_ComboEdited);
            // 
            // tileEditor1
            // 
            this.tileEditor1.BackColor = System.Drawing.Color.White;
            this.tileEditor1.Location = new System.Drawing.Point(8, 161);
            this.tileEditor1.Name = "tileEditor1";
            this.tileEditor1.Rom = null;
            this.tileEditor1.SelectedColor = 0;
            this.tileEditor1.Size = new System.Drawing.Size(64, 64);
            this.tileEditor1.TabIndex = 24;
            this.tileEditor1.Text = "tileEditor1";
            this.tileEditor1.Paint += new System.Windows.Forms.PaintEventHandler(this.tileEditor1_Paint);
            // 
            // frmStruct
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(797, 276);
            this.Controls.Add(this.tsTiles);
            this.Controls.Add(this.NeededBytesLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblFreeMem);
            this.Controls.Add(this.comboEditor);
            this.Controls.Add(this.tileEditor1);
            this.Controls.Add(this.lblTile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStruct";
            this.ShowInTaskbar = false;
            this.Text = "Structure Editor";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picStruct)).EndInit();
            this.cmnStructContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tsTiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picStruct;
        private System.Windows.Forms.ContextMenuStrip cmnStructContext;
        private System.Windows.Forms.ToolStripMenuItem mnuShiftLeft;
        private System.Windows.Forms.ToolStripMenuItem mnuShiftRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Label lblTile;
        private TileSelector tsTiles;
        private ComboEditor comboEditor;
        private System.Windows.Forms.ToolStripMenuItem lblComboData;
        private System.Windows.Forms.ToolStripMenuItem btnDelete;
        private System.Windows.Forms.Label lblFreeMem;
        private TileEditor tileEditor1;
        private System.Windows.Forms.Label NeededBytesLabel;
    }
}