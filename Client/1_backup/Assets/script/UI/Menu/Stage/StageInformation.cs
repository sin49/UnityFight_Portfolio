using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class StageInformation
{
    public string ID;
    public string name;
    public int waveNumber;
    public List<CharacterData> SpawnCharacterList;
    public List<spawnInformation> spawn;
    public int StaminaNeed;
    public List<StageReward> rewardlist;
    public List< StageMission> missionarray = new List<StageMission>();
    public String TerrainDataName;

}
[Serializable]
public class spawnInformation
{
   
     public spawnInformation(int n)
    {
        index = n;
        number = 1;
        round = 1;
    }
    public int index;
    public int number;
    public int round;
    
}
[Serializable]
public class StageReward
{
    public item item;
    [Range(0,1)]
    public float per;
}