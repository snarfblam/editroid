using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Editroid.ROM.Projects;
using System.Text.RegularExpressions;

namespace Editroid.ROM.MMC3
{
    class MMC3Expander
    {
        const int PrgBankSize = 0x2000;
        const int PrgBankCount = 0x40;
        const int ChrBankSize = 0x400;
        const int ChrBankCount = 0x100;

        MetroidRom originalRom;
        Project project;
        byte[] oldRomImage;
        byte[] newRomImage = new byte[0x10 + 0x80000 + 0x40000];

        bool expanded = false;

        public MMC3Expander(MetroidRom rom, Project p) {
            this.originalRom = rom;
            this.project = p;

            oldRomImage = new byte[rom.data.Length];
            //newRomImage = new byte[oldRomImage.Length];
            Array.Copy(rom.data, oldRomImage, rom.data.Length);
            Array.Copy(oldRomImage, newRomImage, rom.data.Length);
            //ExpandRom();


        }

        public bool UpdateExpansionFile { get; set; }
        public bool UpdateFixedBankReferences { get; set; }

        private void ApplyAsmPatches() {
            byte[] targetRom = newRomImage;
            ApplyExpansionPatch(targetRom);
        }

        public  static void ApplyExpansionPatch(byte[] targetRom) {
            // Apply expansion code prior to expansion
            // (for ease... atm expansion code targets the fixed bank at $F instead of $1F)

            // manually include defines
            string asm = Projects.ProjectResources.DefaultDefinesFile + Environment.NewLine + Mmc3Resources.ExpansionASM;
            asm = NopOutScreenLoad(asm);
            snarfblasm.Assembler assm = new snarfblasm.Assembler("mmc3Expander", asm, null);
            var output = assm.Assemble();
            var errors = assm.GetErrors();
            if (errors != null && errors.Count > 0) {
                // todo: more specific exception
                string error = "Assembler error: " + errors[0].File + " line " + errors[0].LineNumber + ": " + errors[0].Message;
                if (errors.Count > 1) error += " (additional errors: " + (errors.Count - 1).ToString() + ")";
                throw new Exception(error);
            }

            var patches = assm.GetPatchSegments();

            for (int i = 0; i < patches.Count; i++) {
                var patch = patches[i];
                int patchLength = patch.Length;
                if (patchLength < 0) {
                    patchLength = output.Length - patch.Start;
                }
                Array.Copy(output, patch.Start, targetRom, patch.PatchOffset, patchLength);
            }
        }

        private static string NopOutScreenLoad(string asm) {
            string NopNopNop = Environment.NewLine + "    NOP" + Environment.NewLine + "    NOP" + Environment.NewLine + "    NOP" + Environment.NewLine;
            return Regex.Replace(asm, "<<<<.*>>>>", NopNopNop, RegexOptions.Singleline);
        }

        /// <summary>
        /// Expands the ROM
        /// </summary>
        /// <param name="remainingWork">A delegate that, if non-null, must be called upon a MetroidRom object created from the returned buffer to complete the expansion.</param>
        /// <returns></returns>
        public byte[] ExpandRom(out PostWorkDelegate remainingWork) {
            if (expanded) throw new InvalidOperationException("ROM already expanded");
            expanded = true;


            // Update PRG bank count
            newRomImage[0x4] = 0x20;
            MoveBanks();

            // ASM will be applied via build process
            ApplyAsmPatches();

            // Convert screen pointers to 3-byte pointer format (Bank:Address)
            UpdateScreenPointers();

            // Project files will be updated automatically
            UpdateProject();

            //bool remainingWorkComplete = false;
            remainingWork = (MetroidRom r, Project p) => {
                //if (remainingWorkComplete) throw new InvalidOperationException("ROM expansion finalization already complete.");
                //remainingWorkComplete = true;

                ConvertChrAnimation(r, p);
                r.ReloadChrAnimationTable();

                WriteDefaultTilePhysics(r, LevelIndex.Brinstar);
                WriteDefaultTilePhysics(r, LevelIndex.Norfair);
                WriteDefaultTilePhysics(r, LevelIndex.Tourian);
                WriteDefaultTilePhysics(r, LevelIndex.Kraid);
                WriteDefaultTilePhysics(r, LevelIndex.Ridley);

            };
            
            return newRomImage;
        }

        private void WriteDefaultTilePhysics(MetroidRom r, LevelIndex level) {
            var tilePhysicsOffset = (int)r.Levels[level].TilePhysicsTableLocation;
            for (int i = 0; i < 256; i++) {
                r.data[tilePhysicsOffset + i] = (byte)i;
            }
        }

        public delegate void PostWorkDelegate(MetroidRom rom, Project project);

        const string FixedBankMatchPattern = @"(^\s*\.PATCH\s+)(0)(F:)";
        private void UpdateProject() {
            if (project == null) return;

            var p = project;
            if (UpdateExpansionFile) {
                ReplaceExpansionFile(p);
            }
            if (UpdateFixedBankReferences) {
                UpdateFixedBankRefs(p);
            }

        }

        /// <summary>
        /// Updates any .PATCH directives that reference bank 0F to instead point to bank 1F in the general code file and any user code files.
        /// </summary>
        /// <param name="p"></param>
        public static void UpdateFixedBankRefs(Project p) {
            Regex r = new Regex(FixedBankMatchPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            p.GeneralCodeFile = r.Replace(p.GeneralCodeFile, "${1}1${3}");

            foreach (var file in p.Files) {
                file.Code = r.Replace(file.Code, "${1}1${3}");
            }
        }

        /// <summary>
        /// Converts a CHR Usage table (enhanco) to a CHR Animation table (MMC3). The ROM must have a valid CHR Usage table with a total of less than 256 frames for all levels together.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="p"></param>
        public static void ConvertChrAnimation(MetroidRom r, Project p) {
            int animationIndex = 0;

            CreateChrAnimationTablesForLevel(r, p, LevelIndex.None, ref animationIndex);
            CreateChrAnimationTablesForLevel(r, p, LevelIndex.Brinstar, ref animationIndex);
            CreateChrAnimationTablesForLevel(r, p, LevelIndex.Norfair, ref animationIndex);
            CreateChrAnimationTablesForLevel(r, p, LevelIndex.Tourian, ref animationIndex);
            CreateChrAnimationTablesForLevel(r, p, LevelIndex.Kraid, ref animationIndex);
            CreateChrAnimationTablesForLevel(r, p, LevelIndex.Ridley, ref animationIndex);

            // If the ROM has this flag set, it needs to be cleared so the CHR animation table is properly serialized
            r.ChrAnimationTableMissing = false;
        }

        private static void CreateChrAnimationTablesForLevel(MetroidRom r, Project p, LevelIndex l, ref int animationIndex) {
            // Get old data
            var oldSpriteBank = r.ChrUsage.GetSprPage(l);
            var oldBgStart = r.ChrUsage.GetBgFirstPage(l);
            var oldBgEnd = r.ChrUsage.GetBgLastPage(l);
            var oldFrameTime = r.ChrUsage.GetAnimRate(l);
            int bgFrameLength = oldBgEnd - oldBgStart + 1;

            // Calculate and apply new SPR data
            var newSpriteBank0 = oldSpriteBank * 4;
            var newSpriteBank1 = newSpriteBank0 + 2;
            r.ChrUsage.MMC3_SetSpr0(l, (byte)newSpriteBank0);
            r.ChrUsage.MMC3_SetSpr1(l, (byte)newSpriteBank1);
            // Flag to indicate CHR animation table instead of old style CHR usage table
            r.ChrUsage.MMC3_SetUnusedByte(l, 0xFF);

            // Calculate and apply new BG data
            var newBgStart = oldBgStart * 4;
            r.ChrUsage.MMC3_SetBgLoopStart(l, (byte)animationIndex);
            AddAnimationFrames(r, ref animationIndex, newBgStart, bgFrameLength, bgFrameLength);
        }

        private static void AddAnimationFrames(MetroidRom rom, ref int frameIndex, int firstChrBank, int frameCount, int frameLength) {
            var chrBank = firstChrBank;

             while (frameCount > 0) {
                rom.SetAnimationTable_Bank0(frameIndex, (byte)chrBank);
                rom.SetAnimationTable_Bank1(frameIndex, (byte)(chrBank + 1));
                rom.SetAnimationTable_Bank2(frameIndex, (byte)(chrBank + 2));
                rom.SetAnimationTable_Bank3(frameIndex, (byte)(chrBank + 3));
                rom.SetAnimationTable_FrameTime(frameIndex, frameLength);
                // Set the 'last frame' flag on the last frame
                rom.SetAnimationTable_FrameLast(frameIndex, frameCount == 1);

                frameCount--;
                frameIndex++;
                chrBank += 4;
            }
        }

        /// <summary>
        /// Updates the specified project's expansion file to the newest version
        /// </summary>
        /// <param name="p"></param>
        public static void ReplaceExpansionFile(Project p) {
            //p.ExpansionFile = Mmc3Resources.Project_ExpansionASM;
            p.ExpansionFile = Mmc3Resources.ExpansionASM;

        }

        private void MoveBanks() {
            const int oFixedBank = 0xF * 0x4000 + 0x10;
            const int oFixedBank_New = 0x1F * 0x4000 + 0x10;

            const int oChr = 0x10 * 0x4000 + 0x10;
            const int oChr_New = 0x20 * 0x4000 + 0x10;


            // Copy MMC1 CHR to first half of MMC3 CHR
            CopyBlock(oChr, oChr_New, 0x1000 * 0x20);
            // All swapppable banks + iNES header
            CopyBlock(0, 0, oFixedBank);
            // Fixed PRG bank 
            CopyBlock(oFixedBank, oFixedBank_New, 0x4000);
        }

        void CopyBlock(int oldOffset, int newOffset, int size) {
            Array.Copy(newRomImage, oldOffset, newRomImage, newOffset, size);
            //Array.Copy(oldRomImage, oldOffset, newRomImage, newOffset, size);
        }


        private void UpdateScreenPointers() {

            for (int level = 0; level < 5; level++) {
                var screens = originalRom.Levels[(LevelIndex)level].Screens;
                var screenBank = screens.DataBank;
                int mmc3BankNumber = screenBank.Index * 2;
                var pointerBank = screens.PointerBank;
                var pointerOffset = screens.Pointers.Offset;

                int offset = pointerOffset;

                List<pCpu> pointers = new List<pCpu>();
                for (int iScreen = 0; iScreen < screens.Count; iScreen++) {
                    pointers.Add(screens.Pointers[iScreen]);
                }
                for (int iScreen = 0; iScreen < screens.Count; iScreen++) {
                    // Write bank, pointer
                    var ptr = pointers[iScreen];

                    var newPtr = ptr;
                    newPtr.Value = ((newPtr.Value & 0x1FFF) | (0xA000));

                    if (ptr.Value < 0xA000) {
                        newRomImage[offset] = (byte)mmc3BankNumber;
                        offset++;
                    } else {
                        // in upper half of 16 k bank (next MMC3 bank)
                        newRomImage[offset] = (byte)(mmc3BankNumber + 1);
                        offset++;
                    }
                    newPtr.Write(newRomImage, offset);
                    offset += 2;

                }
                // Write 00:0000 terminator
                newRomImage[offset] = 0;
                newRomImage[offset + 1] = 0;
                newRomImage[offset + 2] = 0;
                offset += 3;
            }
        }

    }
}
