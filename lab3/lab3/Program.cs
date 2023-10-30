using System.Text;
using lab3;


const int p = 307;
const int q = 283;

void Encrypt(byte[] messageBytes)
{
    var encryptedBytes = Cryptographer.Encrypt(messageBytes, p * q);
    File.WriteAllBytes("./encrypted_message.txt", encryptedBytes);
    Console.WriteLine($"Encrypted message: {Encoding.UTF8.GetString(encryptedBytes)}");
}

void Decrypt(byte[] encryptedBytes)
{
    var decryptedBytes = Cryptographer.Decrypt(encryptedBytes, p, q);
    Console.WriteLine($"Decrypted message: {Encoding.UTF8.GetString(decryptedBytes)}");
}

Encrypt(File.ReadAllBytes("./input.txt"));
Decrypt(File.ReadAllBytes("./encrypted_message.txt"));