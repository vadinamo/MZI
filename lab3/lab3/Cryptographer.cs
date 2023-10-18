namespace lab3;

public static class Cryptographer
{
    public static byte[] Encrypt(byte[] messageBytes, int n)
    {
        var result = new List<byte>();
        foreach (var messageByte in messageBytes)
        {
            result.AddRange(BitConverter.GetBytes((ushort)(messageByte * messageByte % n)));
        }

        return result.ToArray();
    }

    private static double ModularPow(double c, double k, int m)
    {
        var result = 1.0;
        for (var i = 0; i < k; i++)
        {
            result = (result * c) % m;
        }

        return result;
    }

    public static byte[] Decrypt(byte[] messageBytes, int p, int q)
    {
        var (yp, yq) = EuclidAlgorithm(p, q);
        var result = new List<byte>();
        for (var i = 0; i < messageBytes.Length / 2; i++)
        {
            var c = BitConverter.ToUInt16(messageBytes.Skip(i * 2).Take(2).ToArray());
            var variants = ChineseRemainderTheorem(c, p, q, yp, yq);
            foreach (var variant in variants)
            {
                var literalBytes = BitConverter.GetBytes(variant);
                if (literalBytes[1] == 0)
                {
                    result.Add(literalBytes[0]);
                }
            }
        }

        return result.ToArray();
    }

    private static List<ushort> ChineseRemainderTheorem(ushort c, int p, int q, int yp, int yq)
    {
        var n = p * q;
        var mp = ModularPow(c, (p + 1) / 4.0, p);
        var mq = ModularPow(c, (q + 1) / 4.0, q);

        var r1 = (yp * p * mq + yq * q * mp) % n;
        r1 += r1 < 0 ? n : 0;
        var r2 = n - r1;
        var r3 = (yp * p * mq - yq * q * mp) % n;
        r3 += r3 < 0 ? n : 0;
        var r4 = n - r3;

        return new List<ushort>
        {
            (ushort)r1,
            (ushort)r2,
            (ushort)r3,
            (ushort)r4
        };
    }

    private static (int, int) EuclidAlgorithm(int p, int q)
    {
        int x0 = 1, x1 = 0, y0 = 0, y1 = 1;

        while (q != 0)
        {
            var d = p / q;
            var r = p % q;

            p = q;
            q = r;

            var x = x0 - x1 * d;
            var y = y0 - y1 * d;

            x0 = x1;
            y0 = y1;
            x1 = x;
            y1 = y;
        }

        return (x0, y0);
    }
}