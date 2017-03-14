using System.Collections.Generic;

namespace Telepathy.Core.Schema
{
    public class FieldType
    {
        public int FixedLength { get; private set; }
        public bool VarIntEncodesLength { get; private set; }
        public FieldType(int fixedLength, bool varIntEncodesLength)
        {
            FixedLength = fixedLength;
            VarIntEncodesLength = varIntEncodesLength;            
        }

        public static readonly FieldType Reference = new FieldType(-1, false);
        public static readonly FieldType Int = new FieldType(-1, false);
        public static readonly FieldType Long = new FieldType(-1, false);
        public static readonly FieldType Bool = new FieldType(1, false);
        public static readonly FieldType Float = new FieldType(4, false);
        public static readonly FieldType Double = new FieldType(8, false);
        public static readonly FieldType String = new FieldType(-1, true);
        public static readonly FieldType Bytes = new FieldType(-1, true);        
        public static IEnumerable<FieldType> Values
        {
            get
            {
                yield return Reference;
                yield return Int;
                yield return Long;
                yield return Bool;
                yield return Float;
                yield return Double;
                yield return String;
                yield return Bytes;
            }
        }
    }
}
