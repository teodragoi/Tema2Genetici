using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema2
{

	static class Globals
	{
		public const int CHR_LENGTH = 8;
		public const double MUT_PROB = 0.3;
		public const double CROSS_PROB = 0.5;
		public static Random rnd = new Random();
	}

	class BinNum
	{

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
		public void setDim(int dim)
		{
			numLength = dim;
		}
		public int getDim()
		{
			return numLength;
		}
		public void Reset()
		{
			while(num.Any())
				num.RemoveAt(num.Count() - 1);
		}
		public void RandomSet()
		{
			for (int i = 0; i < Globals.CHR_LENGTH * numLength; i++)
			{
				if (Globals.rnd.NextDouble() < 0.5)
				{
					num.Add(0);
				}
				else
				{
					num.Add(1);
				}
			}
		}
		public void Display()
		{
			for (int i = 0; i < numLength * Globals.CHR_LENGTH; i++)
			{
				Console.Write(num[i].ToString());
				if ((i + 1) % Globals.CHR_LENGTH == 0)
					Console.Write(" ");
			}
			Console.Write("\n");
		}
		public List<double> GetNum(double min, double max)
		{
			List<double> Values = new List<double>();


			for (int i = numLength * Globals.CHR_LENGTH - 1; i >= 0; i -= Globals.CHR_LENGTH)
			{
				double number = 0;
				double pow = 1;
				for (int j = i; j >= i - Globals.CHR_LENGTH + 1; j--)
				{
					number += num[j] * pow;
					pow *= 2;
				}
				Values.Add(min + number * (max - min) / (Math.Pow(2, Globals.CHR_LENGTH) - 1));
			}

			return Values;
		}
		public void ShiftBit(int index)
		{
			num[index] = (num[index] + 1) % 2;
		}
		public int GetBit(int index)
		{
			return num[index];
		}
	}

	static class Functions
	{
		public static List<double> Rastrigin(List<BinNum> pop)
		{
			List<double> Values = new List<double>();

			for (int i = 0; i < pop.Count(); i++)
			{
				double sum = 10 * pop[i].getDim();
				double min = -5.12;
				double max = 5.12;

				List<double> tempRes = new List<double>(pop[i].GetNum(min, max));

				for (int j = 0; j < pop[i].getDim(); j++)
					sum += tempRes[j] * tempRes[j] - 10 * Math.Cos(2 * Math.PI * tempRes[j]);

				//10 * n + sum(x(i) ^ 2 - 10·cos(2·pi·x(i)))

				Values.Add(sum);
			}
			return Values;
		}
	}

	static class popOp
	{
		public static void Display(List<BinNum> pop)
		{
			for (int i = 0; i < pop.Count(); i++)
				pop[i].Display();
		}

		public static void InitPop(int dim, List<BinNum>pop)
		{
			BinNum chromozome = new BinNum();
			chromozome.setDim(dim);
			for(int i = 0; i < 100; i++)
			{
				chromozome.Reset();
				chromozome.RandomSet();
				pop.Add(new BinNum(chromozome));
			}
		}

		public static void Mutation(List<BinNum> pop)
		{
			for (int i = 0; i < pop.Count(); i++)
			{
				for (int j = 0; j < Globals.CHR_LENGTH * pop[i].getDim(); j++)
				{
					if (Globals.rnd.NextDouble() < Globals.MUT_PROB)
						pop[i].ShiftBit(j);
				}
			}
		}

		public static void Crossover(List<BinNum> pop)
		{
			int cut = 0;

			for(int i = 1; i < pop.Count(); i += 2)
			{
				for(int k = 0; k < pop[i].getDim(); k++)
					if(Globals.rnd.NextDouble() < Globals.CROSS_PROB)
					{
						cut = Globals.rnd.Next(k, Globals.CHR_LENGTH * (k + 1) - 1);
						for(int j = cut; j < Globals.CHR_LENGTH * k; j++)
						{
							if( pop[i].GetBit(j)  != pop[i - 1].GetBit(j) )
							{
								pop[i].ShiftBit(j);
								pop[i - 1].ShiftBit(j);
							}
						}
					}
			}
		}

		public static List<double> Evaluate(List<BinNum> pop, List<double> funcVal ,double Min, double Max)
		{
			double max = Double.MinValue;
			double min = Double.MaxValue;
			double suma = 0;

			List<double> Fitness = new List<double>();

			for(int i = 0; i < pop.Count(); i++)
			{
				if (funcVal[i] > max)
					max = funcVal[i];
				if (funcVal[i] < min)
					min = funcVal[i];
			}

			for (int i = 0; i < pop.Count; i++)
			{
				suma += max + 2 - funcVal[i];
				Fitness.Add(max + 2 - funcVal[i]);
			}

			Fitness.Add(suma);
			Fitness.Add(min);

			return Fitness;
		}

		public static List<BinNum> Selection(List<BinNum> pop, List<double> Fitness)
		{
			List<BinNum> newPop = new List<BinNum>();
			List<double> prob = new List<double>();

			double max = Fitness[Fitness.Count() - 2];
			double candidate = 0;

			int j = 0;

			prob.Add(Fitness[0] / max);

			for (int i = 1; i < Fitness.Count() - 3; i++)
				prob.Add(prob[i - 1] + Fitness[i] / max);

			prob.Add(1);

			for(int i = 0; i < pop.Count(); i++)
			{
				j = 0;
				candidate = Globals.rnd.NextDouble();

				while(prob[j] < candidate)
				{
					j++;
				}

				newPop.Add(pop[j]);
			}

			return newPop;
		}

		public static List<double> genAlg(string function, double min, double max, int dim)
		{
			List<BinNum> newPop = new List<BinNum>();
			List<BinNum> pop = new List<BinNum>();
			InitPop(dim, pop);

			List<double> fit = new List<double>();

			double avg = 0;
			double Min = Double.MaxValue;
			double Max = Double.MinValue;

			for(int t = 0; t < 1000; t++)
			{
				Mutation(pop);
				Crossover(pop);
				fit = Evaluate(pop, Functions.Rastrigin(pop), min, max);
				newPop = Selection(pop, fit);
				pop = newPop;
				if (Min > fit[fit.Count() - 1])
					Min = fit[fit.Count() - 1];
				if (Max < fit[fit.Count() - 1])
					Max = fit[fit.Count() - 1];
				avg += fit[fit.Count() - 1];
			}

			List<double> res = new List<double>();

			res.Add(Min);
			res.Add(Max);
			res.Add(avg / 1000);

			return res;

		}
	}

	class Program
	{
		
		static void Main(string[] args)
		{
			BinNum chromozome = new BinNum();
			List<BinNum> Population = new List<BinNum>();

			//5 dimensiuni
			//popOp.Display(Population);
			List<double> tempRes;
			double min = Double.MaxValue, max = Double.MinValue, avg = 0;
			for (int i = 0; i < 30; i++)
			{
				tempRes = new List<double>(popOp.genAlg("Rastrigin", -5.12, 5.12, 5));
				if (tempRes[0] < min)
					min = tempRes[0];
				if (tempRes[1] > max)
					max = tempRes[1];
				avg += tempRes[2];
			}
			Console.WriteLine(min.ToString());
			Console.WriteLine(max.ToString());
			Console.WriteLine((avg / 30).ToString());

			Console.Write("\n");
			//Console.WriteLine(Functions.Rastrigin(Population).ToString());
		}
	}
}