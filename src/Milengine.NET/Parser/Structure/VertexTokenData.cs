namespace Milengine.NET.Parser.Structure;

public readonly struct VertexTokenData
{
    public VertexFormatToken VertexFormatToken { get; }
    public Memory<string> Data { get; }

    public VertexTokenData(VertexFormatToken vertexFormatToken, Span<string> data)
    {
        VertexFormatToken = vertexFormatToken;
        Data = data.ToArray(); //TODO: Avoid a related allocation.
    }
}
