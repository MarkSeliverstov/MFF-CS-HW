using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Huffman
{
    #nullable enable
    class Node{
        public int order;
        public Int32 value;
        public byte? ch;
        public Node? Left;
        public Node? Right;

        public Node(Int32 value, byte? ch, int order, Node? Left, Node? Right){
            this.Left = Left;
            this.Right = Right;
            this.value = value;
            this.ch = ch;
            this.order = order;
        }

        public bool IsLessThan(Node comparator){
            if (this.value < comparator.value)
                return true;
            else if(this.value == comparator.value){
                if (this.ch != null && comparator.ch != null){ // if leaf nodes
                    if(this.ch < comparator.ch) return true;
                }
                else if (this.ch == null && comparator.ch == null){ // if inner nodes
                    if(this.order < comparator.order) return true;
                }
                // if this node is leaf and compsarator not leaf
                else if(comparator.ch == null)
                    return true;
            }
            return false;
        }
    }

    class BinaryTree{
        public Node root;

        public BinaryTree(Int32 value, byte? ch, int order, Node? Left, Node? Right){
            root = new Node(value, ch, order, Left, Right);
        }
    }

    class Reader{
        private TextReader r {get; set;}

        public Reader(TextReader reader){
            this.r = reader;
        }

        public int? ReadByte(){
            return r.Read();
        }

    }

    class Writer{   

        public static void Bin_Rec_WriteTree(Node root, BinaryWriter writer){
            if (root.Right == null && root.Left == null){
                WriteLeaf(root.value, root.ch, writer);
                return;
            }
            WriteInner(root.value, writer);
            if (root.Right != null && root.Left != null){
                Bin_Rec_WriteTree(root.Left, writer);
                Bin_Rec_WriteTree(root.Right, writer);
            }
            return;
        }

        private static void WriteLeaf(Int32 value, byte? ch, BinaryWriter w){
            byte[] zeroEnd = {0x00, 0x00, 0x00};
            w.Write(value*2+1);
            w.Write(zeroEnd);
            if (ch != null){
                w.Write((byte)ch);
            }
        }

        private static void WriteInner(Int32 value, BinaryWriter w){
            byte[] zeroEnd = {0x00, 0x00, 0x00, 0x00};
            w.Write(value*2);
            w.Write(zeroEnd);
        }

        public static void  WriteEmptyBytes(BinaryWriter w){
            byte[] empty = new byte[8]{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
            w.Write(empty);

        }

        public static void Bin_WriteHeader(BinaryWriter writer){
            byte[] header = new byte[]{0x7B,0x68,0x75,0x7C,0x6D,0x7D,0x66,0x66};
            writer.Write(header);
        }

        public static void Bin_WriteEncodedData(
                                BinaryWriter w,  
                                BinaryReader r,
                                Dictionary<byte?, string> dict){
            Byte oneByte;
            string bitesFromFile = "";
            try{
                while (true)
                {
                    oneByte = r.ReadByte();
                    bitesFromFile += dict[oneByte];
                    while (bitesFromFile.Length > 8){
                        string byteToFile = bitesFromFile.Substring(0, 8);
                        WriteStringByte(byteToFile, w);
                        bitesFromFile = bitesFromFile.Substring(8, bitesFromFile.Length - 8);
                    }
                }
            }
            catch(EndOfStreamException){
            }
            if(bitesFromFile != ""){
                WriteStringByte(bitesFromFile, w);
            }
        }

        private static void WriteStringByte(string byteToFile, BinaryWriter w){
            byte result = 0;
            for(byte i=0, m = 1; i<byteToFile.Length; i++, m *= 2){
                result += byteToFile[i] == '1' ? m : (byte)0;
            }

            w.Write(result);
        }
    }

    class EncodingTree{
        public static Dictionary<byte?, string> encodingDict = new();
        public static void Encode(Node root, string code){
            if (root.Right == null && root.Left == null){
                encodingDict.Add((byte?)root.ch, code);
                return;
            }
            if (root.Right != null && root.Left != null){
                Encode(root.Left, code+"0");
                Encode(root.Right, code+"1");
            }
            return;
        }
    }

    class Program{
        public static void Main(string[] args){ 
            if(args.Length != 1){
                Console.WriteLine("Argument Error");
                return;
            }
            string fName = args[0];

            try{
                FileStream fIn = new FileStream(fName, FileMode.Open);
                BinaryReader r = new BinaryReader(fIn, Encoding.UTF8);
                FileStream fOut = new FileStream(fName+".huff", FileMode.CreateNew);
                BinaryWriter w = new BinaryWriter(fOut, Encoding.UTF8);

                List<BinaryTree> forest = CreateForest(r);
                if (forest.Count == 0) return;
                BinaryTree tree = CreateHuffmanTree(forest);
                Writer.Bin_WriteHeader(w);
                Writer.Bin_Rec_WriteTree(tree.root, w);
                Writer.WriteEmptyBytes(w);
                EncodingTree.Encode(tree.root, "");
                r.Close();
                fIn = new FileStream(fName, FileMode.Open);
                r = new BinaryReader(fIn, Encoding.UTF8);
                Writer.Bin_WriteEncodedData(w, r, EncodingTree.encodingDict);
                w.Close();
            }
            catch(FileNotFoundException){
                Console.WriteLine("File Error");
            }
        }   

        public static List<BinaryTree> CreateForest(BinaryReader r){
            List<BinaryTree> forest = new List<BinaryTree>();
            Dictionary<byte, int> WeightCharsInText = new();
            byte oneByte;

            // Create Dict
            try{
                while (true)
                {
                    oneByte = r.ReadByte();

                    byte ch = oneByte;

                    WeightCharsInText[ch] = WeightCharsInText.ContainsKey(ch) ? 
                        WeightCharsInText[ch]+1 : WeightCharsInText[ch] = 1;
                }
            }
            catch(EndOfStreamException){
            }
            
            if (WeightCharsInText.Count == 0) return forest;
            // Create forest
            foreach (var item in WeightCharsInText){
                BinaryTree SinglNodeTree = new BinaryTree(item.Value, item.Key, 0, null, null);
                forest.Add(SinglNodeTree);
            }

            return forest;
        }

        public static BinaryTree CreateHuffmanTree(List<BinaryTree> forest){
            int order = 0;
            while (forest.Count != 1){
                BinaryTree firstMin = FindMin(forest);
                forest.Remove(firstMin);
                BinaryTree secondMin = FindMin(forest);
                forest.Remove(secondMin);

                order++;
                Int32 newValue = firstMin.root.value + secondMin.root.value;

                BinaryTree newTree = firstMin.root.IsLessThan(secondMin.root) ?
                    new BinaryTree(newValue, null, order, firstMin.root, secondMin.root):
                    new BinaryTree(newValue, null, order, secondMin.root, firstMin.root);
                forest.Add(newTree);
            }
            return forest[0];
        }

        public static BinaryTree FindMin(List<BinaryTree> forest){
            BinaryTree minValueTree= forest[0];
            foreach (var tree in forest){
                if (tree.root.IsLessThan(minValueTree.root))
                    minValueTree = tree;
            }
            return minValueTree;
        }
    }
}