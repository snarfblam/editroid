using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class frmExportAsm : Form
    {
        public frmExportAsm() {
            InitializeComponent();
            
        }

        public void SetFileList(IList<string> names) {
            FileList.Items.Clear();
            for (int i = 0; i < names.Count; i++) {
                FileList.Items.Add(names[i]);
                FileList.SetItemChecked(i, true);
            }
        }

        public string SelectedFolder { get; private set; }
        public IList<string> SelectedFiles { get; private set; }

        private void button1_Click(object sender, EventArgs e) {
            if (explorerTreeView1.SelectedItem == null) {
                SelectedFolder = string.Empty;
            } else {
                SelectedFolder = explorerTreeView1.SelectedItem.Path;
            }
            if (string.IsNullOrEmpty(SelectedFolder)) {
                lblBadFolderError.Visible = true;
                System.Media.SystemSounds.Beep.Play();
                DialogResult = DialogResult.None;
                return;
            }

            List<string> selection = new List<string>();
            foreach (var item in FileList.CheckedItems) {
                selection.Add(item as string);
            }
            SelectedFiles = selection;

            DialogResult = DialogResult.OK;
        }

        public bool UseAsmExtension { get { return chkAsmExtension.Checked; } }
    }

}
