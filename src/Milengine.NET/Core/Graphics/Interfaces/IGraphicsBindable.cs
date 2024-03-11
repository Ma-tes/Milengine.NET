namespace Milengine.NET.Core.Graphics.Interfaces;

public interface IGraphicsBindable : IDisposable
{
    public uint Handle { get; }

    public void Bind();
}