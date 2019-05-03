using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var prop = other.GetComponent<Grabbable>();
        if (prop && !prop.IsHeld()) {
            Absorb(prop);
            SoundPlayer.PlayWithRandomPitch("fb_scoring_loot", 1f);
        }
    }

    void Absorb(Grabbable prop)
    {
        if (GameManager.instance.lobby) {
        }
        else {
            GameManager.instance.level.currentScore += prop.racoonValue;
            Destroy(prop.gameObject);
        }

    }
}
