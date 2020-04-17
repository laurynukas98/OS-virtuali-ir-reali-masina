using System;
using realMachine;

namespace vm{

    class VMmemory : Memory{
        private Word[] memory;

        public new Word read(int index){
            return memory[index];
        }

        public new void write(int index, int info){
            memory[index] = new Word(info);
        }

        public new void write(int index, Word info){
            memory[index] = new Word(info);
        }

        public void print(){
            for (int i = 0; i < memory.Length; i++){
                if (memory[i]!=null){
                    Console.Write("{0}: ",i);
                    memory[i].print();
                }
            }
        }

        public VMmemory(int size){
            memory = new Word[size];
        }

        public VMmemory(Word[] memory){
            this.memory = memory;
        }
    }

    class VMoutput{

    }

    class VMinput{

    }

    class VirtualMachine{
        private Boolean C;
        private int IC;
        private int RA;
        private int RB;
        private VMmemory memory;
        private VMoutput output;
        private VMinput input;
        private readonly CPU cpu;
        private bool running;
        
        public void run(bool debug){
            while (running){
                if (debug == true){
                    Console.WriteLine("Executing: {0} {1:X}",Enum.GetName(typeof(Commands), (memory.read(IC)).getByte(0)), memory.read(IC).getByte(1));
                    print();
                    Console.ReadKey();
                    Console.Clear();
                }
                execute(memory.read(IC));
                IC++;
            }
        }

        public void execute(Word word){
            Byte b = word.getByte(0);
            switch (b){
                //END
                case (byte)Commands.HALT:
                    running = false;
                    break;
                //Arithmetics
                case (byte)Commands.AD:
                    RA = RA + Word.toInt(memory.read((int)(word.getByte(1))));
                    break;
                case (byte)Commands.ADD:
                    RA = RA + RB;
                    break;
                case (byte)Commands.SB:
                    RA = RA - Word.toInt(memory.read((int)(word.getByte(1))));
                    break;
                case (byte)Commands.SUB:
                    RA = RA - RB;
                    break;
                case (byte)Commands.MU:
                    RA = RA * Word.toInt(memory.read((int)(word.getByte(1))));
                    break;
                case (byte)Commands.MUL:
                    RA = RA * RB;
                    break;
                case (byte)Commands.CBN:
                    C = RA > RB;
                    break;
                case (byte)Commands.CEN:
                    C = RA == RB;
                    break;
                    //Work w/ memory
                case (byte)Commands.LR:
                    RA = Word.toInt(memory.read((int)word.getByte(1)));
                    break;
                case (byte)Commands.LB:
                    RB = Word.toInt(memory.read((int)word.getByte(1)));
                    break;
                case (byte)Commands.SR:
                    memory.write((int)word.getByte(1), RA);
                    //print();
                    break;
                case (byte)Commands.SS:
                    memory.write((int)word.getByte(1), RB);
                    break;
                    //I/O
                case (byte)Commands.GD:
                    Console.WriteLine("Read from virtual device is not implemented!");
                    break;
                case (byte)Commands.PD:
                    Console.WriteLine("Write from virtual device is not implemented!");
                    break;
                case (byte)Commands.PI:
                    Console.WriteLine(Word.toInt( memory.read((int)word.getByte(1))));
                    //Console.ReadKey();
                    break;//print Integer from adress
                case (byte)Commands.PA:
                    Console.WriteLine(RA);
                    //Console.ReadKey();
                    break;//print Integer from register RA
                case (byte)Commands.SAI:
                    RA = Int32.Parse(Console.ReadLine());
                    break;//save Integer from input to register RA
                case (byte)Commands.SXI:
                    memory.write((int)word.getByte(1), Convert.ToInt32(Console.ReadLine()));
                    break;//save Integer from input to adress xy
                    //Jumps
                case (byte)Commands.JP:
                    IC = (int)word.getByte(1)-1;
                    break;
                case (byte)Commands.JC:
                    IC = C?(int)word.getByte(1)-1:IC;
                    break;
                    //Work w/ files
                case (byte)Commands.FO:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                case (byte)Commands.FN:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                case (byte)Commands.FRCL:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                case (byte)Commands.FNCL:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                case (byte)Commands.FR:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                case (byte)Commands.FREA:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                case (byte)Commands.FP:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                case (byte)Commands.FPUT:
                    Console.WriteLine("Work with files not implemented!");
                    break;
                default:
                    Console.WriteLine("Not implemented!"+IC);
                    word.print();
                    break;
            }
        }

        public void print(){
            Console.WriteLine("VirtualMachine print:");
            Console.WriteLine("C: {0};",C);
            Console.WriteLine("IC: {0};",IC);
            Console.WriteLine("RA: {0};",RA);
            Console.WriteLine("RB: {0};",RB);
            Console.WriteLine("Virtual memory output:");
            memory.print();
        }

        public VirtualMachine(CPU cpu, string fileName){
            C = false;
            IC = 0;
            RA = 0;
            RB = 0;
            running = true;
            memory = new VMmemory((new ProgramReader(fileName, 16*16)).getMemory());
            this.cpu = cpu;
        }
    }
}