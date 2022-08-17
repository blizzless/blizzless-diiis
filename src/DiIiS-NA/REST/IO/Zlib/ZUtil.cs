//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.REST.IO.Zlib
{
    public static partial class ZLib
    {
        private const int OS_CODE = 0x0b;

        static readonly string[] z_errmsg = new string[9]
        {
            "need dictionary",		// Z_NEED_DICT       2
			"stream end",			// Z_STREAM_END      1
			"",						// Z_OK              0
			"file error",			// Z_ERRNO          -1
			"stream error",			// Z_STREAM_ERROR   -2
			"data error",			// Z_DATA_ERROR     -3
			"insufficient memory",	// Z_MEM_ERROR      -4
			"buffer error",			// Z_BUF_ERROR      -5
			"incompatible version"	// Z_VERSION_ERROR  -6
		};

        // =========================================================================

        // The application can compare zlibVersion and ZLIB_VERSION for consistency.
        // If the first character differs, the library code actually used is
        // not compatible with the zlib.h header file used by the application.
        // This check is automatically made by deflateInit and inflateInit.

        public static string zlibVersion()
        {
            return ZLIB_VERSION;
        }

        // =========================================================================

        // Return flags indicating compile-time options.
        //
        //    Type sizes, two bits each, 00 = 16 bits, 01 = 32, 10 = 64, 11 = other:
        //     1.0: size of ulong
        //     3.2: size of uint
        //     5.4: size of void* (pointer)
        //     7.6: size of int
        //
        //Compiler, assembler, and debug options:
        // (8: DEBUG)
        // (9: ASMV or ASMINF -- use ASM code [not used in this port])
        // (10: ZLIB_WINAPI -- exported functions use the WINAPI calling convention [not used in this port])
        // 11: 0 (reserved)
        //
        //One-time table building (smaller code, but not thread-safe if true):
        // (12: BUILDFIXED -- build static block decoding tables when needed [not used in this port])
        // (13: DYNAMIC_CRC_TABLE -- build CRC calculation tables when needed [not used in this port])
        // 14,15: 0 (reserved)
        //
        //Library content (indicates missing functionality):
        // 16: NO_GZCOMPRESS -- gz* functions cannot compress (to avoid linking deflate code when not needed)
        // 17: NO_GZIP -- deflate can't write gzip streams, and inflate can't detect and decode gzip streams (to avoid linking crc code)
        // 18-19: 0 (reserved)
        //
        //Operation variations (changes in library functionality):
        // (20: PKZIP_BUG_WORKAROUND -- slightly more permissive inflate [not used in this port])
        // (21: FASTEST -- deflate algorithm with only one, lowest compression level [not used in this port])
        // 22,23: 0 (reserved)
        //
        //The sprintf variant used by gzprintf (zero is best):
        // (24: 0 = vs*, 1 = s* -- 1 means limited to 20 arguments after the format [not used in this port])
        // (25: 0 = *nprintf, 1 = *printf -- 1 means gzprintf() not secure! [not used in this port])
        // (26: 0 = returns value, 1 = void -- 1 means inferred string length returned [not used in this port])
        //
        //Remainder:
        // 27-31: 0 (reserved)

        public static uint zlibCompileFlags()
        {
            uint flags = 2;
            flags += 1 << 2;

            switch (IntPtr.Size)
            {
                case 4: flags += 1 << 4; break;
                case 8: flags += 2 << 4; break;
                default: flags += 3 << 4; break;
            }

            flags += 1 << 6;

            return flags;
        }

        // =========================================================================

        // exported to allow conversion of error code to string for compress() and uncompress()
        public static string zError(int err)
        {
            if (err < -6 || err > 2) throw new ArgumentOutOfRangeException("err", "must be -6<=err<=2");

            return z_errmsg[2 - err];
        }
    }
}
