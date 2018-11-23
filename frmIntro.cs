using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{

    public partial class frmIntro : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        List<String> tips = new List<string>();
        int tipIndex = -1;

        public frmIntro() {
            InitializeComponent();

            LoadTips();
            NextTip();

            //TipView.BackColor = SystemColors.Window;
            TipView.BackColor = SystemColors.Control;

            TipView.Enter += HideRtfCaret;
            TipView.MouseDown += HideRtfCaret;
            TipView.MouseMove += HideRtfCaret;

        }

        void HideRtfCaret(object sender, EventArgs e) {
            HideCaret(TipView.Handle);
        }

        private void NextTip() {
            tipIndex = (tipIndex + 1) % tips.Count;
            TipView.Rtf = tips[tipIndex];
            TipView.SelectionStart = 0;
            TipView.ScrollToCaret();
        }

        private void LoadTips() {
            var intro = Rtf.Intro.Replace(@"\{version\}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            tips.Add(intro);

            tips.Add(Rtf.Expando);
            tips.Add(Rtf.Navigation);
            tips.Add(Rtf.Editing);
            tips.Add(Rtf.Testing);
            //tips.Add(Rtf.ExternalChanges);
            //tips.AddRange(new[] { QuickTips.Intro });
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void NextButton_Click(object sender, EventArgs e) {
            NextTip();
            //openFileDialog1.ShowDialog();
            //TipView.LoadFile(openFileDialog1.FileName);
        }

        private void TipView_LinkClicked(object sender, LinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.LinkText);
        }
        


        //const string urlRtf = "{\\rtf1{\\field{\\*\\fldinst{ HYPERLINK \"*url*\"}}{\\fldresult{ *friendly*} }}}";
        //string Linkify(string rtf) {
        //    rtf = rtf.Replace("*url*","MOREINFO").Replace("*friendly*", "
        //}
    
    }

    
}

