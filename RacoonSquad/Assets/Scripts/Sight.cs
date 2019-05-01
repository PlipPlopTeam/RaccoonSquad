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

        // Add object in range of being seen
        RaycastHit[] sphereHits;
        sphereHits = Physics.SphereCastAll(headPosition, range, transform.forward, range);

        for(int i = 0; i < sphereHits.Length; i++)
        {

            // Checks if the object is in front of the sight
            Vector3 direction = sphereHits[i].transform.position - headPosition;
            float angle = Vector3.Angle(direction, transform.forward);
            if(angle < fieldOfViewAngle * 0.5f)
            {

                // Check if an object is hiding the seen object from the sight
                RaycastHit[] frontsphereHits;
                frontsphereHits = Physics.RaycastAll(headPosition, direction, range, 5, QueryTriggerInteraction.Ignore);
                Debug.DrawRay(headPosition, direction);

                bool seen = true;
                foreach(RaycastHit hit in frontsphereHits)
                {
                    if(hit.transform != transform && hit.transform != sphereHits[i].transform) seen = false;
                }
                if(seen) objects.Add(sphereHits[i].transform.gameObject);
            }
        }
        return objects.ToArray();
    }
}
