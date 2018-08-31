using System.Collections.Generic;

namespace Combo.Models
{
    public class Army
    {
        public int Morale { get; set; } = 3;
        public int Actions { get; set; } = 5;
        public enum UnitType {
            Armor,
            Bowman,
            Cavalry,
            Priest,
            Spearman,
            Mage,
            Wizard,
            Healer,
        }
        public Dictionary<UnitType, int> UnitTypeCount { get; set; } = new Dictionary<UnitType, int>()
        {
            { UnitType.Armor, 0 },
            { UnitType.Bowman, 0 },
            { UnitType.Cavalry, 0 },
            { UnitType.Healer, 0 },
            { UnitType.Priest, 0 },
            { UnitType.Spearman, 0 },
            { UnitType.Mage, 0 },
            { UnitType.Wizard, 0 },
        };
        public List<UnitType> Units { get; set; } = new List<UnitType>();
        public string UnitString { get; set; }
        public int UnitCount { get; set; }

        public Army(List<string> stack)
        {
            foreach(string unit in stack)
            {
                switch(unit)
                {
                    case "A":
                        UnitTypeCount[UnitType.Armor]++;
                        Units.Add(UnitType.Armor);
                        break;
                    case "B":
                        UnitTypeCount[UnitType.Bowman]++;
                        Units.Add(UnitType.Bowman);
                        break;
                    case "C":
                        UnitTypeCount[UnitType.Cavalry]++;
                        Actions++;
                        Units.Add(UnitType.Cavalry);
                        break;
                    case "H":
                        UnitTypeCount[UnitType.Healer]++;
                        Units.Add(UnitType.Healer);
                        break;
                    case "M":
                        UnitTypeCount[UnitType.Mage]++;
                        Units.Add(UnitType.Mage);
                        break;
                    case "P":
                        UnitTypeCount[UnitType.Priest]++;
                        //Morale++;
                        Units.Add(UnitType.Priest);
                        break;
                    case "S":
                        UnitTypeCount[UnitType.Spearman]++;
                        Units.Add(UnitType.Spearman);
                        break;
                    case "W":
                        UnitTypeCount[UnitType.Wizard]++;
                        Units.Add(UnitType.Wizard);
                        break;
                    default:
                        break;
                }
                UnitCount++;
                UnitString = UnitString + unit;
            }
        }
    }
}
