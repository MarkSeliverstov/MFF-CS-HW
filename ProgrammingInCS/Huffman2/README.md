# Huffman tree - 2(+encoding to a binary file)
- *The goal is to write a program capable of constructing and printing a Huffman tree for a specific file.*

A *Huffman tree* is a binary tree whose leaf nodes contain all characters present in the input file at least once. The weight of a character refers to the number of occurrences of this character in the input file. For example, the data "xxyz" contain characters "x", "y" and "z", with "x" having the weight of 2 and both "y" and "z" having the weight of 1. Inner nodes of the tree do not contain any characters and their weight is defined as the sum of the weights of both their child nodes (inner nodes always have two child nodes).

---



**input:** name of the file in the form of a single command-line argument;

**output:** printed out to standard output in the prefix notation (Leaf nodes should be printed in the format *SYMBOL:WEIGHT);

**conditions:** 

A following algorithm is used when building the tree.
First, prepare all leaf nodes based on the frequency analysis of the input data and organize them in a forest of single-node trees. As long as the forest contains more than one tree, always remove two nodes with the lowest weight and add them as children of a newly-created node (the weight of which is the sum of the weights of the two nodes), then add this node to the forest. When adding the child nodes, keep in mind that the node with lower weight (or more generally, the lighter node, the node with higher priority) is always the left child. If two nodes have the same weight, following rules are applied (in this order) to determine which node has priority (these rules also determine which node should be on the left and which on the right for nodes with the same weight):

- leaf nodes are lighter than inner nodes
- among leaf nodes, those with lower-valued character have priority
- among inner nodes, those that were created sooner by the algorithm have priority

**Example:**
```
$>program.exe simple.in


**simple.in**
    aaabbc


_standard output_
    6 *97:3 3 *99:1 *98:2
```

The first version is [here](https://github.com/MarkSeliverstov/HW_Uni/tree/main/ProgrammingInCS/Huffman)

CALCULATE

10000000 0x16 weight=32 + 0x8
byte 128 + 2xbyte 0 + int val + byte 0;

Header: 
    0x7B 0x68 0x75 0x7C 0x6D 0x7D 0x66 0x66
    123 104 117 124 109 125 102 102

FIX
 - Writer tree
 - Writer Data: creat dict...