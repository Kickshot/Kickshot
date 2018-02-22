using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanvasFade : MonoBehaviour {

    public UnityEvent finishedFade;

    private CanvasGroup cGroup;
    private float curAlpha;
    private bool faded = true;
    private float targetAlpha;

    private float timer = 0f;

    private void Start()
    {
        gameObject.SetActive(false);
        cGroup = GetComponent<CanvasGroup>();
        if (Mathf.Approximately(cGroup.alpha, 0f))
            faded = true;
    }

    public void ToggleFade()
    {
        if (faded)
        {
            faded = false;
            targetAlpha = 1f;
            gameObject.SetActive(true);
        }
        else
        {
            faded = true;
            targetAlpha = 0f;
        }
        
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        while (!Mathf.Approximately(Mathf.Round(curAlpha*10f)/10f, targetAlpha)) {
            curAlpha = Mathf.Lerp(curAlpha, targetAlpha, Time.deltaTime);
            cGroup.alpha = curAlpha;
            Debug.Log("Alpha: " + curAlpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        gameObject.SetActive(false);
        finishedFade.Invoke();
    }
}
