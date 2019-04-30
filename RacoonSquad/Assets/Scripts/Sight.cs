using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public float headHeight = 0f;
    public float fieldOfViewAngle = 110f;
    public float range = 5f;

    public GameObject[] Scan()
    {
        List<GameObject> objects = new List<GameObject>();

        Vector3 headPosition = transform.position + new Vector3(0f, headHeight, 0f);

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(headPosition, range, transform.forward, range);

        for(int i = 0; i < hits.Length; i++)
        {
            Vector3 direction = hits[i].transform.position - headPosition;
            float angle = Vector3.Angle(direction, transform.forward);

            if(angle < fieldOfViewAngle * 0.5f)
            {
                objects.Add(hits[i].transform.gameObject);
            }
        }

        return objects.ToArray();
    }
}
