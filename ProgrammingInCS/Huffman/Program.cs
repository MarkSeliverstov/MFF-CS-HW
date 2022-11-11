using System;
using System.Globalization;

namespace Huffman
{
    class Node{
        public int value;
        public decimal? ch;
        public decimal order;
        public Node? Left, Right;
    }

    class BinaryTree{
        public Node root;

        public BinaryTree(int value, decimal? ch, decimal order, Node Left, Node Right){
            this.root.Left = Left;
            this.root.Right = Right;
            this.root.value = value;
            this.root.ch = ch;
            this.root.order = order;
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
        private TextWriter w {get; set;}

        public Writer(TextWriter writer){
            this.w = writer;
        }


        public static void Rec_WriteTree(Node root){
            if (root.Left != null)
                Rec_WriteTree(root.Left);
            if (root.Right != null)
                Rec_WriteTree(root.Right);
            
        }
    }

    class Program{
        public static void Main(string[] args){ 
            // if(args.Length != 1){
            //     Console.WriteLine("Argument Error");
            //     return;
            // }
            // string fName = args[0];

            string fName = "huffman-data/simpl.in";
            try{
                TextReader r = new StreamReader(fName);
                TextWriter w = new StreamWriter("result.out");
                Reader reader = new Reader(r);
                Writer writer = new Writer(w);

                List<BinaryTree> forest = CreateForest(r);
                if (forest == null) return;
                BinaryTree tree = CreateHuffmanTree(forest);
                Writer.Rec_WriteTree(tree.root);
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
            while ((ch = r.Read()) != null)
            {
                WeightCharsInText[ch] = WeightCharsInText.ContainsKey(ch) ? 
                    WeightCharsInText[ch]++ : WeightCharsInText[ch] = 1;
            }

            if (forest.Count() == 0) return null;

            // Create forest
            foreach (var item in WeightCharsInText){
                BinaryTree SinglNodeTree = new BinaryTree(item.Value, item.Key, 0, null, null);
                forest.Add(SinglNodeTree);
            }

            return forest;
        }

        public static BinaryTree CreateHuffmanTree(List<BinaryTree> forest){
            while (forest.Count() != 1){
                BinaryTree firstMin = forest.MinBy(x => x.root.value);
                int firstValue = firstMin.root.value;
                forest.Remove(firstMin);

                BinaryTree secondMin = forest.MinBy(x => x.root.value);
                int secondValue = secondMin.root.value;
                forest.Remove(secondMin);

                decimal order = firstMin.root.order < secondMin.root.order ?
                    secondMin.root.order++ : firstMin.root.order++;
                
                int newValue = firstValue + secondValue;
                BinaryTree newTree = null;
                if (firstValue == secondValue){
                    if (firstMin.root.ch == null && secondMin.root.ch == null){
                        newTree = firstMin.root.ch < secondMin.root.ch ? 
                            new BinaryTree(newValue, null, order, firstMin.root, secondMin.root) :
                            new BinaryTree(newValue, null, order, secondMin.root, firstMin.root);
                    }
                    else if (firstMin.root.ch == null){
                        new BinaryTree(newValue, null, order, firstMin.root, secondMin.root);
                    }
                    else if (secondMin.root.ch == null){
                        new BinaryTree(newValue, null, order, secondMin.root, firstMin.root);
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
    }
}