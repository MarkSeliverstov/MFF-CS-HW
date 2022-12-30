using System;
using System.Collections.Generic;
using System.IO;

namespace evaluating_expression{
    #nullable enable
    class Program{
        static void Main(string[] args){
            TextReader reader = Console.In;
            TextWriter writer = Console.Out;


            try{
                string? input = reader.ReadLine();
                ExpressionTree expression = new ExpressionTree(input);
                expression.Evaluate();
                writer.WriteLine(expression.result);
            }
            catch (OverflowException){
                writer.WriteLine("Overflow Error");
                return;
            }
            catch (DivideByZeroException){
                writer.WriteLine("Divide Error");
                return;
            }
            catch (Exception e){
                    writer.WriteLine(e.Message);
                return;
            }
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

    abstract class Node{}

    sealed class NodeInt: Node{
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
        public int? result;

        public ExpressionTree(string? expression){
            if (expression == null)
                throw new Exception("Format Error");
            Queue<string> tokens = new Queue<string>(expression.Split(" ", StringSplitOptions.RemoveEmptyEntries));
            if (tokens.Count == 0)
                throw new Exception("Format Error");
            else{
                Node? node = LoadToTree_rec(tokens);

                if (tokens.Count > 0 || node == null)
                    throw new Exception("Format Error");
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
            if (!tokens.TryDequeue(out string? token)){
                throw new Exception("Format Error");
            }

            if (uint.TryParse(token, out uint value)){
                if (value < 2147483648){
                    return new NodeInt(Convert.ToInt32(value));
                }
                else{
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
                    return null;
                }
            }
        }

        public void Evaluate(){
            this.result = Evaluate_rec(this.root);
        }

        private int Evaluate_rec(Node? node){
            if (node == null)
                throw new Exception("Format Error");
            else if (node is NodeInt){
                NodeInt nodeInt = node as NodeInt;
                return nodeInt.value;
            }
            else if (node is NodeBinOp)
                return Evaluate_BinOp(node as NodeBinOp);
            else if (node is NodeUnarOp)
                return Evaluate_UnarOp(node as NodeUnarOp);
            else
                throw new Exception("Format Error");
        }

        private int Evaluate_BinOp(NodeBinOp node){
            const decimal maxInt = 2147483647;
            long left = Convert.ToInt64(Evaluate_rec(node.left));
            long right = Convert.ToInt64(Evaluate_rec(node.right));
            long result;
            
            switch (node.op){
                case Operator.Plus:
                    result = left + right;
                    if (result > maxInt || result < -maxInt)
                        throw new OverflowException();
                    break;
                case Operator.Minus:
                    result =  left - right;
                    if (result > maxInt || result < -maxInt)
                        throw new OverflowException();
                    break;
                case Operator.Multiply:
                    result = left * right;
                    if (result > maxInt || result < -maxInt)
                        throw new OverflowException();
                    break;
                case Operator.Divide:
                    result = left / right;
                    break;
                default:
                    throw new Exception("Format Error");
            }
    
            return Convert.ToInt32(result);
        }

        private int Evaluate_UnarOp(NodeUnarOp node){
            int next = Evaluate_rec(node.next);
            switch (node.op){
                case Operator.NextIsNegativeInt:
                    return -next;
                default:
                    throw new Exception("Format Error");
            }
        }
    }

}