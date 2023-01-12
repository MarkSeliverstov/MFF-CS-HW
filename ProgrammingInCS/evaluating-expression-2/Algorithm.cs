using System;
using System.Reflection;
using System.Text;

namespace ExpressionEvaluator2
{
#nullable enable

interface IAlgorithm{
    void Visit(PlusExpression expr);
    void Visit(MinusExpression expr);
    void Visit(MultiplyExpression expr);
    void Visit(DivideExpression expr);
    void Visit(UnaryMinusExpression expr);
    void Visit(ConstantExpression expr);
}

class IntEvaluate: IAlgorithm
{
    public int Result { get; private set; }
    public void Visit(PlusExpression expr)
    {
        int left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = checked(left + right);
    }

    public void Visit(MinusExpression expr)
    {
        int left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = checked(left - right);
    }

    public void Visit(MultiplyExpression expr)
    {
        int left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = checked(left * right);
    }

    public void Visit(DivideExpression expr)
    {
        int left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = checked(left / right);
    }

    public void Visit(UnaryMinusExpression expr)
    {
        expr.Operand.Accept(this);
        Result = checked(-Result);
    }

    public void Visit(ConstantExpression expr)
    {
        Result = expr.Value;
    }

}

class DoubleEvaluate: IAlgorithm
{
    public double Result { get; private set; }
    public void Visit(PlusExpression expr)
    {
        double left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = left + right;
    }

    public void Visit(MinusExpression expr)
    {
        double left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = left - right;
    }

    public void Visit(MultiplyExpression expr)
    {
        double left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = left * right;
    }

    public void Visit(DivideExpression expr)
    {
        double left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = left / right;
    }

    public void Visit(UnaryMinusExpression expr)
    {
        expr.Operand.Accept(this);
        Result = -Result;
    }

    public void Visit(ConstantExpression expr)
    {
        Result = expr.Value;
    }

}

class PrintExpression: IAlgorithm
{
    public StringBuilder Result;    

    private void SetBrackets(BinaryExpression expr, char op){
        StringBuilder left,right;

        expr.LeftOperand.Accept(this);
        left = Result;

        expr.RightOperand.Accept(this);
        right = Result;

        StringBuilder sb = new StringBuilder("(");
        sb.Append(left);
        sb.Append(op);
        sb.Append(right);
        sb.Append(')');
        Result = sb;
    }
    public void Visit(PlusExpression expr)
    {
        SetBrackets(expr, '+');
    }

    public void Visit(MinusExpression expr)
    {
        SetBrackets(expr, '-');
    }

    public void Visit(MultiplyExpression expr)
    {
        SetBrackets(expr, '*');
    }

    public void Visit(DivideExpression expr)
    {
        SetBrackets(expr, '/');
    }

    public void Visit(UnaryMinusExpression expr)
    {
        expr.Operand.Accept(this);
        StringBuilder sb = Result;
        sb.Insert(0, "(-");
        sb.Append(')');
        Result = sb;
    }

    public void Visit(ConstantExpression expr)
    {
        Result = new StringBuilder(expr.Value.ToString());
    }

}

class PrintExpression_WithMinBrackets : IAlgorithm
{
    public StringBuilder Result;

    public void FirstPriority(BinaryExpression expr, char op){
        StringBuilder left,right;

        expr.LeftOperand.Accept(this);
        StringBuilder sb = Result;

        if (expr.LeftOperand is PlusExpression || expr.LeftOperand is MinusExpression){
            sb.Insert(0, '(');
            sb.Append(')');
        }

        left = sb;

        expr.RightOperand.Accept(this);
        sb = Result;
        if (expr.RightOperand is PlusExpression || expr.RightOperand is MinusExpression){
            sb.Insert(0, '(');
            sb.Append(')');
        }
        right = sb;

        sb = left;
        sb.Append(op);
        sb.Append(right);
        Result = sb;
    }

    public void SecondPriority(BinaryExpression expr, char op){
        StringBuilder left,right;

        expr.LeftOperand.Accept(this);
        left = Result;

        expr.RightOperand.Accept(this);
        StringBuilder sb = Result;
        if (expr.RightOperand is PlusExpression || expr.RightOperand is MinusExpression || expr.RightOperand is UnaryMinusExpression){
            sb.Insert(0, '(');
            sb.Append(')');
        }

        right = sb;
        sb = left;
        sb.Append(op);
        sb.Append(right);
        Result = sb;
    }

    public void Visit(PlusExpression expr)
    {
        StringBuilder left, right;

        expr.LeftOperand.Accept(this);
        left = Result;

        expr.RightOperand.Accept(this);
        right = Result;

        StringBuilder sb = left;
        sb.Append('+');
        sb.Append(right);
        Result = sb;
    }

    public void Visit(MinusExpression expr)
    {
        SecondPriority(expr, '-');
    }

    public void Visit(MultiplyExpression expr)
    {
        FirstPriority(expr, '*');
    }

    public void Visit(DivideExpression expr)
    {
        FirstPriority(expr, '/');
    }

    public void Visit(UnaryMinusExpression expr)
    {
        expr.Operand.Accept(this);
        StringBuilder sb = Result;

        if (expr.Operand is not UnaryExpression && expr.Operand is not ConstantExpression){
            sb.Insert(0, '(');
            sb.Append(')');
        }
        sb.Insert(0, '-');
        Result = sb;
    }

    public void Visit(ConstantExpression expr)
    {
        Result = new StringBuilder(expr.Value.ToString());
    }
}
}