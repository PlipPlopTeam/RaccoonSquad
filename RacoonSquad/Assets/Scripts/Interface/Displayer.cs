using UnityEngine;
using UnityEngine.UI;

public enum View{ Front, Plundge, Top }

public class Displayer : MonoBehaviour 
{
	public Camera cam;

	[HideInInspector] public bool available;
	RawImage feed;
	GameObject model;
	float cSpeed;
	RenderTexture cTexture;
	
	public void Stage(GameObject newObject, RawImage image, float scale=1f, float rotation = 0f, float speed = 0f, float camDistance = 3f, float camFOV = 30f,  int size = 64)
	{
		cam.enabled = true;

        // Spawning the new model and applying the rotation
        if (newObject == null) model = new GameObject();
        else model = Instantiate(newObject);

        model.transform.parent = transform;
        model.transform.localPosition = new Vector3();
		model.transform.eulerAngles = new Vector3(-25f, rotation, 25f);


        model.transform.localScale =
            new Vector3(1f, 1f, 1f)*scale;

		// Apply speed
		cSpeed = speed;

		// Creating new texture and applying referencies
		feed = image;
		cTexture = new RenderTexture(size, size, 16, RenderTextureFormat.ARGB32);
		feed.texture = cTexture;

		// Applying camera settings
		cam.targetTexture = cTexture;
		cam.fieldOfView = camFOV;
		cam.transform.localPosition = new Vector3(0f, 0f, -camDistance);

		available = false;
	}

    public GameObject GetModel()
    {
        return model;
    }

	public void Unstage()
	{
		cam.targetTexture = null;
		cam.enabled = false;
		Destroy(model);
		Destroy(cTexture);
		available = true;
	}

	void Update()
	{
		if(model != null)
			model.transform.Rotate(Vector3.up * cSpeed);
	}	
}
