namespace Editroid
{
    partial class frmCodeEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCodeEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.FileMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.btnIndent = new System.Windows.Forms.ToolStripButton();
            this.btnUnindent = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.FileExporter = new System.Windows.Forms.SaveFileDialog();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.chkAutoBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.editor = new MultiMaster.Editors.AsmEditor();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.FileMenuSeparator,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator1,
            this.btnUndo,
            this.btnRedo,
            this.toolStripSeparator2,
            this.toolStripButton1,
            this.btnIndent,
            this.btnUnindent,
            this.toolStripSeparator3,
            this.toolStripButton3,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(538, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkAutoBuild,
            this.toolStripSeparator5,
            this.mnuRename,
            this.mnuExport,
            this.toolStripSeparator4,
            this.mnuDelete});
            this.mnuFile.Image = ((System.Drawing.Image)(resources.GetObject("mnuFile.Image")));
            this.mnuFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(29, 22);
            this.mnuFile.Text = "toolStripButton4";
            this.mnuFile.Visible = false;
            // 
            // mnuRename
            // 
            this.mnuRename.Name = "mnuRename";
            this.mnuRename.Size = new System.Drawing.Size(152, 22);
            this.mnuRename.Text = "Rename...";
            this.mnuRename.Click += new System.EventHandler(this.mnuRename_Click);
            // 
            // mnuExport
            // 
            this.mnuExport.Name = "mnuExport";
            this.mnuExport.Size = new System.Drawing.Size(152, 22);
            this.mnuExport.Text = "Export...";
            this.mnuExport.Click += new System.EventHandler(this.mnuExport_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(152, 22);
            this.mnuDelete.Text = "Delete";
            this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
            // 
            // FileMenuSeparator
            // 
            this.FileMenuSeparator.Name = "FileMenuSeparator";
            this.FileMenuSeparator.Size = new System.Drawing.Size(6, 25);
            this.FileMenuSeparator.Visible = false;
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "C&ut";
            this.cutToolStripButton.Click += new System.EventHandler(this.cutToolStripButton_Click);
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copyToolStripButton.Text = "&Copy";
            this.copyToolStripButton.Click += new System.EventHandler(this.copyToolStripButton_Click);
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pasteToolStripButton.Text = "&Paste";
            this.pasteToolStripButton.Click += new System.EventHandler(this.pasteToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(23, 22);
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("btnRedo.Image")));
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(23, 22);
            this.btnRedo.Text = "Redo";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Toggle Selection Comment";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // btnIndent
            // 
            this.btnIndent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnIndent.Image = ((System.Drawing.Image)(resources.GetObject("btnIndent.Image")));
            this.btnIndent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIndent.Name = "btnIndent";
            this.btnIndent.Size = new System.Drawing.Size(23, 22);
            this.btnIndent.Text = "Indent Selection";
            this.btnIndent.Click += new System.EventHandler(this.btnIndent_Click);
            // 
            // btnUnindent
            // 
            this.btnUnindent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUnindent.Image = ((System.Drawing.Image)(resources.GetObject("btnUnindent.Image")));
            this.btnUnindent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnindent.Name = "btnUnindent";
            this.btnUnindent.Size = new System.Drawing.Size(23, 22);
            this.btnUnindent.Text = "Unindent Selectin";
            this.btnUnindent.Click += new System.EventHandler(this.btnUnindent_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(43, 22);
            this.toolStripButton3.Text = "Find...";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(61, 22);
            this.toolStripButton2.Text = "Replace...";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // FileExporter
            // 
            this.FileExporter.Filter = "Assembly (*.asm)|*.asm|All files (*.*)|*.*";
            this.FileExporter.Title = "Export ASM";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(149, 6);
            // 
            // chkAutoBuild
            // 
            this.chkAutoBuild.Name = "chkAutoBuild";
            this.chkAutoBuild.Size = new System.Drawing.Size(152, 22);
            this.chkAutoBuild.Text = "Auto-build";
            this.chkAutoBuild.Click += new System.EventHandler(this.chkAutoBuild_Click);
            // 
            // editor
            // 
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.Location = new System.Drawing.Point(0, 25);
            this.editor.Name = "editor";
            this.editor.Padding = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.editor.Size = new System.Drawing.Size(538, 253);
            this.editor.TabIndex = 0;
            this.editor.Text = "fastColoredTextBox1";
            this.editor.UndoRedoChanged += new System.EventHandler(this.editor_UndoRedoChanged);
            // 
            // frmCodeEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(538, 278);
            this.Controls.Add(this.editor);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "frmCodeEditor";
            this.ShowInTaskbar = false;
            this.Text = "Code Editor";
            this.Deactivate += new System.EventHandler(this.frmCodeEditor_Deactivate);
            this.Load += new System.EventHandler(this.frmCodeEditor_Load);
            this.Activated += new System.EventHandler(this.frmCodeEditor_Activated);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MultiMaster.Editors.AsmEditor editor;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton btnIndent;
        private System.Windows.Forms.ToolStripButton btnUnindent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripDropDownButton mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuRename;
        private System.Windows.Forms.ToolStripMenuItem mnuExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.Windows.Forms.ToolStripSeparator FileMenuSeparator;
        private System.Windows.Forms.SaveFileDialog FileExporter;
        private System.Windows.Forms.ToolStripMenuItem chkAutoBuild;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    }
}