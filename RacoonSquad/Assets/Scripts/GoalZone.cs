using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var prop = other.GetComponent<Grabbable>();
        if (prop && !prop.IsHeld()) {
            GameManager.instance.level.currentScore += prop.racoonValue;
            print(GameManager.instance.level.currentScore);
            Destroy(prop.gameObject);
        }
    }
}
