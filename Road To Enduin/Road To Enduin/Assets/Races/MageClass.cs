using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageClass : BaseClass
{
   public void SetMageAttributes()
    {
        CharacterRace = Race.Races.Mage;
        HealthPoints = 175;
        SecondaryPoints = 150;
        AgilityMultiplier = 4;
        StrengthMultiplier = 8;
        AbilityPower = 30;
    }
}
