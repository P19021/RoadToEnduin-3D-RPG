using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManClass : BaseClass
{
    public void SetManAttributes()
    {
        CharacterRace = Race.Races.Man;
        HealthPoints = 200;
        SecondaryPoints = 100;
        AgilityMultiplier = 5;
        StrengthMultiplier = 10;
        AbilityPower = 15;
    }
}   
