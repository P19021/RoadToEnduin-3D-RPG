using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfClass : BaseClass
{
   public void SetElfAttributes()
    {
        CharacterRace = Race.Races.Elf;
        HealthPoints = 135;
        SecondaryPoints = 80;
        AgilityMultiplier = 7;
        StrengthMultiplier = 8;
        AbilityPower = 20;
    }
}
