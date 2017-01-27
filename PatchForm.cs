using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.Patches;
using System.IO;

namespace Editroid
{
    public partial class PatchForm : Form
    {
        public PatchForm() {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;
        }

        RomPatch patch;
        public RomPatch Patch {
            get {
                return patch;
            }
            set {
                PropGrid.SelectedObject = value;
                patch = value;
                patchSummaryBox.Text = value.Description;
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            Close();
        }

        /// <summary>
        /// Prompts the user to apply a patch and returns true if the patch is applied.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="rom"></param>
        /// <returns>True if the patch is applied.</returns>
        public static bool TryApplyPatch(RomPatch patch, MetroidRom rom, Form owner) {
            PatchForm form = new PatchForm();
            form.Patch = patch;
            Program.Dialogs.ShowDialog(form, owner); 

            if (form.DialogResult == DialogResult.OK) {
                using (Stream s = new MemoryStream(rom.data)) {
                    patch.Apply(s);
                    return true;
                }

            }

            return false;
        }
    }
}
