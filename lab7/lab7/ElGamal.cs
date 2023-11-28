using System.Numerics;

namespace lab7;

public static class ElGamal
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

    private static EllipticCurvePoint GetPointFromBytes(byte[] messageBytes, EllipticCurvePoint P)
    {
        var pLength = P.p.ToByteArray().Length;
        if (messageBytes.Length >= pLength - 2)
        {
            throw new Exception($"M({messageBytes.Length}) should be less than p (Max M Length = {pLength - 2} symbols)");
        }
        
        var message = messageBytes.Concat(new byte[P.p.ToByteArray().Length - messageBytes.Length]).ToArray();
        message[messageBytes.Length] = 0xff;
        return new EllipticCurvePoint(
            new BigInteger(message),
            0,
            P.a,
            P.b,
            P.p
        );
    }

    private static byte[] GetBytesFromPoint(EllipticCurvePoint P)
    {
        var messageBytes = P.x.ToByteArray();
        return messageBytes.Take(Array.LastIndexOf(messageBytes,(byte)0xff)).ToArray();
    }

    public static EllipticCurvePoint[] Encrypt(byte[] messageBytes, EllipticCurvePoint P, EllipticCurvePoint Q)
    {
        var M = GetPointFromBytes(messageBytes, P);
        var k = GenerateRandomBigInteger(P.p);
        var C1 = EllipticCurvePoint.Multiply(P, k);
        var C2 = M + EllipticCurvePoint.Multiply(Q, k);
        return new[] { C1, C2 };
    }

    public static byte[] Decrypt(EllipticCurvePoint[] CValues, BigInteger d)
    {
        var temp = EllipticCurvePoint.Multiply(CValues[0], d);
        temp.y = -temp.y;
        var P = temp + CValues[1];
        return GetBytesFromPoint(P);
    }
}