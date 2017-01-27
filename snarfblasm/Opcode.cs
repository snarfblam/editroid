using System;
using System.Collections.Generic;
using System.Text;

namespace snarfblasm
{
    static class Opcode
    {

        public enum addressing : byte
        {
            implied,
            immediate,        // #$7F
            //accumulator,
            absolute,         // $007F
            zeropage,         // $7F
            indirect,         // ($7F)
            indirectX,        // ($007F,X)
            indirectY,        // ($007F),Y
            relative,
            zeropageIndexedX, // $7F,X
            zeropageIndexedY, // $7F,Y
            absoluteIndexedX, // $007F,X
            absoluteIndexedY, // $007F,Y
        }

        public struct op
        {
            public byte index;
            public string name;
            public addressing addressing;
            public byte cycleCount;
            public bool valid;
            public bool mystery;

            public op(byte index, string name, addressing addressing, int cycleCount, bool valid, bool mystery) {
                this.index = index;
                this.name = name;
                this.addressing = addressing;
                this.cycleCount = (byte)cycleCount;
                this.valid = valid;
                this.mystery = mystery;
            }
        }
        public static op[] allOps = new op[]{
            new op(0x00, "BRK", addressing.implied, 7, true, true ), 
            new op(0x01, "ORA", addressing.indirectX, 6, true, true ), 
            new op(0x02, "KIL", addressing.implied, 0, false, true ), 
            new op(0x03, "ASO", addressing.indirectX, 8, false, true ), 
            new op(0x04, "DOP", addressing.zeropage, 3, false, true ), 
            new op(0x05, "ORA", addressing.zeropage, 3, true, true ), 
            new op(0x06, "ASL", addressing.zeropage, 5, true, true ), 
            new op(0x07, "ASO", addressing.zeropage, 5, false, true ), 
            new op(0x08, "PHP", addressing.implied, 3, true, true ), 
            new op(0x09, "ORA", addressing.immediate, 2, true, true ), 
            new op(0x0A, "ASL", addressing.implied, 2, true, true ), 
            new op(0x0B, "ANC", addressing.immediate, 2, false, true ), 
            new op(0x0C, "TOP", addressing.absolute, 4, false, true ), 
            new op(0x0D, "ORA", addressing.absolute, 4, true, true ), 
            new op(0x0E, "ASL", addressing.absolute, 6, true, true ), 
            new op(0x0F, "ASO", addressing.absolute, 6, false, true ), 
            new op(0x10, "BPL", addressing.relative, 2, true, true ), 
            new op(0x11, "ORA", addressing.indirectY, 5, true, false ), 
            new op(0x12, "KIL", addressing.implied, 0, false, true ), 
            new op(0x13, "ASO", addressing.indirectY, 8, false, true ), 
            new op(0x14, "DOP", addressing.zeropageIndexedX, 4, false, true ), 
            new op(0x15, "ORA", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0x16, "ASL", addressing.zeropageIndexedX, 6, true, true ), 
            new op(0x17, "ASO", addressing.zeropageIndexedX, 6, false, true ), 
            new op(0x18, "CLC", addressing.implied, 2, true, true ), 
            new op(0x19, "ORA", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0x1A, "NOP", addressing.implied, 2, false, true ), 
            new op(0x1B, "ASO", addressing.absoluteIndexedY, 7, false, true ), 
            new op(0x1C, "TOP", addressing.absoluteIndexedX, 4, false, false ), 
            new op(0x1D, "ORA", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0x1E, "ASL", addressing.absoluteIndexedX, 7, true, true ), 
            new op(0x1F, "ASO", addressing.absoluteIndexedX, 7, false, true ), 
            new op(0x20, "JSR", addressing.absolute, 6, true, true ), 
            new op(0x21, "AND", addressing.indirectX, 6, true, true ), 
            new op(0x22, "KIL", addressing.implied, 0, false, true ), 
            new op(0x23, "RLA", addressing.indirectX, 8, false, true ), 
            new op(0x24, "BIT", addressing.zeropage, 3, true, true ), 
            new op(0x25, "AND", addressing.zeropage, 3, true, true ), 
            new op(0x26, "ROL", addressing.zeropage, 5, true, true ), 
            new op(0x27, "RLA", addressing.zeropage, 5, false, true ), 
            new op(0x28, "PLP", addressing.implied, 4, true, true ), 
            new op(0x29, "AND", addressing.immediate, 2, true, true ), 
            new op(0x2A, "ROL", addressing.implied, 2, true, true ), 
            new op(0x2B, "ANC", addressing.immediate, 2, false, true ), 
            new op(0x2C, "BIT", addressing.absolute, 4, true, true ), 
            new op(0x2D, "AND", addressing.absolute, 4, true, true ), 
            new op(0x2E, "ROL", addressing.absolute, 6, true, true ), 
            new op(0x2F, "RLA", addressing.absolute, 6, false, true ), 
            new op(0x30, "BMI", addressing.relative, 2, true, true ), 
            new op(0x31, "AND", addressing.indirectY, 5, true, false ), 
            new op(0x32, "KIL", addressing.implied, 0, false, true ), 
            new op(0x33, "RLA", addressing.indirectY, 8, false, true ), 
            new op(0x34, "DOP", addressing.zeropageIndexedX, 4, false, true ), 
            new op(0x35, "AND", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0x36, "ROL", addressing.zeropageIndexedX, 6, true, true ), 
            new op(0x37, "RLA", addressing.zeropageIndexedX, 6, false, true ), 
            new op(0x38, "SEC", addressing.implied, 2, true, true ), 
            new op(0x39, "AND", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0x3A, "NOP", addressing.implied, 2, false, true ), 
            new op(0x3B, "RLA", addressing.absoluteIndexedY, 7, false, true ), 
            new op(0x3C, "TOP", addressing.absoluteIndexedX, 4, false, false ), 
            new op(0x3D, "AND", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0x3E, "ROL", addressing.absoluteIndexedX, 7, true, false ), 
            new op(0x3F, "RLA", addressing.absoluteIndexedX, 7, false, true ), 
            new op(0x40, "RTI", addressing.implied, 6, true, true ), 
            new op(0x41, "EOR", addressing.indirectX, 6, true, true ), 
            new op(0x42, "KIL", addressing.implied, 0, false, true ), 
            new op(0x43, "LSE", addressing.indirectX, 8, false, true ), 
            new op(0x44, "DOP", addressing.zeropage, 3, false, true ), 
            new op(0x45, "EOR", addressing.zeropage, 3, true, true ), 
            new op(0x46, "LSR", addressing.zeropage, 5, true, true ), 
            new op(0x47, "LSE", addressing.zeropage, 5, false, true ), 
            new op(0x48, "PHA", addressing.implied, 3, true, true ), 
            new op(0x49, "EOR", addressing.immediate, 2, true, true ), 
            new op(0x4A, "LSR", addressing.implied, 2, true, true ), 
            new op(0x4B, "ALR", addressing.immediate, 2, false, true ), 
            new op(0x4C, "JMP", addressing.absolute, 3, true, true ), 
            new op(0x4D, "EOR", addressing.absolute, 4, true, true ), 
            new op(0x4E, "LSR", addressing.absolute, 6, true, true ), 
            new op(0x4F, "LSE", addressing.absolute, 6, false, true ), 
            new op(0x50, "BVC", addressing.relative, 2, true, true ), 
            new op(0x51, "EOR", addressing.indirectY, 5, true, false ), 
            new op(0x52, "KIL", addressing.implied, 0, false, true ), 
            new op(0x53, "LSE", addressing.indirectY, 8, false, true ), 
            new op(0x54, "DOP", addressing.zeropageIndexedX, 4, false, true ), 
            new op(0x55, "EOR", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0x56, "LSR", addressing.zeropageIndexedX, 6, true, true ), 
            new op(0x57, "LSE", addressing.zeropageIndexedX, 6, false, true ), 
            new op(0x58, "CLI", addressing.implied, 2, true, true ), 
            new op(0x59, "EOR", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0x5A, "NOP", addressing.implied, 2, false, true ), 
            new op(0x5B, "LSE", addressing.absoluteIndexedY, 7, false, true ), 
            new op(0x5C, "TOP", addressing.absoluteIndexedX, 4, false, false ), 
            new op(0x5D, "EOR", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0x5E, "LSR", addressing.absoluteIndexedX, 7, true, true ), 
            new op(0x5F, "LSE", addressing.absoluteIndexedX, 7, false, true ), 
            new op(0x60, "RTS", addressing.implied, 6, true, true ), 
            new op(0x61, "ADC", addressing.indirectX, 6, true, true ), 
            new op(0x62, "KIL", addressing.implied, 0, false, true ), 
            new op(0x63, "RRA", addressing.indirectX, 8, false, true ), 
            new op(0x64, "DOP", addressing.zeropage, 3, false, true ), 
            new op(0x65, "ADC", addressing.zeropage, 3, true, true ), 
            new op(0x66, "ROR", addressing.zeropage, 5, true, true ), 
            new op(0x67, "RRA", addressing.zeropage, 5, false, true ), 
            new op(0x68, "PLA", addressing.implied, 4, true, true ), 
            new op(0x69, "ADC", addressing.immediate, 2, true, true ), 
            new op(0x6A, "ROR", addressing.implied, 2, true, true ), 
            new op(0x6B, "ARR", addressing.immediate, 2, false, true ), 
            new op(0x6C, "JMP", addressing.indirect, 5, true, true ), 
            new op(0x6D, "ADC", addressing.absolute, 4, true, true ), 
            new op(0x6E, "ROR", addressing.absolute, 6, true, true ), 
            new op(0x6F, "RRA", addressing.absolute, 6, false, true ), 
            new op(0x70, "BVS", addressing.relative, 2, true, true ), 
            new op(0x71, "ADC", addressing.indirectY, 5, true, false ), 
            new op(0x72, "KIL", addressing.implied, 0, false, true ), 
            new op(0x73, "RRA", addressing.indirectY, 8, false, true ), 
            new op(0x74, "DOP", addressing.zeropageIndexedX, 4, false, true ), 
            new op(0x75, "ADC", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0x76, "ROR", addressing.zeropageIndexedX, 6, true, true ), 
            new op(0x77, "RRA", addressing.zeropageIndexedX, 6, false, true ), 
            new op(0x78, "SEI", addressing.implied, 2, true, true ), 
            new op(0x79, "ADC", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0x7A, "NOP", addressing.implied, 2, false, true ), 
            new op(0x7B, "RRA", addressing.absoluteIndexedY, 7, false, true ), 
            new op(0x7C, "TOP", addressing.absoluteIndexedX, 4, false, false ), 
            new op(0x7D, "ADC", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0x7E, "ROR", addressing.absoluteIndexedX, 7, true, true ), 
            new op(0x7F, "RRA", addressing.absoluteIndexedX, 7, false, true ), 
            new op(0x80, "DOP", addressing.immediate, 2, false, false ), 
            new op(0x81, "STA", addressing.indirectX, 6, true, true ), 
            new op(0x82, "DOP", addressing.immediate, 2, false, true ), 
            new op(0x83, "AXS", addressing.indirectX, 6, false, true ), 
            new op(0x84, "STY", addressing.zeropage, 3, true, true ), 
            new op(0x85, "STA", addressing.zeropage, 3, true, true ), 
            new op(0x86, "STX", addressing.zeropage, 3, true, true ), 
            new op(0x87, "AXS", addressing.zeropage, 3, false, true ), 
            new op(0x88, "DEY", addressing.implied, 2, true, true ), 
            new op(0x89, "DOP", addressing.immediate, 2, false, true ), 
            new op(0x8A, "TXA", addressing.implied, 2, true, true ), 
            new op(0x8B, "XAA", addressing.immediate, 2, false, true ), 
            new op(0x8C, "STY", addressing.absolute, 4, true, true ), 
            new op(0x8D, "STA", addressing.absolute, 4, true, true ), 
            new op(0x8E, "STX", addressing.absolute, 4, true, true ), 
            new op(0x8F, "AXS", addressing.absolute, 4, false, true ), 
            new op(0x90, "BCC", addressing.relative, 2, true, true ), 
            new op(0x91, "STA", addressing.indirectY, 6, true, true ), 
            new op(0x92, "KIL", addressing.implied, 0, false, true ), 
            new op(0x93, "AXA", addressing.indirectY, 6, false, false ), 
            new op(0x94, "STY", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0x95, "STA", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0x96, "STX", addressing.zeropageIndexedY, 4, true, true ), 
            new op(0x97, "AXS", addressing.zeropageIndexedY, 4, false, true ), 
            new op(0x98, "TYA", addressing.implied, 2, true, true ), 
            new op(0x99, "STA", addressing.absoluteIndexedY, 5, true, true ), 
            new op(0x9A, "TXS", addressing.implied, 2, true, true ), 
            new op(0x9B, "TAS", addressing.absoluteIndexedY, 5, false, true ), 
            new op(0x9C, "SAY", addressing.absoluteIndexedX, 5, false, true ), 
            new op(0x9D, "STA", addressing.absoluteIndexedX, 5, true, true ), 
            new op(0x9E, "XAS", addressing.absoluteIndexedY, 5, false, true ), 
            new op(0x9F, "AXA", addressing.absoluteIndexedY, 5, false, true ), 
            new op(0xA0, "LDY", addressing.immediate, 2, true, true ), 
            new op(0xA1, "LDA", addressing.indirectX, 6, true, true ), 
            new op(0xA2, "LDX", addressing.immediate, 2, true, true ), 
            new op(0xA3, "LAX", addressing.indirectX, 6, false, true ), 
            new op(0xA4, "LDY", addressing.zeropage, 3, true, true ), 
            new op(0xA5, "LDA", addressing.zeropage, 3, true, true ), 
            new op(0xA6, "LDX", addressing.zeropage, 3, true, true ), 
            new op(0xA7, "LAX", addressing.zeropage, 3, false, true ), 
            new op(0xA8, "TAY", addressing.implied, 2, true, true ), 
            new op(0xA9, "LDA", addressing.immediate, 2, true, true ), 
            new op(0xAA, "TAX", addressing.implied, 2, true, true ), 
            new op(0xAB, "OAL", addressing.immediate, 2, false, true ), 
            new op(0xAC, "LDY", addressing.absolute, 4, true, true ), 
            new op(0xAD, "LDA", addressing.absolute, 4, true, true ), 
            new op(0xAE, "LDX", addressing.absolute, 4, true, true ), 
            new op(0xAF, "LAX", addressing.absolute, 4, false, true ), 
            new op(0xB0, "BCS", addressing.relative, 2, true, true ), 
            new op(0xB1, "LDA", addressing.indirectY, 5, true, false ), 
            new op(0xB2, "KIL", addressing.implied, 0, false, true ), 
            new op(0xB3, "LAX", addressing.indirectY, 5, false, false ), 
            new op(0xB4, "LDY", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0xB5, "LDA", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0xB6, "LDX", addressing.zeropageIndexedY, 4, true, true ), 
            new op(0xB7, "LAX", addressing.zeropageIndexedY, 4, false, true ), 
            new op(0xB8, "CLV", addressing.implied, 2, true, true ), 
            new op(0xB9, "LDA", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0xBA, "TSX", addressing.implied, 2, true, true ), 
            new op(0xBB, "LAS", addressing.absoluteIndexedY, 4, false, false ), 
            new op(0xBC, "LDY", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0xBD, "LDA", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0xBE, "LDX", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0xBF, "LAX", addressing.absoluteIndexedY, 4, false, false ), 
            new op(0xC0, "CPY", addressing.immediate, 2, true, true ), 
            new op(0xC1, "CMP", addressing.indirectX, 6, true, true ), 
            new op(0xC2, "DOP", addressing.immediate, 2, false, true ), 
            new op(0xC3, "DCM", addressing.indirectX, 8, false, true ), 
            new op(0xC4, "CPY", addressing.zeropage, 3, true, true ), 
            new op(0xC5, "CMP", addressing.zeropage, 3, true, true ), 
            new op(0xC6, "DEC", addressing.zeropage, 5, true, true ), 
            new op(0xC7, "DCM", addressing.zeropage, 5, true, true ), 
            new op(0xC8, "INY", addressing.implied, 2, true, true ), 
            new op(0xC9, "CMP", addressing.immediate, 2, true, true ), 
            new op(0xCA, "DEX", addressing.implied, 2, true, true ), 
            new op(0xCB, "SAX", addressing.immediate, 2, false, true ), 
            new op(0xCC, "CPY", addressing.absolute, 4, true, true ), 
            new op(0xCD, "CMP", addressing.absolute, 4, true, true ), 
            new op(0xCE, "DEC", addressing.absolute, 6, true, true ), 
            new op(0xCF, "DCM", addressing.absolute, 6, false, true ), 
            new op(0xD0, "BNE", addressing.relative, 2, true, true ), 
            new op(0xD1, "CMP", addressing.indirectY, 5, true, false ), 
            new op(0xD2, "KIL", addressing.implied, 0, false, true ), 
            new op(0xD3, "DCM", addressing.indirectY, 8, false, true ), 
            new op(0xD4, "DOP", addressing.zeropageIndexedX, 4, false, true ), 
            new op(0xD5, "CMP", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0xD6, "DEC", addressing.zeropageIndexedX, 6, true, true ), 
            new op(0xD7, "DCM", addressing.zeropageIndexedX, 6, false, true ), 
            new op(0xD8, "CLD", addressing.implied, 2, true, true ), 
            new op(0xD9, "CMP", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0xDA, "NOP", addressing.implied, 2, false, true ), 
            new op(0xDB, "DCM", addressing.absoluteIndexedY, 7, false, true ), 
            new op(0xDC, "TOP", addressing.absoluteIndexedX, 4, false, false ), 
            new op(0xDD, "CMP", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0xDE, "DEC", addressing.absoluteIndexedX, 7, true, true ), 
            new op(0xDF, "DCM", addressing.absoluteIndexedX, 7, false, true ), 
            new op(0xE0, "CPX", addressing.immediate, 2, true, true ), 
            new op(0xE1, "SBC", addressing.indirectX, 6, true, true ), 
            new op(0xE2, "DOP", addressing.immediate, 2, false, true ), 
            new op(0xE3, "INS", addressing.indirectX, 8, false, true ), 
            new op(0xE4, "CPX", addressing.zeropage, 3, true, true ), 
            new op(0xE5, "SBC", addressing.zeropage, 3, true, true ), 
            new op(0xE6, "INC", addressing.zeropage, 5, true, true ), 
            new op(0xE7, "INS", addressing.zeropage, 5, false, true ), 
            new op(0xE8, "INX", addressing.implied, 2, true, true ), 
            new op(0xE9, "SBC", addressing.immediate, 2, true, true ), 
            new op(0xEA, "NOP", addressing.implied, 2, true, true ), 
            new op(0xEB, "SBC", addressing.immediate, 2, false, true ), 
            new op(0xEC, "CPX", addressing.absolute, 4, true, true ), 
            new op(0xED, "SBC", addressing.absolute, 4, true, true ), 
            new op(0xEE, "INC", addressing.absolute, 6, true, true ), 
            new op(0xEF, "INS", addressing.absolute, 6, false, true ), 
            new op(0xF0, "BEQ", addressing.relative, 2, true, true ), 
            new op(0xF1, "SBC", addressing.indirectY, 5, true, false ), 
            new op(0xF2, "KIL", addressing.implied, 0, false, true ), 
            new op(0xF3, "INS", addressing.indirectY, 8, false, true ), 
            new op(0xF4, "DOP", addressing.zeropageIndexedX, 4, false, true ), 
            new op(0xF5, "SBC", addressing.zeropageIndexedX, 4, true, true ), 
            new op(0xF6, "INC", addressing.zeropageIndexedX, 6, true, true ), 
            new op(0xF7, "INS", addressing.zeropageIndexedX, 6, false, true ), 
            new op(0xF8, "SED", addressing.implied, 2, true, true ), 
            new op(0xF9, "SBC", addressing.absoluteIndexedY, 4, true, false ), 
            new op(0xFA, "NOP", addressing.implied, 2, false, true ), 
            new op(0xFB, "INS", addressing.absoluteIndexedY, 7, false, true ), 
            new op(0xFC, "TOP", addressing.absoluteIndexedX, 4, false, false ), 
            new op(0xFD, "SBC", addressing.absoluteIndexedX, 4, true, false ), 
            new op(0xFE, "INC", addressing.absoluteIndexedX, 7, true, true ), 
            new op(0xFF, "INS", addressing.absoluteIndexedX, 7, false, true ) ,
        };

        public static int GetParamBytes(byte opcode) {
            return GetParamBytes(allOps[opcode].addressing);
        }
        public static int GetParamBytes(addressing addrMode) {
            switch (addrMode) {
                case addressing.implied:
                    // No operand
                    return 0;
                case addressing.relative:
                    // Operand is a signed byte
                    return 1;
                case addressing.immediate:
                    // Operand is a byte literal
                    return 1;
                case addressing.zeropage:
                case addressing.zeropageIndexedX:
                case addressing.zeropageIndexedY:
                case addressing.indirectX:
                case addressing.indirectY:
                    // All operate on zero-page
                    return 1;
                case addressing.absolute:
                case addressing.indirect:
                case addressing.absoluteIndexedX:
                case addressing.absoluteIndexedY:
                    // All use 16-bit address
                    return 2;
                default:
                    throw new ArgumentException("Invalid addressing mode specified.");
            }
        }

    }
}
