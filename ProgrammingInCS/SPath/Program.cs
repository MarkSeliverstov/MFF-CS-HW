using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace SPath
{
    #nullable enable
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
    }

    struct Step{
        public string identifier {get;set;}
        public List<Query>? predicates {get;set;}
    }

    class Query{
        public int? index {get;set;}
        public List<Step>? steps {get;set;}

        public Query(StreamReader sr){    
            char ch;
            string identifier = "";
            List<Query> predicates = new List<Query>();
            this.steps = new List<Step>();

            while (sr.EndOfStream == false)
            {
                ch = (char)sr.Read();
                switch (ch)
                {
                    case ' ': break;
                    case '\t': break;
                    case '\r': break;
                    case '\n': break;
                    case '/':
                        if (identifier != "")
                            steps.Add(new Step(){identifier = identifier, predicates = predicates});
                        identifier = "";
                        predicates = new List<Query>();
                        break;
                    case '[':
                        predicates.Add(new Query(sr));
                        break;
                    case ']':
                        if (int.TryParse(identifier, out int index))
                            this.index = index;
                        else
                            this.steps = steps;
                        if (predicates.Count != 0)
                            steps.Add(new Step(){identifier = identifier, predicates = predicates});
                        else
                            steps.Add(new Step(){identifier = identifier, predicates = null});
                        return;
                    default:
                        identifier += ch;
                        break;
                }
            }
            if (predicates.Count != 0)
                steps.Add(new Step(){identifier = identifier, predicates = predicates});
            else
                steps.Add(new Step(){identifier = identifier, predicates = null});
        }

        public List<Node> Run(Query query, Node root){
            List<Node> set = new List<Node>(){root};
            foreach (Step step in this.steps!)
            {
                set = Select(set, step.identifier);
                if (step.predicates != null)
                {
                    foreach (Query predicate in step.predicates)
                    {
                        set = Filter(set, predicate);
                    }
                }
            }
            return set;
        }

        private List<Node> Select(List<Node> set, string identifier){
            List<Node> temp = new List<Node>();
            if (identifier == "")
            {
                return set;
            }
            if (identifier == "..")
            {
                foreach (Node node in set)
                {
                    if (node.Parent != null){
                        if (temp.Contains(node.Parent) == false)
                            temp.Add(node.Parent);
                    }
                    else return new List<Node>();
                }
                return temp;
            }
            else
                foreach (Node node in set)
                {
                    temp.AddRange(node.childs);
                }
                if (identifier == "*")
                    return temp;
                else
                    return temp.Where(x => x.data == identifier).ToList();
        }

        private List<Node> Filter(List<Node> set, Query predicate){
            List<Node> temp = new List<Node>();
            if (predicate.index != null){
                if (set.Count > predicate.index)
                    return new List<Node>(){set[predicate.index.Value]};
            }
            else{
                foreach (Node node in set)
                {
                    if (predicate.Run(predicate, node).Count != 0)
                        temp.Add(node);
                        
                }
            }
            return temp;
        }
    }
    
    class Writer{
        public static void Write(List<Node> set, StreamWriter sw){
            if (set.Count == 1 && set[0].data == null){
                sw.WriteLine("/");
                return;
            }
            foreach (Node node in set)
            {
                WriteNode(node, sw);
                sw.WriteLine();
            }
        }

        private static void WriteNode(Node node, StreamWriter sw){
            if (node.Parent.data != null)
                WriteNode(node.Parent, sw);
            sw.Write("/");
            sw.Write(node.data);
            sw.Write("(");
            sw.Write(node.Parent.childs.IndexOf(node));
            sw.Write(")");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StreamReader srData = new StreamReader("data.in");
            StreamReader srQuery = new StreamReader("query.in");
            StreamWriter sw = new StreamWriter("results.out");

            Tree tree = new Tree();
            tree.BuildTree(srData);

            Query query = new Query(srQuery);
            List<Node> result = query.Run(query, tree.root);
            if (result.Count == 0)
                return;
            else
                Writer.Write(result, sw);
            srQuery.Close();
            srData.Close();
            sw.Close();
        }
    }
}