using System;

enum Commands{
        //Commands:
        //END
        HALT = 0x00,
        //Arithmetics
        AD = 0x10,
        ADD = 0x11,
        SB = 0x12,
        SUB = 0x13,
        MU = 0x14,
        MUL = 0x15,
        CBN = 0x16,
        CEN = 0x17,
        //Work w/ memory
        LR = 0x20,
        LB = 0x21,
        SR = 0x22,
        SS = 0x23,
        //I/O
        GD = 0x30,
        PD = 0x31,
        PI = 0x32, //print Integer from adress xy
        PA = 0x33, //print Integer from register RA
        SAI = 0x34, //save Integer from input to register RA
        SXI = 0x35, //save Integer from input to adress xy
        //Jumps
        JP = 0x40,
        JC = 0x41,
        //Work w/ files
        FO = 0x50,
        FN = 0x51,
        FRCL = 0x52,
        FNCL = 0x53,
        FR = 0x54,
        FREA = 0x55,
        FP = 0x56,
        FPUT = 0x57
}