namespace Editroid
{
    partial class frmHelp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHelp));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Help Home");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Keyboard Shortcuts");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Editor Map");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Item Editor");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Palette Editor");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Password Data Editor");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Screen Editor");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Structure Editor");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Editors", 2, 2, new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("ROM Tips And Guidelines");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Editor Tips");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Tips", 2, 2, new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11});
            this.tabs = new System.Windows.Forms.TabControl();
            this.specialVersionTab = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rtfAbout = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabGettingStarted = new System.Windows.Forms.TabPage();
            this.GettingStartedHelp = new System.Windows.Forms.WebBrowser();
            this.tabHelp = new System.Windows.Forms.TabPage();
            this.helpBrowser = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.HelpTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.FeedbackTab = new System.Windows.Forms.TabPage();
            this.radQuestion = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.chkEmailReply = new System.Windows.Forms.CheckBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtFeedback = new System.Windows.Forms.TextBox();
            this.radBug = new System.Windows.Forms.RadioButton();
            this.radComment = new System.Windows.Forms.RadioButton();
            this.radSuggestion = new System.Windows.Forms.RadioButton();
            this.FeedbackProgress = new System.Windows.Forms.ProgressBar();
            this.FeedbackWorker = new System.ComponentModel.BackgroundWorker();
            this.tabs.SuspendLayout();
            this.specialVersionTab.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabGettingStarted.SuspendLayout();
            this.tabHelp.SuspendLayout();
            this.FeedbackTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.specialVersionTab);
            this.tabs.Controls.Add(this.tabAbout);
            this.tabs.Controls.Add(this.tabGettingStarted);
            this.tabs.Controls.Add(this.tabHelp);
            this.tabs.Controls.Add(this.FeedbackTab);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.ImageList = this.imageList1;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(631, 427);
            this.tabs.TabIndex = 0;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // specialVersionTab
            // 
            this.specialVersionTab.Controls.Add(this.textBox1);
            this.specialVersionTab.Location = new System.Drawing.Point(4, 23);
            this.specialVersionTab.Name = "specialVersionTab";
            this.specialVersionTab.Size = new System.Drawing.Size(623, 400);
            this.specialVersionTab.TabIndex = 4;
            this.specialVersionTab.Text = "2.0 Alpha";
            this.specialVersionTab.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(623, 400);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            this.textBox1.WordWrap = false;
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.panel1);
            this.tabAbout.Controls.Add(this.pictureBox1);
            this.tabAbout.Location = new System.Drawing.Point(4, 23);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbout.Size = new System.Drawing.Size(623, 400);
            this.tabAbout.TabIndex = 0;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.rtfAbout);
            this.panel1.Location = new System.Drawing.Point(8, 117);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(616, 275);
            this.panel1.TabIndex = 3;
            // 
            // rtfAbout
            // 
            this.rtfAbout.BackColor = System.Drawing.SystemColors.Window;
            this.rtfAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtfAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfAbout.Location = new System.Drawing.Point(1, 1);
            this.rtfAbout.Name = "rtfAbout";
            this.rtfAbout.ReadOnly = true;
            this.rtfAbout.Size = new System.Drawing.Size(614, 273);
            this.rtfAbout.TabIndex = 2;
            this.rtfAbout.Text = "";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(265, 105);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // tabGettingStarted
            // 
            this.tabGettingStarted.Controls.Add(this.GettingStartedHelp);
            this.tabGettingStarted.ImageIndex = 5;
            this.tabGettingStarted.Location = new System.Drawing.Point(4, 23);
            this.tabGettingStarted.Name = "tabGettingStarted";
            this.tabGettingStarted.Padding = new System.Windows.Forms.Padding(3);
            this.tabGettingStarted.Size = new System.Drawing.Size(623, 400);
            this.tabGettingStarted.TabIndex = 1;
            this.tabGettingStarted.Text = "Getting Started";
            this.tabGettingStarted.UseVisualStyleBackColor = true;
            // 
            // GettingStartedHelp
            // 
            this.GettingStartedHelp.AllowNavigation = false;
            this.GettingStartedHelp.AllowWebBrowserDrop = false;
            this.GettingStartedHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GettingStartedHelp.IsWebBrowserContextMenuEnabled = false;
            this.GettingStartedHelp.Location = new System.Drawing.Point(3, 3);
            this.GettingStartedHelp.MinimumSize = new System.Drawing.Size(20, 20);
            this.GettingStartedHelp.Name = "GettingStartedHelp";
            this.GettingStartedHelp.ScriptErrorsSuppressed = true;
            this.GettingStartedHelp.Size = new System.Drawing.Size(617, 394);
            this.GettingStartedHelp.TabIndex = 0;
            this.GettingStartedHelp.WebBrowserShortcutsEnabled = false;
            // 
            // tabHelp
            // 
            this.tabHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabHelp.Controls.Add(this.helpBrowser);
            this.tabHelp.Controls.Add(this.label1);
            this.tabHelp.Controls.Add(this.splitter2);
            this.tabHelp.Controls.Add(this.HelpTree);
            this.tabHelp.ImageIndex = 6;
            this.tabHelp.Location = new System.Drawing.Point(4, 23);
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Size = new System.Drawing.Size(623, 400);
            this.tabHelp.TabIndex = 2;
            this.tabHelp.Text = "Help";
            this.tabHelp.UseVisualStyleBackColor = true;
            // 
            // helpBrowser
            // 
            this.helpBrowser.AllowWebBrowserDrop = false;
            this.helpBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpBrowser.IsWebBrowserContextMenuEnabled = false;
            this.helpBrowser.Location = new System.Drawing.Point(137, 20);
            this.helpBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpBrowser.Name = "helpBrowser";
            this.helpBrowser.ScriptErrorsSuppressed = true;
            this.helpBrowser.Size = new System.Drawing.Size(484, 378);
            this.helpBrowser.TabIndex = 6;
            this.helpBrowser.Url = new System.Uri("http://ilab.ahemm.org/editroid/helpStart", System.UriKind.Absolute);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(137, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(484, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Help";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(134, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 398);
            this.splitter2.TabIndex = 5;
            this.splitter2.TabStop = false;
            // 
            // HelpTree
            // 
            this.HelpTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.HelpTree.ImageIndex = 0;
            this.HelpTree.ImageList = this.imageList1;
            this.HelpTree.Location = new System.Drawing.Point(0, 0);
            this.HelpTree.Name = "HelpTree";
            treeNode1.Name = "Node9";
            treeNode1.Tag = "root";
            treeNode1.Text = "Help Home";
            treeNode2.Name = "Node0";
            treeNode2.Tag = "Keys.html";
            treeNode2.Text = "Keyboard Shortcuts";
            treeNode3.Name = "Node2";
            treeNode3.Tag = "MapEditor.html";
            treeNode3.Text = "Editor Map";
            treeNode4.Name = "Node0";
            treeNode4.Tag = "ItemEditor.html";
            treeNode4.Text = "Item Editor";
            treeNode5.Name = "Node3";
            treeNode5.Tag = "PalEditor.html";
            treeNode5.Text = "Palette Editor";
            treeNode6.Name = "Node1";
            treeNode6.Tag = "PasswordEditor.html";
            treeNode6.Text = "Password Data Editor";
            treeNode7.Name = "SceenEditorNode";
            treeNode7.Tag = "ScreenEditor.html";
            treeNode7.Text = "Screen Editor";
            treeNode8.Name = "Node5";
            treeNode8.Tag = "StructEditor.html";
            treeNode8.Text = "Structure Editor";
            treeNode9.ImageIndex = 2;
            treeNode9.Name = "Node0";
            treeNode9.SelectedImageIndex = 2;
            treeNode9.Text = "Editors";
            treeNode10.Name = "Node7";
            treeNode10.Tag = "RomTips.html";
            treeNode10.Text = "ROM Tips And Guidelines";
            treeNode11.Name = "Node8";
            treeNode11.Tag = "EditorTips.html";
            treeNode11.Text = "Editor Tips";
            treeNode12.ImageIndex = 2;
            treeNode12.Name = "Node6";
            treeNode12.SelectedImageIndex = 2;
            treeNode12.Text = "Tips";
            this.HelpTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode9,
            treeNode12});
            this.HelpTree.SelectedImageIndex = 0;
            this.HelpTree.Size = new System.Drawing.Size(134, 398);
            this.HelpTree.TabIndex = 2;
            this.HelpTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.HelpTree_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Help.gif");
            this.imageList1.Images.SetKeyName(1, "ForwardRed.gif");
            this.imageList1.Images.SetKeyName(2, "folder-closed_16.gif");
            this.imageList1.Images.SetKeyName(3, "folder-closed_16.gif");
            this.imageList1.Images.SetKeyName(4, "Check.gif");
            this.imageList1.Images.SetKeyName(5, "GoGo.gif");
            this.imageList1.Images.SetKeyName(6, "Help.gif");
            this.imageList1.Images.SetKeyName(7, "Play.gif");
            this.imageList1.Images.SetKeyName(8, "Talk.gif");
            // 
            // FeedbackTab
            // 
            this.FeedbackTab.AllowDrop = true;
            this.FeedbackTab.Controls.Add(this.radQuestion);
            this.FeedbackTab.Controls.Add(this.label2);
            this.FeedbackTab.Controls.Add(this.btnSubmit);
            this.FeedbackTab.Controls.Add(this.chkEmailReply);
            this.FeedbackTab.Controls.Add(this.txtEmail);
            this.FeedbackTab.Controls.Add(this.txtFeedback);
            this.FeedbackTab.Controls.Add(this.radBug);
            this.FeedbackTab.Controls.Add(this.radComment);
            this.FeedbackTab.Controls.Add(this.radSuggestion);
            this.FeedbackTab.Controls.Add(this.FeedbackProgress);
            this.FeedbackTab.ImageIndex = 4;
            this.FeedbackTab.Location = new System.Drawing.Point(4, 23);
            this.FeedbackTab.Name = "FeedbackTab";
            this.FeedbackTab.Size = new System.Drawing.Size(623, 400);
            this.FeedbackTab.TabIndex = 3;
            this.FeedbackTab.Text = "Feedback";
            this.FeedbackTab.UseVisualStyleBackColor = true;
            // 
            // radQuestion
            // 
            this.radQuestion.AutoSize = true;
            this.radQuestion.Location = new System.Drawing.Point(8, 26);
            this.radQuestion.Name = "radQuestion";
            this.radQuestion.Size = new System.Drawing.Size(67, 17);
            this.radQuestion.TabIndex = 9;
            this.radQuestion.Text = "Question";
            this.radQuestion.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 339);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "E-mail address:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSubmit.Image = ((System.Drawing.Image)(resources.GetObject("btnSubmit.Image")));
            this.btnSubmit.Location = new System.Drawing.Point(8, 360);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(79, 22);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // chkEmailReply
            // 
            this.chkEmailReply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkEmailReply.AutoSize = true;
            this.chkEmailReply.Location = new System.Drawing.Point(174, 338);
            this.chkEmailReply.Name = "chkEmailReply";
            this.chkEmailReply.Size = new System.Drawing.Size(111, 17);
            this.chkEmailReply.TabIndex = 5;
            this.chkEmailReply.Text = "Request follow-up";
            this.chkEmailReply.UseVisualStyleBackColor = true;
            // 
            // txtEmail
            // 
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmail.Location = new System.Drawing.Point(93, 361);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(522, 20);
            this.txtEmail.TabIndex = 4;
            // 
            // txtFeedback
            // 
            this.txtFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFeedback.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFeedback.Location = new System.Drawing.Point(93, 3);
            this.txtFeedback.Multiline = true;
            this.txtFeedback.Name = "txtFeedback";
            this.txtFeedback.Size = new System.Drawing.Size(522, 329);
            this.txtFeedback.TabIndex = 3;
            // 
            // radBug
            // 
            this.radBug.AutoSize = true;
            this.radBug.Location = new System.Drawing.Point(8, 72);
            this.radBug.Name = "radBug";
            this.radBug.Size = new System.Drawing.Size(79, 17);
            this.radBug.TabIndex = 2;
            this.radBug.Text = "Bug Report";
            this.radBug.UseVisualStyleBackColor = true;
            this.radBug.CheckedChanged += new System.EventHandler(this.radBug_CheckedChanged);
            // 
            // radComment
            // 
            this.radComment.AutoSize = true;
            this.radComment.Checked = true;
            this.radComment.Location = new System.Drawing.Point(8, 3);
            this.radComment.Name = "radComment";
            this.radComment.Size = new System.Drawing.Size(69, 17);
            this.radComment.TabIndex = 1;
            this.radComment.TabStop = true;
            this.radComment.Text = "Comment";
            this.radComment.UseVisualStyleBackColor = true;
            // 
            // radSuggestion
            // 
            this.radSuggestion.AutoSize = true;
            this.radSuggestion.Location = new System.Drawing.Point(8, 49);
            this.radSuggestion.Name = "radSuggestion";
            this.radSuggestion.Size = new System.Drawing.Size(78, 17);
            this.radSuggestion.TabIndex = 0;
            this.radSuggestion.Text = "Suggestion";
            this.radSuggestion.UseVisualStyleBackColor = true;
            // 
            // FeedbackProgress
            // 
            this.FeedbackProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FeedbackProgress.Enabled = false;
            this.FeedbackProgress.Location = new System.Drawing.Point(8, 361);
            this.FeedbackProgress.Name = "FeedbackProgress";
            this.FeedbackProgress.Size = new System.Drawing.Size(78, 21);
            this.FeedbackProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.FeedbackProgress.TabIndex = 8;
            this.FeedbackProgress.Visible = false;
            // 
            // FeedbackWorker
            // 
            this.FeedbackWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FeedbackWorker_DoWork);
            this.FeedbackWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FeedbackWorker_RunWorkerCompleted);
            // 
            // frmHelp
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(631, 427);
            this.Controls.Add(this.tabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmHelp";
            this.Text = "Editroid Help";
            this.tabs.ResumeLayout(false);
            this.specialVersionTab.ResumeLayout(false);
            this.specialVersionTab.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabGettingStarted.ResumeLayout(false);
            this.tabHelp.ResumeLayout(false);
            this.FeedbackTab.ResumeLayout(false);
            this.FeedbackTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.TabPage tabGettingStarted;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox rtfAbout;
        private System.Windows.Forms.TabPage tabHelp;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.WebBrowser GettingStartedHelp;
        private System.Windows.Forms.TreeView HelpTree;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.WebBrowser helpBrowser;
        private System.Windows.Forms.TabPage FeedbackTab;
        private System.Windows.Forms.TextBox txtFeedback;
        private System.Windows.Forms.RadioButton radBug;
        private System.Windows.Forms.RadioButton radComment;
        private System.Windows.Forms.RadioButton radSuggestion;
        private System.Windows.Forms.CheckBox chkEmailReply;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnSubmit;
        private System.ComponentModel.BackgroundWorker FeedbackWorker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar FeedbackProgress;
        private System.Windows.Forms.RadioButton radQuestion;
        private System.Windows.Forms.TabPage specialVersionTab;
        private System.Windows.Forms.TextBox textBox1;
    }
}