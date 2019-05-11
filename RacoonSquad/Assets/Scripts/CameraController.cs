using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [Header("Settings")]
    public Camera cam;
    public float lerpSpeed = 5f;

    float originDistance;
    Vector3 originPosition;
    Vector3 originRotation;

    float distance;
    Vector3 targetPosition;
    Vector3 directionToPivot;
    Transform targetTransform;


    public float noiseSpeed;
    public float noiseAmp;
    public float noiseRot;
    private Vector3 posOff;
    private Vector3 rotOff;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        directionToPivot = (cam.transform.position - transform.position).normalized;
        originDistance = (cam.transform.position - transform.position).magnitude;
        originPosition = transform.position;
        originRotation = cam.transform.localEulerAngles;
        distance = originDistance;
        
        posOff = new Vector3(Random.Range(0f,100f),Random.Range(0f,100f),Random.Range(0f,100f));
        rotOff = new Vector3(Random.Range(0f,100f),Random.Range(0f,100f),Random.Range(0f,100f));

        if (GameManager.instance.sceneType == GameManager.SceneType.Editor) {
            noiseSpeed = 0f;
            noiseAmp = 0f;
            noiseRot = 0f;
        }
    }

    void Update()
    {
        if(targetTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * lerpSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition + new Vector3(Mathf.PerlinNoise(Time.time * noiseSpeed + posOff.x, Time.time * noiseSpeed + posOff.x) * noiseAmp, 
                                                                                               Mathf.PerlinNoise(Time.time * noiseSpeed + posOff.y, Time.time * noiseSpeed + posOff.y) * noiseAmp,
                                                                                               Mathf.PerlinNoise(Time.time * noiseSpeed + posOff.z, Time.time * noiseSpeed + posOff.z) * noiseAmp), Time.deltaTime * lerpSpeed);

            cam.transform.localEulerAngles = originRotation + new Vector3(
                Mathf.PerlinNoise(Time.time * noiseSpeed + rotOff.x, Time.time * noiseSpeed + rotOff.x) * noiseRot,
                Mathf.PerlinNoise(Time.time * noiseSpeed + rotOff.y, Time.time * noiseSpeed + rotOff.y) * noiseRot,
                Mathf.PerlinNoise(Time.time * noiseSpeed + rotOff.z, Time.time * noiseSpeed + rotOff.z) * noiseRot);
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
