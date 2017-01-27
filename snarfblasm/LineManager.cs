using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snarfblasm
{
    /// <summary>
    /// Keeps track of, and converts between, the zero-based pre-processed source listing, and the original source files.
    /// </summary>
    public class LineManager
    {
        public LineManager(string rootSpanName) {
            PushFile(rootSpanName);
            spans.Add(new SpanStart(0, rootSpanName, 1));
        }

        public void GetSourceLocation(int iSourceLine, out int lineIndex, out string file) {
            // Find first span that is <= iSourceLine
            int iSpan = spans.Count - 1;
            while (iSpan >= 0) {
                if (spans[iSpan].ProcessedLineNumber <= iSourceLine) {
                    // index within span
                    int relativeIndex = iSourceLine - spans[iSpan].ProcessedLineNumber;

                    lineIndex = relativeIndex + spans[iSpan].SourceLineNumber;
                    file = spans[iSpan].File;
                    return;
                }
                iSpan--;
            }

            throw new ArgumentException("The specified line number could not be traced back to a specific source file.");

            // Todo: Need to be able to return multiple source locations for macro: the line number the macro is invoked on, and the line within the macro that the error occured on (recursively for macros-in-macros)
            // i.e. Error: game.asm#clearRam#resetRegister(2) Instruction Error: Invalid Addressing Mode
            //         at  game.asm#clearRam(1)
            //         ah  game.asm(1012) 
        }


        /// <summary>
        /// Begins a subspan (include file or macro).
        /// </summary>
        /// <param name="name">The name of the include file or macro (macro should be prefixed with containing file and containing macros, i.e. game.asm#clearRam#resetRegisters)</param>
        /// <param name="lineIndex">The (processed) line index of the enclosing span where the inner span begins</param>
        public void BeginSpan(string name, int lineIndex) {
            int currentSpanSize = lineIndex - spans[spans.Count - 1].ProcessedLineNumber;
            // The containing file will pick up where it left off.
            CurrentFile.CurrentLine += currentSpanSize;

            PushFile(name);

            AddSpan(new SpanStart(lineIndex, CurrentFile.Name, 1));
        }

        public void EndSpan(int lastLineOfSpan) {
            int spanLen = lastLineOfSpan - spans[spans.Count - 1].ProcessedLineNumber + 1;
            PopFile();
            AddSpan(new SpanStart(lastLineOfSpan + 1, CurrentFile.Name, CurrentFile.CurrentLine));
        }

        #region FileStack

        List<FileInfo> fileStack = new List<FileInfo>();

        private FileInfo CurrentFile { get { return fileStack[fileStack.Count - 1]; } }
        private void PopFile() {
            fileStack.RemoveAt(fileStack.Count - 1);
        }
        private void PushFile(string name) {
            fileStack.Add(new FileInfo(name,false));
        }

        /// <summary>Keeps track of filename and current line number within file (where we left off for an include)</summary>
        class FileInfo
        {
            public FileInfo(string name, bool isMacro) {
                this.Name = name;
                this.CurrentLine = 1;
                this.IsMacro = IsMacro;
            }

            public bool IsMacro { get; private set; }
            public string Name { get; private set; }
            /// <summary>The next source line is stored here when an include or macro is expanded, so we can pick up where we left off when the
            /// subspan is closed (end of include or macro).</summary>
            public int CurrentLine { get; set; }

        }

        #endregion

        #region SpanList
        List<SpanStart> spans = new List<SpanStart>();

        void AddSpan(SpanStart span) {
            spans.Add(span);
        }
        struct SpanStart
        {
            public SpanStart(int iProcessedLine, string file, int iSourceLine) {
                this.File = file;
                this.ProcessedLineNumber = iProcessedLine;
                this.SourceLineNumber = iSourceLine;
            }
            public readonly string File;
            public readonly int ProcessedLineNumber;
            public readonly int SourceLineNumber;
        }
        #endregion
        ////CodeSpan root;

        //// List<CodeSpan> spanStack = new List<CodeSpan>();

        ////public LineManager(string rootSpanName) {
        ////    root = new CodeSpan(rootSpanName, 0);
        ////    spanStack.Add(root);
        ////}

        ////public void GetSourceLocation(int iSourceLine, out int lineIndex, out string file) {
        ////    // Find inner-most (named) span that contains iSourceLine
        ////    CodeSpan container = root;

        ////    bool doLoop = true;
        ////    do {
        ////        if (container.HasSubspans) {
        ////            int spanIndex = GetSpanIndex(iSourceLine, container);
        ////            var subspan = container.SubSpans[spanIndex];

        ////            if (subspan.Name == null) {
        ////                // 'container' is the file that contains the code
        ////                doLoop = false;
        ////            } else {
        ////                // 'containingSpan' is the file that contains the code
        ////                container = subspan;
        ////            }
        ////        } else {
        ////            doLoop = false;
        ////        }
        ////    } while (doLoop);

        ////    // If it has subspans, walk list and subract out insertions
        ////    if (container.HasSubspans) {
        ////        int spanIndex = GetSpanIndex(iSourceLine, container);

        ////        // scan through preceeding spans, subtract out expansions
        ////        for (int iSpan = 0; iSpan < spanIndex - 1; iSpan++) {
        ////            var span = container.SubSpans[iSpan];
        ////            bool spanIsExpansion = span.Name == null;
        ////            if (spanIsExpansion) {
        ////                int spanSize = container.SubSpans[iSpan + 1].start - span.start;
        ////                iSourceLine -= spanSize;
        ////            }
        ////        }
        ////    }

        ////    // Return (adjusted iSourceLine) - spanStart + 1
        ////    lineIndex = iSourceLine - container.start + 1;
        ////    file = container.Name;

        ////}

        ////private int GetSpanIndex(int iSourceLine, CodeSpan container) {
        ////    // We need to find the first subspan that comes after the sourceline.
        ////    // The containing span is the one that preceeds.
        ////    // If there is no span after the source line, we consider the
        ////    // last span to be the containing span.
        ////    // If the source line preceeds the first subspan, we return -1.
        ////    int iSubSpan = 0;
        ////    while (iSubSpan < container.SubSpans.Count && container.SubSpans[iSubSpan].start <= iSourceLine) {
        ////        iSubSpan++;
        ////    }

        ////    return iSubSpan - 1;
        ////}


        ////public void BeginSpan(string name, int lineIndex) {
        ////    // There should be a 'default span' to identify the first stretch of code
        ////    // as belonging to the container (unless a subspan starts right at the beginning). If
        ////    // It isn't already presest, create it.
        ////    if (CurrentSpan.SubSpans.Count == 0 && lineIndex != CurrentSpan.start) {
        ////        CurrentSpan.SubSpans.Add(new CodeSpan(null, CurrentSpan.start));
        ////    } 
            
        ////    CodeSpan newSpan = new CodeSpan(name, lineIndex);

        ////    CurrentSpan.SubSpans.Add(newSpan);
        ////    PushSpan(newSpan);
        ////}

        ////public void EndSpan() {
        ////    PopSpan();
        ////}

        ////CodeSpan CurrentSpan { get { return spanStack[spanStack.Count - 1]; } }
        ////void PopSpan() { spanStack.RemoveAt(spanStack.Count - 1); }
        ////void PushSpan(CodeSpan span) {
        ////    spanStack.Add(span);
        ////}


        ////class CodeSpan
        ////{
        ////    public CodeSpan(string name, int start) {
        ////        this.Name = name;
        ////        this.start = start;
        ////        this.SubSpans = subspans;
        ////    }

        ////    List<CodeSpan> subspans = new List<CodeSpan>();
        ////    /// <summary>
        ////    /// The name of the file, or null if this span simply denotes a section of a containing span.
        ////    /// </summary>
        ////    public string Name { get; private set; }
        ////    public int start { get; private set; }
        ////    public IList<CodeSpan> SubSpans { get; private set; }

        ////    public bool HasSubspans { get { return subspans.Count > 0; } }
        ////}
    }
}
