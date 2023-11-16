// See https://aka.ms/new-console-template for more information

using System.Numerics;
using lab6;

var message = "Hello world!";

var m = BigInteger.Parse("6277101735386680763835789423176059013767194773182842284081");
var q = BigInteger.Parse("6277101735386680763835789423176059013767194773182842284081");

var P = new EllipticCurvePoint(
    BigInteger.Parse("602046282375688656758213480587526111916698976636884684818"),
    BigInteger.Parse("174050332293622031404857552280219410364023488927386650641"),
    BigInteger.Parse("-3"),
    BigInteger.Parse("2455155546008943817740293915197451784769108058161191238065"),
    BigInteger.Parse("6277101735386680763835789423207666416083908700390324961279")
);

var d = BigInteger.Parse("123123");
var Q = EllipticCurvePoint.Multiply(P, d);

var digitalSignature = DigitalSignature.GetSignature(message, m, q, P, d, Q);
foreach (var x in digitalSignature)
{
    Console.Write(x.ToString("X"));
}
