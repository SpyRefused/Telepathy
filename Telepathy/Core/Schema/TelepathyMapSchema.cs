using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text;

namespace Telepathy.Core.Schema
{
    public class TelepathyMapSchema : TelepathySchema
    {
        public string KeyType { get; }
        public string ValueType { get; }
        public PrimaryKey HashKey { get; }
        public TelepathyTypeReadState KeyTypeState { get; set; }
        public TelepathyTypeReadState ValueTypeState { get; set; }

        public TelepathyMapSchema(string name, string keyType, string valueType, params string[] hashKeyFieldPaths) : base(name)
        {
            KeyType = keyType;
            ValueType = valueType;
            HashKey = hashKeyFieldPaths == null || hashKeyFieldPaths.Length == 0 ? null : new PrimaryKey(keyType, hashKeyFieldPaths);
        }

        public override SchemaType SchemaType => SchemaType.Map;

        public override void WriteTo(StreamWriter streamWriter)
        {            
            DataOutputStream dos = new Strea(os);

            if (getHashKey() != null)
                dos.write(SchemaType.MAP.getTypeIdWithPrimaryKey());  
            else
                dos.write(SchemaType.MAP.getTypeId());

            dos.writeUTF(getName());
            dos.writeUTF(getKeyType());
            dos.writeUTF(getValueType());

            if (getHashKey() != null)
            {
                VarInt.writeVInt(dos, getHashKey().numFields());
                for (int i = 0; i < getHashKey().numFields(); i++)
                {
                    dos.writeUTF(getHashKey().getFieldPath(i));
                }
            }
        }

        public override bool Equals(object other)
        {
            if (!(other is TelepathyMapSchema))
                return false;

            var otherSchema = (TelepathyMapSchema) other;

            if (!Name.Equals(otherSchema.Name))
                return false;

            if (!KeyType.Equals(otherSchema.KeyType))
                return false;

            return ValueType.Equals(otherSchema.ValueType) && HashKey.Equals(otherSchema.HashKey);
        }

        protected bool Equals(TelepathyMapSchema other)
        {
            return string.Equals(KeyType, other.KeyType) && string.Equals(ValueType, other.ValueType) && HashKey.Equals(other.HashKey) && KeyTypeState.Equals(other.KeyTypeState) && ValueTypeState.Equals(other.ValueTypeState);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = KeyType?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (ValueType?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ HashKey.GetHashCode();
                hashCode = (hashCode * 397) ^ KeyTypeState.GetHashCode();
                hashCode = (hashCode * 397) ^ ValueTypeState.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(Name);
            builder.Append(" Map<").Append(KeyType).Append(",").Append(ValueType).Append(">");

            if (HashKey != null)
            {
                builder.Append(" @HashKey(");
                if (HashKey.numFields() > 0)
                {
                    builder.Append(HashKey.FieldPath(0));
                    for (int i = 1; i < HashKey.NumFields(); i++)
                    {
                        builder.Append(", ").Append(HashKey.FieldPath(i));
                    }
                }
                builder.Append(")");
            }

            builder.Append(";");
            return builder.ToString();
        }
    }
}