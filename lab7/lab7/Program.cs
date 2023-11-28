using System.Numerics;
using System.Text;
using lab7;

var message = File.ReadAllBytes("./input.txt");

var P = new EllipticCurvePoint(
    BigInteger.Parse("2"),
    BigInteger.Parse("4018974056539037503335449422937059775635739389905545080690979365213431566280"),
    BigInteger.Parse("7"),
    BigInteger.Parse("43308876546767276905765904595650931995942111794451039583252968842033849580414"),
    BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564821041")
);

var d = BigInteger.Parse("47296044618658097711785492524343953912234992332820282019728792003956564821041");
var Q = EllipticCurvePoint.Multiply(P, d);

var CValues = ElGamal.Encrypt(message, P, Q);
Console.WriteLine($"C values:\nC1(X: {CValues[0].x}, Y: {CValues[0].y})\nC2(X: {CValues[1].x}, Y: {CValues[1].y})\n");
Console.WriteLine($"Decrypted message:\n{Encoding.UTF8.GetString(ElGamal.Decrypt(CValues, d))}");