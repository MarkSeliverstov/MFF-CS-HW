using System;
using Microsoft.VisualBasic;

namespace EvaluatingExpression{
    class Program{
        static void Main(string[] args){
            TextReader reader = Console.In;
            TextWriter writer = Console.Out;

            //Expression expression = new Expression(reader.ReadLine());
            ExpressionTree expression = new ExpressionTree("~ ~ ~ 2 3");
            expression.Recurcive_Evaluate(expression.root);
            writer.WriteLine(expression.result);
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

    abstract class Node{}

    sealed class NodeInt: Node{
        public Node? left;
        public Node? right;
        public int value;
        public NodeInt(int value) => this.value = value;
    }

    sealed class NodeBinOp: Node{
        public Node? left;
        public Node? right;
        public Operator op;
        public NodeBinOp(Operator op) => this.op = op;
    }

    sealed class NodeUnarOp: Node{
        public Node? next;
        public Operator op;
        public NodeUnarOp(Operator op) => this.op = op;
    }

    class ExpressionTree{
        public Node? root;
        public Errors? error;
        public int? result;


        public ExpressionTree(string expression){
            Queue<string> tokens = new Queue<string>(expression.Split(" ", StringSplitOptions.RemoveEmptyEntries));

            if (tokens.Count == 0)
                this.error = Errors.InvalidFormat;
            else{
                Node? node = LoadToTree_rec(tokens);
                if (tokens.Count > 0)
                    this.error = Errors.InvalidFormat;
                else
                    this.root = node;
            }
            return;
        }

        private Operator GetOperation(string op){
            switch (op){
                case "~":
                    return Operator.NextIsNegativeInt;
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
        
        private Node? LoadToTree_rec(Queue<string> tokens){
            if (!tokens.TryDequeue(out string token)){
                this.error  = Errors.InvalidFormat;
                return null;
            }

            if (uint.TryParse(token, out uint value)){
                if (value < 2147483648){
                    return new NodeInt(Convert.ToInt32(value));
                }
                else{
                    this.error = Errors.InvalidFormat;
                    return null;
                }
            }  
            else{
                Operator currentOp = GetOperation(token);
                if (currentOp == Operator.NextIsNegativeInt){
                    NodeUnarOp node = new NodeUnarOp(currentOp);
                    node.next = LoadToTree_rec(tokens);
                    if (node.next == null)
                        return null;
                    else
                        return node;
                }
                else if(currentOp != Operator.Undefined){
                    NodeBinOp node = new NodeBinOp(currentOp);
                    node.left = LoadToTree_rec(tokens);
                    node.right = LoadToTree_rec(tokens);
                    if (node.left == null || node.right == null)
                        return null;
                    else
                        return node;
                }
                else{
                    this.error = Errors.InvalidFormat;
                    return null;
                }
            }
        }
    }

}