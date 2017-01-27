namespace Editroid
{
    partial class frmFileChangeWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFileChangeWarning));
            this.label1 = new System.Windows.Forms.Label();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.UnchangedLabel = new System.Windows.Forms.Label();
            this.AdvancedPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.chkUnsavedSelective = new System.Windows.Forms.CheckBox();
            this.chkExternalSelective = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.chkUnsaved = new System.Windows.Forms.CheckBox();
            this.chkExternal = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.radUnsaved = new System.Windows.Forms.RadioButton();
            this.radSaved = new System.Windows.Forms.RadioButton();
            this.radDisk = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ChangedBytesLabel = new System.Windows.Forms.Label();
            this.UnsavedBytesLabel = new System.Windows.Forms.Label();
            this.btnPatterns = new System.Windows.Forms.Button();
            this.AdvancedPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(435, 69);
            this.label1.TabIndex = 0;
            this.label1.Text = "The file has beed modified by an external program. You can load the changed file," +
                " ignore the changes, or select an advanced option below.";
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(12, 81);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(104, 23);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "Reload File";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(122, 81);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(104, 23);
            this.btnIgnore.TabIndex = 2;
            this.btnIgnore.Text = "Ignore Changes";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // UnchangedLabel
            // 
            this.UnchangedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.UnchangedLabel.AutoSize = true;
            this.UnchangedLabel.ForeColor = System.Drawing.Color.Red;
            this.UnchangedLabel.Location = new System.Drawing.Point(12, 135);
            this.UnchangedLabel.Name = "UnchangedLabel";
            this.UnchangedLabel.Size = new System.Drawing.Size(258, 13);
            this.UnchangedLabel.TabIndex = 11;
            this.UnchangedLabel.Text = "* Editroid hasn\'t made any changes to the current file.";
            // 
            // AdvancedPanel
            // 
            this.AdvancedPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AdvancedPanel.Controls.Add(this.button1);
            this.AdvancedPanel.Controls.Add(this.chkUnsavedSelective);
            this.AdvancedPanel.Controls.Add(this.chkExternalSelective);
            this.AdvancedPanel.Controls.Add(this.checkBox3);
            this.AdvancedPanel.Controls.Add(this.checkBox4);
            this.AdvancedPanel.Controls.Add(this.chkUnsaved);
            this.AdvancedPanel.Controls.Add(this.chkExternal);
            this.AdvancedPanel.Controls.Add(this.label5);
            this.AdvancedPanel.Controls.Add(this.radUnsaved);
            this.AdvancedPanel.Controls.Add(this.radSaved);
            this.AdvancedPanel.Controls.Add(this.radDisk);
            this.AdvancedPanel.Controls.Add(this.label4);
            this.AdvancedPanel.Controls.Add(this.label3);
            this.AdvancedPanel.Controls.Add(this.label2);
            this.AdvancedPanel.Location = new System.Drawing.Point(0, 154);
            this.AdvancedPanel.Name = "AdvancedPanel";
            this.AdvancedPanel.Size = new System.Drawing.Size(459, 1);
            this.AdvancedPanel.TabIndex = 24;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 310);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 37;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // chkUnsavedSelective
            // 
            this.chkUnsavedSelective.AutoSize = true;
            this.chkUnsavedSelective.Enabled = false;
            this.chkUnsavedSelective.Location = new System.Drawing.Point(233, 295);
            this.chkUnsavedSelective.Name = "chkUnsavedSelective";
            this.chkUnsavedSelective.Size = new System.Drawing.Size(200, 17);
            this.chkUnsavedSelective.TabIndex = 36;
            this.chkUnsavedSelective.Text = "Unsaved Changes (Editroid sections)";
            this.toolTip1.SetToolTip(this.chkUnsavedSelective, "Copy any data that Editroid accesses from the unsaved ROM to the selected file.");
            this.chkUnsavedSelective.UseVisualStyleBackColor = true;
            this.chkUnsavedSelective.CheckedChanged += new System.EventHandler(this.chkUnsavedSelective_CheckedChanged);
            // 
            // chkExternalSelective
            // 
            this.chkExternalSelective.AutoSize = true;
            this.chkExternalSelective.Checked = true;
            this.chkExternalSelective.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExternalSelective.Enabled = false;
            this.chkExternalSelective.Location = new System.Drawing.Point(233, 249);
            this.chkExternalSelective.Name = "chkExternalSelective";
            this.chkExternalSelective.Size = new System.Drawing.Size(216, 17);
            this.chkExternalSelective.TabIndex = 35;
            this.chkExternalSelective.Text = "External Changes (non-Editroid sections)";
            this.toolTip1.SetToolTip(this.chkExternalSelective, "Copy any data from the externally changed file that Editroid does not access.");
            this.chkExternalSelective.UseVisualStyleBackColor = true;
            this.chkExternalSelective.CheckedChanged += new System.EventHandler(this.chkEkternalSelective_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Enabled = false;
            this.checkBox3.Location = new System.Drawing.Point(388, 247);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(175, 17);
            this.checkBox3.TabIndex = 34;
            this.checkBox3.Text = "Unsaved Changes (segmented)";
            this.toolTip1.SetToolTip(this.checkBox3, "Apply any segments (of 256 bytes) that Editroid has changed (since opening or sav" +
                    "ing the ROM) to the selected file.");
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.Visible = false;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Enabled = false;
            this.checkBox4.Location = new System.Drawing.Point(388, 225);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(170, 17);
            this.checkBox4.TabIndex = 33;
            this.checkBox4.Text = "External Changes (segmented)";
            this.toolTip1.SetToolTip(this.checkBox4, "Apply any changed segments (256 bytes) to the selected file.");
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.Visible = false;
            // 
            // chkUnsaved
            // 
            this.chkUnsaved.AutoSize = true;
            this.chkUnsaved.Enabled = false;
            this.chkUnsaved.Location = new System.Drawing.Point(233, 272);
            this.chkUnsaved.Name = "chkUnsaved";
            this.chkUnsaved.Size = new System.Drawing.Size(114, 17);
            this.chkUnsaved.TabIndex = 32;
            this.chkUnsaved.Text = "Unsaved Changes";
            this.toolTip1.SetToolTip(this.chkUnsaved, "Apply any bytes that Editroid has changed (since opening or saving the ROM) to th" +
                    "e selected file.");
            this.chkUnsaved.UseVisualStyleBackColor = true;
            this.chkUnsaved.CheckedChanged += new System.EventHandler(this.chkUnsaved_CheckedChanged);
            // 
            // chkExternal
            // 
            this.chkExternal.AutoSize = true;
            this.chkExternal.Enabled = false;
            this.chkExternal.Location = new System.Drawing.Point(233, 226);
            this.chkExternal.Name = "chkExternal";
            this.chkExternal.Size = new System.Drawing.Size(109, 17);
            this.chkExternal.TabIndex = 31;
            this.chkExternal.Text = "External Changes";
            this.toolTip1.SetToolTip(this.chkExternal, "Apply any changed bytes to the selected file.");
            this.chkExternal.UseVisualStyleBackColor = true;
            this.chkExternal.CheckedChanged += new System.EventHandler(this.chkExternal_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(230, 209);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Apply:";
            // 
            // radUnsaved
            // 
            this.radUnsaved.AutoSize = true;
            this.radUnsaved.Checked = true;
            this.radUnsaved.Enabled = false;
            this.radUnsaved.Location = new System.Drawing.Point(15, 271);
            this.radUnsaved.Name = "radUnsaved";
            this.radUnsaved.Size = new System.Drawing.Size(106, 17);
            this.radUnsaved.TabIndex = 29;
            this.radUnsaved.TabStop = true;
            this.radUnsaved.Text = "Unsaved Version";
            this.radUnsaved.UseVisualStyleBackColor = true;
            this.radUnsaved.CheckedChanged += new System.EventHandler(this.radUnsaved_CheckedChanged);
            // 
            // radSaved
            // 
            this.radSaved.AutoSize = true;
            this.radSaved.Enabled = false;
            this.radSaved.Location = new System.Drawing.Point(15, 248);
            this.radSaved.Name = "radSaved";
            this.radSaved.Size = new System.Drawing.Size(114, 17);
            this.radSaved.TabIndex = 28;
            this.radSaved.Text = "Last saved version";
            this.radSaved.UseVisualStyleBackColor = true;
            this.radSaved.CheckedChanged += new System.EventHandler(this.radSaved_CheckedChanged);
            // 
            // radDisk
            // 
            this.radDisk.AutoSize = true;
            this.radDisk.Enabled = false;
            this.radDisk.Location = new System.Drawing.Point(15, 225);
            this.radDisk.Name = "radDisk";
            this.radDisk.Size = new System.Drawing.Size(78, 17);
            this.radDisk.TabIndex = 27;
            this.radDisk.Text = "File on disk";
            this.radDisk.UseVisualStyleBackColor = true;
            this.radDisk.CheckedChanged += new System.EventHandler(this.radDisk_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(12, 209);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Open:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 19);
            this.label3.TabIndex = 25;
            this.label3.Text = "Advanced";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label2.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(451, 1);
            this.label2.TabIndex = 24;
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // ChangedBytesLabel
            // 
            this.ChangedBytesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ChangedBytesLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ChangedBytesLabel.Location = new System.Drawing.Point(179, 107);
            this.ChangedBytesLabel.Name = "ChangedBytesLabel";
            this.ChangedBytesLabel.Size = new System.Drawing.Size(280, 16);
            this.ChangedBytesLabel.TabIndex = 25;
            this.ChangedBytesLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // UnsavedBytesLabel
            // 
            this.UnsavedBytesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.UnsavedBytesLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.UnsavedBytesLabel.Location = new System.Drawing.Point(179, 123);
            this.UnsavedBytesLabel.Name = "UnsavedBytesLabel";
            this.UnsavedBytesLabel.Size = new System.Drawing.Size(280, 16);
            this.UnsavedBytesLabel.TabIndex = 26;
            this.UnsavedBytesLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnPatterns
            // 
            this.btnPatterns.Location = new System.Drawing.Point(233, 81);
            this.btnPatterns.Name = "btnPatterns";
            this.btnPatterns.Size = new System.Drawing.Size(164, 23);
            this.btnPatterns.TabIndex = 38;
            this.btnPatterns.Text = "Reload Patterns Only";
            this.btnPatterns.UseVisualStyleBackColor = true;
            this.btnPatterns.Click += new System.EventHandler(this.btnPatterns_Click);
            // 
            // frmFileChangeWarning
            // 
            this.AcceptButton = this.btnIgnore;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 155);
            this.Controls.Add(this.UnchangedLabel);
            this.Controls.Add(this.btnPatterns);
            this.Controls.Add(this.UnsavedBytesLabel);
            this.Controls.Add(this.ChangedBytesLabel);
            this.Controls.Add(this.AdvancedPanel);
            this.Controls.Add(this.btnIgnore);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFileChangeWarning";
            this.Text = "File has changed";
            this.AdvancedPanel.ResumeLayout(false);
            this.AdvancedPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Label UnchangedLabel;
        private System.Windows.Forms.Panel AdvancedPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkUnsavedSelective;
        private System.Windows.Forms.CheckBox chkExternalSelective;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox chkUnsaved;
        private System.Windows.Forms.CheckBox chkExternal;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radUnsaved;
        private System.Windows.Forms.RadioButton radSaved;
        private System.Windows.Forms.RadioButton radDisk;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label ChangedBytesLabel;
        private System.Windows.Forms.Label UnsavedBytesLabel;
        private System.Windows.Forms.Button btnPatterns;
    }
}