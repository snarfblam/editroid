using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Editroid.ROM.Projects;
using Editroid.ROM;

namespace Editroid
{
    internal partial class frmBankAllocation : Form
    {
        BankAllocation[] banks;

        public frmBankAllocation() {
            InitializeComponent();

        }

        /// <summary>
        /// Call this method before showing form. Sets bank allocation data to edit.
        /// </summary>
        /// <param name="banks"></param>
        public void SetBanks(BankAllocation[] banks) {
            this.banks = banks;
        }

        private void lstBanks_MeasureItem(object sender, MeasureItemEventArgs e) {
            e.ItemHeight = 16;
            e.ItemWidth = 2400;
        }

        SolidBrush sb = new SolidBrush(Color.White);
        private void lstBanks_DrawItem(object sender, DrawItemEventArgs e) {
            var font = lstBanks.Font;
            var itemText  = banks[e.Index].BankNumber.ToString("X2") + " - " + banks[e.Index].Description;

            if ((e.State &DrawItemState.Selected)!=0) {
                using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(e.Bounds.X, e.Bounds.Bottom + 1), new Point(e.Bounds.X, e.Bounds.Y - 1), SystemColors.Highlight, SystemColors.Window)) {
                    e.Graphics.FillRectangle(lgb, e.Bounds);
                }
                e.Graphics.DrawString(itemText, lstBanks.Font, SystemBrushes.WindowText, new Point(e.Bounds.X + 1, e.Bounds.Y + 1));
                e.Graphics.DrawString(itemText, lstBanks.Font, SystemBrushes.WindowText, new Point(e.Bounds.X - 1, e.Bounds.Y - 1));
                e.Graphics.DrawString(itemText, lstBanks.Font, SystemBrushes.WindowText, new Point(e.Bounds.X - 1, e.Bounds.Y + 1));
                e.Graphics.DrawString(itemText, lstBanks.Font, SystemBrushes.WindowText, new Point(e.Bounds.X + 1, e.Bounds.Y - 1));
                e.Graphics.DrawString(itemText, lstBanks.Font, SystemBrushes.HighlightText, new Point(e.Bounds.X, e.Bounds.Y));

                Rectangle minusOne = e.Bounds;
                minusOne.Width -= 1;
                minusOne.Height -= 1;
                e.Graphics.DrawRectangle(SystemPens.Highlight, minusOne);
            } else {
                Brush brush = sb;
                if (banks[e.Index].Reserved) {
                    sb.Color = Color.LightBlue;
                } else if (banks[e.Index].UserReserved) {
                    sb.Color = Color.Pink;
                } else {
                    brush = SystemBrushes.Window;
                }
                e.Graphics.FillRectangle(brush, e.Bounds);
                e.Graphics.DrawString(itemText, lstBanks.Font, SystemBrushes.WindowText, new Point(e.Bounds.X, e.Bounds.Y));
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void btnReserve_Click(object sender, EventArgs e) {
            if (!selectedBank.Reserved) {
                selectedBank.UserReserved = !(selectedBank.UserReserved);

                lstBanks.SelectedIndex = lstBanks.SelectedIndex;
            }
        }

        private void lstBanks_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = selectedBank;
            if (selectedBank == null || selectedBank.Reserved) {
                btnReserve.Text = "Reserve";
                btnReserve.Enabled = false;
            } else if (selectedBank.UserReserved) {
                btnReserve.Text = "Unreserve";
                btnReserve.Enabled = true;
            } else {
                btnReserve.Text = "Reserve";
                btnReserve.Enabled = true;
            }

            if (selectedBank == null) {
                lblDetails.Text = string.Empty;
            } else {
                lblDetails.Text =
                    "Bank " + selectedBank.BankNumber.ToString("X2") + ": " +
                    (selectedBank.BankNumber * 0x2000 + 10).ToString("X") + " (" +
                    selectedBank.Description + ")";
            }
        }

        BankAllocation selectedBank { get { return lstBanks.SelectedIndex < 0 ? null : banks[lstBanks.SelectedIndex]; } }
    }
}
