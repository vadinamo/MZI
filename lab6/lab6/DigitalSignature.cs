using System.Numerics;

namespace lab6;

public static class DigitalSignature
{
    private static BigInteger GenerateRandomBigInteger(BigInteger N)
    {
        var random = new Random();
        var bytes = N.ToByteArray();
        BigInteger r;

        do
        {
            random.NextBytes(bytes);
            bytes[^1] &= 0x7F;
            r = new BigInteger(bytes);
        } while (r >= N);

        return r;
    }

    private static byte[] BinaryVectors(BigInteger q, BigInteger r, BigInteger s)
    {
        var r1 = r.ToByteArray();
        while (r1.Length != q.GetBitLength() / 8)
        {
            r1 = r1.Reverse().Concat(new byte[] { 0 }).Reverse().ToArray();
        }

        var s1 = s.ToByteArray();
        while (s1.Length != q.GetBitLength() / 8)
        {
            s1 = s1.Reverse().Concat(new byte[] { 0 }).Reverse().ToArray();
        }

        return r1.Concat(s1).ToArray();
    }

    public static byte[] GetSignature(byte[] message, BigInteger q, EllipticCurvePoint P, BigInteger d)
    {
        var hash = GOST3411.Hash(message);
        var alpha = new BigInteger(hash);

        var e = alpha % q;
        if (e.IsZero)
        {
            e = 1;
        }

        do
        {
            var k = GenerateRandomBigInteger(q);

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

    public static bool CheckValidity(byte[] digitalSignature, byte[] message, BigInteger q, EllipticCurvePoint P,
        EllipticCurvePoint Q)
    {
        var r = new BigInteger(digitalSignature[..(digitalSignature.Length / 2)]);
        var s = new BigInteger(digitalSignature[(digitalSignature.Length / 2)..]);

        var hash = GOST3411.Hash(message);
        var alpha = new BigInteger(hash);

        var e = alpha % q;
        if (e.IsZero)
        {
            e = 1;
        }

        var v = BigInteger.Parse(
            new Org.BouncyCastle.Math.BigInteger(
                e.ToString()
            ).ModInverse(
                new Org.BouncyCastle.Math.BigInteger(
                    q.ToString()
                )
            ).ToString()!
        );

        var z1 = s * v % q;
        var z2 = -r * v % q + q;

        var C = EllipticCurvePoint.Multiply(P, z1) + EllipticCurvePoint.Multiply(Q, z2);
        var R = C.x % q;

        return R == r;
    }
}