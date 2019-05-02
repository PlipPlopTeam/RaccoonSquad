using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedModifier
{
    public float value;
    public int ticket;
}

public class MovementSpeed : MonoBehaviour
{
    List<SpeedModifier> speedModifiers = new List<SpeedModifier>();

    public float GetMultiplier()
    {
        float multiplier = 1f;
        foreach(SpeedModifier modifier in speedModifiers) multiplier *= modifier.value;
        return multiplier;
    }
    public int AddSpeedModifier(float value, int ticket)
    {
        if(value < 0) value = 0f;

        SpeedModifier nsm = new SpeedModifier();
        nsm.value = value;
        nsm.ticket = ticket;
        speedModifiers.Add(nsm);
        return nsm.ticket;
    }
    public void RemoveSpeedModifier(int ticket)
    {
        SpeedModifier rsm = FindSpeedModifier(ticket);
        if(rsm != null && speedModifiers.Contains(rsm)) 
        {
            speedModifiers.Remove(rsm);
        }
    }
    SpeedModifier FindSpeedModifier(int ticket)
    {
        foreach(SpeedModifier modifier in speedModifiers)
        {
            if(modifier.ticket == ticket) return modifier;
        }
        return null;
    }
}
