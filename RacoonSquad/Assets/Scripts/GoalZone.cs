using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AbsorbedObject
{
    public GameObject obj;
    public Vector3 startPosition;
    public Vector3 startSize;
}

public class GoalZone : MonoBehaviour
{
    public List<PlayerController> racoonsInside = new List<PlayerController>();

    public MeshRenderer jaugeMeshRenderer;
    public Transform receptorTransform;

    float targetJaugeValue;
    float currentJaugeValue;
    float jaugeLerpSpeed = 5f;

    List<AbsorbedObject> absorbedObjects = new List<AbsorbedObject>();

    void Awake()
    {
        jaugeMeshRenderer.material = Instantiate(jaugeMeshRenderer.material);
    }

    void Update()
    {
        if(GameManager.instance.level != null) UpdateJauge();


        // Oui Rack, on s'en rappelle de ça 
        foreach(AbsorbedObject ao in absorbedObjects.ToArray())
        {
            if(ao.obj != null)
            {
                float progression = 1f - Vector3.Distance(ao.obj.transform.position, receptorTransform.position) / Vector3.Distance(ao.startPosition, receptorTransform.position);
                if(progression < 0.99f)
                {
                    ao.obj.transform.position = Vector3.Lerp(ao.obj.transform.position, receptorTransform.position, Time.deltaTime * 3f);
                    ao.obj.transform.localScale = ao.startSize * (1f - progression);
                }
                else
                {
                    absorbedObjects.Remove(ao);
                    ao.obj.SetActive(false);
                    ao.obj.transform.SetParent(GameManager.instance.transform);
                }
            }
        }
    }

    void UpdateJauge()
    {
        float tierMaxScore = GameManager.instance.level.GetBronzeTier();
        Color tierColor = Library.instance.tierColors[0];
        switch(GameManager.instance.level.GetCurrentTier())
        {
            case 1: 
                tierMaxScore = GameManager.instance.level.GetSilverTier(); 
                tierColor = Library.instance.tierColors[1];
                break;
            case 2: 
                tierMaxScore = GameManager.instance.level.GetGoldTier();
                tierColor = Library.instance.tierColors[2];
                break;
        }
        targetJaugeValue =  (float)GameManager.instance.level.GetScore() / tierMaxScore;
        currentJaugeValue = Mathf.Lerp(currentJaugeValue, targetJaugeValue, Time.deltaTime * jaugeLerpSpeed);
        jaugeMeshRenderer.material.SetFloat("_Value", currentJaugeValue);
        jaugeMeshRenderer.material.SetColor("_ColorB", tierColor);
    }


    private void OnTriggerEnter(Collider other)
    {
        var pc = other.GetComponent<PlayerController>();

        if (pc != null) {
            // A player
            if (pc.IsHolding())
            {
                var prop = pc.GetHeldObject();
                pc.DropHeldObject();
                pc.objectsAtRange.Remove(prop);
                Absorb(prop);
            }
            racoonsInside.RemoveAll(o=>o==pc);
            racoonsInside.Add(pc);

            if (racoonsInside.Count == GameManager.instance.GetPlayers().Count) {
                CheckWin();
            }
        }
        else {
            // A prop
            var prop = other.GetComponent<Grabbable>();
            if (prop != null && !prop.IsHeld())
            {
                Absorb(prop);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pc = other.GetComponent<PlayerController>();

        if (pc != null) {
            racoonsInside.RemoveAll(o => o == pc);
        }
    }

    void Absorb(Grabbable grabbable)
    {
        // ABSORB
        grabbable.GetProp().rigidbody.isKinematic = true;
        grabbable.GetProp().collider.enabled = false;

        AbsorbedObject ao = new AbsorbedObject();
        ao.obj = grabbable.gameObject;
        ao.startPosition = grabbable.transform.position;
        ao.startSize = grabbable.transform.localScale;
        absorbedObjects.Add(ao);
        
        SoundPlayer.PlayWithRandomPitch("fb_scoring_loot", 0.3f);
        if (GameManager.instance.IsInLobby())
        {
            if (grabbable.tag == "GameStarter") GameManager.instance.NextLevel();
        }
        else 
        {
            GameManager.instance.level.Score(grabbable);
        }

        Destroy(grabbable);
    }

    void CheckWin()
    {
        if(GameManager.instance.level == null) return;

        if (GameManager.instance.level.GetScore() >= GameManager.instance.level.GetBronzeTier()) {
            GameManager.instance.Win();
        }
    }
}
