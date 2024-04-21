using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Interfaces;
using Milengine.NET.Core.Graphics.Structures;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Milengine.NET.Core.Material;

public sealed class TextureMapper : IGraphicsBindable
{
    internal Stack<TextureRenderParameter> RenderParameters { get; set; } = new();

    public uint Handle { get; }
    public ImageDataSet<Rgba32> ImageTextureInformation { get; }
    public GLEnum TextureType { get; }

    public Vector2D<int> TextureMapSize { get; }

    public ReadOnlyMemory<Rgba32> TextureMapData { get; }
    public ReadOnlyMemory<Rgba32> RelativeTextureMapData { get; }

    public ReadOnlyMemory<Texture> Textures { get; }

    public static readonly Rgba32 NullableRgb32Color = new(251, 0, 255);

    public TextureMapper(string path, GLEnum textureType, Vector2D<int> textureMapSize)
    {
        ImageTextureInformation = new ImageDataSet<Rgba32>(path);
        TextureType = textureType;
        TextureMapSize = textureMapSize;
        TextureMapData = GetImageDataSet(ImageTextureInformation.RelativeImage);
        Textures = SeparateTextureMap([..TextureMapData.Span]).ToArray();
        RelativeTextureMapData = CreateRelativeTextureDataSet(TextureMapData.Span);
    }

    public void Bind()
    {
        GraphicsContext.Graphics.ActiveTexture(GLEnum.Texture0);
        GraphicsContext.Graphics.BindTexture(TextureType, Handle);

        unsafe{
            ImageTextureInformation.SetGraphicsTexture(
                (ImageDataSet<Rgba32> image) => GraphicsContext.Graphics.TexImage2D(TextureType, 0, InternalFormat.Rgba8,
                    (uint)TextureMapSize.X, (uint)TextureMapSize.Y, 0, GLEnum.Rgba, GLEnum.UnsignedByte,
                        RelativeTextureMapData.Span));
        }
        GraphicsContext.Graphics.GenerateMipmap(TextureType);
    }

    private IEnumerable<Texture> SeparateTextureMap(Rgba32[] bufferMap)
    {
        int bufferLength = bufferMap.Length;
        bool isBufferWrap = false;
 
        int relativeYPosition = 0;
        int textureIndex = 0;
        while(textureIndex < bufferLength && !isBufferWrap)
        {

            var currentTexture = new Texture($"Texture::{textureIndex}",
                new Vector2D<int>(
                    textureIndex - (relativeYPosition * (int)ImageTextureInformation.Width),
                    relativeYPosition * (int)ImageTextureInformation.Height
                ));
            yield return currentTexture;

            textureIndex += TextureMapSize.X;
            if(textureIndex == ImageTextureInformation.Width) relativeYPosition++;

            Rgba32 nextPixelColor = bufferMap[textureIndex + 1];
            if(nextPixelColor.Rgba == NullableRgb32Color.Rgba)
                isBufferWrap = true;
        }
    }

    public Memory<Rgba32> CreateRelativeTextureDataSet(ReadOnlySpan<Rgba32> data)
    {
        int dataLength = data.Length;
        Memory<Rgba32> textureDataSet = new Rgba32[dataLength];
        Span<Rgba32> textureDataSetSpan = textureDataSet.Span;

        int textureWidth = TextureMapSize.X;
        int textureHeight = TextureMapSize.Y;
        int textureAreaSize = textureWidth * textureHeight;

        int currentYPosition = 0;
        int textureCount = 0;

        while(currentYPosition < ImageTextureInformation.Height)
        {
            int relativeXPosition = textureCount * textureWidth;
            for (int i = 0; i < textureHeight; i++)
            {
                int currentDataIndex = (i * textureWidth) + (textureCount * textureAreaSize);
                int currentRowDataSetIndex = relativeXPosition + ((int)ImageTextureInformation.Width * i);
                data[currentRowDataSetIndex..(currentRowDataSetIndex + textureWidth)].CopyTo(textureDataSetSpan[currentDataIndex..]);
            }
            int currentBufferWidthDifference = relativeXPosition - ((int)ImageTextureInformation.Width * currentYPosition);
            if(currentBufferWidthDifference == ImageTextureInformation.Width) currentYPosition += (int)ImageTextureInformation.Height;
            textureCount++;
        }
        return textureDataSet;
    }

    private static Memory<Rgba32> GetImageDataSet(Image<Rgba32> image)
    {
        var imagePixelBuffer = image.GetPixelMemoryGroup();
        Memory<Rgba32> returnBuffer = new Rgba32[imagePixelBuffer.TotalLength];
        Span<Rgba32> returnBufferSpan = returnBuffer.Span;

        int relativeAllocationLenght = 0;
        for (int i = 0; i < imagePixelBuffer.Count; i++)
        {
            Span<Rgba32> currentPixelBuffer = imagePixelBuffer[i].Span;
            currentPixelBuffer.CopyTo(returnBufferSpan[relativeAllocationLenght..]);

            relativeAllocationLenght += currentPixelBuffer.Length;
        }
        return returnBuffer;
    }

    public void Dispose()
    {
        GraphicsContext.Graphics.DeleteTexture(Handle);
        ImageTextureInformation.Dispose();
    }
}
