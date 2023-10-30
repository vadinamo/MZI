using lab5;

var messageBytes = File.ReadAllBytes("./input.txt");

var cryptographer = new MD5();
Console.WriteLine(Convert.ToHexString(cryptographer.Hash(messageBytes)));