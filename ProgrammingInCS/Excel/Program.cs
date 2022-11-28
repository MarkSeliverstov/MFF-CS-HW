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

        private char? OperatorFrom(string str){
            char[] operators = new char[]{'-', '+', '*', '/'};
            foreach (char op in operators){
                if (str.Contains(op)) return op;
            }
            return null;
        }

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
                str = str.Substring(1);
                char? op = OperatorFrom(str);
                if (op == null) 
                    this.err = missOP;
                else{
                    string[] coords = str.Split(Convert.ToChar(op));
                    if (coords.Length != 2) 
                        this.err = formulaErr;
                    else{
                        // A123 AA123 AA
                        string[] cols = new string[2]{"",""};
                        int[] rows = new int[2]{0,0};

                        for(int k = 0; k<2; k++){
                            if (!(coords[k][0] >= 65 && coords[k][0] <= 90) || 
                                coords[k].Length < 2){
                                this.err = formulaErr;
                                return;
                            }

                            cols[k] += coords[k][0];

                            int i  = 1;
                            while (coords[k][i] >= 65 && coords[k][i] <= 90){
                                if (!(coords[k][i] >= 65 && coords[k][i] <= 90)) break;
                                if (i == coords[k].Length){
                                    this.err = formulaErr;
                                    return;
                                }
                                cols[k] += coords[k][i];
                                i+=1;
                            }

                            if (!int.TryParse(coords[k].Substring(i), out rows[k])){
                                this.err = formulaErr;
                                return;
                            }
                        }

                        this.formula = new Formula( (char)op,
                                                    coords[0],
                                                    coords[1],
                                                    cols,
                                                    rows
                                                    );
                    }
                }
            }
            return;
        }
    }

    class Formula{
        public char op;
        public string firstColRow;
        public string secondColRow;
        public Int32[] firstColRowInt = new Int32[2];
        public Int32[] secondColRowInt = new Int32[2];

        public Formula(char op, string firstColRow, string secondColRow, string[] cols, int[] rows){
            this.op = op;
            this.firstColRow = firstColRow;
            this.secondColRow = secondColRow;
            this.firstColRowInt[0] = ParseToInt(cols[0]);
            this.secondColRowInt[0] = ParseToInt(cols[1]);
            this.firstColRowInt[1] = rows[0];
            this.secondColRowInt[1] = rows[1];
        }

        private Int32 ParseToInt(string col){
            Int32 value = 0;
            for(int i=0; i<col.Length; i++){
                value += (col[i] - 65+1) * (Int32)Math.Pow(25, col.Length-i-1);
            }
            return value;
        }

        public Int32 GetResult(Int32 first, Int32 second){
            Int32 result = 0;
            switch (this.op)
            {
                case '+':
                    result = first + second;
                    break;
                case '-':
                    result = first - second;
                    break;
                case '*':
                    result = first * second;
                    break;
                case '/':
                    result = first / second;
                    break;
            }
            return result;
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
                sheet.Add(row);
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

        private static Int32[] GetValues(Cell cell){

        }

        private static Int32 GetResult(Cell cell){

        }

        private static List<Cell[]> EvaluatingSheet(List<Cell[]> sheet){
            foreach (Cell[] row in sheet){
                foreach (Cell cell in row){
                    if (cell.formula != null){
                        cell.value = GetResult(cell); // TODO: process every cells
                    }
                }
            }
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