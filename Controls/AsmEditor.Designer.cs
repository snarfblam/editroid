namespace MultiMaster.Editors
{
    partial class AsmEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewCollapse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.increaseIndentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decreaseIndentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuGotolabel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.collapseSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnEditorMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.EditBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.menuStrip1.SuspendLayout();
            this.cmnEditorMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(494, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewCollapse});
            this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // mnuViewCollapse
            // 
            this.mnuViewCollapse.Name = "mnuViewCollapse";
            this.mnuViewCollapse.Size = new System.Drawing.Size(170, 22);
            this.mnuViewCollapse.Text = "Collapse Selection";
            this.mnuViewCollapse.Click += new System.EventHandler(this.mnuViewCollapse_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.mnuSelectAll,
            this.toolStripSeparator2,
            this.increaseIndentToolStripMenuItem,
            this.decreaseIndentToolStripMenuItem,
            this.toolStripSeparator3,
            this.commentToolStripMenuItem,
            this.uncommentToolStripMenuItem,
            this.toolStripSeparator4,
            this.mnuGotolabel,
            this.toolStripSeparator5,
            this.collapseSelectionToolStripMenuItem});
            this.toolStripMenuItem1.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(39, 20);
            this.toolStripMenuItem1.Text = "&Edit";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(214, 6);
            // 
            // mnuSelectAll
            // 
            this.mnuSelectAll.Name = "mnuSelectAll";
            this.mnuSelectAll.Size = new System.Drawing.Size(217, 22);
            this.mnuSelectAll.Text = "Select All";
            this.mnuSelectAll.Click += new System.EventHandler(this.mnuSelectAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(214, 6);
            // 
            // increaseIndentToolStripMenuItem
            // 
            this.increaseIndentToolStripMenuItem.Name = "increaseIndentToolStripMenuItem";
            this.increaseIndentToolStripMenuItem.ShortcutKeyDisplayString = "Tab";
            this.increaseIndentToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.increaseIndentToolStripMenuItem.Text = "Increase Indent";
            this.increaseIndentToolStripMenuItem.Click += new System.EventHandler(this.increaseIndentToolStripMenuItem_Click);
            // 
            // decreaseIndentToolStripMenuItem
            // 
            this.decreaseIndentToolStripMenuItem.Name = "decreaseIndentToolStripMenuItem";
            this.decreaseIndentToolStripMenuItem.ShortcutKeyDisplayString = "Shift+Tab";
            this.decreaseIndentToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.decreaseIndentToolStripMenuItem.Text = "Decrease Indent";
            this.decreaseIndentToolStripMenuItem.Click += new System.EventHandler(this.decreaseIndentToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(214, 6);
            // 
            // commentToolStripMenuItem
            // 
            this.commentToolStripMenuItem.Name = "commentToolStripMenuItem";
            this.commentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.commentToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.commentToolStripMenuItem.Text = "Comment";
            this.commentToolStripMenuItem.Click += new System.EventHandler(this.commentToolStripMenuItem_Click);
            // 
            // uncommentToolStripMenuItem
            // 
            this.uncommentToolStripMenuItem.Name = "uncommentToolStripMenuItem";
            this.uncommentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.U)));
            this.uncommentToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.uncommentToolStripMenuItem.Text = "Uncomment";
            this.uncommentToolStripMenuItem.Click += new System.EventHandler(this.uncommentToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(214, 6);
            // 
            // mnuGotolabel
            // 
            this.mnuGotolabel.Name = "mnuGotolabel";
            this.mnuGotolabel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.mnuGotolabel.Size = new System.Drawing.Size(217, 22);
            this.mnuGotolabel.Text = "GoTo Label...";
            this.mnuGotolabel.Click += new System.EventHandler(this.mnuGotolabel_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(214, 6);
            // 
            // collapseSelectionToolStripMenuItem
            // 
            this.collapseSelectionToolStripMenuItem.Name = "collapseSelectionToolStripMenuItem";
            this.collapseSelectionToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.collapseSelectionToolStripMenuItem.Text = "Collapse Selection";
            this.collapseSelectionToolStripMenuItem.Click += new System.EventHandler(this.collapseSelectionToolStripMenuItem_Click);
            // 
            // cmnEditorMenu
            // 
            this.cmnEditorMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCut,
            this.mnuCopy,
            this.mnuPaste});
            this.cmnEditorMenu.Name = "cmnEditorMenu";
            this.cmnEditorMenu.Size = new System.Drawing.Size(103, 70);
            this.cmnEditorMenu.Opening += new System.ComponentModel.CancelEventHandler(this.cmnEditorMenu_Opening);
            // 
            // mnuCut
            // 
            this.mnuCut.Name = "mnuCut";
            this.mnuCut.Size = new System.Drawing.Size(102, 22);
            this.mnuCut.Text = "Cut";
            this.mnuCut.Click += new System.EventHandler(this.mnuCut_Click);
            // 
            // mnuCopy
            // 
            this.mnuCopy.Name = "mnuCopy";
            this.mnuCopy.Size = new System.Drawing.Size(102, 22);
            this.mnuCopy.Text = "Copy";
            this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
            // 
            // mnuPaste
            // 
            this.mnuPaste.Name = "mnuPaste";
            this.mnuPaste.Size = new System.Drawing.Size(102, 22);
            this.mnuPaste.Text = "Paste";
            this.mnuPaste.Click += new System.EventHandler(this.mnuPaste_Click);
            // 
            // EditBox
            // 
            this.EditBox.AutoIndentOnEnterOnly = true;
            this.EditBox.AutoScrollMinSize = new System.Drawing.Size(135, 15);
            this.EditBox.CommentPrefix = ";";
            this.EditBox.ContextMenuStrip = this.cmnEditorMenu;
            this.EditBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.EditBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditBox.Location = new System.Drawing.Point(0, 24);
            this.EditBox.Name = "EditBox";
            this.EditBox.PreferredLineWidth = 0;
            this.EditBox.Size = new System.Drawing.Size(494, 329);
            this.EditBox.TabIndex = 0;
            this.EditBox.Text = "fastColoredTextBox1";
            this.EditBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditBox_KeyPress);
            this.EditBox.FindFormShow += new System.EventHandler(this.EditBox_FindFormShow);
            this.EditBox.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.EditBox_TextChangedDelayed);
            this.EditBox.SelectionChanged += new System.EventHandler(this.EditBox_SelectionChanged);
            this.EditBox.AutoIndentNeeded += new System.EventHandler<FastColoredTextBoxNS.AutoIndentEventArgs>(this.EditBox_AutoIndentNeeded);
            this.EditBox.FindFormClose += new System.EventHandler(this.EditBox_FindFormClose);
            this.EditBox.KeyPressing += new System.Windows.Forms.KeyPressEventHandler(this.EditBox_KeyPressing);
            this.EditBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.EditBox_TextChanged);
            this.EditBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditBox_KeyDown);
            // 
            // AsmEditor
            // 
            this.Controls.Add(this.EditBox);
            this.Controls.Add(this.menuStrip1);
            this.Padding = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.Size = new System.Drawing.Size(494, 353);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.cmnEditorMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox EditBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCollapse;
        private System.Windows.Forms.ContextMenuStrip cmnEditorMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuCut;
        private System.Windows.Forms.ToolStripMenuItem mnuCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuPaste;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem increaseIndentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decreaseIndentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem commentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncommentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mnuGotolabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem collapseSelectionToolStripMenuItem;

    }
}
