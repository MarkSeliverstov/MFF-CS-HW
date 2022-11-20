# Huffman tree - 2(+encoding to a binary file)

### The first version is [here](https://github.com/MarkSeliverstov/HW_Uni/tree/main/ProgrammingInCS/Huffman)

## Input data format and tree building (same as in Huffman I)

- *The program will receive the name of the file in the form of a single command-line argument, read all data from the input file and then proceed with the Huffman tree building and compression. The compressed file will be written to a binary file whose name is gained by adding the .huff extension to the input file name.*

## Output data format (new for Huffman II)

The output file has the following format: header, encoding tree and encoded data. The header consists of 8 bytes with the following values:
```
0x7B 0x68 0x75 0x7C 0x6D 0x7D 0x66 0x66
```
The tree is written in the prefix notation, with each node stored as a 64-bit number encoded in the Little Endian format (bit-ordering used e.g. on the IA-32/x86 platform). Each node therefore takes exactly 8B space in the file. The tree description is terminated via a special sequence consisting of 8 zero bytes (i.e. 64-bit 0 value), which is not used for encoding any node (serving only as a description terminator).

*The inner nodes have the following format:*

 - **bit 0**: is set to 0, indicating that this node is an inner node

 - **bits 1-55**: contain lower 55 bits of the weight of the inner node

 - **bits 56-63**: are set to zero

*And the leaf nodes have the following format:*

 - **bit 0**: is set to 1, indicating that this node is a leaf node
- **bits 1-55**: contain lower 55 bits of the weight of the leaf node
- **bits 56-63**: 8-bit value of the symbol this leaf node corresponds to

The encoding works as follows. Each character in input file is encoded as a bit sequence corresponding to the path from the Huffman tree root to the leaf containing this character. Edges towards left child nodes represent 0 bit value,  while edges towards right child nodes represent 1. Data are encoded as a bit stream, as the various symbols can have bit sequence codes of different length (even a sequence much longer than 8 bits). The sequences for individual input characters are placed in the output bit stream next to each other (in the same order as the original characters in the input file). Because only full bytes can be written to an output file, zero bits are added to the output bit stream to achieve a total bit count of the nearest multiple of 8. When encoding, store the 0th bit of the stream in the 0 bit (i.e. lowest) of the first byte, etc. up to the 7th bit of the stream. The 8th bit is then stored in the 0 bit of the second byte, 16th bit in the 0 bit of the third byte, etc.

For example, the sequence 1101 0010 0001 1010 111 (spaces are used only for increased readability) will be encoded into three bytes: 0x4B 0x58 0x07.

**Example:**
Assume that we already have a Huffman tree which generates the following codes for characters A, B, C, D (used only for illustrative purposes, the tree shape does not correspond to real frequencies):

A: 0
B: 11
C: 100
D: 101

Then the following input sequence (i.e. the contents of the input file): BDAACB
will be converted to this bit sequence (bit stream): 1110 1001 0011
which will be stored as the following bytes: 0x97 0x0C
i.e. whereas the size of the input file is 6 bytes, the size of the encoded data will be only 2 bytes.

Caution: the following (and different) character sequence: BDAACBAA
will be encoded as a different bit sequence: 1110 1001 0011 00
but due to the addition of zero bits will result in the same bytes as with the previous example: 0x97 0x0C
