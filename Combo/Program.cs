using Combo.Models;
using System;
using System.Collections.Generic;

namespace Combo
{
    class Program
    {
        static Dice _dice;
        static readonly int simCount = 1000;

        static void Main(string[] args)
        {
            var set = new List<string> { "L", "A", "C", "M", "S", "P" };
            var results = new List<List<double>>();
            var combinations = GenerateCombinations(set, 4);

            if (args.Length > 0)
            {
                _dice = new Dice(new Random(), Int32.Parse(args[0]));
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter($"./Engagements/{_dice.Sides}d.txt"))
            {
                file.Write($"{_dice.Sides}d\t");
                foreach (var combination in combinations)
                {
                    string combinationStr = string.Join("", combination);
                    file.Write($"{combinationStr}\t");
                }

                file.WriteLine();

                var index = 0;
                foreach (var combination in combinations)
                {
                    results.Add(new List<double>());
                    string combinationStr = string.Join("", combination);
                    file.Write($"{combinationStr}\t");
                    var _index = 0;
                    foreach (var _combination in combinations)
                    {
                        if (_index >= index)
                        {
                            results[index].Add(Simulate(combination, _combination));
                            
                        } else
                        {
                            results[index].Add(100-results[_index][index]);
                        }
                        file.Write(String.Format("{0,5:##0.0}\t", results[index][_index]));
                        _index++;
                    }
                    file.WriteLine();
                    index++;
                }
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static List<List<T>> GenerateCombinations<T>(List<T> combinationList, int k)
        {
            var combinations = new List<List<T>>();

            if (k == 0)
            {
                var emptyCombination = new List<T>();
                combinations.Add(emptyCombination);

                return combinations;
            }

            if (combinationList.Count == 0)
            {
                return combinations;
            }

            T head = combinationList[0];
            var copiedCombinationList = new List<T>(combinationList);

            List<List<T>> subcombinations = GenerateCombinations(copiedCombinationList, k - 1);

            foreach (var subcombination in subcombinations)
            {
                subcombination.Insert(0, head);
                combinations.Add(subcombination);
            }

            combinationList.RemoveAt(0);
            combinations.AddRange(GenerateCombinations(combinationList, k));

            return combinations;
        }

        private static double Simulate(List<string> armyOne, List<string> armyTwo)
        {
            var wins = 0;

            for (int i = 0; i < simCount; i++)
            {
                Engagement engagement = new Engagement(new Army(armyOne), new Army(armyTwo), _dice);
                if (engagement.Battle())
                {
                    wins++;
                }
            }
            //Console.WriteLine($" wins: {wins}");
            return (double) wins/simCount*100;
        }
    }
}
