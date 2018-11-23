using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Editroid.UndoRedo;
using System.Security;
using System.Diagnostics;

namespace Editroid
{
	static class Program
	{
		/// <summary>
		/// The main entry points for the application.
		/// </summary>
		[STAThread]
		static void Main() {
            ProgramDirectory = Application.StartupPath;
            PluginDirectory = Path.Combine(ProgramDirectory, "Plugin");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            //(new frmBankAllocation()).Show();
            //PluginManager.LoadPlugins();

            TemporaryFileManager.DeleteTemporaryFiles();
            mainForm = new frmMain();
			Application.Run(mainForm);

            TemporaryFileManager.DeleteTemporaryFiles();
		}

        static frmMain mainForm;
        public static frmMain MainForm { get { return mainForm; } }


        static frmStruct structForm;
        public static bool StructFormIsLoaded { get { return structForm != null && !structForm.IsDisposed; } }
        public static frmStruct StructForm {
            get {
                if (!StructFormIsLoaded)
                    LoadStructForm();

                return structForm;
            }
        }
    
        public static void LoadStructForm() {
            if (StructFormIsLoaded) return;

            structForm = new frmStruct();
            structForm.Owner = mainForm;
            mainForm.InitializeStructureEditorData(structForm);
        }

        static frmBuildErrors _buildErrorForm;
        public static bool BuildErrorFormIsLoaded { get { return _buildErrorForm != null && !_buildErrorForm.IsDisposed; } }
        public static frmBuildErrors BuildErrorForm {
            get {
                if (!BuildErrorFormIsLoaded)
                    LoadBuildErrorForm();

                return _buildErrorForm;
            }
        }

        private static void LoadBuildErrorForm() {
            if (BuildErrorFormIsLoaded) return;

            _buildErrorForm = new frmBuildErrors();
            _buildErrorForm.Owner = mainForm;
        }

        static frmPalette paletteForm;
        public static bool PaletteFormIsLoaded { get { return paletteForm != null && !paletteForm.IsDisposed; } }
        public static frmPalette PaletteForm {
            get {
                if (!PaletteFormIsLoaded)
                    LoadPaletteForm();

                return paletteForm;
            }
        }

        public static void LoadPaletteForm() {
            if (PaletteFormIsLoaded) return;

            paletteForm = new frmPalette();
            paletteForm.Owner = mainForm;
            mainForm.InitializePaletteEditorData(paletteForm);
        }

        static frmCodeEditor codeEditor;
        public static bool CodeEditorLoaded { get { return codeEditor != null && !codeEditor.IsDisposed; } }
        public static frmCodeEditor CodeEditor {
            get {
                if (!CodeEditorLoaded)
                    LoadCodeEditor();

                return codeEditor;
            }
        }

        private static void LoadCodeEditor() {
            if (CodeEditorLoaded) {
                return;
            }

            codeEditor = new frmCodeEditor();
            codeEditor.Owner = mainForm;
        }



        public static ActionGenerator Actions { get { return mainForm.Actions; } }
        public static void PerformAction(EditroidAction a) {
            mainForm.PerformAction(a);
        }

        static frmPointers pointerExplererForm;
        public static frmPointers PointerExplorerForm {
            get {
                LoadPointerExplorerForm();
                return pointerExplererForm;
            }
        }
        public static bool PointerExplorerFormIsLoaded { get { return pointerExplererForm != null && !pointerExplererForm.IsDisposed; } }
        public static void LoadPointerExplorerForm() {
            if (PointerExplorerFormIsLoaded) return;

            pointerExplererForm = new frmPointers();
            pointerExplererForm.SetRom(mainForm.GameRom);
            pointerExplererForm.Owner = mainForm;
        }

        private static TestRomPatcher romPatcher = new TestRomPatcher();

        public static TestRomPatcher RomPatcher {
            get { return romPatcher; }
        }

        public static frmAdvancedPalette AdvancedPalForm { get; set; }
        public static void CreateAdvancedPalForm() {
            if(AdvancedPalForm == null || AdvancedPalForm.IsDisposed)
                AdvancedPalForm = new frmAdvancedPalette();
        }
        public static bool AdvancedPalFormIsLoaded { get { return AdvancedPalForm != null && !AdvancedPalForm.IsDisposed; } }

        public static string ProgramDirectory { get; private set; }
        public static string PluginDirectory { get; private set; }

        public static class TemporaryFileManager
        {
            static Random rand = new Random();

            public static readonly string AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Editroid");
            public static readonly string TempDirectoryPath = Path.Combine(AppDataDirectory, "Temporary");


            /// <summary>
            /// The number of recently used temporary folders to track
            /// </summary>
            const int TempFolderMruLimit = 8;
            /// <summary>
            /// List of recently used temporary folders. Those found in this list will not be deleted until the application closes.
            /// </summary>
            static List<string> TempFolderMRU = new List<string>();

            public static TemporaryFolder GetTemporaryFolder() {
                string folderName = GetRandomName();
                string folderPath = Path.Combine(TempDirectoryPath, folderName);
                while (Directory.Exists(folderName)) {
                    folderName = GetRandomName();
                    folderPath = Path.Combine(TempDirectoryPath, folderName);
                }

                // Add folder to recently used list
                TempFolderMRU.Add(folderPath);
                if (TempFolderMRU.Count > TempFolderMruLimit) {
                    TempFolderMRU.RemoveAt(0);
                }

                return new TemporaryFolder(folderPath);
            }

            private static string GetRandomName() {
                const int nameLength = 6;
                char[] name = new char[nameLength];
                for (int i = 0; i < nameLength; i++) {
                    name[i] = (char)('a' + rand.Next(26));
                }

                return new string(name);
            }

            public static void DeleteTemporaryFiles() {
                var instances = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
                // Don't bother if there are other instances running (they may be using files)
                if (instances.Length <2 ) {
                    if (Directory.Exists(TempDirectoryPath))
                    {
                        string[] dirs = Directory.GetDirectories(TempDirectoryPath);
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            string whatever;
                            bool dirInMRU = null != TempFolderMRU.Find(item => dirs[i].Equals(item, StringComparison.OrdinalIgnoreCase));
                            if (!dirInMRU)
                            {
                                PerformFileOperation(() => Directory.Delete(dirs[i], true), out whatever);
                            }
                        }
                    }
                }
            }


            public class TemporaryFolder
            {
                string path;
                public string Path { get { return path; } }

                public TemporaryFolder(string folderPath) {
                    this.path = folderPath;
                    Directory.CreateDirectory(folderPath);
                }

                public bool WriteFile(string name, byte[] contents, out string errorMsg) {
                    return PerformFileOperation(
                        () => {
                            File.WriteAllBytes(System.IO.Path.Combine(path, name), contents);
                        },
                        out errorMsg);
                }

                public bool WriteFile(string name, string contents, out string errorMsg) {
                    return PerformFileOperation(
                        () => {
                            File.WriteAllText(System.IO.Path.Combine(path, name), contents);
                        },
                        out errorMsg);
                }
                /// <summary>
                /// Gets the full path of the specified file
                /// </summary>
                /// <param name="filename">Filename relative to the temporary directory</param>
                /// <returns></returns>
                public string GetFullPath(string filename) {
                    return System.IO.Path.Combine(path, filename);
                }

            }
        }

        internal delegate void fileop();

        internal static bool PerformFileOperation(fileop operation, out string errorMsg) {
            errorMsg = null;
            try {
                operation();
                return true;
            } catch (ArgumentException) {
                errorMsg = "Invalid file path";
            } catch (PathTooLongException) {
                errorMsg = "Path too long";
            } catch (DirectoryNotFoundException) {
                errorMsg = "Directory not found";
            } catch (IOException) {
                errorMsg = "An IO error occurred.";
            } catch (UnauthorizedAccessException) {
                errorMsg = "Unsupported operation or insufficient permissions";
            } catch (NotSupportedException) {
                errorMsg = "Invalid path format";
            } catch (SecurityException) {
                errorMsg = "Insufficient permissions";
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="showWindow">Shows the window and leaves it open for debugging purposes</param>
        internal static void ExecuteCommand(string command, bool showWindow) {
            try {
                string flag = showWindow ? "/s /k " : "/s /c ";
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd",flag + "\"" + command + "\"");

                //procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                if (!showWindow) {
                    procStartInfo.CreateNoWindow = true;
                }
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                //string result = proc.StandardOutput.ReadToEnd();

                //MessageBox.Show(result);
                // Display the command output.
                //Console.WriteLine(result);
                //} catch (Exception objException) {
                // Log the exception
            } finally {
            }

        }

        static ProgramDialogs _Dialogs = new ProgramDialogs();

        public static ProgramDialogs Dialogs { get { return _Dialogs; } }
    }

    class ProgramDialogs
    {
        List<Form> OpenDialogs = new List<Form>();

        public ProgramDialogs() {

        }

        public DialogResult ShowDialog(Form f, Form owner) {
            f.FormClosed += new FormClosedEventHandler(OnFormClosed);   
            OpenDialogs.Add(f);
            return f.ShowDialog(owner);
        }

        void OnFormClosed(object sender, FormClosedEventArgs e) {
            var f = sender as Form;
            if (f != null) {
                f.FormClosed -= OnFormClosed;

                int index = OpenDialogs.IndexOf(f);
                Debug.Assert(index >= 0);
                OpenDialogs.RemoveAt(index);
            }
        }

        public void CloseAllDialogs() {
            List<Form> UnclosedForms = new List<Form>();

            // A simple for loop won't work since dialogs can do things like open and close other dialogs or refuse to close.
            while (OpenDialogs.Count > 0) {
                var dialog = PickDialogToClose();

                var dialogCount = OpenDialogs.Count;

                dialog.DialogResult = DialogResult.Cancel;
                // Manually close the dialog if necessary
                if (dialog.Visible) {
                    dialog.Close();
                }

                bool DialogClosed = OpenDialogs.Count < dialogCount;

                // If we can't close a dialog, we'll take it out of the equation for the moment
                if (!DialogClosed) {
                    OpenDialogs.Remove(dialog);
                    UnclosedForms.Add(dialog);
                }
            }

            // Any dialogs we couldn't force closed will just remain open, I guess
            OpenDialogs.AddRange(UnclosedForms);
        }

        private Form PickDialogToClose() {
            // Find the first dialog without children
            for (int i = 0; i < OpenDialogs.Count; i++) {
                if (OpenDialogs[i].OwnedForms.Length == 0)
                    return OpenDialogs[i];
            }

            // If none are found, find the first dialog with children
            return OpenDialogs[0];
        }
    }
}