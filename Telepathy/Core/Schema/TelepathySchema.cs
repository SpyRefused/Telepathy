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
                    if (setSchema?.HashKey != null)
                        setSchema = TelepathySetSchema(setSchema.Name, setSchema.ElementType);
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
                    return ReadObjectSchemaFrom(dis, schemaName, SchemaType.HasKey(schemaTypeId));
                case LIST:
                    return ReadObjectSchemaFrom(dis, schemaName);
                case SET:
                    return ReadObjectSchemaFrom(dis, schemaName, SchemaType.hasKey(schemaTypeId));
                case MAP:
                    return ReadObjectSchemaFrom(dis, schemaName, SchemaType.hasKey(schemaTypeId));
            }

            throw new IOException();
        }


        private static TelepathyObjectSchema ReadObjectSchemaFrom(Stream stream, string schemaName, bool hasPrimaryKey)
        {
            string[] keyFieldPaths = null;
            
            var streamReader = new StreamReader(stream);

            if (hasPrimaryKey)
            {
                stream.re
                int numFields = VarInt.readVInt(is);
                keyFieldPaths = new string[numFields];
                for (int i = 0; i < numFields; i++)
                {
                    keyFieldPaths[i] = is.readUTF();
                }
            }

            int numFields = is.readShort();
            TelepathyObjectSchema schema = new HollowObjectSchema(schemaName, numFields, keyFieldPaths);

            for (int i = 0; i < numFields; i++)
            {
                String fieldName = is.readUTF();
                FieldType fieldType = FieldType.valueOf(is.readUTF());
                String referencedType = fieldType == FieldType.REFERENCE ? is.readUTF() : null;
                schema.addField(fieldName, fieldType, referencedType);
            }

            return schema;
        }
    }
}
