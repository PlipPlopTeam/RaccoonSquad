using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var prop = other.GetComponent<Grabbable>();
        if (prop && !prop.IsHeld()) {
            Destroy(prop.gameObject);
            GameManager.instance.level.currentScore += prop.racoonValue;
        }
    }
}
