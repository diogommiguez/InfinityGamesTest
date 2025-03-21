using System;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject parentObject; // Assign this in the Inspector or find it in code
    public Material planeMaterial1;
    public Material planeMaterial2;
    private Plane fullPlane;
    private bool[,] isTileAvailable = new bool[30, 30];
    /*public enum TileType
    {
        Empty,      // 0
        Road,       // 1
        Tree,       // 2
        House,      // 3
        Shop       // 4
    }*/
    ObjectType[,] cityLayout; // Example 10x10 grid

    void Start()
    {
        cityLayout = new ObjectType[30, 30]; 

        cityLayout[0, 0] = ObjectType.Empty;

        fullPlane = new Plane( new Vector3(0,1,0), new Vector3(0,0.01f,0) );

        // -------------------------------------------
        if (parentObject == null)
        {
            Debug.LogError("Parent Object is not assigned!");
            return;
        }

        for(int i =0; i < 30; i++)
        {
            for(int j =0; j < 30; j++)
            {
                isTileAvailable[i,j] = true;
                // Create a new plane
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                Vector3 localPosition = new Vector3(i,0.01f,j);

                // Set the parent
                plane.transform.SetParent(parentObject.transform, false); // 'false' keeps local position

                // Set local position relative to the parent
                plane.transform.localPosition = localPosition;

                // Optionally, set local rotation and scale
                plane.transform.localRotation = Quaternion.identity;
                plane.transform.localScale = new Vector3(0.2f,0.2f,0.2f);

                if (planeMaterial1 != null)
                {
                    if((i*j+i*i-36*j*j*j/(1+i))%2==0) plane.GetComponent<Renderer>().material = planeMaterial1;
                    else plane.GetComponent<Renderer>().material = planeMaterial2;
                }
                else
                {
                    Debug.LogWarning("No material assigned. Using default.");
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButton(0))
        {
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            float enter = 0.0f;

            
            if (fullPlane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);
                if(hitPoint.x>0 && hitPoint.x<30 && hitPoint.z>0 && hitPoint.z<30)
                     m_Cube.transform.position = new Vector3((float)Math.Truncate(hitPoint.x)+0.5f,0.01f,(float)Math.Truncate(hitPoint.z)+0.5f);
            }
        }
        */
    }

    public void PlaceWorldObject(GameObject worldObjectPrefab, ObjectType objectType)
    {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Initialise the enter variable
        float enter = 0.0f;
        
        if (fullPlane.Raycast(ray, out enter))
        {
            // Get the point that is clicked
            Vector3 mouseWorldPosition = ray.GetPoint(enter);

            // Check if its within city bounds
            if( IsWithinCityBounds(mouseWorldPosition) == false)
            {
                return; // call animation to puff the dragging UI icon
            }

            // Position in dicrete grid of 1x1 tiles (x,y)
            Vector2Int gridPosition = GetGridPosition(mouseWorldPosition);

            // Check wether selected tile(s) are available
            if(isTileAvailable[gridPosition.x,gridPosition.y])
            {
                GameObject worldObject = Instantiate(worldObjectPrefab, GetGridPositionInWorldCoordinates(gridPosition), Quaternion.identity);
                
                GameObject objectsParent = GameObject.Find("Objects");
                if(objectsParent == null){
                    objectsParent = new GameObject("Objects");
                } 
                worldObject.transform.SetParent(objectsParent.transform, false); // 'false' keeps world position
                worldObject.name = objectType.ToString();
                isTileAvailable[gridPosition.x,gridPosition.y] = false;
            } 
            else 
            {
                return;
            }
            
        }
    }

    bool IsWithinCityBounds(Vector3 worldPosition)
    {
        if(worldPosition.x < 0 || worldPosition.x > 30) return false;
        if(worldPosition.z < 0 || worldPosition.z > 30) return false;
        return true;
    }

    Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        // add a small y offset to prevent mesh overlap
        return new Vector2Int((int)Math.Truncate(worldPosition.x),(int)Math.Truncate(worldPosition.z));
    }

    Vector3 GetGridPositionInWorldCoordinates(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x,0.01f,gridPosition.y);
    }
}
