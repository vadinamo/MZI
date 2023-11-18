using System.Numerics;
using System.Text;

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
            r1 = r1.Reverse().Concat(new byte[] { 0 }).Reverse().ToArray();
        var s1 = s.ToByteArray();
        while (s1.Length != q.GetBitLength() / 8)
            s1 = s1.Reverse().Concat(new byte[] { 0 }).Reverse().ToArray();
        return r1.Concat(s1).ToArray();
    }

    public static byte[] GetSignature(string message, BigInteger m, BigInteger q, EllipticCurvePoint P,
        BigInteger d, EllipticCurvePoint Q)
    {
        var hash = GOST3411.Hash(Encoding.UTF8.GetBytes(message));
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

    public static bool CheckValidity(byte[] digitalSignature, string message, BigInteger m, BigInteger q,
        EllipticCurvePoint P,
        BigInteger d, EllipticCurvePoint Q)
    {
        var r = new BigInteger(digitalSignature.Take(digitalSignature.Length / 2).ToArray());
        var s = new BigInteger(digitalSignature.Skip(digitalSignature.Length / 2).Take(digitalSignature.Length / 2).ToArray());
        
        var hash = GOST3411.Hash(Encoding.UTF8.GetBytes(message));
        var alpha = new BigInteger(hash);

        var e = alpha % q;
        if (e.IsZero)
        {
            e = 1;
        }

        var temp = new Org.BouncyCastle.Math.BigInteger(e.ToByteArray().Reverse().ToArray());
        var v = new BigInteger(temp.ModInverse(new Org.BouncyCastle.Math.BigInteger(q.ToByteArray().Reverse().ToArray())).ToByteArray().Reverse().ToArray());

        var z1 = s * v % q;
        var z2 = -r * v % q;

        var C = EllipticCurvePoint.Add(EllipticCurvePoint.Multiply(P, z1), EllipticCurvePoint.Multiply(Q, z2));
        var R = C.x % q;

        return R == r;
    }
}