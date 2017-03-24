using System;
using System.IO;

namespace Telepathy.Core.Schema
{
    public class TelepathySetSchema : TelepathySchema
    {
        public TelepathySetSchema(string name) : base(name)
        {
        }

        public override SchemaType SchemaType { get; set; }
        public override void WriteTo(StreamWriter streamWriter)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteTo(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}