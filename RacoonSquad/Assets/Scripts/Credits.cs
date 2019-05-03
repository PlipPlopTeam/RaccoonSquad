using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{

    public Animator anim;

    public bool creditsOn = false;

    private void Start()
    {
        //Invoke("ToggleCredits", 3);
    }

    void Update()
    {
        if (Input.GetButtonDown("Credits"))
        {
            //CancelInvoke("ToggleCredits");
            ToggleCredits();
        }
    }

    void ToggleCredits()
    {
        creditsOn = !creditsOn;
        anim.SetBool("In", creditsOn);
    }
}
