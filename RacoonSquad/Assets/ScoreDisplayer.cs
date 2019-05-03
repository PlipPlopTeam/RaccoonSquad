using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XInputDotNetPure;

public class ScoreDisplayer : MonoBehaviour
{
    public TextMeshProUGUI initialText;
    public TextMeshProUGUI finalText;

    public TextMeshProUGUI countText;
    public TextMeshProUGUI dollarText;

    string countString;
    string dollarString;
    bool isFinishedCounting = false;

    private void Start()
    {
        countString = countText.text;
        dollarString = dollarText.text;
        
        StartCoroutine(ShowScores());
    }

    public void Update()
    {
        foreach(var p in GameManager.instance.GetPlayers()) {

            var state = GamePad.GetState(p.index);
            if (state.Buttons.X == ButtonState.Pressed && isFinishedCounting) {
                Destroy(gameObject);
                GameManager.instance.NextLevel();
            }
        }
    }

    IEnumerator ShowScores()
    {
        yield return new WaitForSeconds(2.5f);
        Show(initialText);
        yield return new WaitForSeconds(1.5f);

        Show(countText);
        var parent = GameObject.Find("OBJECTS_SPAWN");
        var skips = 0;
        for (int i = 0; i < GameManager.instance.transform.childCount; i++) {
            var child = GameManager.instance.transform.GetChild(i).gameObject.GetComponent<Prop>();
            if (child == null) {
                skips++;
                continue;
            }
            countText.text = string.Format(countString, i+1- skips);

            child.transform.position = parent.transform.position + new Vector3(Random.value * 10f - 5f, 0f);
            child.transform.localScale = new Vector3(1f, 1f, 1f);
            child.gameObject.AddComponent<Grabbable>();
            child.GetComponent<Collider>().enabled = true;
            child.gameObject.SetActive(true);
            child.rigidbody.isKinematic = false;

            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);

        Show(dollarText);
        var dollars = GameManager.instance.previousLevel.GetDollars();
        for (int i = 0; i < dollars; i++) {
            dollarText.text = string.Format(dollarString, i + 1);
            yield return new WaitForSeconds(3f/dollars);
        }

        yield return new WaitForSeconds(2.5f);

        Show(finalText);
        isFinishedCounting = true;
        yield return true;

    }

    public void Show(Behaviour comp)
    {
        comp.enabled = true;
        SoundPlayer.Play("fb_raccoon_bumping");
    }

}
