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
        LR01 //i registra RA ideda reiksme 50 (taip pat uzema 1 zodi?)
        LB01 //i registra RB ideda reiksme 50
        ADD_ //sudeda RA ir RB, ats: RA (uzema 1 zodi?)
        SR01 //issaugo RA reiksme
        FO02 //atidaro "test.txt" faila, deskriptorius RA
        LB01 //registro RB reiksme - 100.
        FPUT //i faila iraso RB reiksme.
        FRCL //uzdaro failo deskriptoriu.
        $END //HALT
        //Toliau failas nera skaitomas
    */

    public byte convertInt(int t){
        if(t>=10){
            return (byte)(65+t%10);
        }
        return (byte)(48+t);
    }

    private bool read(){
        StreamReader sr = new StreamReader(fileName);
        int arrow = 0;
        bool reading = false;
        bool foundEnd = false;
        try{
            while (!sr.EndOfStream){
                string[] t = (sr.ReadLine()).Split(' ');
                //Console.WriteLine(t[0]);
                if (t[0] == "$DATA" && reading != true){
                    reading = true;
                }
                else if (t[0] == "$START" && reading){
                    memory[0] = new Word(new byte[]{Convert.ToByte('J'),Convert.ToByte('P'),convertInt((arrow/16)),convertInt((arrow-(arrow/16)*16))});
                    continue;
                }
                else if (t[0] == "$END" && reading){
                    memory[arrow] = new Word("HALT");
                    foundEnd = true;
                    break;
                }
                else if (t[0] == "DB" && reading){
                    memory[arrow] = new Word(t[1]);
                }
                else if (t[0] == "DW" && reading){
                    memory[arrow] = new Word(int.Parse(t[1]));
                }
                else if (reading){
                    memory[arrow] = new Word(new Byte[]{(byte)t[0][0],(byte)t[0][1],(byte)t[0][2],(byte)t[0][3]});
                }
                if (reading)
                    arrow++;
            }
            if (!foundEnd)
                pass = false;
        }catch(Exception e){
            Console.WriteLine(e);
        }
        sr.Close();
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
                new Word(0).print();
        }
    }

    public ProgramReader(string fileName, int maxSize){
        this.fileName = fileName;
        this.maxSize = 16*16;
        memory = new Word[maxSize];
        pass = this.read();
    }
}