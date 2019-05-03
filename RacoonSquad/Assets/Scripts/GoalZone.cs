using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public List<PlayerController> racoonsInside = new List<PlayerController>();

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
            racoonsInside.RemoveAll(o=>o==pc);
            racoonsInside.Add(pc);

            if (racoonsInside.Count == GameManager.instance.GetPlayers().Count) {
                CheckWin();
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

    private void OnTriggerExit(Collider other)
    {
        var pc = other.GetComponent<PlayerController>();

        if (pc != null) {
            racoonsInside.RemoveAll(o => o == pc);
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
            GameManager.instance.level.Score(prop);
        }

        Destroy(prop.gameObject);
    }

    void CheckWin()
    {
        if (GameManager.instance.level.GetScore() >= GameManager.instance.level.GetBronzeTier()) {
            GameManager.instance.Win();
        }
    }
}
