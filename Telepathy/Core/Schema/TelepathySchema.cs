using System.IO;
using System.Text;

namespace Telepathy.Core.Schema
{
    public abstract class TelepathySchema
    {
        protected TelepathySchema(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public abstract SchemaType SchemaType { get; }
        public abstract void WriteTo(Stream stream); //Throws IOException
        public static TelepathySchema WithoutKeys(TelepathySchema schema)
        {
            switch (schema.SchemaType.TypeId)
            {
                case SchemaType.SetSchemaTypeId:
                    var setSchema = schema as TelepathySetSchema;        
                    return setSchema;

                case SchemaType.MapSchemaTypeId:
                    var mapSchema = schema as TelepathyMapSchema;
                    if (mapSchema?.HashKey != null)
                        mapSchema = new TelepathyMapSchema(mapSchema.Name, mapSchema.KeyType, mapSchema.ValueType);
                    return mapSchema;

                default:
                    return schema;
            }
        }

        public static TelepathySchema ReadFrom(Stream stream)
        {
            var streamReader = new StreamReader(stream, new UTF8Encoding());
            var schemaTypeId = streamReader.Read();

            var schemaName = streamReader.ReadLine();

            switch (SchemaType.TypeId(schemaTypeId))
            {
                case OBJECT:
                    return readObjectSchemaFrom(dis, schemaName, SchemaType.HasKey(schemaTypeId));
                case LIST:
                    return readListSchemaFrom(dis, schemaName);
                case SET:
                    return readSetSchemaFrom(dis, schemaName, SchemaType.hasKey(schemaTypeId));
                case MAP:
                    return readMapSchemaFrom(dis, schemaName, SchemaType.hasKey(schemaTypeId));
            }

            throw new IOException();
        }



    }
}
