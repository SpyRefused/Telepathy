using System;
using Telepathy.Core.Schema;

namespace Telepathy.Core.Index.Key
{
    public class PrimaryKey
    {        
        public PrimaryKey(string type, params string[] fieldPaths)
        {
            if (fieldPaths == null || fieldPaths.Length == 0) throw new ArgumentException("fieldPaths can't not be null or empty");

            Type = type;
            FieldPaths = fieldPaths;
        }

        public string Type { get; }
        public readonly string[] FieldPaths;

        public FieldType GetFieldType(HollowDataset dataset, int fieldPathIdx)
        {
            return GetFieldType(dataset, Type, FieldPaths[fieldPathIdx]);
        }

        public int[] GetFieldPathIndex(HollowDataset dataset, int fieldPathIdx)
        {
            return GetFieldPathIndex(dataset, Type, FieldPaths[fieldPathIdx]);
        }

        public static FieldType GetFieldType(HollowDataset dataAccess, string type, string fieldPath)
        {
            TelepathyObjectSchema schema = (TelepathyObjectSchema)dataAccess.GetSchema(type);
            var pathIndexes = GetFieldPathIndex(dataAccess, type, fieldPath);

            for (var i = 0; i < pathIndexes.Length - 1; i++)
                schema = (TelepathyObjectSchema) dataAccess.GetSchema(schema.GetReferencedType(pathIndexes[i]));
            return schema.GetFieldType(pathIndexes[pathIndexes.Length - 1]);
        }

        public static int[] GetFieldPathIndex(HollowDataset dataset, string type, string fieldPath)
        {
            var paths = fieldPath.Split(new[] {"\\."}, StringSplitOptions.None);
            var pathIndexes = new int[paths.Length];
            var refType = type;

            for (var i = 0; i < paths.Length; i++)
            {
                TelepathyObjectSchema schema = (TelepathyObjectSchema)dataset.GetSchema(refType);
                try
                {
                    pathIndexes[i] = schema.GetPosition(paths[i]);
                    refType = schema.GetReferencedType(pathIndexes[i]);
                }
                catch (Exception)
                {
                    throw new Exception("Failed create path index for fieldPath=" + fieldPath + ", fieldName=" + paths[i] + " schema=" + schema.Name);
                }
            }

            return pathIndexes;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            
            result = prime * result + FieldPaths.GetHashCode();
            result = prime * result + (Type?.GetHashCode() ?? 0);
            return result;
        }

        public override bool Equals(object obj)
        {

            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            var other = (PrimaryKey) obj;
            if (!FieldPaths.Equals(other.FieldPaths))
                return false;
            if (Type == null)
            {
                if (other.Type != null)
                    return false;
            }
            else if (!Type.Equals(other.Type))
                return false;
            return true;
        }

        public override string ToString()
        {
            return "PrimaryKey [Type=" + Type + ", FieldPaths=" + FieldPaths + "]";
        }
    }
}
