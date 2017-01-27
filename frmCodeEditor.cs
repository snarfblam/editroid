using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM.Projects;
using System.IO;

namespace Editroid
{
    public partial class frmCodeEditor : Form
    {
        pseudofiles loadedDocument;
        Screen loadedScreen;
        string loadedUserFile;


        public frmCodeEditor() {
            InitializeComponent();
        }

        private void frmCodeEditor_Load(object sender, EventArgs e) {
            Application.Idle += new EventHandler(Application_Idle);
        }

        bool updateUndoRedo;
        void Application_Idle(object sender, EventArgs e) {
            if (updateUndoRedo) {
                updateUndoRedo = false;
                btnUndo.Enabled = editor.CanUndo;
                btnRedo.Enabled = editor.CanRedo;
            }
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);

            Application.Idle -= new EventHandler(Application_Idle);

        }
        /// <summary>Updates the form's caption to indicate the specified filename</summary>
        /// <param name="documentName"></param>
        internal void SetDocumentName(string documentName) {
            Text = "Code Editor - " + documentName;
        }

        internal void LoadScreen(Screen screen) {

            //loadedDocument = pseudofiles.screenLoad;
            //SetDocumentName(screen.Level.Index.ToString() + " Screen " + screen.Index.ToString("x"));
            //editor.Text = screen.ScreenLoadASM ?? string.Empty ;
            LoadDocument(
                pseudofiles.screenLoad,
                screen.Level.Index.ToString() + " Screen " + screen.Index.ToString("x"),
                screen.ScreenLoadASM ?? GetScreenDefaultASM(screen));
            loadedScreen = screen;

            Show();
        }

        static readonly string defaultScreenLoadASM = Environment.NewLine + "; Remember, kids, don't forget to RTS!" + Environment.NewLine + "    rts" + Environment.NewLine;
        private string GetScreenDefaultASM(Screen screen) {
            return "; " + screen.Level.Index.ToString() + " Screen " + screen.Index.ToString("X2") + Environment.NewLine + defaultScreenLoadASM;
        }

        internal void LoadExpansionFile() {
            LoadDocument(
                pseudofiles.expansionFile,
                "Expansion",
                Program.MainForm.GameProject.ExpansionFile);
            Show();
        }

        internal void SelectLine(int line, bool focus) {

            if (focus) {
                editor.Focus();
                BringToFront();
            }

            editor.SelectLine(line);

        }

        internal void LoadGeneralFile() {
            LoadDocument(
                pseudofiles.generalFile,
                "General Code",
                Program.MainForm.GameProject.GeneralCodeFile);
            Show();
        }

        internal void LoadUserFile(string name) {
            var file = GetFile(name);
            if (file == null) return;

            LoadDocument(pseudofiles.userFile, name, file.Code);
            chkAutoBuild.Checked = file.AutoBuild;
            loadedUserFile = name;
            Show();
        }
        AsmFile GetFile(string name) {
            var files = Program.MainForm.GameProject.Files;
            for (int i = 0; i < files.Count; i++) {
                if (string.Equals(name, files[i].Name, StringComparison.OrdinalIgnoreCase)) {
                    return files[i];
                }
            }

            return null;
        }
        private void LoadDocument(pseudofiles type, string name, string text) {
            SaveCurrentDocument();

            loadedDocument = type;
            SetDocumentName(name);
            editor.Text = text ?? string.Empty;

            editor.ScrollToTop();
            editor.ClearUndo();
            btnUndo.Enabled = btnRedo.Enabled = false;

            mnuFile.Visible = FileMenuSeparator.Visible = (type == pseudofiles.userFile);
        }

        /// <summary>
        /// Copies the contents of the text editor to project or screen data.
        /// </summary>
        public void SaveCurrentDocument() {
            switch (loadedDocument) {
                case pseudofiles.screenLoad:
                    if (loadedScreen.ScreenLoadASM != editor.Text) Program.MainForm.GameProject.UnsavedChanges = true;
                    loadedScreen.ScreenLoadASM = editor.Text;
                    break;
                case pseudofiles.expansionFile:
                    if (Program.MainForm.GameProject.ExpansionFile != editor.Text) Program.MainForm.GameProject.UnsavedChanges = true;
                    Program.MainForm.GameProject.ExpansionFile = editor.Text;
                    break;
                case pseudofiles.generalFile:
                    if (Program.MainForm.GameProject.GeneralCodeFile != editor.Text) Program.MainForm.GameProject.UnsavedChanges = true;
                    Program.MainForm.GameProject.GeneralCodeFile = editor.Text;
                    break;
                case pseudofiles.declarations:
                    if (Program.MainForm.GameProject.DefinesFile != editor.Text) Program.MainForm.GameProject.UnsavedChanges = true;
                    Program.MainForm.GameProject.DefinesFile = editor.Text;
                    break;
                case pseudofiles.userFile:
                    var file = GetFile(loadedUserFile);
                    if (file.Code != editor.Text) Program.MainForm.GameProject.UnsavedChanges = true;
                    file.Code = editor.Text;
                    break;
                case pseudofiles.none:
                    break;
                default:
                    break;
            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            SaveCurrentDocument();
        }

        private enum pseudofiles
        {
            none,
            screenLoad,
            expansionFile,
            generalFile,
            declarations,
            userFile
        }

        private void frmCodeEditor_Deactivate(object sender, EventArgs e) {
            SaveCurrentDocument();
            toolStrip1.Enabled = false;
        }


        private void frmCodeEditor_Activated(object sender, EventArgs e) {
            toolStrip1.Enabled = true;
        }

        internal void LoadDeclaresFile() {
            LoadDocument(
                pseudofiles.declarations,
                "Defines",
                Program.MainForm.GameProject.DefinesFile);
            Show();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e) {
            editor.Cut();
        }

        void editor_UndoRedoChanged(object sender, EventArgs e) {
            //btnUndo.Enabled = editor.CanUndo;
            //btnRedo.Enabled = editor.CanRedo;
            updateUndoRedo = true;
        }

        private void copyToolStripButton_Click(object sender, EventArgs e) {
            editor.Copy();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e) {
            editor.Paste();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            editor.ToggleComment();
        }

        private void btnIndent_Click(object sender, EventArgs e) {
            editor.Indent();
        }

        private void btnUnindent_Click(object sender, EventArgs e) {
            editor.Unindent();
        }

        private void btnUndo_Click(object sender, EventArgs e) {
            editor.Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e) {
            editor.Redo();
        }

        private void toolStripButton3_Click(object sender, EventArgs e) {
            editor.ShowFind();
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            editor.ShowReplace();
        }

        private void mnuExport_Click(object sender, EventArgs e) {
            string filename = loadedUserFile;
            
            if (!string.Equals(Path.GetExtension(filename), ".asm", StringComparison.OrdinalIgnoreCase)) {
                filename = Path.ChangeExtension(filename, ".asm");
            }

            FileExporter.FileName = filename;
            if (FileExporter.ShowDialog() == DialogResult.OK) {
                string error = null;
                try {
                    File.WriteAllText(FileExporter.FileName, editor.Text);
                } catch (IOException) {
                    error = "An IO error occurred";
                } catch (NotSupportedException) {
                    error = "Invalid path or unsupported operation";
                } catch (UnauthorizedAccessException) {
                    error = "Unauthorized access";
                }

                if (error != null) MessageBox.Show("Failed to export file:" + error, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuRename_Click(object sender, EventArgs e) {
            List<string> takenNames = new List<string>();
            takenNames.AddRange(ROM.Projects.Project.ReservedFilenames);

            foreach (var file in Program.MainForm.GameProject.Files) {
                if (!file.Name.Equals(loadedUserFile, StringComparison.OrdinalIgnoreCase)) {
                    takenNames.Add(file.Name);
                }
            }

            using (var frm = new FilenameForm()) {
                frm.Text = "Rename File";
                frm.EnteredFilename = loadedUserFile;
                frm.ReservedFilenames = takenNames;

                if (Program.Dialogs.ShowDialog(frm, this) == DialogResult.OK) {
                    Program.MainForm.GameProject.GetFileByName(loadedUserFile).Name = frm.EnteredFilename;
                    loadedUserFile = frm.EnteredFilename;
                    SetDocumentName(frm.EnteredFilename);
                }
            }
        }

        private void mnuDelete_Click(object sender, EventArgs e) {
            if (MessageBox.Show("The file \"" + loadedUserFile + "\" will be deleted permanently.", "Delete File", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
                Close();
                var file = Program.MainForm.GameProject.GetFileByName(loadedUserFile);
                Program.MainForm.GameProject.Files.Remove(file);
            }
        }

        private void chkAutoBuild_Click(object sender, EventArgs e) {
            chkAutoBuild.Checked = !chkAutoBuild.Checked;

            var file = Program.MainForm.GameProject.TryGetFileByName(loadedUserFile);
            if (file != null) {
                file.AutoBuild = chkAutoBuild.Checked;
            }
        }


    }
}