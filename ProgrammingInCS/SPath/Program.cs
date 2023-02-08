using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Spath
{
    class Node
    {
        public string? data = null;
        public Node? Parent;
        public List<Node> childs;

        public Node(string? data, Node? Parent = null)
        {
            this.data = data;
            this.Parent = Parent;
            childs = new List<Node>();
        }
    }

    class Tree{
        Node root;
        Node CurrentNode;

        public Tree()
        {
            root = new Node(null);
            CurrentNode = root;
        }

        public void BuildTree(StreamReader sr)
        {
            string line = "";
            while ((line = sr.ReadLine()!) != null)
            {
                string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    switch (token)
                    {
                        case "(":
                            this.CurrentNode = this.CurrentNode!.childs.Last()!;
                            break;
                        case ")":
                            this.CurrentNode = this.CurrentNode.Parent!;
                            break;
                        default:
                            this.CurrentNode.childs.Add(new Node(token, this.CurrentNode));
                            break;
                    }
                }
            }
        }

        public List<Node> Search(StreamReader sr)
        {
            List<Node> result = new List<Node>();
            List<Node> preResult = new List<Node>();
            result.Add(this.CurrentNode);
            string[] tokens = sr.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string path = string.Join(null, tokens);
            tokens = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                string[] subTokens = token.Split(new char[]{'[', ']'}, StringSplitOptions.RemoveEmptyEntries);
                
                // TODO: Add search by index tokens
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StreamReader srData = new StreamReader("data.in");
            StreamReader srQuery = new StreamReader("query.in");
            StreamWriter sw = new StreamWriter("result.out");

            Tree tree = new Tree();
            tree.BuildTree(srData);
            List<Node> result = tree.Search(srQuery);
            srQuery.Close();
            srData.Close();

            
        }
    }
}
