using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var pc = other.GetComponent<PlayerController>();

        if (pc != null) {
            // A player
            if (pc.IsHolding()) {
                var prop = pc.GetHeldObject();
                pc.DropHeldObject();
                Absorb(prop);

            }
        }
        else {
            // A prop
            var prop = other.GetComponent<Grabbable>();
            if (prop && !prop.IsHeld()) {
                Absorb(prop);
            }
        }
    }

    void Absorb(Grabbable prop)
    {
        SoundPlayer.PlayWithRandomPitch("fb_scoring_loot", 0.3f);

        if (GameManager.instance.lobby) {
            if (prop.tag == "GameStarter") {
                GameManager.instance.NextLevel();
            }
        }
        else {
            GameManager.instance.level.currentScore += prop.racoonValue;
        }

        Destroy(prop.gameObject);
    }
}
