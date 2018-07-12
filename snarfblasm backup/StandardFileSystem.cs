using System;
using System.IO;
using System.Windows.Forms;
using snarfblasm;

namespace snarfblasm
{
    /// <summary>
    /// Exposes the operating system's file system as an IFileSystem.
    /// </summary>
    class StandardFileSystem : IFileSystem
    {
        public const string Psuedo_Form = "%form%";
        public const string Pseudo_Clip = "%clip%";

        public static readonly StandardFileSystem FileSystem = new StandardFileSystem();

        private StandardFileSystem() { }

        #region IFileSystem Members

        public string GetFileText(string filename) {
            //if (filename.Equals(Psuedo_Form, StringComparison.InvariantCultureIgnoreCase)) {
            //    return snarfblasm.TextForm.GetText();
            // } else 
            if (filename.Equals(Pseudo_Clip, StringComparison.InvariantCultureIgnoreCase)) {
                return Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;
            } else {
                return System.IO.File.ReadAllText(filename);
            }
        }


        public void WriteFile(string filename, byte[] data) {
            //if (filename.Equals(Psuedo_Form, StringComparison.InvariantCultureIgnoreCase)) {
            //    TextForm.GetText(Romulus.Hex.FormatHex(data));
            //} else 
            if (filename.Equals(Pseudo_Clip, StringComparison.InvariantCultureIgnoreCase)) {
                if (data.Length == 0)
                    Clipboard.SetText(" ");
                else
                    Clipboard.SetText(Romulus.Hex.FormatHex(data));
            } else {
                File.WriteAllBytes(filename, data);
            }
        }

        #endregion

        public static bool IsPseudoFile(string file) {
            if (file == null) return false;
            if (file.Length < 3) return false;
            if (file[0] == '%' && file[file.Length - 1] == '%') return true;
            return false;
        }

        #region IFileSystem Members


        public long GetFileSize(string filename) {
            if (filename.Equals(Psuedo_Form, StringComparison.InvariantCultureIgnoreCase)) return 0;

            //   System.Security.SecurityException:
            //     The caller does not have the required permission.
            //
            //   System.ArgumentException:
            //     The file name is empty, contains only white spaces, or contains invalid characters.
            //
            //   System.UnauthorizedAccessException:
            //     Access to fileName is denied.
            //
            //   System.IO.PathTooLongException:
            //     The specified path, file name, or both exceed the system-defined maximum
            //     length. For example, on Windows-based platforms, paths must be less than
            //     248 characters, and file names must be less than 260 characters.
            //
            //   System.NotSupportedException:
            //     fileName contains a colon (:) in the middle of the string.
            try {
                return new FileInfo(filename).Length;
            } catch (System.Security.SecurityException) {
                return -1;
            } catch (System.IO.IOException) {
                return -1;
            } catch (ArgumentException) {
                return -1;
            } catch (UnauthorizedAccessException) {
                return -1;
            }
        }

        public Stream GetFileReadStream(string filename) {
            return new FileStream(filename, FileMode.Open);
        }

        public bool FileExists(string name) {
            if (name.Equals(Psuedo_Form, StringComparison.InvariantCultureIgnoreCase)) return true;

            return File.Exists(name);
        }

        #endregion

    }
}
