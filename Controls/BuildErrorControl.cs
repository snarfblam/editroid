using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM.Projects;

namespace Editroid.Controls
{
    public partial class BuildErrorControl : UserControl
    {
        List<BuildError> _errors = new List<BuildError>();

        public BuildErrorControl() {
            InitializeComponent();
        }

        internal void ShowErrors(IList<BuildError> errors) {
            _errors.Clear();

            errorList.BeginUpdate();
            errorList.Items.Clear();

            AddErrors(errors);

            errorList.EndUpdate();
        }

        private void AddErrors(IList<BuildError> errors) {
            // Subitems:
            // - List number
            // - File
            // - Line
            // - Error code/type
            // - Message

            foreach (var error in errors) {
                // #
                ListViewItem i = new ListViewItem(errorList.Items.Count.ToString());

                // File
                i.SubItems.Add(ParseErrorFileName(error));

                // Line
                i.SubItems.Add((error.Line > 0)? error.Line.ToString() : string.Empty);

                // Code / type
                if (error.ErrorCode == 0 && string.IsNullOrEmpty(error.ErrorName)) {
                    i.SubItems.Add(string.Empty);
                }else{
                    if (string.IsNullOrEmpty(error.ErrorName)) {
                        i.SubItems.Add(error.ErrorCode.ToString());
                    } else {
                        i.SubItems.Add(error.ErrorCode.ToString() + " - " + error.ErrorName);
                    }
                }

                // Message
                string warning = (error.ErrorType == BuildErrorType.Warning ) ? "(Warning) " : string.Empty;
                i.SubItems.Add(warning + error.Message);

                errorList.Items.Add(i);

                _errors.Add(error);
            }
        }

        private static string ParseErrorFileName(BuildError error) {
            if (error.File == null) return string.Empty;
            if (error.File.Length == 5 && error.File[0] == 'l' && error.File[2] == 's') {
                int level, screen;

                if (Editroid.ROM.Projects.Project.Builder.ParseScreenFilename(error.File, out level, out screen)) {
                    return ((LevelIndex)level).ToString() + " " + screen.ToString("X2") + " Load";
                }
            }
            if (error.File.StartsWith(ROM.Projects.Project.Builder.UserFilePrefix)) return error.File.Substring(ROM.Projects.Project.Builder.UserFilePrefix.Length);
            return error.File;
        }

        public bool ShowCaption { get { return label1.Visible; } set { label1.Visible = linkLabel1.Visible = value; } }

        private void errorList_ItemActivate(object sender, EventArgs e) {
            if (errorList.SelectedIndices.Count == 1) {
                int index = errorList.SelectedIndices[0];

                var error = _errors[index];
                string errorFile = error.File ?? "this is not the file you are looking for";
                if (errorFile.Equals(Project.generalCodeFilename, StringComparison.OrdinalIgnoreCase)) {
                    Program.CodeEditor.LoadGeneralFile();
                    Program.CodeEditor.SelectLine(error.Line, true);
                }
                if (errorFile.Equals(Project.declaresFilename, StringComparison.OrdinalIgnoreCase)) {
                    Program.CodeEditor.LoadDeclaresFile();
                    Program.CodeEditor.SelectLine(error.Line, true);
                }
                if (errorFile.Equals(Project.expansionCodeFilename, StringComparison.OrdinalIgnoreCase)) {
                    Program.CodeEditor.LoadExpansionFile();
                    Program.CodeEditor.SelectLine(error.Line, true);
                }
                if (errorFile.StartsWith(ROM.Projects.Project.Builder.UserFilePrefix)) {
                    string userFileName = error.File.Substring(ROM.Projects.Project.Builder.UserFilePrefix.Length);
                    Program.CodeEditor.LoadUserFile(userFileName);
                    Program.CodeEditor.SelectLine(error.Line, true);
                }

                int level, screen;
                if (Project.Builder.ParseScreenFilename(errorFile, out level, out screen)) {
                    var levelData = Program.MainForm.GameRom.Levels[(LevelIndex)level];
                    if(screen < levelData.Screens.Count){
                        Program.CodeEditor.LoadScreen(levelData.Screens[screen]);
                    }
                    Program.CodeEditor.SelectLine(error.Line,true);
                }
                var file = Program.MainForm.GameProject.TryGetFileByName(errorFile);
                if (file != null) {
                    Program.CodeEditor.LoadUserFile(file.Name);
                    Program.CodeEditor.SelectLine(error.Line, true);
                }
            }
        }
    }
}
