using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DataGateway.Abstractions;

public sealed class VariableMetadataCollection : IReadOnlyCollection<object>
{
    public static readonly VariableMetadataCollection Empty = new(Array.Empty<object>());

    private readonly object[] _items;
    private readonly ConcurrentDictionary<Type, object> _cache;

    public VariableMetadataCollection(IEnumerable<object> items)
    {
        _items = items != null ? items.ToArray() : throw new ArgumentNullException(nameof(items));
        _cache = new ConcurrentDictionary<Type, object>();
    }
    
    public VariableMetadataCollection(params object[] items) 
        : this((IEnumerable<object>) items)
    {
    }

    public int Count => _items.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetMetadata<T>() where T : class
    {
        if (!_cache.TryGetValue(typeof(T), out var metadata))
            return GetMetadataSlow<T>();
        return metadata as T;
    }
    
    private T? GetMetadataSlow<T>() where T : class
    {
        foreach (var obj in _items)
        {
            if (obj is not T value)
            {
                continue;
            }
            _cache.TryAdd(typeof(T), value);
            return value;
        }
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetRequiredMetadata<T>() where T : class
    {
        var metadata = GetMetadata<T>();
        if (metadata is null)
        {
            throw new InvalidOperationException($"元数据 '{typeof(T)}' 不存在");
        }
        return metadata;
    }

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<object> IEnumerable<object>.GetEnumerator() => GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public struct Enumerator : IEnumerator<object>
    {
        private readonly object[] _items;
        private int _index;
        private object _current;

        internal Enumerator(VariableMetadataCollection collection)
        {
            _items = collection._items;
            _index = 0;
            _current = null!;
        }

        public object Current => _current;

        public void Dispose() { }

        public bool MoveNext()
        {
            if (_index < _items.Length)
            {
                _current = _items[_index++];
                return true;
            }
            _current = null!;
            return false;
        }

        public void Reset()
        {
            _index = 0;
            _current = null!;
        }
    }
}