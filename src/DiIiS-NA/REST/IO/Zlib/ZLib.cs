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
        // Maximum value for memLevel in deflateInit2
        private const int MAX_MEM_LEVEL = 9;

        // Maximum value for windowBits in deflateInit2 and inflateInit2.
        private const int MAX_WBITS = 15; // 32K LZ77 window
        private const int DEF_WBITS = MAX_WBITS; // default windowBits for decompression. MAX_WBITS is for compression only

        // The memory requirements for deflate are (in bytes):
        //    (1 << (windowBits+2)) +  (1 << (memLevel+9))
        // that is: 128K for windowBits=15  +  128K for memLevel = 8  (default values)
        // plus a few kilobytes for small objects. For example, if you want to reduce
        // the default memory requirements from 256K to 128K, compile with
        //    make CFLAGS="-O -DMAX_WBITS=14 -DMAX_MEM_LEVEL=7"
        // Of course this will generally degrade compression (there's no free lunch).

        // The memory requirements for inflate are (in bytes) 1 << windowBits
        // that is, 32K for windowBits=15 (default value) plus a few kilobytes
        // for small objects.

        private const int DEF_MEM_LEVEL = 8; // default memLevel

        // The three kinds of block type
        private const int STORED_BLOCK = 0;
        private const int STATIC_TREES = 1;
        private const int DYN_TREES = 2;

        // The minimum and maximum match lengths
        private const int MIN_MATCH = 3;
        private const int MAX_MATCH = 258;

        public const string ZLIB_VERSION = "1.2.5";
        public const uint ZLIB_VERNUM = 0x1250;

        // The 'zlib' compression library provides in-memory compression and
        // decompression functions, including integrity checks of the uncompressed
        // data. This version of the library supports only one compression method
        // (deflation) but other algorithms will be added later and will have the same
        // stream interface.
        //
        //  Compression can be done in a single step if the buffers are large enough,
        // or can be done by repeated calls of the compression function. In the latter
        //  case, the application must provide more input and/or consume the output
        // (providing more output space) before each call.
        //
        //  The compressed data format used by default by the in-memory functions is
        // the zlib format, which is a zlib wrapper documented in RFC 1950, wrapped
        // around a deflate stream, which is itself documented in RFC 1951.
        //
        //  The zlib format was designed to be compact and fast for use in memory
        // and on communications channels. 
        //
        //  The library does not install any signal handler. The decoder checks
        // the consistency of the compressed data, so the library should never crash
        // even in case of corrupted input.

        public class z_stream
        {
            public byte[] in_buf;   // input buffer
            public uint next_in;    // next input byte index
            public uint avail_in;   // number of bytes available at next_in
            public uint total_in;   // total nb of input bytes read so far

            public byte[] out_buf;  // output buffer
            public int next_out;    // next output byte should be put there
            public uint avail_out;  // remaining free space at next_out
            public uint total_out;  // total nb of bytes output so far

            public string msg;      // last error message, NULL if no error
            public object state;    // not visible by applications

            public uint adler;      // adler32 value of the uncompressed data

            public void CopyTo(z_stream s)
            {
                s.adler = adler;
                s.avail_in = avail_in;
                s.avail_out = avail_out;
                s.in_buf = in_buf;
                s.msg = msg;
                s.next_in = next_in;
                s.next_out = next_out;
                s.out_buf = out_buf;
                s.state = state;
                s.total_in = total_in;
                s.total_out = total_out;
            }
        }

        // gzip header information passed to and from zlib routines.  See RFC 1952
        // for more details on the meanings of these fields.
        public class gz_header
        {
            public int text;            // true if compressed data believed to be text
            public uint time;           // modification time
            public int xflags;          // extra flags (not used when writing a gzip file)
            public int os;              // operating system
            public byte[] extra;        // pointer to extra field or Z_NULL if none
            public uint extra_len;      // extra field length (valid if extra != Z_NULL)
            public uint extra_max;      // space at extra (only when reading header)
            public byte[] name;         // pointer to zero-terminated file name or Z_NULL
            public uint name_max;       // space at name (only when reading header)
            public byte[] comment;      // pointer to zero-terminated comment or Z_NULL
            public uint comm_max;       // space at comment (only when reading header)
            public int hcrc;            // true if there was or will be a header crc
            public int done;            // true when done reading gzip header (not used when writing a gzip file)
        }

        // The application must update next_in and avail_in when avail_in has
        // dropped to zero. It must update next_out and avail_out when avail_out
        // has dropped to zero. All other fields are set by the
        // compression library and must not be updated by the application.
        //
        // The fields total_in and total_out can be used for statistics or
        // progress reports. After compression, total_in holds the total size of
        // the uncompressed data and may be saved for use in the decompressor
        // (particularly if the decompressor wants to decompress everything in
        // a single step).

        // constants

        // Allowed flush values; see deflate() and inflate() below for details
        public const int Z_NO_FLUSH = 0;
        public const int Z_PARTIAL_FLUSH = 1;
        public const int Z_SYNC_FLUSH = 2;
        public const int Z_FULL_FLUSH = 3;
        public const int Z_FINISH = 4;
        public const int Z_BLOCK = 5;
        public const int Z_TREES = 6;

        // Return codes for the compression/decompression functions. Negative
        // values are errors, positive values are used for special but normal events.
        public const int Z_OK = 0;
        public const int Z_STREAM_END = 1;
        public const int Z_NEED_DICT = 2;
        public const int Z_ERRNO = -1;
        public const int Z_STREAM_ERROR = -2;
        public const int Z_DATA_ERROR = -3;
        public const int Z_MEM_ERROR = -4;
        public const int Z_BUF_ERROR = -5;
        public const int Z_VERSION_ERROR = -6;

        // compression levels
        public const int Z_NO_COMPRESSION = 0;
        public const int Z_BEST_SPEED = 1;
        public const int Z_BEST_COMPRESSION = 9;
        public const int Z_DEFAULT_COMPRESSION = (-1);

        // compression strategy; see deflateInit2() below for details
        public const int Z_FILTERED = 1;
        public const int Z_HUFFMAN_ONLY = 2;
        public const int Z_RLE = 3;
        public const int Z_FIXED = 4;
        public const int Z_DEFAULT_STRATEGY = 0;

        // The deflate compression method (the only one supported in this version)
        public const int Z_DEFLATED = 8;

        public const string zlib_version = ZLIB_VERSION; // for compatibility with versions < 1.0.2
    }
}
