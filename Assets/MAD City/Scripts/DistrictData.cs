using Roadmans_Fortnite.Scripts.Classes.Stats.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DistrictData", menuName = "CityGen/District", order = 4)]
public class DistrictData : ScriptableObject
{
    public enum Religion
    {
        CHRISTIAN,
        ISLAM,
        HINDU,
        SIKH,
        BUDDHIST,
        NONE,
        OTHER,

        RELIGION_NUM
    }

    public Religion religion;
    public int volume;

    
    

}