using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [Header("Settings")]
    public Camera cam;
    public float lerpSpeed = 5f;
    public float distanceMin = 1f;
    public float distanceMax = 10f;

    float originDistance;
    float distance;
    Vector3 targetPosition;
    Vector3 directionToPivot;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        directionToPivot = (cam.transform.position - transform.position).normalized;
        originDistance = (cam.transform.position - transform.position).magnitude;
        distance = originDistance;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, directionToPivot * distance, Time.deltaTime * lerpSpeed);
    }

    public void Set(Vector3 newPoint, float newDistance)
    {
        targetPosition = newPoint;
        distance = newDistance;
    }
}
