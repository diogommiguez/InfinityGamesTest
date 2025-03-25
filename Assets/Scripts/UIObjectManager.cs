using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIObjectManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Texture iconTexture;
    public Vector2Int iconSize = new Vector2Int(75,75);
    public Vector2Int iconPositionOffset = new Vector2Int(50,50);
    public ObjectPlacer objectPlacer;
    public ObjectData objectData;
    private CanvasGroup canvasGroup;
    private GameObject iconObject;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        RawImage iconRawImage = gameObject.GetComponent<RawImage>();
        iconRawImage.texture = iconTexture;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Make UI icon transparent while dragging and ignore interactions
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        // Spawn the 2d icon
        iconObject = new GameObject("ObjectIcon");
        iconObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
        RawImage rawImage = iconObject.AddComponent<RawImage>();
        rawImage.texture = iconTexture;
        rawImage.CrossFadeAlpha(0.8f,0,false);
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = iconSize; // Set width & height
        rectTransform.anchoredPosition = Input.mousePosition; // Center in Canvas

        // Create a placement plane (visual aid - see in ObjectPlacer class)
        objectPlacer.CreatePlacementPlane(objectData.objectSize);
        
    }

    public void OnDrag(PointerEventData eventData)
    {   
        // Update dragged icon position
        if (iconObject)
        {            
            // Convert mousePosition from screen space to local space relative to the parent canvas
            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                iconObject.GetComponent<RectTransform>().parent.GetComponent<RectTransform>(), // Parent RectTransform
                Input.mousePosition, // Mouse position in screen space
                null, // Camera, use null if canvas is in "Screen Space - Overlay" mode
                out localPosition); // Output local position

            localPosition += new Vector2(iconPositionOffset.x,iconPositionOffset.y);

            // Set the position of the icon's RectTransform to the converted mousePosition
            iconObject.GetComponent<RectTransform>().localPosition = localPosition;
        } 
        else
        {
            Debug.LogError("Object icon reference is missing!");
        }

        // Tell objectPlacer which tile we are focusing on now
        objectPlacer.UpdateFocusedTilePosition(Input.mousePosition);
        // And tell it to update the placement plane
        objectPlacer.UpdatePlacementPlane(objectData.objectSize);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int objectPlacement = 0;
        if (objectPlacer != null)
        {
            objectPlacer.DestroyPlacementPlane();
            objectPlacement = objectPlacer.PlaceWorldObject(objectData);
        }
        else
        {
            Debug.LogError("ObjectPlacer reference is missing!");
        }

        switch(objectPlacement){
            case 0: // failure in object placement
                StartCoroutine(ReturnIconToPanel(0.2f));
                break;
            case 1: // success in object placement
                StartCoroutine(PuffIcon(0.2f));
                break;
        }
        
        // Set icon to normal transparency and allow interactions
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    // Animation when object can't be placed: icon floats back to panel
    IEnumerator ReturnIconToPanel(float duration)
    {
        Vector3 endPosition = transform.position;
        Vector3 startPosition = iconObject.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Lerp position
            iconObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Destroy icon at the end of the animation
        Destroy(iconObject);
    }

    // Animation when object can be placed: iconscales to zero as it fades out
    IEnumerator PuffIcon(float duration)
    {
        // Fade out icon
        iconObject.GetComponent<RawImage>().CrossFadeAlpha(0,duration,false);

        // Squeeze icon (animate scale to zero)
        Vector3 endScale = new Vector3(0,0,0);
        Vector3 startScale = iconObject.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Lerp position
            iconObject.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Destroy icon at the end of the animation
        Destroy(iconObject);
    }


}
