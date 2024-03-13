using Milengine.NET.Core.Graphics;

namespace Milengine.NET.Parser.Structure;

public readonly struct VertexFormatToken
{
    public string Name { get; }
    public VerticesType Type { get; }

    public VertexFormatToken(string name, VerticesType type)
    {
        Name = name;
        Type = type;
    }
}
