namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// Thrown when a query cannot be written as a graph (a value the model leaves missing)
/// or a graph cannot be read as a query (a missing or unexpected node, relationship or
/// property).
/// </summary>
public sealed class GraphMappingException : Exception
{
    /// <summary>Creates the exception with a default message.</summary>
    public GraphMappingException()
        : base("The graph does not map to a PureQL query.") { }

    /// <summary>Creates the exception with a message.</summary>
    /// <param name="message">What is wrong, and where.</param>
    public GraphMappingException(string message)
        : base(message) { }

    /// <summary>Creates the exception with a message and the exception that caused it.</summary>
    /// <param name="message">What is wrong, and where.</param>
    /// <param name="innerException">The cause.</param>
    public GraphMappingException(string message, Exception innerException)
        : base(message, innerException) { }
}
