using System;
using System.Collections.Generic;
using System.Text;
using Editroid.Asm;
using Editroid.ROM;
using System.ComponentModel;

namespace Editroid.Patches
{
    /// <summary>
    /// A patch to apply Super Metroid style doors to Metroid. Will not work on expanded ROMs.
    /// </summary>
    class SmetDoorPatch: RomPatch
    {
        int expandoAdjust = 0x20000;
        public SmetDoorPatch(bool Expando){
            if (!Expando) expandoAdjust = 0;
            ComboSample = StructureSample = true;

            asmPatch = new PatchSegment((pRom)(expandoAdjust + 0x1e817), asmPatchData);
            standardDoorComboPatch = new PatchSegment((pRom)0x6F80, standardDoorComboPatchData);
            hDoorComboPatch = new PatchSegment((pRom)0x6FC0, hDoorComboPatchData);
            standardDoorStructPatch = new PatchSegment((pRom)0x6Cd9, standardDoorStructPatchData);
            hDoorStructPatch = new PatchSegment((pRom)0x6D2A, hDoorStructPatchData);
            outerDoorTilePatch = new PatchSegment((pRom)(expandoAdjust + 0x1A100), outerDoorTilePatchData);
            outerHDoorTilePatch = new PatchSegment((pRom)(expandoAdjust + 0x1A1A0), outerDoorTilePatchData);
            innerDoorTilePatch = new PatchSegment((pRom)(expandoAdjust + 0x1A600), innerDoorTilePatchData);
            structPointerTableCorrectionPatch = new PatchSegment((pRom)0x639C, structPointerTableCorrectionData);
        }
        PatchSegment asmPatch; // = new PatchSegment((HRomOffset)(expandoAdjust + 0x1e817), asmPatchData);
        PatchSegment standardDoorComboPatch; // = new PatchSegment((HRomOffset)0x6F80, standardDoorComboPatchData);
        PatchSegment hDoorComboPatch; // = new PatchSegment((HRomOffset)0x6FC0, hDoorComboPatchData);
        PatchSegment standardDoorStructPatch; // = new PatchSegment((HRomOffset)0x6Cd9, standardDoorStructPatchData);
        PatchSegment hDoorStructPatch; // = new PatchSegment((HRomOffset)0x6D2A, hDoorStructPatchData);
        PatchSegment outerDoorTilePatch; // = new PatchSegment((HRomOffset)(expandoAdjust + 0x1A100), outerDoorTilePatchData);
        PatchSegment outerHDoorTilePatch; // = new PatchSegment((HRomOffset)(expandoAdjust + 0x1A1A0), outerDoorTilePatchData);
        PatchSegment innerDoorTilePatch; // = new PatchSegment((HRomOffset)(expandoAdjust  + 0x1A600), innerDoorTilePatchData);
        PatchSegment structPointerTableCorrectionPatch; // = new PatchSegment((HRomOffset)0x639C, structPointerTableCorrectionData);

        [DefaultValue(true)]
        [Description("Set to true to apply sample combos to Brinstar.")]
        public bool ComboSample { get; set; }
        [DefaultValue(true)]
        [Description("Set to true to apply sample structures to Brinstar.")]
        public bool StructureSample { get; set; }

        protected override void BeforePatchApplied() {
            Segments.Clear();

            Segments.Add(asmPatch);
            Segments.Add(outerDoorTilePatch);
            Segments.Add(outerHDoorTilePatch);
            Segments.Add(innerDoorTilePatch);

            if (ComboSample) {
                Segments.Add(standardDoorComboPatch);
                Segments.Add(hDoorComboPatch);
            }
            if (StructureSample) {
                Segments.Add(standardDoorStructPatch);
                Segments.Add(hDoorStructPatch);
                Segments.Add(structPointerTableCorrectionPatch);
            }
        }

        static byte[] asmPatchData = {
                                     (byte)Opcodes.cmp_im, 0xAA, 
                                     (byte)Opcodes.bcc, 0x06,
                                     (byte)Opcodes.cmp_im, 0xB4,
                                     (byte)Opcodes.bcs, 0x04
                                 };
        static byte[] standardDoorComboPatchData = {   
                                                    0xA0, 0xF0, 0xA1, 0xF1, // l door top
                                                    0xA2, 0xF2, 0xA3, 0xF3, // l door middle
                                                    0xA4, 0xF4, 0xA0, 0xF0, // l door bottom
                                                    0xF5, 0xA5, 0xF6, 0xA6, // r door top
                                                    0xF7, 0xA7, 0xF8, 0xA8, // r door middle
                                                    0xF9, 0xA9, 0xF5, 0xA5, // r door bottom
                                               };

        static byte[] hDoorComboPatchData = {
                                                0xAA, 0xF0, 0xAB, 0xF1, // l h-door top
                                                0xAC, 0xF2, 0xAD, 0xF3, // l h-door middle
                                                0xAE, 0xF4, 0xAA, 0xF0, // l h-door bottom
                                                0xF5, 0xAF, 0xF6, 0xB0, // r h-door top
                                                0xF7, 0xB1, 0xF8, 0xB2, // r h-door middle
                                                0xF9, 0xB3, 0xF5, 0xAF, // r h-door bottom
                                            };
        static byte[] standardDoorStructPatchData = {
                                        0x01, 0x20, 0x01, 0x21, 0x01, 0x22, 0xFF, // l door
                                        0x01, 0x23, 0x01, 0x24, 0x01, 0x25, 0xFF, // r door
                                                    };
        static byte[] hDoorStructPatchData = {
                                        0x01, 0x30, 0x01, 0x31, 0x01, 0x32, 0xFF, // l h-door
                                        0x01, 0x33, 0x01, 0x34, 0x01, 0x35, 0xFF, // r h-door
                                             };

        static byte[] outerDoorTilePatchData = {
                            0xFF, 0xFF, 0xA5, 0xEF, 0xFF, 0xA4, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0xFF, 0xFF, 0xEF, 0xFF, // Ldoor
                            0xB6, 0xA7, 0xB4, 0xA4, 0xB4, 0xA4, 0xB4, 0xA4, 0xFD, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 
                            0xB7, 0xA6, 0xA7, 0xEF, 0xA4, 0xA4, 0xB4, 0xFF, 0xFF, 0xFD, 0xFF, 0xB4, 0xFF, 0xFF, 0xFF, 0xA4, 
                            0xFF, 0xB4, 0xA4, 0xA4, 0xEF, 0xA7, 0xA6, 0xB7, 0xA4, 0xFF, 0xFF, 0xFF, 0xB4, 0xFF, 0xFD, 0xFF, 
                            0xA4, 0xB4, 0xA4, 0xB4, 0xA4, 0xB4, 0xA7, 0xB6, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFD, 
                            0xFF, 0xFF, 0xA5, 0xF7, 0xFF, 0x25, 0xF7, 0xFF, 0xFF, 0xFF, 0xFF, 0xF7, 0xFF, 0xFF, 0xF7, 0xFF, // r door
                            0x6D, 0xE5, 0x2D, 0x25, 0x2D, 0x25, 0x2D, 0x25, 0xBF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 
                            0xED, 0x65, 0xE5, 0xF7, 0x25, 0x25, 0x2D, 0xFF, 0xFF, 0xBF, 0xFF, 0x2D, 0xFF, 0xFF, 0xFF, 0x25, 
                            0xFF, 0x2D, 0x25, 0x25, 0xF7, 0xE5, 0x65, 0xED, 0x25, 0xFF, 0xFF, 0xFF, 0x2D, 0xFF, 0xBF, 0xFF, 
                            0x25, 0x2D, 0x25, 0x2D, 0x25, 0x2D, 0xE5, 0x6D, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xBF,
                                              };
        
        static byte[] innerDoorTilePatchData = {
                            0xFF, 0xFF, 0xD8, 0xFF, 0xFF, 0xC0, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // l door
                            0x98, 0xF8, 0xC0, 0xC0, 0xC0, 0xC0, 0xC0, 0xC0, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 
                            0xF8, 0x98, 0xF8, 0xFF, 0xC0, 0xC0, 0xC0, 0xFF, 0xFF, 0x7F, 0xFF, 0xC0, 0xFF, 0xFF, 0xFF, 0xC0, 
                            0xFF, 0xC0, 0xC0, 0xC0, 0xFF, 0xF8, 0x98, 0xF8, 0xC0, 0xFF, 0xFF, 0xFF, 0xC0, 0xFF, 0x7F, 0xFF, 
                            0xC0, 0xC0, 0xC0, 0xC0, 0xC0, 0xC0, 0xF8, 0x98, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 
                            0xFF, 0xFF, 0x1B, 0xFF, 0xFF, 0x03, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // h door
                            0x19, 0x1F, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 
                            0x1F, 0x19, 0x1F, 0xFF, 0x03, 0x03, 0x03, 0xFF, 0xFF, 0xFE, 0xFF, 0x03, 0xFF, 0xFF, 0xFF, 0x03, 
                            0xFF, 0x03, 0x03, 0x03, 0xFF, 0x1F, 0x19, 0x1F, 0x03, 0xFF, 0xFF, 0xFF, 0x03, 0xFF, 0xFE, 0xFF, 
                            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x1F, 0x19, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE,
                                               };
        static byte[] structPointerTableCorrectionData = { 0x21 };
        public override string Description {
            get {
                return description;
            }
        }
        static string description = "Applies an assembly patch and pattern patch to produce Super-Metroid styled doors. " +
            "It can also apply sample combos and structures to Brinstar data so the doors are ready to use. Note that the patterns, " +
            "combos, and structures will overwrite existing data that is already in use.";
    }
}
