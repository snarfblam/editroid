using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.Asm
{
    //    = implied or absolute, or relative for branch instructions
    // xi = x indexed
    // yi = y indexed
    // im = immediate
    // z  = zero page
    enum Opcodes: byte
    {
        brk = 0x00,
        ora_ix,
        op02,
        op03,
        op04,
        ora_z,
        asl_z,
        op07,

        php,
        ora_im,
        asl_a,
        op0b,
        op0C,
        ora_i,
        asl,
        op0F,

        bpl, //    $10
        ora_zpy,
        op12,
        op13,
        op14,
        ora_zpx,
        als_zpx,
        op17,

        clc,
        ora_yi,
        op1A,
        op1B,
        op1C,
        ora_xi,
        asl_xi,
        op1F,
                                                      
        jsr = 0x20,

//21    AND (zp,X)              AND (zp,X)              AND (zp,X)
//22  * HALT                  * ?                     * ?
//23  * ROL-AND (zp,X)        * ?                     * ?
//24    BIT zp                  BIT zp                  BIT zp
//25    AND zp                  AND zp                  AND zp
//26    ROL zp                  ROL zp                  ROL zp
//27  * ROL-AND zp            * ?                     * ?
//28    PLP                     PLP                     PLP
//29    AND #n                  AND #n                  AND #n
//2A    ROL A                   ROL A                   ROL A
//2B  * AND #n-MOV b7->Cy     * ?                     * ?
//2C    BIT abs                 BIT abs                 BIT abs
//2D    AND abs                 AND abs                 AND abs
//2E    ROL abs                 ROL abs                 ROL abs
//2F  * ROL-AND abs           * ?                       BBR 0,zp,rel

//30    BMI rel                 BMI rel                 BMI rel
//31    AND (zp),Y              AND (zp),Y              AND (zp),Y
//32  * HALT                    AND (zp)                AND (zp)
//33  * ROL-AND (zp),Y        * ?                     * ?
//34  * NOP zp                  BIT zp,X                BIT zp,X
//35    AND zp,X                AND zp,X                AND zp,X
//36    ROL zp,X                ROL zp,X                ROL zp,X
//37  * ROL-AND zp,X          * ?                     * ?
//38    SEC                     SEC                     SEC
//39    AND abs,Y               AND abs,Y               AND abs,Y
//3A  * NOP                     DEC A                   DEC A
//3B  * ROL-AND abs,Y         * ?                     * ?
//3C  * NOP abs                 BIT abs,X               BIT abs,X
//3D    ORA abs,X               ORA abs,X               ORA abs,X
//3E    ASL abs,X               ASL abs,X               ASL abs,X
//3F  * ROL-AND abs,X         * ?                       BBR 1,zp,rel

//40    RTI                     RTI                     RTI
//41    EOR (zp,X)              EOR (zp,X)              EOR (zp,X)
//42  * HALT                  * ?                     * ?
//43  * LSR-EOR (zp,X)        * ?                     * ?
//44  * NOP zp                * ?                     * ?
//45    EOR zp                  EOR zp                  EOR zp
//46    LSR zp                  LSR zp                  LSR zp
//47  * LSR-EOR zp            * ?                     * ?
//48    PHA                     PHA                     PHA
//49    EOR #n                  EOR #n                  EOR #n
//4A    LSR A                   LSR A                   LSR A
//4B  * AND #n-LSR A          * ?                     * ?
jmp = 0x4C,
//4D    EOR abs                 EOR abs                 EOR abs
//4E    LSR abs                 LSR abs                 LSR abs
//4F  * LSR-EOR abs           * ?                       BBR 0,zp,rel

//50    BVC rel                 BVC rel                 BVC rel
//51    EOR (zp),Y              EOR (zp),Y              EOR (zp),Y
//52  * HALT                    EOR (zp)                EOR (zp)
//53  * LSR-EOR (zp),Y        * ?                     * ?
//54  * NOP zp                * ?                     * ?
//55    EOR zp,X                EOR zp,X                EOR zp,X
//56    LSR zp,X                LSR zp,X                LSR zp,X
//57  * LSR-EOR abs,X         * ?                     * ?
//58    CLI                     CLI                     CLI
//59    EOR abs,Y               EOR abs,Y               EOR abs,Y
//5A  * NOP                     PHY                     PHY
//5B  * LSR-EOR abs,Y         * ?                     * ?
//5C  * NOP abs               * ?                     * ?
//5D    EOR abs,X               EOR abs,X               EOR abs,X
//5E    LSR abs,X               LSR abs,X               LSR abs,X
//5F  * LSR-EOR abs,X         * ?                       BBR 1,zp,rel

rts = 0x60,                  
adc_xi,
op_62,
op_63,
op_64,
adc_z,
ror_z,
op_67,
pla,
adc_im,
ror_a,
op_6B,
jmp_i,
adc,
ror,
op_6F,

//70    BCS rel                 BCS rel                 BCS rel
//71    ADC (zp),Y              ADC (zp),Y              ADC (zp),Y
//72  * HALT                    ADC (zp)                ADC (zp)
//73  * ROR-ADC (zp),Y        * ?                     * ?
//74  * NOP zp                  STZ zp,X                STZ zp,X
//75    ADC zp,X                ADC zp,X                ADC zp,X
//76    ROR zp,X                ROR zp,X                ROR zp,X
//77  * ROR-ADC abs,X         * ?                     * ?
//78    SEI                     SEI                     SEI
//79    ADC abs,Y               ADC abs,Y               ADC abs,Y
//7A  * NOP                     PLY                     PLY
//7B  * ROR-ADC abs,Y         * ?                     * ?
//7C  * NOP abs                 JMP (abs,X)             JMP (abs,X)
//7D    ADC abs,X               ADC abs,X               ADC abs,X
//7E    ROR abs,X               ROR abs,X               ROR abs,X
//7F  * ROR-ADC abs,X         * ?                       BBR 1,zp,rel

op_80 = 0x80,
sta_xi,
op_82,
op_83,
sty_z = 0x84,
sta_z,
stx_z,
op_87,
dey = 0x88,
//89  * NOP zp                  BIT #n                  BIT #n
//8A    TXA A                   TXA                     TXA
//8B  * TXA-AND #n            * ?                     * ?
sty=0x8C,
sta,
stx,
//8F  * STA-STX abs           * ?                       BBR 0,zp,rel

bcc = 0x90,
sta_iy,
op_92,
op_93,
sty_zx = 0x94,
sta_zx,
stx_zy,
op_97,
tya,
sta_y,
txs,
op_9B,
op_9C,
sta_x,
op_9E,
op_9F,

ldy_im = 0xA0,
lda_xi,
ldx_im,
op_A3,
ldy_z,
lda_z,
ldx_z,
op_A7,
tay,
lda_im,
tax,
//AB  * LDA-LDX               * ?                     * ?
//AC    LDY abs                 LDY abs                 LDY abs
//AD    LDA abs                 LDA abs                 LDA abs
//AE    LDX abs                 LDX abs                 LDX abs
//AF  * LDA-LDX abs           * ?                       BBR 0,zp,rel

bcs = 0xB0,
//B1    LDA (zp),Y              LDA (zp),Y              LDA (zp),Y
//B2  * HALT                    LDA (zp)                LDA (zp)
//B3  * LDA-LDX (zp),Y        * ?                     * ?
//B4    LDY zp                  LDY zp                  LDY zp
//B5    LDA zp,X                LDA zp,X                LDA zp,X
//B6    LDX zp,Y                LDX zp,Y                LDX zp,Y
//B7  * LDA-LDX zp,Y          * ?                     * ?
//B8    CLV                     CLV                     CLV
//B9    LDA abs,Y               LDA abs,Y               LDA abs,Y
//BA    TSX                     TSX                     TSX
//BB  * LDA-LDX abs,Y         * ?                     * ?
//BC    LDY abs,X               LDY abs,X               LDY abs,X
//BD    LDA abs,X               LDA abs,X               LDA abs,X
//BE    LDX abs,Y               LDX abs,Y               LDX abs,Y
//BF  * LDA-LDX abs,Y         * ?                       BBR 1,zp,rel

//C0    CPY #n                  CPY #n                  CPY #n
//C1    CMP (zp,X)              CMP (zp,X)              CMP (zp,X)
//C2  * HALT                  * ?                     * ?
//C3  * DEC-CMP (zp,X)        * ?                     * ?
//C4    CPY zp                  CPY zp                  CPY zp
//C5    CMP zp                  CMP zp                  CMP zp
//C6    DEC zp                  DEC zp                  DEC zp
//C7  * DEC-CMP zp            * ?                     * ?
//C8    INY                     INY                     INY
cmp_im = 0xC9,
//CA    DEX                     DEX                     DEX
//CB  * SBX #n                * ?                     * ?
//CC    CPY abs                 CPY abs                 CPY abs
//CD    CMP abs                 CMP abs                 CMP abs
//CE    DEC abs                 DEC abs                 DEC abs
//CF  * DEC-CMP abs           * ?                       BBR 0,zp,rel

//D0    BNE rel                 BNE rel                 BNE rel
//D1    CMP (zp),Y              CMP (zp),Y              CMP (zp),Y
//D2  * HALT                    CMP (zp)                CMP (zp)
//D3  * DEC-CMP (zp),Y        * ?                     * ?
//D4  * NOP zp                * ?                     * ?
//D5    CMP zp,X                CMP zp,X                CMP zp,X
//D6    DEC zp,X                DEC zp,X                DEC zp,X
//D7  * DEC-CMP zp,X          * ?                     * ?
//D8    CLD                     CLD                     CLD
//D9    CMP abs,Y               CMP abs,Y               CMP abs,Y
//DA  * NOP                     PHX                     PHX
//DB  * DEC-CMP abs,Y         * ?                     * ?
//DC  * NOP abs               * ?                     * ?
//DD    CMP abs,X               CMP abs,X               CMP abs,X
//DE    DEC abs,X               DEC abs,X               DEC abs,X
//DF  * DEC-CMP abs,X         * ?                       BBR 1,zp,rel

//E0    CPX #n                  CPX #n                  CPX #n
//E1    SBC (zp,X)              SBC (zp,X)              SBC (zp,X)
//E2  * HALT                  * ?                     * ?
//E3  * INC-SBC (zp,X)        * ?                     * ?
//E4    CPX zp                  CPX zp                  CPX zp
//E5    SBC zp                  SBC zp                  SBC zp
//E6    INC zp                  INC zp                  INC zp
//E7  * INC-SBC zp            * ?                     * ?
//E8    INX                     INX                     INX
//E9    SBC #n                  SBC #n                  SBC #n
nop = 0xEa,
//EB    SBC #n                  SBC #n                  SBC #n
//EC    CPX abs                 CPX abs                 CPX abs
//ED    SBC abs                 SBC abs                 SBC abs
//EE    INC abs                 INC abs                 INC abs
//EF  * INC-SBC abs           * ?                       BBR 0,zp,rel

//F0    BEQ rel                 BEQ rel                 BEQ rel
//F1    SBC (zp),Y              SBC (zp),Y              SBC (zp),Y
//F2  * HALT                    SBC (zp)                SBC (zp)
//F3  * INC-SBC (zp),Y        * ?                     * ?
//F4  * NOP zp                * ?                     * ?
//F5    SBC zp,X                SBC zp,X                SBC zp,X
//F6    INC zp,X                INC zp,X                INC zp,X
//F7  * INC-SBC zp,X          * ?                     * ?
//F8    SED                     SED                     SED
//F9    SBC abs,Y               SBC abs,Y               SBC abs,Y
//FA  * NOP                     PLX                     PLX
//FB  * INC-SBC abs,Y         * ?                     * ?
//FC  * NOP abs               * ?                     * ?
//FD    SBC abs,X               SBC abs,X               SBC abs,X
//FE    INC abs,X               INC abs,X               INC abs,X
//FF  * INC-SBC abs,X  
    }
}
