using System;
using System.Collections.Generic;

namespace ExpressionEvaluator2{
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



class Program
{
    static void Main(string[] args)
    {
        Expression? expr = null; //memmory

        string? input = Console.ReadLine()!;

        while ((input != null && input != "end")){
            if (input == ""){
                input = Console.ReadLine()!;
                continue;
            }


            if (input.Length == 1){
                if (input[0] == '='){
                    expr = null;
                    Console.WriteLine("Format Error");
                    input = Console.ReadLine()!;
                    continue;
                }
                ExecuteCommand(input, expr);
            }
            else{
                expr = Parse(input);
            }

            input = Console.ReadLine()!;
        }
    }

    private static void ExecuteCommand(string cmd, Expression? expr){
        if (expr == null)
        {
            Console.WriteLine("Expression Missing");
            return;
        }
        
        switch (cmd)
        {
            case "i":
                try{
                    IntEvaluate IntEval = new IntEvaluate();
                    expr.Accept(IntEval);
                    Console.WriteLine(IntEval.Result);
                }
                catch (OverflowException){
                    Console.WriteLine("Overflow Error");
                }
                catch (DivideByZeroException){
                    Console.WriteLine("Divide Error");
                }
                return;
            case "d":
                DoubleEvaluate DoubleEval = new DoubleEvaluate();
                expr.Accept(DoubleEval);
                Console.WriteLine(DoubleEval.Result.ToString("f05"));
                return;
            case "p":
                PrintExpression PrintExpr = new PrintExpression();
                expr.Accept(PrintExpr);
                Console.WriteLine(PrintExpr.Result);
                return;
            case "P":
                PrintExpression_WithMinBrackets printWithBrackets = new PrintExpression_WithMinBrackets();
                expr.Accept(printWithBrackets);
                Console.WriteLine(printWithBrackets.Result);
                return;
            default:
                Console.WriteLine("Format Error");
                return;
        }
    }

    private static Expression? Parse(string input){
        if (input[0] != '=' || input.Length < 3 || input[2] == ' ')
        {
            Console.WriteLine("Format Error");
            return null;
        }

        Expression? expr = Expression.ParsePrefixExpression(input.Substring(2));
        if (expr == null)
        {
            Console.WriteLine("Format Error");
        }
        return expr;
    }
}
}