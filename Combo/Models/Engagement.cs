using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Combo.Models
{
    public class Engagement
    {
        public BattleSide _attacker { get; private set; }
        public BattleSide _defender { get; private set; }

        public Engagement(Army attacker, Army defender, Dice dice)
        {
            _attacker = new BattleSide(attacker, dice);
            _defender = new BattleSide(defender, dice);
        }

        public bool Battle()
        {
            while(_attacker._army.Morale > 0 && _defender._army.Morale > 0)
            {
                Fight();
            }

            return _attacker._army.Morale > 0;
        }

        public bool Fight()
        {
            int attackerSum = 0;
            int defenderSum = 0;
            while(attackerSum == defenderSum)
            {
                attackerSum = _attacker.Battle(_defender._army);
                defenderSum = _defender.Battle(_attacker._army);
            }
            
            if (attackerSum > defenderSum)
            {
                _defender._army.Morale--;
                return true;
            } else
            {
                _attacker._army.Morale--;
                return false;
            }
        }
    }

    public class BattleSide
    {
        public Army _army { get; set; }
        public List<int> _rolls { get; set; }
        public Dice _dice;

        public BattleSide(Army army, Dice dice)
        {
            _army = army;
            _dice = dice;
        }

        public int Battle(Army enemy)
        {
            Roll(_dice);

            // Account for Mage(s)
            MageAffect();

            var sum = _rolls.Sum();

            // Account for Archer Bonus (Bowman > Spearman)
            sum = sum + _army.UnitTypeCount[Army.UnitType.Bowman] * enemy.UnitTypeCount[Army.UnitType.Spearman];

            // Account for Cavalry Bonus (Cavalry > Bowman)
            sum = sum + _army.UnitTypeCount[Army.UnitType.Cavalry] * enemy.UnitTypeCount[Army.UnitType.Bowman];

            // Account for Levy Bonus (Spearman > Cavalry)
            sum = sum + _army.UnitTypeCount[Army.UnitType.Spearman] * enemy.UnitTypeCount[Army.UnitType.Cavalry];

            // Account for Wizard Bonus (Wizard > Mage & Wizard > Priest)
            sum = sum + _army.UnitTypeCount[Army.UnitType.Wizard] * enemy.UnitTypeCount[Army.UnitType.Mage];
            sum = sum + _army.UnitTypeCount[Army.UnitType.Wizard] * enemy.UnitTypeCount[Army.UnitType.Priest];

            // Account for Armor Bonus
            sum = sum + _army.UnitTypeCount[Army.UnitType.Armor];

            return sum;
        }

        private void Roll(Dice dice)
        {
            _rolls = new List<int>();
            for (int i = 0; i < _army.UnitCount; i++)
            {
                _rolls.Add(dice.Roll());
            }
        }

        private (int val, int index) GetMin()
        {
            var min = _rolls.Min();
            return (min, _rolls.IndexOf(min));
        }

        private void MageAffect()
        {
            var mageCount = _army.UnitTypeCount[Army.UnitType.Mage];
            while (mageCount > 0)
            {
                (int val, int index) = GetMin();
                if (val <= Math.Ceiling((double)_dice.Sides / 2))
                {
                    ReRoll(index);
                }
                mageCount--;
            }
        }

        private void ReRoll(int index)
        {
            _rolls[index] = _dice.Roll();
        }
    }
}
