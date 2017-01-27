using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class frmChrTable : Form
    {
        ChrUsageEntryEditor titleEntry;
        ChrUsageEntryEditor brinEntry;
        ChrUsageEntryEditor norEntry;
        ChrUsageEntryEditor tourEntry;
        ChrUsageEntryEditor kraidEntry;
        ChrUsageEntryEditor ridleyEntry;

        MetroidRom _rom;
        public MetroidRom Rom {
            get { return _rom; }
            set {
                _rom = value;
                if (_rom == null) {
                    brinEntry = norEntry = tourEntry = kraidEntry = ridleyEntry = null;
                } else {
                    titleEntry = new ChrUsageEntryEditor(Rom, 0, titleSpr, titleBg1, titleBg2, titleRate);
                    brinEntry = new ChrUsageEntryEditor(Rom, 1, brinSpr, brinBg1, brinBg2, brinRate);
                    norEntry = new ChrUsageEntryEditor(Rom, 2, norSpr, norBg1, norBg2, norRate);
                    tourEntry = new ChrUsageEntryEditor(Rom, 3, touSpr, touBg1, touBg2, touRate);
                    kraidEntry = new ChrUsageEntryEditor(Rom, 4, kraidSpr, kraidBg1, kraidBg2, kraidRate);
                    ridleyEntry = new ChrUsageEntryEditor(Rom, 5, ridleySpr, ridleyBg1, ridleyBg2, ridleyRate);
                }

                LoadFromRom();
            }
        }

        private frmChrTable() {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }
        public static DialogResult ShowChrDialog(MetroidRom rom, Form owner) {
            using (frmChrTable frm = new frmChrTable()) {
                try {
                    frm.Rom = rom;
                } catch (ArgumentOutOfRangeException ex) { // Thrown when initializing numericupdown controls with invalid values
                    MessageBox.Show("This ROM does not appear to have a valid CHR usage table.");
                    return DialogResult.Cancel;
                }
                return Program.Dialogs.ShowDialog(frm, owner);
            }
        }

        void LoadFromRom() {
            if (brinEntry != null) {
                titleEntry.ReadData();
                brinEntry.ReadData();
                norEntry.ReadData();
                tourEntry.ReadData();
                ridleyEntry.ReadData();
                kraidEntry.ReadData();
            }
        }
        void SaveToRom() {
            if (brinEntry == null) throw new InvalidOperationException("OH GOD NO!");
            titleEntry.WriteData();
            brinEntry.WriteData();
            norEntry.WriteData();
            tourEntry.WriteData();
            ridleyEntry.WriteData();
            kraidEntry.WriteData();
        }

        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);

            if (Rom == null) throw new InvalidOperationException("ROM must be specified before showing the form.");

        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            SaveToRom();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

    }

    class ChrUsageEntryEditor
    {
        NumericUpDown spr,bg1,bg2,rate;
        MetroidRom rom;
        int index;

        public ChrUsageEntryEditor(MetroidRom rom, int index, NumericUpDown spr, NumericUpDown bg1, NumericUpDown bg2, NumericUpDown rate) {
            this.rom = rom;
            this.index = index;
            if (!rom.Format.HasChrUsageTable) throw new ArgumentException("Specified ROM does not have a CHR usage table.");

            this.spr = spr;
            this.bg1 = bg1;
            this.bg2 = bg2;
            this.rate = rate;
        }

        public void ReadData() {
            var pChrUsage = rom.Banks[0xE].GetPtr(Editroid.ROM.Formats.RomFormat.ChrUsagePointer);
            pChrUsage += index * 4;

            var bank = rom.Banks[0xF];

            spr.Value = bank[pChrUsage];
            bg1.Value = bank[pChrUsage + 1];
            bg2.Value = bank[pChrUsage + 2];
            rate.Value = bank[pChrUsage + 3];

        }
        public void WriteData() {
            var pChrUsage = rom.Banks[0xE].GetPtr(Editroid.ROM.Formats.RomFormat.ChrUsagePointer);
            pChrUsage += index * 4;

            var bank = rom.Banks[0xF];

            bank[pChrUsage] = (byte)spr.Value;
            bank[pChrUsage + 1] = (byte)bg1.Value;
            bank[pChrUsage + 2] = (byte)bg2.Value;
            bank[pChrUsage + 3] = (byte)rate.Value;

        }

        
    }
}
