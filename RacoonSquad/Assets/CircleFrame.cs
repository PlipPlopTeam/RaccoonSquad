using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrameAnimation
{
    public string name;
    public AnimationCurve curve;
    public float duration;
}

public class CircleFrame : MonoBehaviour
{
    [Header("Referencies")]
    public GameObject mask;
    [Header("Settings")]
    public float distanceToCamera = 10f;
    [Header("Animations")]
    public FrameAnimation[] animations;
    

    FrameAnimation currentAnimation;
    Vector3 initialMaskSize;
    float timer = 0f;

    void Awake()
    {
        initialMaskSize = mask.transform.localScale;
    }

    public void Update()
    {
        // Facing Camera
        if(Camera.main != null)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * distanceToCamera;
            transform.forward = -(Camera.main.transform.position - transform.position);
        }

        if(currentAnimation != null)
        {
            if(timer <= currentAnimation.duration)
            {
                timer += Time.deltaTime;
                mask.transform.localScale = initialMaskSize * currentAnimation.curve.Evaluate(timer/currentAnimation.duration);
            }
            else
            {
                if(currentAnimation.name == "GameOver") GameManager.instance.GoToLobby();
                else if(currentAnimation.name == "Win") GameManager.instance.GoToWinScene();
                currentAnimation = null;
            }
        }
    }

    public void PlayFrameAnimation(string name)
    {
        timer = 0;
        currentAnimation = GetFrameAnimation(name);
    }
    public void PlayFrameAnimation(FrameAnimation FrameAnimation)
    {
        timer = 0;
        currentAnimation = FrameAnimation;
    }

    FrameAnimation GetFrameAnimation(string name)
    {
        foreach(FrameAnimation fa in animations)
        {
            if(fa.name == name) return fa;
        }
        return null;
    }
}
