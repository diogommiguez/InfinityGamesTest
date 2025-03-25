using UnityEngine;
using System.Collections;

public class CityInteraction : MonoBehaviour
{
    public float worldHeight;
    public float undergroundHeight;
    public float loweringDuration = 3f;
    private bool isNight = false;
    private bool isBelow = false;

    public GameObject extrinsicLandscape;

    public void GoBelow()
    {
        if(!isBelow)
        {
            StartCoroutine(LowerLevel(worldHeight,undergroundHeight));
            isBelow = true;
            Debug.Log("up!");
        }
        else
        {
            StartCoroutine(LowerLevel(undergroundHeight,worldHeight));
            isBelow = false;
            Debug.Log("below!");
        }
    }

    private IEnumerator LowerLevel(float startHeight, float endHeight)
    {
        float elapsedTime = 0;

        float Xpos = extrinsicLandscape.transform.localPosition.x;
        float Ypos;
        float Zpos = extrinsicLandscape.transform.localPosition.z;

        while (elapsedTime < loweringDuration)
        {
            Ypos = Mathf.Lerp(startHeight, endHeight, elapsedTime / loweringDuration);
            extrinsicLandscape.transform.localPosition = new Vector3(Xpos,Ypos,Zpos);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Ypos = endHeight;
        extrinsicLandscape.transform.localPosition = new Vector3(Xpos,Ypos,Zpos);
    }
}
