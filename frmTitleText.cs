using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    internal partial class frmTitleText:Form
    {
        public frmTitleText() {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set {
                rom = value;
                LoadText();
            }
        }

        private void LoadText() {
            txtLine1.Text = rom.TitleText.Line1;
            txtLine2.Text = rom.TitleText.Line2;
        }

        private void textBox_TextChanged(object sender, EventArgs e) {
            TextBox Sender = sender as TextBox;

            int cursorPosition = Sender.SelectionStart;

            if(Sender.Text.Contains("@")) {// Convert @ to ©
                Sender.Text = Sender.Text.Replace('@', '©');
                Sender.SelectionStart = cursorPosition;
            }
    
            string correction = Sender.Text;
            bool needsCorrection = false;
            for(int i = 0; i < correction.Length; i++) {
                char c = correction[i];
                if( // Is char invalid?
                    (c < '0' || c > '9') && // Not a digit
                    (c < 'A' || c > 'Z') && // Not a letter
                    (c < 'a' || c > 'z') && // Not a lowercase letter
                    (c != '©' && c != '?' && c != '-' && c != ' ')) { // Not ©, ?, -, or space
                    needsCorrection = true;
                    // If so, replace invalid char with a dash
                    correction = correction.Substring(0,i) + "-" + correction.Substring(i + 1);
                }
            }

            if(needsCorrection) {
                Sender.Text = correction;
                Sender.SelectionStart = cursorPosition;
            }
        }

        protected override void OnClosing(CancelEventArgs e) {
            if(DialogResult == DialogResult.OK) {
                rom.TitleText.Line1 = txtLine1.Text;
                rom.TitleText.Line2 = txtLine2.Text;
            }

            base.OnClosing(e);
        }

        private void btnOK_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }

    }

}