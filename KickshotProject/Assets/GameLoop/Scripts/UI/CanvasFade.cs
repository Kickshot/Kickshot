using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanvasFade : MonoBehaviour {

    public UnityEvent finishedFade;

    private CanvasGroup cGroup;
    private float curAlpha;
    private bool faded = false;
    private float targetAlpha;

    private float timer = 0f;

    private void Start()
    {
        cGroup = GetComponent<CanvasGroup>();
        if (Mathf.Approximately(cGroup.alpha, 0f))
            faded = true;
    }

    public void ToggleFade() {
        if (faded) {
            faded = false;
            targetAlpha = 1f;
        } else {
            faded = true;
            targetAlpha = 0f;
        }
        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        while (!Mathf.Approximately(Mathf.Round(curAlpha*10f)/10f, targetAlpha)) {
            curAlpha = Mathf.Lerp(curAlpha, targetAlpha, Time.deltaTime);
            cGroup.alpha = curAlpha;
            Debug.Log("Alpha: " + curAlpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        finishedFade.Invoke();
    }
}
