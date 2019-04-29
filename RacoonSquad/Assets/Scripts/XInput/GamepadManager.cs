using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadManager : MonoBehaviour {
    public int GamepadCount = 4; // Number of gamepads to support
    public int actif;
    private List<x360_Gamepad> gamepads;     // Holds gamepad instances
    private static GamepadManager singleton; // Singleton instance

    // Initialize on 'Awake'
    void Awake()
    {
        // Found a duplicate instance of this class, destroy it!
        if (singleton != null && singleton != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            // Create singleton instance
            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            // Lock GamepadCount to supported range
            GamepadCount = Mathf.Clamp(GamepadCount, 1, 4);

            gamepads = new List<x360_Gamepad>();

			// Create specified number of gamepad instances
			for ( int i = 0; i < GamepadCount; ++i )
			{
				gamepads.Add(new x360_Gamepad(i + 1));
			}	
        }
    }

    // Return instance
    public static GamepadManager Instance
    {
        get
        {
            if (singleton == null)
            {
                Debug.LogError("[GamepadManager]: Instance does not exist!");
                return null;
            }

            return singleton;
        }
    }
    // Normal unity update
    void Update()
    {
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
		    Application.Quit();
	    }
	    
        for (int i = 0; i < gamepads.Count; ++i)
            gamepads[i].Update();
    }
    // Refresh gamepad states for next update
    public void Refresh()
    {
        for (int i = 0; i < gamepads.Count; ++i)
            gamepads[i].Refresh();
    }
    // Return specified gamepad
    // (Pass index of desired gamepad, eg. 1)
    public x360_Gamepad GetGamepad(int index)
    {


		for (int i = 0; i < gamepads.Count;)
        {
			// Indexes match, return this gamepad
			if ( gamepads [i].Index == ( index - 1 ) )
			{
				return gamepads [i];
			}
			else
				++i;
        }

        Debug.LogError("[GamepadManager]: " + index + " is not a valid gamepad index!");

        return null;
    }
    // Return number of connected gamepads
    public int ConnectedTotal()
    {
        int total = 0;

        for (int i = 0; i < gamepads.Count; ++i)
        {
			if ( gamepads [i].IsConnected )
			{
				total++;
			}
        }
		
        return total;
    }
    // Check across all gamepads for button press.
    // Return true if the conditions are met by any gamepad
    public bool GetButtonAny(string button)
    {
        #pragma warning disable
        for (int i = 0; i < gamepads.Count; ++i)
        {
            // Gamepad meets both conditions
            if (gamepads[i].IsConnected && gamepads[i].GetButton(button))
                actif = i;
            //Debug.Log("numero" + i);
                return true;
        }

        return false;
    }
    // Check across all gamepads for button press - CURRENT frame.
    // Return true if the conditions are met by any gamepad
    public bool GetButtonDownAny(string button)
    {
        for (int i = 0; i < gamepads.Count; ++i)
        {
            // Gamepad meets both conditions
            if (gamepads[i].IsConnected && gamepads[i].GetButtonDown(button))
                return true;
        }

        return false;
    }

	public int GetButtonDownAnyIndex(string button)
	{
		for (int i = 0; i < gamepads.Count; ++i)
		{
			// Gamepad meets both conditions
			if (gamepads[i].IsConnected && gamepads[i].GetButtonDown(button))
				return i+1;
		}

		return 0;
	}

	//Faut que je change le GetButtonDownAny pour récupérer l'index du gamepad que je choppe
	
}
