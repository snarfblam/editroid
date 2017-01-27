using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;
using Editroid.Actions;

namespace Editroid
{
    public partial class PasswordDetailEditor : UserControl
    {
        public PasswordDetailEditor() {
            InitializeComponent();
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { 
                rom = value;
                if (rom != null)
                    LoadData();
            }
        }

        private void LoadData() {
            BeginUpdate();
            entryList.Items.Clear();

            for (int i = 0; i < PasswordData.DataCount; i++) {
                entryList.Items.Add(rom.PasswordData.GetDatum(i));
            }

            EndUpdate();
        }

        int updateLevel = 0;
        void BeginUpdate() {
            updateLevel++;
        }
        void EndUpdate() {
            updateLevel--;
        }
        bool isUpdating { get { return updateLevel != 0; } }
        internal void SelectEntry(int index) {
            BeginUpdate();
            entryList.SelectedIndex = index;
            ShowValueForSelectedItem();
            EndUpdate();
        }

        private void ShowValueForSelectedItem() {
            if (entryList.SelectedIndex < 0) return;

            BeginUpdate();

            valueList.SelectedIndices.Clear();
            int numericValue = rom.PasswordData.GetDatum(entryList.SelectedIndex).Item;
            if(numericValue < valueList.Items.Count )
                valueList.Items[numericValue].Selected = true;

            EndUpdate();
        }

        private void entryList_SelectedIndexChanged(object sender, EventArgs e) {
            if (isUpdating) return;

            BeginUpdate();

            OnEntrySelected();
            ShowValueForSelectedItem();

            EndUpdate();
        }
        protected virtual void OnEntrySelected() {
            if (EntrySelected != null)
                EntrySelected(this, new EventArgs());
        }
        public event EventHandler EntrySelected;


        public int SelectedEntryIndex {
            get {
                return entryList.SelectedIndex;
            }
        }

        private void valueList_SelectedIndexChanged(object sender, EventArgs e) {
            if (isUpdating || valueList.SelectedIndices.Count == 0 || entryList.SelectedIndex == -1) return;
            if (rom.PasswordData.GetDatum(entryList.SelectedIndex).Item == valueList.SelectedIndices[0]) return;
           
            Program.PerformAction(
                Program.Actions.SetPassDataItem(entryList.SelectedIndex, valueList.SelectedIndices[0])
            );
        }

        internal void NotifyAction(Editroid.UndoRedo.EditroidAction a) {
            PasswordDataAction action = a as PasswordDataAction;
            if (action != null) {
                entryList.Items[action.DataIndex] = rom.PasswordData.GetDatum(action.DataIndex);
            }
            if (a is OverwritePasswordData) {
                for (int i = 0; i < entryList.Items.Count; i++) {
                    RefreshEntry(i);
                    ShowValueForSelectedItem();
                }
            }
            ////if (a is SetItemTilePosition && ((SetItemTilePosition)a).UpdatesPassword) {
            ////    int index = ((SetItemTilePosition)a).PasswordDataIndex;
            ////    entryList.Items[index] = rom.PasswordData.GetDatum(index);
            ////}
            ////if (a is SetItemRowPosition) {
            ////    ((SetItemRowPosition)a).ForEachPasswordEntry(delegate(int i) {
            ////        RefreshEntry(i);
            ////    });
            ////}
        }

        private void RefreshEntry(int i) {
            entryList.Items[i] = rom.PasswordData.GetDatum(i);
        }

        private void valueList_SizeChanged(object sender, EventArgs e) {
        }
    }
}
