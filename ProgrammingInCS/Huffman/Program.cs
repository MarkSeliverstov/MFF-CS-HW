using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Huffman
{
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
            if (root.Right == null && root.Left == null)
                writer.Write("*{0}:{1} ", root.ch, root.value);
            else{
                writer.Write(root.value + " ");
                Rec_WriteTree(root.Left, writer);
                Rec_WriteTree(root.Right, writer);
            }
            return;
        }
    }

    class Program{
        public static void Main(string[] args){ 
            // if(args.Length != 1){
            //     Console.WriteLine("Argument Error");
            //     return;
            // }
            // string fName = args[0];

            string fName = "huffman-data/simple2.in";
            try{
                TextReader r = new StreamReader(fName);
                TextWriter w = new StreamWriter("result.out");

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

        public static List<BinaryTree> CreateForest(TextReader r){
            List<BinaryTree> forest = new List<BinaryTree>();
            Dictionary<int, int> WeightCharsInText = new();
            int ch;

            // Create Dict
            while ((ch = r.Read()) != -1)
            {
                WeightCharsInText[ch] = WeightCharsInText.ContainsKey(ch) ? 
                    WeightCharsInText[ch]+1 : WeightCharsInText[ch] = 1;
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
            while (forest.Count != 1){
                BinaryTree firstMin = FindMin(forest);
                decimal firstValue = firstMin.root.value;
                forest.Remove(firstMin);

                BinaryTree secondMin = FindMin(forest);
                decimal secondValue = secondMin.root.value;
                forest.Remove(secondMin);

                decimal order = firstMin.root.order < secondMin.root.order ?
                    secondMin.root.order+1 : firstMin.root.order+1;
                
                decimal newValue = firstValue + secondValue;
                BinaryTree newTree = null;
                if (firstValue == secondValue){
                    if (firstMin.root.ch == null && secondMin.root.ch == null){
                        newTree = firstMin.root.ch < secondMin.root.ch ? 
                            new BinaryTree(newValue, null, order, firstMin.root, secondMin.root) :
                            new BinaryTree(newValue, null, order, secondMin.root, firstMin.root);
                    }
                    else if (firstMin.root.ch != null){
                        newTree = new BinaryTree(newValue, null, order, firstMin.root, secondMin.root);
                    }
                    else if (secondMin.root.ch != null){
                        newTree = new BinaryTree(newValue, null, order, secondMin.root, firstMin.root);
                    }
                    else{
                        newTree = firstMin.root.order < secondMin.root.order ? 
                            new BinaryTree(newValue, null, order, firstMin.root, secondMin.root) :
                            new BinaryTree(newValue, null, order, secondMin.root, firstMin.root);
                    }

                }
                else{
                    newTree = firstValue < secondValue ? 
                        new BinaryTree(newValue, null, order, firstMin.root, secondMin.root) :
                        new BinaryTree(newValue, null, order, secondMin.root, firstMin.root);
                }
                forest.Add(newTree);

            }
            return forest[0];
        }

        public static BinaryTree FindMin(List<BinaryTree> forest){
            BinaryTree minValueTree= forest[0];
            foreach (var tree in forest){
                if (tree.root.value < minValueTree.root.value)
                    minValueTree = tree;
            }
            return minValueTree;
        }
    }
}