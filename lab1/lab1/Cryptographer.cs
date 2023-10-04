using System.Text;

namespace lab1;

public class Cryptographer
{
    private List<int[]> Key { get; set; }
    private List<int[]> SubstitutionTable { get; set; }
    private Random Random { get; }

    public Cryptographer()
    {
        Random = new Random();
        Key = GenerateKey();
        SubstitutionTable = GenerateSubstitutionTable();
    }

    private List<int[]> GenerateKey()
    {
        var key = Enumerable.Range(0, 8)
            .Select(_ => Enumerable.Range(0, 32)
                .Select(_ => Random.Next(0, 2))
                .ToArray())
            .ToList();

        return key;
    }

    private List<int[]> GenerateSubstitutionTable()
    {
        var table = Enumerable.Range(0, 8)
            .Select(_ => Enumerable.Range(0, 16)
                .Select(_ => Random.Next(0, 16))
                .ToArray())
            .ToList();

        return table;
    }

    private static List<int[]> SplitMessageIntoBlocks(IReadOnlyList<byte> messageBytes)
    {
        var blocks = new List<int[]>();
        for (var i = 0; i < messageBytes.Count; i += 8)
        {
            var block = new byte[8];
            for (var j = 0; j < 8; j++)
            {
                if (i + j < messageBytes.Count)
                {
                    block[j] = messageBytes[i + j];
                }
                else
                {
                    block[j] = 0;
                }
            }

            blocks.Add(BytesToBits(block));
        }

        return blocks;
    }

    private int[] AddModulo32(int[] array1, int[] array2)
    {
        if (array1.Length != array2.Length)
        {
            throw new ArgumentException("Arrays must have the same length");
        }

        var result = new int[array1.Length];
        var carry = 0;

        for (var i = array1.Length - 1; i >= 0; i--)
        {
            var sum = array1[i] + array2[i] + carry;
            result[i] = sum & 1;
            carry = sum >> 1;
        }

        return result;
    }


    private int[] SimpleReplacementMode(int[] block, int roundCount, bool encrypt)
    {
        var n1 = block[..32];
        var n2 = block[32..];

        for (var round = 0; round < roundCount; round++)
        {
            var sBits = AddModulo32(n1, Key[
                encrypt
                ? round < 24 ? round % 8 : 7 - round % 8
                : round < 8 ? round % 8 : 7 - round % 8
            ]);
            var subsequences = new List<int[]>();
            for (var i = 0; i < sBits.Length; i += 4)
            {
                subsequences.Add(sBits[i..(i + 4)]);
            }

            var s = new List<int>();

            for (var i = 0; i < subsequences.Count; i++)
            {
                var newSI = SubstitutionTable[i][BitsToInt(subsequences[i])];
                s.AddRange(ByteToBits(BitConverter.GetBytes(newSI)));
            }

            var shiftedS = BitwiseOr(ShiftBitsLeft(s, 11), ShiftBitsRight(s, 21));
            var result = BitXor(shiftedS, n2);

            if (round < 31)
            {
                n2 = n1;
                n1 = result;
            }
            else
            {
                n2 = result;
            }
        }

        return n1.Concat(n2).ToArray();
        ;
    }

    private int[] GetMAC(List<int[]> blocks)
    {
        var s = new int[64];
        for (var i = 0; i < blocks.Count; i++)
        {
            s = SimpleReplacementMode(BitXor(s, blocks[i]), 16, true);
            blocks[i] = SimpleReplacementMode(blocks[i], 32, true);
        }

        return s;
    }

    public void Encrypt()
    {
        var messageBytes = File.ReadAllBytes("./input.txt");
        var blocks = SplitMessageIntoBlocks(messageBytes);

        var mac = GetMAC(blocks)[..32];
        Console.WriteLine($"MAC: {string.Join("", mac)}");
        File.WriteAllText("./MAC.txt", string.Join("", mac));

        Console.WriteLine(
            $"Encrypted message: {Encoding.UTF8.GetString(BitsToBytes(blocks.SelectMany(b => b).ToArray()))}");
        File.WriteAllText("./encrypted_message.txt",
            string.Join("", string.Join("\n", blocks.Select(block => string.Join("", block.Select(b => b))))));

        File.WriteAllText("./key.txt", string.Join("\n", Key.Select(k => string.Join(" ", k))));
        File.WriteAllText("./substitution_table.txt",
            string.Join("\n", SubstitutionTable.Select(k => string.Join(" ", k))));
    }

    public void Decrypt()
    {
        Key = File.ReadAllLines("./key.txt")
            .Select(line => line.Split(' ').Select(int.Parse).ToArray())
            .ToList();
        SubstitutionTable = File.ReadAllLines("./substitution_table.txt")
            .Select(line => line.Split(' ').Select(int.Parse).ToArray())
            .ToList();

        var messageBits = File.ReadAllText("./encrypted_message.txt");
        var blocks = messageBits
            .Split('\n')
            .Select(line => line
                .Select(c => int.Parse(c.ToString())
                ).ToArray()
            ).ToList();

        for (var i = 0; i < blocks.Count; i++)
        {
            blocks[i] = SimpleReplacementMode(blocks[i], 32, false);
        }

        var message = Encoding.UTF8.GetString(BitsToBytes(blocks.SelectMany(b => b).ToArray()));

        var mac = GetMAC(blocks)[..32];
        var initialMac = File.ReadAllText("./MAC.txt");
        if (string.Join("", mac) != initialMac)
        {
            throw new Exception("MAC does not match");
        }

        Console.WriteLine($"Decrypted message: {message}");
    }

    private static byte[] BitsToBytes(IReadOnlyList<int> bits)
    {
        var numBytes = (bits.Count + 7) / 8;

        var bytes = new byte[numBytes];
        for (var i = 0; i < bits.Count; i++)
        {
            var byteIndex = i / 8;
            var bitIndex = i % 8;
            bytes[byteIndex] |= (byte)(bits[i] << (7 - bitIndex));
        }

        return bytes;
    }


    private static int[] BytesToBits(IEnumerable<byte> bytes)
    {
        return bytes.SelectMany(b => Enumerable.Range(0, 8).Select(i => (b >> (7 - i)) & 1)).ToArray();
    }

    private static int[] ByteToBits(IEnumerable<byte> bytes)
    {
        return bytes.Take(4).SelectMany(b => Enumerable.Range(0, 8).Select(i => (b >> i) & 1)).Reverse()
            .ToArray()[^4..];
    }

    private static int[] BitXor(IReadOnlyList<int> array1, IReadOnlyList<int> array2)
    {
        if (array1.Count != array2.Count)
        {
            throw new ArgumentException("Arrays should have same length.");
        }

        var result = new int[array1.Count];
        for (var i = 0; i < array1.Count; i++)
        {
            result[i] = array1[i] ^ array2[i];
        }

        return result;
    }

    private static int BitsToInt(IEnumerable<int> bits)
    {
        return Convert.ToInt32(string.Join("", bits), 2);
    }

    private static int[] ShiftBitsLeft(IReadOnlyList<int> bits, int shiftAmount)
    {
        var result = new int[bits.Count];

        for (var i = shiftAmount; i < bits.Count; i++)
        {
            result[i - shiftAmount] = bits[i];
        }

        return result;
    }

    private static int[] ShiftBitsRight(IReadOnlyList<int> bits, int shiftAmount)
    {
        var result = new int[bits.Count];

        for (var i = 0; i < bits.Count - shiftAmount; i++)
        {
            result[i + shiftAmount] = bits[i];
        }

        return result;
    }

    private static int[] BitwiseOr(int[] bits1, int[] bits2)
    {
        if (bits1.Length != bits2.Length)
        {
            throw new ArgumentException("Arrays should have same length.");
        }

        var result = new int[bits1.Length];

        for (var i = 0; i < bits1.Length; i++)
        {
            result[i] = bits1[i] | bits2[i];
        }

        return result;
    }
}