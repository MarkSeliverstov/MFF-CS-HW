using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Huffman
{
    #nullable enable
    class Node{
        public decimal order;
        public decimal value;
        public decimal? ch;
        public Node? Left;
        public Node? Right;

        public Node(decimal value, decimal? ch, decimal order, Node? Left, Node? Right){
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

        public BinaryTree(decimal value, decimal? ch, decimal order, Node? Left, Node? Right){
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
        public static void Rec_WriteTree(Node root, TextWriter writer){
            if (root.Right == null && root.Left == null){
                writer.Write("*{0}:{1} ", root.ch, root.value);
                return;
            }
            writer.Write(root.value + " ");
            if (root.Right != null && root.Left != null){
                Rec_WriteTree(root.Left, writer);
                Rec_WriteTree(root.Right, writer);
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
                FileStream fs = new FileStream(fName, FileMode.Open);
                BinaryReader r = new BinaryReader(fs, Encoding.ASCII);
                TextWriter w = Console.Out;

                List<BinaryTree> forest = CreateForest(r);
                if (forest.Count == 0) return;
                BinaryTree tree = CreateHuffmanTree(forest);
                Writer.Rec_WriteTree(tree.root, w);
                w.Close();
            }
            catch(FileNotFoundException){
                Console.WriteLine("File Error");
            }
        }   

        public static List<BinaryTree> CreateForest(BinaryReader r){
            List<BinaryTree> forest = new List<BinaryTree>();
            Dictionary<char, int> WeightCharsInText = new();
            byte oneByte;

            // Create Dict
            try{
                while (true)
                {
                    oneByte = r.ReadByte();

                    char ch = (char) oneByte;

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
            decimal order = 0;
            while (forest.Count != 1){
                BinaryTree firstMin = FindMin(forest);
                forest.Remove(firstMin);
                BinaryTree secondMin = FindMin(forest);
                forest.Remove(secondMin);

                order++;
                decimal newValue = firstMin.root.value + secondMin.root.value;

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