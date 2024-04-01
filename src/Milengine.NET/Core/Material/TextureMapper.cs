using System.Runtime.InteropServices;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.ColorSpaces;
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

    public ReadOnlyMemory<Texture> Textures { get; }
    public Rgb24 NullableRgb24Color { get; set; } = new Rgb24(251, 0, 255);

    public TextureMapper(string path, GLEnum textureType, Vector2D<int> textureMapSize)
    {
        ImageTextureInformation = new ImageDataSet<Rgba32>(path);
        TextureType = textureType;
        TextureMapSize = textureMapSize;
        //TODO: Creates a specific buffer, which is for resolution 2048x1080 separeted
        //into three buffer segments.
        var currentImageMemoryGroup = ImageTextureInformation.RelativeImage.GetPixelMemoryGroup();
        TextureMapData = GetImageDataSet(ImageTextureInformation.RelativeImage);
    }

    public void Bind()
    {
        GraphicsContext.Graphics.ActiveTexture(GLEnum.Texture0);
        GraphicsContext.Graphics.BindTexture(TextureType, Handle);

        unsafe{
            ImageTextureInformation.SetGraphicsTexture(
                (ImageDataSet<Rgba32> image) => GraphicsContext.Graphics.TexImage2D(TextureType, 0, InternalFormat.Rgba8,
                    image.Width, image.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, null));
        }
        GraphicsContext.Graphics.GenerateMipmap(TextureType);
    }

    //private IEnumerable<Texture> SeparateTextureMap()

    private Memory<Rgba32> GetImageDataSet(Image<Rgba32> image) =>
        image.GetPixelMemoryGroup()[0];

    public void Dispose()
    {
        GraphicsContext.Graphics.DeleteTexture(Handle);
    }
}
