using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
		{
			Int32 M = 0;
			Int32 N = 0;

			while (true)
			{
				Console.Clear();
				Console.WriteLine("Input N and M");

				var input = Console.ReadLine();
				Regex regex = new Regex(@"^\s*(\d+)\s+(\d)+$\s*");

				if (input != null)
				{
					Match match = regex.Match(input);

					if (!match.Success)
					{
						Console.WriteLine("Wrong input, should be: M N where M and N - integers");
						continue;
					}

					Group group = match.Groups[0];
					M = Int32.Parse(group.Value);

					if (M < 0)
					{
						Console.WriteLine("Wrong input: M must be bigger then 0");
						continue;
					}

					group = match.Groups[1];
					N = Int32.Parse(group.Value);

					if (N < 0 || N > 9)
					{
						Console.WriteLine("Wrong input: N must be in [0, 9] range");
						continue;
					}
				}

				Int32 i = M;
				Int32 j = 0;
				Int32 k = 0;

				while (i > 0)
				{
					j += 1;
					i /= 10;
				}

				List<Int32> digitsHolder = new List<Int32>(j);
				SortedSet<Int32> cyclicSumSet = new SortedSet<Int32>();

                for (var currentNumber = 0; currentNumber <= M; currentNumber++)
				{
					if ()

					i = currentNumber;
					j = 0;

					var cyclicSum = 0;



					//Prepare for permutations
					while (i > 0)
					{
						digitsHolder[j] = i % 10;
                        i /= 10;
						j++;
					}
                }

			}



		}
    }
}
