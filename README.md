<img align="left" src="assets/Milengine.NET-logo.svg" width=350px height=400px>

# Milengine.NET
Milengine is just yet another OpenGL "retro style" renderer/engine implementation. Futhermore the project started as a small learning project, which should not reach more then 1000 lines of code.
However I found out, that it is not possible to deploy easy to use engine, which would not act as a higher layer of OpenGL bindings.

### Implementation status
Currently every implementation is not going to stay as it is... The only file which is probably not going to be changed is [InlineParameter.cs](https://github.com/Ma-tes/Milengine.NET/blob/main/src/Milengine.NET/Core/Utilities/InlineOptimalizations/Buffers/InlineParameterBuffer/InlineParameter.cs).

---
<div align="center">
  
  ![GitHub Repo stars](https://img.shields.io/github/stars/Ma-tes/Milengine.NET)
  ![GitHub commit activity](https://img.shields.io/github/commit-activity/t/Ma-tes/Milengine.NET)
  [![Scc Count Badge](https://sloc.xyz/github/Ma-tes/Milengine.NET/)](https://github.com/Ma-tes/Milengine.NET/)

</div>
<br>

## Archive - Project stage - Fixed(22.04.2024)
Recently, I have been trying to find and also fix, rendering artefacts, which always appears from the center point of specific collection of meshes.

https://github.com/Ma-tes/Milengine.NET/assets/78597003/86f92b9e-c9ec-4199-bb05-d8f88113dc5f

### Solution
Everything happened on this specific line [Model.cs - Line:51](https://github.com/Ma-tes/Milengine.NET/blob/0fe55309226b14ec59c51c25a1fcf98e0e5b38ce/src/Milengine.NET/Core/Model.cs#L51).
```cs
GraphicsContext.Graphics.DrawArrays(GraphicsContext.Global.CurrentRenderingType, 0, (uint)Meshes.Span[i].Indices.Buffer.Length);
```

In a short sentence, this method invokes to OpenGL, which calls to draw multiple combinations of geometric data(Vertex, Normal, Texture coordinate, or color), in a one API call.
Although, it may sound as a ideal solution, you must provide those geometric data in a certain offset as a one big allocated block of memory. So in some situations, when you are writing your own parser for individual format type, you would be limited, of how you are going to save geometric data.

If you want more informations about this API call, you could found it on [khronos OpenGL documentation](https://registry.khronos.org/OpenGL-Refpages/gl4/html/glDrawArrays.xhtml).


**What caused the issue ?** Well... for rendering [Models](https://github.com/Ma-tes/Milengine.NET/blob/main/src/Milengine.NET/Core/Model.cs) from [Obj format](https://github.com/Ma-tes/Milengine.NET/blob/main/src/Milengine.NET/Parser/ObjFormat.cs), I specified `VertexFormatType`, which basically says order, of how the format parser is going to separate data.

[ObjFormat.cs - Line:19-24](https://github.com/Ma-tes/Milengine.NET/blob/0fe55309226b14ec59c51c25a1fcf98e0e5b38ce/src/Milengine.NET/Parser/ObjFormat.cs#L19C4-L24C7).
```cs
protected override ImmutableArray<VertexFormatToken> vertexTokens { get; } = [
        new VertexFormatToken("v ", VerticesType.Position),
        new VertexFormatToken("vt ", VerticesType.Texture),
        new VertexFormatToken("vn ", VerticesType.Color),
        new VertexFormatToken("f ", VerticesType.FacesInternal)
];
```

For now, we could say that `new VertexFormatToken("f ", VerticesType.FacesInternal)` is irelevant. What we could deducate is, that our `obj` parser could handle three geometric data. So our indicies must do the same...
>[!warning]
>Indicies are containing three different sets of indexes.
>[1, 1, 1] - [1, 1, 2] - [1, 2, 1]

Back to the problematic line - `GraphicsContext.Graphics.DrawArrays(GraphicsContext.Global.CurrentRenderingType, 0, (uint)Meshes.Span[i].Indices.Buffer.Length);`, parameters of this method are...
1. `GLEnum` mode - Flag to set spesific type of buffer rendering.
2. `int` first - Defines the starting index.
3. `uint` count - Defines the number of indicies, which are going to be rendered.

Have you found it ? The `uint` count, which I set as a `(uint)Meshes.Span[i].Indices.Buffer.Length` is not correct, because it is a count of all three types of geometric data. So... what ended up as a result, was a block of memory containing overflowing the original count.

> [!important]
> The count means, specific number of geometric data in memory block and not count of all indexes...

Buffer: [1, 1, 1] - [1, 1, 2] - [1, 2, 1]
- Bad count: `buffer.Length` = 9
- Correct count: `buffer.Length / 3` = 3

```cs
GraphicsContext.Graphics.DrawArrays(GraphicsContext.Global.CurrentRenderingType, 0, (uint)Meshes.Span[i].Indices.Buffer.Length / 3);
```
