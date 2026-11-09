namespace PureQL.CSharp.Model.Graph.Generator;

// Generates the graph writers and readers of PureQL.CSharp.Model.Graph from the model
// this project references.
internal static class Program
{
    public static int Main()
    {
        Console.WriteLine(
            $"PureQL.CSharp.Model {typeof(PureQLQuery).Assembly.GetName().Version}"
        );
        return 0;
    }
}
