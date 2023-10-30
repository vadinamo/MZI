namespace lab5;

public class MD5
{
    private static readonly uint[] T =
    {
        0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
        0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
        0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
        0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
        0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
        0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
        0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
        0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
        0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
        0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
        0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
        0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
        0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
        0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
        0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
        0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
    };

    private static readonly int[] s =
    {
        7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
        5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20,
        4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
        6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21
    };

    private uint FunF(uint x, uint y, uint z)
    {
        return (x & y) | (~x & z);
    }

    private uint FunG(uint x, uint y, uint z)
    {
        return (x & z) | (~z & y);
    }

    private uint FunH(uint x, uint y, uint z)
    {
        return x ^ y ^ z;
    }

    private uint FunI(uint x, uint y, uint z)
    {
        return y ^ (~z | x);
    }

    private static uint RotateLeft(uint x, int c)
    {
        return (x << c) | (x >> (32 - c));
    }

    public byte[] Hash(byte[] messageBytes)
    {
        var bytes = messageBytes.ToList();
        bytes.Add(0x80);
        if (bytes.Count % 64 != 56)
        {
            var blockCount = bytes.Count % 64;
            bytes = bytes.Concat(new byte[56 - blockCount]).ToList();
        }

        var dataLengthBytes = BitConverter.GetBytes((long)messageBytes.Length * 8);
        bytes = bytes.Concat(dataLengthBytes).ToList();

        var a0 = 0x67452301u;
        var b0 = 0xefcdab89u;
        var c0 = 0x98badcfeu;
        var d0 = 0x10325476u;

        for (var i = 0; i < bytes.Count; i += 64)
        {
            var block = Enumerable.Range(0, 16)
                .Select(j =>
                    BitConverter.ToUInt32(bytes.ToArray(), i + j * 4)
                )
                .ToArray();

            var A = a0;
            var B = b0;
            var C = c0;
            var D = d0;
            var F = 0u;
            var g = 0u;

            for (var j = 0u; j < 64; j++)
            {
                if (j < 16)
                {
                    F = FunF(B, C, D);
                    g = j;
                }
                else if (j is >= 16 and < 32)
                {
                    F = FunG(B, C, D);
                    g = (5 * j + 1) % 16;
                }
                else if (j is >= 32 and < 48)
                {
                    F = FunH(B, C, D);
                    g = (3 * j + 5) % 16;
                }
                else
                {
                    F = FunI(B, C, D);
                    g = 7 * j % 16;
                }

                var buffer = D;
                D = C;
                C = B;
                B += RotateLeft(A + F + T[j] + block[g], s[j]);
                A = buffer;
            }

            a0 += A;
            b0 += B;
            c0 += C;
            d0 += D;
        }

        return BitConverter.GetBytes(a0).Concat(
                BitConverter.GetBytes(b0)).Concat(
                BitConverter.GetBytes(c0)).Concat(
                BitConverter.GetBytes(d0))
            .ToArray();
    }
}