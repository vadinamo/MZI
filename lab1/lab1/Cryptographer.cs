namespace lab1;

public class Cryptographer
{
    private List<int[]> Key { get; set; }
    private List<int[]> SubstitutionTable { get; set; }
    private Random Random { get; set; }

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

        key = new List<int[]>
        {
            new int[] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1 },
            new int[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1 },
            new int[] { 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0 },
            new int[] { 0, 1, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1 },
            new int[] { 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0 },
            new int[] { 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0 },
            new int[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0 },
            new int[] { 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1 }
        };

        return key;
    }

    private List<int[]> GenerateSubstitutionTable()
    {
        var table = Enumerable.Range(0, 8)
            .Select(_ => Enumerable.Range(0, 16)
                .Select(_ => Random.Next(0, 16))
                .ToArray())
            .ToList();
        
        table = new List<int[]>
        {
            new int[] { 11, 5, 7, 11, 11, 7, 5, 1, 11, 5, 7, 6, 0, 12, 9, 3 },
            new int[] { 14, 15, 9, 1, 3, 11, 9, 0, 14, 0, 10, 2, 12, 6, 11, 14 },
            new int[] { 1, 6, 7, 8, 9, 2, 2, 2, 0, 10, 4, 9, 12, 14, 10, 14 },
            new int[] { 2, 9, 8, 7, 15, 0, 4, 14, 7, 9, 11, 4, 0, 13, 12, 6 },
            new int[] { 5, 15, 9, 4, 13, 6, 6, 2, 0, 5, 10, 14, 15, 12, 14, 6 },
            new int[] { 1, 14, 0, 3, 15, 6, 5, 8, 13, 0, 9, 9, 7, 0, 5, 3 },
            new int[] { 5, 12, 0, 14, 8, 13, 11, 10, 10, 13, 15, 10, 0, 10, 8, 6 },
            new int[] { 15, 10, 10, 6, 0, 2, 7, 10, 10, 0, 6, 8, 2, 9, 11, 14 }
        };

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

    private static int[] BytesToBits(IEnumerable<byte> bytes)
    {
        return bytes.SelectMany(b => Enumerable.Range(0, 8).Select(i => (b >> (7 - i)) & 1)).ToArray();
    }

    private static int[] ByteToBits(IEnumerable<byte> bytes)
    {
        return bytes.Take(4).SelectMany(b => Enumerable.Range(0, 8).Select(i => (b >> i) & 1)).ToArray()[..4];
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

    private int[] SimpleReplacementMode(int[] block)
    {
        foreach (var key in Key)
        {
            var n1 = block[..32];
            var n2 = block[32..];

            var sBits = BitXor(n1, key);
            var subsequences = new List<int[]>();
            for (var i = 0; i < sBits.Length; i += 4)
            {
                subsequences.Add(sBits[i..(i + 4)]);
            }

            var s = new List<int[]>();
            
            for (var i = 0; i < subsequences.Count; i++)
            {
                var newSI = SubstitutionTable[i][BitsToInt(subsequences[i])];
                Console.WriteLine(string.Join("", ByteToBits(BitConverter.GetBytes(newSI))));
                Console.WriteLine(newSI);
                s.Add(ByteToBits(BitConverter.GetBytes(newSI)));
            }

            // Console.WriteLine($"[[{string.Join("], [", subsequences.Select(s => string.Join(", ", s.Select(b => b))))}]]");
        }

        return block;
    }

    private int[] GetMAC(List<int[]> blocks)
    {
        var s = new int[64];
        foreach (var block in blocks)
        {
            for (var i = 0; i < 2; i++)
            {
                s = SimpleReplacementMode(BitXor(s, block));
            }
        }

        return s;
    }

    public void Encrypt(byte[] messageBytes)
    {
        var blocks = SplitMessageIntoBlocks(messageBytes);
        var mac = GetMAC(blocks);
        // Console.WriteLine(string.Join("\n", blocks.Select(block => string.Join("", block.Select(b => b)))));
    }
}