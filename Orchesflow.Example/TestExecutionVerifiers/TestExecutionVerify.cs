namespace Orchesflow.Example.TestExecutionVerifiers;

public class TestExecutionVerify
{
    public static List<(string origin, bool execution)> Executions { get; set; } = new List<(string origin, bool execution)>();

    public static void Restart()
    {
        Executions = new List<(string origin, bool execution)>();
    }
    
}

