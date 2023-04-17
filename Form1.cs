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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            p = GetRandomBigInteger(1000, 10000);
            g = GetRandomBigInteger(700, (int)p);
            x = GetRandomBigInteger(end: (int)p);
            var input = richTextBox1.Text;
            y = BigInteger.ModPow(g, x, p);
            textBox1.Text = $"({p},{g},{y})";
            textBox2.Text = x.ToString();
            if (input.Length > 0)
            {
                char[] temp = input.ToCharArray();
                for (int i = 0; i <= input.Length - 1; i++)
                {
                    int m = temp[i];
                    if (m > 0)
                    {
                        var k = GetRandomBigInteger(1, (int)p - 1); // 1 < k < (p-1)
                        var a = BigInteger.ModPow(g, k, p);
                        var b = ModMultiply(BigInteger.ModPow(y, k, p), m, p);
                        richTextBox2.Text += $"{a} {b};";
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            var input = richTextBox1.Text;
            string[] strA = input.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (strA.Length > 0)
            {
                for (var i = 0; i < strA.Length; i++)
                {
                    var ai = BigInteger.Parse(strA[i].Split(' ')[0]);
                    var bi = BigInteger.Parse(strA[i].Split(' ')[1]);
                    if ((ai != 0) && (bi != 0))
                    {
                        var s = BigInteger.ModPow(ai, x, p);
                        var sInverse = ModInverse(s, p);
                        var m = BigInteger.Remainder(bi * sInverse, p);
                        var symbol = Encoding.UTF8.GetString(m.ToByteArray());
                        richTextBox2.Text += symbol;
                    }
                }
            }
        }

        private static BigInteger GetRandomBigInteger(int start = 0, int end = 100) =>
            new(random.Next(start, end));

        private static BigInteger ModMultiply(BigInteger a, BigInteger b, BigInteger n)
        {
            var sum = BigInteger.Zero;
            for (var i = 0; i < b; i++)
            {
                sum += a;
                if (sum >= n)
                {
                    sum -= n;
                }
            }
            return sum;
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

        public static bool IsPrime(long number)
        {
            if (number == 0 || number == 1)
            {
                return false;
            }
            int counter = 0;
            for (long i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    counter++;
                }
            }
            return counter == 0;
        }
    }
}