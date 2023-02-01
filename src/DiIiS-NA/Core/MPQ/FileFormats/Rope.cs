//Blizzless Project 2022
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.Core.MPQ.FileFormats.Types;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Rope)]
    public class Rope : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public float F0 { get; private set; }
        /*
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)RopeDefinition_Fields, &byte_D6A4A3, DT_INT, 12, 0LL, 0x80001u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[1], &byte_D6A4A3, DT_INT, 16, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[2], &byte_D6A4A3,DT_FLOAT, 20, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[3], &byte_D6A4A3, DT_MASS, 24, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[4], &byte_D6A4A3, DT_FLOAT, 28, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[5], &byte_D6A4A3, DT_MASS, 32, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[6], &byte_D6A4A3, DT_FLOAT, 36, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[7], &byte_D6A4A3, DT_FLOAT, 40, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[8], &byte_D6A4A3, DT_UBERMATERIAL, 48, 0LL, 1u);
          (&RopeDefinition_Fields[9], &v3, sizeof(const TypeDescriptorField));			(&v3, &byte_D6A4A3, DT_SNO, 152, &byte_D6A4A3, 0x1Cu, 0, -1); 
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[10], &byte_D6A4A3, DT_TIME, 156, (const void *)(unsigned int)&loc_3C, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[11], &byte_D6A4A3, DT_TIME, 160, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[12], &byte_D6A4A3, DT_TIME, 164, (const void *)((unsigned int)&dword_1C + 2), 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[13], &byte_D6A4A3, DT_TIME, 168, 0LL, 1u); 
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[14], &byte_D6A4A3, DT_COLOR_PATH, 176, &cgpathNullColorPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[15], &byte_D6A4A3, DT_COLOR_PATH, 224, &cgpathNullColorPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[16], &byte_D6A4A3, DT_FLOAT_PATH, 272, &cgpathUnitFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[17], &byte_D6A4A3, DT_FLOAT_PATH, 320, &cgpathUnitFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[18], &byte_D6A4A3, DT_FLOAT_PATH, 368, &cgpathUnitFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[19], &byte_D6A4A3, DT_FLOAT_PATH, 416, &cgpathUnitFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[20], &byte_D6A4A3, DT_FLOAT_PATH, 464, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[21], &byte_D6A4A3, DT_FLOAT_PATH, 512, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[22], &byte_D6A4A3, DT_FLOAT_PATH, 560, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[23], &byte_D6A4A3, DT_FLOAT_PATH, 608, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[24], &byte_D6A4A3, DT_FLOAT_PATH, 656, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[25], &byte_D6A4A3, DT_FLOAT_PATH, 704, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[26], &byte_D6A4A3, DT_VECTOR_PATH, 752, &cgpathNullVectorPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[27], &byte_D6A4A3, DT_VELOCITY_PATH, 800, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[28], &byte_D6A4A3, DT_FLOAT, 848, (const void *)0x3C23D70A, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[29], &byte_D6A4A3, DT_FLOAT, 852, 0LL, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[30], &byte_D6A4A3, DT_FLOAT_PATH, 856, &cgpathUnitFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[31], &byte_D6A4A3, DT_FLOAT_PATH, 904, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[32], &byte_D6A4A3, DT_FLOAT_PATH, 952, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[33], &byte_D6A4A3, DT_FLOAT_PATH, 1000, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[34], &byte_D6A4A3, DT_FLOAT_PATH, 1048, &cgpathNullFloatPath, 1u);
          TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[35], &byte_D6A4A3, DT_SERIALIZEDATA, 1096, 0LL, 0);
            memcpy(&RopeDefinition_Fields[36], &v2, sizeof(const TypeDescriptorField));		( &v2, &byte_D6A4A3, DT_MSGTRIGGEREDEVENT, 1112, 0x21u, -16);
            memcpy(&RopeDefinition_Fields[37], &v1, sizeof(const TypeDescriptorField)); 	( &v1, &byte_D6A4A3, 1104, 0x10000u, DT_MSGTRIGGEREDEVENT, -8);
          END - TypeDescriptorField::TypeDescriptorField( (TypeDescriptorField *)&RopeDefinition_Fields[38], 0LL, DT_NULL, 1120, 0LL, 0);
        //*/

        public Rope(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            I0 = stream.ReadValueS32();
            F0 = stream.ReadValueF32();

            stream.Close();
        }
    }
}
