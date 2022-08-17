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
        private const uint BASE = 65521;    // largest prime smaller than 65536
        private const uint NMAX = 5552; // NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1

        // =========================================================================

        // Update a running Adler-32 checksum with the bytes buf[0..len-1] and return the updated checksum.
        // If buf is NULL, this function returns the required initial value for the checksum.
        // An Adler-32 checksum is almost as reliable as a CRC32 but can be computed much faster. 
        // 
        // Usage example:
        //	uint adler=adler32(0, null, 0);
        //	while(read_buffer(buffer, length)!=EOF) 
        //	{
        //		adler=adler32(adler, buffer, length);
        //	}
        //	if(adler!=original_adler) error();

        public static uint adler32(uint adler, byte[] buf, uint len)
        {
            return adler32(adler, buf, 0, len);
        }

        public static uint adler32(uint adler, byte[] buf, uint ind, uint len)
        {
            // initial Adler-32 value (deferred check for len==1 speed)
            if (buf == null) return 1;

            // split Adler-32 into component sums
            uint sum2 = (adler >> 16) & 0xffff;
            adler &= 0xffff;

            //uint ind=0; // index in buf

            // in case user likes doing a byte at a time, keep it fast
            if (len == 1)
            {
                adler += buf[ind];
                if (adler >= BASE) adler -= BASE;
                sum2 += adler;
                if (sum2 >= BASE) sum2 -= BASE;
                return adler | (sum2 << 16);
            }

            // in case short lengths are provided, keep it somewhat fast
            if (len < 16)
            {
                while (len-- != 0)
                {
                    adler += buf[ind++];
                    sum2 += adler;
                }
                if (adler >= BASE) adler -= BASE;
                sum2 %= BASE;               // only added so many BASE's
                return adler | (sum2 << 16);
            }

            // do length NMAX blocks -- requires just one modulo operation
            while (len >= NMAX)
            {
                len -= NMAX;
                uint n = NMAX / 16;             // NMAX is divisible by 16
                do
                {
                    // 16 sums unrolled
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                } while (--n != 0);

                adler %= BASE;
                sum2 %= BASE;
            }

            // do remaining bytes (less than NMAX, still just one modulo)
            if (len != 0)
            { // avoid modulos if none remaining
                while (len >= 16)
                {
                    len -= 16;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                    adler += buf[ind++]; sum2 += adler;
                }

                while (len-- != 0)
                {
                    adler += buf[ind++];
                    sum2 += adler;
                }

                adler %= BASE;
                sum2 %= BASE;
            }

            // return recombined sums
            return adler | (sum2 << 16);
        }

        // =========================================================================

        // Combine two Adler-32 checksums into one.  For two sequences of bytes, seq1
        // and seq2 with lengths len1 and len2, Adler-32 checksums were calculated for
        // each, adler1 and adler2.  adler32_combine() returns the Adler-32 checksum of
        // seq1 and seq2 concatenated, requiring only adler1, adler2, and len2.

        public static uint adler32_combine_(uint adler1, uint adler2, uint len2)
        { // the derivation of this formula is left as an exercise for the reader
            uint rem = len2 % BASE;
            uint sum1 = adler1 & 0xffff;
            uint sum2 = (rem * sum1) % BASE;
            sum1 += (adler2 & 0xffff) + BASE - 1;
            sum2 += ((adler1 >> 16) & 0xffff) + ((adler2 >> 16) & 0xffff) + BASE - rem;
            if (sum1 >= BASE) sum1 -= BASE;
            if (sum1 >= BASE) sum1 -= BASE;
            if (sum2 >= (BASE << 1)) sum2 -= (BASE << 1);
            if (sum2 >= BASE) sum2 -= BASE;
            return sum1 | (sum2 << 16);
        }

        // =========================================================================
        public static uint adler32_combine(uint adler1, uint adler2, uint len2)
        {
            return adler32_combine_(adler1, adler2, len2);
        }

        public static uint adler32_combine64(uint adler1, uint adler2, uint len2)
        {
            return adler32_combine_(adler1, adler2, len2);
        }
    }
}
