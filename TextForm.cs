using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Editroid
{
    public partial class TextForm : Form
    {
        public TextForm() {
            InitializeComponent();
        }

        public string EditorText { get { return textBox1.Text; } set { textBox1.Text = value; } }

        private void toolStripLabel1_Click(object sender, EventArgs e) {
            if (TextSaver.ShowDialog() == DialogResult.OK) {
                try {
                    File.WriteAllText(TextSaver.FileName, textBox1.Text);
                } catch{
                    MessageBox.Show("Could not write file.");
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            Clipboard.SetText(textBox1.Text);
        }

        public static void ShowText(string title, string text) {
            TextForm form = new TextForm();
            form.Text = title;
            form.EditorText = text;
            form.Show();

        }

    }
}
