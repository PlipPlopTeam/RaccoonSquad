using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverAnimation : MonoBehaviour
{
    public RectTransform mask;
    public Image blackScreen;
    public float lerpSpeed = 10f;

    float firstStep = 1600f;

    private void Start()
    {
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);

        while (mask.sizeDelta.y > firstStep+10) {
            mask.sizeDelta = new Vector2(6000f, Mathf.Lerp(mask.sizeDelta.y, firstStep, lerpSpeed*Time.deltaTime));
            yield return true;
        }

        yield return new WaitForSeconds(1.5f);

        while (mask.sizeDelta.y > -10f) {
            mask.sizeDelta = new Vector2(6000f, Mathf.Lerp(mask.sizeDelta.y, -100f, lerpSpeed * Time.deltaTime));
            if (mask.sizeDelta.y < 150f) {
                blackScreen.enabled = true;
            }
            yield return true;
        }

        Destroy(gameObject);
        GameManager.instance.GoToLobby();
        yield return true;
    }
}
