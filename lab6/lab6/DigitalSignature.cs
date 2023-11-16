using System.Numerics;
using System.Text;

namespace lab6;

public static class DigitalSignature
{
    private static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max)
    {
        var random = new Random();
        var data = new byte[max.ToByteArray().Length];
        random.NextBytes(data);

        return new BigInteger(data) % (max - min) + min;
    }

    private static byte[] BinaryVectors(BigInteger q, BigInteger r, BigInteger s)
    {
        var r1 = r.ToByteArray();
        while (r1.Length != q.GetBitLength() / 8)
            r1 = r1.Reverse().Concat(new byte[] { 0 }).Reverse().ToArray();
        var s1 = s.ToByteArray();
        while (s1.Length != q.GetBitLength() / 8)
            s1 = s1.Reverse().Concat(new byte[] { 0 }).Reverse().ToArray();
        return r1.Concat(s1).ToArray();
    }

    public static byte[] GetSignature(string message, BigInteger m, BigInteger q, EllipticCurvePoint P,
        BigInteger d, EllipticCurvePoint Q)
    {
        var hash = GOST3411.Hash(Encoding.UTF8.GetBytes(message)); // step 1
        var alpha = new BigInteger(hash); // step 2

        var e = alpha % q;
        if (e.IsZero)
        {
            e = 1;
        }

        do
        {
            var k = GenerateRandomBigInteger(0, q);

            var C = EllipticCurvePoint.Multiply(P, k);
            var r = C.x % q;
            if (r.IsZero)
            {
                continue;
            }

            var s = (r * d + k * e) % q;
            if (s.IsZero)
            {
                continue;
            }

            return BinaryVectors(q, r, s);
        } while (true);
    }
}