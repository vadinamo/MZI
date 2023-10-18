using System.Numerics;
using System.Text;
using lab3;
using static System.Numerics.BigInteger;

var messageBytes = File.ReadAllBytes("./input.txt");
const int p = 239;
const int q = 283;

var encryptedBytes = Cryptographer.Encrypt(messageBytes, p * q);
var decryptedBytes = Cryptographer.Decrypt(encryptedBytes, p, q);

Console.WriteLine(Encoding.UTF8.GetString(decryptedBytes));