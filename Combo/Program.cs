using System;
using System.Collections.Generic;
using System.Linq;

namespace Combo
{
    class Program
    {
        static readonly Random rand = new Random();
        static int sides = 6;
        static readonly int simCount = 1000;

        static void Main(string[] args)
        {
            var set = new List<string> { "L", "A", "C", "M", "S"};
            var results = new List<List<double>>();
            var combinations = GenerateCombinations(set, 4);

            if (args.Length > 0)
            {
                sides = Int32.Parse(args[0]);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter($"./sim_{sides}d.txt"))
            {
                file.Write($"{sides}d\t");
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
                            results[index].Add(results[_index][index]);
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
            //Console.Write($"{string.Join("", armyOne)} fighting {string.Join("", armyTwo)}");
            var wins = 0;

            for (int i = 0; i < simCount; i++)
            {
                var tie = true;
                var victory = false;
                while(tie)
                {
                    (victory, tie) = Battle(armyOne, armyTwo);
                    if (victory && !tie)
                    {
                        wins++;
                    }
                }
            }
            //Console.WriteLine($" wins: {wins}");
            return (double) wins/simCount*100;
        }

        private static (bool, bool) Battle(List<string> armyOne, List<string> armyTwo)
        {
            var rolls1 = Fight(armyOne);
            var rolls2 = Fight(armyTwo);

            // Account for Mage(s)
            rolls1 = MageAffect(armyOne, rolls1);
            rolls2 = MageAffect(armyTwo, rolls2);

            var sum1 = rolls1.Sum();
            var sum2 = rolls2.Sum();

            // Account for Archer Bonus (Archer > Levy)
            sum1 = sum1 + CountItemsInList("A", armyOne) * CountItemsInList("L", armyTwo);
            sum2 = sum2 + CountItemsInList("A", armyTwo) * CountItemsInList("L", armyOne);

            // Account for Cavalry Bonus (Cavalry > Archer)
            sum1 = sum1 + CountItemsInList("C", armyOne) * CountItemsInList("A", armyTwo);
            sum2 = sum2 + CountItemsInList("C", armyTwo) * CountItemsInList("A", armyOne);

            // Account for Levy Bonus (Levy > Cavalry)
            sum1 = sum1 + CountItemsInList("L", armyOne) * CountItemsInList("C", armyTwo);
            sum2 = sum2 + CountItemsInList("L", armyTwo) * CountItemsInList("C", armyOne);

            // Account for Swordsman Bonus
            sum1 = sum1 + CountItemsInList("S", armyOne);
            sum2 = sum2 + CountItemsInList("S", armyTwo);

            return (sum1 > sum2, sum1 == sum2);
        }

        private static List<int> MageAffect(List<string> army, List<int> rolls)
        {
            var mageCount = CountItemsInList("M", army);
            var result = new List<int>(rolls);
            while(mageCount > 0)
            {
                result = MageReroll(result);
                mageCount--;
            }
            return result;
        }

        private static List<int> MageReroll(List<int> rolls)
        {
            var result = new List<int>(rolls);
            (int val, int index) = FindLowest(rolls);

            if (val < Math.Ceiling((double)sides/2))
            {
                result[index] = rand.Next(1, sides+1);
            }
            
            return result;
        }

        private static List<int> Fight<T>(List<T> army)
        {
            var rolls = new List<int>();

            foreach (var _soldier in army)
            {
                rolls.Add(rand.Next(1, sides+1));
            }

            return rolls;
        }

        private static int CountItemsInList(string item, List<string> items)
        {
            var result = 0;
            foreach (var _item in items)
            {
                if (item == _item)
                {
                    result++;
                }
            }
            return result;
        }

        private static (T, int) FindLowest<T>(List<T> items) where T : IComparable<T>
        {
            var min = items.Min();
            return (min, items.IndexOf(min));
        }

        private (T, int) FindMax<T>(List<T> items) where T : IComparable<T>
        {
            var max = items.Max();
            return (max, items.IndexOf(max));
        }
    }
}
