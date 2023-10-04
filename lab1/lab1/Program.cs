using lab1;


var message = File.ReadAllBytes("./input.txt");

// Console.WriteLine($"Initial message: {Encoding.UTF8.GetString(message)}");

var cryptographer = new Cryptographer();
cryptographer.Encrypt();
cryptographer.Decrypt();