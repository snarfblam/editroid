using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;
using Editroid.Actions;

namespace Editroid
{
    public partial class frmAdvancedPalette : Form
    {
        public frmAdvancedPalette() {
            InitializeComponent();

            // Designer is gHey
            PaletteControl.CellClicked += new EventHandler<EventArgs<Point>>(PaletteControl_CellClicked);
            PaletteControl.ColorGrabbed += new EventHandler<EventArgs<int>>(PaletteControl_ColorGrabbed);
        }

        static string[] Descriptions = new string[]{
            "Base Palette",
            "Samus",
            "Samus Varia",
            "Samus Missile",
            "Samus M+V",
            "Alternate Palette",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "-",
            "Fade-in 0",
            "Fade-in 1",
            "Fade-in 2",
            "Fade-in 3",
            "-",
            "Suitless",
            "Suitless Varia",
            "Suitless Missile",
            "Suitless M+V",
        };
        
        public Level Level { get; private set; }

        public void LoadPalettes(Level level) {
            UnloadLevel();
            this.Level = level;
            LoadLevel();

            LevelLabel.Text = LevelLabel.Tag.ToString().Replace("*", level.Index.ToString());
        }

        private void LoadLevel() {
            PopulateListView();
            PaletteList.SelectedIndices.Clear();
            PaletteList.SelectedIndices.Add(0);
            ShowSelectedEntry();
        }

        int selectedPaletteIndex {
            get {
                int selectionIndex = 0;
                if (PaletteList.SelectedIndices.Count != 0)
                    selectionIndex = PaletteList.SelectedIndices[0];

                return selectionIndex;
            }
        }

        private void ShowSelectedEntry() {
            BeginControlUpdate();

            int selectionIndex = 0;
            if(PaletteList.SelectedIndices.Count != 0)
                selectionIndex = PaletteList.SelectedIndices[0];


            var pointer = GetPpuMacroPointer(selectionIndex);
            var macro = new PpuMacro();
            if(pointer.IsLevelBank)
                macro = GetPpuMacro(selectionIndex);


            txtPointer.Value = pointer.Value;
            if (pointer.IsLevelBank){
                txtPpuPointer.Enabled = true;
                txtMacroBytes.Enabled = true;
                txtPpuPointer.Value = macro.PpuDestination.Value;
                txtMacroBytes.Value = macro.MacroSize;
            }else {
                txtPpuPointer.Enabled = false;
                txtMacroBytes.Enabled = false;
                txtPpuPointer.Text = string.Empty;
                txtMacroBytes.Text = string.Empty;
            }
                PaletteControl.Macro = macro;

                EndControlUpdate();
        }

        private void PopulateListView() {
            PaletteList.BeginUpdate();
            for (int i = 0; i < Level.PaletteCount; i++) {
                pCpu PpuMacroPointer = GetPpuMacroPointer(i);
                if (PpuMacroPointer.IsLevelBank) {
                    //var macro = GetPpuMacro(i);
                    //if (macro.IsPaletteMacro)
                        AddEntry(i);
                    //else
                    //    AddInvalidEntry_BadPpuTarget(i);
                } else {
                    AddInvalidEntry_BadPointer(i);
                }
            }
            PaletteList.EndUpdate();
        }

        private void AddEntry(int index) {
            ListViewItem newItem = GetEmptyItem(index);

            UpdateValidItem(index, newItem);

            PaletteList.Items.Add(newItem);
        }

        private void AddInvalidEntry_BadPointer(int index) {
            ListViewItem newItem = GetEmptyItem(index);

            UpdateInvalidItem(index, newItem);

            PaletteList.Items.Add(newItem);
        }

        /// <summary>
        /// Returns a new ListViewItem containing all necessary subitems, but all initialized with empty strings.
        /// </summary>
        /// <returns></returns>
        ListViewItem GetEmptyItem(int index) {
            ListViewItem newItem = new ListViewItem(index.ToString());
            newItem.SubItems.Add("");
            newItem.SubItems.Add("");
            newItem.SubItems.Add("");
            newItem.SubItems.Add("");
            newItem.SubItems.Add("");

            return newItem;
        }




        //private void AddInvalidEntry_BadPpuTarget(int index) {
        //    ListViewItem newItem = new ListViewItem(index.ToString());

        //}

        pCpu GetPpuMacroPointer(int index) {
            var pointerTable = Level.ToPRom(Level.PalettePointers_Offset);
            return Level.Rom.GetPointer(pointerTable + index * 2);
        }
        PpuMacro GetPpuMacro(int index) {
            var offset = Level.ToPRom(GetPpuMacroPointer(index));
            return new PpuMacro(Level.Rom, offset);
        }

        private void UnloadLevel() {
            if (Level != null) {
                ClearListView();    
            }
            Level = null;
        }

        private void ClearListView() {
            PaletteList.Items.Clear();
        }


        private void PaletteList_SelectedIndexChanged(object sender, EventArgs e) {
            ShowSelectedEntry();
        }
        void PaletteControl_ColorGrabbed(object sender, EventArgs<int> e) {
            colorPicker.Selection = e.Value;
        }



        int controlUpdateLevel = 0;
        void BeginControlUpdate() {
            controlUpdateLevel++;
        }
        void EndControlUpdate() {
            controlUpdateLevel--;
        }
        bool IsUpdatingControls { get { return controlUpdateLevel != 0; } }

        private void txtPointer_TextChanged(object sender, EventArgs e) {
            if (IsUpdatingControls) return;
            if (string.IsNullOrEmpty(txtPointer.Text.Trim())) return;

            Program.PerformAction(Program.Actions.SetPalMacroOffset(Level, selectedPaletteIndex, new pCpu(txtPointer.Value)));
        }

        private void txtPpuPointer_TextChanged(object sender, EventArgs e) {
            if (IsUpdatingControls) return;
            if (string.IsNullOrEmpty(txtPpuPointer.Text.Trim())) return;


            Program.PerformAction(Program.Actions.SetPalMacroDest(Level,selectedPaletteIndex,new pCpu(txtPpuPointer.Value)));
        }

        private void txtMacroBytes_TextChanged(object sender, EventArgs e) {
            if (IsUpdatingControls) return;
            if (string.IsNullOrEmpty(txtMacroBytes.Text.Trim())) return;

            Program.PerformAction(Program.Actions.SetPalMacroSize(Level, selectedPaletteIndex, txtMacroBytes.Value));
        }

        void PaletteControl_CellClicked(object sender, EventArgs<Point> e) {
            Program.PerformAction(Program.Actions.SetPalMacroByte(Level, selectedPaletteIndex, e.Value.X, (byte)colorPicker.Selection));
        }



        internal void NotifyAction(Editroid.UndoRedo.EditroidAction a, bool undo) {
            var advancedPaletteEdit = a as AdvancedPaletteEdit;
            var palEdit = a as Actions.SetPaletteColor;

            if (advancedPaletteEdit != null && advancedPaletteEdit.Level == Level) {
                for (int i = 0; i < advancedPaletteEdit.Edits.Count ; i++) {
                    var edit = advancedPaletteEdit.Edits[i];
                    UpdateEntry(edit.ppuMacroIndex);
                }
            }
            if (palEdit != null) {
                if (palEdit.Type == PaletteType.Background || palEdit.Type == PaletteType.Sprite || palEdit.Type == PaletteType.ZeroEntry) {
                    UpdateEntry(0);
                }
                if (palEdit.Type == PaletteType.AltBackground || palEdit.Type == PaletteType.AltSprite || palEdit.Type == PaletteType.ZeroEntry) {
                    UpdateEntry(5);
                }
            }
        }

        private void UpdateEntry(int index) {
            UpdateEntry(index, PaletteList.Items[index]);
        }
        private void UpdateEntry(int index, ListViewItem item) {
            var pointer = Level.PalettePointers[index];
            if (pointer.IsLevelBank)
                UpdateValidItem(index, item);
            else
                UpdateInvalidItem(index, item);

            if (index == selectedPaletteIndex) {
                ShowSelectedEntry();
            }
        }
        private void UpdateValidItem(int index, ListViewItem newItem) {

            var offset = Level.ToPRom(GetPpuMacroPointer(index));
            var macro = GetPpuMacro(index);

            newItem.SubItems[0].Text = "$" + index.ToString("X2") + "/" + index.ToString();
            newItem.SubItems[1].Text = ((int)offset).ToString("X");
            newItem.SubItems[2].Text = macro.PpuDestination.ToString();
            newItem.SubItems[3].Text = (macro.MacroSize.ToString());

            StringBuilder itemText = new StringBuilder(macro.MacroSize * 3);
            for (int i = 0; i < macro.MacroSize; i++) {
                itemText.Append(macro.GetMacroByte(i).ToString("X2"));
                itemText.Append(' ');
            }

            newItem.SubItems[4].Text = (itemText.ToString());
            string desc = "";
            if (index < Descriptions.Length) {
                desc = Descriptions[index];
            }
            newItem.SubItems[5].Text = desc;
        }

        private void UpdateInvalidItem(int index, ListViewItem newItem) {
            var pointer = GetPpuMacroPointer(index);

            newItem.SubItems[0].Text = index.ToString();
            newItem.SubItems[1].Text = ("[" + pointer.ToString() + "]");
            newItem.SubItems[2].Text = string.Empty;
            newItem.SubItems[3].Text = string.Empty;
            newItem.SubItems[4].Text = string.Empty;
        }

    }

}
