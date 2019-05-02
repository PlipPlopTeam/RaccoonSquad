using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster
{
    public int currentScore = 0;
    int maximumScore = 0;

    public LevelMaster()
    {
        foreach(var prop in Object.FindObjectsOfType<Grabbable>()) {
            maximumScore += prop.racoonValue;
            prop.GetProp().onHit += (x) => {
                // Play Sound
                GameObject.Instantiate(Library.instance.hitFX, x.GetContact(0).point, Library.instance.hitFX.transform.rotation);
            };
        }        
    }

    public int GetBronzeTier()
    {
        return Mathf.FloorToInt(maximumScore / 2f);
    }

    public int GetSilverTier()
    {
        return Mathf.FloorToInt((maximumScore *2f) / 3f);
    }

    public int GetGoldTier()
    {
        return maximumScore;
    }

}
