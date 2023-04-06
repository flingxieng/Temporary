using DynamicExpresso;

namespace DataGateway.Clients.Compute.Internal;

public static class ExpressionFactory
{
    private const string SelfName = "s";
    private const string ParamName = "p";
    private static readonly Interpreter Interpreter = new();
    
    static ExpressionFactory()
    {
        Interpreter.Reference(typeof(Aggregator));
    }
    
    public static Func<double, double[]?, double>? Compile(string expression)
    {
        try
        {
            return Interpreter.ParseAsDelegate<Func<double, double[]?, double>>(expression, SelfName, ParamName);
        }
        catch
        {
            return default;
        }
    }
}