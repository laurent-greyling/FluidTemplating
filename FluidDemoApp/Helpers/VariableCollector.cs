namespace FluidDemoApp.Helpers;

public static class VariableCollector
{
    public static Dictionary<string, object?> Generate()
    {
        Console.WriteLine("Add variables (key/value). Type 'done' as the key to finish.\n"); 
        var variables = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        while (true)
        {
            Console.Write("Variable name (or 'done'): ");
            var key = (Console.ReadLine() ?? "").Trim();
            if (key.Equals("done", StringComparison.OrdinalIgnoreCase)) break;
            if (string.IsNullOrWhiteSpace(key))
            { 
                Console.WriteLine("Key cannot be empty.");
                continue;
            }
    
            Console.Write("Value: ");
            variables[key] = Console.ReadLine();
        }
        
        return variables;
    }
}