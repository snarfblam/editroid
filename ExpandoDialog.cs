using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class ExpandoDialog : Form
    {
        public ExpandoDialog() {
            InitializeComponent();

            ShowExpandMessage();
        }

        private void ShowExpandMessage() {
            richTextBox1.Rtf = Rtf.EnhancoNotes;
            string romFormatString;
            switch (_currentRomFormat) {
                case RomFormats.Standard: romFormatString = "Unexpanded"; break;
                case RomFormats.Expando: romFormatString = "1/2 Expanded"; break;
                case RomFormats.Enhanco: romFormatString = "Expanded"; break;
                default: romFormatString = "Unknown"; break;
            }
            richTextBox1.AppendText(Environment.NewLine + Environment.NewLine + "Current ROM format: " + romFormatString);
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Hide();
            DialogResult = DialogResult.Cancel;
        }

        bool _showingMmc3Options = true;
        public bool ShowMMC3Options {
            get { return _showingMmc3Options; }
            set {
                if (value != _showingMmc3Options)
                    SetMMC3OptionsVisible(value);
            }
        }

        private void SetMMC3OptionsVisible(bool value) {
            if (value) { // Show
                Height += pnlOptions.Height + pnlOptions.Top - btnOX.Bottom;
            } else { // Hide
                Height -= pnlOptions.Height + pnlOptions.Top - btnOX.Bottom;
            }

            _showingMmc3Options = value;
        }

        private void btnOX_Click(object sender, EventArgs e) {
            UpdateExpansionFile = chkUpdateExp.Checked;
            UpdateFixedBankRefs = chkUpdateFixed.Checked;

            Hide();
            DialogResult = DialogResult.OK;
        }

        public bool UpdateExpansionFile { get; set; }
        public bool UpdateFixedBankRefs { get; set; }


        RomFormats _currentRomFormat;
        internal RomFormats CurrentRomFormat {
            get { return _currentRomFormat; }
            set {
                _currentRomFormat = value;
                OnCurrentRomFormatChanged();
            }
        }

        private void OnCurrentRomFormatChanged() {
            ShowExpandMessage();
        }
    }
}
