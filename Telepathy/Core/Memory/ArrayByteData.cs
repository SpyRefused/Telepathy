namespace Telepathy.Core.Memory
{
    public class ArrayByteData : IByteData
    {
        private readonly byte[] _data;

        public ArrayByteData(byte[] data)
        {
            _data = data;
        }
        
        public byte Get(long position)
        {
            return _data[(int)position];
        }
    }
}
