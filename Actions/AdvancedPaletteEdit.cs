using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid.Actions
{
    class AdvancedPaletteEdit:LevelAction
    {
        List<Edit> edits = new List<Edit>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="l"></param>
        /// <param name="edit"></param>
        /// <param name="ppuMacroIndex"></param>
        /// <param name="newValue"></param>
        /// <param name="macroByteIndex">Applied only to edits to actual macro data.</param>
        public AdvancedPaletteEdit(EditroidUndoRedoQueue q, Level l, AdvancedPaletteEditType edit, int ppuMacroIndex, int newValue, int macroByteIndex)
            : base(q, l) {
            Edit newEdit;
            newEdit.editType = edit;
            newEdit.newValue = newValue;
            newEdit.ppuMacroIndex = ppuMacroIndex;
            newEdit.oldValue = GetCurrentValue(edit, ppuMacroIndex,macroByteIndex);
            newEdit.macroByteIndex = macroByteIndex;

            edits.Add(newEdit);

            if (edit != AdvancedPaletteEditType.DataOffset) {
                var pointer = Level.PalettePointers[ppuMacroIndex];
                if (!pointer.IsLevelBank)
                    throw new InvalidOperationException("Attempted to modify ppu macro data, but the pointer to the data is invalid.");
            }
        }
        public AdvancedPaletteEdit(EditroidUndoRedoQueue q, Level l, AdvancedPaletteEditType edit, int ppuMacroIndex, int newValue)
        :this(q,l,edit,ppuMacroIndex,newValue,-1){
        }

        private int GetCurrentValue(AdvancedPaletteEditType edit, int ppuMacroIndex) {
            return GetCurrentValue(edit, ppuMacroIndex, -1);
        }
        private int GetCurrentValue(AdvancedPaletteEditType edit, int ppuMacroIndex, int dataByteIndex) {
            var pointer = Level.PalettePointers[ppuMacroIndex];
            PpuMacro macro = new PpuMacro();
            if (pointer.IsLevelBank)
                macro = Level.GetPaletteMacro(ppuMacroIndex);

            switch (edit) {
                case AdvancedPaletteEditType.DataOffset:
                    return pointer.Value;
                    break;
                case AdvancedPaletteEditType.DataDest:
                    return macro.PpuDestination.Value;
                    break;
                case AdvancedPaletteEditType.DataSize:
                    return macro.MacroSize;
                    break;
                case AdvancedPaletteEditType.MacroDataEdit:
                    if(dataByteIndex == -1)
                        throw new InvalidOperationException("AdvancedPaletteEdit.GetCurrentValue: edit type was MacroDataEdit, but a macro byte index was not specified.");

                    return macro.GetMacroByte(dataByteIndex);
                default:
                    throw new ArgumentException("AdvancedPaletteEdit.GetCurrentValue: Invalid edit type.");
            }
        }
        /// <summary>
        /// Returns true if an edit is made to a level's bg or sprite palette. This does not include alternate palettes.
        /// This also does not account for the possibility of two palettes using the same or overlapping data, which
        /// could allow editing of another palette to affect the bg or sprite palette.
        /// </summary>
        public bool EditsLevelPalette {
            get {
                for (int i = 0; i < edits.Count; i++) {
                    var index = edits[i].ppuMacroIndex;
                    if (index == 0) return true;
                }
                    return false;
            }
        }
                /// <summary>
        /// Returns true if an edit is made to a level's alternate palette.
        /// This does not account for the possibility of two palettes using the same or overlapping data, which
        /// could allow editing of another palette to affect the alternate palette.
        /// </summary>
        public bool EditsAltLevelPalette {
            get {
                for (int i = 0; i < edits.Count; i++) {
                    var index = edits[i].ppuMacroIndex;
                    if (index == 5) return true;
                }
                    return false;
            }
        }
        public override void Do() {
            for (int i = 0; i < edits.Count; i++) {
                DoEdit(i, false);
            }
        }

        public override void Undo() {
            for (int i = edits.Count - 1; i >= 0; i--) {
                DoEdit(i, true);
            }
        }

        void DoEdit(int i, bool undo) {
            var edit = edits[i];
            var newValue = undo ? edit.oldValue : edit.newValue;
            var macro = Level.GetPaletteMacro(edit.ppuMacroIndex);
            switch (edit.editType) {
                case AdvancedPaletteEditType.DataOffset:
                    Level.PalettePointers[edit.ppuMacroIndex] = new pCpu(newValue);
                    break;
                case AdvancedPaletteEditType.DataDest:
                    macro.PpuDestination = new pCpu(newValue);
                    break;
                case AdvancedPaletteEditType.DataSize:
                    macro.MacroSize = (byte)newValue;
                    break;
                case AdvancedPaletteEditType.MacroDataEdit:
                    Level.GetPaletteMacro(edit.ppuMacroIndex).WriteMacroByte(edit.macroByteIndex, (byte)newValue);
                    break;
                default:
                    throw new InvalidOperationException("AdvancedPaletteEdit.DoEdit: Invalid edit type");
            }
        }


        public override string GetText() {
            if (edits.Count > 1)
                return "Multiple palette edits";
            int editTypeIndex = (int)edits[0].editType;
            if (editTypeIndex < 0 || editTypeIndex >= EditNames.Length)
                return "Misc. palette edit";
            return EditNames[editTypeIndex];
        }

        IList<Edit> readOnlyEdits;

        public IList<Edit> Edits { get {
                if (readOnlyEdits == null)
                    readOnlyEdits = edits.AsReadOnly();
                return readOnlyEdits;
            }
        }

        public struct Edit
        {
            public AdvancedPaletteEditType editType;
            public int ppuMacroIndex;
            public int oldValue, newValue;
            /// <summary>
            /// Only relevant to macro data edits
            /// </summary>
            public int macroByteIndex;
        }

        static string[] EditNames = { "Edit palette data offset", "Edit palette data destination", "Edit palette data size", "Edit palette data" };
    }
    public enum AdvancedPaletteEditType:byte
    {
        DataOffset,
        DataDest,
        DataSize,
        MacroDataEdit,
    }
}
