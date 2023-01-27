using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.IO.Zlib
{
    public static partial class ZLib
    {
        // ===========================================================================
        // Constants
        //

        // Bit length codes must not exceed MAX_BL_BITS bits
        private const int MAX_BL_BITS = 7;

        // end of block literal code
        private const int END_BLOCK = 256;

        // repeat previous bit length 3-6 times (2 bits of repeat count)
        private const int REP_3_6 = 16;

        // repeat a zero length 3-10 times (3 bits of repeat count)
        private const int REPZ_3_10 = 17;

        // repeat a zero length 11-138 times (7 bits of repeat count)
        private const int REPZ_11_138 = 18;

        // extra bits for each length code
        private static readonly int[] extra_lbits = new int[LENGTH_CODES] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0 };

        // extra bits for each distance code
        private static readonly int[] extra_dbits = new int[D_CODES] { 0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13 };

        // extra bits for each bit length code
        private static readonly int[] extra_blbits = new int[BL_CODES] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7 };

        // The lengths of the bit length codes are sent in order of decreasing
        // probability, to avoid transmitting the lengths for unused bit length codes.
        private static readonly byte[] bl_order = new byte[BL_CODES] { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };

        // Number of bits used within bi_buf. (bi_buf might be implemented on
        // more than 16 bits on some systems.)
        private const int Buf_size = 8 * 2 * sizeof(byte);

        // ===========================================================================
        // Local data. These are initialized only once.

        // see definition of array dist_code below
        private const int DIST_CODE_LEN = 512;

        #region Tables
        private static readonly ct_data[] static_ltree = new ct_data[L_CODES + 2]
        {
            new ct_data( 12, 8), new ct_data(140, 8), new ct_data( 76, 8), new ct_data(204, 8),
            new ct_data( 44, 8), new ct_data(172, 8), new ct_data(108, 8), new ct_data(236, 8),
            new ct_data( 28, 8), new ct_data(156, 8), new ct_data( 92, 8), new ct_data(220, 8),
            new ct_data( 60, 8), new ct_data(188, 8), new ct_data(124, 8), new ct_data(252, 8),
            new ct_data(  2, 8), new ct_data(130, 8), new ct_data( 66, 8), new ct_data(194, 8),
            new ct_data( 34, 8), new ct_data(162, 8), new ct_data( 98, 8), new ct_data(226, 8),
            new ct_data( 18, 8), new ct_data(146, 8), new ct_data( 82, 8), new ct_data(210, 8),
            new ct_data( 50, 8), new ct_data(178, 8), new ct_data(114, 8), new ct_data(242, 8),
            new ct_data( 10, 8), new ct_data(138, 8), new ct_data( 74, 8), new ct_data(202, 8),
            new ct_data( 42, 8), new ct_data(170, 8), new ct_data(106, 8), new ct_data(234, 8),
            new ct_data( 26, 8), new ct_data(154, 8), new ct_data( 90, 8), new ct_data(218, 8),
            new ct_data( 58, 8), new ct_data(186, 8), new ct_data(122, 8), new ct_data(250, 8),
            new ct_data(  6, 8), new ct_data(134, 8), new ct_data( 70, 8), new ct_data(198, 8),
            new ct_data( 38, 8), new ct_data(166, 8), new ct_data(102, 8), new ct_data(230, 8),
            new ct_data( 22, 8), new ct_data(150, 8), new ct_data( 86, 8), new ct_data(214, 8),
            new ct_data( 54, 8), new ct_data(182, 8), new ct_data(118, 8), new ct_data(246, 8),
            new ct_data( 14, 8), new ct_data(142, 8), new ct_data( 78, 8), new ct_data(206, 8),
            new ct_data( 46, 8), new ct_data(174, 8), new ct_data(110, 8), new ct_data(238, 8),
            new ct_data( 30, 8), new ct_data(158, 8), new ct_data( 94, 8), new ct_data(222, 8),
            new ct_data( 62, 8), new ct_data(190, 8), new ct_data(126, 8), new ct_data(254, 8),
            new ct_data(  1, 8), new ct_data(129, 8), new ct_data( 65, 8), new ct_data(193, 8),
            new ct_data( 33, 8), new ct_data(161, 8), new ct_data( 97, 8), new ct_data(225, 8),
            new ct_data( 17, 8), new ct_data(145, 8), new ct_data( 81, 8), new ct_data(209, 8),
            new ct_data( 49, 8), new ct_data(177, 8), new ct_data(113, 8), new ct_data(241, 8),
            new ct_data(  9, 8), new ct_data(137, 8), new ct_data( 73, 8), new ct_data(201, 8),
            new ct_data( 41, 8), new ct_data(169, 8), new ct_data(105, 8), new ct_data(233, 8),
            new ct_data( 25, 8), new ct_data(153, 8), new ct_data( 89, 8), new ct_data(217, 8),
            new ct_data( 57, 8), new ct_data(185, 8), new ct_data(121, 8), new ct_data(249, 8),
            new ct_data(  5, 8), new ct_data(133, 8), new ct_data( 69, 8), new ct_data(197, 8),
            new ct_data( 37, 8), new ct_data(165, 8), new ct_data(101, 8), new ct_data(229, 8),
            new ct_data( 21, 8), new ct_data(149, 8), new ct_data( 85, 8), new ct_data(213, 8),
            new ct_data( 53, 8), new ct_data(181, 8), new ct_data(117, 8), new ct_data(245, 8),
            new ct_data( 13, 8), new ct_data(141, 8), new ct_data( 77, 8), new ct_data(205, 8),
            new ct_data( 45, 8), new ct_data(173, 8), new ct_data(109, 8), new ct_data(237, 8),
            new ct_data( 29, 8), new ct_data(157, 8), new ct_data( 93, 8), new ct_data(221, 8),
            new ct_data( 61, 8), new ct_data(189, 8), new ct_data(125, 8), new ct_data(253, 8),
            new ct_data( 19, 9), new ct_data(275, 9), new ct_data(147, 9), new ct_data(403, 9),
            new ct_data( 83, 9), new ct_data(339, 9), new ct_data(211, 9), new ct_data(467, 9),
            new ct_data( 51, 9), new ct_data(307, 9), new ct_data(179, 9), new ct_data(435, 9),
            new ct_data(115, 9), new ct_data(371, 9), new ct_data(243, 9), new ct_data(499, 9),
            new ct_data( 11, 9), new ct_data(267, 9), new ct_data(139, 9), new ct_data(395, 9),
            new ct_data( 75, 9), new ct_data(331, 9), new ct_data(203, 9), new ct_data(459, 9),
            new ct_data( 43, 9), new ct_data(299, 9), new ct_data(171, 9), new ct_data(427, 9),
            new ct_data(107, 9), new ct_data(363, 9), new ct_data(235, 9), new ct_data(491, 9),
            new ct_data( 27, 9), new ct_data(283, 9), new ct_data(155, 9), new ct_data(411, 9),
            new ct_data( 91, 9), new ct_data(347, 9), new ct_data(219, 9), new ct_data(475, 9),
            new ct_data( 59, 9), new ct_data(315, 9), new ct_data(187, 9), new ct_data(443, 9),
            new ct_data(123, 9), new ct_data(379, 9), new ct_data(251, 9), new ct_data(507, 9),
            new ct_data(  7, 9), new ct_data(263, 9), new ct_data(135, 9), new ct_data(391, 9),
            new ct_data( 71, 9), new ct_data(327, 9), new ct_data(199, 9), new ct_data(455, 9),
            new ct_data( 39, 9), new ct_data(295, 9), new ct_data(167, 9), new ct_data(423, 9),
            new ct_data(103, 9), new ct_data(359, 9), new ct_data(231, 9), new ct_data(487, 9),
            new ct_data( 23, 9), new ct_data(279, 9), new ct_data(151, 9), new ct_data(407, 9),
            new ct_data( 87, 9), new ct_data(343, 9), new ct_data(215, 9), new ct_data(471, 9),
            new ct_data( 55, 9), new ct_data(311, 9), new ct_data(183, 9), new ct_data(439, 9),
            new ct_data(119, 9), new ct_data(375, 9), new ct_data(247, 9), new ct_data(503, 9),
            new ct_data( 15, 9), new ct_data(271, 9), new ct_data(143, 9), new ct_data(399, 9),
            new ct_data( 79, 9), new ct_data(335, 9), new ct_data(207, 9), new ct_data(463, 9),
            new ct_data( 47, 9), new ct_data(303, 9), new ct_data(175, 9), new ct_data(431, 9),
            new ct_data(111, 9), new ct_data(367, 9), new ct_data(239, 9), new ct_data(495, 9),
            new ct_data( 31, 9), new ct_data(287, 9), new ct_data(159, 9), new ct_data(415, 9),
            new ct_data( 95, 9), new ct_data(351, 9), new ct_data(223, 9), new ct_data(479, 9),
            new ct_data( 63, 9), new ct_data(319, 9), new ct_data(191, 9), new ct_data(447, 9),
            new ct_data(127, 9), new ct_data(383, 9), new ct_data(255, 9), new ct_data(511, 9),
            new ct_data(  0, 7), new ct_data( 64, 7), new ct_data( 32, 7), new ct_data( 96, 7),
            new ct_data( 16, 7), new ct_data( 80, 7), new ct_data( 48, 7), new ct_data(112, 7),
            new ct_data(  8, 7), new ct_data( 72, 7), new ct_data( 40, 7), new ct_data(104, 7),
            new ct_data( 24, 7), new ct_data( 88, 7), new ct_data( 56, 7), new ct_data(120, 7),
            new ct_data(  4, 7), new ct_data( 68, 7), new ct_data( 36, 7), new ct_data(100, 7),
            new ct_data( 20, 7), new ct_data( 84, 7), new ct_data( 52, 7), new ct_data(116, 7),
            new ct_data(  3, 8), new ct_data(131, 8), new ct_data( 67, 8), new ct_data(195, 8),
            new ct_data( 35, 8), new ct_data(163, 8), new ct_data( 99, 8), new ct_data(227, 8)
        };

        private static readonly ct_data[] static_dtree = new ct_data[D_CODES]
        {
            new ct_data( 0, 5), new ct_data(16, 5), new ct_data( 8, 5), new ct_data(24, 5), new ct_data( 4, 5),
            new ct_data(20, 5), new ct_data(12, 5), new ct_data(28, 5), new ct_data( 2, 5), new ct_data(18, 5),
            new ct_data(10, 5), new ct_data(26, 5), new ct_data( 6, 5), new ct_data(22, 5), new ct_data(14, 5),
            new ct_data(30, 5), new ct_data( 1, 5), new ct_data(17, 5), new ct_data( 9, 5), new ct_data(25, 5),
            new ct_data( 5, 5), new ct_data(21, 5), new ct_data(13, 5), new ct_data(29, 5), new ct_data( 3, 5),
            new ct_data(19, 5), new ct_data(11, 5), new ct_data(27, 5), new ct_data( 7, 5), new ct_data(23, 5)
        };

        private static readonly byte[] _dist_code = new byte[DIST_CODE_LEN]
        {
             0,  1,  2,  3,  4,  4,  5,  5,  6,  6,  6,  6,  7,  7,  7,  7,  8,  8,  8,  8,
             8,  8,  8,  8,  9,  9,  9,  9,  9,  9,  9,  9, 10, 10, 10, 10, 10, 10, 10, 10,
            10, 10, 10, 10, 10, 10, 10, 10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
            11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
            12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 13, 13, 13, 13,
            13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
            13, 13, 13, 13, 13, 13, 13, 13, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,  0,  0, 16, 17,
            18, 18, 19, 19, 20, 20, 20, 20, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22, 22, 22,
            23, 23, 23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29
        };

        private static readonly byte[] _length_code = new byte[MAX_MATCH - MIN_MATCH + 1]
        {
             0,  1,  2,  3,  4,  5,  6,  7,  8,  8,  9,  9, 10, 10, 11, 11, 12, 12, 12, 12,
            13, 13, 13, 13, 14, 14, 14, 14, 15, 15, 15, 15, 16, 16, 16, 16, 16, 16, 16, 16,
            17, 17, 17, 17, 17, 17, 17, 17, 18, 18, 18, 18, 18, 18, 18, 18, 19, 19, 19, 19,
            19, 19, 19, 19, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20,
            21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 22, 22, 22, 22,
            22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23,
            23, 23, 23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28
        };

        private static readonly int[] base_length = new int[LENGTH_CODES]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 20, 24, 28, 32, 40, 48, 56,
            64, 80, 96, 112, 128, 160, 192, 224, 0
        };

        private static readonly int[] base_dist = new int[D_CODES]
        {
               0,     1,     2,     3,     4,     6,     8,    12,    16,    24,
              32,    48,    64,    96,   128,   192,   256,   384,   512,   768,
            1024,  1536,  2048,  3072,  4096,  6144,  8192, 12288, 16384, 24576
        };
        #endregion

        class static_tree_desc
        {
            public readonly ct_data[] static_tree;  // static tree or NULL
            public readonly int[] extra_bits;       // extra bits for each code or NULL
            public int extra_base;                  // base index for extra_bits
            public int elems;                       // max number of elements in the tree
            public int max_length;                  // max bit length for the codes

            public static_tree_desc(ct_data[] static_tree, int[] extra_bits, int extra_base, int elems, int max_length)
            {
                this.static_tree = static_tree;
                this.extra_bits = extra_bits;
                this.extra_base = extra_base;
                this.elems = elems;
                this.max_length = max_length;
            }
        }

        private static readonly static_tree_desc static_l_desc = new static_tree_desc(static_ltree, extra_lbits, LITERALS + 1, L_CODES, MAX_BITS);
        private static readonly static_tree_desc static_d_desc = new static_tree_desc(static_dtree, extra_dbits, 0, D_CODES, MAX_BITS);
        private static readonly static_tree_desc static_bl_desc = new static_tree_desc(null, extra_blbits, 0, BL_CODES, MAX_BL_BITS);

        // ===========================================================================
        // Local (static) routines in this file.
        //

        // Send a code of the given tree. c and tree must not have side effects
        //#define send_code(s, c, tree) send_bits(s, tree[c].Code, tree[c].Len)
        static void send_code(deflate_state s, int c, ct_data[] tree)
        {
            ushort value = tree[c].Code;
            ushort len = tree[c].Len;
            if (s.bi_valid > (int)Buf_size - len)
            {
                int val = value;
                s.bi_buf |= (ushort)(val << s.bi_valid);
                //was put_short(s, s.bi_buf);
                s.pending_buf[s.pending++] = (byte)(s.bi_buf & 0xff);
                s.pending_buf[s.pending++] = (byte)((ushort)s.bi_buf >> 8);
                s.bi_buf = (ushort)(val >> (Buf_size - s.bi_valid));
                s.bi_valid += len - Buf_size;
            }
            else
            {
                s.bi_buf |= (ushort)(value << s.bi_valid);
                s.bi_valid += len;
            }
        }

        // ===========================================================================
        // Output a short LSB first on the stream.
        // IN assertion: there is enough room in pendingBuf.
        //#define put_short(s, w) { \
        //		put_byte(s, (unsigned char)((w) & 0xff)); \
        //		put_byte(s, (unsigned char)((unsigned short)(w) >> 8)); \
        //}

        // ===========================================================================
        // Send a value on a given number of bits.
        // IN assertion: length <= 16 and value fits in length bits.
        //#define send_bits(s, value, length) { \
        //		int len = length; \
        //		if(s.bi_valid > (int)Buf_size - len) { \
        //			int val = value; \
        //			s.bi_buf |= (val << s.bi_valid); \
        //		//	put_short(s, s.bi_buf); \
        //			s.pending_buf[s.pending++] = (unsigned char)(s.bi_buf & 0xff);\
        //			s.pending_buf[s.pending++] = (unsigned char)((unsigned short)s.bi_buf >> 8);\
        //			s.bi_buf = (unsigned short)val >> (Buf_size - s.bi_valid); \
        //			s.bi_valid += len - Buf_size; \
        //		} else { \
        //			s.bi_buf |= (value) << s.bi_valid; \
        //			s.bi_valid += len; \
        //		} \
        //	}

        static void send_bits(deflate_state s, int value, int length)
        {
            int len = length;
            if (s.bi_valid > (int)Buf_size - len)
            {
                int val = value;
                s.bi_buf |= (ushort)(val << s.bi_valid);
                //was put_short(s, s.bi_buf);
                s.pending_buf[s.pending++] = (byte)(s.bi_buf & 0xff);
                s.pending_buf[s.pending++] = (byte)((ushort)s.bi_buf >> 8);
                s.bi_buf = (ushort)(val >> (Buf_size - s.bi_valid));
                s.bi_valid += len - Buf_size;
            }
            else
            {
                s.bi_buf |= (ushort)(value << s.bi_valid);
                s.bi_valid += len;
            }
        }

        // the arguments must not have side effects

        // ===========================================================================
        // Initialize the tree data structures for a new zlib stream.
        static void _tr_init(deflate_state s)
        {
            s.l_desc.dyn_tree = s.dyn_ltree;
            s.l_desc.stat_desc = static_l_desc;

            s.d_desc.dyn_tree = s.dyn_dtree;
            s.d_desc.stat_desc = static_d_desc;

            s.bl_desc.dyn_tree = s.bl_tree;
            s.bl_desc.stat_desc = static_bl_desc;

            s.bi_buf = 0;
            s.bi_valid = 0;
            s.last_eob_len = 8; // enough lookahead for inflate

            // Initialize the first block of the first file:
            init_block(s);
        }

        // ===========================================================================
        // Initialize a new block.
        static void init_block(deflate_state s)
        {
            // Initialize the trees.
            for (int n = 0; n < L_CODES; n++) s.dyn_ltree[n].Freq = 0;
            for (int n = 0; n < D_CODES; n++) s.dyn_dtree[n].Freq = 0;
            for (int n = 0; n < BL_CODES; n++) s.bl_tree[n].Freq = 0;

            s.dyn_ltree[END_BLOCK].Freq = 1;
            s.opt_len = s.static_len = 0;
            s.last_lit = s.matches = 0;
        }

        // Index within the heap array of least frequent node in the Huffman tree
        private const int SMALLEST = 1;

        // ===========================================================================
        // Remove the smallest element from the heap and recreate the heap with
        // one less element. Updates heap and heap_len.
        //#define pqremove(s, tree, top) \
        //		top = s.heap[SMALLEST]; \
        //		s.heap[SMALLEST] = s.heap[s.heap_len--]; \
        //		pqdownheap(s, tree, SMALLEST);

        // ===========================================================================
        // Compares to subtrees, 
        // the subtrees have equal frequency. This minimizes the worst case length.
        //#define smaller(tree, n, m, depth) \
        //		(tree[n].Freq < tree[m].Freq || \
        //		(tree[n].Freq == tree[m].Freq && depth[n] <= depth[m]))

        // ===========================================================================
        // Restore the heap property by moving down the tree starting at node k,
        // exchanging a node with the smallest of its two sons if necessary, stopping
        // when the heap property is re-established (each father smaller than its
        // two sons).

        // tree:	the tree to restore
        // k:		node to move down
        static void pqdownheap(deflate_state s, ct_data[] tree, int k)
        {
            int v = s.heap[k];
            int j = k << 1;  // left son of k
            while (j <= s.heap_len)
            {
                // Set j to the smallest of the two sons:
                //was if (j < s.heap_len && smaller(tree, s.heap[j+1], s.heap[j], s.depth)) 
                if (j < s.heap_len && (tree[s.heap[j + 1]].Freq < tree[s.heap[j]].Freq ||
                    (tree[s.heap[j + 1]].Freq == tree[s.heap[j]].Freq && s.depth[s.heap[j + 1]] <= s.depth[s.heap[j]]))) j++;

                // Exit if v is smaller than both sons
                //was if (smaller(tree, v, s.heap[j], s.depth)) break;
                if (tree[v].Freq < tree[s.heap[j]].Freq ||
                    (tree[v].Freq == tree[s.heap[j]].Freq && s.depth[v] <= s.depth[s.heap[j]])) break;

                // Exchange v with the smallest son
                s.heap[k] = s.heap[j]; k = j;

                // And continue down the tree, setting j to the left son of k
                j <<= 1;
            }
            s.heap[k] = v;
        }

        // ===========================================================================
        // Compute the optimal bit lengths for a tree and update the total bit length
        // for the current block.
        // IN assertion: the fields freq and dad are set, heap[heap_max] and
        //    above are the tree nodes sorted by increasing frequency.
        // OUT assertions: the field len is set to the optimal bit length, the
        //     array bl_count contains the frequencies for each bit length.
        //     The length opt_len is updated; static_len is also updated if stree is
        //     not null.

        // desc:	the tree descriptor
        static void gen_bitlen(deflate_state s, ref tree_desc desc)
        {
            ct_data[] tree = desc.dyn_tree;
            int max_code = desc.max_code;
            ct_data[] stree = desc.stat_desc.static_tree;
            int[] extra = desc.stat_desc.extra_bits;
            int @base = desc.stat_desc.extra_base;
            int max_length = desc.stat_desc.max_length;
            int h;          // heap index
            int n, m;       // iterate over the tree elements
            int bits;       // bit length
            int xbits;      // extra bits
            ushort f;       // frequency
            int overflow = 0;   // number of elements with bit length too large

            for (bits = 0; bits <= MAX_BITS; bits++) s.bl_count[bits] = 0;

            // In a first pass, compute the optimal bit lengths (which may
            // overflow in the case of the bit length tree).
            tree[s.heap[s.heap_max]].Len = 0; // root of the heap

            for (h = s.heap_max + 1; h < HEAP_SIZE; h++)
            {
                n = s.heap[h];
                bits = tree[tree[n].Dad].Len + 1;
                if (bits > max_length) { bits = max_length; overflow++; }
                tree[n].Len = (ushort)bits;
                // We overwrite tree[n].Dad which is no longer needed

                if (n > max_code) continue; // not a leaf node

                s.bl_count[bits]++;
                xbits = 0;
                if (n >= @base) xbits = extra[n - @base];
                f = tree[n].Freq;
                s.opt_len += (uint)(f * (bits + xbits));
                if (stree != null) s.static_len += (uint)(f * (stree[n].Len + xbits));
            }
            if (overflow == 0) return;

            //Trace((stderr,"\nbit length overflow\n"));
            // This happens for example on obj2 and pic of the Calgary corpus

            // Find the first bit length which could increase:
            do
            {
                bits = max_length - 1;
                while (s.bl_count[bits] == 0) bits--;
                s.bl_count[bits]--;     // move one leaf down the tree
                s.bl_count[bits + 1] += 2;  // move one overflow item as its brother
                s.bl_count[max_length]--;
                // The brother of the overflow item also moves one step up,
                // but this does not affect bl_count[max_length]
                overflow -= 2;
            } while (overflow > 0);

            // Now recompute all bit lengths, scanning in increasing frequency.
            // h is still equal to HEAP_SIZE. (It is simpler to reconstruct all
            // lengths instead of fixing only the wrong ones. This idea is taken
            // from 'ar' written by Haruhiko Okumura.)
            for (bits = max_length; bits != 0; bits--)
            {
                n = s.bl_count[bits];
                while (n != 0)
                {
                    m = s.heap[--h];
                    if (m > max_code) continue;
                    if ((uint)tree[m].Len != (uint)bits)
                    {
                        //Trace((stderr,"code %d bits %d->%d\n", m, tree[m].Len, bits));
                        s.opt_len += ((uint)bits - tree[m].Len) * tree[m].Freq;
                        tree[m].Len = (ushort)bits;
                    }
                    n--;
                }
            }
        }

        // ===========================================================================
        // Generate the codes for a given tree and bit counts (which need not be
        // optimal).
        // IN assertion: the array bl_count contains the bit length statistics for
        // the given tree and the field len is set for all tree elements.
        // OUT assertion: the field code is set for all tree elements of non
        //     zero code length.

        // tree:		the tree to decorate
        // max_code:	largest code with non zero frequency
        // bl_count:	number of codes at each bit length
        static void gen_codes(ct_data[] tree, int max_code, ushort[] bl_count)
        {
            ushort[] next_code = new ushort[MAX_BITS + 1];  // next code value for each bit length
            ushort code = 0;    // running code value
            int bits;       // bit index
            int n;          // code index

            // The distribution counts are first used to generate the code values
            // without bit reversal.
            for (bits = 1; bits <= MAX_BITS; bits++) next_code[bits] = code = (ushort)((code + bl_count[bits - 1]) << 1);

            // Check that the bit counts in bl_count are consistent. The last code
            // must be all ones.
            //Assert (code + bl_count[MAX_BITS]-1 == (1<<MAX_BITS)-1, "inconsistent bit counts");
            //Tracev((stderr, "\ngen_codes: max_code %d ", max_code));

            for (n = 0; n <= max_code; n++)
            {
                int len = tree[n].Len;
                if (len == 0) continue;
                // Now reverse the bits
                tree[n].Code = bi_reverse(next_code[len]++, len);

                //Tracecv(tree != static_ltree, (stderr,"\nn %3d %c l %2d c %4x (%x) ", n, (isgraph(n) ? n : ' '), len, tree[n].Code, next_code[len]-1));
            }
        }

        // ===========================================================================
        // Construct one Huffman tree and assigns the code bit strings and lengths.
        // Update the total bit length for the current block.
        // IN assertion: the field freq is set for all tree elements.
        // OUT assertions: the fields len and code are set to the optimal bit length
        //     and corresponding code. The length opt_len is updated; static_len is
        //     also updated if stree is not null. The field max_code is set.

        // desc:	the tree descriptor
        static void build_tree(deflate_state s, ref tree_desc desc)
        {
            ct_data[] tree = desc.dyn_tree;
            ct_data[] stree = desc.stat_desc.static_tree;
            int elems = desc.stat_desc.elems;
            int n, m;           // iterate over heap elements
            int max_code = -1;  // largest code with non zero frequency
            int node;           // new node being created

            // Construct the initial heap, with least frequent element in
            // heap[SMALLEST]. The sons of heap[n] are heap[2*n] and heap[2*n+1].
            // heap[0] is not used.
            s.heap_len = 0;
            s.heap_max = HEAP_SIZE;

            for (n = 0; n < elems; n++)
            {
                if (tree[n].Freq != 0)
                {
                    s.heap[++(s.heap_len)] = max_code = n;
                    s.depth[n] = 0;
                }
                else tree[n].Len = 0;
            }

            // The pkzip format requires that at least one distance code exists,
            // and that at least one bit should be sent even if there is only one
            // possible code. So to avoid special checks later on we force at least
            // two codes of non zero frequency.
            while (s.heap_len < 2)
            {
                node = s.heap[++(s.heap_len)] = (max_code < 2 ? ++max_code : 0);
                tree[node].Freq = 1;
                s.depth[node] = 0;
                s.opt_len--; if (stree != null) s.static_len -= stree[node].Len;
                // node is 0 or 1 so it does not have extra bits
            }
            desc.max_code = max_code;

            // The elements heap[heap_len/2+1 .. heap_len] are leaves of the tree,
            // establish sub-heaps of increasing lengths:
            for (n = s.heap_len / 2; n >= 1; n--) pqdownheap(s, tree, n);

            // Construct the Huffman tree by repeatedly combining the least two
            // frequent nodes.
            node = elems;               // next internal node of the tree
            do
            {
                //was pqremove(s, tree, n);  // n = node of least frequency
                n = s.heap[SMALLEST];
                s.heap[SMALLEST] = s.heap[s.heap_len--];
                pqdownheap(s, tree, SMALLEST);

                m = s.heap[SMALLEST]; // m = node of next least frequency

                s.heap[--(s.heap_max)] = n; // keep the nodes sorted by frequency
                s.heap[--(s.heap_max)] = m;

                // Create a new node father of n and m
                tree[node].Freq = (ushort)(tree[n].Freq + tree[m].Freq);
                s.depth[node] = (byte)((s.depth[n] >= s.depth[m] ? s.depth[n] : s.depth[m]) + 1);
                tree[n].Dad = tree[m].Dad = (ushort)node;

                // and insert the new node in the heap
                s.heap[SMALLEST] = node++;
                pqdownheap(s, tree, SMALLEST);

            } while (s.heap_len >= 2);

            s.heap[--(s.heap_max)] = s.heap[SMALLEST];

            // At this point, the fields freq and dad are set. We can now
            // generate the bit lengths.
            gen_bitlen(s, ref desc);

            // The field len is now set, we can generate the bit codes
            gen_codes(tree, max_code, s.bl_count);
        }

        // ===========================================================================
        // Scan a literal or distance tree to determine the frequencies of the codes
        // in the bit length tree.

        // tree:		the tree to be scanned
        // max_code:	and its largest code of non zero frequency
        static void scan_tree(deflate_state s, ct_data[] tree, int max_code)
        {
            int n;                      // iterates over all tree elements
            int prevlen = -1;               // last emitted length
            int curlen;                 // length of current code
            int nextlen = tree[0].Len;  // length of next code
            int count = 0;              // repeat count of the current code
            int max_count = 7;          // max repeat count
            int min_count = 4;          // min repeat count

            if (nextlen == 0) { max_count = 138; min_count = 3; }
            tree[max_code + 1].Len = (ushort)0xffff; // guard

            for (n = 0; n <= max_code; n++)
            {
                curlen = nextlen; nextlen = tree[n + 1].Len;
                if (++count < max_count && curlen == nextlen) continue;

                if (count < min_count) s.bl_tree[curlen].Freq += (ushort)count;
                else if (curlen != 0)
                {
                    if (curlen != prevlen) s.bl_tree[curlen].Freq++;
                    s.bl_tree[REP_3_6].Freq++;
                }
                else if (count <= 10) s.bl_tree[REPZ_3_10].Freq++;
                else s.bl_tree[REPZ_11_138].Freq++;

                count = 0; prevlen = curlen;
                if (nextlen == 0) { max_count = 138; min_count = 3; }
                else if (curlen == nextlen) { max_count = 6; min_count = 3; }
                else { max_count = 7; min_count = 4; }
            }
        }

       

        // tree:		the tree to be scanned
        // max_code:	and its largest code of non zero frequency
        static void send_tree(deflate_state s, ct_data[] tree, int max_code)
        {
            int n;                      // iterates over all tree elements
            int prevlen = -1;               // last emitted length
            int curlen;                 // length of current code
            int nextlen = tree[0].Len;  // length of next code
            int count = 0;              // repeat count of the current code
            int max_count = 7;          // max repeat count
            int min_count = 4;          // min repeat count

            // tree[max_code+1].Len = -1;
            // guard already set
            if (nextlen == 0) { max_count = 138; min_count = 3; }

            for (n = 0; n <= max_code; n++)
            {
                curlen = nextlen; nextlen = tree[n + 1].Len;
                if (++count < max_count && curlen == nextlen) continue;

                if (count < min_count)
                {
                    do { send_code(s, curlen, s.bl_tree); } while (--count != 0);
                }
                else if (curlen != 0)
                {
                    if (curlen != prevlen) { send_code(s, curlen, s.bl_tree); count--; }

                    //Assert(count>=3&&count<=6, " 3_6?");
                    send_code(s, REP_3_6, s.bl_tree); send_bits(s, count - 3, 2);
                }
                else if (count <= 10) { send_code(s, REPZ_3_10, s.bl_tree); send_bits(s, count - 3, 3); }
                else { send_code(s, REPZ_11_138, s.bl_tree); send_bits(s, count - 11, 7); }

                count = 0; prevlen = curlen;
                if (nextlen == 0) { max_count = 138; min_count = 3; }
                else if (curlen == nextlen) { max_count = 6; min_count = 3; }
                else { max_count = 7; min_count = 4; }
            }
        }

        // ===========================================================================
        // Construct the Huffman tree for the bit lengths and return the index in
        // bl_order of the last bit length code to send.
        static int build_bl_tree(deflate_state s)
        {
            int max_blindex;  // index of last bit length code of non zero freq

            // Determine the bit length frequencies for literal and distance trees
            scan_tree(s, s.dyn_ltree, s.l_desc.max_code);
            scan_tree(s, s.dyn_dtree, s.d_desc.max_code);

            // Build the bit length tree:
            build_tree(s, ref s.bl_desc);
            // opt_len now includes the length of the tree representations, except
            // the lengths of the bit lengths codes and the 5+5+4 bits for the counts.

            // Determine the number of bit length codes to send. The pkzip format
            // requires that at least 4 bit length codes be sent. (appnote.txt says
            // 3 but the actual value used is 4.)
            for (max_blindex = BL_CODES - 1; max_blindex >= 3; max_blindex--)
            {
                if (s.bl_tree[bl_order[max_blindex]].Len != 0) break;
            }
            // Update opt_len to include the bit length tree and counts
            s.opt_len += (uint)(3 * (max_blindex + 1) + 5 + 5 + 4);
            //Tracev((stderr, "\ndyn trees: dyn %ld, stat %ld", s.opt_len, s.static_len));

            return max_blindex;
        }

        
        // lengths of the bit length codes, the literal tree and the distance tree.
        // IN assertion: lcodes >= 257, dcodes >= 1, blcodes >= 4.

        // lcodes, dcodes, blcodes: number of codes for each tree
        static void send_all_trees(deflate_state s, int lcodes, int dcodes, int blcodes)
        {
            int rank;                       // index in bl_order

            //Assert (lcodes >= 257 && dcodes >= 1 && blcodes >= 4, "not enough codes");
            //Assert (lcodes <= L_CODES && dcodes <= D_CODES && blcodes <= BL_CODES, "too many codes");
            //Tracev((stderr, "\nbl counts: "));
            send_bits(s, lcodes - 257, 5);  // not +255 as stated in appnote.txt
            send_bits(s, dcodes - 1, 5);
            send_bits(s, blcodes - 4, 4);       // not -3 as stated in appnote.txt
            for (rank = 0; rank < blcodes; rank++)
            {
                //Tracev((stderr, "\nbl code %2d ", bl_order[rank]));
                send_bits(s, s.bl_tree[bl_order[rank]].Len, 3);
            }
            //Tracev((stderr, "\nbl tree: sent %ld", s.bits_sent));

            send_tree(s, s.dyn_ltree, lcodes - 1); // literal tree
                                                   //Tracev((stderr, "\nlit tree: sent %ld", s.bits_sent));

            send_tree(s, s.dyn_dtree, dcodes - 1); // distance tree
                                                   //Tracev((stderr, "\ndist tree: sent %ld", s.bits_sent));
        }

        // ===========================================================================
        // Send a stored block

        // buf:			input block
        // stored_len:	length of input block
        // last:		one if this is the last block for a file
        static void _tr_stored_block(deflate_state s, byte[] buf, uint stored_len, int last)
        {
            send_bits(s, (STORED_BLOCK << 1) + last, 3);    // send block type
            copy_block(s, buf, 0, stored_len, 1);       // with header
        }

        static void _tr_stored_block(deflate_state s, byte[] buf, int buf_ind, uint stored_len, int last)
        {
            send_bits(s, (STORED_BLOCK << 1) + last, 3);    // send block type
            copy_block(s, buf, buf_ind, stored_len, 1); // with header
        }

        // ===========================================================================
        // Send one empty static block to give enough lookahead for inflate.
        // This takes 10 bits, of which 7 may remain in the bit buffer.
        // The current inflate code requires 9 bits of lookahead. If the
        // last two codes for the previous block (real code plus EOB) were coded
        // on 5 bits or less, inflate may have only 5+3 bits of lookahead to decode
        // the last real code. In this case we send two empty static blocks instead
        // of one. (There are no problems if the previous block is stored or fixed.)
        // To simplify the code, we assume the worst case of last real code encoded
        // on one bit only.
        static void _tr_align(deflate_state s)
        {
            send_bits(s, STATIC_TREES << 1, 3);
            send_code(s, END_BLOCK, static_ltree);

            bi_flush(s);
            // Of the 10 bits for the empty block, we have already sent
            // (10 - bi_valid) bits. The lookahead for the last real code (before
            // the EOB of the previous block) was thus at least one plus the length
            // of the EOB plus what we have just sent of the empty static block.
            if (1 + s.last_eob_len + 10 - s.bi_valid < 9)
            {
                send_bits(s, STATIC_TREES << 1, 3);
                send_code(s, END_BLOCK, static_ltree);

                bi_flush(s);
            }
            s.last_eob_len = 7;
        }

        // ===========================================================================
        // Determine the best encoding for the current block: dynamic trees, static
        // trees or store, and output the encoded block to the zip file.

        // buf:			input block, or NULL if too old
        // stored_len:	length of input block
        // last:		one if this is the last block for a file
        static void _tr_flush_block(deflate_state s, byte[] buf, int buf_ind, uint stored_len, int last)
        {
            uint opt_lenb, static_lenb; // opt_len and static_len in bytes
            int max_blindex = 0;            // index of last bit length code of non zero freq

            // Build the Huffman trees unless a stored block is forced
            if (s.level > 0)
            {
                // Construct the literal and distance trees
                build_tree(s, ref s.l_desc);
                //Tracev((stderr, "\nlit data: dyn %ld, stat %ld", s.opt_len, s.static_len));

                build_tree(s, ref s.d_desc);
                //Tracev((stderr, "\ndist data: dyn %ld, stat %ld", s.opt_len, s.static_len));
                // At this point, opt_len and static_len are the total bit lengths of
                // the compressed block data, excluding the tree representations.

                // Build the bit length tree for the above two trees, and get the index
                // in bl_order of the last bit length code to send.
                max_blindex = build_bl_tree(s);

                // Determine the best encoding. Compute the block lengths in bytes.
                opt_lenb = (s.opt_len + 3 + 7) >> 3;
                static_lenb = (s.static_len + 3 + 7) >> 3;

                //Tracev((stderr, "\nopt %lu(%lu) stat %lu(%lu) stored %lu lit %u ", opt_lenb, s.opt_len, static_lenb, s.static_len, stored_len, s.last_lit));

                if (static_lenb <= opt_lenb) opt_lenb = static_lenb;
            }
            else
            {
                //Assert(buf!=(char*)0, "lost buf");
                opt_lenb = static_lenb = stored_len + 5; // force a stored block
            }

            if (stored_len + 4 <= opt_lenb && buf != null)
            {
                // 4: two words for the lengths
                // The test buf != NULL is only necessary if LIT_BUFSIZE > WSIZE.
                // Otherwise we can't have processed more than WSIZE input bytes since
                // the last block flush, because compression would have been
                // successful. If LIT_BUFSIZE <= WSIZE, it is never too late to
                // transform a block into a stored block.
                _tr_stored_block(s, buf, buf_ind, stored_len, last);
            }
            else if (s.strategy == Z_FIXED || static_lenb == opt_lenb)
            {
                send_bits(s, (STATIC_TREES << 1) + last, 3);
                compress_block(s, static_ltree, static_dtree);
            }
            else
            {
                send_bits(s, (DYN_TREES << 1) + last, 3);
                send_all_trees(s, s.l_desc.max_code + 1, s.d_desc.max_code + 1, max_blindex + 1);
                compress_block(s, s.dyn_ltree, s.dyn_dtree);
            }
            //Assert (s.compressed_len == s.bits_sent, "bad compressed size");
            // The above check is made mod 2^32, for files larger than 512 MB
            // and unsigned int implemented on 32 bits.
            init_block(s);

            if (last != 0) bi_windup(s);
            //Tracev((stderr,"\ncomprlen %lu(%lu) ", s.compressed_len>>3, s.compressed_len-7*eof));
        }

        // ===========================================================================
        // Save the match info and tally the frequency counts. Return true if
        // the current block must be flushed.

        // dist:	distance of matched string
        // lc:		match length-MIN_MATCH or unmatched char (if dist==0)
        static bool _tr_tally(deflate_state s, uint dist, uint lc)
        {
            s.d_buf[s.last_lit] = (ushort)dist;
            s.l_buf[s.last_lit++] = (byte)lc;
            if (dist == 0)
            {
                // lc is the unmatched char
                s.dyn_ltree[lc].Freq++;
            }
            else
            {
                s.matches++;
                // Here, lc is the match length - MIN_MATCH
                dist--;             // dist = match distance - 1
                                    //Assert((ushort)dist < (ushort)MAX_DIST(s) &&
                                    //		(ushort)lc <= (ushort)(MAX_MATCH-MIN_MATCH) &&
                                    //		(ushort)(dist < 256 ? _dist_code[dist] : _dist_code[256+(dist>>7)]) < (ushort)D_CODES,
                                    //		"_tr_tally: bad match");

                s.dyn_ltree[_length_code[lc] + LITERALS + 1].Freq++;
                s.dyn_dtree[(dist < 256 ? _dist_code[dist] : _dist_code[256 + (dist >> 7)])].Freq++;
            }

            return (s.last_lit == s.lit_bufsize - 1);
            // We avoid equality with lit_bufsize because of wraparound at 64K
            // on 16 bit machines and because stored blocks are restricted to
            // 64K-1 bytes.
        }

        
        static void compress_block(deflate_state s, ct_data[] ltree, ct_data[] dtree)
        {
            uint dist;  // distance of matched string
            int lc;     // match length or unmatched char (if dist == 0)
            uint lx = 0;    // running index in l_buf
            uint code;  // the code to send
            int extra;  // number of extra bits to send

            if (s.last_lit != 0)
            {
                do
                {
                    dist = s.d_buf[lx];
                    lc = s.l_buf[lx++];
                    if (dist == 0)
                    {
                        send_code(s, lc, ltree); // send a literal byte
                                                 //Tracecv(isgraph(lc), (stderr," '%c' ", lc));
                    }
                    else
                    {
                        // Here, lc is the match length - MIN_MATCH
                        code = _length_code[lc];
                        send_code(s, (int)(code + LITERALS + 1), ltree); // send the length code
                        extra = extra_lbits[code];
                        if (extra != 0)
                        {
                            lc -= base_length[code];
                            send_bits(s, lc, extra);        // send the extra length bits
                        }
                        dist--; // dist is now the match distance - 1
                        code = (dist < 256 ? _dist_code[dist] : _dist_code[256 + (dist >> 7)]);
                        //Assert (code < D_CODES, "bad d_code");

                        send_code(s, (int)code, dtree);     // send the distance code
                        extra = extra_dbits[code];
                        if (extra != 0)
                        {
                            dist -= (uint)base_dist[code];
                            send_bits(s, (int)dist, extra); // send the extra distance bits
                        }
                    } // literal or match pair ?
                } while (lx < s.last_lit);
            }
            send_code(s, END_BLOCK, ltree);
            s.last_eob_len = ltree[END_BLOCK].Len;
        }

        
        static ushort bi_reverse(ushort code, int len)
        {
            ushort res = 0;
            do
            {
                res |= (ushort)(code & 1);
                code >>= 1;
                res <<= 1;
            } while (--len > 0);
            return (ushort)(res >> 1);
        }

        // ===========================================================================
        // Flush the bit buffer, keeping at most 7 bits in it.
        static void bi_flush(deflate_state s)
        {
            if (s.bi_valid == 16)
            {
                //was put_short(s, s.bi_buf);
                s.pending_buf[s.pending++] = (byte)(s.bi_buf & 0xff);
                s.pending_buf[s.pending++] = (byte)((ushort)s.bi_buf >> 8);

                s.bi_buf = 0;
                s.bi_valid = 0;
            }
            else if (s.bi_valid >= 8)
            {
                //was put_byte(s, (unsigned char)s.bi_buf);
                s.pending_buf[s.pending++] = (byte)s.bi_buf;

                s.bi_buf >>= 8;
                s.bi_valid -= 8;
            }
        }

        // ===========================================================================
        // Flush the bit buffer and align the output on a byte boundary
        static void bi_windup(deflate_state s)
        {
            if (s.bi_valid > 8)
            {
                //was put_short(s, s.bi_buf);
                s.pending_buf[s.pending++] = (byte)(s.bi_buf & 0xff);
                s.pending_buf[s.pending++] = (byte)((ushort)s.bi_buf >> 8);
            }
            else if (s.bi_valid > 0)
            {
                //was put_byte(s, (unsigned char)s.bi_buf);
                s.pending_buf[s.pending++] = (byte)s.bi_buf;
            }
            s.bi_buf = 0;
            s.bi_valid = 0;
        }

        // ===========================================================================
        // Copy a stored block, storing first the length and its
        // one's complement if requested.

        // buf:		the input data
        // len:		its length
        // header:	true if block header must be written
        static void copy_block(deflate_state s, byte[] buf, int buf_ind, uint len, int header)
        {
            bi_windup(s);       // align on byte boundary
            s.last_eob_len = 8; // enough lookahead for inflate

            if (header != 0)
            {
                //was put_short(s, (unsigned short)len);
                s.pending_buf[s.pending++] = (byte)(((ushort)len) & 0xff);
                s.pending_buf[s.pending++] = (byte)(((ushort)len) >> 8);

                //was put_short(s, (unsigned short)~len);
                s.pending_buf[s.pending++] = (byte)(((ushort)~len) & 0xff);
                s.pending_buf[s.pending++] = (byte)(((ushort)~len) >> 8);
            }

            while (len-- != 0)
            {
                //was put_byte(s, *buf++);
                s.pending_buf[s.pending++] = buf[buf_ind++];
            }
        }
    }
}
