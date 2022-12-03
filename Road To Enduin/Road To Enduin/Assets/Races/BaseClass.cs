using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass 
{
    
    [SerializeField] int healthPoints;
    [SerializeField] int secondaryPoints;
    [SerializeField] int agilityMultiplier;
    [SerializeField] int strengthMultiplier;
    [SerializeField] int abilityPower;

    Race.Races characterRace;
    string characterClassDescription;

    public Race.Races CharacterRace { 
        get { return characterRace; } 
        set { characterRace = value; } 
    }

    public string CharacterClassDescription
    {
        get { return characterClassDescription; }
        set { characterClassDescription = value; }
    }

    public int HealthPoints
    {
        get { return healthPoints; }
        set { healthPoints = value; }
    }

    public int SecondaryPoints
    {
        get { return secondaryPoints; }
        set { secondaryPoints = value; }
    }

    public int AgilityMultiplier
    {
        get { return agilityMultiplier; }
        set { agilityMultiplier = value; }
    }

    public int StrengthMultiplier
    {
        get { return strengthMultiplier; }
        set { strengthMultiplier = value; }
    }

    public int AbilityPower
    {
        get { return abilityPower; }
        set { abilityPower = value; }
    }

}
