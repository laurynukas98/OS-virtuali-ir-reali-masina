using System;

class Word{
    private const int SIZE = 4;
    private byte[] data;

    public byte getByte(int i){
        return data[i];
    }
    
    public void setByte(int i, byte b){
        this.data[i] = b;
    }
    
    public void setInt(int value){
        data = new byte[SIZE];
        data[0] = (byte)(value/Math.Pow(16,6));
        value -= (int)(data[0]*Math.Pow(16,6));
        data[1] = (byte)(value/Math.Pow(16,4));
        value -= (int)(data[1]*Math.Pow(16,4));
        data[2] = (byte)(value/Math.Pow(16,2));
        value -= (int)(data[1]*Math.Pow(16,2));
        data[3] = (byte)value;
    }

    public void setBytes(byte[] value){
        data = new byte[SIZE]{value[0],value[1],value[2],value[3]};
    }

    public static int toInt(Word word){
        return (int)(word.data[0]*Math.Pow(16,6)+word.data[1]*Math.Pow(16,4)+word.data[2]*Math.Pow(16,2)+word.data[3]);
    }

    public static Word toWord(int value){
        Word word = new Word(value);
        return word;
    }

    public void print(){
        Console.Write("{0:X} {1:X} {2:X} {3:X}\n", data[0], data[1], data[2], data[3]);
    }

    public Word(){
        data = new byte[SIZE];
    }

    public Word(Word word){
        this.data = word.data;
    }

    public Word(int value){
        setInt(value);
    }

    public Word(string value){
        data = new byte[SIZE];
        for (int i = 0; i < ((value.Length>3)?4:value.Length); i++)
            data[i] = (byte)value[i];
    }

    public Word(byte[] value){
        setBytes(value);
    }
}