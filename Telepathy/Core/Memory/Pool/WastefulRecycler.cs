namespace Telepathy.Core.Memory.Pool
{
    public class WastefulRecycler : IArraySegmentRecycler
    {

        public static WastefulRecycler DefaultInstance = new WastefulRecycler(11, 8);
        public static WastefulRecycler SmallArrayRecycler = new WastefulRecycler(5, 2);

        private readonly int _log2OfByteSegmentSize;
        private readonly int _log2OfLongSegmentSize;

        public WastefulRecycler(int log2OfByteSegmentSize, int log2OfLongSegmentSize)
        {
            _log2OfByteSegmentSize = log2OfByteSegmentSize;
            _log2OfLongSegmentSize = log2OfLongSegmentSize;
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
            return new long[(1 << _log2OfLongSegmentSize) + 1];
        }

        public byte[] GetByteArray()
        {
            return new byte[1 << _log2OfByteSegmentSize];
        }

        public void RecycleLongArray(long[] arr)
        {}

        public void RecycleByteArray(byte[] arr)
        {}   

        public void Swap()
        {}
    }
}