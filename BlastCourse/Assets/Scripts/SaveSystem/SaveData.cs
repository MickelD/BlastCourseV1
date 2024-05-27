using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    /*
     * VARIABLES TO SAVE
     * -SpawnPosition
     * -Collectibles Aquired
     * -Races Completed
     * -Scene the player is in
     * 
     * VARIABLES NOT TO SAVE
     * -Health => Reset on Load
     * -Energy => Reset on Load
     * -Position => Set to spawnpoint
     * -Rotation => Reset on Load
     * -Which Rocket are they using => Reset on Load
     */

    public int _scene;
    public float[] _spawnPosition;
    public List<string> _collectiblesAquired;
    public List<string> _keyObjects;
    public bool[] _rpgs;
    //Strings that hold a pointer to the reference

    public SaveData(int scn, float[] spP, List<string> clAq, List<string> kObj, bool[] rpg)
    {
        _scene = scn;

        _spawnPosition = new float[3];
        _spawnPosition[0] = spP[0];
        _spawnPosition[1] = spP[1];
        _spawnPosition[2] = spP[2];

        _collectiblesAquired = new List<string>();
        if(clAq.Count > 0) for (int i = 0; i < clAq.Count; i++) _collectiblesAquired.Add(clAq[i]);

        _keyObjects = new List<string>();
        if (kObj.Count > 0) for (int i = 0; i < kObj.Count; i++) _keyObjects.Add(kObj[i]);

        _rpgs = new bool[4];
        if (rpg.Length >= 4)
        {
            _rpgs[0] = rpg[0];
            _rpgs[1] = rpg[1];
            _rpgs[2] = rpg[2];
            _rpgs[3] = rpg[3];
        }
        
    }
}
