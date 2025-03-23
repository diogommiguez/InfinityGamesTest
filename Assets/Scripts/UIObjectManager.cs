using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIObjectManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Texture iconTexture;
    public ObjectPlacer objectPlacer;
    public ObjectData objectData;
    private CanvasGroup canvasGroup;
    private GameObject iconObject;
    

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Make UI icon transparent while dragging
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;

        // Spawn the 2d icon
        iconObject = new GameObject("ObjectIcon");
        iconObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
        RawImage rawImage = iconObject.AddComponent<RawImage>();
        rawImage.texture = iconTexture;
        rawImage.CrossFadeAlpha(0.6f,0,false);

        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 100); // Set width & height
        rectTransform.anchoredPosition = Input.mousePosition; // Center in Canvas
        
    }

    public void OnDrag(PointerEventData eventData)
    {   
        // call a function from ObjectPlacer that renders green(free) or red(used) tiles where the mouse pos is (or in a bigger area depending on the prefab area)
        if (iconObject)
        {            
            // Convert mousePosition from screen space to local space relative to the parent canvas
            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                iconObject.GetComponent<RectTransform>().parent.GetComponent<RectTransform>(), // Parent RectTransform
                Input.mousePosition, // Mouse position in screen space
                null, // Camera, use null if canvas is in "Screen Space - Overlay" mode
                out localPosition); // Output local position

            // Set the position of the icon's RectTransform to the converted mousePosition
            iconObject.GetComponent<RectTransform>().localPosition = localPosition;
        } 
        else
        {
            Debug.LogError("Object icon reference is missing!");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int objectPlacement = 0;
        if (objectPlacer != null)
        {
            objectPlacement = objectPlacer.PlaceWorldObject(objectData);
        }
        else
        {
            Debug.LogError("ObjectPlacer reference is missing!");
        }

        switch(objectPlacement){
            case 0:
                iconObject.GetComponent<RawImage>().CrossFadeAlpha(0,0.5f,false);
                Invoke("DestroyIconObject", 0.1f); 
                break;
            case 1:
                Destroy(iconObject);
                break;
        }
        // Destroy the UI icon when placed in the world
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    void DestroyIconObject()
    {
        Destroy(iconObject);
    }
}
