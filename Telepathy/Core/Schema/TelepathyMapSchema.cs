﻿using System.IO;
using System.Text;
using Telepathy.Core.Index.Key;

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

        public override void WriteTo(Stream stream)
        {            
            var streamWriter = new StreamWriter(stream);

            Stre
            if (HashKey != null)
                
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
            return string.Equals(KeyType, other.KeyType) && string.Equals(ValueType, other.ValueType) &&
                   HashKey.Equals(other.HashKey) && KeyTypeState.Equals(other.KeyTypeState) &&
                   ValueTypeState.Equals(other.ValueTypeState);
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
                if (HashKey.FieldPaths.Length > 0)
                {
                    builder.Append(HashKey.FieldPaths[0]);
                    for (var i = 1; i < HashKey.FieldPaths.Length; i++)
                    {
                        builder.Append(", ").Append(HashKey.FieldPaths[i]);
                    }
                }
                builder.Append(")");
            }

            builder.Append(";");
            return builder.ToString();
        }
    }
}