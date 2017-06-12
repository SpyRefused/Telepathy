using System.Collections.Generic;
using Telepathy.Core.Memory.Pool;

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

        _longSegmentRecycler = new Recycler<long[]>(new Creator<long[]>());
            
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
            if (_currentSegments.Count > 0)
            {
                var first = _currentSegments.First.Value;
                _currentSegments.RemoveFirst();
                return first;
            }
            return _creator.Create();
        }

        public void Recycle(T reuse)
        {
            _nextSegments.AddLast(reuse);
        }

        public void Swap()
        {
            foreach(var segment in _nextSegments)
            {
                _currentSegments.AddLast(segment);
            }            
            _nextSegments.Clear();
        }
    }

    private interface ICreator<T>
    {
        T Create();
    }

    private class Creator<T> : ICreator<T>
    {
        public long[] Create(int log2LongArraySize)
        {
            return new long[(1 << log2LongArraySize) + 1];
        }
    }
}

