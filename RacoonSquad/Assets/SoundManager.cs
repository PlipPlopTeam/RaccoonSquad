using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [HideInInspector] public List<Hearing> ears = new List<Hearing>();

    void Awake()
    {
        instance = this;
    }

    public void HearingCall(Vector3 position)
    {
        foreach(Hearing h in ears) h.TryHeard(position);
    }
}
