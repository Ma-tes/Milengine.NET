using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Milengine.NET.Core;

public readonly struct ImageDataSet<T>
    where T : unmanaged, IPixel<T>
{
    public uint Width { get; }
    public uint Height { get; }

    public string Path { get; }

    public ImageDataSet(string path)
    {
        using var image = Image.Load<T>(path);
        Width = (uint)image.Width;
        Height = (uint)image.Height;
        Path = path;
    }

    public unsafe void SetGraphicsTexture(Action<ImageDataSet<T>> onInitializationGraphicsFunction, Action<uint, int, T[]> onRowGraphicsFunction)
    {
        using var image = Image.Load<T>(Path);
        onInitializationGraphicsFunction(this);
        image.ProcessPixelRows(pixelAccessor =>
        {
            int height = pixelAccessor.Height;
            for (int i = 0; i < height; i++) { onRowGraphicsFunction((uint)pixelAccessor.Width, i, [..pixelAccessor.GetRowSpan(i)]);  }
        });
    }
}

public readonly struct TextureRenderParameter
{
    public GLEnum Name { get; }
    public float Value { get; }

    public TextureRenderParameter(GLEnum name, float value)
    {
        Name = name;
        Value = value;
    }
}

public sealed class Texture : IGraphicsBindable
{
    public uint Handle { get; }
    public ImageDataSet<Rgba32> ImageTextureInformation { get; }

    public GLEnum TextureType { get; }
    public Stack<TextureRenderParameter> RenderParameters { get; set; } = new();

    public Texture(string path, GLEnum textureType)
    {
        ImageTextureInformation = new ImageDataSet<Rgba32>(path);
        TextureType = textureType;
    }

    public unsafe void LoadUnsafeTexture()
    {
        ImageTextureInformation.SetGraphicsTexture(
            (ImageDataSet<Rgba32> image) => GraphicsContext.Graphics.TexImage2D(TextureType, 0, InternalFormat.Rgba8,
                image.Width, image.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, null),
            (uint rowWidth, int yPosition, Rgba32[] values) =>
            {
                GraphicsContext.Graphics.TexSubImage2D<Rgba32>(TextureType, 0, 0, yPosition, rowWidth, 1, GLEnum.Rgba, GLEnum.UnsignedByte, values);
            });
        while(RenderParameters.TryPop(out TextureRenderParameter parameter))
        {
            GraphicsContext.Graphics.TexParameter(TextureType, parameter.Name, parameter.Value);
        }
        GraphicsContext.Graphics.GenerateMipmap(TextureType);
    }

    public void Bind()
    {
        GraphicsContext.Graphics.ActiveTexture(GLEnum.Texture0);
        GraphicsContext.Graphics.BindTexture(TextureType, Handle);
    }

    public void Dispose() { }
}
