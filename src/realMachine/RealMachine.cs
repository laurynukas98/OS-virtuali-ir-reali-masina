using System;

namespace realMachine{

    class RMmemory : Memory{
        
    }

    class InputDevice{

    }

    class OutputDevice{

    }
    
    class CPU{
        //Registers
        private int MODE; //user - 0; supervisor - 1
        private int C;
        private int IC;
        private int PTR;
        private int RA, RB;
        private int TI;
        private int PI;
        private int SI;
        private int CHST1, CHST2, CHST3;
        private RMmemory memory;
        
        public const int TIME = 5;

        public void resetTimer(){
            TI = TIME;
        }

        public void resetInterruptions(){
            TI = TIME;
            PI = 0;
            SI = 0;
        }

        //GET&SET
        public int getMODE(){return MODE;}
        public void setMODE(int MODE){this.MODE = MODE;}

        public int getC(){return C;}
        public void setC(int C){this.C = C;}

        public int getIC(){return IC;}
        public void setIC(int IC){this.IC = IC;}

        public int getPTR(){return PTR;}
        public void setPTR(int PTR){this.PTR = PTR;}

        public int getRA(){return RA;}
        public void setRA(int RA){this.RA = RA;}

        public int getRB(){return RB;}
        public void setRB(int RB){this.RB = RB;}

        public int getTI(){return TI;}
        public void setTI(int TI){this.TI = TI;}

        public int getPI(){return PI;}
        public void setPI(int PI){this.PI = PI;}

        public int getSI(){return SI;}
        public void setSI(int SI){this.SI = SI;}

        public int getCHST1(){return CHST1;}
        public void setCHST1(int CHST1){this.CHST1 = CHST1;}

        public int getCHST2(){return CHST2;}
        public void setCHST2(int CHST2){this.CHST2 = CHST2;}

        public int getCHST3(){return CHST3;}
        public void setCHST3(int CHST3){this.CHST3 = CHST3;}

        //HLP

        //Commands:
        //END
        public const byte HALT = 0x01;
        //Arithmetics
        public const byte ADxy = 0x10;
        public const byte ADD = 0x11;
        public const byte SBxy = 0x12;
        public const byte SUB = 0x13;
        public const byte MUxy = 0x14;
        public const byte MUL = 0x15;
        public const byte CBN = 0x16;
        public const byte CEN = 0x17;
        //Work w/ memory
        public const byte LRxy = 0x20;
        public const byte LBxy = 0x21;
        public const byte SRxy = 0x22;
        public const byte SSxy = 0x23;
        //I/O
        public const byte GDxy = 0x30;
        public const byte PDxy = 0x31;
        //Jumps
        public const byte JPxy = 0x40;
        public const byte JCxy = 0x41;
        //Work w/ files
        public const byte FOxy = 0x50;
        public const byte FNxy = 0x51;
        public const byte FRCL = 0x52;
        public const byte FNCL = 0x53;
        public const byte FRxy = 0x54;
        public const byte FREA = 0x55;
        public const byte FPxy = 0x56;
        public const byte FPUT = 0x57;

        private void interpretate(Word word){

        }

        public CPU(){
            MODE = 1;
            C = IC = PTR = RA = RB = PI = SI = CHST1 = CHST2 = CHST3 = 0;
            TI = TIME;
            memory = new RMmemory();
        }
    }

    class RealMachine{
        private CPU cpu;
        private RMmemory memory;

        RealMachine(){
            cpu = new CPU();
            memory = new RMmemory();
        }
    }

}