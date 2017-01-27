using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Romulus;

namespace snarfblasm
{
    class PreProcessor
    {
        Assembler assembler;

        static readonly char[] lineBreaks = { '\r', '\n' };
        string includeDirective = ".include ";

        public PreProcessor(Assembler assm) {
            this.assembler = assm;
        }

        public void Process(StringSection source, List<StringSection> output, IFileSystem files) {
            bool moreLines = !source.IsNullOrEmpty;
            while (moreLines) {
                int iLineBreak = source.IndexOfAny(lineBreaks);
                StringSection line;

                if (iLineBreak == -1) {
                    // Last line
                    line = source;
                    source = StringSection.Empty;
                    moreLines = false;
                } else {
                    line = source.Substring(0, iLineBreak);
                    bool isCRLF = (source[iLineBreak] == '\r') && (source.Length > iLineBreak + 1) && (source[iLineBreak + 1] == '\n');
                    if (isCRLF) {
                        source = source.Substring(iLineBreak + 2);
                    } else {
                        source = source.Substring(iLineBreak + 1);
                    }
                }

                if (IsIncludeLine(line)) {
                    string includeFile = line.Substring(includeDirective.Length).Trim().ToString();
                    // Need to remove optional quotes
                    if (includeFile.Length >= 2 && includeFile[0] == '\"' && includeFile[includeFile.Length - 1] == '\"') {
                        includeFile = includeFile.Substring(1, includeFile.Length - 2).Trim();
                    }
                    if (assembler.FileSystem.FileExists(includeFile)) {
                        ProcessInclude(output, files, includeFile);
                    } else {
                        assembler.AddError(new Error(ErrorCode.File_Error, string.Format(Error.Msg_FileNotFound_name, includeFile), output.Count));
                    }
                } else {
                    output.Add(line);
                }


            }
        }

        private void ProcessInclude(List<StringSection> output, IFileSystem files, string filename) {
            var fileContents = files.GetFileText(filename);

            int nextLineIndex = output.Count;
            assembler.LineManager.BeginSpan(filename, nextLineIndex);
            Process(fileContents, output, files);
            assembler.LineManager.EndSpan(output.Count - 1);

            // Empty line replaces the processed "include"
            output.Add(StringSection.Empty);
        }


        private bool IsIncludeLine(StringSection line) {
            if (line.Length < includeDirective.Length) return false;
            var lineStart = line.Substring(0, includeDirective.Length);
            
            if (StringSection.Compare(lineStart, includeDirective, true) == 0) {
                return true;
            }
            return false;
        }

    }
}
