using System.Collections;

record class ValidationError
{
    public string Reason { get; init; }

    public ValidationError(string reason)
    {
        Reason = reason;
    }
}

record Order
{
    public required string Id { get; set; }
    public int Amount { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Comment { get; set; }
}

record SuperOrder : Order;

/* ========== My Validation ========== */

interface IValidator<in T>
{
    // Return list of errors
    IEnumerable<ValidationError> Validate(T value);
}

abstract class Validator<T>: IValidator<T>
{
    public abstract IEnumerable<ValidationError> Validate(T value);

    public IEnumerable<ValidationError> Validate<TValue>(TValue value, Validator<TValue> RequiredValidator, params Validator<TValue>[] OtherValidators){
        var errors = new List<ValidationError>();
        errors.AddRange(RequiredValidator.Validate(value));

        foreach (var validator in OtherValidators)
        {
            errors.AddRange(validator.Validate(value));
        }

        return errors;
    }
}

class NonBlankStringValidator : Validator<string>
{
    public override IEnumerable<ValidationError> Validate(string value)
    {
        var allErrors = new List<ValidationError>();
        if (value.Trim().Length == 0)
        {
            allErrors.Add(new ValidationError($"\"{value}\" is empty or just whitespaces."));
        }
        return allErrors;
    }
}

class RangeValidator<T> : Validator<T> where T : IComparable<T>
{
    public required T Min { get; init; }
    public required T Max { get; init; }

    public override IEnumerable<ValidationError> Validate(T value)
    {
        var allErrors = new List<ValidationError>();
        if (value.CompareTo(Min) < 0)
        {
            allErrors.Add(new ValidationError($"{value} is less than minimum {Min}."));
        }
        else if (value.CompareTo(Max) > 0)
        {
            allErrors.Add(new ValidationError($"{value} is greater than maximum {Max}."));
        }
        return allErrors;
    }
}

class StringLengthValidator : Validator<string>
{
    Validator<int> validator { get; init; }

    public StringLengthValidator(Validator<int> validator)
    {
        this.validator = validator;
    }

    public override IEnumerable<ValidationError> Validate(string value)
    {
        var errors = validator.Validate(value.Length);
        if (errors.Count() > 0)
        {
            errors = errors.Select(e => new ValidationError($"\"{value}\" length {e.Reason}")).ToList();
        }
        return errors;
    }
}

class NotNullValidator : Validator<object?>
{
    public override IEnumerable<ValidationError> Validate(object? value)
    {
        var errors = new List<ValidationError>();

        if (value == null){
            errors.Add(new ValidationError("\"\" is null."));
        }

        return errors;
    }
}

/* ========== Me Extantions ========== */

static class ValidatorExtensions
{
    public static void Print(this IEnumerable<ValidationError> errors)
    {
        if (errors.Count() == 0)
        {
            Console.WriteLine("  >>> Validation successful >>>");
        }
        else
        {
            Console.WriteLine("  >>> Validation failed >>>");
            foreach (var e in errors)
            {
                Console.WriteLine($"    > {e.Reason}");
            }
        }
        Console.WriteLine();
    }
}

static class Create{
    public static RangeValidator<T> RangeValidator<T>(T min, T max) where T : IComparable<T> => new RangeValidator<T> { Min = min, Max = max };
    public static StringLengthValidator StringLengthValidator(int min, int max) => new StringLengthValidator(RangeValidator(min, max));
    public static NotNullValidator NotNullValidator() => new NotNullValidator();
    public static NonBlankStringValidator NonBlankStringValidatorr() => new NonBlankStringValidator();
}


/* =================================== */

class OrderValidator : Validator<Order>
{
    public override List<ValidationError> Validate(Order value){
    	var allErrors = new List<ValidationError>();
    	allErrors.AddRange(Validate(value.Amount, new RangeValidator<int> { Min = 1, Max = 10 }));
    	allErrors.AddRange(Validate(value.Id, new NonBlankStringValidator(), new StringLengthValidator(new RangeValidator<int> { Min = 1, Max = 8 })));
    	allErrors.AddRange(Validate(value.TotalPrice, new RangeValidator<decimal> { Min = 0.01M, Max = 999.99M }));
    	allErrors.AddRange(Validate(value.Comment, new NotNullValidator()));
    	return allErrors;
    }
}

class AdvancedOrderValidator : Validator<Order>
{
    public override List<ValidationError> Validate(Order value){
    	var allErrors = new List<ValidationError>();
    	allErrors.AddRange(Validate(value.Amount, Create.RangeValidator(1,10)));
    	allErrors.AddRange(Validate(value.Id, Create.NonBlankStringValidatorr(), Create.StringLengthValidator(1,8)));
    	allErrors.AddRange(Validate(value.TotalPrice, Create.RangeValidator(0.01M, 999.99M)));
    	allErrors.AddRange(Validate(value.Comment, Create.NotNullValidator()));
    	return allErrors;
    }
}

class Program
{
    static void ValidateSuperOrders(IEnumerable<SuperOrder> orders, IValidator<SuperOrder> validator)
    {
        foreach (var o in orders)
        {
            validator.Validate(o).Print();
        }
    }

    static void ValidateAll<T>(IEnumerable<T> orders, IValidator<T> validator)
    {
        foreach (var o in orders)
        {
            validator.Validate(o).Print();
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("--- plain Validators ---");

        var nonBlankStringValidator = new NonBlankStringValidator();
        nonBlankStringValidator.Validate("   ").Print();
        nonBlankStringValidator.Validate("hello").Print();

        var rangeValidator = new RangeValidator<int> { Min = 1, Max = 6 };
        rangeValidator.Validate(7).Print();
        rangeValidator.Validate(1).Print();

        var stringLengthValidator = new StringLengthValidator(new RangeValidator<int> { Min = 5, Max = 6 });
        stringLengthValidator.Validate("Jack").Print();
        stringLengthValidator.Validate("hello-world").Print();
        stringLengthValidator.Validate("hello").Print();

        var notNullValidator = new NotNullValidator();
        object? obj = null;
        notNullValidator.Validate(obj).Print();
        string? s = null;
        notNullValidator.Validate(s).Print();
        Order? order = null;
        notNullValidator.Validate(order).Print();
        s = "hello";
        notNullValidator.Validate(s).Print();

        Console.WriteLine("--- AdvancedOrderValidator.Validate() ---");

        AdvancedOrderValidator advancedValidator = new AdvancedOrderValidator();

        var o1 = new Order { Id = "    ", Amount = 5 };
        advancedValidator.Validate(o1).Print();

        var o2 = new Order { Id = "AC405", Amount = 5 };
        advancedValidator.Validate(o2).Print();

        var o3 = new Order { Id = "AC405", Amount = 600 };
        advancedValidator.Validate(o3).Print();

        var o4 = new Order { Id = "", Amount = 600 };
        advancedValidator.Validate(o4).Print();

        var o5 = new Order { Id = "AC405-12345678", Amount = 5, TotalPrice = 42, Comment = "Best order ever" };
        advancedValidator.Validate(o5).Print();

        var o6 = new Order { Id = "AC405", Amount = 5, TotalPrice = 42, Comment = "Best order ever" };
        advancedValidator.Validate(o6).Print();

        Console.WriteLine("--- OrderValidator.Validate() ---");

        OrderValidator orderValidator = new OrderValidator();

        orderValidator.Validate(o1).Print();
        orderValidator.Validate(o2).Print();
        orderValidator.Validate(o3).Print();
        orderValidator.Validate(o4).Print();
        orderValidator.Validate(o5).Print();
        orderValidator.Validate(o6).Print();

        Console.WriteLine("--- ValidateSuperOrders() ---");

        var s1 = new SuperOrder { Id = "SO501", Amount = 5, TotalPrice = 42, Comment = "Super order 1" };
        var s2 = new SuperOrder { Id = "SO502", Amount = 700, TotalPrice = 41, Comment = "Super order 2" };
        var s3 = new SuperOrder { Id = "", Amount = 800, Comment = "Super order 2" };

        var orders = new List<SuperOrder> { s1, s2, s3 };
        ValidateSuperOrders(orders, orderValidator);

        Console.WriteLine("--- ValidateAll() ---");

        ValidateAll(orders, orderValidator);

        Console.WriteLine("--- ValidateAll<SuperOrder>() ---");

        ValidateAll<SuperOrder>(orders, orderValidator);
    }
}