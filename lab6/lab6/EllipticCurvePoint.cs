using System.Collections;
using System.Numerics;

namespace lab6;

public class EllipticCurvePoint
{
    public BigInteger x { get; set; }
    public BigInteger y { get; set; }
    public BigInteger a { get; set; }
    public BigInteger b { get; set; }
    public BigInteger p { get; set; }

    public EllipticCurvePoint()
    {
        x = BigInteger.Zero;
        y = BigInteger.Zero;
        a = BigInteger.Zero;
        b = BigInteger.Zero;
        p = BigInteger.Zero;
    }

    public EllipticCurvePoint(BigInteger x, BigInteger y, BigInteger a, BigInteger b, BigInteger p)
    {
        this.x = x;
        this.y = y;
        this.a = a;
        this.b = b;
        this.p = p;
    }

    public static EllipticCurvePoint Add(EllipticCurvePoint P, EllipticCurvePoint Q)
    {
        var m = P.Equals(Q) ? 
            (3 * BigInteger.Pow(P.x, 2) + P.a) / (2 * P.y) :
            (P.y - Q.y) / (P.x - Q.x);
        
        var newX = BigInteger.Pow(m, 2) - P.x - Q.x;
        var newY = P.y + m * (newX - P.x);
        return new EllipticCurvePoint(newX, newY, P.a, P.b, P.p);
    }

    public static EllipticCurvePoint Multiply(EllipticCurvePoint P, BigInteger k)
    {
        var result = new EllipticCurvePoint();
        var addend = P;
        var kBits = new BitArray(k.ToByteArray());

        for (var i = 0; i < kBits.Length; i++) {
            if (kBits[i]) {
                result = Add(result, addend);
            }
            addend = Add(addend, addend);
        }

        return result;
    }

    private bool Equals(EllipticCurvePoint obj)
    {
        return x == obj.x && y == obj.y && a == obj.a;
    }
}