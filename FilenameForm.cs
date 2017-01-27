using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class FilenameForm : Form
    {
        public FilenameForm() {
            InitializeComponent();
        }

        string _EnteredFilename;
        public string EnteredFilename {
            get { return _EnteredFilename; }
            set {
                _EnteredFilename = value;
                txtFilename.Text = value;
            }
        }
        /// <summary>
        /// A list of filenames the user is not allowed to use
        /// </summary>
        public IList<string> ReservedFilenames { get; set; }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Enter) {
                if (ValidateFilename()) {
                    _EnteredFilename = txtFilename.Text;
                    DialogResult = DialogResult.OK;
                    return true;
                }
            } else if (keyData == Keys.Escape) {
                DialogResult = DialogResult.Cancel;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool ValidateFilename() {
            string filename = txtFilename.Text;

            if (string.IsNullOrEmpty(filename.Trim())) {
                ShowError("Please enter a file name");
                return false;
            }
            if (char.IsWhiteSpace(filename[0]) || char.IsWhiteSpace(filename[filename.Length - 1])) {
                filename = filename.Trim();
                txtFilename.Text = filename;
            }

            if (ReservedFilenames != null) {
                for (int i = 0; i < ReservedFilenames.Count; i++) {
                    if (string.Equals(filename, ReservedFilenames[i], StringComparison.OrdinalIgnoreCase)) {
                        ShowError("The filename is already in use");
                        return false;
                    }
                }
            }
            for (int i = 0; i < filename.Length; i++) {
                bool valid = false;
                var c = filename[i];
                if (char.IsLetterOrDigit(c)) valid = true;
                if (c == ' ' || c == '_' || c == '.' || c == '-') valid = true;

                if (!valid) {
                    ShowError("The character \"" + c.ToString() + "\" is not valid");
                    return false;
                }
            }



            return true;
        }

        private void ShowError(string errorText) {
            lblFilenameError.Text = errorText;
            lblFilenameError.Visible = true;
            System.Media.SystemSounds.Beep.Play();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            if (ValidateFilename()) {
                _EnteredFilename = txtFilename.Text;
                DialogResult = DialogResult.OK;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            
        }
    }
}
