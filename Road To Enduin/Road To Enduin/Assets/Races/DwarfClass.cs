using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfClass : BaseClass
{
    public void SetDwarfAttributes()
    {
        CharacterRace = Race.Races.Dwarf;
        HealthPoints = 350;
        SecondaryPoints = 80;
        AgilityMultiplier = 3;
        StrengthMultiplier = 7;
        AbilityPower = 15;
    }
}
