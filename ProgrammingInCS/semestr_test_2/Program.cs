using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics.Contracts;

namespace semestrProject;

class Reader{
    public Dictionary<string, StreamReader>? Many_sr;
    public StreamReader? One_sr;

    public Reader(string[] files){
        if (files == null){ 
            throw new NullReferenceException("files are null");
        }
        Many_sr = new Dictionary<string, StreamReader>();
        foreach (string file in files){
            try{
                Many_sr.Add(file, new StreamReader(file));
            } catch (FileNotFoundException){
                Console.WriteLine($"File {file} not found");
                return;
            }
        }
    }

    public Reader(StreamReader sr){
        One_sr = sr;
    }

    public void CloseAllFiles(){
    }
}

class Writer{
    private StreamWriter? sw;

    public Writer(string file){
        try{
            sw = new StreamWriter(file);
        } catch (FileNotFoundException){
            Console.WriteLine($"File {file} not found");
            return;
        }
    }

    public void Write(string str){
        if (sw == null){
            throw new NullReferenceException("file is null");
        }
        sw.Write(str);
    }

    public void CloseFile(){
        if (sw == null){
            throw new NullReferenceException("file is null");
        }
        sw.Close();
    }
}

class Program{
    static void Main(string[] args){
        
    }
}