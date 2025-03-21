using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableToWorld : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject worldObjectPrefab; // Assign a 3D prefab in the Inspector
    private GameObject spawnedObject;
    private Camera mainCamera;
    private CanvasGroup canvasGroup;

    public ObjectPlacer objectPlacer;

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

        // Spawn the 3D object in front of the camera
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (spawnedObject)
        {
            // Move the spawned object with the mouse
            //spawnedObject.transform.position = GetMouseWorldPosition();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Destroy the UI icon when placed in the world
        //Destroy(gameObject);

        if (objectPlacer != null)
        {
            objectPlacer.PlaceWorldObject(worldObjectPrefab);
        }
        else
        {
            Debug.LogError("ObjectPlacer reference is missing!");
        }

        //spawnedObject = Instantiate(worldObjectPrefab, GetMouseWorldPosition(), Quaternion.identity);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Distance from the camera
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
