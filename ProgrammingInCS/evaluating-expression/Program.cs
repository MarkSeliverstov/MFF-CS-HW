using System;

namespace EvaluatingExpression{
    class Program{
        static void Main(string[] args){
            TextReader reader = Console.In;
            TextWriter writer = Console.Out;

            // Expression expression = new Expression(reader.ReadLine());
            // Expression expression = new Expression("- - 2000000000 4000000000");
            // expression.Recurcive_Evaluate(expression.root);
            // writer.WriteLine(expression.result);
            if (uint.TryParse("4000000000", out uint value)){
                writer.WriteLine(value);
            }
            else writer.WriteLine("ERROR");
        }
    }

    enum Operator{
        NextIsNegativeInt,
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

    abstract class Node{
        public Node? left;
        public Node? right;
    }

    sealed class NodeVal: Node{
        public int? value;
        public NodeVal(int value) => this.value = value;
    }

    sealed class NodeOp: Node{
        public Operator? op;
        public NodeOp(Operator op) => this.op = op;
    }

    class Expression{
        public Node? root{get; private set;}
        public Errors? error{get; private set;} // TODO: get to string
        public int? result{get; private set;}
        public string[] expression{get; private set;}
        public int indexOfExp = 0;

        public Expression(string expression){
            if (expression.Length == 0)
                error = Errors.InvalidFormat;
            else{
                this.expression = expression.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                Node? newRoot = TryLoadToTree();

                // if didn't get error
                if(error == null && newRoot != null) 
                    this.root = newRoot;
            }
            return;
        }

        private Operator GetOperation(string op){
            switch (op){
                case "~":
                    return Operator.NextIsNegative;
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

        private (Node?, Errors?) ValidateToken(string token, bool isNeg = false){
            Node node;
            if (uint.TryParse(token, out uint value)){
                if (value > 2147483648)
                    return (null, Errors.OverflowError);

                node = new Node(Convert.ToInt32(value));
                node.isNegative = isNeg;
            }else{
                Operator currentOp = GetOperation(token);
                if (currentOp == Operator.Undefined)
                    return (null, Errors.InvalidFormat);
                else if (currentOp == Operator.NextIsNegative){
                    
                }
                else{
                    node = new Node(currentOp);
                    node.isNegative = isNeg;
                }
            }
            return (node, null);
        }

        private Node? TryLoadToTree(Node? node = null){
            
            if (indexOfExp == 0){
                
            }
            

            Operator currentOp;
            int i = 0;

            // what we need

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