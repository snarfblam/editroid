namespace Editroid
{
    partial class PatchForm
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
            this.PropGrid = new System.Windows.Forms.PropertyGrid();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.patchSummaryBox = new System.Windows.Forms.TextBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PropGrid
            // 
            this.PropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PropGrid.Location = new System.Drawing.Point(4, 4);
            this.PropGrid.Name = "PropGrid";
            this.PropGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.PropGrid.Size = new System.Drawing.Size(257, 327);
            this.PropGrid.TabIndex = 0;
            this.PropGrid.ToolbarVisible = false;
            // 
            // ApplyButton
            // 
            this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyButton.Location = new System.Drawing.Point(467, 305);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 1;
            this.ApplyButton.Text = "Apply Patch";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // patchSummaryBox
            // 
            this.patchSummaryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.patchSummaryBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.patchSummaryBox.Location = new System.Drawing.Point(267, 46);
            this.patchSummaryBox.Multiline = true;
            this.patchSummaryBox.Name = "patchSummaryBox";
            this.patchSummaryBox.ReadOnly = true;
            this.patchSummaryBox.Size = new System.Drawing.Size(275, 253);
            this.patchSummaryBox.TabIndex = 2;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(386, 305);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(267, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 39);
            this.label1.TabIndex = 4;
            this.label1.Text = "Patches are applied in memory. Save to ROM to retain changes.";
            // 
            // PatchForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(549, 335);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.patchSummaryBox);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.PropGrid);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PatchForm";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.ShowInTaskbar = false;
            this.Text = "Apply Patch";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid PropGrid;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.TextBox patchSummaryBox;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label1;
    }
}