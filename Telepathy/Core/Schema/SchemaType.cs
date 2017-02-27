using System;
using System.Collections.Generic;

namespace Telepathy.Core.Schema
{
    public enum SchemaTypeEnum
    {
        ObjectSchemaTypeId = 0,
        SetSchemaTypeId = 1,
        ListSchemaTypeId = 2,
        MapSchemaTypeId = 4
    }

    public class SchemaType
    {
        public const int ObjectSchemaTypeId = 0;
        public const int SetSchemaTypeId = 1;
        public const int ListSchemaTypeId = 2;
        public const int MapSchemaTypeId = 3;

        public int TypeId { get; private set; }
        public int TypeIdWithPrimaryKey { get; private set; }
        public SchemaTypeEnum Type{get; private set; }
        public SchemaType(int typeId, int typeIdWithPrimaryKey)
        {
            TypeId = typeId;
            TypeIdWithPrimaryKey = typeIdWithPrimaryKey;

            switch (typeId)
            {
                case ObjectSchemaTypeId:
                    Type = SchemaTypeEnum.ObjectSchemaTypeId;
                    break;
                case SetSchemaTypeId:
                    Type = SchemaTypeEnum.SetSchemaTypeId;
                    break;
                case ListSchemaTypeId:
                    Type = SchemaTypeEnum.ListSchemaTypeId;
                    break;
                case MapSchemaTypeId:
                    Type = SchemaTypeEnum.MapSchemaTypeId;
                    break;
            }
        }

        public static readonly SchemaType Object = new SchemaType(0, 6);
        public static readonly SchemaType Set = new SchemaType(1, 4);
        public static readonly SchemaType List = new SchemaType(2, -1);
        public static readonly SchemaType Map = new SchemaType(3, 5);

        public static IEnumerable<SchemaType> Values
        {
            get
            {
                yield return Object;
                yield return Set;
                yield return List;
                yield return Map;
            }
        }

        public static SchemaType FromTypeId(int id)
        {
            switch (id)
            {
                case 0:
                case 6:
                    return Object;
                case 1:
                case 4:
                    return Set;
                case 2:
                    return List;
                case 3:
                case 5:
                    return Map;
            }
            throw new ArgumentException("Cannot recognize HollowSchema type id " + id);
        }
        public static bool HasKey(int typeId)
        {
            return typeId == 4 || typeId == 5 || typeId == 6;
        }
    }
}
