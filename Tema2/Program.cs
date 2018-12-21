using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema2
{

	static class Constants
	{
		public const int CHR_LENGTH = 8;
	}

	class BinNum
	{

		private Random rnd = new Random();
		private List<int> num;
		private int numLength;

		public BinNum()
		{
			num = new List<int>();
			numLength = 0;
		}
		public BinNum(BinNum number)
		{
			num = new List<int>(number.num);
			numLength = number.numLength;
		}
		public void Reset()
		{
			while(num.Any())
			{
				num.RemoveAt(numLength - 1);
				numLength--;
			}
		}
		public void RandomSet()
		{
			for (int i = 0; i < Constants.CHR_LENGTH; i++)
			{
				if (rnd.NextDouble() < 0.5)
				{
					num.Add(0);
					numLength++;
				}
				else
				{
					num.Add(1);
					numLength++;
				}
			}
		}
		public void Display()
		{
			for (int i = 0; i < numLength; i++)
				Console.Write(num[i].ToString());
			Console.Write("\n");
		}
		public double GetNum(double min, double max)
		{
			double number = 0;
			double pow = 1;
			for (int i = 7; i >= 0; i--)
			{
				number += num[i] * pow;
				pow *= 2;
			}

			return (min + number * (max - min) / (Math.Pow(2, Constants.CHR_LENGTH) - 1));
		}
		public void ShiftBit(int index)
		{
			num[index] = (num[index] + 1) % 2;
		}
	}

	static class Functions
	{
		public static double Rastrigin(List<BinNum> pop)
		{
			double sum = 10 * pop.Count();
			double min = -5.12;
			double max = 5.12;
			for (int i = 0; i < pop.Count(); i++)
				sum += pop[i].GetNum(min, max);

			return sum;
		}
	}

	static class popOp
	{
		public static void InitPop(int dim, List<BinNum>pop)
		{
			BinNum chromozome = new BinNum();
			for(int i = 0; i < dim; i++)
			{
				chromozome.Reset();
				chromozome.RandomSet();
				pop.Add(new BinNum(chromozome));
			}
		}
	}

	class Program
	{
		
		static void Main(string[] args)
		{
			BinNum chromozome = new BinNum();
			List<BinNum> Population = new List<BinNum>();

			//5 dimensiuni
			popOp.InitPop(5, Population);
			Console.WriteLine(Functions.Rastrigin(Population).ToString());
		}
	}
}