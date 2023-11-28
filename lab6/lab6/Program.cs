using System.Numerics;
using System.Text;
using lab6;

var message = File.ReadAllBytes("./input.txt");

var q = BigInteger.Parse("57896044618658097711785492504343953927082934583725450622380973592137631069619");

var P = new EllipticCurvePoint(
    BigInteger.Parse("2"),
    BigInteger.Parse("4018974056539037503335449422937059775635739389905545080690979365213431566280"),
    BigInteger.Parse("7"),
    BigInteger.Parse("43308876546767276905765904595650931995942111794451039583252968842033849580414"),
    BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564821041")
);

var d = BigInteger.Parse("47296044618658097711785492524343953912234992332820282019728792003956564821041");
var Q = EllipticCurvePoint.Multiply(P, d);

Console.WriteLine($"Initial message:\n{Encoding.UTF8.GetString(message)}\n");
var digitalSignature = DigitalSignature.GetSignature(message, q, P, d);
File.WriteAllBytes("./digital_signature.txt",digitalSignature);
Console.WriteLine($"Digital signature:\n{Encoding.UTF8.GetString(digitalSignature)}\n");
// Console.WriteLine($"Digital signature:\n{string.Join("", digitalSignature.Select(ds => ds.ToString("X")))}\n");

digitalSignature = File.ReadAllBytes("./digital_signature.txt");
var validity = DigitalSignature.CheckValidity(digitalSignature, message, q, P, Q);
Console.WriteLine($"Signature is{(validity ? "" : " not")} valid.");