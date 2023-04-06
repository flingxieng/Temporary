using System.Buffers;

namespace DataGateway.Clients.Modbus.Internal;

internal readonly struct ByteMemoryOwner : IDisposable
{
    private readonly byte[] _array;
    public readonly Memory<byte> Memory;
    
    private ByteMemoryOwner(int size)
    {
        _array = ArrayPool<byte>.Shared.Rent(size);
        Memory = _array.AsMemory()[..size];
    }
    
    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(_array, true);
    }
    
    public static ByteMemoryOwner Rent(int size)
    {
        return new ByteMemoryOwner(size);
    }
}