namespace Telepathy.Core.Memory.Pool
{
    public interface IArraySegmentRecycler
    {
        int GetLog2OfByteSegmentSize();

        int GetLog2OfLongSegmentSize();

        long[] GetLongArray();

        void RecycleLongArray(long[] arr);

        byte[] GetByteArray();

        void RecycleByteArray(byte[] arr);

        void Swap();
    }
}
