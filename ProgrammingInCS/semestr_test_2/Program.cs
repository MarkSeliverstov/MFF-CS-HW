using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace semestrProject{
#nullable enable
class Reader{
    public StreamReader sr;

    public Reader(string fname){
        sr = new StreamReader(fname+".cse");
    }

    public void CloseFile(){
        sr.Close();
    }
}

class Writer{
    private StreamWriter sw;

    public Writer(string fname){
        this.sw = new StreamWriter(fname);
    }

    public void WriteLine(string str){
        sw.WriteLine(str);
    }

    public void CloseFile(){
        sw.Close();
    }
}

struct DefineSymbols{
    public static List<string> symb = new List<string>();
}

class Preprocessor{
    string fname;
    StreamReader? sr;
    int currLine = 1;
    Writer w;
    Stack<Tuple<Conditional, bool>> stack = new Stack<Tuple<Conditional, bool>>();

    public Preprocessor(string fname, Writer w){
        this.fname = fname;
        this.w = w;
        try{
            sr = new StreamReader(fname);
        }
        catch (FileNotFoundException){
            sr = null;
        }     
    }

    public void Start(){
        if (sr == null){
            throw new FileNotFoundException ("File not found");
        }

        string? line;
        while ((line=sr.ReadLine()) != null){
            ProcessLine(line);
            currLine++;
        }
        if (stack.Count() != 0){
            throw new Exception($"{fname}#{currLine}: Missing #endif");
        }
        sr.Close();
        return;
    }

    private void ProcessLine(string line){
        string[] fromLine = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (fromLine.Length == 0) return;
        switch (fromLine[0])
        {
            case "#define":
                if (Mode.GetMode())
                    DefineSymbols.symb.Add(fromLine[1]);
                break;
            case "#undef":
                if (Mode.GetMode()){
                    if (DefineSymbols.symb.Contains(fromLine[1]))
                        DefineSymbols.symb.Remove(fromLine[1]);
                }
                break;
            case "#include":
                if (Mode.GetMode()){
                    try{
                        Preprocessor preprocessor = new Preprocessor(fromLine[1], w);
                        preprocessor.Start();
                    }
                    catch (FileNotFoundException){
                        throw new FileNotFoundException($"{fname}#{currLine}: #include invalid file name '{fromLine[1]}'");
                    }
                }
                break;
            case "#if":
                if (Mode.GetMode()) {
                    stack.Push(new Tuple<Conditional, bool>(Conditional.c_if, true));
                    if (!DefineSymbols.symb.Contains(fromLine[1])){
                        Mode.deactive();
                    }
                }
                else stack.Push(new Tuple<Conditional, bool>(Conditional.c_if, false));
                break;
            case "#else":
                if (stack.Count() == 0){
                    throw new Exception($"{fname}#{currLine}: Standalone #else");
                }
                if (stack.Peek().Item1 == Conditional.c_if){
                    if (stack.Peek().Item2 == true)
                        Mode.Reverse();
                    //stack.Push(new Tuple<Conditional, bool>(Conditional.c_else, true));
                }
                else{
                    throw new Exception($"{fname}#{currLine}: Standalone #else");
                }
                break;
            case "#endif":
                if (stack.Count() == 0){
                    throw new Exception($"{fname}#{currLine}: Standalone #endif");
                }
                if (stack.Peek().Item1 == Conditional.c_if){
                    stack.Pop();
                    if (stack.Count() == 0){
                        Mode.activate();
                    }
                    else{
                        if (stack.Peek().Item2 == true){
                            Mode.activate();
                        }
                        else{
                            Mode.deactive();
                        }
                    }
                }
                else{
                    throw new Exception($" {fname}#{currLine}: Standalone #endif");
                }
                break;
            default:
                if (Mode.GetMode()){
                    w.WriteLine(line);
                }
                break;
        }   
    }
}

struct Mode{
    static bool active = true;
    public static void activate(){
        active = true;
    }
    public static void deactive(){
        active = false;
    }
    public static bool GetMode(){
        return active;
    }
    public static void Reverse(){
        active = !active;
    }
}

enum Conditional{
    c_if,
    c_else,
    c_endif
}

class Parser{
    public static string? IsCorrectExtension(string file){
        List<string> parseFile = file.Split('.').ToList();
        string extension = parseFile.Last();
        if (!extension.Equals("cse")) 
            return null;

        parseFile.RemoveAt(parseFile.Count()-1);
        string fname = String.Join("",parseFile.ToArray());
        if (fname == null || fname.Equals("")) 
            return null;
        else 
            return fname;
    }
}

class Program{
    static void Main(string[] args){
        if (args.Length != 1){
            Console.WriteLine("Missing argument");
            return;
        }
        string? fname;
        if ((fname = Parser.IsCorrectExtension(args[0])) != null ){
            try{
                Writer w = new Writer(fname + ".cs");
                Preprocessor preprocessor = new Preprocessor(fname+".cse", w);
                preprocessor.Start();
                w.CloseFile();
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
            }
        }
        else{
            Console.WriteLine("Unsupported file");
            return;
        }
    }
}
}