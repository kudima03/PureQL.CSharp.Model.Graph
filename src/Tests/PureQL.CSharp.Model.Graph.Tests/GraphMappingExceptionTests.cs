namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record GraphMappingExceptionTests
{
    [Fact]
    public void GraphMappingExceptionsCarryAMessageAndACause()
    {
        InvalidOperationException cause = new InvalidOperationException("cause");

        Assert.Equal(
            "The graph does not map to a PureQL query.",
            new GraphMappingException().Message
        );
        Assert.Equal("message", new GraphMappingException("message").Message);
        Assert.Same(cause, new GraphMappingException("message", cause).InnerException);
    }

    [Fact]
    public void GraphNamesAreTheContractOnes()
    {
        Assert.Equal(
            ["Node", "Query", "id", "queryId", "position"],
            [
                GraphLabels.Node,
                GraphLabels.Query,
                GraphProperties.Id,
                GraphProperties.QueryId,
                GraphProperties.Position,
            ]
        );
    }
}
