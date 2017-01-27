using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snarfblasm
{
    // Todo: proper note that lables must be added in order
    class AnonymousLabelCollection
    {
        List<entry> entries = new List<entry>();
        /// <summary>
        /// The index of the next entry to be resolved. (While the assembler is making the pass that calculates labels, the anonymous labels will be resolved in order.)
        /// </summary>
        int iEntry_Resolution = 0;

        public void AddStarLabel(int insructionIndex) {
            entries.Add(new entry(entryType.Star, 0, insructionIndex));
        }
        public void AddPlusLabel(int level, int instructionIndex) {
            entries.Add(new entry(entryType.Plus, level, instructionIndex));
        }
        public void AddMinusLabel(int level, int instructionIndex) {
            entries.Add(new entry(entryType.Minus, level, instructionIndex));
        }
        public void AddLeftBraceLabel(int insructionIndex) {
            entries.Add(new entry(entryType.LeftBrace, 0, insructionIndex));
        }
        public void AddRightBraceLabel(int insructionIndex) {
            entries.Add(new entry(entryType.RightBrace, 0, insructionIndex));
        }

        /// <summary>
        /// Returns the address of the specified label (star or minus).
        /// </summary>
        /// <param name="level"></param>
        /// <param name="iSourceLine"></param>
        /// <returns>The address of the label, or -1 if the label was not found.</returns>
        public int FindLabel_Back(int level, int iSourceLine) {
            int labelIndex = indexOfLabelBeforeInstruction(iSourceLine);

            int starLevel = 0;

            while (labelIndex >= 0) {
                var currentLabel = entries[labelIndex];

                if (currentLabel.type == entryType.Star) {
                    starLevel++;
                    if (level == starLevel)
                        return currentLabel.value;
                } else if (currentLabel.type == entryType.Minus && currentLabel.level == level) {
                    return currentLabel.value;
                }

                labelIndex--;
            }

            // Oops. Ran out of labels.
            return -1;
        }
        /// <summary>
        /// Finds the address of the specified left brace.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="iSourceLine"></param>
        /// <returns>Address, or -1 if not found</returns>
        public int FindBrace_Back(int level, int iSourceLine) {
            int labelIndex = indexOfLabelBeforeInstruction(iSourceLine);

            int braceLevel = 0;

            while (labelIndex >= 0) {
                var currentLabel = entries[labelIndex];

                if (currentLabel.type == entryType.LeftBrace) {
                    braceLevel++;
                    if (level == braceLevel)
                        return currentLabel.value;
                } else if (currentLabel.type == entryType.RightBrace) {
                    braceLevel--;
                }

                labelIndex--;
            }

            // Oops. Ran out of labels.
            return -1;
        }

        /// <summary>
        /// Finds the address of the specified label (star or plus).
        /// </summary>
        /// <param name="level"></param>
        /// <param name="iSourceLine"></param>
        /// <returns>The address of the label, or -1 if the label was not found</returns>
        public int FindLabel_Forward(int level, int iSourceLine) {
            // Get first label following instruction
            int labelIndex = indexOfLabelBeforeInstruction(iSourceLine) + 1;

            int starLevel = 0;

            while (labelIndex < entries.Count) {
                entry currentLabel;
                if (labelIndex == -1) {
                    // Use a 'default' value of invalid label so that it won't match anything
                    currentLabel = new entry(entryType.Minus, Int32.MinValue, -1);
                } else {
                    currentLabel = entries[labelIndex];
                }
                if (currentLabel.type == entryType.Star) {
                    starLevel++;
                    if (level == starLevel)
                        return currentLabel.value;
                } else if (currentLabel.type == entryType.Plus && currentLabel.level == level) {
                    return currentLabel.value;
                }

                labelIndex++;
            }

            return -1;
        }
        /// <summary>
        /// Finds the address of the specified right brace.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="iSourceLine"></param>
        /// <returns>The address of the label, or -1 if the label was not found</returns>
        public int FindBrace_Forward(int level, int iSourceLine) {
            // Get first label following instruction
            int labelIndex = indexOfLabelBeforeInstruction(iSourceLine) + 1;

            int braceLevel = 0;

            while (labelIndex < entries.Count) {
                entry currentLabel;
                if (labelIndex == -1) {
                    // Use a 'default' value of invalid label so that it won't match anything
                    currentLabel = new entry(entryType.Minus, Int32.MinValue, -1);
                } else {
                    currentLabel = entries[labelIndex];
                }

                if (currentLabel.type == entryType.RightBrace) {
                    braceLevel++;
                    if (level == braceLevel)
                        return currentLabel.value;
                } else if (currentLabel.type == entryType.LeftBrace) {
                    braceLevel--;
                }

                labelIndex++;
            }

            return -1;
        }


        int indexOfLabelBeforeInstruction(int iSourceLine) {
            if (iSourceLine < entries[0].iSourceLine) return -1;

            int start = 0;
            int len = entries.Count;

            while (len > 1) {
                int middle = start + len / 2;

                if (entries[middle].iSourceLine > iSourceLine) {
                    // The label is in the first half
                    len = middle - start;
                } else {
                    // The label is in the second half
                    len -= middle - start;
                    start = middle;
                }
            }

            // We've narrowed it down to 1 label
            return start;
        }

        public void ResolveNextPointer(ushort address) {
            if (iEntry_Resolution >= entries.Count)
                throw new InvalidOperationException("No remaining entries to resolve.");

            var currentEntry = entries[iEntry_Resolution];
            entries[iEntry_Resolution] = new entry(currentEntry, address);
            iEntry_Resolution++;
        }
        
        
        // Todo: need way to resolve ADDRESS of these labels. This will probably need  to occur on the first pass. 
        struct entry
        {
            public entry(entryType type, int level, int iSourceLine) {
                this.type = type;
                this.level = level;
                this.iSourceLine = iSourceLine;
                this.value = 0;
            }
            public entry(entry original, ushort value) {
                this = original;
                this.value = value;
            }

            public readonly entryType type;
            public readonly int level;
            public readonly int iSourceLine;
            public readonly ushort value;

        }
        enum entryType:byte
        {
            Star,
            Plus,
            Minus,
            LeftBrace,
            RightBrace
        }

    }
}
