namespace WilsonProgramming
{
    partial class ExplorerTreeView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeWnd = new WilsonProgramming.ExplorerTreeViewWnd();
            this.SuspendLayout();
            // 
            // treeWnd
            // 
            this.treeWnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeWnd.Location = new System.Drawing.Point(0, 0);
            this.treeWnd.Name = "treeWnd";
            this.treeWnd.Size = new System.Drawing.Size(289, 454);
            this.treeWnd.TabIndex = 0;
            // 
            // ExplorerTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.treeWnd);
            this.Name = "ExplorerTreeView";
            this.Size = new System.Drawing.Size(289, 454);
            this.ResumeLayout(false);

        }

        #endregion

        private ExplorerTreeViewWnd treeWnd;
    }
}
