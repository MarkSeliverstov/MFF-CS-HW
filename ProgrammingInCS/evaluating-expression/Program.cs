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
        isNegative,
        Plus,
        Minus,
        Multiply,
        Divide,
        Undefined
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
                Node? newRoot = TryLoadToTree(expression.Split(" ", StringSplitOptions.RemoveEmptyEntries));

                // if didn't get error
                if(error == null && newRoot != null) 
                    this.root = newRoot;
            }
            return;
        }

        private Operator GetOperation(string op){
            switch (op){
                case "~":
                    return Operator.isNegative;
                case "+":
                    return Operator.Plus;
                case "-":
                    return Operator.Minus;
                case "*":
                    return Operator.Multiply;
                case "/":
                    return Operator.Divide;
                default:
                    return Operator.Undefined;
            }
        }

        private Node? TryLoadToTree(string[] exp){
            // TODO: create tree with while
            Operator currentOp;
            int i = 0;

            // what we need
            if (int.TryParse(exp[i], out int value)){
                root = new Node(value);
                root.isNegative = true;
                return root;
            }else{
                currentOp = GetOperation(exp[i]);
                if (currentOp == Operator.Undefined){
                    error = Errors.InvalidFormat;
                    return null;
                }else{
                    root = new Node(currentOp);
                    root.isNegative = true;
                }
            }





            // create root = first node
            try {
                if (GetOperation(exp[i]) == Operator.isNegative){
                    i++;
                    if (int.TryParse(exp[i], out int value)){
                        root = new Node(value);
                        root.isNegative = true;
                        return root;
                    }else{
                        currentOp = GetOperation(exp[i]);
                        if (currentOp == Operator.Undefined){
                            error = Errors.InvalidFormat;
                            return null;
                        }else{
                            root = new Node(currentOp);
                            root.isNegative = true;
                        }
                    }
                }
                else if (int.TryParse(exp[i], out int value)){
                    root = new Node(value);
                    return root;
                }else{
                    currentOp = GetOperation(exp[i]);
                    if (currentOp == Operator.Undefined){
                        error = Errors.InvalidFormat;
                        return null;
                    }else{
                        root = new Node(currentOp);
                    }
                }
            }
            catch (IndexOutOfRangeException){
                error = Errors.InvalidFormat;
                return null;
            }

            Node? node = root;
            node.left = new Node(1);
            node = node.left;

            return root;
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