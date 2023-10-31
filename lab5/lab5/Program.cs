using lab5;

var messageBytes = File.ReadAllBytes("./input.txt");

Console.WriteLine($"GOST31.11 Hash:\n{Convert.ToHexString(GOST3411.Hash(messageBytes))}");
Console.WriteLine($"MD5 Hash:\n{Convert.ToHexString(MD5.Hash(messageBytes))}");