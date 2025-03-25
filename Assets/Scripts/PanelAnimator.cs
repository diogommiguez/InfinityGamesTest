using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PanelAnimator : MonoBehaviour
{
    public float movementDuration = 0.5f;
    public float openPosition_x;
    public float hiddenPosition_x;
    private bool isHidden = false;

    private void Awake()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        // force show panel
        rectTransform.anchoredPosition = new Vector2(openPosition_x,rectTransform.anchoredPosition.y);
    }

    public void HidePanel()
    {
        if(isHidden == false)
        {
            StartCoroutine(MovePanel(openPosition_x, hiddenPosition_x));
            isHidden = true;
        }
        else 
        {
            StartCoroutine(MovePanel(hiddenPosition_x, openPosition_x));
            isHidden = false;
        }
    }

    private IEnumerator MovePanel(float startPosition, float endPosition)
    {
        float elapsedTime = 0;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        while (elapsedTime < movementDuration)
        {
            rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(startPosition, endPosition, elapsedTime / movementDuration),rectTransform.anchoredPosition.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}