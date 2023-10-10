using lab2;

var random = new Random();

byte[] GenerateByteArray(int count)
{
    var key = Enumerable.Range(0, count)
        .Select(_ => (byte)random.Next(0, 16))
        .ToArray();

    return key;
}

var messageBytes = File.ReadAllBytes("./input.txt");

var cryptographer = new Cryptographer();
var result = cryptographer.Encrypt(messageBytes, GenerateByteArray(32), GenerateByteArray(16));

Console.WriteLine(string.Join(" ", messageBytes));
Console.WriteLine(string.Join(" ", result));