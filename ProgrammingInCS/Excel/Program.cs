
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Excel
{
    class Cell{
        public Int32? value = null;
        public Formula? formula = null;
        public string? err = null;

        // types of errors
        static string invalidData = "#INVVAL";
        static string calculate = "#ERROR";
        static string divByZero = "#DIV0";
        static string cycle = "#CYCLE";
        static string missOP = "#MISSOP";
        static string formulaErr = "#FORMULA";

        public Cell(string str){
            int value;
            if (int.TryParse(str, out value)){
                if (value >= 0) this.value = value;
                else this.err = invalidData;
            }
            else if (str.Equals("[]")){
                this.value = 0;
            }
            else if (!str[0].Equals('=')){
                this.err = invalidData;
            }
            else{
                if(Regex.IsMatch(str, pattern)) this.formula = new Formula(str);
                else 
            }
            return;
        }
    }

    class Formula{
        string firstOp;
        string secondOp;
        char operation;
        int? result = null;
        public Formula(string str){

        }
        public void GetResult(){
            switch (this.operation)
            {
                case '+':
                    break;
                case '-':
                    break;
                case '*':
                    break;
                case '/':
                    break;
            }
            
        }
    }

    class Reader{
        internal static List<Cell[]> ReadSheet(TextReader r){
            List<Cell[]> sheet = new();
            string? line;
            Cell[] row;
            while ((line = r.ReadLine()) != null){
                if (line == null) break;
                row = ParseToCells(line);
            }
            return sheet;
        }

        private static Cell[] ParseToCells(string line){
            string[] row = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            Cell[] cellsRow = new Cell[row.Length];
            for (int i = 0; i < row.Length; i++){
                cellsRow[i] = new Cell(row[i]);
            }
            return cellsRow;
        }
    }

    class Program{

        private static void WriteSheet(List<Cell[]> sheet, TextWriter wr){

        }

        private static List<Cell[]> EvaluatingSheet(List<Cell[]> sheet){

            return sheet;
        }

        public static void Main(string[] args)
        {
            args = new string[]{"sample.sheet", "result.sheet"};
            // if (args.Length != 2){
            //     Console.Write("Argument Error");
            //     return;
            // }

            TextReader? fin = null;
            TextWriter? fout = null;
            

            try{
                fin = new StreamReader(args[0]);
                fout = new StreamWriter(args[1]);
                List<Cell[]> sheet = Reader.ReadSheet(fin);
                sheet = EvaluatingSheet(sheet);
                WriteSheet(sheet, fout);
            }
            catch (FileNotFoundException){
                Console.WriteLine("File Error");   
            }
            catch (IOException){
                Console.WriteLine("File Error");   
            }
            catch (UnauthorizedAccessException){
                Console.WriteLine("File Error");   
            }
            catch (System.Security.SecurityException){
                Console.WriteLine("File Error");   
            }
            catch (ArgumentException){
                Console.WriteLine("File Error");   
            }
            finally{
                if (fin != null) fin.Close();
                if (fout != null) fout.Close();
            }
        }
    }
}