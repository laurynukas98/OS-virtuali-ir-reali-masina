using System;
using vm;

class Run{

    public static void Main(string[] args){
        if (args.Length == 1 || args.Length == 2){
            VirtualMachine vMachine = new VirtualMachine(null,args[0]);
            vMachine.run(args.Length==2?true:false);
        }
        else{
            if (args.Length < 1){
                Console.WriteLine("Too few arguments (need 1 for filename and/or 2nd for debug)!");
            }
            else{
                Console.WriteLine("Too much arguments (need 1 for filename and/or 2nd for debug)!");
            }
        }
    }
}