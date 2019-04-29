using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    // Referencies
    GamepadManager manager;
    [HideInInspector] public x360_Gamepad gamepad;

    void Awake()
    {
        manager = GamepadManager.Instance;
    }

    public void LoadGamepad(int index)
    {
        gamepad = manager.GetGamepad(index+1);
    }

/*
    void Update()
    {
        if(gamepad.GetButtonDown("A"))
        {
            
        }
    }
*/
}
