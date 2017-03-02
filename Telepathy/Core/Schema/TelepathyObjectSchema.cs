using System;
using System.Collections.Generic;
using System.Linq;
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


            PrimaryKey primaryKey = isNullableObjectEquals(this.primaryKey, otherSchema.getPrimaryKey()) ? this.primaryKey : null;
            HollowObjectSchema commonSchema = new HollowObjectSchema(getName(), commonFields, primaryKey);

            for (int i = 0; i < fieldNames.length; i++)
            {
                int otherFieldIndex = otherSchema.getPosition(fieldNames[i]);
                if (otherFieldIndex != -1)
                {
                    if (fieldTypes[i] != otherSchema.getFieldType(otherFieldIndex)
                            || !referencedTypesEqual(referencedTypes[i], otherSchema.getReferencedType(otherFieldIndex)))
                    {
                        throw new IllegalArgumentException("No common schema exists for " + getName() + ": field " + fieldNames[i] + " has unmatched types");
                    }

                    commonSchema.addField(fieldNames[i], fieldTypes[i], referencedTypes[i]);
                }
            }

            return commonSchema;
        }
    }
}
