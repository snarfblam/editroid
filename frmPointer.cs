using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
	/// <summary>
	/// Form that allows conversion of pointers from ROM offset values to Metroid pointers.
	/// </summary>
	public partial class frmPointer:Form
	{
		pCpu converter;

		/// <summary>
		/// Instantiates this class.
		/// </summary>
		public frmPointer() {
			InitializeComponent();

			cboLevel.SelectedIndex = 0;
		}

		void CalcPointer() {
			int realOffset;
			if(!Updating) {
				Updating = true;
				if(int.TryParse(txtROM.Text, System.Globalization.NumberStyles.HexNumber, null, out realOffset)) {
					converter.SetDataOffset((LevelIndex)(cboLevel.SelectedIndex), realOffset);
					txtPointer.Text = converter.Byte1.ToString("X").PadLeft(2, '0') + converter.Byte2.ToString("X").PadLeft(2, '0');
				}
				Updating = false;
			}
		}

		void CalcOffset() {
			if(!Updating) {
				Updating = true;
				
				string ptrText = txtPointer.Text.PadLeft(4, '0');
				Byte b1, b2;

				if(ptrText.Length <= 4
					&& byte.TryParse(ptrText.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out b1)
					&& byte.TryParse(ptrText.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out b2)) {

					converter.Byte1 = b1;
					converter.Byte2 = b2;
					int realOffset = converter.GetDataOffset((LevelIndex)cboLevel.SelectedIndex);
					txtROM.Text = realOffset.ToString("X");
				}
				Updating = false;
			}
		}

		bool Updating = false;
		private void txtROM_TextChanged(object sender, EventArgs e) {
			CalcPointer();
		}

		private void txtPointer_TextChanged(object sender, EventArgs e) {
			CalcOffset();
		}

		private void cboLevel_SelectedIndexChanged(object sender, EventArgs e) {
			CalcPointer();
		}
	}
}