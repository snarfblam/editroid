using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    partial class frmMapStatus:Form
    {
        public frmMapStatus() {
            InitializeComponent();
        }


        public int RoomCount {
            get {
                return progress.Maximum;
            }
            set {
                progress.Maximum = value;
            }
        }

        public void Reset() {
            progress.Value = 0;
        }
        public void CompleteRoom() {
            progress.Value++;
        }
    }
}