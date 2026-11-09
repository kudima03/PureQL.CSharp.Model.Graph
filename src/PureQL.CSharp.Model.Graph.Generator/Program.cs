using System.Reflection;

namespace PureQL.CSharp.Model.Graph.Generator;

// Generates the graph writers and readers of PureQL.CSharp.Model.Graph from the model
// this project references.
//
//   dotnet run --project PureQL.CSharp.Model.Graph.Generator
//       [-- --output <directory of the generated files>]
//
// Every .g.cs file of the output directory is replaced.
internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        string output = Path.GetFullPath(
            Option(args, "--output")
                ?? Path.Combine("PureQL.CSharp.Model.Graph", "Generated")
        );
        string project = Path.Combine(
            Path.GetDirectoryName(output)!,
            "PureQL.CSharp.Model.Graph.csproj"
        );
        if (!File.Exists(project))
        {
            await Console
                .Error.WriteLineAsync(
                    $"{output} is not in the PureQL.CSharp.Model.Graph project."
                )
                .ConfigureAwait(false);
            return 1;
        }

        Assembly model = typeof(PureQLQuery).Assembly;
        SortedDictionary<string, string> files = new Emitter(
            new ModelCatalog(typeof(PureQLQuery)),
            Emitter.VersionOf(model)
        ).Emit();

        _ = Directory.CreateDirectory(output);
        foreach (string stale in Directory.EnumerateFiles(output, "*.g.cs"))
        {
            File.Delete(stale);
        }

        foreach (KeyValuePair<string, string> file in files)
        {
            await File.WriteAllTextAsync(Path.Combine(output, file.Key), file.Value)
                .ConfigureAwait(false);
        }

        Console.WriteLine($"Wrote {files.Count} files to {output}.");
        return 0;
    }

    private static string? Option(string[] args, string name)
    {
        int index = Array.IndexOf(args, name);
        return (index >= 0) && ((index + 1) < args.Length) ? args[index + 1] : null;
    }
}
