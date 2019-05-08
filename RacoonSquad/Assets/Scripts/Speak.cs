using System.Collections;
using UnityEngine;
using TMPro;

public class Speak : MonoBehaviour
{
    [Header("References")]
    public Transform origin;
    public Material material;
    public TMP_FontAsset fontAsset;

    [Header("Settings")]
    public Vector3 offSet = Vector3.zero;
    public Vector2 boxSize;
    public float fontSize;

    GameObject textObject;
    RectTransform textTransform;
    TextMeshPro textMesh;

    void Awake()
    {
        if(origin == null) origin = transform;
        Setup();
    }

    void Setup()
    {
        textObject = new GameObject();
        // GameObject settup
        textObject.name = "Voice";
        // Adding a mesh renderer and applaying material
        textObject.AddComponent<MeshRenderer>().material = material;
        textMesh = textObject.AddComponent<TextMeshPro>();


        textMesh.font = fontAsset;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = fontSize;

        textTransform = textObject.GetComponent<RectTransform>();
        textTransform.sizeDelta = boxSize;
    }

    public void Say(string text)
    {
        textMesh.text = text;
        StartCoroutine(HideAtfer(text.Length * 0.25f));
    }

    IEnumerator HideAtfer(float time) 
    {
        yield return new WaitForSeconds(time);
        Hide();
    }

    public void Hide()
    {
        textMesh.text = "";
    }

    void Update()
    {
        textObject.transform.position = origin.position + offSet;
        textObject.transform.forward = -(Camera.main.transform.position - transform.position);
    }
}
