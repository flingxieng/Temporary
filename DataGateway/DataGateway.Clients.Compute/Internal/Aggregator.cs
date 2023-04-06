namespace DataGateway.Clients.Compute.Internal;

public static class Aggregator
{
    public static double Sum(double[]? values)
    {
        if (values == null || values.Length == 0)
            return default;
        return values.Sum();
    }
    
    public static double Avg(double[]? values)
    {
        if (values == null || values.Length == 0)
            return default;
        return values.Average();
    }
}