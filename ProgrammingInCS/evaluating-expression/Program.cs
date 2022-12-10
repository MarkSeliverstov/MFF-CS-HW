using System;

namespace EvaluatingExpression{
    class Program{
        static void Main(string[] args){
            TextReader reader = Console.In;
            TextWriter writer = Console.Out;

            // Expression expression = new Expression(reader.ReadLine());
            Expression expression = new Expression("- - 2000000000 4000000000");
            expression.Recurcive_Evaluate(expression.root);
            writer.WriteLine(expression.result);
        }
    }

    enum Operator{
        Plus,
        Minus,
        Multiply,
        Divide
    }

    enum Errors{
        InvalidFormat,
        DividedByZero,
        OverflowError
    }

    class Node{
        public bool isNegative = false;
        public int? value;
        public Operator? op;

        public Node? parent;
        public Node? left;
        public Node? right;

        public Node(int value){
            this.value = value;
        }

        public Node(Operator op){
            this.op = op;
        }
    }

    class Expression{
        public Node? root{get; private set;}
        public Errors? error{get; private set;} // TODO: get to string
        public int? result{get; private set;}

        public Expression(string expression){
            if (expression.Length == 0)
                error = Errors.InvalidFormat;
            else{
                Node newRoot = TryLoadToTree(expression.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                if(error == null) this.root = newRoot; // if didn't get error
            }
            return;
        }

        private Node TryLoadToTree(string[] expression){
            // TODO: create tree with while
            Node root;

            return null;
        }

        public void Recurcive_Evaluate(Node? root){
            // TODO: evaluate tree fix
            if (root.op != null)
                Console.Write(root.op + " ");
            else if (root.value != null){
                if (root.isNegative) Console.Write("~ ");
                Console.Write(root.value + " ");
            }
            if (root.left != null) Recurcive_Evaluate(root.left);
            if (root.right != null) Recurcive_Evaluate(root.right);
        }
    }

}