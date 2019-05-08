using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayerManager
{
	static List<Displayer> displayers = new List<Displayer>();

    static public Displayer SetRotationFeed(GameObject newObject, RawImage image, int size = 64, float scale=1f, float rotation = 0f, float speed = 0f, float camDistance = 3f, float camFOV = 30f)
	{
        Displayer d = GetDisplayer();
		d.Stage(newObject, image, scale, rotation, speed, camDistance, camFOV, size);
		return d;
	}

	static Displayer GetDisplayer()
	{
		foreach(Displayer d in displayers)
		{
			if(d.available)
				return d;
		}
		Vector3 newPosition = new Vector3(displayers.Count * 3, -1000f, -1000f);
		displayers.Add(GameObject.Instantiate(Library.instance.displayerPrefab, newPosition, Quaternion.identity).GetComponent<Displayer>());
		return displayers[displayers.Count - 1];
	}

    public void UnstageAll()
    {
        foreach(Displayer displayer in displayers) {
            displayer.Unstage();
        }
    }
}
