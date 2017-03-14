using System.Collections.Generic;
using System.IO;

namespace Telepathy.Core.Schema
{
    public abstract class TelepathySchema
    {
        public TelepathySchema(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public abstract SchemaType SchemaType { get; }

        public abstract void WriteTo(Stream stream);

        public static TelepathySchema WithoutKeys(TelepathySchema schema)
        {
            switch (schema.SchemaType.TypeId)
            {
                case SchemaType.SetSchemaTypeId:
                    var setSchema = schema as TelepathySetSchema;        
                    return setSchema;
                case Schema.SchemaType.Map.TypeId:
                    var mapSchema = schema as TelepathyMapSchema;
                    if (mapSchema.HashKey() != null)
                        mapSchema = new HollowMapSchema(mapSchema.getName(), mapSchema.getKeyType(), mapSchema.getValueType());
                    return mapSchema;
                default:
                    return schema;
            }
        }


    }
}
