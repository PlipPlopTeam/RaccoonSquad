using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [Header("Settings")]
    public Camera cam;
    public float lerpSpeed = 5f;

    float originDistance;
    Vector3 originPosition;

    float distance;
    Vector3 targetPosition;
    Vector3 directionToPivot;
    Transform targetTransform;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        directionToPivot = (cam.transform.position - transform.position).normalized;
        originDistance = (cam.transform.position - transform.position).magnitude;
        originPosition = transform.position;
        distance = originDistance;
    }

    void Update()
    {
        if(targetTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * lerpSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
        }
        


        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, directionToPivot * distance, Time.deltaTime * lerpSpeed);
    }

    public void JumpTo(Vector3 newPoint, float newDistance)
    {
        transform.position = newPoint;
        cam.transform.localPosition = directionToPivot * distance;
    }

    public void FocusOn(Vector3 newPosition, float newDistance)
    {
        targetPosition = newPosition;
        targetTransform = null;
        distance = newDistance;
    }
    public void FocusOn(Transform newTransform, float newDistance)
    {
        targetTransform = newTransform;
        targetPosition = Vector3.zero;
        distance = newDistance;
    }

    public void Reset()
    {
        targetTransform = null;
        targetPosition = originPosition;
        distance = originDistance;
    }
}
