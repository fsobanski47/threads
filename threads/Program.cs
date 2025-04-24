using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace threads
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter the number of threads: ");
                int threadCount = int.Parse(Console.ReadLine());
                if (threadCount <= 0)
                {
                    Console.WriteLine("Invalid number of threads.");
                    return;
                }
                Console.WriteLine("Enter the size of the matrix: ");
                int size = int.Parse(Console.ReadLine());
                if (size <= 0)
                {
                    Console.WriteLine("Invalid size of the matrix.");
                    return;
                }
                Console.WriteLine("Enter the seed for random number generation: ");
                int seed = int.Parse(Console.ReadLine());
                Matrix A = new Matrix(size, seed);
                Matrix B = new Matrix(size, seed + 10);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Matrix C = A.Multiply(B);
                //C.Print();
                watch.Stop();
                Console.WriteLine("Single Threaded Time: " + watch.ElapsedMilliseconds + " ms");
                var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };
                watch.Restart();
                Matrix D = A.MultiplyThreaded(B, threadCount);
                //Matrix D = A.MultiplyParallel(B, options);
                //D.Print();
                watch.Stop();
                Console.WriteLine("Multi Threaded Time: " + watch.ElapsedMilliseconds + " ms");
            }
        }             
    }
}
