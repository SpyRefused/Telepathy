using System;

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
        public string[] FieldPaths;
    }
}
