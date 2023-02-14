using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPath
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
        public Node root {get;}
        Node CurrentNode;
        List<Node> Set;

        public Tree()
        {
            root = new Node(null);
            CurrentNode = root;
            Set = new List<Node>(){root};
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
    }

    class Query{
        string query {get;set;}
        List<Node> set {get;set;}

        public Query(StreamReader sr, Tree tree){
            this.query = ReadQuery(sr);
            this.set = new List<Node>(){tree.root};
        }
        
        string ReadQuery(StreamReader sr){
            StringBuilder sb = new StringBuilder();
            char ch = (char)sr.Read();
            while (sr.EndOfStream == false)
            {
                if (ch != ' ' && ch != '\t'){
                    ch = (char)sr.Read();
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        public List<Node> Run(){
            
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

            Query query = new Query(srQuery, tree);
            List<Node> result = query.Run();

            srQuery.Close();
            srData.Close();
        }
    }
}
