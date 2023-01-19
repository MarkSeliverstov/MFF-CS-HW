using System;
using System.Collections.Generic;
using System.IO;

namespace semestr_test
{
    #nullable enable
    class Parser{
        public static string? ParseDelimiter(string delimiter){
            if (delimiter.Length < 3 || delimiter[0] != '"' || delimiter[delimiter.Length-1] != '"') 
                return null;
            return delimiter.Substring(1, delimiter.Length-2);
        }

        public static List<int> ParseFields(string fields){
            string[] fieldsStr = fields.Split(',', StringSplitOptions.RemoveEmptyEntries);
            List<int> fieldsInt = new List<int>();

            for (int i = 0; i < fieldsStr.Length; i++){
                if (fieldsStr[i].Contains('-')){
                    string[] fieldsRange = fieldsStr[i].Split('-');

                    if (fieldsRange.Length != 2) 
                        throw new ArgumentException("Field format error");

                    if (int.TryParse(fieldsRange[0], out int start) && int.TryParse(fieldsRange[1], out int end)){
                        if (start < end) {
                            for (int j = start; j <= end; j++){
                                fieldsInt.Add(j);
                            }
                        }
                        else{
                            for (int j = start; j >= end; j--){
                                fieldsInt.Add(j);
                            }
                        }
                    }
                    else    
                        throw new ArgumentException("Field format error");
                }
                else{
                    if (int.TryParse(fieldsStr[i], out int field)){
                        fieldsInt.Add(field);
                    }
                    else    
                        throw new ArgumentException("Field format error");
                }
            }
            return fieldsInt;
        }

        public static CutAndPaste? Parse(string[] input){
            List<ActionsWithFile>? actionsWithFiles = new List<ActionsWithFile>();
            string? fileIn = null;
            string delimiter = "\t";
            List<int>? fields = null;
            string? fileOut = null;
            string outputDelimiter = "\t";

            for (int i = 0; i < input.Length; i++){
                switch (input[i]){
                    case "-d":
                        try{
                            delimiter = input[i+1];
                        }
                        catch (ArgumentException){
                            return null;
                        }
                        i++;
                        break;
                    case "-f":
                        if (input.Length == i + 1) 
                            return null;
                        fields = ParseFields(input[i + 1]);
                        if (fields == null) 
                            return null;
                        i++;
                        break;
                    case "--od":
                        try{
                            outputDelimiter = input[i+1];
                        }
                        catch (ArgumentException){
                            return null;
                        }
                        i++;
                        break;
                    case "--out":
                        if (fileOut != null || input.Length == i + 1) 
                            return null;
                        fileOut = input[i + 1];
                        i++;
                        break;
                    default:
                        if (fileIn != null) 
                            return null;
                        fileIn = input[i];
                        ActionsWithFile actions = new ActionsWithFile(fileIn, delimiter, fields);
                        actionsWithFiles.Add(actions);
                        fileIn = null;
                        delimiter = "\t";
                        fields = null;
                        fileOut = null;
                        break;
                }
            }
            return new CutAndPaste(actionsWithFiles, fileOut, outputDelimiter);
        }
    }

    class CutAndPaste{
        public List<ActionsWithFile>? CutFiles = null;
        List<List<string>> result = new List<List<string>>();
        public string outputDelimiter = "\t";
        public string? PasteFile = null;

        public CutAndPaste(List<ActionsWithFile>? CutFiles, string? PasteFile, string outputDelimiter = "\t"){
            this.CutFiles = CutFiles;
            this.PasteFile = PasteFile;
            this.outputDelimiter = outputDelimiter;
        }

        public void CutFromFiles(){
            foreach (ActionsWithFile actions in CutFiles){
                List<List<string>> cut = Cut(actions);
                ConnectToResult(cut);
            };
        }

        public List<List<string>> Cut(ActionsWithFile actions){

            List<List<string>> cut = new List<List<string>>();

            using (TextReader fin = new StreamReader(actions.fileIn)){
                string? line = null;

                while ((line = fin.ReadLine()) != null){
                    string[] fields = line.Split(actions.delimiter);
                    List<string> lineToCut = new List<string>();

                    if (actions.fields == null){
                        for (int i = 0; i < fields.Length; i++){
                            lineToCut.Add(fields[i]);
                            lineToCut.Add(outputDelimiter);
                        }
                    }
                    else{
                        for (int i=0; i<actions.fields.Count; i++){
                            if (actions.fields[i] <= fields.Length){
                                lineToCut.Add(fields[actions.fields[i]-1]);
                                lineToCut.Add(outputDelimiter);
                            }
                        }
                    }
                    cut.Add(lineToCut);
                }
            }

            return cut;
        }

        public void ConnectToResult(List<List<string>> cut){
            for (int i = 0; i < cut.Count; i++){
                if (result.Count <= i){
                    result.Add(new List<string>());
                }
                for (int j = 0; j < cut[i].Count; j++){
                    result[i].Add(cut[i][j]);
                }
            }
        }

        public void PasteToFiles(TextWriter? fout){
            for (int i = 0; i < this.result.Count; i++){
                for (int j = 0; j < this.result[i].Count; j++){
                    fout.Write(this.result[i][j]);
                }
                fout.WriteLine();
            }
        }
    }

    class ActionsWithFile{
        public string? fileIn = null;
        public string? delimiter = null;
        public List<int>? fields = null;

        public ActionsWithFile( string? fileIn, string? delimiter, 
                                List<int>? fields){
            this.fileIn = fileIn;
            this.delimiter = delimiter;
            this.fields = fields;
        }
    }


    class Program{
        public static void Main(string[] args)
        {
            //string input = "--od \"|\" -d \" \" data/file.in.txt -f 1-3,6 -d \" \" data/file.in.txt --out file.out";
            // string input = "--od \"|\" -f 1-3,6,11-9 -d \" \" data/file.in.txt";
            TextReader? fin = null;
            TextWriter? fout = null;

            try{
                CutAndPaste? CNP = Parser.Parse(args);
                if (CNP == null){
                    Console.WriteLine("Format error");
                    return;
                }

                CNP.CutFromFiles();

                if (CNP.PasteFile != null)
                    fout = new StreamWriter(CNP.PasteFile);
                else
                    fout = Console.Out;

                CNP.PasteToFiles(fout);
            }
            catch (FileNotFoundException){
                Console.WriteLine("File not found");   
            }
            finally{
                if (fin != null) fin.Close();
                if (fout != null) fout.Close();
            }
        }
    }
}