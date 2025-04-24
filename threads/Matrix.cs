using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace threads
{
    class Matrix
    {
        private readonly int Size;
        private readonly List<List<int>> Content;

        public Matrix(int size, int seed)
        {
            Size = size;
            Random random = new Random(seed);
            Content = new List<List<int>>();
            for(int i = 0; i < Size; i++)
            {
                Content.Add(new List<int>());
                for(int j = 0; j < Size; j++)
                {
                    Content[i].Add(random.Next(100));
                }
            }
        }

        public void Print()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Console.Write(Content[i][j] + " ");
                }
                Console.WriteLine();
            }
        }

        public Matrix Multiply(Matrix second)
        {
            Matrix result = new Matrix(Size, 0);
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    result.Content[i][j] = 0;
                    for (int k = 0; k < Size; k++)
                    {
                        result.Content[i][j] += Content[i][k] * second.Content[k][j];
                    }
                }
            }
            return result;
        }

        public Matrix MultiplyParallel(Matrix second, ParallelOptions? options = null)
        {
            Matrix result = new Matrix(Size, 0);
            Parallel.For(0, Size, options ?? new ParallelOptions(), i =>
            {
                for (int j = 0; j < Size; j++)
                {
                    result.Content[i][j] = 0;
                    for (int k = 0; k < Size; k++)
                    {
                        result.Content[i][j] += Content[i][k] * second.Content[k][j];
                    }
                }
            });
            return result;
        }

        public Matrix MultiplyThreaded(Matrix second, int threadCount)
        {
            if(threadCount <= 0)
            {
                throw new ArgumentException("Invalid number of threads.");
            }

            Matrix result = new Matrix(Size, 0);
            var threads = new List<Thread>();
            int rowsPerThread = (Size + threadCount - 1) / threadCount;
            using var mtx = new Mutex();

            for (int i = 0; i < threadCount; i++)
            {
                int startRow = i * rowsPerThread;
                int endRow = Math.Min(startRow + rowsPerThread, Size);
                if(startRow >= Size)
                {
                    break;
                }

                var thread = new Thread(() =>
                {
                    for (int j = startRow; j < endRow; j++)
                    {
                        for (int k = 0; k < Size; k++)
                        {
                            int sum = 0;
                            for (int l = 0; l < Size; l++)
                            {
                                sum += Content[j][l] * second.Content[l][k];
                            }
                            result.Content[j][k] = sum;
                        }
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }
    }
}
