# General Information

You are to complete the assignment within a time limit of 3 hours. The time starts after the assignment is well explained and all questions are answered. It is possible to ask additional questions during the test as well, but in such a form that won't disturb others in their work.
Your solution will be tested in the ReCodEx environment. It is necessary to fully comply with assignment details (i.e. the error messages). You are graded by the implementation of main concepts and it's functionality. The code you write is expected to be of a certain quality (i.e. code style and design). Implementation that only uses few methods without any decomposition of the problem is not valid.
You are allowed to use any documentation in both textual and electronic form, expect for existing solutions to this (or similar) assignment. It is not allowed to communicate with anyone for any reason.
Once your solution is finished, call an examiner to take a look at your submission.
It is necessary to pass all the tests in ReCodEx.

# Assignment Description

Your goal is to create a program that acts as extended preprocessor of the C# language. That means a tool, which reads an input file (C# source code), understands special preprocessor directives, beginning with a hash character (#) and produces output C# code to a different file.

If the program is run with different amount of command line arguments then 1, the program prints Missing argument to the standard output and exits. The only parameter on the command line is the name of the file, ending with .cse extension. If the name of the file is with different extension, the program prints out Unsupported file and exits. Otherwise, the command line argument is treated as a file name to be processed. The processed file will be stored in a file with the same name as the input file, but with the .cs extension. That means, that calling the program as Program.exe library.cse will produce a file library.cs. If the input file cannot be read for any reason, the program will exit. If the program already processed some of the input into an output file, it is allowed to leave the partial output on the disk, should any error happen.

The program works in two modes: passive and active, and switches between these modes based on the instructions it reads. Program processes the input file by lines. For every line, the program checks what is the first non-whitespace character, and if it is a hash (#), it performs an instruction based on the specification below and doesn't produce any output for this line. If the line is not a preprocessor directive and the program is in active mode, then the line is copied to the output. If the line is not a preprocessor directive and the program is in passive mode, the line is ignored.

# Preprocessor directives

Program supports the following instructions. For the purpose of error messages, assume the following conventions:

F character denotes a name of the file, in which the error happened. L character denotes a line number, at which the error happened.

`#define Symbol`

If the program is in active mode, then a symbol named Symbol is declared. If the symbol was already declared, or the program is in passive mode, the directive is ignored.

`#undef Symbol`

If the program is in active mode, then a declaration of a symbol named Symbol is forgotten. If the symbol was not declared, or the program is in passive mode, the directive is ignored.

`#include file`

If the program is in active mode, then a file with a name file is processed, according to these rules (except for checking for the right extension), and processed output is inserted to current location in the output file (can span across multiple lines). Processing of nested files shares the same symbol table. It is therefore possible to declare new or forget old symbols while processing nested file. If the program is in passive mode, the directive is ignored.

If the file file doesn't exist, or can't be read for any reason, the program prints the following text:F#L: #include invalid file name 'file' to the standard output and exits.

# Conditional command #if, #else, #endif

The directive is split among multiple lines in the following way:

```
#if Symbol
Code1
#else
Code2
#endif
```

Sequence Code1 denotes all lines between #if and #else. Sequence Code2 denotes all lines between #else and #endif. These sequences can also be empty (although the #if, #else and #endif commands must be terminated by a line break character and must be on it's individual line). The #else block is optional. The semantics is the same, as if the else block (sequence of lines Code2) was empty.

If the symbol Symbol is declared, then the code between #if and #else (Code1) is active. Otherwise, the code between #else and #endif (Code2) is active. Program is active within the conditional command, if all the blocks the program currently is in, are active. The main block (within the beginning and the end of a file) is always active.

Therefore, even if the program is in passive mode, it is necessary to process the #if, #else, #endif commands, and remember which symbol was defined and which was not.

If the #if directive doesn't have a pairing #endif directive (i.e. the program will reach the end of file while processing the conditional command), the program will print F#L: Missing #endif to the standard output and will exit, where L is a number of the last line in the file.

If the #else directive is out of #if-#endif block, the program prints F#L: Standalone #else to the standard output and exits.

If the #endif directive is without pairing #if directive, the program prints F#L: Standalone #endif to the standard output and exits.

# Restrictions

You may assume that the Symbol never contains any whitespace characters and always has at least one character and always is separated from the directive by at least one space (i.e. that the #define, #undef a #if directives are always syntactically correct)
You may assume that the conditional command is always within a single file. It is not required to handle a situations where a part of the conditional command is in different file that is included by the #include directive.
You may assume that there are no other preprocessor directives than the ones specified in this document.

# Examples

Processing of an input file named input.cse with the following content:

```
#define DEBUG
#define EXTRA_DEBUG

Console.WriteLine("Hello");

#if DEBUG
    Console.WriteLine("Debug");
  #if EXTRA_DEBUG
    Console.WriteLine("Extra");
  #endif
#else
    Console.WriteLine("Release");
#endif

#undef EXTRA_DEBUG

#if EXTRA_DEBUG
    Console.WriteLine("Extra");
#endif

return 0;
```

will produce an output file named input.cs with the following content:

```
Console.WriteLine("Hello");

    Console.WriteLine("Debug");
    Console.WriteLine("Extra");

return 0;
```