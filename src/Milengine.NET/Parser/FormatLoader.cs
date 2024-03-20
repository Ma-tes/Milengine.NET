using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Structures;
using Milengine.NET.Core.Utilities;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Milengine.NET.Parser.Structure;
using Silk.NET.Maths;

namespace Milengine.NET.Parser;

public sealed class VertexDataInformation
{
    public int PositionCount { get; set; }
    public int TextureCount { get; set; }
    public int ColorCount { get; set; }

    public VertexDataInformation() { }

    public void Copy(VertexDataInformation vertexDataInformation)
    {
        PositionCount = vertexDataInformation.PositionCount;
        TextureCount = vertexDataInformation.TextureCount;
        ColorCount = vertexDataInformation.ColorCount;
    }

    public void Reset()
    {
        PositionCount = 0;
        TextureCount = 0;
        ColorCount = 0;
    }
}

public abstract class FormatLoader<T>
    where T : GraphicsMesh, IFactoryInstance<T, ReadOnlyMemory<Vertex<float>>,ReadOnlyMemory<uint>>
{
    protected abstract ImmutableArray<VertexFormatToken> vertexTokens { get; }
    protected VertexDataInformation indicesDataInformation { get; set; } = new();
    protected VertexDataInformation lastIndicesDataInformation { get; set; } = new();

    public T? CurrentGraphicsMesh { get; private set; }

    //TODO: Find more memory efficient way of handling
    //related vertex data
    private List<Vertex<float>>[] vertexData = [
        new List<Vertex<float>>(),
        new List<Vertex<float>>(),
        new List<Vertex<float>>()];

    public ReadOnlyMemory<T> LoadFormatModelData(string path)
    {
        if(!TryGetFormatDataLines(out Span<string> returnDataLines, path))
            throw new ArgumentException($"Relative mode path: {path} is invalid.");

        int currentMeshDataCount = 0;
        var returnModelData = new List<T>();
        while(currentMeshDataCount < (returnDataLines.Length - 1))
        {
            ReadOnlySpan<VertexTokenData> formatMeshData = GetFormatMeshData(out int meshDataLength,
                returnDataLines[currentMeshDataCount..]).Span;

            var indices = GetIndicesData(formatMeshData[formatMeshData.Length - 1].Data.Span);
            for (int i = 0; i < formatMeshData.Length - 1; i++)
            {
                Span<string> currentFormatData = formatMeshData[i].Data.Span;
                VertexFormatToken currentFormatToken = formatMeshData[i].VertexFormatToken;
                ref List<Vertex<float>> currentVertex = ref vertexData[i];
                for (int j = 0; j < currentFormatData.Length; j++)
                {
                    var currentVertexValue = new Vertex<float>();
                    SetVertexData(currentFormatToken, currentFormatData[j], ref currentVertexValue);
                    currentVertex.Add(currentVertexValue);
                }
            }
            returnModelData.Add(T.CreateInstance(
                CreateRelativeFaceVertexData(vertexData, indices.Span).ToArray(), indices.ToArray()
            ));
            currentMeshDataCount += meshDataLength;
            lastIndicesDataInformation.Copy(indicesDataInformation);
        }

        indicesDataInformation.Reset();
        lastIndicesDataInformation.Reset();
        return returnModelData.ToArray();
    }

    protected abstract void SetVertexData(VertexFormatToken vertexFormatToken, string data, ref Vertex<float> relativeVertex);

    protected abstract ReadOnlyMemory<uint> GetIndicesData(ReadOnlySpan<string> indicesData);

    protected virtual bool TryGetRelativeSeparateFormatMesh([NotNullWhen(true)]out VertexTokenData returnVertexTokenData, Span<string> dataLines, int relativeIndex)
    {
        VertexFormatToken currentVertexFormat = vertexTokens[relativeIndex];
        if(!TryGetLastIndexOfLineVertexToken(out int nextVertexIndex, currentVertexFormat, dataLines))
        {
            returnVertexTokenData = default;
            return false;
        }
        returnVertexTokenData = new VertexTokenData(vertexTokens[relativeIndex], dataLines[..nextVertexIndex]);
        return true;
    }

    private Span<Vertex<float>> CreateRelativeFaceVertexData(List<Vertex<float>>[] vertexData, ReadOnlySpan<uint> indices)
    {
        Span<Vertex<float>> relativeVertexData = new Vertex<float>[indices.Length / 3];
        for (int i = 0; i < relativeVertexData.Length; i++)
        {
            int indicesLength = i != relativeVertexData.Length - 1 ? (i * 3) + 3 : indices.Length;
            ReadOnlySpan<uint> vertexIndices = indices[(i * 3)..indicesLength];

            Vector3D<float> vertices = vertexData[0][(int)vertexIndices[0] - 1].Position;
            Vector2D<float> texture = vertexData[1][(int)vertexIndices[1] - 1].TextureCoordinates;
            Vector3D<float> color = vertexIndices[2] == 0 ? Vector3D<float>.Zero : vertexData[2][(int)vertexIndices[2] - 1].ColorNormals;

            relativeVertexData[i] = new Vertex<float>(vertices, color, texture);
        }
        return relativeVertexData;
    }

    private ReadOnlyMemory<VertexTokenData> GetFormatMeshData(out int meshLength, Span<string> dataLines)
    {
        int vertexTokensLength = vertexTokens.Length;
        _ = TryGetIndexOfLineVertexToken(out meshLength, vertexTokens[0], dataLines);

        var returnVertexTokenData = new List<VertexTokenData>();
        for (int i = 0; i < vertexTokensLength; i++)
        {
            if(TryGetRelativeSeparateFormatMesh(out VertexTokenData currentTokenData,
                dataLines[meshLength..], i))
            {
                returnVertexTokenData.Add(currentTokenData);
                meshLength += currentTokenData.Data.Length;
            }
        }
        return returnVertexTokenData.ToArray().AsMemory();
    }

    private int GetVertexLength(VerticesType verticesType, ReadOnlySpan<VertexTokenData> vertexTokenData)
    {
        int vertexTokenDataLength = vertexTokenData.Length;
        for (int i = 0; i < vertexTokenDataLength; i++)
        {
            VertexTokenData relativeVertexSize = vertexTokenData[i];
            if(relativeVertexSize.VertexFormatToken.Type == verticesType)
                return relativeVertexSize.Data.Length;
        }
        return -1;
    }

    private int GetMaximumVertexLength(ReadOnlySpan<VertexTokenData> formatTokens)
    {
        int formatTokensLength = formatTokens.Length;
        int currentVertexSize = 0;
        for (int i = 0; i < formatTokensLength; i++)
        {
            int relativeVertexSize = formatTokens[i].Data.Length;
            if(relativeVertexSize > currentVertexSize) currentVertexSize = relativeVertexSize;
        }
        return currentVertexSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetIndexOfLineVertexToken(out int index, VertexFormatToken vertexFormatToken, Span<string> dataLines)
    {
        int tokenLength = vertexFormatToken.Name.Length;
        int dataLinesLength = dataLines.Length;

        for (int i = 0; i < dataLinesLength; i++)
        {
            string currentTokenData = dataLines[i].Length >= tokenLength ? dataLines[i][..tokenLength] : string.Empty;
            if(currentTokenData == vertexFormatToken.Name)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndexOfLineVertex(Span<string> dataLines)
    {
        int returnIndex = -1;
        int vertexTokensLength = vertexTokens.Length;
        for (int i = 0; i < vertexTokensLength; i++)
        {
            VertexFormatToken currentVertexFormat = vertexTokens[i];
            returnIndex = TryGetIndexOfLineVertexToken(out int currentIndex,
                currentVertexFormat, dataLines) && (currentIndex < returnIndex || returnIndex == -1) ? currentIndex : returnIndex;
        }
        return returnIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetLastIndexOfLineVertexToken(out int index, VertexFormatToken vertexFormatToken, Span<string> dataLines)
    {
        int tokenLength = vertexFormatToken.Name.Length;
        int dataLinesLength = dataLines.Length;

        int firstVertexIndex = GetIndexOfLineVertex(dataLines);
        for (int i = 0; i < dataLinesLength; i++)
        {
            int relativeIndex = i + firstVertexIndex;
            string currentTokenData = dataLines[relativeIndex].Length >= tokenLength ? dataLines[relativeIndex][..tokenLength] : string.Empty;
            if(currentTokenData != vertexFormatToken.Name &&
                IsVertexTypeToken(dataLines[relativeIndex]))
            {
                index = relativeIndex;
                return true;
            }
            if(currentTokenData == vertexFormatToken.Name && i == (dataLinesLength - 1) ||
                currentTokenData == string.Empty && i == (dataLinesLength - 1))
            {
                index = relativeIndex + 1;
                return true;
            }
        }
        index = -1;
        return false;
    }

    private bool IsVertexTypeToken(string data)
    {
        int vertexTokensLength = vertexTokens.Length;
        for (int i = 0; i < vertexTokensLength; i++)
        {
            VertexFormatToken currentVertexTokenData = vertexTokens[i];
            int vertexNameLength = currentVertexTokenData.Name.Length;

            string currentTokenData = data.Length >= vertexNameLength ? data[..vertexNameLength] : string.Empty;
            if(currentTokenData == currentVertexTokenData.Name)
                return true;
        }
        return false;
    }

    private static bool TryGetFormatDataLines([NotNullWhen(true)]out Span<string> returnDataLines, string path)
    {
        if(!File.Exists(path)) { returnDataLines = null!; return false; }
        returnDataLines = File.ReadAllLines(path);
        return true;
    }
}