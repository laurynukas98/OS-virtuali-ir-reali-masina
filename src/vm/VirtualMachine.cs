using System;
using realMachine;

namespace VM{

    class VMmemory : Memory{
        private readonly RMmemory memory;

        public new Word read(int index){
            return memory.read(index);
        }

        public new void write(int index, int info){
            memory.write(index, info);
        }

        public new void write(int index, Word info){
            memory.write(index, info);
        }

        public void print(){
            memory.printMemory();
            memory.printMemoryVM();
        }

        public VMmemory(RMmemory memory){
            this.memory = memory;
        }
    }

    class VirtualMachine{
        private Boolean C;
        private int IC;
        private int RA;
        private int RB;
        private VMmemory memory;
        private readonly CPU cpu;
        private bool running;
        private bool debug = false;
        
        public void update(){
            running = cpu.update();
            C = cpu.getC();
            IC = cpu.getIC();
            RA = cpu.getRA();
            RB = cpu.getRB();
        }

        public void run(){
            while (running){
                if (debug == true){
                    print();
                    Console.ReadKey();
                    Console.Clear();
                }
                cpu.execute(memory.read(IC));
                update();
            }
        }

        public void print(){
            memory.print();
            cpu.printReg();
            Console.WriteLine("Virtual Machine Registers:");
            Console.WriteLine("\tC: {0};",C);
            Console.WriteLine("\tIC: {0};",IC);
            Console.WriteLine("\tRA: {0};",RA);
            Console.WriteLine("\tRB: {0};",RB);
            Console.Write("Executing: ");memory.read(IC).printC();Console.Write("\n");
        }

        public VirtualMachine(CPU cpu, bool debug){
            memory = new VMmemory(cpu.getMemory());
            this.cpu = cpu;
            update();
            running = true;
            this.debug = debug;
        }
    }
}