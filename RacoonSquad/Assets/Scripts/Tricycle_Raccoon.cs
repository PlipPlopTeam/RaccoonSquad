using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tricycle_Raccoon : MonoBehaviour
{

    public Animator anim;

    public float maxTimer;
    
    void Start()
    {
        Invoke("Check", Random.Range(maxTimer/2, maxTimer));
    }

    void Check()
    {
        anim.SetTrigger("Check");
        
        Invoke("Check", Random.Range(maxTimer/2, maxTimer));

    }

}
