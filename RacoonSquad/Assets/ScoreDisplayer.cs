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
        var list = GameManager.instance.previousLevel.GetGatheredObjects();
        var parent = GameObject.Find("OBJECTS_SPAWN");
        for (int i = 0; i < list.Count; i++) {
            countText.text = string.Format(countString, i+1);
            Instantiate(list[i], parent.transform.position + new Vector3(Random.value * 10f - 5f, 0f), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);

        Show(dollarText);
        for (int i = 0; i < GameManager.instance.previousLevel.GetDollars(); i++) {
            dollarText.text = string.Format(dollarString, i + 1);
            Instantiate(list[i], parent.transform.position + new Vector3(Random.value * 10f - 5f, 0f), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2.5f);
        yield break;

    }

    public void Show(Behaviour comp)
    {
        comp.enabled = true;
        SoundPlayer.Play("fb_raccoon_bumping");
    }

}
