using System;
using System.IO;

class ProgramReader{
    private Word[] memory;
    private int maxSize;
    private string fileName;
    public bool pass;

    /* 
        $DATA //programos pradzia, uzema 1 zodi (soka i $START)
        DW 50 //uzema 1 zodi
        DB 8tes
        DB t.tx
        DB tnnn  //1 baitas nurodo string ilgi(max 15), toliau - kiekvienas simbolis, po 1 baita.
        $START //nuo 5 zodzio atmintyje
        LR 01 //i registra RA ideda reiksme 50 (taip pat uzema 1 zodi?)
        LB 01 //i registra RB ideda reiksme 50
        ADD //sudeda RA ir RB, ats: RA (uzema 1 zodi?)
        SR 01 //issaugo RA reiksme
        FO 02 //atidaro "test.txt" faila, deskriptorius RA
        LB 01 //registro RB reiksme - 100.
        FPUT //i faila iraso RB reiksme.
        FRCL //uzdaro failo deskriptoriu.
        $END //HALT
        //Toliau failas nera skaitomas
    */

    private bool read(){
        StreamReader sr = new StreamReader(fileName);
        int arrow = 0;
        bool reading = false;
        bool foundEnd = false;
        try{
        while (!sr.EndOfStream){
            string[] t = (sr.ReadLine()).Split(' ');
            if (t[0] == "$DATA" && reading != true){
                reading = true;
            }
            else if (t[0] == "$START" && reading){
                memory[0] = new Word(new byte[]{0x40,(byte)arrow,0,0});
                continue;
            }
            else if (t[0] == "$END" && reading){
                memory[arrow] = new Word(0);
                foundEnd = true;
                break;
            }
            else if ( t[0] == "DB" && reading){
                memory[arrow] = new Word(t[1]);
            }
            else if (t[0] == "DW" && reading){
                memory[arrow] = new Word(Int32.Parse(t[1]));
            }
            else if (reading){
                int number = (int)Enum.Parse(typeof(Commands), t[0]);
                memory[arrow] = new Word(new Byte[]{(byte)number,t.Length>1?(byte)Int32.Parse(t[1]):(byte)0,0,0});
            }
            if (reading)
                arrow++;
        }
        if (!foundEnd)
            pass = false;
        }catch(Exception e){
            Console.WriteLine(e);
        }
        return pass;
    }

    public Word[] getMemory(){
        return memory;
    }

    public void print(){
        for (int i = 0; i < maxSize; i++){
            if (memory[i]!=null)
                memory[i].print();
            else
                break;
        }
    }

    public ProgramReader(string fileName, int maxSize){
        this.fileName = fileName;
        this.maxSize = maxSize;
        memory = new Word[maxSize];
        pass = this.read();
        //this.print();
    }
}