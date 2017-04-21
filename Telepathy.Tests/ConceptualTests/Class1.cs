using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Telepathy.Tests.ConceptualTests
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void Test()
        {
            var stream = new MemoryStream();

            var streamWriter = new StreamWriter(stream);

            var streamReader = new StreamReader(stream);


            streamWriter.Write(123);

            var b = (byte)streamReader.Read();

            if (b == 0x80)
                throw new ArgumentNullException("Attempting to read null value as int");

            var value = b & 0x7F;

            while ((b & 0x80) != 0)
            {
                b = (byte)streamReader.Read();
                value <<= 7;
                value |= b & 0x7F;
            }            
        }
    }
}
