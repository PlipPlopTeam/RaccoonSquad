using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster
{
    List<GameObject> gatheredObjects = new List<GameObject>();
    int humanScore = 0;
    int maximumScore = 0;
    int currentScore = 0;

    int spawnPlayerCount = 1;

    public event System.Action<Vector3> soundAt;

    public LevelMaster()
    {
        SoundPlayer.StopEverySound();
        SoundPlayer.Play("amb_suburbs_cars", 0.8f);
        SoundPlayer.Play("mus_funkstyle_04", 0.1f);

        foreach(var prop in Object.FindObjectsOfType<Grabbable>()) {
            maximumScore += prop.racoonValue;
            prop.GetProp().onHit += (x) => {
                SoundPlayer.PlayWithRandomPitch("fb_raccoon_bumping", 0.1f);
                GameObject.Instantiate(Library.instance.hitFX, x.GetContact(0).point, Library.instance.hitFX.transform.rotation);
                soundAt.Invoke(x.GetContact(0).point);
            };
        }        
    }

    public void Score(Grabbable prop)
    {
        humanScore += prop.humanValue;
        gatheredObjects.Add(prop.gameObject);
        currentScore += prop.racoonValue;

        if(currentScore % (spawnPlayerCount * 10) == 0) 
        {
            spawnPlayerCount++;
            GameManager.instance.SpawnHuman();
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public int GetDollars()
    {
        return humanScore;
    }


    public List<GameObject> GetGatheredObjects()
    {
        return new List<GameObject>(gatheredObjects.ToArray());
    }

    public int GetBronzeTier()
    {
        return Mathf.FloorToInt(maximumScore / 3f);
    }

    public int GetSilverTier()
    {
        return Mathf.FloorToInt((maximumScore *2f) / 3f);
    }

    public int GetGoldTier()
    {
        return maximumScore;
    }

    public int GetCurrentTier()
    {
        if(currentScore < GetBronzeTier()) return 0;
        if(currentScore < GetSilverTier()) return 1;
        if(currentScore < GetGoldTier()) return 2;
        return 3;
    }
}
