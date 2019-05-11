using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCube : MonoBehaviour
{
    public string action;
    public Color color = Color.white;

    private void Start()
    {
        var inst = new Material(Shader.Find("Unlit/Color"));
        inst.color = color;
        GetComponent<MeshRenderer>().material = inst;
    }

    private void Update()
    {
        if (transform.position.y < -1f) {
            transform.position = new Vector3();
        }
    }
}
