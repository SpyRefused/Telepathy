using System;
using System.Collections.Generic;
using Telepathy.Core.Memory.Pool;
using Telepathy.Core.Util;

// A RecyclingRecycler is an ArraySegmentRecycler which actually pools arrays, in contrast
 // with a WastefulRecycler.  

public class RecyclingRecycler : IArraySegmentRecycler
{

    private readonly int _log2OfByteSegmentSize;
    private readonly int _log2OfLongSegmentSize;
    private readonly Recycler<long[]> _longSegmentRecycler;
    private readonly Recycler<byte[]> _byteSegmentRecycler;

    public RecyclingRecycler() : this(11, 8) { }

    public RecyclingRecycler(int log2ByteArraySize, int log2LongArraySize)
    {
        _log2OfByteSegmentSize = log2ByteArraySize;
        _log2OfLongSegmentSize = log2LongArraySize;

        _longSegmentRecycler = new Recycler<long[]>(new Creator<long[]>{ Create = () => new long[(1 << log2LongArraySize) + 1]});
        _byteSegmentRecycler = new Recycler<byte[]>(new Creator<byte[]> { Create = () => new byte[1 << log2ByteArraySize] });

    }

    public int GetLog2OfByteSegmentSize()
    {
        return _log2OfByteSegmentSize;
    }

    public int GetLog2OfLongSegmentSize()
    {
        return _log2OfLongSegmentSize;
    }

    public long[] GetLongArray()
    {
        var arr = _longSegmentRecycler.Get();
        arr.Fill(0);
        return arr;
    }

    private class Recycler<T>
    {

        private readonly ICreator<T> _creator;
        private readonly LinkedList<T> _currentSegments;
        private readonly LinkedList<T> _nextSegments;

        public Recycler(ICreator<T> creator)
        {
            _currentSegments = new LinkedList<T>();
            _nextSegments = new LinkedList<T>();
            _creator = creator;
        }

        public T Get()
        {
            if (_currentSegments.Count <= 0) return _creator.Create();

            var first = _currentSegments.First.Value;
            _currentSegments.RemoveFirst();

            return first;
        }

        public void Recycle(T reuse)
        {
            _nextSegments.AddLast(reuse);
        }

        public void Swap()
        {
            foreach (var segment in _nextSegments)
            {
                _currentSegments.AddLast(segment);
            }
            _nextSegments.Clear();
        }
    }

    private interface ICreator<T>
    {
        Func<T> Create { get; set; }
    }

    private class Creator<T> : ICreator<T>
    {
        public Func<T> Create { get; set; }
    }
}    

    


