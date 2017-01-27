using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM.Projects;

namespace Editroid
{
    public partial class frmBuildErrors : Form
    {
        public frmBuildErrors() {
            InitializeComponent();
        }

        internal void SetErrorList(IList<BuildError> errors) {
            errorControl.ShowErrors(errors);
        }
    }
}
