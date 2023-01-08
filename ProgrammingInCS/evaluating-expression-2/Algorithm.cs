namespace ExpressionEvaluator2;
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
        Result = checked(left + right);
    }

    public void Visit(MinusExpression expr)
    {
        double left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = checked(left - right);
    }

    public void Visit(MultiplyExpression expr)
    {
        double left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = checked(left * right);
    }

    public void Visit(DivideExpression expr)
    {
        double left, right;
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

class PrintExpression: IAlgorithm
{
    public string Result { get; private set; }
    public void Visit(PlusExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = $"({left}+{right})";
    }

    public void Visit(MinusExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = $"({left}-{right})";
    }

    public void Visit(MultiplyExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = $"({left}*{right})";
    }

    public void Visit(DivideExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        Result = $"({left}/{right})";
    }

    public void Visit(UnaryMinusExpression expr)
    {
        expr.Operand.Accept(this);
        Result = $"(-{Result})";
    }

    public void Visit(ConstantExpression expr)
    {
        Result = expr.Value.ToString();
    }

}

class PrintExpression_WithMinBrackets : IAlgorithm
{
    public string Result { get; private set; }
    public void Visit(PlusExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        if (expr.RightOperand is UnaryExpression)
            right = $"({right})";
        Result = $"{left}+{right}";
    }

    public void Visit(MinusExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        if (expr.RightOperand is PlusExpression || expr.RightOperand is MinusExpression || expr.RightOperand is UnaryExpression)
            right = $"({right})";
        Result = $"{left}-{right}";
    }

    public void Visit(MultiplyExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        if (expr.LeftOperand is PlusExpression || expr.LeftOperand is MinusExpression)
            left = $"({left})";
        if (expr.RightOperand is PlusExpression || expr.RightOperand is MinusExpression || expr.RightOperand is UnaryExpression)
            right = $"({right})";
        Result = $"{left}*{right}";
    }

    public void Visit(DivideExpression expr)
    {
        string left, right;
        expr.LeftOperand.Accept(this);
        left = Result;
        expr.RightOperand.Accept(this);
        right = Result;
        if (expr.LeftOperand is PlusExpression || expr.LeftOperand is MinusExpression)
            left = $"({left})";
        if (expr.RightOperand is PlusExpression || expr.RightOperand is MinusExpression || expr.RightOperand is UnaryExpression)
            right = $"({right})";
        Result = $"{left}/{right}";
    }

    public void Visit(UnaryMinusExpression expr)
    {
        expr.Operand.Accept(this);
        
        Result = $"-{Result}";
    }

    public void Visit(ConstantExpression expr)
    {
        Result = expr.Value.ToString();
    }
}