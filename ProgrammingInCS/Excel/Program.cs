using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Excel
{
    #nullable enable
    struct Errors{
        public static string invalidData = "#INVVAL";
        public static string missOP = "#MISSOP";
        public static string formulaErr = "#FORMULA";
        public static string calculateErr = "#ERROR";
        public static string divByZero = "#DIV0";
        public static string cycle = "#CYCLE";
    }

    class Cell{
        public int? value = null;
        public Formula? formula = null;
        public string? err = null;
        public bool empty = false;

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
                else this.err = Errors.invalidData;
            }
            else if (str.Equals("[]")){
                this.value = 0;
                this.empty = true;
            }
            else if (!str[0].Equals('=')){
                this.err = Errors.invalidData;
            }
            else{
                str = str.Substring(1);
                char? op = OperatorFrom(str);
                if (op == null) this.err = Errors.missOP;
                else{
                    string[] coords = str.Split((char)op);

                    if (coords.Length != 2) this.err = Errors.formulaErr;
                    else{
                        string[] cols = new string[2]{"",""};
                        int[] rows = new int[2]{0,0};

                        for(int k = 0; k<2; k++){
                            // is Letter and there is min 2 symbol (letter and number)
                            if (!(coords[k][0] >= 65 && coords[k][0] <= 90) || coords[k].Length < 2){
                                this.err = Errors.formulaErr;
                                return;
                            }

                            cols[k] += coords[k][0];

                            int i  = 1;
                            while (coords[k][i] >= 65 && coords[k][i] <= 90){
                                if (!(coords[k][i] >= 65 && coords[k][i] <= 90)) break;
                                if (i == coords[k].Length-1){
                                    this.err = Errors.formulaErr;
                                    return;
                                }
                                cols[k] += coords[k][i];
                                i+=1;
                            }

                            // BB\
                            if (!int.TryParse(coords[k].Substring(i), out rows[k])){
                                this.err = Errors.formulaErr;
                                return;
                            }
                            // BB-1
                            else if (rows[k] < 0){
                                this.err = Errors.formulaErr;
                                return;
                            }
                        }
                        this.formula = new Formula( (char)op, cols, rows);
                    }
                }
            }
            return;
        }
    }

    class Formula{
        public char op;
        public int[] firstColRowInt = new int[2];
        public int[] secondColRowInt = new int[2];

        public Formula(char op, string[] cols, int[] rows){
            this.op = op;
            this.firstColRowInt[0] = ParseToInt(cols[0]);
            this.secondColRowInt[0] = ParseToInt(cols[1]);
            this.firstColRowInt[1] = rows[0];
            this.secondColRowInt[1] = rows[1];
        }

        private int ParseToInt(string col){
            int value = 0;
            for(int i=0; i<col.Length; i++){
                value += (col[i] - 65+1) * (int)Math.Pow(25, col.Length-i-1);
            }
            return value;
        }

        public int GetResult(int first, int second){
            int result = 0;
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

    class Sheet{
        private List<Cell[]> sheet = new();

        public Cell GetCell(int coll, int row){
            try{
                return this.sheet[row-1][coll-1];
            }
            catch{
                return new Cell("0");
            }
        }

        public void AddRowOfCell(Cell[] row){
            this.sheet.Add(row);
        }

        private void ProcessingCycle(List<Cell> cells){
            foreach (Cell c in cells){
                c.err = Errors.cycle;
                c.formula = null;
            }
        }
        
        private int? ProcessingFormula(List<Cell> cells){
            int? result = null;

            int leftColl = cells.Last().formula.firstColRowInt[0];
            int leftRow = cells.Last().formula.firstColRowInt[1];
            int rightColl = cells.Last().formula.secondColRowInt[0];
            int rightRow = cells.Last().formula.secondColRowInt[1];

            Cell[] refCells = new Cell[]{
                GetCell(leftColl, leftRow),
                GetCell(rightColl, rightRow)
            };

            int?[] values = new int?[2];

            for (int i = 0; i<2; i++){
                if (cells.Contains(refCells[i])){
                    int ind = cells.IndexOf(refCells[i]);
                    for (int j = ind; j < cells.Count(); j++){
                        cells[j].err = Errors.cycle;
                    }
                    refCells[i].err = Errors.cycle;
                }
                else if (refCells[i].value != null) 
                    values[i] = refCells[i].value;
                else if(refCells[i].err != null){
                    cells.Last().err = Errors.calculateErr;
                }
                else{
                    cells.Add(refCells[i]); 
                    values[i] = ProcessingFormula(cells);
                    cells.Remove(refCells[i]); 
                    if (refCells[i].err != null){
                        if (refCells[i].err == Errors.cycle){
                            if (cells.Last().err == null)
                                cells.Last().err = Errors.formulaErr;
                        }
                        else{
                            cells.Last().err = Errors.formulaErr;
                        }
                    }
                }
            }

            if (values[0] != null && values[1] != null){
                if (values[1] == 0 && cells.Last().formula.op == '/')
                    cells.Last().err = Errors.divByZero;
                else
                    result = cells.Last().formula.GetResult((int)values[0], (int)values[1]);
            }

            cells.Last().formula = null;
            cells.Last().value = result;
            return result;
        }

        public void Evaluating(){
            foreach (Cell[] row in sheet){
                foreach (Cell cell in row){
                    if (cell.formula != null){
                        cell.value = ProcessingFormula(new List<Cell>(){cell});    
                    }
                }
            }
        }

        public void Write(TextWriter wr){
            foreach (Cell[] row in this.sheet){
                if (row.Length != 0){
                    foreach (Cell cell in row){
                        if (cell.err != null) wr.Write(cell.err + " ");
                        else if (cell.empty == true) wr.Write("[] ");
                        else wr.Write(cell.value + " ");
                    }
                    wr.WriteLine();
                }
            }
        }
    }

    class Reader{
        internal static Sheet ReadSheet(TextReader r){
            Sheet sheet = new Sheet();
            string? line;
            Cell[] row;
            while ((line = r.ReadLine()) != null){
                if (line == null) break;
                row = ParseToCells(line);
                sheet.AddRowOfCell(row);
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
        public static void Main(string[] args)
        {
            // args = new string[]{"sample.sheet", "result.sheet"};
            if (args.Length != 2){
                Console.Write("Argument Error");
                return;
            }

            TextReader? fin = null;
            TextWriter? fout = null;

            try{
                fin = new StreamReader(args[0]);
                fout = new StreamWriter(args[1]);
                Sheet sheet = Reader.ReadSheet(fin);
                sheet.Evaluating();
                sheet.Write(fout);
            }
            catch (FileNotFoundException){
                Console.WriteLine("File Error");   
            }
            finally{
                if (fin != null) fin.Close();
                if (fout != null) fout.Close();
            }
        }
    }
}
