using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableToWorld : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject worldObjectPrefab; // Assign a 3D prefab in the Inspector
    private GameObject objectIcon;
    private Camera mainCamera;
    private CanvasGroup canvasGroup;

    public ObjectPlacer objectPlacer;
    private GameObject rawImageObject;
    public Texture iconTexture;
    private void Awake()
    {
        mainCamera = Camera.main;
        canvasGroup = GetComponent<CanvasGroup>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Make UI icon transparent while dragging
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        // Spawn the 2d icon

        rawImageObject = new GameObject("ObjectIcon");
        rawImageObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
        RawImage rawImage = rawImageObject.AddComponent<RawImage>();
        rawImage.texture = iconTexture;

        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 100); // Set width & height
        rectTransform.anchoredPosition = Input.mousePosition; // Center in Canvas
        
    }

    public void OnDrag(PointerEventData eventData)
    {   
        // call a function from ObjectPlacer that renders green(free) or red(used) tiles where the mouse pos is (or in a bigger area depending on the prefab area)
        if (rawImageObject)
        {            
            // Convert mousePosition from screen space to local space relative to the parent canvas
            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rawImageObject.GetComponent<RectTransform>().parent.GetComponent<RectTransform>(), // Parent RectTransform
                Input.mousePosition, // Mouse position in screen space
                null, // Camera, use null if canvas is in "Screen Space - Overlay" mode
                out localPosition); // Output local position

            // Set the position of the icon's RectTransform to the converted mousePosition
            rawImageObject.GetComponent<RectTransform>().localPosition = localPosition;
        } 
        else
        {
            Debug.LogError("Object icon reference is missing!");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (objectPlacer != null)
        {
            objectPlacer.PlaceWorldObject(worldObjectPrefab);
        }
        else
        {
            Debug.LogError("ObjectPlacer reference is missing!");
        }
        
        // Destroy the UI icon when placed in the world
        Destroy(rawImageObject);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
