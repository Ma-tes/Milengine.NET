using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Structures;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Milengine.NET.Parser.Structure;
using Silk.NET.Maths;

namespace Milengine.NET.Parser;

public sealed class ObjFormat : FormatLoader<GraphicsMesh>
{
    private static readonly string vertexSeparator = " ";
    private static readonly string indicesSeparator = "/";

    protected override ImmutableArray<VertexFormatToken> vertexTokens { get; } = [
            new VertexFormatToken("v ", VerticesType.Position),
            new VertexFormatToken("vt ", VerticesType.Texture),
            new VertexFormatToken("vn ", VerticesType.Color),
            new VertexFormatToken("f ", VerticesType.FacesInternal)
        ];

    protected override void SetVertexData(VertexFormatToken vertexFormatToken, string data,
        ref Vertex<float> relativeVertex)
    {
        if(data.Length == 0 || !TrySeparateVertexData(out InlineParameter_Three<float> vertexData, data[vertexFormatToken.Name.Length..], vertexSeparator))
            return;
        VerticesType currentVerticesType = vertexFormatToken.Type;
        var vertexVector = new Vector3D<float>(vertexData[0], vertexData[1], vertexData[2]);

        if(currentVerticesType == VerticesType.Position)
            { relativeVertex.Position = vertexVector; indicesDataInformation.PositionCount++; }
        if(currentVerticesType == VerticesType.Color)
            { relativeVertex.ColorNormals = vertexVector; indicesDataInformation.ColorCount++; }
        if(currentVerticesType == VerticesType.Texture)
            { relativeVertex.TextureCoordinates = new Vector2D<float>(vertexVector.X, vertexVector.Y); indicesDataInformation.TextureCount++; }
    }

    protected override ReadOnlyMemory<uint> GetIndicesData(ReadOnlySpan<string> indicesData)
    {
        var returnIndices = new List<uint>();
        for (int i = 0; i < indicesData.Length; i++)
        {
            string currentData = indicesData[i];
            if(currentData.Length != 0 && TryGetCombinationIndicesData(out uint[] currentIndices, currentData[2..]))
                returnIndices.AddRange(currentIndices);
        }
        return returnIndices.ToArray();
    }

    private bool TryGetCombinationIndicesData([NotNullWhen(true)] out uint[] returnIndices, string data)
    {
        if(!data.Contains(indicesSeparator)) { returnIndices = null!; return false; }
        string[] dataSeparation = data.Split(vertexSeparator)[..3];

        returnIndices = new uint[3 * dataSeparation.Length];
        for (int i = 0; i < dataSeparation.Length; i++)
        {
            int relativeIndex = i * 3;
            if(TrySeparateVertexData(out InlineParameter_Three<uint> returnVertexData,
                dataSeparation[i], indicesSeparator))
            {
                returnIndices[relativeIndex] = returnVertexData[0] - CalculateVertexOffset(indicesDataInformation.PositionCount);
                returnIndices[relativeIndex + 1] = returnVertexData[1] - CalculateVertexOffset(indicesDataInformation.TextureCount);
                returnIndices[relativeIndex + 2] = returnVertexData[2] - CalculateVertexOffset(indicesDataInformation.ColorCount);
            }
        }
        return true;
    }

    private static uint CalculateVertexOffset(int offset) =>
        offset == 0 ? 0 : (uint)offset - 1;
    private static bool TrySeparateVertexData<T>([NotNullWhen(true)] out InlineParameter_Three<T> returnVertexData,
        string data, string separator) where T : INumber<T>
    {
        //TODO: Create more memory efficient way of
        //creating relative value tokens.
        string[] dataSeparation = data.Split(separator);
        Span<T> vertexData = new T[3];
        for (int i = 0; i < dataSeparation.Length; i++)
        {
            if(!T.TryParse(dataSeparation[i], CultureInfo.InvariantCulture, out vertexData[i]!))
            {
                returnVertexData = default;
                return false;
            }
        }
        returnVertexData = InlineValueParameter_Three<T>.CreateInstance(
            vertexData[0], vertexData[1], vertexData[2]
        );
        return true;
    }
}