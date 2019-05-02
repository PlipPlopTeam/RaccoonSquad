using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slower : MonoBehaviour
{
    [Range(0f, 1f)]
    public float slow = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.gameObject.GetComponent<PlayerController>();
        if(pc != null) pc.AddSpeedModifier(slow, this.GetInstanceID());
        else
        {
            HumanBehavior hb = other.gameObject.GetComponent<HumanBehavior>();
            if(hb != null) hb.AddSpeedModifier(slow, this.GetInstanceID());
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerController pc = other.gameObject.GetComponent<PlayerController>();
        if(pc != null) pc.RemoveSpeedModifier(this.GetInstanceID());
        else
        {
            HumanBehavior hb = other.gameObject.GetComponent<HumanBehavior>();
            if(hb != null) hb.RemoveSpeedModifier(this.GetInstanceID());
        }
    }
}
