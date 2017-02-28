using Telepathy.Core.Index.Key;

namespace Telepathy.Core.Schema
{
    public class TelepathyObjectSchema : TelepathySchema
    {
        private readonly Map<string, int> _nameFieldIndexLookup;
        private readonly string fieldNames[];
    private readonly FieldType fieldTypes[];
    protected readonly string referencedTypes[];
    private readonly HollowTypeReadState referencedFieldTypeStates[]; 
    private readonly PrimaryKey primaryKey;
    }
}
