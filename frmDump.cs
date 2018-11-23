using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class frmDump : Form
    {
        CheckBox[] allOptions = new CheckBox[0];

        public frmDump() {
            InitializeComponent();

            allOptions = new CheckBox[] {
                chkBrinstar, chkNorfair, chkTourian, chkKraid, chkRidley,
                chkAreaChrAnim, chkAreaPalette, chkAreaItems, chkAreaChr, chkAreaCombos, chkAreaStructures, chkAreaScreens, chkAreaAltMusic, chkAreaAsm, chkAreaTilePhysics,
                chkPassword, chkTitleChr, chkTitleChrAnim, chkAsm,
            };
        }

        private void lnkSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            foreach (CheckBox c in allOptions) c.Checked = true;
        }

        private void lnkUnselectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            foreach (CheckBox c in allOptions) c.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            List<DumpInsertItem> dataList = new List<DumpInsertItem>();
            List<LevelIndex> selectedLevels = new List<LevelIndex>();

            if (chkBrinstar.Checked) selectedLevels.Add(LevelIndex.Brinstar);
            if (chkNorfair.Checked) selectedLevels.Add(LevelIndex.Norfair);
            if (chkTourian.Checked) selectedLevels.Add(LevelIndex.Tourian);
            if (chkKraid.Checked) selectedLevels.Add(LevelIndex.Kraid);
            if (chkRidley.Checked) selectedLevels.Add(LevelIndex.Ridley);

            foreach (var area in selectedLevels) {
                if (chkAreaChrAnim.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.CHRAnimation));
                if (chkAreaPalette.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.Palette));
                if (chkAreaItems.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.Item));
                if (chkAreaChr.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.CHR));
                if (chkAreaCombos.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.Combo));
                if (chkAreaStructures.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.Structure));
                if (chkAreaScreens.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.Screen));
                if (chkAreaAltMusic.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.AltMusic));
                if (chkAreaAsm.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.Asm));
                if (chkAreaTilePhysics.Checked)
                    dataList.Add(new DumpInsertItem(area, DumpInsertType.TilePhysics));
            }

            if (chkPassword.Checked)
                dataList.Add(new DumpInsertItem(LevelIndex.None, DumpInsertType.Password));
            if (chkTitleChr.Checked)
                dataList.Add(new DumpInsertItem(LevelIndex.None, DumpInsertType.CHR));
            if (chkTitleChrAnim.Checked)
                dataList.Add(new DumpInsertItem(LevelIndex.None, DumpInsertType.CHRAnimation));
            if (chkAsm.Checked)
                dataList.Add(new DumpInsertItem(LevelIndex.None, DumpInsertType.Asm));
        }

    }
}
