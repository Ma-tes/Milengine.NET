using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Milengine.NET.Core.Material;

public readonly struct ImageDataSet<T> : IDisposable
    where T : unmanaged, IPixel<T>
{
    public uint Width { get; }
    public uint Height { get; }

    public Image<T> RelativeImage { get; }
    public string Path { get; }

    public ImageDataSet(string path)
    {
        RelativeImage = Image.Load<T>(path);
        Width = (uint)RelativeImage.Width;
        Height = (uint)RelativeImage.Height;
        Path = path;
    }

    public unsafe void SetGraphicsTexture(Action<ImageDataSet<T>> onInitializationGraphicsFunction)
    {
        using var image = Image.Load<T>(Path);
        onInitializationGraphicsFunction(this);
    }

    public void Dispose()
    {
        RelativeImage.Dispose();
    }
}


public sealed class TextureMapper : IGraphicsBindable
{
    internal Stack<TextureRenderParameter> RenderParameters { get; set; } = new();

    public uint Handle { get; }
    public ImageDataSet<Rgba32> ImageTextureInformation { get; }
    public GLEnum TextureType { get; }

    public Vector2D<int> TextureMapSize { get; }
    public ReadOnlyMemory<Rgba32> TextureMapData { get; }
    public Rgba32[][] TextureMapData2D { get; }

    public ReadOnlyMemory<Texture> Textures { get; }

    public static readonly Rgba32 NullableRgb32Color = new(251, 0, 255);

    public TextureMapper(string path, GLEnum textureType, Vector2D<int> textureMapSize)
    {
        ImageTextureInformation = new ImageDataSet<Rgba32>(path);
        TextureType = textureType;
        TextureMapSize = textureMapSize;
        //TODO: Creates a specific buffer, which is for resolution 2048x1080 separeted
        //into three buffer segments.
        TextureMapData = GetImageDataSet(ImageTextureInformation.RelativeImage);
        Textures = SeparateTextureMap([..TextureMapData.Span]).ToArray();
        TextureMapData2D = CreateTwoDimensionalRgba32Buffer();
    }

    public void Bind()
    {
        GraphicsContext.Graphics.ActiveTexture(GLEnum.Texture0);
        GraphicsContext.Graphics.BindTexture(TextureType, Handle);

        unsafe{
            ImageTextureInformation.SetGraphicsTexture(
                (ImageDataSet<Rgba32> image) => GraphicsContext.Graphics.TexImage2D(TextureType, 0, InternalFormat.Rgba8,
                    (uint)TextureMapSize.X, (uint)TextureMapSize.Y, 0, GLEnum.Rgba, GLEnum.UnsignedByte, null));
                    //(uint)ImageTextureInformation.RelativeImage.Width, (uint)ImageTextureInformation.RelativeImage.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, null));
        }
        GraphicsContext.Graphics.GenerateMipmap(TextureType);
    }

    public Rgba32[][] CreateTwoDimensionalRgba32Buffer()
    {
        int imageWidht = ImageTextureInformation.RelativeImage.Width;
        int imageHeight = ImageTextureInformation.RelativeImage.Height;

        ReadOnlySpan<Rgba32> mapDataSpan = TextureMapData.Span;
        Rgba32[][] returnRgba32Buffer = new Rgba32[imageHeight][];
        for (int i = 0; i < imageHeight; i++)
        {
            int dataShiftIndex = i * imageWidht;
            Rgba32[] currentData = [..mapDataSpan[dataShiftIndex..(dataShiftIndex + imageWidht)]];
            returnRgba32Buffer[i] = currentData;
        }
        return returnRgba32Buffer;
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
    }
}
