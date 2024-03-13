namespace Milengine.NET.Parser.Structure;

public ref struct VertexTokenData
{
    public VertexFormatToken VertexFormatToken { get; }
    public ReadOnlySpan<string> Data { get; }

    public VertexTokenData(VertexFormatToken vertexFormatToken, ReadOnlySpan<string> data)
    {
        VertexFormatToken = vertexFormatToken;
        Data = data;
    }
}
