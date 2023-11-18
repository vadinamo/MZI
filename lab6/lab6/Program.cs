using System.Numerics;
using System.Text;
using lab6;

var message = File.ReadAllBytes("./input.txt");

var m = BigInteger.Parse("57896044618658097711785492504343953927082934583725450622380973592137631069619");
var q = BigInteger.Parse("57896044618658097711785492504343953927082934583725450622380973592137631069619");

var P = new EllipticCurvePoint(
    BigInteger.Parse("2"),
    BigInteger.Parse("4018974056539037503335449422937059775635739389905545080690979365213431566280"),
    BigInteger.Parse("7"),
    BigInteger.Parse("43308876546767276905765904595650931995942111794451039583252968842033849580414"),
    BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564821041")
);

var d = BigInteger.Parse("55441196065363246126355624130324183196576709222340016572108097750006097525544");
var Q = EllipticCurvePoint.Multiply(P, d);

Console.WriteLine($"Initial message:\n{Encoding.UTF8.GetString(message)}\n");
var digitalSignature = DigitalSignature.GetSignature(message, m, q, P, d, Q);
File.WriteAllBytes("./digital_signature.txt",digitalSignature);
Console.WriteLine($"Digital signature:\n{Encoding.UTF8.GetString(digitalSignature)}\n");

var validity = DigitalSignature.CheckValidity(digitalSignature, message, m, q, P, d, Q);
Console.WriteLine($"Signature is{(validity ? "" : " not")} valid.");