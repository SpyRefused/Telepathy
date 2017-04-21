using System;
using System.IO;

namespace Telepathy.Core.Memory.Encoding
{
    public class VarInt
    {
        /**
         * Read a variable length integer from the supplied InputStream
         */

        public static int ReadVarInt(BinaryReader binaryReader) // throws IOException
        {
            var b = (byte)binaryReader.Read();

            if (b == 0x80)
                throw new ArgumentNullException("Attempting to read null value as int");            

            var value = b & 0x7F;

            while ((b & 0x80) != 0)
            {
                b = (byte)binaryReader.Read();
                value <<= 7;
                value |= b & 0x7F;
            }
            return value;
        }
    }
}
