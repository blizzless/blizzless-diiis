using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.IO.Zlib
{
    public static partial class ZLib
    {
        #region deflate.h
        // ===========================================================================
        // Internal compression state.

        // number of length codes, not counting the special END_BLOCK code
        private const int LENGTH_CODES = 29;

        // number of literal bytes 0..255
        private const int LITERALS = 256;

        // number of Literal or Length codes, including the END_BLOCK code
        private const int L_CODES = LITERALS + 1 + LENGTH_CODES;

        // number of distance codes
        private const int D_CODES = 30;

        // number of codes used to transfer the bit lengths
        private const int BL_CODES = 19;

        // maximum heap size
        private const int HEAP_SIZE = 2 * L_CODES + 1;

        // All codes must not exceed MAX_BITS bits
        private const int MAX_BITS = 15;

        // Stream status
        private const int INIT_STATE = 42;
        private const int EXTRA_STATE = 69;
        private const int NAME_STATE = 73;
        private const int COMMENT_STATE = 91;
        private const int HCRC_STATE = 103;
        private const int BUSY_STATE = 113;
        private const int FINISH_STATE = 666;

        // Data structure describing a single value and its code string.
        struct ct_data
        {
            ushort freq;
            public ushort Freq { get => freq;
                set => freq = value;
            } // frequency count
            public ushort Code { get => freq;
                set => freq = value;
            }   // bit string

            ushort dad;
            public ushort Dad { get => dad;
                set => dad = value;
            }      // father node in Huffman tree
            public ushort Len { get => dad;
                set => dad = value;
            }      // length of bit string

            public ct_data(ushort freq, ushort dad)
            {
                this.freq = freq;
                this.dad = dad;
            }

            public ct_data(ct_data data)
            {
                freq = data.freq;
                dad = data.dad;
            }
        }

        struct tree_desc
        {
            public ct_data[] dyn_tree;          // the dynamic tree
            public int max_code;                // largest code with non zero frequency
            public static_tree_desc stat_desc;  // the corresponding static tree

            public tree_desc(tree_desc desc)
            {
                dyn_tree = desc.dyn_tree;
                max_code = desc.max_code;
                stat_desc = desc.stat_desc;
            }
        }

        class deflate_state //internal_state 
        {
            public z_stream strm;           // pointer back to this zlib stream
            public int status;              // as the name implies
            public byte[] pending_buf;      // output still pending
            public uint pending_buf_size;   // size of pending_buf

            public int pending_out;         // next pending byte to output to the stream

            public uint pending;            // nb of bytes in the pending buffer
            public int wrap;                // bit 0 true for zlib, bit 1 true for gzip
            public gz_header gzhead;        // gzip header information to write
            public uint gzindex;            // where in extra, name, or comment
            public byte method;             // STORED (for zip only) or DEFLATED
            public int last_flush;          // value of flush param for previous deflate call

            // used by deflate.c:
            public uint w_size; // LZ77 window size (32K by default)
            public uint w_bits; // log2(w_size)  (8..16)
            public uint w_mask; // w_size - 1

            // Sliding window. Input bytes are read into the second half of the window,
            // and move to the first half later to keep a dictionary of at least wSize
            // bytes. With this organization, matches are limited to a distance of
            // wSize-MAX_MATCH bytes, but this ensures that IO is always
            // performed with a length multiple of the block size. Also, it limits
            // the window size to 64K, which is quite useful on MSDOS.
            // To do: use the user input buffer as sliding window.
            public byte[] window;

            // Actual size of window: 2*wSize, except when the user input buffer
            // is directly used as sliding window.
            public uint window_size;

            // Link to older string with same hash index. To limit the size of this
            // array to 64K, this link is maintained only for the last 32K strings.
            // An index in this array is thus a window index modulo 32K.
            public ushort[] prev;

            public ushort[] head;   // Heads of the hash chains or NIL.

            public uint ins_h;      // hash index of string to be inserted
            public uint hash_size;  // number of elements in hash table
            public uint hash_bits;  // log2(hash_size)
            public uint hash_mask;  // hash_size-1

            // Number of bits by which ins_h must be shifted at each input
            // step. It must be such that after MIN_MATCH steps, the oldest
            // byte no longer takes part in the hash key, that is:
            //   hash_shift * MIN_MATCH >= hash_bits
            public uint hash_shift;

            // Window position at the beginning of the current output block. Gets
            // negative when the window is moved backwards.
            public int block_start;

            public uint match_length;   // length of best match
            public uint prev_match;     // previous match
            public int match_available; // set if previous match exists
            public uint strstart;       // start of string to insert
            public uint match_start;    // start of matching string
            public uint lookahead;      // number of valid bytes ahead in window

            // Length of the best match at previous step. Matches not greater than this
            // are discarded. This is used in the lazy match evaluation.
            public uint prev_length;

            // To speed up deflation, hash chains are never searched beyond this
            // length.  A higher limit improves compression ratio but degrades the speed.
            public uint max_chain_length;

            // Attempt to find a better match only when the current match is strictly
            // smaller than this value. This mechanism is used only for compression
            // levels >= 4.
            public uint max_lazy_match;

            // Insert new strings in the hash table only if the match length is not
            // greater than this length. This saves time but degrades compression.
            // max_insert_length is used only for compression levels <= 3.
            //#define max_insert_length max_lazy_match

            public int level;       // compression level (1..9)
            public int strategy;    // favor or force Huffman coding

            public uint good_match; // Use a faster search when the previous match is longer than this

            public int nice_match;  // Stop searching when current match exceeds this

            // used by trees.c:
            public ct_data[] dyn_ltree = new ct_data[HEAP_SIZE];        // literal and length tree
            public ct_data[] dyn_dtree = new ct_data[2 * D_CODES + 1];  // distance tree
            public ct_data[] bl_tree = new ct_data[2 * BL_CODES + 1];       // Huffman tree for bit lengths

            public tree_desc l_desc = new tree_desc();  // desc. for literal tree
            public tree_desc d_desc = new tree_desc();  // desc. for distance tree
            public tree_desc bl_desc = new tree_desc(); // desc. for bit length tree

            // number of codes at each bit length for an optimal tree
            public ushort[] bl_count = new ushort[MAX_BITS + 1];

            // The sons of heap[n] are heap[2*n] and heap[2*n+1]. heap[0] is not used.
            // The same heap array is used to build all trees.
            public int[] heap = new int[2 * L_CODES + 1];       // heap used to build the Huffman trees
            public int heap_len;    // number of elements in the heap
            public int heap_max;    // element of largest frequency

            // Depth of each subtree used as tie breaker for trees of equal frequency
            public byte[] depth = new byte[2 * L_CODES + 1];

            public byte[] l_buf;        // buffer for literals or lengths

            // Size of match buffer for literals/lengths.  There are 4 reasons for
            // limiting lit_bufsize to 64K:
            //   - frequencies can be kept in 16 bit counters
            //   - if compression is not successful for the first block, all input
            //     data is still in the window so we can still emit a stored block even
            //     when input comes from standard input.  (This can also be done for
            //     all blocks if lit_bufsize is not greater than 32K.)
            //   - if compression is not successful for a file smaller than 64K, we can
            //     even emit a stored file instead of a stored block (saving 5 bytes).
            //     This is applicable only for zip (not zlib).
            //   - creating new Huffman trees less frequently may not provide fast
            //     adaptation to changes in the input data statistics. (Take for
            //     example a binary file with poorly compressible code followed by
            //     a highly compressible string table.) Smaller buffer sizes give
            //     fast adaptation but have of course the overhead of transmitting
            //     trees more frequently.
            //   - I can't count above 4
            public uint lit_bufsize;

            public uint last_lit;      // running index in l_buf

            // Buffer for distances. To simplify the code, d_buf and l_buf have
            // the same number of elements. To use different lengths, an extra flag
            // array would be necessary.
            public ushort[] d_buf;

            public uint opt_len;        // bit length of current block with optimal trees
            public uint static_len;     // bit length of current block with static trees
            public uint matches;        // number of string matches in current block
            public int last_eob_len;    // bit length of EOB code for last block

            // Output buffer. bits are inserted starting at the bottom (least
            // significant bits).
            public ushort bi_buf;

            // Number of valid bits in bi_buf.  All bits above the last valid bit
            // are always zero.
            public int bi_valid;

            // High water mark offset in window for initialized bytes -- bytes above
            // this are set to zero in order to avoid memory check warnings when
            // longest match routines access bytes past the input.  This is then
            // updated to the new high water mark.
            public uint high_water;

            public deflate_state Clone()
            {
                deflate_state ret = (deflate_state)MemberwiseClone();

                ret.dyn_ltree = new ct_data[HEAP_SIZE];
                for (int i = 0; i < HEAP_SIZE; i++) ret.dyn_ltree[i] = new ct_data(dyn_ltree[i]);

                ret.dyn_dtree = new ct_data[2 * D_CODES + 1];
                for (int i = 0; i < (2 * D_CODES + 1); i++) ret.dyn_dtree[i] = new ct_data(dyn_dtree[i]);

                ret.bl_tree = new ct_data[2 * BL_CODES + 1];
                for (int i = 0; i < (2 * BL_CODES + 1); i++) ret.bl_tree[i] = new ct_data(bl_tree[i]);

                ret.bl_count = new ushort[MAX_BITS + 1]; bl_count.CopyTo(ret.bl_count, 0);
                ret.heap = new int[2 * L_CODES + 1]; heap.CopyTo(ret.heap, 0);
                ret.depth = new byte[2 * L_CODES + 1]; depth.CopyTo(ret.depth, 0);

                ret.l_desc = new tree_desc(l_desc); // desc. for literal tree
                ret.d_desc = new tree_desc(d_desc); // desc. for distance tree
                ret.bl_desc = new tree_desc(bl_desc);

                return ret;
            }
        }

        // Output a byte on the stream.
        // IN assertion: there is enough room in pending_buf.
        //#define put_byte(s, c) {s.pending_buf[s.pending++] = (c);}

        // Minimum amount of lookahead, except at the end of the input file.
        // See deflate.c for comments about the MIN_MATCH+1.
        private const int MIN_LOOKAHEAD = MAX_MATCH + MIN_MATCH + 1;

        // In order to simplify the code, particularly on 16 bit machines, match
        // distances are limited to MAX_DIST instead of WSIZE.
        //#define MAX_DIST(s)  (s.w_size-MIN_LOOKAHEAD)

        // Number of bytes after end of data in window to initialize in order to avoid
        // memory checker errors from longest match routines
        private const int WIN_INIT = MAX_MATCH;

        // Mapping from a distance to a distance code. dist is the distance - 1 and
        // must not have side effects. _dist_code[256] and _dist_code[257] are never
        // used.
        //#define d_code(dist) ((dist) < 256 ? _dist_code[dist] : _dist_code[256+((dist)>>7)])

        #endregion

        // If you use the zlib library in a product, an acknowledgment is welcome
        // in the documentation of your product. If for some reason you cannot
        // include such an acknowledgment, I would appreciate that you keep this
        // copyright string in the executable of your product.
        private const string deflate_copyright = " deflate 1.2.5 Copyright 1995-2010 Jean-loup Gailly ";

        // ===========================================================================
        // Function prototypes.

        enum block_state
        {
            need_more,      // block not completed, need more input or more output
            block_done,     // block flush performed
            finish_started, // finish started, need only more output at next deflate
            finish_done     // finish done, accept no more input or output
        }

        // Compression function. Returns the block state after the call.
        delegate block_state compress_func(deflate_state s, int flush);

        // ===========================================================================
        // Local data

        // Tail of hash chains
        private const int NIL = 0;

        // Matches of length 3 are discarded if their distance exceeds TOO_FAR
        private const int TOO_FAR = 4096;

        // Values for max_lazy_match, good_match and max_chain_length, depending on
        // the desired pack level (0..9). The values given below have been tuned to
        // exclude worst case performance for pathological files. Better values may be
        // found for specific files.
        struct config
        {
            public ushort good_length;  // reduce lazy search above this match length
            public ushort max_lazy;     // do not perform lazy search above this match length
            public ushort nice_length;  // quit search above this match length
            public ushort max_chain;
            public compress_func func;

            public config(ushort good_length, ushort max_lazy, ushort nice_length, ushort max_chain, compress_func func)
            {
                this.good_length = good_length;
                this.max_lazy = max_lazy;
                this.nice_length = nice_length;
                this.max_chain = max_chain;
                this.func = func;
            }
        }

        static readonly config[] configuration_table = new config[]
        { // good lazy nice chain
			new config( 0,   0,   0,    0, deflate_stored),	// store only
			new config( 4,   4,   8,    4, deflate_fast),	// max speed, no lazy matches
			new config( 4,   5,  16,    8, deflate_fast),
            new config( 4,   6,  32,   32, deflate_fast),
            new config( 4,   4,  16,   16, deflate_slow),	// lazy matches
			new config( 8,  16,  32,   32, deflate_slow),
            new config( 8,  16, 128,  128, deflate_slow),
            new config( 8,  32, 128,  256, deflate_slow),
            new config(32, 128, 258, 1024, deflate_slow),
            new config(32, 258, 258, 4096, deflate_slow)	// max compression
		};

        // Note: the deflate() code requires max_lazy >= MIN_MATCH and max_chain >= 4
        // For deflate_fast() (levels <= 3) good is ignored and lazy has a different
        // meaning.

        // ===========================================================================
        // Update a hash value with the given input byte
        // IN  assertion: all calls to to UPDATE_HASH are made with consecutive
        //    input characters, so that a running hash key can be computed from the
        //    previous key instead of complete recalculation each time.
        //#define UPDATE_HASH(s,h,c) h = ((h<<s.hash_shift) ^ c) & s.hash_mask

        // ===========================================================================
        // Insert string str in the dictionary and set match_head to the previous head
        // of the hash chain (the most recent string with same hash key). Return
        // the previous length of the hash chain.
        // If this file is compiled with -DFASTEST, the compression level is forced
        // to 1, and no hash chains are maintained.
        // IN  assertion: all calls to to INSERT_STRING are made with consecutive
        //    input characters and the first MIN_MATCH bytes of str are valid
        //    (except for the last MIN_MATCH-1 bytes of the input file).
        //#define INSERT_STRING(s, str, match_head) \
        //		s.ins_h = ((s.ins_h<<(int)s.hash_shift) ^ s.window[(str) + (MIN_MATCH-1)]) & s.hash_mask; \
        //		match_head = s.prev[(str) & s.w_mask] = s.head[s.ins_h]; \
        //		s.head[s.ins_h] = (unsigned short)str

        // ===========================================================================
        // Initialize the hash table (avoiding 64K overflow for 16 bit systems).
        // prev[] will be initialized on the fly.

        // =========================================================================
        //   Initializes the internal stream state for compression. The fields
        // zalloc, zfree and opaque must be initialized before by the caller.
        // If zalloc and zfree are set to Z_NULL, deflateInit updates them to
        // use default allocation functions.

        //   The compression level must be Z_DEFAULT_COMPRESSION, or between 0 and 9:
        // 1 gives best speed, 9 gives best compression, 0 gives no compression at
        // all (the input data is simply copied a block at a time).
        // Z_DEFAULT_COMPRESSION requests a default compromise between speed and
        // compression (currently equivalent to level 6).

        //   deflateInit returns Z_OK if success, Z_MEM_ERROR if there was not
        // enough memory, Z_STREAM_ERROR if level is not a valid compression level,
        // Z_VERSION_ERROR if the zlib library version (zlib_version) is incompatible
        // with the version assumed by the caller (ZLIB_VERSION).
        // msg is set to null if there is no error message.  deflateInit does not
        // perform any compression: this will be done by deflate().
        public static int deflateInit(z_stream strm, int level)
        {
            return deflateInit2(strm, level, Z_DEFLATED, MAX_WBITS, DEF_MEM_LEVEL, Z_DEFAULT_STRATEGY);
            // Todo: ignore strm.next_in if we use it as window
        }

        // =========================================================================
        //   This is another version of deflateInit with more compression options. The
        // fields next_in, zalloc, zfree and opaque must be initialized before by
        // the caller.

        //   The method parameter is the compression method. It must be Z_DEFLATED in
        // this version of the library.

        //   The windowBits parameter is the base two logarithm of the window size
        // (the size of the history buffer). It should be in the range 8..15 for this
        // version of the library. Larger values of this parameter result in better
        // compression at the expense of memory usage. The default value is 15 if
        // deflateInit is used instead.

        //   windowBits can also be -8..-15 for raw deflate. In this case, -windowBits
        // determines the window size. deflate() will then generate raw deflate data
        // with no zlib header or trailer, and will not compute an adler32 check value.

        //   windowBits can also be greater than 15 for optional gzip encoding. Add
        // 16 to windowBits to write a simple gzip header and trailer around the
        // compressed data instead of a zlib wrapper. The gzip header will have no
        // file name, no extra data, no comment, no modification time (set to zero),
        // no header crc, and the operating system will be set to 255 (unknown).  If a
        // gzip stream is being written, strm.adler is a crc32 instead of an adler32.

        //   The memLevel parameter specifies how much memory should be allocated
        // for the internal compression state. memLevel=1 uses minimum memory but
        // is slow and reduces compression ratio; memLevel=9 uses maximum memory
        // for optimal speed. The default value is 8. See zconf.h for total memory
        // usage as a function of windowBits and memLevel.

        //   The strategy parameter is used to tune the compression algorithm. Use the
        // value Z_DEFAULT_STRATEGY for normal data, Z_FILTERED for data produced by a
        // filter (or predictor), Z_HUFFMAN_ONLY to force Huffman encoding only (no
        // string match), or Z_RLE to limit match distances to one (run-length
        // encoding). Filtered data consists mostly of small values with a somewhat
        // random distribution. In this case, the compression algorithm is tuned to
        // compress them better. The effect of Z_FILTERED is to force more Huffman
        // coding and less string matching; it is somewhat intermediate between
        // Z_DEFAULT and Z_HUFFMAN_ONLY. Z_RLE is designed to be almost as fast as
        // Z_HUFFMAN_ONLY, but give better compression for PNG image data. The strategy
        // parameter only affects the compression ratio but not the correctness of the
        // compressed output even if it is not set appropriately.  Z_FIXED prevents the
        // use of dynamic Huffman codes, allowing for a simpler decoder for special
        // applications.

        //    deflateInit2 returns Z_OK if success, Z_MEM_ERROR if there was not enough
        // memory, Z_STREAM_ERROR if a parameter is invalid (such as an invalid
        // method). msg is set to null if there is no error message.  deflateInit2 does
        // not perform any compression: this will be done by deflate().

        public static int deflateInit2(z_stream strm, int level, int method, int windowBits, int memLevel, int strategy)
        {
            if (strm == null) return Z_STREAM_ERROR;
            strm.msg = null;

            if (level == Z_DEFAULT_COMPRESSION) level = 6;

            int wrap = 1;

            if (windowBits < 0)
            { // suppress zlib wrapper
                wrap = 0;
                windowBits = -windowBits;
            }
            else if (windowBits > 15)
            {
                wrap = 2;       // write gzip wrapper instead
                windowBits -= 16;
            }

            if (memLevel < 1 || memLevel > MAX_MEM_LEVEL || method != Z_DEFLATED || windowBits < 8 || windowBits > 15 || level < 0 || level > 9 ||
                strategy < 0 || strategy > Z_FIXED) return Z_STREAM_ERROR;

            if (windowBits == 8) windowBits = 9;  // until 256-byte window bug fixed

            deflate_state s;
            try
            {
                s = new deflate_state();
            }
            catch (Exception)
            {
                return Z_MEM_ERROR;
            }

            strm.state = s;
            s.strm = strm;

            s.wrap = wrap;
            s.w_bits = (uint)windowBits;
            s.w_size = 1U << (int)s.w_bits;
            s.w_mask = s.w_size - 1;

            s.hash_bits = (uint)memLevel + 7;
            s.hash_size = 1U << (int)s.hash_bits;
            s.hash_mask = s.hash_size - 1;
            s.hash_shift = (s.hash_bits + MIN_MATCH - 1) / MIN_MATCH;

            try
            {
                s.window = new byte[s.w_size * 2];
                s.prev = new ushort[s.w_size];
                s.head = new ushort[s.hash_size];
                s.high_water = 0; // nothing written to s->window yet

                s.lit_bufsize = 1U << (memLevel + 6); // 16K elements by default

                s.pending_buf = new byte[s.lit_bufsize * 4];
                s.pending_buf_size = s.lit_bufsize * 4;

                s.d_buf = new ushort[s.lit_bufsize];
                s.l_buf = new byte[s.lit_bufsize];
            }
            catch (Exception)
            {
                s.status = FINISH_STATE;
                strm.msg = zError(Z_MEM_ERROR);
                deflateEnd(strm);
                return Z_MEM_ERROR;
            }

            s.level = level;
            s.strategy = strategy;
            s.method = (byte)method;

            return deflateReset(strm);
        }

        // =========================================================================
        //   Initializes the compression dictionary from the given byte sequence
        // without producing any compressed output. This function must be called
        // immediately after deflateInit, deflateInit2 or deflateReset, before any
        // call of deflate. The compressor and decompressor must use exactly the same
        // dictionary (see inflateSetDictionary).

        //   The dictionary should consist of strings (byte sequences) that are likely
        // to be encountered later in the data to be compressed, with the most commonly
        // used strings preferably put towards the end of the dictionary. 
        // dictionary is most useful when the data to be compressed is short and can be
        // predicted with good accuracy; the data can then be compressed better than
        // with the default empty dictionary.

        //   Depending on the size of the compression data structures selected by
        // deflateInit or deflateInit2, a part of the dictionary may in effect be
        // discarded, for example if the dictionary is larger than the window size in
        // deflate or deflate2. Thus the strings most likely to be useful should be
        // put at the end of the dictionary, not at the front. In addition, the
        // current implementation of deflate will use at most the window size minus
        // 262 bytes of the provided dictionary.

        //   Upon return of this function, strm.adler is set to the adler32 value
        // of the dictionary; the decompressor may later use this value to determine
        // which dictionary has been used by the compressor. (The adler32 value
        // applies to the whole dictionary even if only a subset of the dictionary is
        // actually used by the compressor.) If a raw deflate was requested, then the
        // adler32 value is not computed and strm.adler is not set.

        //   deflateSetDictionary returns Z_OK if success, or Z_STREAM_ERROR if a
        // parameter is invalid (such as NULL dictionary) or the stream state is
        // inconsistent (for example if deflate has already been called for this stream
        // or if the compression method is bsort). deflateSetDictionary does not
        // perform any compression: this will be done by deflate().

        public static int deflateSetDictionary(z_stream strm, byte[] dictionary, uint dictLength)
        {
            uint length = dictLength;
            uint n;
            uint hash_head = 0;

            if (strm == null || strm.state == null || dictionary == null) return Z_STREAM_ERROR;

            deflate_state s = strm.state as deflate_state;
            if (s == null || s.wrap == 2 || (s.wrap == 1 && s.status != INIT_STATE))
                return Z_STREAM_ERROR;

            if (s.wrap != 0) strm.adler = adler32(strm.adler, dictionary, dictLength);

            if (length < MIN_MATCH) return Z_OK;

            int dictionary_ind = 0;
            if (length > s.w_size)
            {
                length = s.w_size;
                dictionary_ind = (int)(dictLength - length); // use the tail of the dictionary
            }

            //was memcpy(s.window, dictionary+dictionary_ind, length);
            Array.Copy(dictionary, dictionary_ind, s.window, 0, length);

            s.strstart = length;
            s.block_start = (int)length;

            // Insert all strings in the hash table (except for the last two bytes).
            // s.lookahead stays null, so s.ins_h will be recomputed at the next
            // call of fill_window.
            s.ins_h = s.window[0];

            //was UPDATE_HASH(s, s.ins_h, s.window[1]);
            s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[1]) & s.hash_mask;

            for (n = 0; n <= length - MIN_MATCH; n++)
            {
                //was INSERT_STRING(s, n, hash_head);
                s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[n + (MIN_MATCH - 1)]) & s.hash_mask;
                hash_head = s.prev[n & s.w_mask] = s.head[s.ins_h];
                s.head[s.ins_h] = (ushort)n;
            }
            if (hash_head != 0) hash_head = 0;  // to make compiler happy
            return Z_OK;
        }

        // =========================================================================
        //   This function is equivalent to deflateEnd followed by deflateInit,
        // but does not free and reallocate all the internal compression state.
        // The stream will keep the same compression level and any other attributes
        // that may have been set by deflateInit2.

        //    deflateReset returns Z_OK if success, or Z_STREAM_ERROR if the source
        // stream state was inconsistent (such as zalloc or state being NULL).
        public static int deflateReset(z_stream strm)
        {
            if (strm == null || strm.state == null) return Z_STREAM_ERROR;

            strm.total_in = strm.total_out = 0;
            strm.msg = null;

            deflate_state s = (deflate_state)strm.state;
            s.pending = 0;
            s.pending_out = 0;

            if (s.wrap < 0) s.wrap = -s.wrap; // was made negative by deflate(..., Z_FINISH);

            s.status = s.wrap != 0 ? INIT_STATE : BUSY_STATE;
            strm.adler = s.wrap == 2 ? crc32(0, null, 0) : adler32(0, null, 0);
            s.last_flush = Z_NO_FLUSH;

            _tr_init(s);
            lm_init(s);

            return Z_OK;
        }

        // =========================================================================
        //    deflateSetHeader() provides gzip header information for when a gzip
        // stream is requested by deflateInit2().  deflateSetHeader() may be called
        // after deflateInit2() or deflateReset() and before the first call of
        // deflate().  The text, time, os, extra field, name, and comment information
        // in the provided gz_header structure are written to the gzip header (xflag is
        // ignored -- the extra flags are set according to the compression level).  The
        // caller must assure that, if not Z_NULL, name and comment are terminated with
        // a zero byte, and that if extra is not Z_NULL, that extra_len bytes are
        // available there.  If hcrc is true, a gzip header crc is included.  Note that
        // the current versions of the command-line version of gzip (up through version
        // 1.3.x) do not support header crc's, and will report that it is a "multi-part
        // gzip file" and give up.

        //    If deflateSetHeader is not used, the default gzip header has text false,
        // the time set to zero, and os set to 255, with no extra, name, or comment
        // fields.  The gzip header is returned to the default state by deflateReset().

        //    deflateSetHeader returns Z_OK if success, or Z_STREAM_ERROR if the source
        // stream state was inconsistent.

        public static int deflateSetHeader(z_stream strm, gz_header head)
        {
            if (strm == null || strm.state == null) return Z_STREAM_ERROR;
            deflate_state s = (deflate_state)strm.state;
            if (s.wrap != 2) return Z_STREAM_ERROR;
            s.gzhead = head;
            return Z_OK;
        }

        // =========================================================================
        //    deflatePrime() inserts bits in the deflate output stream.  The intent
        // is that this function is used to start off the deflate output with the
        // bits leftover from a previous deflate stream when appending to it.  As such,
        // this function can only be used for raw deflate, and must be used before the
        // first deflate() call after a deflateInit2() or deflateReset().  bits must be
        // less than or equal to 16, and that many of the least significant bits of
        // value will be inserted in the output.

        //    deflatePrime returns Z_OK if success, or Z_STREAM_ERROR if the source
        // stream state was inconsistent.

        public static int deflatePrime(z_stream strm, int bits, int value)
        {
            if (strm == null || strm.state == null) return Z_STREAM_ERROR;
            deflate_state s = (deflate_state)strm.state;
            s.bi_valid = bits;
            s.bi_buf = (ushort)(value & ((1 << bits) - 1));
            return Z_OK;
        }

        // =========================================================================
        //   Dynamically update the compression level and compression strategy.  The
        // interpretation of level and strategy is as in deflateInit2.  This can be
        // used to switch between compression and straight copy of the input data, or
        // to switch to a different kind of input data requiring a different
        // strategy. If the compression level is changed, the input available so far
        // is compressed with the old level (and may be flushed); the new level will
        // take effect only at the next call of deflate().

        //   Before the call of deflateParams, the stream state must be set as for
        // a call of deflate(), since the currently available input may have to
        // be compressed and flushed. In particular, strm.avail_out must be non-zero.

        //   deflateParams returns Z_OK if success, Z_STREAM_ERROR if the source
        // stream state was inconsistent or if a parameter was invalid, Z_BUF_ERROR
        // if strm.avail_out was zero.

        public static int deflateParams(z_stream strm, int level, int strategy)
        {
            if (strm == null || strm.state == null) return Z_STREAM_ERROR;
            deflate_state s = (deflate_state)strm.state;

            if (level == Z_DEFAULT_COMPRESSION) level = 6;
            if (level < 0 || level > 9 || strategy < 0 || strategy > Z_FIXED) return Z_STREAM_ERROR;

            compress_func func = configuration_table[s.level].func;
            int err = Z_OK;

            if ((strategy != s.strategy || func != configuration_table[level].func) && strm.total_in != 0) // Flush the last buffer:
                err = deflate(strm, Z_BLOCK);

            if (s.level != level)
            {
                s.level = level;
                s.max_lazy_match = configuration_table[level].max_lazy;
                s.good_match = configuration_table[level].good_length;
                s.nice_match = configuration_table[level].nice_length;
                s.max_chain_length = configuration_table[level].max_chain;
            }

            s.strategy = strategy;
            return err;
        }

        // =========================================================================
        //   Fine tune deflate's internal compression parameters.  This should only be
        // used by someone who understands the algorithm used by zlib's deflate for
        // searching for the best matching string, and even then only by the most
        // fanatic optimizer trying to squeeze out the last compressed bit for their
        // specific input data.  Read the deflate.cs source code for the meaning of the
        // max_lazy, good_length, nice_length, and max_chain parameters.

        //   deflateTune() can be called after deflateInit() or deflateInit2(), and
        // returns Z_OK on success, or Z_STREAM_ERROR for an invalid deflate stream.

        public static int deflateTune(z_stream strm, uint good_length, uint max_lazy, int nice_length, uint max_chain)
        {
            if (strm == null || strm.state == null) return Z_STREAM_ERROR;
            deflate_state s = (deflate_state)strm.state;
            s.good_match = good_length;
            s.max_lazy_match = max_lazy;
            s.nice_match = nice_length;
            s.max_chain_length = max_chain;
            return Z_OK;
        }

        // =========================================================================
        // For the default windowBits of 15 and memLevel of 8, this function returns
        // a close to exact, as well as small, upper bound on the compressed size.
        // They are coded as constants here for a reason--if the #define's are
        // changed, then this function needs to be changed as well.  The return
        // value for 15 and 8 only works for those exact settings.
        //
        // For any setting other than those defaults for windowBits and memLevel,
        // the value returned is a conservative worst case for the maximum expansion
        // resulting from         // can emit on compressed data for some combinations of the parameters.
        //
        // This function could be more sophisticated to provide closer upper bounds for
        // every combination of windowBits and memLevel. But even the conservative
        // upper bound of about 14% expansion does not seem onerous for output buffer
        // allocation.

        //   deflateBound() returns an upper bound on the compressed size after
        // deflation of sourceLen bytes.  It must be called after deflateInit()
        // or deflateInit2().  This would be used to allocate an output buffer
        // for deflation in a single pass, and so would be called before deflate().

        public static uint deflateBound(z_stream strm, uint sourceLen)
        {
            // conservative upper bound for compressed data
            uint complen = sourceLen + ((sourceLen + 7) >> 3) + ((sourceLen + 63) >> 6) + 5;

            // if can't get parameters, return conservative bound plus zlib wrapper
            if (strm == null || strm.state == null) return complen + 6;

            // compute wrapper length
            deflate_state s = (deflate_state)strm.state;
            uint wraplen;
            byte[] str;
            switch (s.wrap)
            {
                case 0: // raw deflate
                    wraplen = 0;
                    break;
                case 1: // zlib wrapper
                    wraplen = (uint)(6 + (s.strstart != 0 ? 4 : 0));
                    break;
                case 2: // gzip wrapper
                    wraplen = 18;
                    if (s.gzhead != null) // user-supplied gzip header
                    {
                        if (s.gzhead.extra != null) wraplen += 2 + s.gzhead.extra_len;
                        str = s.gzhead.name;
                        int str_ind = 0;
                        if (str != null)
                        {
                            do
                            {
                                wraplen++;
                            } while (str[str_ind++] != 0);
                        }
                        str = s.gzhead.comment;
                        if (str != null)
                        {
                            do
                            {
                                wraplen++;
                            } while (str[str_ind++] != 0);
                        }
                        if (s.gzhead.hcrc != 0) wraplen += 2;
                    }
                    break;
                default: wraplen = 6; break; // for compiler happiness
            }

            // if not default parameters, return conservative bound
            if (s.w_bits != 15 || s.hash_bits != 8 + 7) return complen + wraplen;

            // default settings: return tight bound for that case
            return sourceLen + (sourceLen >> 12) + (sourceLen >> 14) + (sourceLen >> 25) + 13 - 6 + wraplen;
        }

        // =========================================================================
        // Put a short in the pending buffer. The 16-bit value is put in MSB order.
        // IN assertion: the stream state is correct and there is enough room in
        // pending_buf.
        static void putShortMSB(deflate_state s, uint b)
        {
            //was put_byte(s, (byte)(b >> 8));
            s.pending_buf[s.pending++] = (byte)(b >> 8);
            //was put_byte(s, (byte)(b & 0xff));
            s.pending_buf[s.pending++] = (byte)(b & 0xff);
        }

        // =========================================================================
        // Flush as much pending output as possible. All deflate() output goes
        // through this function so some applications may wish to modify it
        // to avoid allocating a large strm.next_out buffer and copying into it.
        // (See also read_buf()).
        static void flush_pending(z_stream strm)
        {
            deflate_state s = (deflate_state)strm.state;
            uint len = s.pending;

            if (len > strm.avail_out) len = strm.avail_out;
            if (len == 0) return;

            //was memcpy(strm.next_out, s.pending_out, len);
            Array.Copy(s.pending_buf, s.pending_out, strm.out_buf, strm.next_out, len);

            strm.next_out += (int)len;
            s.pending_out += (int)len;
            strm.total_out += len;
            strm.avail_out -= len;
            s.pending -= len;
            if (s.pending == 0) s.pending_out = 0;
        }

        const int PRESET_DICT = 0x20; // preset dictionary flag in zlib header

        #region deflate
        // =========================================================================
        //   deflate compresses as much data as possible, and stops when the input
        // buffer becomes empty or the output buffer becomes full. It may introduce some
        // output latency (reading input without producing any output) except when
        // forced to flush.

        //   The detailed semantics are as follows. deflate performs one or both of the
        // following actions:

        // - Compress more input starting at next_in and update next_in and avail_in
        //   accordingly. If not all input can be processed (because there is not
        //   enough room in the output buffer), next_in and avail_in are updated and
        //   processing will resume at this point for the next call of deflate().

        // - Provide more output starting at next_out and update next_out and avail_out
        //   accordingly. This action is forced if the parameter flush is non zero.
        //   Forcing flush frequently degrades the compression ratio, so this parameter
        //   should be set only when necessary (in interactive applications).
        //   Some output may be provided even if flush is not set.

        // Before the call of deflate(), the application should ensure that at least
        // one of the actions is possible, by providing more input and/or consuming
        // more output, and updating avail_in or avail_out accordingly; avail_out
        // should never be zero before the call. The application can consume the
        // compressed output when it wants, for example when the output buffer is full
        // (avail_out == 0), or after each call of deflate(). If deflate returns Z_OK
        // and with zero avail_out, it must be called again after making room in the
        // output buffer because there might be more output pending.

        //   Normally the parameter flush is set to Z_NO_FLUSH, which allows deflate to
        // decide how much data to accumualte before producing output, in order to
        // maximize compression.

        //   If the parameter flush is set to Z_SYNC_FLUSH, all pending output is
        // flushed to the output buffer and the output is aligned on a byte boundary, so
        // that the decompressor can get all input data available so far. (In particular
        // avail_in is zero after the call if enough output space has been provided
        // before the call.)  Flushing may degrade compression for some compression
        // algorithms and so it should be used only when necessary.

        //   If flush is set to Z_FULL_FLUSH, all output is flushed as with
        // Z_SYNC_FLUSH, and the compression state is reset so that decompression can
        // restart from this point if previous compressed data has been damaged or if
        // random access is desired.         // compression.

        //   If deflate returns with avail_out == 0, this function must be called again
        // with the same value of the flush parameter and more output space (updated
        // avail_out), until the flush is complete (deflate returns with non-zero
        // avail_out). In the case of a Z_FULL_FLUSH or Z_SYNC_FLUSH, make sure that
        // avail_out is greater than six to avoid repeated flush markers due to
        // avail_out == 0 on return.

        //   If the parameter flush is set to Z_FINISH, pending input is processed,
        // pending output is flushed and deflate returns with Z_STREAM_END if there
        // was enough output space; if deflate returns with Z_OK, this function must be
        // called again with Z_FINISH and more output space (updated avail_out) but no
        // more input data, until it returns with Z_STREAM_END or an error. After
        // deflate has returned Z_STREAM_END, the only possible operations on the
        // stream are deflateReset or deflateEnd.

        //   Z_FINISH can be used immediately after deflateInit if all the compression
        // is to be done in a single step. In this case, avail_out must be at least
        // the value returned by deflateBound (see below). If deflate does not return
        // Z_STREAM_END, then it must be called again as described above.

        //   deflate() sets strm.adler to the adler32 checksum of all input read
        // so far (that is, total_in bytes).

        //   deflate() returns Z_OK if some progress has been made (more input
        // processed or more output produced), Z_STREAM_END if all input has been
        // consumed and all output has been produced (only when flush is set to
        // Z_FINISH), Z_STREAM_ERROR if the stream state was inconsistent (for example
        // if next_in or next_out was NULL), Z_BUF_ERROR if no progress is possible
        // (for example avail_in or avail_out was zero). Note that Z_BUF_ERROR is not
        // fatal, and deflate() can be called again with more input and more output
        // space to continue compressing.

        public static int deflate(z_stream strm, int flush)
        {
            if (strm == null || strm.state == null || flush > Z_BLOCK || flush < 0) return Z_STREAM_ERROR;
            deflate_state s = (deflate_state)strm.state;

            if (strm.out_buf == null || (strm.in_buf == null && strm.avail_in != 0) || (s.status == FINISH_STATE && flush != Z_FINISH))
            {
                strm.msg = zError(Z_STREAM_ERROR);
                return Z_STREAM_ERROR;
            }

            if (strm.avail_out == 0)
            {
                strm.msg = zError(Z_BUF_ERROR);
                return Z_BUF_ERROR;
            }

            s.strm = strm; // just in case
            int old_flush = s.last_flush;// value of flush param for previous deflate call
            s.last_flush = flush;

            // Write the header
            if (s.status == INIT_STATE)
            {
                if (s.wrap == 2)
                {
                    strm.adler = crc32(0, null, 0);
                    s.pending_buf[s.pending++] = 31;
                    s.pending_buf[s.pending++] = 139;
                    s.pending_buf[s.pending++] = 8;
                    if (s.gzhead == null)
                    {
                        s.pending_buf[s.pending++] = 0;

                        s.pending_buf[s.pending++] = 0;
                        s.pending_buf[s.pending++] = 0;
                        s.pending_buf[s.pending++] = 0;
                        s.pending_buf[s.pending++] = 0;

                        s.pending_buf[s.pending++] = (byte)(s.level == 9 ? 2 : (s.strategy >= Z_HUFFMAN_ONLY || s.level < 2 ? 4 : 0));
                        s.pending_buf[s.pending++] = OS_CODE;
                        s.status = BUSY_STATE;
                    }
                    else
                    {
                        s.pending_buf[s.pending++] = (byte)((s.gzhead.text != 0 ? 1 : 0) + (s.gzhead.hcrc != 0 ? 2 : 0) + (s.gzhead.extra == null ? 0 : 4) +
                                    (s.gzhead.name == null ? 0 : 8) + (s.gzhead.comment == null ? 0 : 16));
                        s.pending_buf[s.pending++] = (byte)(s.gzhead.time & 0xff);
                        s.pending_buf[s.pending++] = (byte)((s.gzhead.time >> 8) & 0xff);
                        s.pending_buf[s.pending++] = (byte)((s.gzhead.time >> 16) & 0xff);
                        s.pending_buf[s.pending++] = (byte)((s.gzhead.time >> 24) & 0xff);
                        s.pending_buf[s.pending++] = (byte)(s.level == 9 ? 2 : (s.strategy >= Z_HUFFMAN_ONLY || s.level < 2 ? 4 : 0));
                        s.pending_buf[s.pending++] = (byte)(s.gzhead.os & 0xff);
                        if (s.gzhead.extra != null)
                        {
                            s.pending_buf[s.pending++] = (byte)(s.gzhead.extra_len & 0xff);
                            s.pending_buf[s.pending++] = (byte)((s.gzhead.extra_len >> 8) & 0xff);
                        }
                        if (s.gzhead.hcrc != 0) strm.adler = crc32(strm.adler, s.pending_buf, s.pending);
                        s.gzindex = 0;
                        s.status = EXTRA_STATE;
                    }
                }
                else
                {
                    uint header = (Z_DEFLATED + ((s.w_bits - 8) << 4)) << 8;
                    uint level_flags;

                    if (s.strategy >= Z_HUFFMAN_ONLY || s.level < 2) level_flags = 0;
                    else if (s.level < 6) level_flags = 1;
                    else if (s.level == 6) level_flags = 2;
                    else level_flags = 3;

                    header |= (level_flags << 6);
                    if (s.strstart != 0) header |= PRESET_DICT;
                    header += 31 - (header % 31);

                    s.status = BUSY_STATE;
                    putShortMSB(s, header);

                    // Save the adler32 of the preset dictionary:
                    if (s.strstart != 0)
                    {
                        putShortMSB(s, (uint)(strm.adler >> 16));
                        putShortMSB(s, (uint)(strm.adler & 0xffff));
                    }
                    strm.adler = adler32(0, null, 0);
                }
            }
            if (s.status == EXTRA_STATE)
            {
                if (s.gzhead.extra != null)
                {
                    uint beg = s.pending;  // start of bytes to update crc

                    while (s.gzindex < (s.gzhead.extra_len & 0xffff))
                    {
                        if (s.pending == s.pending_buf_size)
                        {
                            if (s.gzhead.hcrc != 0 && s.pending > beg) strm.adler = crc32(strm.adler, s.pending_buf, beg, s.pending - beg);
                            flush_pending(strm);
                            beg = s.pending;
                            if (s.pending == s.pending_buf_size) break;
                        }
                        s.pending_buf[s.pending++] = s.gzhead.extra[s.gzindex];
                        s.gzindex++;
                    }
                    if (s.gzhead.hcrc != 0 && s.pending > beg) strm.adler = crc32(strm.adler, s.pending_buf, beg, s.pending - beg);
                    if (s.gzindex == s.gzhead.extra_len)
                    {
                        s.gzindex = 0;
                        s.status = NAME_STATE;
                    }
                }
                else s.status = NAME_STATE;
            }
            if (s.status == NAME_STATE)
            {
                if (s.gzhead.name != null)
                {
                    uint beg = s.pending;  // start of bytes to update crc
                    byte val;

                    do
                    {
                        if (s.pending == s.pending_buf_size)
                        {
                            if (s.gzhead.hcrc != 0 && s.pending > beg) strm.adler = crc32(strm.adler, s.pending_buf, beg, s.pending - beg);
                            flush_pending(strm);
                            beg = s.pending;
                            if (s.pending == s.pending_buf_size)
                            {
                                val = 1;
                                break;
                            }
                        }
                        val = s.gzhead.name[s.gzindex++];
                        s.pending_buf[s.pending++] = val;
                    } while (val != 0);
                    if (s.gzhead.hcrc != 0 && s.pending > beg) strm.adler = crc32(strm.adler, s.pending_buf, beg, s.pending - beg);
                    if (val == 0)
                    {
                        s.gzindex = 0;
                        s.status = COMMENT_STATE;
                    }
                }
                else s.status = COMMENT_STATE;
            }
            if (s.status == COMMENT_STATE)
            {
                if (s.gzhead.comment != null)
                {
                    uint beg = s.pending;  // start of bytes to update crc
                    byte val;

                    do
                    {
                        if (s.pending == s.pending_buf_size)
                        {
                            if (s.gzhead.hcrc != 0 && s.pending > beg) strm.adler = crc32(strm.adler, s.pending_buf, beg, s.pending - beg);
                            flush_pending(strm);
                            beg = s.pending;
                            if (s.pending == s.pending_buf_size)
                            {
                                val = 1;
                                break;
                            }
                        }
                        val = s.gzhead.comment[s.gzindex++];
                        s.pending_buf[s.pending++] = val;
                    } while (val != 0);
                    if (s.gzhead.hcrc != 0 && s.pending > beg) strm.adler = crc32(strm.adler, s.pending_buf, beg, s.pending - beg);
                    if (val == 0) s.status = HCRC_STATE;
                }
                else s.status = HCRC_STATE;
            }
            if (s.status == HCRC_STATE)
            {
                if (s.gzhead.hcrc != 0)
                {
                    if (s.pending + 2 > s.pending_buf_size) flush_pending(strm);
                    if (s.pending + 2 <= s.pending_buf_size)
                    {
                        s.pending_buf[s.pending++] = (byte)(strm.adler & 0xff);
                        s.pending_buf[s.pending++] = (byte)((strm.adler >> 8) & 0xff);
                        strm.adler = crc32(0, null, 0);
                        s.status = BUSY_STATE;
                    }
                }
                else s.status = BUSY_STATE;
            }

            // Flush as much pending output as possible
            if (s.pending != 0)
            {
                flush_pending(strm);
                if (strm.avail_out == 0)
                {
                    // Since avail_out is 0, deflate will be called again with
                    // more output space, but possibly with both pending and
                    // avail_in equal to zero. There won't be anything to do,
                    // but this is not an error situation so make sure we
                    // return OK instead of BUF_ERROR at next call of deflate:
                    s.last_flush = -1;
                    return Z_OK;
                }

                // Make sure there is something to do and avoid duplicate consecutive
                // flushes. For repeated and useless calls with Z_FINISH, we keep
                // returning Z_STREAM_END instead of Z_BUF_ERROR.
            }
            else if (strm.avail_in == 0 && flush <= old_flush && flush != Z_FINISH)
            {
                strm.msg = zError(Z_BUF_ERROR);
                return Z_BUF_ERROR;
            }

            // User must not provide more input after the first FINISH:
            if (s.status == FINISH_STATE && strm.avail_in != 0)
            {
                strm.msg = zError(Z_BUF_ERROR);
                return Z_BUF_ERROR;
            }

            // Start a new block or continue the current one.
            if (strm.avail_in != 0 || s.lookahead != 0 || (flush != Z_NO_FLUSH && s.status != FINISH_STATE))
            {
                block_state bstate = s.strategy == Z_HUFFMAN_ONLY ? deflate_huff(s, flush) : (s.strategy == Z_RLE ? deflate_rle(s, flush) : configuration_table[s.level].func(s, flush));

                if (bstate == block_state.finish_started || bstate == block_state.finish_done) s.status = FINISH_STATE;
                if (bstate == block_state.need_more || bstate == block_state.finish_started)
                {
                    if (strm.avail_out == 0) s.last_flush = -1; // avoid BUF_ERROR next call, see above
                    return Z_OK;
                    // If flush != Z_NO_FLUSH && avail_out == 0, the next call
                    // of deflate should use the same flush parameter to make sure
                    // that the flush is complete. So we don't have to output an
                    // empty block here, this will be done at next call. This also
                    // ensures that for a very small output buffer, we emit at most
                    // one empty block.
                }
                if (bstate == block_state.block_done)
                {
                    if (flush == Z_PARTIAL_FLUSH) _tr_align(s);
                    else if (flush != Z_BLOCK)
                    { // FULL_FLUSH or SYNC_FLUSH
                        _tr_stored_block(s, null, 0, 0);
                        // For a full flush, this empty block will be recognized
                        // as a special marker by inflate_sync().
                        if (flush == Z_FULL_FLUSH)
                        {
                            s.head[s.hash_size - 1] = NIL; // forget history

                            //was memset((byte*)s.head, 0, (uint)(s.hash_size-1)*sizeof(*s.head));
                            for (int i = 0; i < s.hash_size - 1; i++) s.head[i] = 0;

                            if (s.lookahead == 0)
                            {
                                s.strstart = 0;
                                s.block_start = 0;
                            }
                        }
                    }
                    flush_pending(strm);
                    if (strm.avail_out == 0)
                    {
                        s.last_flush = -1; // avoid BUF_ERROR at next call, see above
                        return Z_OK;
                    }
                }
            }
            //Assert(strm.avail_out>0, "bug2");

            if (flush != Z_FINISH) return Z_OK;
            if (s.wrap <= 0) return Z_STREAM_END;

            // Write the trailer
            if (s.wrap == 2)
            {
                s.pending_buf[s.pending++] = (byte)(strm.adler & 0xff);
                s.pending_buf[s.pending++] = (byte)((strm.adler >> 8) & 0xff);
                s.pending_buf[s.pending++] = (byte)((strm.adler >> 16) & 0xff);
                s.pending_buf[s.pending++] = (byte)((strm.adler >> 24) & 0xff);
                s.pending_buf[s.pending++] = (byte)(strm.total_in & 0xff);
                s.pending_buf[s.pending++] = (byte)((strm.total_in >> 8) & 0xff);
                s.pending_buf[s.pending++] = (byte)((strm.total_in >> 16) & 0xff);
                s.pending_buf[s.pending++] = (byte)((strm.total_in >> 24) & 0xff);
            }
            else
            {
                putShortMSB(s, (uint)(strm.adler >> 16));
                putShortMSB(s, (uint)(strm.adler & 0xffff));
            }

            flush_pending(strm);
            // If avail_out is zero, the application will call deflate again
            // to flush the rest.
            if (s.wrap > 0) s.wrap = -s.wrap; // write the trailer only once!
            return s.pending != 0 ? Z_OK : Z_STREAM_END;
        }
        #endregion

        // =========================================================================
        //   All dynamically allocated data structures for this stream are freed.
        // This function discards any unprocessed input and does not flush any
        // pending output.

        //   deflateEnd returns Z_OK if success, Z_STREAM_ERROR if the
        // stream state was inconsistent, Z_DATA_ERROR if the stream was freed
        // prematurely (some input or output was discarded). In the error case,
        // msg may be set but then points to a static string (which must not be
        // deallocated).

        public static int deflateEnd(z_stream strm)
        {
            if (strm == null || strm.state == null) return Z_STREAM_ERROR;

            deflate_state s = (deflate_state)strm.state;
            int status = s.status;
            if (status != INIT_STATE && status != EXTRA_STATE && status != NAME_STATE && status != COMMENT_STATE &&
                status != HCRC_STATE && status != BUSY_STATE && status != FINISH_STATE) return Z_STREAM_ERROR;

            // Deallocate in reverse order of allocations:
            //if(s.pending_buf!=null) free(s.pending_buf);
            //if(s.l_buf!=null) free(s.l_buf);
            //if(s.d_buf!=null) free(s.d_buf);
            //if(s.head!=null) free(s.head);
            //if(s.prev!=null) free(s.prev);
            //if(s.window!=null) free(s.window);
            s.pending_buf = s.l_buf = s.window = null;
            s.d_buf = s.head = s.prev = null;

            //free(strm.state);
            strm.state = s = null;

            return status == BUSY_STATE ? Z_DATA_ERROR : Z_OK;
        }

        // =========================================================================
        //   Sets the destination stream as a complete copy of the source stream.

        //   This function can be useful when several compression strategies will be
        // tried, for example when there are several ways of pre-processing the input
        // data with a filter. The streams that will be discarded should then be freed
        // by calling deflateEnd.  Note that deflateCopy duplicates the internal
        // compression state which can be quite large, so this strategy is slow and
        // can consume lots of memory.

        //   deflateCopy returns Z_OK if success, Z_MEM_ERROR if there was not
        // enough memory, Z_STREAM_ERROR if the source stream state was inconsistent
        // (such as zalloc being NULL). msg is left unchanged in both source and
        // destination.

        // Copy the source state to the destination state.
        public static int deflateCopy(z_stream dest, z_stream source)
        {
            if (source == null || dest == null || source.state == null) return Z_STREAM_ERROR;

            deflate_state ss = (deflate_state)source.state;

            //was memcpy(dest, source, sizeof(z_stream));
            source.CopyTo(dest);

            deflate_state ds;
            try
            {
                ds = ss.Clone();
            }
            catch (Exception)
            {
                return Z_MEM_ERROR;
            }
            dest.state = ds;
            //(done above) memcpy(ds, ss, sizeof(deflate_state));
            ds.strm = dest;

            try
            {
                ds.window = new byte[ds.w_size * 2];
                ds.prev = new ushort[ds.w_size];
                ds.head = new ushort[ds.hash_size];
                ds.pending_buf = new byte[ds.lit_bufsize * 4];
                ds.d_buf = new ushort[ds.lit_bufsize];
                ds.l_buf = new byte[ds.lit_bufsize];
            }
            catch (Exception)
            {
                deflateEnd(dest);
                return Z_MEM_ERROR;
            }

            //was memcpy(ds.window, ss.window, ds.w_size*2*sizeof(byte));
            ss.window.CopyTo(ds.window, 0);

            //was memcpy(ds.prev, ss.prev, ds.w_size*sizeof(ushort));
            ss.prev.CopyTo(ds.prev, 0);

            //was memcpy(ds.head, ss.head, ds.hash_size*sizeof(ushort));
            ss.head.CopyTo(ds.head, 0);

            //was memcpy(ds.pending_buf, ss.pending_buf, (uint)ds.pending_buf_size);
            ss.pending_buf.CopyTo(ds.pending_buf, 0);
            ss.d_buf.CopyTo(ds.d_buf, 0);
            ss.l_buf.CopyTo(ds.l_buf, 0);

            ds.l_desc.dyn_tree = ds.dyn_ltree;
            ds.d_desc.dyn_tree = ds.dyn_dtree;
            ds.bl_desc.dyn_tree = ds.bl_tree;

            return Z_OK;
        }

        // ===========================================================================
        // Read a new buffer from the current input stream, update the adler32
        // and total number of bytes read.  All deflate() input goes through
        // this function so some applications may wish to modify it to avoid
        // allocating a large strm.next_in buffer and copying from it.
        // (See also flush_pending()).
        static int read_buf(z_stream strm, byte[] buf, uint size)
        {
            return read_buf(strm, buf, 0, size);
        }

        static int read_buf(z_stream strm, byte[] buf, int buf_ind, uint size)
        {
            uint len = strm.avail_in;

            if (len > size) len = size;
            if (len == 0) return 0;

            strm.avail_in -= len;

            deflate_state s = (deflate_state)strm.state;

            if (s.wrap == 1) strm.adler = adler32(strm.adler, strm.in_buf, (uint)strm.next_in, len);
            else if (s.wrap == 2) strm.adler = crc32(strm.adler, strm.in_buf, strm.next_in, len);

            //was memcpy(buf, strm.in_buf+strm.next_in, len);
            Array.Copy(strm.in_buf, strm.next_in, buf, buf_ind, len);
            strm.next_in += len;
            strm.total_in += len;

            return (int)len;
        }

        // ===========================================================================
        // Initialize the "longest match" routines for a new zlib stream
        static void lm_init(deflate_state s)
        {
            s.window_size = (uint)2 * s.w_size;

            s.head[s.hash_size - 1] = NIL;

            //was memset((byte*)s.head, 0, (uint)(s.hash_size-1)*sizeof(*s.head));
            for (int i = 0; i < (s.hash_size - 1); i++) s.head[i] = 0;

            // Set the default configuration parameters:
            s.max_lazy_match = configuration_table[s.level].max_lazy;
            s.good_match = configuration_table[s.level].good_length;
            s.nice_match = configuration_table[s.level].nice_length;
            s.max_chain_length = configuration_table[s.level].max_chain;

            s.strstart = 0;
            s.block_start = 0;
            s.lookahead = 0;
            s.match_length = s.prev_length = MIN_MATCH - 1;
            s.match_available = 0;
            s.ins_h = 0;
        }

        // ===========================================================================
        // Set match_start to the longest match starting at the given string and
        // return its length. Matches shorter or equal to prev_length are discarded,
        // in which case the result is equal to prev_length and match_start is
        // garbage.
        // IN assertions: cur_match is the head of the hash chain for the current
        //   string (strstart) and its distance is <= MAX_DIST, and prev_length >= 1
        // OUT assertion: the match length is not greater than s.lookahead.
        static uint longest_match(deflate_state s, uint cur_match)
        {
            uint chain_length = s.max_chain_length; // max hash chain length
            byte[] scan = s.window;                 // current string
            int scan_ind = (int)s.strstart;
            int len;                                // length of current match
            int best_len = (int)s.prev_length;      // best match length so far
            int nice_match = s.nice_match;          // stop if match long enough
            uint limit = s.strstart > (uint)(s.w_size - MIN_LOOKAHEAD) ? s.strstart - (uint)(s.w_size - MIN_LOOKAHEAD) : NIL;
            // Stop when cur_match becomes <= limit. To simplify the code,
            // we prevent matches with the string of window index 0.
            ushort[] prev = s.prev;
            uint wmask = s.w_mask;

            int strend_ind = (int)s.strstart + MAX_MATCH;
            byte scan_end1 = scan[scan_ind + best_len - 1];
            byte scan_end = scan[scan_ind + best_len];

            // The code is optimized for HASH_BITS >= 8 and MAX_MATCH-2 multiple of 16.
            // It is easy to get rid of this optimization if necessary.
            //Assert(s.hash_bits >= 8 && MAX_MATCH == 258, "Code too clever");

            // Do not waste too much time if we already have a good match:
            if (s.prev_length >= s.good_match) chain_length >>= 2;

            // Do not look for matches beyond the end of the input. This is necessary
            // to make deflate deterministic.
            if ((uint)nice_match > s.lookahead) nice_match = (int)s.lookahead;

            //Assert((uint)s.strstart <= s.window_size-MIN_LOOKAHEAD, "need lookahead");

            byte[] match = s.window;
            do
            {
                //Assert(cur_match<s.strstart, "no future");
                int match_ind = (int)cur_match;

                // Skip to next match if the match length cannot increase
                // or if the match length is less than 2.  Note that the checks below
                // for insufficient lookahead only occur occasionally for performance
                // reasons.  Therefore uninitialized memory will be accessed, and
                // conditional jumps will be made that depend on those values.
                // However the length of the match is limited to the lookahead, so
                // the output of deflate is not affected by the uninitialized values.
                if (match[match_ind + best_len] != scan_end || match[match_ind + best_len - 1] != scan_end1 ||
                    match[match_ind] != scan[scan_ind] || match[++match_ind] != scan[scan_ind + 1]) continue;

                // The check at best_len-1 can be removed because it will be made
                // again later. (This heuristic is not always a win.)
                // It is not necessary to compare scan[2] and match[2] since they
                // are always equal when the other bytes match, given that
                // the hash keys are equal and that HASH_BITS >= 8.
                scan_ind += 2;
                match_ind++;
                //Assert(scan[scan_ind]==match[match_ind], "match[2]?");

                // We check for insufficient lookahead only every 8th comparison;
                // the 256th check will be made at strstart+258.
                do
                {
                } while (scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                         scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                         scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                         scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                         scan_ind < strend_ind);

                //Assert(scan_ind <= (uint)(s.window_size-1), "wild scan");

                len = MAX_MATCH - (int)(strend_ind - scan_ind);
                scan_ind = strend_ind - MAX_MATCH;

                if (len > best_len)
                {
                    s.match_start = cur_match;
                    best_len = len;
                    if (len >= nice_match) break;

                    scan_end1 = scan[scan_ind + best_len - 1];
                    scan_end = scan[scan_ind + best_len];
                }
            } while ((cur_match = prev[cur_match & wmask]) > limit && --chain_length != 0);

            if ((uint)best_len <= s.lookahead) return (uint)best_len;
            return s.lookahead;
        }

        // ---------------------------------------------------------------------------
        // Optimized version for FASTEST only
        static uint longest_match_fast(deflate_state s, uint cur_match)
        {
            byte[] scan = s.window;
            int scan_ind = (int)s.strstart; // current string
            int strend_ind = (int)s.strstart + MAX_MATCH;

            // The code is optimized for HASH_BITS >= 8 and MAX_MATCH-2 multiple of 16.
            // It is easy to get rid of this optimization if necessary.
            //Assert(s.hash_bits >= 8 && MAX_MATCH == 258, "Code too clever");

            //Assert((uint)s.strstart <= s.window_size-MIN_LOOKAHEAD, "need lookahead");

            //Assert(cur_match < s.strstart, "no future");

            byte[] match = s.window;
            int match_ind = (int)cur_match;

            // Return failure if the match length is less than 2:
            if (match[match_ind] != scan[scan_ind] || match[match_ind + 1] != scan[scan_ind + 1]) return MIN_MATCH - 1;

            // The check at best_len-1 can be removed because it will be made
            // again later. (This heuristic is not always a win.)
            // It is not necessary to compare scan[2] and match[2] since they
            // are always equal when the other bytes match, given that
            // the hash keys are equal and that HASH_BITS >= 8.
            scan_ind += 2;
            match_ind += 2;
            //Assert(scan[scan_ind] == match[match_ind], "match[2]?");

            // We check for insufficient lookahead only every 8th comparison;
            // the 256th check will be made at strstart+258.
            do
            {
            } while (scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                     scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                     scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                     scan[++scan_ind] == match[++match_ind] && scan[++scan_ind] == match[++match_ind] &&
                     scan_ind < strend_ind);

            //Assert(scan_ind <= (uint)(s.window_size-1), "wild scan");

            int len = MAX_MATCH - (int)(strend_ind - scan_ind);// length of current match

            if (len < MIN_MATCH) return MIN_MATCH - 1;

            s.match_start = cur_match;
            return (uint)len <= s.lookahead ? (uint)len : s.lookahead;
        }

        // ===========================================================================
        // Fill the window when the lookahead becomes insufficient.
        // Updates strstart and lookahead.
        //
        // IN assertion: lookahead < MIN_LOOKAHEAD
        // OUT assertions: strstart <= window_size-MIN_LOOKAHEAD
        //    At least one byte has been read, or avail_in == 0; reads are
        //    performed for at least two bytes (required for the zip translate_eol
        //    option -- not supported here).
        static void fill_window(deflate_state s)
        {
            uint n, m;
            uint more;    // Amount of free space at the end of the window.
            uint wsize = s.w_size;

            do
            {
                more = (uint)(s.window_size - (uint)s.lookahead - (uint)s.strstart);

                // If the window is almost full and there is insufficient lookahead,
                // move the upper half to the lower one to make room in the upper half.
                if (s.strstart >= wsize + s.w_size - MIN_LOOKAHEAD)
                {
                    //was memcpy(s.window, s.window+wsize, (uint)wsize);
                    Array.Copy(s.window, wsize, s.window, 0, wsize);

                    s.match_start -= wsize;
                    s.strstart -= wsize; // we now have strstart >= MAX_DIST
                    s.block_start -= (int)wsize;

                   
                    n = s.hash_size;
                    uint p = n;
                    do
                    {
                        m = s.head[--p];
                        s.head[p] = (ushort)(m >= wsize ? m - wsize : NIL);
                    } while ((--n) != 0);

                    n = wsize;
                    p = n;
                    do
                    {
                        m = s.prev[--p];
                        s.prev[p] = (ushort)(m >= wsize ? m - wsize : NIL);
                        // If n is not on any hash chain, prev[n] is garbage but
                        // its value will never be used.
                    } while ((--n) != 0);
                    more += wsize;
                }
                if (s.strm.avail_in == 0) return;

                // If there was no sliding:
                //    strstart <= WSIZE+MAX_DIST-1 && lookahead <= MIN_LOOKAHEAD - 1 &&
                //    more == window_size - lookahead - strstart
                // => more >= window_size - (MIN_LOOKAHEAD-1 + WSIZE + MAX_DIST-1)
                // => more >= window_size - 2*WSIZE + 2
                // In the BIG_MEM or MMAP case (not yet supported),
                //   window_size == input_size + MIN_LOOKAHEAD  &&
                //   strstart + s.lookahead <= input_size => more >= MIN_LOOKAHEAD.
                // Otherwise, window_size == 2*WSIZE so more >= 2.
                // If there was sliding, more >= WSIZE. So in all cases, more >= 2.
                //Assert(more>=2, "more < 2");

                n = (uint)read_buf(s.strm, s.window, (int)(s.strstart + s.lookahead), more);
                s.lookahead += n;

                // Initialize the hash value now that we have some input:
                if (s.lookahead >= MIN_MATCH)
                {
                    s.ins_h = s.window[s.strstart];
                    //was UPDATE_HASH(s, s.ins_h, s.window[s.strstart+1]);
                    s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[s.strstart + 1]) & s.hash_mask;

                }
                // If the whole input has less than MIN_MATCH bytes, ins_h is garbage,
                // but this is not important since only literal bytes will be emitted.
            } while (s.lookahead < MIN_LOOKAHEAD && s.strm.avail_in != 0);

            // If the WIN_INIT bytes after the end of the current data have never been
            // written, then zero those bytes in order to avoid memory check reports of
            // the use of uninitialized (or uninitialised as Julian writes) bytes by
            // the longest match routines.  Update the high water mark for the next
            // time through here.  WIN_INIT is set to MAX_MATCH since the longest match
            // routines allow scanning to strstart + MAX_MATCH, ignoring lookahead.
            if (s.high_water < s.window_size)
            {
                uint curr = s.strstart + s.lookahead;
                uint init;

                if (s.high_water < curr)
                {
                    // Previous high water mark below current data -- zero WIN_INIT
                    // bytes or up to end of window, whichever is less.
                    init = s.window_size - curr;
                    if (init > WIN_INIT) init = WIN_INIT;
                    for (int i = 0; i < init; i++) s.window[curr + i] = 0;
                    s.high_water = curr + init;
                }
                else if (s.high_water < curr + WIN_INIT)
                {
                    // High water mark at or above current data, but below current data
                    // plus WIN_INIT -- zero out to current data plus WIN_INIT, or up
                    // to end of window, whichever is less.
                    init = curr + WIN_INIT - s.high_water;
                    if (init > s.window_size - s.high_water) init = s.window_size - s.high_water;
                    for (int i = 0; i < init; i++) s.window[s.high_water + i] = 0;
                    s.high_water += init;
                }
            }
        }

        // ===========================================================================
        // Flush the current block, with given end-of-file flag.
        // IN assertion: strstart is set to the end of the current match.
        //#define FLUSH_BLOCK_ONLY(s, last) \
        //{ \
        //    _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0?s.block_start:0, \
        //		(uint)((int)s.strstart - s.block_start), (last)); \
        //    s.block_start = s.strstart; \
        //    flush_pending(s.strm); \
        //    Tracev((stderr,"[FLUSH]")); \
        //}

        // Same but force premature exit if necessary.
        //#define FLUSH_BLOCK(s, last) \
        //{ \
        //    _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0?s.block_start:0, \
        //		(uint)((int)s.strstart - s.block_start), (last)); \
        //    s.block_start = s.strstart; \
        //    flush_pending(s.strm); \
        //    Tracev((stderr,"[FLUSH]")); \
        //    if (s.strm.avail_out == 0) return (last) ? finish_started : need_more; \
        //}

        // ===========================================================================
        // Copy without compression as much as possible from the input stream, return
        // the current block state.
        // This function does not insert new strings in the dictionary since
        // uncompressible data is probably not useful. This function is used
        // only for the level=0 compression option.
        // NOTE: this function should be optimized to avoid extra copying from
        // window to pending_buf.
        static block_state deflate_stored(deflate_state s, int flush)
        {
            // Stored blocks are limited to 0xffff bytes, pending_buf is limited
            // to pending_buf_size, and each stored block has a 5 byte header:
            uint max_block_size = 0xffff;
            uint max_start;

            if (max_block_size > s.pending_buf_size - 5) max_block_size = s.pending_buf_size - 5;

            // Copy as much as possible from input to output:
            for (; ; )
            {
                // Fill the window as much as possible:
                if (s.lookahead <= 1)
                {
                    //Assert(s.strstart<s.w_size+MAX_DIST(s)||s.block_start>=(int)s.w_size, "slide too late");

                    fill_window(s);
                    if (s.lookahead == 0 && flush == Z_NO_FLUSH) return block_state.need_more;

                    if (s.lookahead == 0) break; // flush the current block
                }
                //Assert(s.block_start>=0, "block gone");

                s.strstart += s.lookahead;
                s.lookahead = 0;

                // Emit a stored block if pending_buf will be full:
                max_start = (uint)s.block_start + max_block_size;
                if (s.strstart == 0 || (uint)s.strstart >= max_start)
                {
                    // strstart == 0 is possible when wraparound on 16-bit machine
                    s.lookahead = (uint)(s.strstart - max_start);
                    s.strstart = (uint)max_start;

                    //was FLUSH_BLOCK(s, 0);
                    _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                        (uint)((int)s.strstart - s.block_start), 0);
                    s.block_start = (int)s.strstart;
                    flush_pending(s.strm);
                    //Tracev((stderr,"[FLUSH]"));
                    if (s.strm.avail_out == 0) return block_state.need_more;
                }
                // Flush if we may have to slide, otherwise block_start may become
                // negative and the data will be gone:
                if (s.strstart - (uint)s.block_start >= (s.w_size - MIN_LOOKAHEAD))
                {
                    //was FLUSH_BLOCK(s, 0);
                    _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                        (uint)((int)s.strstart - s.block_start), 0);
                    s.block_start = (int)s.strstart;
                    flush_pending(s.strm);
                    //Tracev((stderr,"[FLUSH]"));
                    if (s.strm.avail_out == 0) return block_state.need_more;
                }
            }

            //was FLUSH_BLOCK(s, flush==Z_FINISH);
            _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                (uint)((int)s.strstart - s.block_start), flush == Z_FINISH ? 1 : 0);
            s.block_start = (int)s.strstart;
            flush_pending(s.strm);
            //Tracev((stderr,"[FLUSH]"));
            if (s.strm.avail_out == 0) return flush == Z_FINISH ? block_state.finish_started : block_state.need_more;

            return flush == Z_FINISH ? block_state.finish_done : block_state.block_done;
        }

        // ===========================================================================
        // Compress as much as possible from the input stream, return the current
        // block state.
        // This function does not perform lazy evaluation of matches and inserts
        // new strings in the dictionary only for unmatched strings or for short
        // matches. It is used only for the fast compression options.
        static block_state deflate_fast(deflate_state s, int flush)
        {
            uint hash_head = NIL; // head of the hash chain
            int bflush;           // set if current block must be flushed

            for (; ; )
            {
                // Make sure that we always have enough lookahead, except
                // at the end of the input file. We need MAX_MATCH bytes
                // for the next match, plus MIN_MATCH bytes to insert the
                // string following the next match.
                if (s.lookahead < MIN_LOOKAHEAD)
                {
                    fill_window(s);
                    if (s.lookahead < MIN_LOOKAHEAD && flush == Z_NO_FLUSH) return block_state.need_more;
                    if (s.lookahead == 0) break; // flush the current block
                }

                // Insert the string window[strstart .. strstart+2] in the
                // dictionary, and set hash_head to the head of the hash chain:
                hash_head = NIL;
                if (s.lookahead >= MIN_MATCH)
                {
                    //was INSERT_STRING(s, s.strstart, hash_head);
                    s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[s.strstart + (MIN_MATCH - 1)]) & s.hash_mask;
                    hash_head = s.prev[s.strstart & s.w_mask] = s.head[s.ins_h];
                    s.head[s.ins_h] = (ushort)s.strstart;
                }

                // Find the longest match, discarding those <= prev_length.
                // At this point we have always match_length < MIN_MATCH
                if (hash_head != NIL && s.strstart - hash_head <= (s.w_size - MIN_LOOKAHEAD))
                {
                    // To simplify the code, we prevent matches with the string
                    // of window index 0 (in particular we have to avoid a match
                    // of the string with itself at the start of the input file).
                    s.match_length = longest_match_fast(s, hash_head);
                    // longest_match_fast() sets match_start
                }
                if (s.match_length >= MIN_MATCH)
                {
                    //was _tr_tally_dist(s, s.strstart - s.match_start, s.match_length - MIN_MATCH, bflush);
                    {
                        byte len = (byte)(s.match_length - MIN_MATCH);
                        ushort dist = (ushort)(s.strstart - s.match_start);
                        s.d_buf[s.last_lit] = dist;
                        s.l_buf[s.last_lit++] = len;
                        dist--;
                        s.dyn_ltree[_length_code[len] + LITERALS + 1].Freq++;
                        s.dyn_dtree[(dist < 256 ? _dist_code[dist] : _dist_code[256 + (dist >> 7)])].Freq++;
                        bflush = (s.last_lit == s.lit_bufsize - 1) ? 1 : 0;
                    }

                    s.lookahead -= s.match_length;

                    // Insert new strings in the hash table only if the match length
                    // is not too large. This saves time but degrades compression.
                    if (s.match_length <= s.max_lazy_match && s.lookahead >= MIN_MATCH) // max_lazy_match was max_insert_length as #define
                    {
                        s.match_length--; // string at strstart already in table
                        do
                        {
                            s.strstart++;
                            //was INSERT_STRING(s, s.strstart, hash_head);
                            s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[s.strstart + (MIN_MATCH - 1)]) & s.hash_mask;
                            hash_head = s.prev[s.strstart & s.w_mask] = s.head[s.ins_h];
                            s.head[s.ins_h] = (ushort)s.strstart;

                            // strstart never exceeds WSIZE-MAX_MATCH, so there are
                            // always MIN_MATCH bytes ahead.
                        } while (--s.match_length != 0);
                        s.strstart++;
                    }
                    else
                    {
                        s.strstart += s.match_length;
                        s.match_length = 0;
                        s.ins_h = s.window[s.strstart];
                        //was UPDATE_HASH(s, s.ins_h, s.window[s.strstart+1]);
                        s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[s.strstart + 1]) & s.hash_mask;

                        // If lookahead < MIN_MATCH, ins_h is garbage, but it does not
                        // matter since it will be recomputed at next deflate call.
                    }
                }
                else
                {
                    // No match, output a literal byte
                    //Tracevv((stderr,"%c", s.window[s.strstart]));

                    //was _tr_tally_lit (s, s.window[s.strstart], bflush);
                    {
                        byte cc = s.window[s.strstart];
                        s.d_buf[s.last_lit] = 0;
                        s.l_buf[s.last_lit++] = cc;
                        s.dyn_ltree[cc].Freq++;
                        bflush = (s.last_lit == s.lit_bufsize - 1) ? 1 : 0;
                    }

                    s.lookahead--;
                    s.strstart++;
                }

                if (bflush != 0)
                {
                    //was FLUSH_BLOCK(s, 0);
                    _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                        (uint)((int)s.strstart - s.block_start), 0);
                    s.block_start = (int)s.strstart;
                    flush_pending(s.strm);
                    //Tracev((stderr,"[FLUSH]"));
                    if (s.strm.avail_out == 0) return block_state.need_more;
                }
            }
            //was FLUSH_BLOCK(s, flush==Z_FINISH);
            _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                (uint)((int)s.strstart - s.block_start), flush == Z_FINISH ? 1 : 0);
            s.block_start = (int)s.strstart;
            flush_pending(s.strm);
            //Tracev((stderr,"[FLUSH]"));
            if (s.strm.avail_out == 0) return flush == Z_FINISH ? block_state.finish_started : block_state.need_more;

            return flush == Z_FINISH ? block_state.finish_done : block_state.block_done;
        }

        // ===========================================================================
        // Same as above, but achieves better compression. We use a lazy
        // evaluation for matches: a match is finally adopted only if there is
        // no better match at the next window position.
        static block_state deflate_slow(deflate_state s, int flush)
        {
            uint hash_head = NIL;   // head of hash chain
            int bflush;         // set if current block must be flushed

            // Process the input block.
            for (; ; )
            {
                // Make sure that we always have enough lookahead, except
                // at the end of the input file. We need MAX_MATCH bytes
                // for the next match, plus MIN_MATCH bytes to insert the
                // string following the next match.
                if (s.lookahead < MIN_LOOKAHEAD)
                {
                    fill_window(s);
                    if (s.lookahead < MIN_LOOKAHEAD && flush == Z_NO_FLUSH) return block_state.need_more;
                    if (s.lookahead == 0) break; // flush the current block
                }

                // Insert the string window[strstart .. strstart+2] in the
                // dictionary, and set hash_head to the head of the hash chain:
                hash_head = NIL;
                if (s.lookahead >= MIN_MATCH)
                {
                    //was INSERT_STRING(s, s.strstart, hash_head);
                    s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[s.strstart + (MIN_MATCH - 1)]) & s.hash_mask;
                    hash_head = s.prev[s.strstart & s.w_mask] = s.head[s.ins_h];
                    s.head[s.ins_h] = (ushort)s.strstart;
                }

                // Find the longest match, discarding those <= prev_length.
                s.prev_length = s.match_length;
                s.prev_match = s.match_start;
                s.match_length = MIN_MATCH - 1;

                if (hash_head != NIL && s.prev_length < s.max_lazy_match && s.strstart - hash_head <= (s.w_size - MIN_LOOKAHEAD))
                {
                    // To simplify the code, we prevent matches with the string
                    // of window index 0 (in particular we have to avoid a match
                    // of the string with itself at the start of the input file).
                    s.match_length = longest_match(s, hash_head);
                    // longest_match() sets match_start

                    if (s.match_length <= 5 && (s.strategy == Z_FILTERED ||
                        (s.match_length == MIN_MATCH && s.strstart - s.match_start > TOO_FAR)))
                    {
                        // If prev_match is also MIN_MATCH, match_start is garbage
                        // but we will ignore the current match anyway.
                        s.match_length = MIN_MATCH - 1;
                    }
                }

                // If there was a match at the previous step and the current
                // match is not better, output the previous match:
                if (s.prev_length >= MIN_MATCH && s.match_length <= s.prev_length)
                {
                    uint max_insert = s.strstart + s.lookahead - MIN_MATCH;
                    // Do not insert strings in hash table beyond this.

                    //was _tr_tally_dist(s, s.strstart -1 - s.prev_match, s.prev_length - MIN_MATCH, bflush);
                    {
                        byte len = (byte)(s.prev_length - MIN_MATCH);
                        ushort dist = (ushort)(s.strstart - 1 - s.prev_match);
                        s.d_buf[s.last_lit] = dist;
                        s.l_buf[s.last_lit++] = len;
                        dist--;
                        s.dyn_ltree[_length_code[len] + LITERALS + 1].Freq++;
                        s.dyn_dtree[(dist < 256 ? _dist_code[dist] : _dist_code[256 + (dist >> 7)])].Freq++;
                        bflush = (s.last_lit == s.lit_bufsize - 1) ? 1 : 0;
                    }

                    // Insert in hash table all strings up to the end of the match.
                    // strstart-1 and strstart are already inserted. If there is not
                    // enough lookahead, the last two strings are not inserted in
                    // the hash table.
                    s.lookahead -= s.prev_length - 1;
                    s.prev_length -= 2;
                    do
                    {
                        if (++s.strstart <= max_insert)
                        {
                            //was INSERT_STRING(s, s.strstart, hash_head);
                            s.ins_h = ((s.ins_h << (int)s.hash_shift) ^ s.window[s.strstart + (MIN_MATCH - 1)]) & s.hash_mask;
                            hash_head = s.prev[s.strstart & s.w_mask] = s.head[s.ins_h];
                            s.head[s.ins_h] = (ushort)s.strstart;
                        }
                    } while (--s.prev_length != 0);
                    s.match_available = 0;
                    s.match_length = MIN_MATCH - 1;
                    s.strstart++;

                    if (bflush != 0)
                    {
                        //was FLUSH_BLOCK(s, 0);
                        _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                            (uint)((int)s.strstart - s.block_start), 0);
                        s.block_start = (int)s.strstart;
                        flush_pending(s.strm);
                        //Tracev((stderr,"[FLUSH]"));
                        if (s.strm.avail_out == 0) return block_state.need_more;
                    }
                }
                else if (s.match_available != 0)
                {
                    // If there was no match at the previous position, output a
                    // single literal. If there was a match but the current match
                    // is longer, truncate the previous match to a single literal.
                    //Tracevv((stderr,"%c", s.window[s.strstart-1]));

                    //was _tr_tally_lit(s, s.window[s.strstart-1], bflush);
                    {
                        byte cc = s.window[s.strstart - 1];
                        s.d_buf[s.last_lit] = 0;
                        s.l_buf[s.last_lit++] = cc;
                        s.dyn_ltree[cc].Freq++;
                        bflush = (s.last_lit == s.lit_bufsize - 1) ? 1 : 0;
                    }

                    if (bflush != 0)
                    {
                        //was FLUSH_BLOCK_ONLY(s, 0);
                        _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                            (uint)((int)s.strstart - s.block_start), 0);
                        s.block_start = (int)s.strstart;
                        flush_pending(s.strm);
                        //Tracev((stderr,"[FLUSH]"));
                    }
                    s.strstart++;
                    s.lookahead--;
                    if (s.strm.avail_out == 0) return block_state.need_more;
                }
                else
                {
                    // There is no previous match to compare with, wait for
                    // the next step to decide.
                    s.match_available = 1;
                    s.strstart++;
                    s.lookahead--;
                }
            }
            //Assert(flush!=Z_NO_FLUSH, "no flush?");
            if (s.match_available != 0)
            {
                //Tracevv((stderr,"%c", s.window[s.strstart-1]));

                //was _tr_tally_lit(s, s.window[s.strstart-1], bflush);
                {
                    byte cc = s.window[s.strstart - 1];
                    s.d_buf[s.last_lit] = 0;
                    s.l_buf[s.last_lit++] = cc;
                    s.dyn_ltree[cc].Freq++;
                    bflush = (s.last_lit == s.lit_bufsize - 1) ? 1 : 0;
                }

                s.match_available = 0;
            }
            //was FLUSH_BLOCK(s, flush==Z_FINISH);
            _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                (uint)((int)s.strstart - s.block_start), flush == Z_FINISH ? 1 : 0);
            s.block_start = (int)s.strstart;
            flush_pending(s.strm);
            //Tracev((stderr,"[FLUSH]"));
            if (s.strm.avail_out == 0) return flush == Z_FINISH ? block_state.finish_started : block_state.need_more;

            return flush == Z_FINISH ? block_state.finish_done : block_state.block_done;
        }

        // ===========================================================================
        // For Z_RLE, simply look for runs of bytes, generate matches only of distance
        // one.  Do not maintain a hash table.  (It will be regenerated if this run of
        // deflate switches away from Z_RLE.)
        static block_state deflate_rle(deflate_state s, int flush)
        {
            bool bflush;            // set if current block must be flushed
            uint prev;              // byte at distance one to match
            int scan, strend;   // scan goes up to strend for length of run

            for (; ; )
            {
                // Make sure that we always have enough lookahead, except
                // at the end of the input file. We need MAX_MATCH bytes
                // for the longest encodable run.
                if (s.lookahead < MAX_MATCH)
                {
                    fill_window(s);
                    if (s.lookahead < MAX_MATCH && flush == Z_NO_FLUSH) return block_state.need_more;
                    if (s.lookahead == 0) break; // flush the current block
                }

                // See how many times the previous byte repeats
                s.match_length = 0;
                if (s.lookahead >= MIN_MATCH && s.strstart > 0)
                {
                    scan = (int)(s.strstart - 1);
                    prev = s.window[scan];
                    if (prev == s.window[++scan] && prev == s.window[++scan] && prev == s.window[++scan])
                    {
                        strend = (int)(s.strstart + MAX_MATCH);
                        do
                        {
                        } while (prev == s.window[++scan] && prev == s.window[++scan] &&
                                prev == s.window[++scan] && prev == s.window[++scan] &&
                                prev == s.window[++scan] && prev == s.window[++scan] &&
                                prev == s.window[++scan] && prev == s.window[++scan] &&
                                scan < strend);
                        s.match_length = MAX_MATCH - (uint)(strend - scan);
                        if (s.match_length > s.lookahead) s.match_length = s.lookahead;
                    }
                }

                // Emit match if have run of MIN_MATCH or longer, else emit literal
                if (s.match_length >= MIN_MATCH)
                {
                    //was _tr_tally_dist(s, 1, s.match_length-MIN_MATCH, bflush);
                    {
                        byte len = (byte)(s.match_length - MIN_MATCH);
                        ushort dist = 1;
                        s.d_buf[s.last_lit] = dist;
                        s.l_buf[s.last_lit++] = len;
                        dist--;
                        s.dyn_ltree[_length_code[len] + LITERALS + 1].Freq++;
                        s.dyn_dtree[(dist < 256 ? _dist_code[dist] : _dist_code[256 + (dist >> 7)])].Freq++;
                        bflush = (s.last_lit == s.lit_bufsize - 1) ? true : false;
                    }

                    s.lookahead -= s.match_length;
                    s.strstart += s.match_length;
                    s.match_length = 0;
                }
                else
                {
                    // No match, output a literal byte
                    //Tracevv((stderr,"%c", s.window[s.strstart]));
                    //was _tr_tally_lit(s, s.window[s.strstart], bflush);
                    {
                        byte cc = s.window[s.strstart];
                        s.d_buf[s.last_lit] = 0;
                        s.l_buf[s.last_lit++] = cc;
                        s.dyn_ltree[cc].Freq++;
                        bflush = (s.last_lit == s.lit_bufsize - 1) ? true : false;
                    }

                    s.lookahead--;
                    s.strstart++;
                }
                if (bflush)
                {
                    // FLUSH_BLOCK(s, 0);
                    _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                        (uint)((int)s.strstart - s.block_start), 0);
                    s.block_start = (int)s.strstart;
                    flush_pending(s.strm);
                    //Tracev((stderr,"[FLUSH]"));
                    if (s.strm.avail_out == 0) return block_state.need_more;
                }
            }

            //was FLUSH_BLOCK(s, flush==Z_FINISH);
            _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                (uint)((int)s.strstart - s.block_start), flush == Z_FINISH ? 1 : 0);
            s.block_start = (int)s.strstart;
            flush_pending(s.strm);
            //Tracev((stderr,"[FLUSH]"));
            if (s.strm.avail_out == 0) return flush == Z_FINISH ? block_state.finish_started : block_state.need_more;

            return flush == Z_FINISH ? block_state.finish_done : block_state.block_done;
        }

        // ===========================================================================
        // For Z_HUFFMAN_ONLY, do not look for matches.  Do not maintain a hash table.
        // (It will be regenerated if this run of deflate switches away from Huffman.)
        static block_state deflate_huff(deflate_state s, int flush)
        {
            bool bflush;                // set if current block must be flushed

            for (; ; )
            {
                // Make sure that we have a literal to write.
                if (s.lookahead == 0)
                {
                    fill_window(s);
                    if (s.lookahead == 0)
                    {
                        if (flush == Z_NO_FLUSH)
                            return block_state.need_more;
                        break; // flush the current block
                    }
                }

                // Output a literal byte
                s.match_length = 0;
                //Tracevv((stderr,"%c", s.window[s.strstart]));

                //was _tr_tally_lit(s, s.window[s.strstart], bflush);
                {
                    byte cc = s.window[s.strstart];
                    s.d_buf[s.last_lit] = 0;
                    s.l_buf[s.last_lit++] = cc;
                    s.dyn_ltree[cc].Freq++;
                    bflush = (s.last_lit == s.lit_bufsize - 1) ? true : false;
                }

                s.lookahead--;
                s.strstart++;
                if (bflush)
                {
                    // FLUSH_BLOCK(s, 0);
                    _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                        (uint)((int)s.strstart - s.block_start), 0);
                    s.block_start = (int)s.strstart;
                    flush_pending(s.strm);
                    //Tracev((stderr,"[FLUSH]"));
                    if (s.strm.avail_out == 0) return block_state.need_more;
                }
            }

            //was FLUSH_BLOCK(s, flush==Z_FINISH);
            _tr_flush_block(s, s.block_start >= 0 ? s.window : null, s.block_start >= 0 ? s.block_start : 0,
                (uint)((int)s.strstart - s.block_start), flush == Z_FINISH ? 1 : 0);
            s.block_start = (int)s.strstart;
            flush_pending(s.strm);
            //Tracev((stderr,"[FLUSH]"));
            if (s.strm.avail_out == 0) return flush == Z_FINISH ? block_state.finish_started : block_state.need_more;

            return flush == Z_FINISH ? block_state.finish_done : block_state.block_done;
        }
    }
}
