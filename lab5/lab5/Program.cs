using lab5;

var messageBytes = File.ReadAllBytes("./input.txt");

var cryptographer = new MD5();
cryptographer.Hash(messageBytes);