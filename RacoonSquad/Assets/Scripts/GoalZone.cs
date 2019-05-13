using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

class AbsorbedObject
{
    public GameObject obj;
    public Vector3 startPosition;
    public Vector3 startSize;
}

[System.Serializable]
public class ScoreStar
{
    public GameObject obj;
    public Transform transform;
    public MeshRenderer mr;
}

public class GoalZone : MonoBehaviour
{
    [Header("Referencies")]
    public GameObject raccountObject;
    public TextMeshPro raccountText;
    public MeshRenderer jaugeMeshRenderer;
    public Transform receptorTransform;
    public ScoreStar[] stars;

    [HideInInspector] public List<PlayerController> raccoonsInside = new List<PlayerController>();
    Speak speak;
    

    // Lerp jauge value
    float targetJaugeValue;
    float currentJaugeValue;
    float currentTierMaxScore;
    float jaugeLerpSpeed = 5f;
    Color currentTierColor;

    List<AbsorbedObject> absorbedObjects = new List<AbsorbedObject>();

    void Start()
    {
        speak = GetComponent<Speak>();
        jaugeMeshRenderer.material = Instantiate(jaugeMeshRenderer.material);
        foreach(ScoreStar ss in stars)
        {
            ss.mr.material = Instantiate(ss.mr.material);
            ss.mr.material.SetColor("_ColorA", Library.instance.colorLocked);
        }

        if(GameManager.instance.level != null) 
        {
            OnScoreChange();
            GameManager.instance.level.OnScoreChange += () => this.OnScoreChange();
        }

        speak.Say("Good luck boys!");
        raccountObject.SetActive(false);
        UpdateRaccount();
    }

    void Update()
    {
        if(GameManager.instance.level != null) UpdateJauge();

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

    void OnScoreChange()
    {
        // Get Current Tier index;
        int currentTier = GameManager.instance.level.GetCurrentTier();

        // Change Jauge values
        switch(currentTier)
        {
            case 0:
                currentTierMaxScore = GameManager.instance.level.GetBronzeTier();
                currentTierColor = Library.instance.tierColors[0];
                break;
            case 1: 
                currentTierMaxScore = GameManager.instance.level.GetSilverTier(); 
                currentTierColor = Library.instance.tierColors[1];
                break;
            case 2: 
                currentTierMaxScore = GameManager.instance.level.GetGoldTier();
                currentTierColor = Library.instance.tierColors[2];
                break;
            case 3: 
                currentTierMaxScore = GameManager.instance.level.GetGoldTier();
                currentTierColor = Library.instance.tierColors[3];
                break;
            default:
                currentTierMaxScore = GameManager.instance.level.GetBronzeTier();
                currentTierColor = Library.instance.tierColors[0];
                break;
        }

        // Change Star values
        for(int i = 0; i <  GameManager.instance.level.GetCurrentTier(); i++)
        {
            stars[i].mr.material.SetColor("_ColorA", Library.instance.colorUnlocked);
        }

        if(GameManager.instance.level.GetScore() > GameManager.instance.level.GetBronzeTier())
        {
            speak.Say("We got enough but can you get some more?");
            raccountObject.SetActive(true);
        }
    }

    void UpdateJauge()
    {
        targetJaugeValue =  (float)GameManager.instance.level.GetScore() / currentTierMaxScore;
        currentJaugeValue = Mathf.Lerp(currentJaugeValue, targetJaugeValue, Time.deltaTime * jaugeLerpSpeed);
        jaugeMeshRenderer.material.SetFloat("_Value", currentJaugeValue);
        jaugeMeshRenderer.material.SetColor("_ColorB", currentTierColor);
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
            raccoonsInside.RemoveAll(o=>o==pc);
            raccoonsInside.Add(pc);

            if (raccoonsInside.Count == GameManager.instance.GetPlayers().Count) {
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

        UpdateRaccount();
    }

    private void OnTriggerExit(Collider other)
    {
        var pc = other.GetComponent<PlayerController>();

        if (pc != null) {
            raccoonsInside.RemoveAll(o => o == pc);
        }

        UpdateRaccount();
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
        if (!GameManager.instance.IsInGame())
        {
            var action = grabbable.GetComponent<ActionCube>();
            if (action != null) {
                switch (action.action) {
                    case "startGame": GameManager.instance.NextLevel(); break;
                    case "levelEditor": GameManager.instance.GoToLevelEditor(); break;
                    case "saveLevel": GameManager.instance.SaveLevelWindow(); Instantiate(Library.instance.editorSaveLevelCube, new Vector3(), Quaternion.identity); break;
                }
            }
        }
        else if (GameManager.instance.IsInGame())
        {
            GameManager.instance.level.Score(grabbable);
            OnScoreChange();
        }
        Destroy(grabbable);
    }

    void UpdateRaccount()
    {
        raccountText.text = raccoonsInside.Count + "/" + GameManager.instance.GetPlayers().Count;
    }

    void CheckWin()
    {
        if(GameManager.instance.level == null) return;

        if (GameManager.instance.level.GetScore() >= GameManager.instance.level.GetBronzeTier()) {
            GameManager.instance.Win();
        }
    }
}
