using System.Numerics;
using System.Text;

namespace lab6;

public class DigitalSignature
{
    private BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max)
    {
        var random = new Random();
        var data = new byte[max.ToByteArray().Length];
        random.NextBytes(data);

        return new BigInteger(data) % (max - min) + min;
    } 
        
    public void GetSignature(string message, BigInteger p, BigInteger a, BigInteger b, BigInteger q)
    {
        var hash = GOST3411.Hash(Encoding.UTF8.GetBytes(message));
        var alpha = new BigInteger(hash);

        var e = alpha % q;
        if (e == 0)
        {
            e = 1;
        }

        var k = GenerateRandomBigInteger(0, q);
    }
}