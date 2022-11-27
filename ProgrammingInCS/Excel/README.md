# Excel

The goal is write a simple Excel-style sheet evaluator. The application will receive a text file containing a simplified excel sheet, evaluate all expressions in the cells, and then save the resulting table in an output file. 

The names of the input and output files are provided as command-line argument. In case of invalid number of command-line arguments, the program should print the following string to standard output: "Argument Error". In case of problems encountered when opening (the file does not exist, invalid filename, insufficient access rights, etc.) or reading the files, the program should output the following string: "File Error".

---

- A single row of the table is stored on a single line. The cell data are separated by one or more spaces, with no spaces being present in the cell data values. 
- The number of rows and columns is not known in advance and each row can contain a different number of used cells (columns). 
- Rows are numbered from 1 to infinity (it is guaranteed that the row number fits within a 32-bit integer)
- Columns are denoted with large letters of English alphabet in the same way as in the actual Excel (e.g. A, B, ... Z, AA, AB, ... AZ, BA, BB, ... ZZ, AAA, ...).
- The output file must be a copy of the input table (without any structural changes, cells in the output file are separated with a single space), with the formulas being replaced by the result of their evaluation

**Obsahem jednotlivých buněk může být:**

- Empty square brackets [] represent an **empty cell**. When present in the input, such explicitly denoted empty cells must also be present in the same form in the output (unlike other empty cells). - all empty cells are understood as having the value 0
- A non-negative integer **value**, guaranteed to fit within a 32-bit signed integer type (note that negative values can be encountered when evaluating expressions, so the output file may have to contain negative values as well).
- A **formula** starting with the = character (see below). 
- **#INVVAL** If a cell contains invalid data (e.g. an arbitrary string), its content will be replaced by the following string

Formula cells start with the = character, followed by an infix description of a standard binary operation (+, -, *, or /), with the operands being references to two other cells. A reference is written in the format COLUMNrow. For example, the following cell is a valid formula description: =A1+BC2. Both operands must be cell references (i.e. it is not possible to add a cell and a numeric constant). All operations should be performed on integers, also note that a cell referred from a formula may contain a formula as well (i.e. there is a necessary inherent order to cell evaluation).

If an error is encountered when evaluating a cell, the program must not crash, but must instead log the error by storing one of the following strings as the result of the cell evaluation:

- #ERROR — unable to evaluate the cell value; at least one operand is invalid or impossible to calculate
- #DIV0 — encountered division by zero when evaluating the cell value
- #CYCLE — a cycle was encountered (all cells that are part of the cycle must be set to this value). The result of evaluation of cells that are not part of the cycle but depend on the cycle cells will be set to #ERROR (if the formula in the cell is otherwise valid). It is guaranteed that the input table will not contain any nested or otherwise tangled cycles.
- #MISSOP — an operator is missing in the formula
- #FORMULA — at least one of the operands contains syntax errors, or the formula contains an error other than #MISSOP

When implementing the application, expect that you have O(N) memory available, with N corresponding to the length of the input file. Your solution should also be as fast as possible, i.e. evaluating a formula more than once must be avoided.

---
**Example:**
```
$>program.exe sample.sheet sample.eval


**sample.sheet**
    [] 3 =B1*A2
    19 =C1+C2 42
    car
    =B2/A1 =A1-B4 =C2+A4
    =error =A1+bus


**sample.eval**
    [] 3 57
    19 99 42
    #INVVAL
    #DIV0 #CYCLE #ERROR
    #MISSOP #FORMULA
```