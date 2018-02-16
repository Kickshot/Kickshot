using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScrollView : MonoBehaviour
{
    public GameObject[] levelButtons;
    public GameObject levelButtonHolder;
    public int viewCapacity = 3;
    public float buttonWidth = 500;
    public float buttonPadding = 100;
    public float lerpDuration = 0.5f;

    private int currentButton = 0;

    private float currentDelta = 0;
    private float targetDelta = 0;

    private bool lerping = false;

    private void Start()
    {
        if (levelButtons.Length == 0)
            Debug.LogError("No levels found in levelButtons");

        currentDelta = levelButtonHolder.transform.localPosition.x;
        targetDelta = currentDelta;
    }

    public void ScrollLeft()
    {
        if (lerping)
            return;
        if (currentButton == 0)
            return;
        currentButton--;
        targetDelta += buttonWidth + buttonPadding;
        StartCoroutine(LerpView(lerpDuration));
    }

    public void ScrollRight()
    {
        if (lerping)
            return;
        if (currentButton == levelButtons.Length - viewCapacity)
            return;
        currentButton++;
        targetDelta -= buttonWidth + buttonPadding;
        StartCoroutine(LerpView(lerpDuration));
    }

    private IEnumerator LerpView(float duration) {
        lerping = true;
        float curDur = 0f;

        while (curDur < duration) {
            curDur += Time.deltaTime;
            float p = Mathf.Clamp01(curDur / duration);
            Vector3 temp = levelButtonHolder.transform.localPosition;
            temp.x = Mathf.Lerp(currentDelta, targetDelta, p);
            levelButtonHolder.transform.localPosition = temp;
            yield return null;
        }

        currentDelta = targetDelta;
        lerping = false;
    }
}
