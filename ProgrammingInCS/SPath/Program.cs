using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

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
            result.Add(root);
            List<string> predicats = new List<string>();
            string identefier = "";

            char ch = (char)sr.Read();
            while (sr.EndOfStream == false)
            {
                ch = (char)sr.Read();
                switch (ch)
                {
                    case ' ': break;
                    case '\t': break;
                    case '[':
                        predicats.Add(AddPredicat(sr));
                        break;
                    case '/':
                        Select(ref result, identefier);
                        Filter(ref result, predicats);
                        identefier = "";
                        predicats.Clear();
                        break;
                    case '*':
                        identefier = "*";
                        break;
                    case '.':
                        identefier = "..";
                        break;
                    default:
                        identefier += ch;
                        break;
                }
            }
            
            return result;
        }

        private string AddPredicat(StreamReader sr)
        {
            string predicat = "";
            char ch = (char)sr.Read();
            while (ch != ']')
            {
                predicat += ch;
                ch = (char)sr.Read();
            }
            return predicat;
        }

        private void Select(ref List<Node> result, string identefier)
        {
            List<Node> preResult = new List<Node>();
            foreach (Node node in result)
            {
                if (identefier == "*")
                {
                    foreach (Node child in node.childs)
                    {
                        preResult.Add(child);
                    }
                }
                else if (identefier == "..")
                {
                    if (!preResult.Contains(node.Parent!))
                    {
                        preResult.Add(node.Parent!);
                    }
                }
                else
                {
                    foreach (Node child in node.childs)
                    {
                        if (child.data == identefier)
                        {
                            preResult.Add(child);
                        }
                    }
                }
            }
            result = preResult;
        }

        private void Filter(ref List<Node> result, List<string> predicats)
        {
            List<Node> preResult = new List<Node>();
            foreach (string predicat in predicats)
            {
                foreach (Node node in result)
                {
                    //TODO: Filter
                }
            }
            result = preResult;
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
