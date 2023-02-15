using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
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
            string line = sr.ReadLine()!;
            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join("", tokens);
        }

        public void Run(){
            string identifier = "";
            string predicate = "";
            

            foreach (char ch in this.query)
            {
                switch (ch)
                {
                    case '/':
                        // this.set = this.set.SelectMany(x => x.childs).ToList();
                        identifier = "";
                        predicate = "";
                        // Step(this.set);
                        break;
                    case '[': 

                        break;
                    case ']':

                        break;
                    default:
                        // this.set = this.set.Where(x => x.data == ch.ToString()).ToList();
                        break;
                }
            }
        }

        // private List<Node> Step(List<Node> set){
            
        // }

        // private List<Node> Select(List<Node> set, string identifier){

        // }

        // private List<Node> Filter(List<Node> set, string predicate){

        // }
        
    }

    struct Step{
        public string identifier {get;set;}
        public List<Predicate>? predicates {get;set;}
    }

    struct Predicate{
        public int? index {get;set;}   
        public List<Step>? steps {get;set;}

        public Predicate(StreamReader sr){    
            char ch;
            string identifier = "";
            List<Predicate> predicates = new List<Predicate>();
            List<Step> steps = new List<Step>();

            while ((ch = (char)sr.Read()) != ']')
            {
                 switch (ch)
                {
                    case ' ': break;
                    case '\t': break;
                    case '\r': break;
                    case '\n': break;
                    case '/':
                        steps.Add(new Step(){identifier = identifier, predicates = predicates});
                        identifier = "";
                        predicates = new List<Predicate>();
                        break;
                    case '[':
                        predicates.Add(new Predicate(sr));
                        break;
                    case ']':
                        if (int.TryParse(identifier, out int index))
                            this.index = index;
                        else
                            this.steps = steps;
                        break;
                    default:
                        identifier += ch;
                        break;
                }
            }
        }
    }

    // /a/b[a/b][a[a/b[v[1]]]][2]/c 
    class Program
    {

        static List<Step> ReadQuery(StreamReader sr){
            char ch;
            string identifier = "";
            List<Step> steps = new List<Step>();
            List<Predicate> predicates = new List<Predicate>();
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
                        predicates = new List<Predicate>();
                        break;
                    case '[':
                        predicates.Add(new Predicate(sr));
                        break;
                    default:
                        identifier += ch;
                        break;
                }
            }
            steps.Add(new Step(){identifier = identifier, predicates = predicates});
            return steps;
        }

        static void Main(string[] args)
        {
            StreamReader srData = new StreamReader("data.in");
            StreamReader srQuery = new StreamReader("query.in");
            StreamWriter sw = new StreamWriter("result.out");

            // Tree tree = new Tree();
            // tree.BuildTree(srData);

            // Query query = new Query(srQuery, tree);
            // List<Node> result = query.Run();

            List<Step> steps = ReadQuery(srQuery);
            srQuery.Close();
            srData.Close();
        }
    }
}