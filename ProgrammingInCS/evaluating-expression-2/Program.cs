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
    public abstract double Evaluate_double();
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

    public sealed override double Evaluate_double()
    {
        return Convert.ToDouble(Value);
    }
}

sealed class ConstantExpression : ValueExpression
{
    public ConstantExpression(int value)
    {
        Value = value;
    }

    public override int Value { get; }
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

    public sealed override double Evaluate_double()
    {
        return Compute_double(operand!.Evaluate());
    }

    protected abstract int Compute(int opValue);
    protected abstract double Compute_double(double opValue);
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

    public sealed override double Evaluate_double()
    {
        return Compute_double(leftOperand!.Evaluate_double(), rightOperand!.Evaluate_double());
    }

    protected abstract int Compute(int op0Value, int op1Value);
    protected abstract double Compute_double(double op0Value, double op1Value);
}

sealed class PlusExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return checked(op0Value + op1Value);
    }

    protected override double Compute_double(double op0Value, double op1Value)
    {
        return op0Value + op1Value;
    }

}

sealed class MinusExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return checked(op0Value - op1Value);
    }

    protected override double Compute_double(double op0Value, double op1Value)
    {
        return op0Value - op1Value;
    }
}

sealed class MultiplyExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return checked(op0Value * op1Value);
    }

    protected override double Compute_double(double op0Value, double op1Value)
    {
        return op0Value * op1Value;
    }
}

sealed class DivideExpression : BinaryExpression
{
    protected override int Compute(int op0Value, int op1Value)
    {
        return (op0Value / op1Value);
    }

    protected override double Compute_double(double op0Value, double op1Value)
    {
        return (op0Value / op1Value);
    }
}

sealed class UnaryMinusExpression : UnaryExpression
{
    protected override int Compute(int opValue)
    {
        return checked(-opValue);
    }

    protected override double Compute_double(double opValue)
    {
        return -opValue;
    }
}

abstract class Command
{
    public abstract void Execute();

    public static Command? ParseCommand(string input)
    {
        string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (tokens[0])
        {
            case "=":
                if (input.Length < 3)
                {
                    Memory.expr = null;
                    return null;
                }
                return new Calculate_cmd(input.Substring(2));

            case "i":
                return new Calculate_in_int();

            case "d":
                return new Calculate_in_double();

            default:
                return null;
        }
    }
}

sealed class Calculate_cmd : Command
{
    public Calculate_cmd(string input)
    {
        Memory.expr = Expression.ParsePrefixExpression(input);
    }
    
    public override void Execute()
    {
        if (Memory.expr == null)
        {
            Console.WriteLine("Format Error");
        }
        else
        {
            Console.Write("");
        }
    }
}

sealed class Calculate_in_int : Command
{
    public override void Execute()
    {
        if (Memory.expr != null)
        {
            try {
                Console.WriteLine(Memory.expr.Evaluate());
            }
            catch (OverflowException) {
                Console.WriteLine("Overflow Error");
            }
            catch (DivideByZeroException) {
                Console.WriteLine("Divide Error");
            }
        }
        else
        {
            Console.WriteLine("Expression Missing");
        }
    }
}

sealed class Calculate_in_double : Command
{
    public override void Execute()
    {
        if (Memory.expr != null)
        {
            try {
                Console.WriteLine(Memory.expr.Evaluate_double().ToString("F5"));
            }
            catch (OverflowException) {
                Console.WriteLine("Overflow Error");
            }
            catch (DivideByZeroException) {
                Console.WriteLine("Divide Error");
            }
        }
        else
        {
            Console.WriteLine("Expression Missing");
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

            Command? cmd = Command.ParseCommand(input)!;

            if (cmd == null){
                Console.WriteLine("Format Error");
            }
            else{
                cmd.Execute();
            }

            input = Console.ReadLine()!;
        }
    }
}