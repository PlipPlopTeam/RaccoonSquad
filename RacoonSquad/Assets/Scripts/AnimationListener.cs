using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListener : MonoBehaviour
{
    public void Step()
    {
        SoundPlayer.Play("si_step",.3f,Random.Range(.9f,1.1f));
    }
}
