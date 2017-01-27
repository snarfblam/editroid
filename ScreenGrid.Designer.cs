namespace Editroid
{
    partial class ScreenGrid
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.mapEditor = new Editroid.MapControl();
            this.ScreenBorder = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mapEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // mapEditor
            // 
            this.mapEditor.Location = new System.Drawing.Point(2, 2);
            this.mapEditor.Name = "mapEditor";
            this.mapEditor.Rom = null;
            this.mapEditor.Size = new System.Drawing.Size(256, 256);
            this.mapEditor.TabIndex = 0;
            this.mapEditor.TabStop = false;
            // 
            // ScreenBorder
            // 
            this.ScreenBorder.BackColor = System.Drawing.SystemColors.Highlight;
            this.ScreenBorder.Location = new System.Drawing.Point(0, 0);
            this.ScreenBorder.Name = "ScreenBorder";
            this.ScreenBorder.Size = new System.Drawing.Size(260, 260);
            this.ScreenBorder.TabIndex = 1;
            // 
            // ScreenGrid
            // 
            this.Controls.Add(this.mapEditor);
            this.Controls.Add(this.ScreenBorder);
            this.Name = "ScreenGrid";
            this.Size = new System.Drawing.Size(458, 387);
            ((System.ComponentModel.ISupportInitialize)(this.mapEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MapControl mapEditor;
        private System.Windows.Forms.Label ScreenBorder;
    }
}
