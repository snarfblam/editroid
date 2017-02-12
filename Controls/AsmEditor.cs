using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Romulus.Plugin;
using FastColoredTextBoxNS;

namespace MultiMaster.Editors
{
    public partial class AsmEditor : Control
    {
        public AsmEditor() {
            InitializeComponent();

            Controls.Remove(menuStrip1);

            // Try to get consolas font
            FontFamily consolasFamily = null;
            try {
                consolasFamily = new FontFamily("Consolas");
            } catch (ArgumentException) {
                // Font is not installed
            }

            if (consolasFamily == null) {
                EditBox.Font = new Font(FontFamily.GenericMonospace, Font.Size);
            } else {
                EditBox.Font = new Font(consolasFamily, Font.Size);
            }


        }


        #region old SuiteNES overrides
        protected void OnLoadFile(string filename) {
            //base.OnLoadFile(filename);

            EditBox.Text = System.IO.File.ReadAllText(filename);
            EditBox.ClearUndo();
        }

        protected void OnSaveFile() {
            //base.OnSaveFile();

            //System.IO.File.WriteAllText(Filename, EditBox.Text);
        }
        protected void OnInitialize() {
            //base.OnInitialize();

            //Host.UI.Menu = this.menuStrip1;
        }
        protected void OnCopy() {
            //base.OnCopy();

            EditBox.Copy();
        }
        protected void OnCut() {
            //base.OnCut();

            EditBox.Cut();
        }
        protected void OnPaste() {
            //base.OnPaste();

            EditBox.Paste();
        }
        protected void OnClipboardChanged() {
            //base.onclipboardchanged();

            //Host.UI.CanPaste = Host.Clipboard.HasText;
        }
        #endregion

        public override string Text {
            get {
                return EditBox.Text;
            }
            set {
                EditBox.Text = value;
                base.Text = value;
            }
        }
        private void mnuCut_Click(object sender, EventArgs e) {
            EditBox.Cut();
        }

        private void mnuCopy_Click(object sender, EventArgs e) {
            EditBox.Copy();
        }

        private void mnuPaste_Click(object sender, EventArgs e) {
            EditBox.Paste();
        }


        private void UpdateUndoRedo() {
            UndoRedoChanged.Raise(this);
        }


        #region Syntax highlighting
        TextStyle normalStyle = new TextStyle(SystemBrushes.WindowText, null, FontStyle.Regular);
        TextStyle commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        TextStyle directiveStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        TextStyle instructionStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle labelStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);

        private void EditBox_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            e.ChangedRange.ClearStyle(StyleIndex.All);
            e.ChangedRange.SetStyle(commentStyle, ";.*$", System.Text.RegularExpressions.RegexOptions.Multiline);
            e.ChangedRange.SetStyle(labelStyle, @"^(\s*(@?([a-z][a-z0-9_]*:)|\+|\-|\*))+", System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(directiveStyle, @"(\s|^)\.([a-z]*)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(instructionStyle, @"\b(BRK|PHP|BPL|CLC|ORA|ASL|ASO|JSR|PLP|ANC|BIT|BMI|SEC|AND|ROL|RLA|RTI|PHA|ALR|BVC|CLI|EOR|LSR|LSE|RTS|PLA|ARR|JMP|BVS|SEI|ADC|ROR|RRA|DEY|TXA|XAA|BCC|STY|STX|AXS|TYA|TXS|TAS|SAY|STA|XAS|AXA|TAY|TAX|OAL|BCS|CLV|TSX|LAS|LDY|LDA|LDX|LAX|INY|DEX|SAX|CPY|BNE|CLD|CMP|DEC|DCM|INX|CPX|BEQ|KIL|DOP|SED|NOP|TOP|SBC|INC|INS)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers(@"^\s*;?{", @"^\s*;?}", System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        private void EditBox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            e.ChangedRange.SetStyle(commentStyle, ";.*$");
            UpdateUndoRedo();
        }
        #endregion

        private void EditBox_AutoIndentNeeded(object sender, AutoIndentEventArgs e) {

            e.ShiftNextLines = 0;
        }

        private void mnuViewCollapse_Click(object sender, EventArgs e) {
            EditBox.CollapseBlock(EditBox.Selection.Start.iLine, EditBox.Selection.End.iLine);

        }

        private void EditBox_KeyDown(object sender, KeyEventArgs e) {

        }

        private void EditBox_KeyPress(object sender, KeyPressEventArgs e) {

        }

        private void EditBox_KeyPressing(object sender, KeyPressEventArgs e) {
            //if (e.KeyChar == '\t' && ModifierKeys == Keys.Shift) {
            //        EditBox.DecreaseIndent();
            //    e.Handled = true;
            //}
        }

        private void cmnEditorMenu_Opening(object sender, CancelEventArgs e) {
            bool hasSelection = EditBox.SelectionLength > 0;
            mnuCopy.Enabled = mnuCut.Enabled = hasSelection;
            mnuPaste.Enabled = Clipboard.ContainsText();
        }

        private void EditBox_SelectionChanged(object sender, EventArgs e) {

        }



        private void mnuSelectAll_Click(object sender, EventArgs e) {
            EditBox.SelectAll();
        }

        private void increaseIndentToolStripMenuItem_Click(object sender, EventArgs e) {
            EditBox.IncreaseIndent();
        }

        private void decreaseIndentToolStripMenuItem_Click(object sender, EventArgs e) {
            EditBox.DecreaseIndent();
        }

        private void commentToolStripMenuItem_Click(object sender, EventArgs e) {
            EditBox.CommentLines(";");
        }

        private void uncommentToolStripMenuItem_Click(object sender, EventArgs e) {
            EditBox.UncommentLines(";");
        }


        #region IPluginCustomUndo Members

        public bool CanUndo {
            get { return EditBox.UndoCount > 0; }
        }

        public bool CanRedo {
            get { return EditBox.RedoCount > 0; }
        }

        public void Undo() {
            EditBox.Undo();
        }

        public void Redo() {
            EditBox.Redo();
        }

        public event EventHandler UndoRedoChanged;


        #endregion

        private void mnuGotolabel_Click(object sender, EventArgs e) {
            ////int lineNum;
            ////if (LabelDialog.ShowLabelDialog(EditBox.Text, out lineNum) == DialogResult.OK) {
            ////    EditBox.Navigate(lineNum);
            ////}
            throw new NotImplementedException();
        }

        private void collapseSelectionToolStripMenuItem_Click(object sender, EventArgs e) {
            EditBox.CollapseBlock(EditBox.Selection.Start.iLine, EditBox.Selection.End.iLine);
        }

        internal void ScrollToTop() {
            EditBox.SelectionStart = 0;
            EditBox.SelectionLength = 0;
            EditBox.DoCaretVisible();
        }

        internal void SelectLine(int line) {
            line = Math.Min(line, EditBox.LinesCount);
            line -= 1;

            EditBox.Selection.BeginUpdate();
            EditBox.Selection.Start = new Place(0, line);
            EditBox.Selection.End = new Place(EditBox.Lines[line].Length, line);
            EditBox.Selection.EndUpdate();
            EditBox.Invalidate();
            EditBox.DoCaretVisible();
            EditBox.Focus();

        }

        public void Cut() {
            EditBox.Cut();
        }
        public void Copy() {
            EditBox.Copy();
        }
        public void Paste() {
            EditBox.Paste();
        }


        public void ClearUndo() {
            EditBox.ClearUndo();
        }
        public void ToggleComment() {
            EditBox.CommentSelected();
        }
        public void Indent() {
            EditBox.IncreaseIndent();
        }
        public void Unindent() {
            EditBox.DecreaseIndent();
        }



        internal void ShowFind() {
            EditBox.ShowFindDialog();
        }

        internal void ShowReplace() {
            EditBox.ShowReplaceDialog();

        }

        
        FindControl FindControl;
        void wireFindControl() {
            if (FindControl == null) return;
            FindControl.CloseClicked += new EventHandler(FindControl_CloseClicked);
        }
        void unwireFindControl() {
            if (FindControl == null) return;
            FindControl.CloseClicked -= new EventHandler(FindControl_CloseClicked);
        }

        void FindControl_CloseClicked(object sender, EventArgs e) {
            FindControl.Hide();
        }

        private void EditBox_FindFormShow(object sender, EventArgs e) {
            if (EditBox.FindControl != FindControl) {
                Controls.Remove(FindControl);
                unwireFindControl();
                if (FindControl != null) FindControl.Dispose();
                FindControl = EditBox.FindControl;
                wireFindControl();
            }

            if (!Controls.Contains(FindControl)) {
                SuspendLayout();
                Controls.Add(FindControl);
                FindControl.SendToBack();
                FindControl.Dock = DockStyle.Bottom;
                ResumeLayout();
            } else {
                FindControl.Show();
                FindControl.Focus();
            }
        }

        private void EditBox_FindFormClose(object sender, EventArgs e) {
            if (FindControl != null && !FindControl.IsDisposed) {
                FindControl.Hide();
            }
        }
    }
}
