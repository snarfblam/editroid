using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class frmFileChangeWarning : Form
    {
        public frmFileChangeWarning() {
            InitializeComponent();
        }

        public bool HasUnsavedChanges {
            get { return UnchangedLabel.Visible; }
            set {
                UnchangedLabel.Visible = !value;
                AdvancedPanel.Enabled = value;
            }
        }

        public void SetChangedBytes(int count) {
            ChangedBytesLabel.Text = "Bytes changed in file: " + count.ToString();
        }
        public void SetUnsavedBytes(int count) {
            UnsavedBytesLabel.Text = "Bytes unsaved in Editroid: " + count.ToString();
        }

        private void btnIgnore_Click(object sender, EventArgs e) {
            action = FileChangeAction.Ignore;
            Close();
        }

        private FileChangeAction action;

        internal FileChangeAction Action {
            get { return action; }
            set { action = value; }
        }

        private bool applyExternalChanges;

        public bool ApplyExternalChanges {
            get { return applyExternalChanges; }
            set { applyExternalChanges = value; }
        }

        private bool applyExternalChangesSelective;

        public bool ApplyExternalChangesSelective {
            get { return applyExternalChangesSelective; }
            set { applyExternalChangesSelective = value; }
        }

        private bool applyUnsavedChanges;

        public bool ApplyUnsavedChanges {
            get { return applyUnsavedChanges; }
            set { applyUnsavedChanges = value; }
        }

        private bool applyUnsavedChangesSelective;

        public bool ApplyUnsavedChangesSelective {
            get { return applyUnsavedChangesSelective; }
            set { applyUnsavedChangesSelective = value; }
        }

        private void radDisk_CheckedChanged(object sender, EventArgs e) {
            action = FileChangeAction.LoadDisk;
        }

        private void radSaved_CheckedChanged(object sender, EventArgs e) {
            action = FileChangeAction.LoadSaved;
        }

        private void radUnsaved_CheckedChanged(object sender, EventArgs e) {
            action = FileChangeAction.LoadUnsaved;
        }

        private void chkExternal_CheckedChanged(object sender, EventArgs e) {
            applyExternalChanges = chkExternal.Checked;

            if (chkExternal.Checked)
                chkExternalSelective.Checked = false;
        }

        private void chkEkternalSelective_CheckedChanged(object sender, EventArgs e) {
            applyExternalChangesSelective = chkExternalSelective.Checked;

            if (chkExternalSelective.Checked)
                chkExternal.Checked = false;

        }

        private void chkUnsaved_CheckedChanged(object sender, EventArgs e) {
            ApplyUnsavedChanges = chkUnsaved.Checked;

            if (chkUnsaved.Checked)
                chkUnsavedSelective.Checked = false;

        }

        private void chkUnsavedSelective_CheckedChanged(object sender, EventArgs e) {
            applyUnsavedChangesSelective = chkUnsavedSelective.Checked;

            if (chkUnsavedSelective.Checked)
                chkUnsaved.Checked = false;

        }

        private void btnPatterns_Click(object sender, EventArgs e) {
            action = FileChangeAction.LoadPatternsOnly;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e) {
            action = FileChangeAction.Reload;
            Close();
        }



	
    }

    enum FileChangeAction
    {
        Ignore,
        Reload,
        LoadDisk,
        LoadSaved,
        LoadUnsaved,
        LoadPatternsOnly
    }
}