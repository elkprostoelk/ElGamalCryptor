using System.Numerics;
using System.Text;

namespace ElGamalCryptor
{
    public partial class Form1 : Form
    {
        private static readonly Random random = new();
        private BigInteger p, g, x, y;

        public Form1()
        {
            InitializeComponent();
            GenerateKeys();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            var input = richTextBox1.Text;
            List<Tuple<BigInteger, BigInteger>> cipherText = new();
            BigInteger k = GetRandomBigInteger(1, (int)p - 2);

            foreach (char ch in input)
            {
                BigInteger m = (BigInteger)ch;
                BigInteger a = BigInteger.ModPow(g, k, p);
                BigInteger b = (BigInteger.ModPow(y, k, p) * m) % p;
                cipherText.Add(new Tuple<BigInteger, BigInteger>(a, b));
            }

            richTextBox2.Text = string.Join(';', cipherText.Select(ch => $"{ch.Item1},{ch.Item2}"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            var input = richTextBox1.Text;
            List<Tuple<BigInteger, BigInteger>> cipherText = new();
            foreach (var item in input.Split(';'))
            {
                cipherText.Add(new (
                    BigInteger.Parse(item.Split(',').First()),
                    BigInteger.Parse(item.Split(',').Last())
                    ));
            }
            foreach (var pair in cipherText)
            {
                BigInteger a = pair.Item1;
                BigInteger b = pair.Item2;
                BigInteger aInverse = ModInverse(BigInteger.ModPow(a, x, p), p);
                BigInteger m = aInverse * b % p;
                richTextBox2.Text += (char)(int)m;
            }
        }

        private static BigInteger GetRandomBigInteger(int start = 0, int end = 100)
        {
            byte[] bytes = new byte[end / 8];
            random.NextBytes(bytes);
            BigInteger result = new(bytes);
            return BigInteger.Abs(result % (end - start)) + start;
        }

        public static BigInteger ModInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        public void GenerateKeys()
        {
            p = GenerateLargePrime();
            g = GetRandomBigInteger(2, (int)p - 1);
            x = GetRandomBigInteger(1, (int)p - 2);
            y = BigInteger.ModPow(g, x, p);
            textBox1.Text = $"({p},{g},{y})";
            textBox2.Text = x.ToString();
        }

        private BigInteger GenerateLargePrime()
        {
            while (true)
            {
                BigInteger primeCandidate = GetRandomBigInteger(32768, 65536);
                if (IsPrime(primeCandidate, 10))
                {
                    return primeCandidate;
                }
            }
        }

        private bool IsPrime(BigInteger n, int k)
        {
            if (n == 2 || n == 3)
                return true;
            if (n <= 1 || n % 2 == 0)
                return false;

            BigInteger r = 0;
            BigInteger s = n - 1;
            while (s % 2 == 0)
            {
                r++;
                s /= 2;
            }

            for (int i = 0; i < k; i++)
            {
                BigInteger a = GetRandomBigInteger(2, (int)n - 2);
                BigInteger x = BigInteger.ModPow(a, s, n);
                if (x == 1 || x == n - 1)
                    continue;

                for (var j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }
            return true;
        }
    }
}