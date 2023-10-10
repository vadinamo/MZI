using System.Text;
using lab2;

var random = new Random();

byte[] GenerateByteArray(int count)
{
    var key = Enumerable.Range(0, count)
        .Select(_ => (byte)random.Next(0, 256))
        .ToArray();

    return key;
}

void Encrypt()
{
    var messageBytes = File.ReadAllBytes("./input.txt");

    var key = GenerateByteArray(32);
    var syncMessage = GenerateByteArray(16);
    
    var cryptographer = new Cryptographer();
    var encryptedMessageBytes = cryptographer.CounterMode(messageBytes, key, syncMessage);
    Console.WriteLine($"Encrypted message: {Encoding.UTF8.GetString(encryptedMessageBytes)}");

    File.WriteAllBytes("./encrypted_message.txt", encryptedMessageBytes);
    File.WriteAllBytes("./key.txt", key);
    File.WriteAllBytes("./sync_message.txt", syncMessage);
}

void Decrypt()
{
    var encryptedMessageBytes = File.ReadAllBytes("./encrypted_message.txt");
    var key = File.ReadAllBytes("./key.txt");
    var syncMessage = File.ReadAllBytes("./sync_message.txt");
    
    var cryptographer = new Cryptographer();
    var decryptedMessage = Encoding.UTF8.GetString(cryptographer.CounterMode(encryptedMessageBytes, key, syncMessage));
    Console.WriteLine($"Decrypted message: {decryptedMessage}");
}

Encrypt();
Decrypt();