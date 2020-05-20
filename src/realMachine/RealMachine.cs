using System;
using System.IO;
using System.Collections.Generic;
using Commands;
using VM;

namespace realMachine{

    class RMmemory : Memory{
        public int maxBlockSize;
        private Word[] memory;
        public readonly CPU cpu;

        public bool isFreeBlock(int index){
            for (int i = 0; i < 16; i++)
                if (memory[index*16+i] != null){
                    return false;
                } 
            return true;
        }

        public void write(int index, int info){
            memory[ Word.toInt(memory[ (Word.convertChar(cpu.getPTR().getByte(2)))*16*16+(Word.convertChar(cpu.getPTR().getByte(3)))*16+index/16 ])*16 +(index%16) ] = new Word(info);
        }

        public void write(int index, Word info){
            memory[ Word.toInt(memory[ ( Word.convertChar(cpu.getPTR().getByte(2)) )*16*16+( Word.convertChar(cpu.getPTR().getByte(3)) )*16+index/16 ])*16 +(index%16) ] = new Word(info);
        }

        public void write(int program, int index, Word info){
            if (info != null)
                memory[Word.toInt(memory[program*16+index/16 ])*16 +(index%16)] = new Word(info);
        }

        public Word read(int index){
            return memory[ Word.toInt(memory[ (Word.convertChar(cpu.getPTR().getByte(2)))*16*16+(Word.convertChar(cpu.getPTR().getByte(3)))*16+index/16 ])*16 +(index%16) ];
        }

        public string readString(int index){
            int size = Word.toInt(read( (Word.convertChar(read(cpu.getIC()-1).getByte(2)))*16 + (Word.convertChar(read(cpu.getIC()-1).getByte(3))) ));
            string ttext = "";
            for (int i = 1; i <= size; i++){
                ttext += Word.toString(read( ((Word.convertChar(read(cpu.getIC()-1).getByte(2)))*16 + (Word.convertChar(read(cpu.getIC()-1).getByte(3))))+i ));
            }
            return ttext.Replace("$","");
        }

        public void allocateBlock(int index){
            for (int i = 0; i < 16; i++)
                memory[index*16+i] = new Word();
        }

        public void allocateMemoryVM(int pagingIndex){
            for (int i = 0; i < 16; i++){//paging cells
                bool found = false;
                for (int o = maxBlockSize-1; o >= 0; o--){
                    if (isFreeBlock(o)){
                        allocateBlock(o);
                        memory[pagingIndex*16+i] = new Word(o);
                        found = true;
                        break;
                    }
                }
                /*for (int o = 0; o <= 16*maxBlockSize; o++){
                    if (isFreeBlock(o)){
                        allocateBlock(o);
                        memory[pagingIndex*16+i] = new Word(o);
                        found = true;
                        break;
                    }
                }*/
                if (!found){
                    Console.Write("ERROR!, NO MEMORY LEFT!");
                    break;
                }
            }
        }

        public void freeBlock(int index){
            for (int i = 0; i < 16; i++)
                memory[index*16+i] = null;
        }

        public int newPaging(){
            int pagingIndex = -1;
            for (int i = 0; i < 16; i++){
                if (isFreeBlock(i)){
                    pagingIndex = i;
                    allocateMemoryVM(pagingIndex);
                    break;
                }
            }
            return pagingIndex;
        }

        public int newPaging(Word[] program){
            int pagingIndex = -1;
            Random rnd = new Random();
            for (int i = 0; i < 5; i++){
                int t = rnd.Next(0,16*16/2);
                if (isFreeBlock(t)){
                    allocateBlock(t);
                    pagingIndex = t;
                    allocateMemoryVM(pagingIndex);
                    break;
                }
            }
            /*for (int i = 0; i < 16*16; i++){
                if (isFreeBlock(i)){
                    allocateBlock(i);
                    pagingIndex = i;
                    allocateMemoryVM(pagingIndex);
                    break;
                }
            }*/
            if (pagingIndex >= 0){
                cpu.setPTR(new Word(new Byte[]{(byte)'0',(byte)'F',(byte)Word.convertInt(pagingIndex/16),(byte)Word.convertInt(pagingIndex%16)}));
                for (int i = 0; i < 16*16; i++){
                    if (program[i] != null)
                        write(pagingIndex,i,program[i]);
                }
            }
            else{
                Console.WriteLine("OUT OF SPACE!");
            }
            return pagingIndex;
        }

        public void printPaging(){
            for (int i = 0; i < 16*16; i++){
                Console.Write("{0,5}: ",i);
                if (memory[i] != null){
                    memory[i].print();
                }
                else {
                    Console.Write(" 0  0  0  0 | ");
                }
                if ((i+1)%10 == 0){
                    Console.Write("\n");
                }
            }
            Console.Write("\n");
        }

        public void printMemory(){
            Console.WriteLine("Real Machine Memory:");
            for (int i = 0; i < 16*16*16; i++){
                if (i%16 == 0)
                    Console.Write("{0,3}: ",i/16);
                if (memory[i] != null){
                    if (memory[i].isEqual(new Word())){
                        Console.Write("$$$$ ");
                    }
                    else{
                        memory[i].printC();
                        Console.Write(" ");
                    }
                }
                else {
                    Console.Write("$$$$ ");
                }
                if ((i+1)%16 == 0){
                    Console.Write("\n");
                }
            }
            Console.Write("\n");
        }

        public void printMemoryVM(){
            Console.WriteLine("Virtual Machine Memory:");
            for (int i = 0; i < 16*16; i++){
                if (read(i) != null){
                    if(read(i).isEqual(new Word())){
                        Console.Write("$$$$ ");
                    }
                    else{
                        read(i).printC();
                        Console.Write(" ");                        
                    }
                }
                else{
                    Console.Write("$$$$ ");
                }
                if ((i+1)%16 == 0){
                    Console.Write("\n");
                }
            }
            Console.Write("\n");
        }

        public RMmemory(int maxBlockSize, CPU cpu){
            this.maxBlockSize = maxBlockSize;
            this.cpu = cpu;
            memory = new Word[maxBlockSize*16];
        }
    }

    class InputDevice{
        public string getInputStr(){
            return Console.ReadLine();
        }

        public int getInputInt(){
            try{
                return  Convert.ToInt32(Console.ReadLine());
            }catch(Exception e){
                Console.WriteLine(e);
                return -1;
            }
        }
    }

    class OutputDevice{
        public void print(int value){
            Console.WriteLine(value);
        }

        public void print(string text){
            Console.WriteLine(text);
        }
    }

    class HardDrive{
        private Dictionary<int, StreamReader> filesRead;
        private Dictionary<int, StreamWriter> filesWrite;

        public bool isEndOfRead(int sr){
            StreamReader src;
            if (filesRead.TryGetValue(sr, out src)){
                return src.EndOfStream;
            }
            return true;
        }

        public bool writeInt(int sr, int value){
            StreamWriter src;
            if (filesWrite.TryGetValue(sr, out src)){
                src.WriteLine(value);
                return true;
            }
            return false;
        }

        public bool writeString(int sr, string value){
            StreamWriter src;
            if (filesWrite.TryGetValue(sr, out src)){
                src.WriteLine(value);
                return true;
            }
            return false;
        }

        public int readInt(int sr){
            StreamReader src;
            if (filesRead.TryGetValue(sr, out src)){
                string text = src.ReadLine();
                return Convert.ToInt32(text);
            }
            return -1;
        }

        public string readString(int sr){
            StreamReader src;
            if (filesRead.TryGetValue(sr, out src)){
                string t;
                if (!src.EndOfStream)
                    t = src.ReadLine();
                else
                    t = "";
                return t;
            }
            return null;
        }

        public int openReadFile(string path){
            int t = filesRead.Count+11;
            if (!File.Exists(path)){
                return -1;
            }
            else{
                filesRead.Add( t, new StreamReader(path));
            }
            return t;
        }

        public bool closeReadFile(int sr){
            StreamReader src;
            if (!filesRead.TryGetValue(sr, out src)){
                return false;
            }
            else{
                src.Close();
                filesRead.Remove(sr);
            }
            return true;
        }

        public int openWriteFile(string path){
            int t = filesWrite.Count+20;
            filesWrite.Add( t, new StreamWriter(path));
            return t;
        }

        public bool closeWriteFile(int sr){
            StreamWriter src;
            if (!filesWrite.TryGetValue(sr, out src)){
                return false;
            }
            else{
                src.Close();
                filesWrite.Remove(sr);
            }
            return true;
        }

        public HardDrive(){
            filesRead = new Dictionary<int, StreamReader>();
            filesWrite = new Dictionary<int, StreamWriter>();
        }
    }
    
    class CPU{
        public RMmemory memory;
        public readonly InputDevice input;
        public readonly OutputDevice output;
        public readonly HardDrive hardDrive;
        //Registers
        private int MODE; //user - 0; supervisor - 1
        private bool C;
        private int IC;
        private Word PTR;
        private int RA, RB;
        private int TI;
        private int PI;
        private int SI;
        private int CHST1, CHST2, CHST3;
        private bool debug;
        
        public const int TIME = 1000;

        public void resetTimer(){
            TI = TIME;
        }

        public void resetInterruptions(){
            TI = TIME;
            PI = 0;
            SI = 0;
        }

        public void resetVM(){
            IC = RA = RB = PI = SI = CHST1 = CHST2 = CHST3 = 0;
            C = false;
            TI = TIME;
        }

        public void printReg(){
            Console.WriteLine("Real machine Registers:");
            Console.WriteLine("\tMODE: {0}",MODE);
            Console.WriteLine("\tC: {0}",C);
            Console.WriteLine("\tIC: {0}",IC);
            Console.Write("\tPTR: ");PTR.printC();Console.Write("\n");
            Console.WriteLine("\tRA: {0}",RA);
            Console.WriteLine("\tRB: {0}",RB);
            Console.WriteLine("\tTI: {0}",TI);
            Console.WriteLine("\tPI: {0}",PI);
            Console.WriteLine("\tSI: {0}",SI);
            Console.WriteLine("\tCHST1: {0}",CHST1);
            Console.WriteLine("\tCHST2: {0}",CHST2);
            Console.WriteLine("\tCHST3: {0}",CHST3);
        }

        //GET&SET
        public int getMODE(){return MODE;}
        public void setMODE(int MODE){this.MODE = MODE;}

        public bool getC(){return C;}
        public void setC(bool C){this.C = C;}

        public int getIC(){return IC;}
        public void setIC(int IC){this.IC = IC;}

        public Word getPTR(){return PTR;}
        public void setPTR(Word PTR){this.PTR = PTR;}

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

        public void setDebug(bool value){this.debug = value;}
        public bool getDebug(){return debug;}

        //HLP

        private static readonly Dictionary<string, CommandsType> commandsDictionary = new Dictionary<string, CommandsType>{
            {"HALT", CommandsType.HALT},
            {"AD", CommandsType.ADxy},
            {"ADD_", CommandsType.ADD},
            {"SB", CommandsType.SBxy},
            {"SUB_", CommandsType.SUB},
            {"MU", CommandsType.MUxy},
            {"MUL_", CommandsType.MUL},
            {"CBN_", CommandsType.CBN},
            {"CEN_", CommandsType.CEN},
            {"LR", CommandsType.LRxy},
            {"LB", CommandsType.LBxy},
            {"SR", CommandsType.SRxy},
            {"SS", CommandsType.SSxy},
            {"GD", CommandsType.GDxy},
            {"PD", CommandsType.PDxy},
            {"JP", CommandsType.JPxy},
            {"JC", CommandsType.JCxy},
            {"FO", CommandsType.FOxy},
            {"FN", CommandsType.FNxy},
            {"FRCL", CommandsType.FRCL},
            {"FNCL", CommandsType.FNCL},
            {"FR", CommandsType.FRxy},
            {"FREA", CommandsType.FREA},
            {"FP", CommandsType.FPxy},
            {"FPUT", CommandsType.FPUT},
            {"PA__", CommandsType.PA},
            {"SAI_", CommandsType.SAI},
            {"SI", CommandsType.SIxy},
            {"PI", CommandsType.PIxy},
            {"FEND", CommandsType.FEND}
        };

        private static CommandsType interpretate(Word word){
            string s = Word.toString(word);
            CommandsType value;
            if (commandsDictionary.TryGetValue(s, out value)){
                return value;
            }
            else if (commandsDictionary.TryGetValue(s.Substring(0,2),out value)){
                return value;
            }
            else{
                Console.WriteLine("Command \""+s+"\" not implemented!");
            }
            return CommandsType.NONE;
        }

        public void writeString(string text){
            int countS = 0;
            int countB = 0;
            Word[] tarray = new Word[16];
            if (text.Length > 4*15){
                PI = 3;
            }
            else{
                for (int i = 1; i < 16; i+=1){
                    countS++;
                    tarray[i] = new Word();
                    for (int o = 0; o < 4; o++){
                        if (countB < text.Length){
                            tarray[i].setByte(o,(byte)text[countB]);
                            countB++;
                        }
                        else{
                            tarray[i].setByte(o,(byte)'$');
                        }
                    }
                    if (countB >= text.Length)
                        break;
                }
                tarray[0] = new Word(countS);
                for (int i = 0; i <= countS; i++)
                    memory.write((convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3)))+i, tarray[i]);
            }
        }

        public bool update(VirtualMachine vm){
            //TI PI SI
            bool running = true;
            bool g = false;
            MODE = 1;
            if (SI > 0){
                g = true;
                if (debug){
                    IC--;
                    vm.print();
                    Console.ReadKey();
                    Console.Clear();
                    IC++;
                }
                if (SI == 1){//GDxyVeikia
                    int countS = 0;
                    int countB = 0;
                    Word[] tarray = new Word[16];
                    string text = input.getInputStr();
                    if (text.Length > 4*15){
                        PI = 3;
                    }
                    else{
                        for (int i = 1; i < 16; i+=1){
                            countS++;
                            tarray[i] = new Word();
                            for (int o = 0; o < 4; o++){
                                if (countB < text.Length){
                                    tarray[i].setByte(o,(byte)text[countB]);
                                    countB++;
                                }
                                else{
                                    tarray[i].setByte(o,(byte)'$');
                                }
                            }
                            if (countB >= text.Length)
                                break;
                        }
                        tarray[0] = new Word(countS);
                        for (int i = 0; i <= countS; i++)
                            memory.write((convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3)))+i, tarray[i]);
                    }
                }
                else if (SI == 2){//PDxy VEIKIA
                    int size = Word.toInt(memory.read( (convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3))) ));
                    string ttext = "";
                    for (int i = 1; i <= size; i++){
                        ttext += Word.toString(memory.read( ((convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3))))+i ));
                    }
                    output.print(ttext.Replace("$",""));
                }
                else if (SI == 3){//FRxy
                    string t = hardDrive.readString(RA);
                    if(t!=null){
                        writeString(t);
                    }
                    else{
                        PI = 8;
                    }
                }
                else if (SI == 4){//FPxy
                    string t = memory.readString( memory.read(IC-1).getByte(2)*16 + (convertChar(memory.read(IC-1).getByte(3))) );
                    if (!hardDrive.writeString(RA,t)){
                        PI = 9;
                    }
                }
                else if (SI == 5){//HALT
                    running = false;
                }
                else if (SI == 6){//PA__
                    output.print(RA);
                }
                else if (SI == 7){//SAI
                    int t = input.getInputInt();
                    if (t < -1 || t > 16*16*16*16)
                        PI = 3;
                    else
                        RA = t;
                }
                else if (SI == 8){//PIxy
                    output.print(Word.toInt(memory.read( ((convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3)))))));
                }
                else if (SI == 9){//SIxy
                    memory.write( (convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3))) , input.getInputInt());
                }
                else if (SI == 10){//FOxy
                    int t = hardDrive.openReadFile(memory.readString((convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3)))));
                    if (t >= 0){
                        RA = t;
                    }
                    else{
                        PI = 5;
                    }
                }
                else if (SI == 11){//FNxy
                    int t = hardDrive.openWriteFile(memory.readString((convertChar(memory.read(IC-1).getByte(2)))*16 + (convertChar(memory.read(IC-1).getByte(3)))));
                    if (t >= 0){
                        RA = t;
                    }
                    else{
                        PI = 6;
                    }
                }
                else if (SI == 12){//FRCL
                    if (!hardDrive.closeReadFile(RA)){
                        PI = 7;
                    }
                }
                else if (SI == 13){//FNCL
                    if (!hardDrive.closeWriteFile(RA)){
                        PI = 7;
                    }
                }
                else if (SI == 14){//FREA
                    int t = hardDrive.readInt(RA);
                    if(t >= 0){
                        if (t > 16*16*16*16)
                            PI = 4;
                        else{
                            Console.WriteLine("TO RB");
                            RB = t;
                        }
                    }
                    else{
                        PI = 8;
                    }
                }
                else if (SI == 15){//FPUT
                    if (!hardDrive.writeInt(RA,RB)){
                        PI = 9;
                    }
                }
                else if (SI == 16){//FEND
                    C = hardDrive.isEndOfRead((int)RA);
                }
                SI = 0;
            }
            if (PI > 0){
                g = true;
                if (PI == 1){
                    Console.WriteLine("ERROR! Wrong adress!");
                }
                else if (PI == 2){
                    Console.WriteLine("ERROR! Wrong operation code!");
                }
                else if (PI == 3){
                    Console.WriteLine("ERROR! Wrong assignment!");
                }
                else if (PI == 4){
                    Console.WriteLine("ERROR! Overflow!");
                }
                else if (PI == 5){
                    Console.WriteLine("ERROR! File not exsists or can not be opened!");
                }
                else if (PI == 6){
                    Console.WriteLine("ERROR! Failed to create file!");
                }
                else if (PI == 7){
                    Console.WriteLine("ERROR! Failed to close file!");
                }
                else if (PI == 8){
                    Console.WriteLine("ERROR! Failed to read from file!");
                }
                else if (PI == 9){
                    Console.WriteLine("ERROR! Failed to write to file!");
                }
                else{
                    Console.WriteLine("ERROR! Undefined programming interruption!");
                }
                running = false;
            }
            if (TI == 0){
                g = true;
                Console.WriteLine("ERROR! Timer interruption!");
                running = false;
            }
            MODE = 0;
            if (g == true){
                if (debug){
                    IC--;
                    vm.print();
                    Console.ReadKey();
                    Console.Clear();
                    IC++;
                }
            }
            return running;
        }

        public int convertChar(byte t){
            if(t-65 >=0){
                return 10+(t-65);
            }
            return t-48;
        }

        public byte convertInt(int t){
            if(t>=10){
                return (byte)(65+t%10);
            }
            return (byte)(48+t);
        }

        public void execute(Word word){
            switch(interpretate(word)){
                //END
                case CommandsType.HALT:
                    SI = 5;
                    break;
                //Arithmetics
                case CommandsType.ADxy:
                    RA = RA + Word.toInt(memory.read( (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3))) ));
                    if (RA >= 16*16*16*16 || RA < 0)
                        PI = 4;
                    break;
                case CommandsType.ADD:
                    RA = RA + RB;
                    if (RA >= 16*16*16*16 || RA < 0)
                        PI = 4;
                    break;
                case CommandsType.SBxy:
                    RA = RA - Word.toInt(memory.read( (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3))) ));
                    if (RA >= 16*16*16*16 || RA < 0)
                        PI = 4;
                    break;
                case CommandsType.SUB:
                    RA = RA - RB;
                    if (RA >= 16*16*16*16 || RA < 0)
                        PI = 4;
                    break;
                case CommandsType.MUxy:
                    RA = RA * Word.toInt(memory.read( (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3))) ));
                    if (RA >= 16*16*16*16 || RA < 0)
                        PI = 4;
                    break;
                case CommandsType.MUL:
                    RA = RA * RB;
                    if (RA >= 16*16*16*16 || RA < 0)
                        PI = 4;
                    break;
                case CommandsType.CBN:
                    C = RA > RB;
                    break;
                case CommandsType.CEN:
                    C = (RA == RB);
                    break;
                    //Work w/ memory
                case CommandsType.LRxy:
                    RA = Word.toInt(memory.read( (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3))) ));
                    break;
                case CommandsType.LBxy:
                    RB = Word.toInt(memory.read( (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3))) ));
                    break;
                case CommandsType.SRxy:
                    memory.write( (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3))) , RA);
                    break;
                case CommandsType.SSxy:
                    memory.write( (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3))) , RB);
                    break;
                    //I/O
                case CommandsType.GDxy:
                    SI = 1;
                    break;
                case CommandsType.PDxy:
                    SI = 2;
                    break;
                case CommandsType.PIxy:
                    SI = 8;
                    break;//print Integer from adress
                case CommandsType.PA:
                    SI = 6;
                    break;//print Integer from register RA
                case CommandsType.SAI:
                    SI = 7;
                    break;//save Integer from input to register RA
                case CommandsType.SIxy:
                    SI = 9;
                    break;//save Integer from input to adress xy
                    //Jumps
                case CommandsType.JPxy:
                    IC = (convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3)))-1;
                    break;
                case CommandsType.JCxy:
                    IC = C?(convertChar(word.getByte(2)))*16 + (convertChar(word.getByte(3)))-1:IC;
                    break;
                    //Work w/ files
                case CommandsType.FOxy:
                    SI = 10;
                    break;
                case CommandsType.FNxy:
                    SI = 11;
                    break;
                case CommandsType.FRCL:
                    SI = 12;
                    break;
                case CommandsType.FNCL:
                    SI = 13;
                    break;
                case CommandsType.FRxy:
                    SI = 3;
                    break;
                case CommandsType.FREA:
                    SI = 14;
                    break;
                case CommandsType.FPxy:
                    SI = 4;
                    break;
                case CommandsType.FPUT:
                    SI = 15;
                    break;
                case CommandsType.FEND:
                    SI = 16;
                    break;
                default:
                    Console.WriteLine("Not implemented!"+IC);
                    PI = 1;
                    word.print();
                    break;
            }
            IC++;
            TI--;
        }

        public RMmemory getMemory(){
            return memory;
        }

        public CPU(){
            MODE = 1;
            IC = RA = RB = PI = SI = CHST1 = CHST2 = CHST3 = 0;
            PTR = new Word(new Byte[]{0,0,0,0});
            C = false;
            TI = TIME;
            memory = new RMmemory(16*16, this);
            debug = false;
            input = new InputDevice();
            output = new OutputDevice();
            hardDrive = new HardDrive();
        }
    }

    class RealMachine{
        private CPU cpu;
        private bool exit;

        public void run(){
            Console.WriteLine("0.0.1V use with caution...");
            while(!exit){
                Console.Write("RMCMD>");
                string[] t = (Console.ReadLine()).Split(' ');
                if (File.Exists(t[0])){
                    if (t.Length > 1)
                        if (t[1] == "d"){
                            cpu.setDebug(true);
                        }
                        else{
                            cpu.setDebug(false);
                        }
                    else{
                        cpu.setDebug(false);
                    }
                    cpu.resetVM();
                    ProgramReader tmp = new ProgramReader(t[0],16*16);
                    cpu.getMemory().newPaging(tmp.getMemory());
                    VirtualMachine vm = new VirtualMachine(cpu,cpu.getDebug());
                    vm.run();
                }
                else if (t[0] == "exit"){
                    exit = true;
                }
                else if (t[0] == "cls" || t[0] == "clear"){
                    Console.Clear();
                }
                else if (t[0] == "pm"){
                    cpu.memory.printMemory();
                }
                else if (t[0] == "alloc" && t.Length > 1){
                    cpu.memory.allocateBlock(Convert.ToInt32(t[1]));
                }
                else{
                    Console.WriteLine("File/Command \""+t[0]+"\" not found!");
                }
            }
        }

        public RealMachine(){
            cpu = new CPU();
            exit = false;
        }
    }
}