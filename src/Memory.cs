using System;

abstract class Memory{
    public Word read(int index){
        Console.WriteLine("Read function is not implemented. Class: "+this.GetType().Name);
        return null;
    }

    public void write(int index, int info){
        Console.WriteLine("Write function (int) is not implemented. Class: "+this.GetType().Name);
    }

        public void write(int index, Word info){
        Console.WriteLine("Write function (Word) is not implemented. Class: "+this.GetType().Name);
    }
}