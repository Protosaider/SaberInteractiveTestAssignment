using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task3
{
    class Program
    {

		static (Int32, Int32) Input()
		{
			Int32 M = 0;
			Int32 N = 0;

			while (true)
			{
				Console.WriteLine("Input M and N");

				var input = Console.ReadLine();
				Regex regex = new Regex(@"^\s*(?<M>\d+)\s+(?<N>\d)+$\s*");

				if (input != null)
				{
					Match match = regex.Match(input);

					if (!match.Success)
					{
						Console.Clear();
                        Console.WriteLine("Wrong input, should be: M N where M and N - integers");
						continue;
					}

					Group group = match.Groups["M"];
					M = Int32.Parse(group.Value);

					if (M < 0)
					{
						Console.Clear();
						Console.WriteLine("Wrong input: M must be bigger then 0");
						continue;
					}

					group = match.Groups["N"];
					N = Int32.Parse(group.Value);

					if (N < 0 || N > 9)
					{
						Console.Clear();
						Console.WriteLine("Wrong input: N must be in [0, 9] range");
						continue;
					}
				}
				else
					continue;

				break;
			}

			Console.Clear();
			Console.WriteLine($@"M = {M}, N = {N}");

            return (M, N);
		}

		static Int32 CalculateCyclicSum(Int32 number)
		{
            Int32 i = number;
			Int32 j = 0;
			Int32 sum = 0;

            while (true)
			{
				while (i > 0)
				{
					j += 1;
					sum += i % 10;
					i /= 10;
				}

				if (j == 1 || j == 0)
					return sum;

				i = sum;
				j = 0;
				sum = 0;
			}
		}

		static void Swap<T>(ref T from, ref T to)
		{
			var temp = from;
			from = to;
			to = temp;
		}

        private static void PermuteWithRepetitions(ref List<Int32> resultInts, ref Int32 m, ref Int32 currentNumber, Int32[] arrayToPermute, Int32 endIndex, Int32 startIndex = 0)
		{
			//Print(arrayToPermute);
			Int32 digitDecade = (Int32)Math.Pow(10, endIndex);
			Int32 result = 0;

			for (var i = 0; i <= endIndex; i++)
			{
				result += arrayToPermute[i] * digitDecade;
				digitDecade /= 10;
            }

			if (result <= m && result > currentNumber)
				resultInts.Add(result);


            for (var left = endIndex - 1; left >= startIndex; left--)
			{
				for (var right = left + 1; right <= endIndex; right++)
				{
					if (arrayToPermute[left].CompareTo(arrayToPermute[right]) == 0)
						continue;

					Swap(ref arrayToPermute[left], ref arrayToPermute[right]);
					PermuteWithRepetitions(ref resultInts, ref m, ref currentNumber, arrayToPermute, endIndex, left + 1);
				}

				var firstElement = arrayToPermute[left];

				for (var i = left; i <= endIndex - 1; i++)
					arrayToPermute[i] = arrayToPermute[i + 1];

				arrayToPermute[endIndex] = firstElement;
			}
		}


        ////      Boolean NextSet(List<Int32> a, Int32 n)
        ////{
        ////	Int32 j = n - 2;
        ////	while (j != -1 && a[j] >= a[j + 1])
        ////		j--;
        ////	if (j == -1)
        ////		return false; // больше перестановок нет
        ////	Int32 k = n - 1;
        ////	while (a[j] >= a[k])
        ////		k--;
        ////	swap(a, j, k);
        ////	Int32 l = j + 1, r = n - 1; // сортируем оставшуюся часть последовательности
        ////	while (l < r)
        ////		swap(a, l++, r--);
        ////	return true;
        ////}

		static void FindStraightforward(ref SortedSet<Int32> sumSet, Int32 m, Int32 n)
		{
			if (n == 0)
			{
				sumSet.Add(0);
				return;
			}

			for (var k = 1; k <= m; k++)
			{
				var sum = CalculateCyclicSum(k);

				if (sum == n)
					sumSet.Add(k);
			}
		}

        static void FindCustom(ref SortedSet<Int32> sumSet, Int32 m, Int32 n)
		{
			if (n == 0)
			{
				sumSet.Add(0);
				return;
			}

            Int32 i = m;
            Int32 j = 0;

            while (i > 0)
            {
                j += 1;
                i /= 10;
            }

            Int32[] digitsHolder = new Int32[j];
            var digitDecades = 0;
			var dividerToFindDecade = 10;
			List<Int32> permutationsList = new List<Int32>(2);

            for (var currentNumber = 1; currentNumber <= m; currentNumber++)
            {
				//check if already inside
				if (sumSet.Contains(currentNumber))
					continue;

				var cyclicSum = CalculateCyclicSum(currentNumber);

				if (cyclicSum == n)
				{
					if (currentNumber / dividerToFindDecade > 0)
					{
						dividerToFindDecade *= 10;
						digitDecades++;
						permutationsList = new List<Int32>((Int32)Math.Pow(2, digitDecades));
                    }

                    sumSet.Add(currentNumber);

                    //Prepare for permutations
					i = currentNumber;
					j = 0;
                    while (i > 0)
					{
						digitsHolder[j] = i % 10;
						i /= 10;
						j++;
					}

					permutationsList.Clear();
					PermuteWithRepetitions(ref permutationsList, ref m, ref currentNumber, digitsHolder, digitDecades,
						0);

					foreach (var value in permutationsList)
						sumSet.Add(value);
				}

            }

        }

        static void Main(String[] args)
		{
			Int32 M;
			Int32 N;

			var mn = Input();

			M = mn.Item1;
			N = mn.Item2;

			SortedSet<Int32> cyclicSumSet = new SortedSet<Int32>();


			////PerformanceCounter performanceCounter = new PerformanceCounter();
			////performanceCounter.CategoryName = "Process";
			////performanceCounter.CounterName = "Working Set";
			////performanceCounter.InstanceName = Process.GetCurrentProcess().ProcessName;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			
			//var memoryBefore = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
			var memoryBefore = GC.GetTotalMemory(false);
            sw.Start();

			FindStraightforward(ref cyclicSumSet, M, N);

            sw.Stop();
			//var memoryAfter = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
			var memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine("Straightforward");
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine(memoryAfter - memoryBefore);

			cyclicSumSet.Clear();
            sw.Reset();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			//memoryBefore = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
			memoryBefore = GC.GetTotalMemory(false);
            sw.Start();

			FindCustom(ref cyclicSumSet, M, N);

			sw.Stop();
			//memoryAfter = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
			memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine("Custom");
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine(memoryAfter - memoryBefore);

            //PrintResult(cyclicSumSet);
            WriteResultToFile(cyclicSumSet);

			Console.ReadLine();
		}

        private static void PrintResult(SortedSet<Int32> cyclicSumSet)
		{
			Console.WriteLine("Result:");
			foreach (var i in cyclicSumSet)
				Console.WriteLine(i);
		}

		private static void WriteResultToFile(SortedSet<Int32> cyclicSumSet)
		{
			using (StreamWriter sw = new StreamWriter("Result.txt"))
			{
				foreach (var i in cyclicSumSet)
					sw.WriteLine(i);
			}
        }

    }
}
