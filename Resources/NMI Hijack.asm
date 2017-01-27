; Simple one-liner that hijacks the NMI routine to perform CHR bank swapping during NMI

.org $C106
JMP $C669   ; If the location of the bank cycling code changes, this needs to be updated.