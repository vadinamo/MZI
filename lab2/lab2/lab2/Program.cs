using System.Text;
using lab2;

var random = new Random();

byte[] GenerateByteArray(int count)
{
    var key = Enumerable.Range(0, count)
        .Select(_ => (byte)random.Next(0, 16))
        .ToArray();

    return key;
}

void Encrypt()
{
    var messageBytes = File.ReadAllBytes("./input.txt");

    var key = new byte[32];
    var syncMessage = new byte[16];

    Console.WriteLine("Generate new key and syncmessage? Y/n");
    var pick = Console.ReadLine();
    if (pick == "Y")
    {
        key = GenerateByteArray(32);
        syncMessage = GenerateByteArray(16);
    }
    else
    {
        key = File.ReadAllBytes("./key.txt");
        syncMessage = File.ReadAllBytes("./sync_message.txt");
    }
    Console.Clear();
    
    var cryptographer = new Cryptographer();
    var encryptedMessage = Encoding.UTF8.GetString(cryptographer.CounterMode(messageBytes, key, syncMessage));
    Console.WriteLine($"Encrypted message: {encryptedMessage}");

    File.WriteAllText("./encrypted_message.txt", encryptedMessage);
    File.WriteAllText("./key.txt", Encoding.UTF8.GetString(key));
    File.WriteAllText("./sync_message.txt", Encoding.UTF8.GetString(syncMessage));
}

void Decrypt()
{
    var encryptedMessage = File.ReadAllBytes("./encrypted_message.txt");
    Console.WriteLine($"Encrypted message: {Encoding.UTF8.GetString(encryptedMessage)}");
    var key = File.ReadAllBytes("./key.txt");
    var syncMessage = File.ReadAllBytes("./sync_message.txt");
    
    var cryptographer = new Cryptographer();
    var decryptedMessage = Encoding.UTF8.GetString(cryptographer.CounterMode(encryptedMessage, key, syncMessage));
    Console.WriteLine($"Decrypted message: {decryptedMessage}");
}

Encrypt();
Decrypt();