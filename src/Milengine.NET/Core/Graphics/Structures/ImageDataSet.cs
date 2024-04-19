using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Milengine.NET.Core.Graphics.Structures;

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
        image.Dispose();
    }

    public void Dispose()
    {
        RelativeImage.Dispose();
    }
}