using System;
using System.Collections.Generic;

namespace ExpressionEvaluator2;
#nullable enable
abstract class Expression
{
    public static Expression? ParsePrefixExpression(string exprString)
    {
        string[] tokens = exprString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        Expression? result = null;
        Stack<OperatorExpression> unresolved = new Stack<OperatorExpression>();



        foreach (string token in tokens)
        {
            if (result != null)
            {
                // We correctly parsed the whole tree, but there was at least one more unprocessed token left.
                // This implies incorrect input, thus return null.

                return null;
            }

            switch (token)
            {
                case "+":
                    unresolved.Push(new PlusExpression());
                    break;

                case "-":
                    unresolved.Push(new MinusExpression());
                    break;

                case "*":
                    unresolved.Push(new MultiplyExpression());
                    break;

                case "/":
                    unresolved.Push(new DivideExpression());
                    break;

                case "~":
                    unresolved.Push(new UnaryMinusExpression());
                    break;

                default:
                    if (!int.TryParse(token, out var value))
                    {
                        return null;    // Invalid token format
                    }

                    Expression? expr = new ConstantExpression(value);
                    while (unresolved.Count > 0)
                    {
                        OperatorExpression oper = unresolved.Peek();
                        if (oper.AddOperand(expr))
                        {
                            unresolved.Pop();
                            expr = oper;
                        }
                        else
                        {
                            expr = null;
                            break;
                        }
                    }

                    if (expr != null)
                    {
                        result = expr;
                    }

                    break;
            }
        }

        return result;
    }

    public abstract int Evaluate();
    public abstract void Accept(IAlgorithm alg);
}

abstract class ValueExpression : Expression
{
    public abstract int Value
    {
        get;
    }

    public sealed override int Evaluate()
    {
        return Value;
    }
}

sealed class ConstantExpression : ValueExpression
{
    public ConstantExpression(int value)
    {
        Value = value;
    }

    public override int Value { get; }

    public override void Accept(IAlgorithm alg)
    {
        alg.Visit(this);
    }
}

abstract class OperatorExpression : Expression
{
    public abstract bool AddOperand(Expression op);
}

abstract class UnaryExpression : OperatorExpression
{
    protected Expression? operand;

    public Expression Operand => operand!;

    public override bool AddOperand(Expression op)
    {
        if (operand == null)
        {
            operand = op;
        }
        return true;
    }

    public sealed override int Evaluate()
    {
        return Compute(operand!.Evaluate());
    }

    protected abstract int Compute(int opValue);
}

abstract class BinaryExpression : OperatorExpression
{
    protected Expression? leftOperand, rightOperand;

    public Expression LeftOperand => leftOperand!;
    public Expression RightOperand => rightOperand!;

    public override bool AddOperand(Expression op)
    {
        if (leftOperand == null)
        {
            leftOperand = op;
            return false;
        }
        if (rightOperand == null)
        {
            rightOperand = op;
        }
        return true;
    }

    public sealed override int Evaluate()
    {
        return Compute(leftOperand!.Evaluate(), rightOperand!.Evaluate());
    }

    protected abstract int Compute(int op0Value, int op1Value);
}

sealed class PlusExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return checked(op0Value + op1Value);
    }

    public override void Accept(IAlgorithm alg)
    {
        alg.Visit(this);
    }

}

sealed class MinusExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return checked(op0Value - op1Value);
    }

    public override void Accept(IAlgorithm alg)
    {
        alg.Visit(this);
    }
}

sealed class MultiplyExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return checked(op0Value * op1Value);
    }

    public override void Accept(IAlgorithm alg)
    {
        alg.Visit(this);
    }
}

sealed class DivideExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return (op0Value / op1Value);
    }

    public override void Accept(IAlgorithm alg)
    {
        alg.Visit(this);
    }
}

sealed class UnaryMinusExpression : UnaryExpression
{
    protected override int Compute(int opValue)
    {
        return checked(-opValue);
    }

    public override void Accept(IAlgorithm alg)
    {
        alg.Visit(this);
    }
}

class Command
{
    public static void Parse(string input)
    {
        string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        try{
            switch (tokens[0])
            {
                case "=":
                    if (input.Length < 3)
                    {
                        Memory.expr = null;
                        Console.WriteLine("Format Error");
                        break;
                    }
                    ParseExpression(input.Substring(2));
                    break;
                case "i":
                    if (Memory.expr == null){
                        Console.WriteLine("Expression Missing");
                        break;
                    }
             
                    IntEvaluate alg = new IntEvaluate();
                    Memory.expr.Accept(alg);
                    Console.WriteLine(alg.Result);
                    break;
                case "d":
                    if (Memory.expr == null){
                        Console.WriteLine("Expression Missing");
                        break;
                    }
             
                    DoubleEvaluate alg2 = new DoubleEvaluate();
                    Memory.expr.Accept(alg2);
                    Console.WriteLine(alg2.Result.ToString("F5"));
                    break;
                case "p":
                    if (Memory.expr == null){
                        Console.WriteLine("Expression Missing");
                        break;
                    }
             
                    PrintExpression alg3 = new PrintExpression();
                    Memory.expr.Accept(alg3);
                    Console.WriteLine(alg3.Result);
                    break;
                case "P":
                   if (Memory.expr == null){
                        Console.WriteLine("Expression Missing");
                        break;
                    }
             
                    PrintExpression_WithMinBrackets alg4 = new PrintExpression_WithMinBrackets();
                    Memory.expr.Accept(alg4);
                    Console.WriteLine(alg4.Result);
                    break;
                default:
                    Console.WriteLine("Format Error");
                    break;
            }
        }
        catch (OverflowException) {
            Console.WriteLine("Overflow Error");
        }
        catch (DivideByZeroException) {
            Console.WriteLine("Divide Error");
        }
    }

    public static void ParseExpression(string input)
    {
        Memory.expr = Expression.ParsePrefixExpression(input);
        
        if (Memory.expr == null)
        {
            Console.WriteLine("Format Error");
        }
    }
}

class Memory{
    public static Expression? expr = null;
}

class Program
{
    static void Main(string[] args)
    {
        string? input = Console.ReadLine()!;

        while ((input != null && input != "end"))
        {
            if (input == "")
            {
                input = Console.ReadLine()!;
                continue;
            }

            Command.Parse(input);

            input = Console.ReadLine()!;
        }
    }
}