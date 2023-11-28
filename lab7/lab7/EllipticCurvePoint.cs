using System.Numerics;

namespace lab7;

public class EllipticCurvePoint
{
    public BigInteger x { get; set; }
    public BigInteger y { get; set; }
    public BigInteger a { get; init; }
    public BigInteger b { get; init; }
    public BigInteger p { get; init; }

    public EllipticCurvePoint(BigInteger x, BigInteger y, BigInteger a, BigInteger b, BigInteger p)
    {
        this.x = x;
        this.y = y;
        this.a = a;
        this.b = b;
        this.p = p;
    }

    public static EllipticCurvePoint operator +(EllipticCurvePoint p1, EllipticCurvePoint p2)
    {
        var p3 = new EllipticCurvePoint(0, 0, p1.a, p1.b, p1.p);

        var dy = p2.y - p1.y;
        var dx = p2.x - p1.x;

        if (dx < 0)
        {
            dx += p1.p;
        }

        if (dy < 0)
        {
            dy += p1.p;
        }

        var m = dy * BigInteger.Parse(new Org.BouncyCastle.Math.BigInteger(dx.ToString())
            .ModInverse(new Org.BouncyCastle.Math.BigInteger(p1.p.ToString())).ToString()!) % p1.p;

        if (m < 0)
        {
            m += p1.p;
        }

        p3.x = (m * m - p1.x - p2.x) % p1.p;
        p3.y = (m * (p1.x - p3.x) - p1.y) % p1.p;

        if (p3.x < 0)
        {
            p3.x += p1.p;
        }

        if (p3.y < 0)
        {
            p3.y += p1.p;
        }

        return p3;
    }

    private static EllipticCurvePoint DoubleValue(EllipticCurvePoint p)
    {
        var p2 = new EllipticCurvePoint(0, 0, p.a, p.b, p.p);

        var dy = 3 * p.x * p.x + p.a;
        var dx = 2 * p.y;

        if (dx < 0)
        {
            dx += p.p;
        }

        if (dy < 0)
        {
            dy += p.p;
        }

        var m = dy * BigInteger.Parse(new Org.BouncyCastle.Math.BigInteger(dx.ToString())
            .ModInverse(new Org.BouncyCastle.Math.BigInteger(p.p.ToString())).ToString()!) % p.p;
        p2.x = (m * m - p.x - p.x) % p.p;
        p2.y = (m * (p.x - p2.x) - p.y) % p.p;

        if (p2.x < 0)
        {
            p2.x += p.p;
        }

        if (p2.y < 0)
        {
            p2.y += p.p;
        }

        return p2;
    }

    public static EllipticCurvePoint Multiply(EllipticCurvePoint p, BigInteger x)
    {
        var temp = p;
        x -= 1;

        while (x != 0)
        {
            if (x % 2 != 0)
            {
                if (temp.x == p.x || temp.y == p.y)
                {
                    temp = DoubleValue(temp);
                }
                else
                {
                    temp += p;
                }

                x -= 1;
            }

            x /= 2;
            p = DoubleValue(p);
        }

        return temp;
    }
}