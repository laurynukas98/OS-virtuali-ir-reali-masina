using System;
namespace Commands{
enum CommandsType{
    //END
    HALT = 0x00,
    //Arithmetics
    ADxy = 0x10,
    ADD = 0x11,
    SBxy = 0x12,
    SUB = 0x13,
    MUxy = 0x14,
    MUL = 0x15,
    CBN = 0x16,
	CEN = 0x17,
    //Work w/ memory
    LRxy = 0x20,
    LBxy = 0x21,
    SRxy = 0x22,
    SSxy = 0x23,
    //I/O
    GDxy = 0x30,
    PDxy = 0x31,
    PA = 0x32,//Print RA
    SAI = 0x33,//READ INT FROM INPUT n SET TO RA SI = 7
    SIxy = 0x34,
    PIxy = 0x35,//print integer xy
    //Jumps
    JPxy = 0x40,
    JCxy = 0x41,
    //Work w/ files
    FOxy = 0x50,
    FNxy = 0x51,
    FRCL = 0x52,
    FNCL = 0x53,
    FRxy = 0x54,
    FREA = 0x55,
    FPxy = 0x56,
    FPUT = 0x57,
    FEND = 0x58,
    NONE = 0x255
}
}