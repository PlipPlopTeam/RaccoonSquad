using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshGamepads : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Normal unity update
    void Update()
    {
        GamepadManager.Instance.Refresh();
    }
}
