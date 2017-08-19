using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;
using System.Drawing;
using Editroid.Asm;

namespace Editroid
{
    public class TestRomPatcher
    {
        public TestRomPatcher() {
            Narpassword = true;
        }

        private MetroidRom rom;
        public MetroidRom Rom {
            get { return rom; }
            set {
                rom = value;
                if (rom != null) {
                    _FixedBankOffset = rom.FixedBank.Offset;
                    _FixedBankAdjust = _FixedBankOffset - 0x1C010;
                }
            }
        }

        int _FixedBankOffset = 0x20010;
        int _FixedBankAdjust = 0x0;

        #region configuration
        private bool verticalScrolling = false;
        public bool VerticalScrolling { get { return verticalScrolling; } set { verticalScrolling = value; } }

        private LevelIndex startLevel = LevelIndex.Brinstar;
        public LevelIndex StartLevel { get { return startLevel; } set { startLevel = value; } }

        private Point startMapLocation;
        public Point StartMapLocation { get { return startMapLocation; } set { startMapLocation = value; } }

        private int initialHealth = 30;
        public int InitialHealth { get { return initialHealth; } set { initialHealth = value; } }

        private bool skipTitle = true;
        public bool SkipTitle { get { return skipTitle; } set { skipTitle = value; } }

        private bool skipStartScreen = true;
        public bool SkipStartScreen { get { return skipStartScreen; } set { skipStartScreen = value; } }

        private Equipment equipment = Equipment.MaruMari | Equipment.Bombs | Equipment.LongBeam | Equipment.HighJump | Equipment.IceBeam | Equipment.ScrewAttack | Equipment.Varia;
        public Equipment Equipment { get { return equipment; } set { equipment = value; } }

        public bool Narpassword { get; set; }

        private int tankCount = 5;
        public int TankCount { get { return tankCount; } set { tankCount = value; } }

        private int fullTankCount = 4;
        public int FullTankCount { get { return fullTankCount; } set { fullTankCount = value; } }

        private int missileCount = 90;
        public int MissileCount { get { return missileCount; } set { missileCount = value; } }

        private int missileCapacity = 100;
        public int MissileCapacity { get { return missileCapacity; } set { missileCapacity = value; } }

        private Point initialScreenPosition = new Point(80, 80);
        public Point InitialScreenPosition { get { return initialScreenPosition; } set { initialScreenPosition = value; } }

        private bool skipIntro = true;
        public bool SkipIntro { get { return skipIntro; } set { skipIntro = value; } }

        private bool resetOnDie = true;
        public bool ResetOnDie { get { return resetOnDie; } set { resetOnDie = value; } }

        #endregion

        public byte[] CreatePatchedRom() {
            byte[] result = new byte[rom.data.Length];
            Array.Copy(rom.data, result, result.Length);

            ////bool expando = rom.ExpandedRom;
            ////int expandoAdjust = expando ? 0x20000 : 0;
            if (verticalScrolling)
                Patch.VerticalScrolling.Apply(result, _FixedBankAdjust);
            if (skipTitle)
                Patch.SkipTitle.Apply(result, _FixedBankAdjust);

            if (skipStartScreen) {
                Patch.SkipStartPass.Apply(result, _FixedBankAdjust);
                Patch.BlankPasswordScreen.Apply(result, _FixedBankAdjust);
            }
            if (skipIntro)
                Patch.SkipStartMusic.Apply(result, _FixedBankAdjust);

            result[Rom.Levels[startLevel].LevelStartOffset] = (byte)startMapLocation.X;
            result[Rom.Levels[startLevel].LevelStartOffset + 1] = (byte)startMapLocation.Y;

            Patch.ScreenY.Apply(result, new byte[] { (byte)(initialScreenPosition.Y & 0xFF) }, _FixedBankAdjust);
            Patch.ScreenX.Apply(result, new byte[] { (byte)(initialScreenPosition.X & 0xFF) }, _FixedBankAdjust);

            if (rom.RomFormat == RomFormats.Expando) {
                ExpandoAreaInitializers.SetStartArea(result, startLevel);
            }else if(rom.RomFormat == RomFormats.Enhanco){
                EnhancoAreaInitializers.SetStartArea(result, startLevel);
            } else if(rom.RomFormat == RomFormats.Standard) {
                StandardAreaInitializers.SetStartArea(result, startLevel);
            } else if (rom.RomFormat == RomFormats.MMC3) {
                MMC3AreaInitializers.SetStartArea(result, startLevel);
            }


            PatchInitAssembly(result);

            byte healthLsB = (byte)((initialHealth % 10) << 4);
            byte healthMsB = (byte)(
                ((initialHealth / 10))
                    |
                (fullTankCount & 0x0F) << 4);
            result[0x1C931 + _FixedBankAdjust] = healthLsB;
            result[0x1C936 + _FixedBankAdjust] = healthMsB;

            if (resetOnDie)
                Patch.ResetOnDie.Apply(result, _FixedBankAdjust);

            return result;
        }

        private void PatchInitAssembly(byte[] result) {
            AssmWriter assembler = new AssmWriter(result);

            //// Todo: Hack: This gets rid of a branch that is not needed in MMC3 in order to allow test room to work, but this is very brittle.
            //// Instead, MMC3 should have a separate test-room boot routine where the TestRoom feature can either update a table that is used to init the game or can at
            //// least simply modify a standard, stable initialization routine dedicated to TestRoom
            //assembler.Offset = 0x10E7;
            //assembler.WriteInstruction(Opcodes.ldy_im, (byte)0x00);
            //assembler.WriteInstruction(Opcodes.sty, 0x010e);
            //assembler.WriteInstruction(Opcodes.dey);
            //assembler.WriteInstruction(Opcodes.sty, 0x780D);
            ////assembler.WriteInstruction(Opcodes.nop);
            
            assembler.Offset = 0x10F4;

            // Change a JMP to a JSR
            assembler.WriteInstruction(Opcodes.jsr, 0x932B);
            // assembler.SkipBytes(2);
            

            // LDA #equipment
            assembler.WriteInstruction(Opcodes.lda_im, (byte)equipment);
            // STA $6878
            assembler.WriteInstruction(Opcodes.sta, 0x6878);

            // LDA #TankCount
            assembler.WriteInstruction(Opcodes.lda_im, (byte)(tankCount & 0xFF));
            // STA $6877
            assembler.WriteInstruction(Opcodes.sta, 0x6877);

            // LDA #missileCount
            assembler.WriteInstruction(Opcodes.lda_im, (byte)(missileCount & 0xFF));
            // STA $6879
            assembler.WriteInstruction(Opcodes.sta, 0x6879);

            // LDA #missileCapacity
            assembler.WriteInstruction(Opcodes.lda_im, (byte)(missileCapacity & 0xFF));
            // STA $687A
            assembler.WriteInstruction(Opcodes.sta, 0x687A);

            if (Narpassword) {
                assembler.WriteInstruction(Opcodes.lda_im, (byte)0x01);
                assembler.WriteInstruction(Opcodes.sta, 0x69B2);
            }

            // RTS
            assembler.WriteInstruction(Opcodes.rts);
        }

        class AssmWriter
        {
            byte[] data;
            private int offset;

            public int Offset {
                get { return offset; }
                set { offset = value; }
            }

            public AssmWriter(byte[] data) {
                this.data = data;
            }

            public void WriteInstruction(Asm.Opcodes opcode) {
                WriteByte((byte)opcode);
            }
            public void WriteInstruction(Asm.Opcodes opcode, byte operand) {
                WriteByte((byte)opcode);
                WriteByte(operand);
            }
            public void WriteInstruction(Asm.Opcodes opcode, byte operand, byte operand2) {
                WriteByte((byte)opcode);
                WriteByte(operand);
                WriteByte(operand2);
            }
            public void WriteInstruction(Asm.Opcodes opcode, int ptr) {
                WriteByte((byte)opcode);
                WritePtr(ptr);
            }
            public void WriteByte(byte b) {
                data[offset] = b;
                offset++;
            }
            public void WritePtr(int ptr) {
                WriteByte((byte)(ptr & 0xFF));
                WriteByte((byte)((ptr >> 8) & 0xFF));
            }
            public void WritePtr(byte msB, byte lsB) {
                WriteByte(lsB);
                WriteByte(msB);
            }
            public void SkipByte() { SkipBytes(1); }
            public void SkipBytes(int count) { offset += count; }
        }

        static class StandardAreaInitializers
        {
            static int StartLevelOffset = 0x1365;
            // This is the offset of the JMP operand that calls each level's init routine
            static int BankInitBranchTableOffset = 0x1C531;
            static int[] BankInitBranchTableValues = { 0xc552, 0xc583, 0xc590, 0xc5b6, 0xc5c3 };

            // These are the offsets within each level's init routines for the JMP operand that calls the level's gfx init routine
            static int[] GraphicInitJumpInstructionOffsets = { 0x1C582, 0x1C59a, 0x1C5b2, 0x1C5cd, 0x1C5da, };
            // These are the original operands to the JMP instructions
            static int[] GraphicInitJumpInstructionValues = { 0xc604, 0xc622, 0xc645, 0xc677, 0xc69f };

            public static void SetStartArea(byte[] rom, LevelIndex start) {
                if (start == LevelIndex.Brinstar) return;

                rom[StartLevelOffset] = (byte)((2 + (int)start) & 0xFF);

                // Swap area init w/ brinstars
                WritePointer(rom, BankInitBranchTableOffset, BankInitBranchTableValues[(int)start]);
                WritePointer(rom, BankInitBranchTableOffset + ((int)start * 2), BankInitBranchTableValues[(int)LevelIndex.Brinstar]);

                // Swap graphic routines
                WritePointer(rom,
                    GraphicInitJumpInstructionOffsets[0] + 1,
                    GraphicInitJumpInstructionValues[(int)start]);
                WritePointer(rom,
                    GraphicInitJumpInstructionOffsets[(int)start] + 1,
                    GraphicInitJumpInstructionValues[0]);
            }

            static void WritePointer(byte[] data, int offset, int ptr) {
                data[offset] = (byte)(ptr & 0xFF);
                data[offset + 1] = (byte)((ptr >> 8) & 0xFF);
            }
        }
        static class ExpandoAreaInitializers
        {
            static int StartLevelOffset = 0x1365;
            static int BankInitBranchTableOffset = 0x3C531;
            static int[] BankInitBranchTableValues = { 0xc552, 0xc583, 0xc590, 0xc5b6, 0xc5c3 };

            static int[] GraphicInitJumpInstructionOffsets = { 0x3C582, 0x3C59a, 0x3C5b2, 0x3C5cd, 0x3C5da, };
            static int[] GraphicInitJumpInstructionValues = { 0xc5FA, 0xc604, 0xc60e, 0xc618, 0xc622 };

            public static void SetStartArea(byte[] rom, LevelIndex start) {
                if (start == LevelIndex.Brinstar) return;

                rom[StartLevelOffset] = (byte)((2 + (int)start) & 0xFF);

                // Swap area init w/ brinstars
                WritePointer(rom, BankInitBranchTableOffset, BankInitBranchTableValues[(int)start]);
                WritePointer(rom, BankInitBranchTableOffset + ((int)start * 2), BankInitBranchTableValues[(int)LevelIndex.Brinstar]);

                // Swap graphic routines
                WritePointer(rom,
                    GraphicInitJumpInstructionOffsets[0] + 1,
                    GraphicInitJumpInstructionValues[(int)start]);
                WritePointer(rom,
                    GraphicInitJumpInstructionOffsets[(int)start] + 1,
                    GraphicInitJumpInstructionValues[0]);
            }

            static void WritePointer(byte[] data, int offset, int ptr) {
                data[offset] = (byte)(ptr & 0xFF);
                data[offset + 1] = (byte)((ptr >> 8) & 0xFF);
            }
        }

        static class EnhancoAreaInitializers
        {
            static int StartLevelOffset = 0x1365;
            static int BankInitBranchTableOffset = 0x3C531;
            static int[] BankInitBranchTableValues = { 0xc552, 0xc583, 0xc590, 0xc5b6, 0xc5c3 };

            static int[] GraphicInitJumpInstructionOffsets = { 0x3C582, 0x3C59a, 0x3C5b2, 0x3C5cd, 0x3C5da, };
            //static int[] GraphicInitJumpInstructionValues = { 0xc5FA, 0xc604, 0xc60e, 0xc618, 0xc622 };

            public static void SetStartArea(byte[] rom, LevelIndex start) {
                if (start == LevelIndex.Brinstar) return;


                rom[StartLevelOffset] = (byte)((2 + (int)start) & 0xFF);

                // Swap area init w/ brinstars
                WritePointer(rom, BankInitBranchTableOffset, BankInitBranchTableValues[(int)start]);
                WritePointer(rom, BankInitBranchTableOffset + ((int)start * 2), BankInitBranchTableValues[(int)LevelIndex.Brinstar]);

                // Swap graphic routines
                var ptrA = ReadPointer(rom, GraphicInitJumpInstructionOffsets[0] + 1);
                var ptrB = ReadPointer(rom, GraphicInitJumpInstructionOffsets[(int)start] + 1);
                WritePointer(rom, GraphicInitJumpInstructionOffsets[0] + 1,ptrB);
                WritePointer(rom, GraphicInitJumpInstructionOffsets[(int)start] + 1, ptrB);
            }

            static void WritePointer(byte[] data, int offset, int ptr) {
                data[offset] = (byte)(ptr & 0xFF);
                data[offset + 1] = (byte)((ptr >> 8) & 0xFF);
            }
            static int ReadPointer(byte[] data, int offset) {
                return data[offset] + (data[offset + 1] << 8);
            }
        }

        static class MMC3AreaInitializers
        {
            static int StartLevelOffset = 0x1365;
            static int BankInitBranchTableOffset = 0x7C531;
            static int[] BankInitBranchTableValues = { 0xc552, 0xc583, 0xc590, 0xc5b6, 0xc5c3 };

            static int[] GraphicInitJumpInstructionOffsets = { 0x7C582, 0x7C59a, 0x7C5b2, 0x7C5cd, 0x7C5da, };
            //static int[] GraphicInitJumpInstructionValues = { 0xc5FA, 0xc604, 0xc60e, 0xc618, 0xc622 };

            public static void SetStartArea(byte[] rom, LevelIndex start) {
                if (start == LevelIndex.Brinstar) return;


                rom[StartLevelOffset] = (byte)((2 + (int)start) & 0xFF);

                // Swap area init w/ brinstars
                WritePointer(rom, BankInitBranchTableOffset, BankInitBranchTableValues[(int)start]);
                WritePointer(rom, BankInitBranchTableOffset + ((int)start * 2), BankInitBranchTableValues[(int)LevelIndex.Brinstar]);

                // Swap graphic routines
                var ptrA = ReadPointer(rom, GraphicInitJumpInstructionOffsets[0] + 1);
                var ptrB = ReadPointer(rom, GraphicInitJumpInstructionOffsets[(int)start] + 1);
                WritePointer(rom, GraphicInitJumpInstructionOffsets[0] + 1, ptrB);
                WritePointer(rom, GraphicInitJumpInstructionOffsets[(int)start] + 1, ptrB);
            }

            static void WritePointer(byte[] data, int offset, int ptr) {
                data[offset] = (byte)(ptr & 0xFF);
                data[offset + 1] = (byte)((ptr >> 8) & 0xFF);
            }
            static int ReadPointer(byte[] data, int offset) {
                return data[offset] + (data[offset + 1] << 8);
            }
        }


        class Patch
        {
            byte[] data;
            int offset;
            /// <summary>
            /// (relative offsets of values to be inserted into patch)
            /// </summary>
            int[] insertOffsets;

            private Patch(byte[] data, int offset, int[] insertOffsets) {
                this.data = data;
                this.offset = offset;
                this.insertOffsets = insertOffsets;
            }
            private Patch(byte[] data, int offset) {
                this.data = data;
                this.offset = offset;
            }

            public static Patch VerticalScrolling = new Patch(
                new byte[] { 0xEA, 0xEA },
                0x1C912);
            public static Patch ResetOnDie = new Patch(
                new byte[] { 0xB0, 0xFF },
                0x1C159);
            public static Patch SkipTitle = new Patch(
                new byte[] { 0xEA, 0xEA },
                0x1A);
            public static Patch SkipStartPass = new Patch(
                new byte[] { 0xEA, 0xEA },
                0x10ED);

            public static Patch ScreenY = new Patch(
                new byte[] { 0xEA, 0xA9, 0xFF },
                0x1C91E,
                new int[] { 2 });
            public static Patch ScreenX = new Patch(
                new byte[] { 0xFF },
                0x1C925,
                new int[] { 0 });
            public static Patch SkipStartMusic = new Patch(
                new byte[] { 0x00 },
                0x1C8E6);
            public static Patch BlankPasswordScreen = new Patch(
                new byte[] { 0x0F, 0x0F, 0x0F, 0x0F },
                0x1755);


            ////public void Apply(byte[] data) {
            ////    Apply(data, false);

            ////}
            public void Apply(byte[] data, int fixedBankAdjust) {
                int adjust = (offset > 0x1C010) ? fixedBankAdjust : 0;
                Array.Copy(this.data, 0, data, this.offset + adjust, this.data.Length);
            }

            ////public void Apply(byte[] data, byte[] inserts) {
            ////    Apply(data, inserts, false);
            ////}
            public void Apply(byte[] data, byte[] inserts, int fixedBankAdjust) {
                int adjust = (offset > 0x1C010) ? fixedBankAdjust : 0;
                
                Array.Copy(this.data, 0, data, this.offset + adjust, this.data.Length);

                for (int i = 0; i < inserts.Length; i++) {
                    data[this.offset + adjust + insertOffsets[i]] = inserts[i];
                }
            }
        }
    }
}
