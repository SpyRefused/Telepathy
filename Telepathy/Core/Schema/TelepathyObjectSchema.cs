using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telepathy.Core.Index.Key;

namespace Telepathy.Core.Schema
{
    public class TelepathyObjectSchema : TelepathySchema
    {
        private readonly IDictionary<string, int?> _nameFieldIndexLookup;
        private readonly string[] _fieldNames;
        private readonly FieldType[] _fieldTypes;
        protected readonly string[] ReferencedTypes;
        private readonly TelepathyTypeReadState[] _referencedFieldTypeStates;

        public TelepathyObjectSchema(string schemaName, int numFields, params string[] keyFieldPaths) : this(schemaName, numFields, keyFieldPaths == null || keyFieldPaths.Length == 0 ? null : new PrimaryKey(schemaName, keyFieldPaths))
        {}

        public TelepathyObjectSchema(string schemaName, int numFields, PrimaryKey primaryKey) 
        : base(schemaName)
        {            
            _nameFieldIndexLookup = new Dictionary<string, int>(numFields);
            _fieldNames = new string[numFields];
            _fieldTypes = new FieldType[numFields];
            ReferencedTypes = new string[numFields];
            _referencedFieldTypeStates = new TelepathyTypeReadState[numFields];
            PrimaryKey = primaryKey;
        }

        public int NumFields { get; private set; }

        public PrimaryKey PrimaryKey { get; }

        public int AddField(string fieldName, FieldType fieldType)
        {
            return AddField(fieldName, fieldType, null, null);
        }

        public int AddField(string fieldName, FieldType fieldType, string referencedType)
        {
            return AddField(fieldName, fieldType, referencedType, null);
        }

        public int AddField(string fieldName, FieldType fieldType, string referencedType, TelepathyTypeReadState referencedTypeState)
        {
            if (fieldType == FieldType.Reference && referencedType == null)
                throw new Exception("When adding a Reference field to a schema, the referenced type must be provided.  Check type: " + Name + " field: " + fieldName);

            _fieldNames[NumFields] = fieldName;
            _fieldTypes[NumFields] = fieldType;
            ReferencedTypes[NumFields] = referencedType;

            _nameFieldIndexLookup.Add(fieldName, NumFields);

            NumFields++;

            return NumFields - 1;
        }

        public int GetPosition(string fieldName)
        {
            var index = _nameFieldIndexLookup[fieldName];
            if (index == null) return - 1;
            return index.Value;
        }

        public string GetFieldName(int fieldPosition)
        {
            return _fieldNames[fieldPosition];
        }

        public FieldType GetFieldType(string fieldName)
        {
            var fieldPosition = GetPosition(fieldName);
            return fieldPosition == -1 ? null : GetFieldType(fieldPosition);
        }

        public FieldType GetFieldType(int fieldPosition)
        {
            return _fieldTypes[fieldPosition];
        }

        public string GetReferencedType(String fieldName)
        {
            var fieldPosition = GetPosition(fieldName);
            return fieldPosition == -1 ? null : GetReferencedType(fieldPosition);
        }

        public string GetReferencedType(int fieldPosition)
        {
            return ReferencedTypes[fieldPosition];
        }
        public TelepathyTypeReadState GetReferencedTypeState(int fieldPosition)
        {
            return _referencedFieldTypeStates[fieldPosition];
        }

        public TelepathyObjectSchema FindCommonSchema(TelepathyObjectSchema otherSchema)
        {
            if (!Name.Equals(otherSchema.Name))
            {
                throw new ArgumentException("Cannot find common schema of two schemas with different names!");
            }

            var commonFields = _fieldNames.Count(fieldName => otherSchema.GetPosition(fieldName) != -1);

            var primaryKey = PrimaryKey.Equals(otherSchema.PrimaryKey) ? PrimaryKey : null;
            
            TelepathyObjectSchema commonSchema = new TelepathyObjectSchema(Name, commonFields, primaryKey);

            for (int i = 0; i < _fieldNames.Length; i++)
            {
                int otherFieldIndex = otherSchema.GetPosition(_fieldNames[i]);
                if (otherFieldIndex != -1)
                {
                    if (_fieldTypes[i] != otherSchema.GetFieldType(otherFieldIndex)
                            || !ReferenceEquals(ReferencedTypes[i], otherSchema.GetReferencedType(otherFieldIndex)))
                    {
                        throw new ArgumentException("No common schema exists for " + Name + ": field " + _fieldNames[i] + " has unmatched types");
                    }                    
                    commonSchema.AddField(_fieldNames[i], _fieldTypes[i], ReferencedTypes[i]);
                }
            }

            return commonSchema;
        }
        public TelepathyObjectSchema FindUnionSchema(TelepathyObjectSchema otherSchema)
        {
            if (!Name.Equals(otherSchema.Name))
            {
                throw new ArgumentException("Cannot find common schema of two schemas with different names!");
            }

            int totalFields = otherSchema.NumFields;

            foreach (var fieldName in _fieldNames)
            {            
                if (otherSchema.GetPosition(fieldName) == -1)
                    totalFields++;
            }
            

            var primaryKey = PrimaryKey.Equals(otherSchema.PrimaryKey) ? PrimaryKey : null;
            var unionSchema = new TelepathyObjectSchema(Name, totalFields, primaryKey);

            for (int i = 0; i < _fieldNames.Length; i++)
            {
                unionSchema.AddField(_fieldNames[i], _fieldTypes[i], ReferencedTypes[i]);
            }

            for (int i = 0; i < otherSchema.NumFields; i++)
            {
                if (GetPosition(otherSchema.GetFieldName(i)) == -1)
                {
                    unionSchema.AddField(otherSchema.GetFieldName(i), otherSchema.GetFieldType(i),
                        otherSchema.GetReferencedType(i));
                }
            }

            return unionSchema;
        }

        public TelepathyObjectSchema FilterSchema(TelepathyFilterConfig config)
        {
            var typeConfig = config.GetObjectTypeConfig(Name);

            int includedFields = 0;

            for (int i = 0; i < NumFields; i++)
            {
                if (typeConfig.includesField(GetFieldName(i)))
                    includedFields++;
            }

            var filteredSchema = new TelepathyObjectSchema(Name, includedFields, PrimaryKey);

            for (var i = 0; i < NumFields; i++)
            {
                if (typeConfig.includesField(GetFieldName(i)))
                    filteredSchema.AddField(GetFieldName(i), GetFieldType(i), GetReferencedType(i));
            }

            return filteredSchema;
        }

        public override SchemaType GetSchemaType()
        {
            return SchemaType.Object;
        }

        public override bool Equals(object other)
        {
            if (!(other is TelepathyObjectSchema))
                return false;
            var otherSchema = (TelepathyObjectSchema)other;
            if (!Name.Equals(otherSchema.Name))
                return false;
            if (otherSchema.NumFields != NumFields)
                return false;

            if (!PrimaryKey.Equals(otherSchema.PrimaryKey))
                return false;

            for (var i = 0; i < NumFields; i++)
            {
                if (GetFieldType(i) != otherSchema.GetFieldType(i))
                    return false;
                if (!GetFieldName(i).Equals(otherSchema.GetFieldName(i)))
                    return false;
            }

            return true;
        }
        
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(Name);
            if (PrimaryKey != null)
            {
                builder.Append(" @PrimaryKey(");
                if (PrimaryKey.FieldPaths.Length > 0)
                {
                    builder.Append(PrimaryKey.FieldPaths[0]);
                    for (var i = 1; i < PrimaryKey.FieldPaths.Length; i++)
                    {
                        builder.Append(", ").Append(PrimaryKey.FieldPaths[i]);
                    }
                }
                builder.Append(")");
            }

            builder.Append(" {\n");
            for (var i = 0; i < NumFields; i++)
            {
                builder.Append("\t");

                builder.Append(GetFieldType(i) == FieldType.Reference
                    ? GetReferencedType(i)
                    : GetFieldType(i).ToString().ToLower());
                builder.Append(" ").Append(GetFieldName(i)).Append(";\n");
            }
            builder.Append("}");

            return builder.ToString();
        }


        public override void WriteTo(OutputStream os)
        {
            DataOutputStream dos = new DataOutputStream(os);

            dos.write(PrimaryKey != null ? SchemaType.Object.TypeIdWithPrimaryKey : SchemaType.Object.TypeId);

            dos.writeUTF(getName());
            if (primaryKey != null)
            {
                VarInt.writeVInt(dos, primaryKey.numFields());
                for (int i = 0; i < primaryKey.numFields(); i++)
                {
                    dos.writeUTF(primaryKey.getFieldPath(i));
                }
            }

            dos.writeShort(size);
            for (int i = 0; i < size; i++)
            {
                dos.writeUTF(fieldNames[i]);
                dos.writeUTF(fieldTypes[i].name());
                if (fieldTypes[i] == FieldType.REFERENCE)
                    dos.writeUTF(referencedTypes[i]);
            }

        }
    }

}
