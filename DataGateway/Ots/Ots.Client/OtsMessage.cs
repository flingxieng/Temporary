using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using System.Threading;

namespace Ots.Client
{
    public class OtsMessage
    {
        public const int HeaderByteSize = 12;
        private static long _messageId = 0;
        private static readonly int[] MoveInit = { 7, 1, 3, 2, 6, 0, 4, 5 };

        public long Id { get; private set; }
        public short Event { get; }
        public ushort Length { get; }
        public byte[] Data { get; }

        public OtsMessage(short @event) : this(@event, Array.Empty<byte>()) { }
        public OtsMessage(short @event, string data) : this(@event, Encoding.UTF8.GetBytes(data)) { }
        public OtsMessage(short @event, byte[] data) : this(default, @event, data) { }
        public OtsMessage(long id, short @event, byte[]? data = null)
        {
            Id = id;
            Event = @event;
            Data = data ?? Array.Empty<byte>();
            Length = (ushort)Data.Length;
        }

        internal OtsMessageBuffer GetBuffer()
        {
            var size = HeaderByteSize + Length;
            var id = NewId();
            var buffer = new OtsMessageBuffer(id, size);
            var span = buffer.Memory.Span;
            BinaryPrimitives.WriteInt64LittleEndian(span, id);
            BinaryPrimitives.WriteInt16LittleEndian(span.Slice(8, 2), Event);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(10, 2), Length);
            Data.CopyTo(span[12..]);
            return buffer;
        }

        internal static OtsMessage Parse(byte[] bytes)
        {
            var id = BinaryPrimitives.ReadInt64LittleEndian(bytes.AsSpan()[..8]);
            var @event = BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan().Slice(8, 2));
            var length = BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan().Slice(10, 2));
            var data = length == 0 ? Array.Empty<byte>() : bytes.AsSpan().Slice(12, length).ToArray();
            return new OtsMessage(id, @event, data);
        }

        private static long NewId()
        {
            return Interlocked.Increment(ref _messageId);
        }
    }

    internal readonly struct OtsMessageBuffer : IDisposable
    {
        private readonly byte[] _array;
        
        public readonly long Id;
        public readonly Memory<byte> Memory;

        public OtsMessageBuffer(long id, int size)
        {
            _array = ArrayPool<byte>.Shared.Rent(size);
            Id = id;
            Memory = _array.AsMemory(0, size);
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(_array);
        }
    }
}