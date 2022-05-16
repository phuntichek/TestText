using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;

namespace TextAnalysis
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к текстовому файлу:");
            string adress1 = Console.ReadLine();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            char[] arr = System.IO.File.ReadAllText(adress1).ToCharArray();
            int taskCount = Environment.ProcessorCount;
            int c = 0;
            ConcurrentDictionary<(char, char, char), int> countMap = new ConcurrentDictionary<(char, char, char), int>();

            for (int i = 1; i <= taskCount; i++)
            {
                Chunk chunk = new Chunk(arr, (arr.Length - arr.Length / i), (arr.Length / i + 1), countMap);
                Thread t = new Thread(new ThreadStart(chunk.SearchChunk));
                t.Start();
                t.Join();
            }

            var countMap1 = countMap.OrderByDescending(pair => pair.Value);
            foreach (var item in countMap1)
            {
                Console.WriteLine(item.Key + " " + item.Value);
                c++;
                if (c == 10)
                    break;
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00} часов, {1:00} минут, {2:00} секунд, {3:000} миллисекунд",
                       ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            Console.WriteLine("Время выполнения программы: " + elapsedTime);

            Console.WriteLine("Нажмите любую клавишу для выхода из программы");
            Console.ReadKey();

            Console.WriteLine();
        }
    }
    public class Chunk
    {
        char[] arr;
        int startIndex;
        int endIndex;
        ConcurrentDictionary<(char, char, char), int> countMap;

        public Chunk(char[] _arr, int _startIndex, int _endIndex, ConcurrentDictionary<(char, char, char), int> _countMap)
        {
            this.arr = _arr;
            this.startIndex = _startIndex;
            this.endIndex = _endIndex;
            this.countMap = _countMap;
        }
        public void SearchChunk()
        {
            bool flag = true;
            for (int i = startIndex; i < endIndex && i < arr.Length - 3; i++)
            {
                if (startIndex != 0 && char.IsLetter(arr[startIndex]) && flag == true)
                {
                    flag = false;
                    while (char.IsLetter(arr[i]) && i > 0)
                        i--;
                }
                if (char.IsLetter(arr[i]) & char.IsLetter(arr[i + 1]) & char.IsLetter(arr[i + 2]))
                {
                    (char, char, char) triplet = (arr[i], arr[i + 1], arr[i + 2]);
                    if (!countMap.ContainsKey(triplet))
                    {
                        countMap[triplet] = 1;
                    }
                    else
                    {
                        ++countMap[triplet];
                    }
                }
            }
        }
    }
}
