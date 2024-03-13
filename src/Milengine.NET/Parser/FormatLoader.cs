using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Parser.Structure;

namespace Milengine.NET.Parser;

public abstract class FormatLoader<T>
    where T : GraphicsMesh
{
    protected abstract ImmutableArray<VertexFormatToken> vertexTokens { get; }

    public T? CurrentGraphicsMesh { get; private set; }

    public IEnumerable<T> LoadFormatMeshData(string path)
    {
        if(!TryGetFormatDataLines(out string[] returnDataLines, path))
            throw new ArgumentException($"Relative mode path: {path} is invalid.");
    }

    protected abstract void SetVertexData(VertexFormatToken vertexFormatToken, string data);
  
    private static bool TryGetFormatDataLines([NotNullWhen(true)]out string[] returnDataLines, string path)
    {
        if(!File.Exists(path)) { returnDataLines = null!; return false; }
        returnDataLines = File.ReadAllLines(path);
        return true;
    }
}