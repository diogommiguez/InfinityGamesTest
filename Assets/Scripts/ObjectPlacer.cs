using System;
using UnityEngine;
using System.Collections;

public class ObjectPlacer : MonoBehaviour
{
    public Material freeTileMaterial;
    public Material occupiedTileMaterial;
    public Material SandMaterial;
    public Material roadMaterial;
    private float floorHeight = 0.1f;
    public GameObject roadsParentObject;
    public GameObject naturalElementsParentObject;
    private Plane fullPlane;
    private bool[,] isTileAvailable = new bool[30, 30];
   
    private TerrainType[,] terrainLayout;
    private GameObject placementPlane;
    private Vector2Int focusedTilePosition;

    void Start()
    {
        terrainLayout = new TerrainType[30, 30]; 

        fullPlane = new Plane( new Vector3(0,1,0), new Vector3(0,0.01f,0) );

        for(int i =0; i < 30; i++)
        {
            for(int j =0; j < 30; j++)
            {
                // Initializing terrain
                terrainLayout[i, j] = TerrainType.Sand;
                if(j == 6 || i == 4) terrainLayout[i, j] = TerrainType.Road;

                isTileAvailable[i,j] = true;
            }
        }
        CreateCityTayout();
    }

    void CreateCityTayout()
    {
        for(int i =0; i < 30; i++)
        {
            for(int j =0; j < 30; j++)
            {
                // Initializing terrain
                terrainLayout[i, j] = TerrainType.Sand;
                isTileAvailable[i,j] = true;
            }
        }
        // ROADS
        int x_min, x_max, z_min, z_max;
        foreach(Transform road in roadsParentObject.transform)
        {
            x_min = (int)Math.Round(road.localPosition.x - 5 * road.localScale.x);
            x_max = (int)Math.Round(road.localPosition.x + 5 * road.localScale.x);

            z_min = (int)Math.Round(road.localPosition.z - 5 * road.localScale.z);
            z_max = (int)Math.Round(road.localPosition.z + 5 * road.localScale.z);

            for(int i = x_min; i < x_max; i++)
            {
                for(int j = z_min; j < z_max; j++)
                {
                    if(i < 0 || i >= 30 || j < 0 || j >= 30)
                    {
                        continue;
                    }
                    terrainLayout[i,j] = TerrainType.Road;
                    isTileAvailable[i,j] = false;
                }
            }
            road.localPosition = new Vector3(road.localPosition.x,-floorHeight,road.localPosition.z);
        }

        // Natural Elements
        foreach(Transform naturalElement in naturalElementsParentObject.transform)
        {
            x_min = (int)Math.Round(naturalElement.localPosition.x );
            x_max = (int)Math.Round(naturalElement.localPosition.x + naturalElement.localScale.x);

            z_min = (int)Math.Round(naturalElement.localPosition.z );
            z_max = (int)Math.Round(naturalElement.localPosition.z + naturalElement.localScale.z);

            for(int i = x_min; i < x_max; i++)
            {
                for(int j = z_min; j < z_max; j++)
                {
                    if(i < 0 || i >= 30 || j < 0 || j >= 30)
                    {
                        continue;
                    }
                    isTileAvailable[i,j] = false;
                }
            }
        }
        
        GameObject cityLayout = new GameObject("Terrain");
        cityLayout.transform.SetParent(transform,false);

        for(int i =0; i < 30; i++)
        {
            for(int j =0; j < 30; j++)
            {
                if(terrainLayout[i,j]==TerrainType.Sand)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tile.transform.SetParent(cityLayout.transform,false);
                    tile.transform.localScale = new Vector3(1, floorHeight, 1);
                    tile.transform.localPosition = new Vector3(i+0.5f,-floorHeight/2.0f,j+0.5f);
                    tile.name = "Terrain(" + i.ToString() + "," + j.ToString() + ")";
                    tile.GetComponent<Renderer>().material = SandMaterial;
                }
                    /*GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    tile.transform.SetParent(cityLayout.transform,false);
                    tile.transform.localScale = new Vector3(0.1f, 1, 0.1f);
                    tile.transform.localPosition = new Vector3(i+0.5f,floorHeight,j+0.5f);
                    tile.name = "Terrain(" + i.ToString() + "," + j.ToString() + ")";
                    tile.GetComponent<Renderer>().material = SandMaterial;*/
            }
        }
        
    }

    void Update()
    {
        
    }

    public void UpdateFocusedTilePosition(Vector2 mousePosition)
    {
         //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        //Initialise the enter variable
        float enter = 0.0f;
        
        if (fullPlane.Raycast(ray, out enter))
        {
            // Get the point that is clicked
            Vector3 mouseWorldPosition = ray.GetPoint(enter);

            // Position in dicrete grid of 1x1 tiles (x,y)
            focusedTilePosition = GetGridPosition(mouseWorldPosition);
        }
    }
    
    public void CreatePlacementPlane(Vector2Int objectSize)
    {
        placementPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        placementPlane.name = "Auxiliary Placement Plane";
        // Set the parent
        placementPlane.transform.SetParent(transform,false);
        // Set scale (based on default 10x10 plane) to match the objectSize
        placementPlane.transform.localScale = new Vector3(0.1f * objectSize.x , 1, 0.1f * objectSize.y );

        // Don't render the plane just yet
        placementPlane.SetActive(false);
    }

    public void UpdatePlacementPlane(Vector2Int objectSize)
    {
        placementPlane.transform.localPosition = new Vector3(focusedTilePosition.x + objectSize.x/2.0f , 0.01f ,focusedTilePosition.y + objectSize.y/2.0f);

        int availabilityState = CheckAvailability(focusedTilePosition,objectSize);
        
        switch(availabilityState){
            case -1: // completely off map
                placementPlane.SetActive(false);
                break;
            case 0: // occupied or partially off map
                if (occupiedTileMaterial != null) 
                    placementPlane.GetComponent<Renderer>().material = occupiedTileMaterial;
                else
                    Debug.LogWarning("No material assigned to occupied tiles. Using default.");
                placementPlane.SetActive(true);
                break;
            case 1: // free
                if (freeTileMaterial != null) 
                    placementPlane.GetComponent<Renderer>().material = freeTileMaterial;
                else
                    Debug.LogWarning("No material assigned to free tiles. Using default.");
                placementPlane.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void DestroyPlacementPlane()
    {
        if(placementPlane == null) return;
        else Destroy(placementPlane); 
    }
   
    public int PlaceWorldObject(ObjectData objectData)//)GameObject worldObjectPrefab, ObjectType objectType)
    { 
        // Check wether selected tile(s) are available
        if(CheckAvailability(focusedTilePosition,objectData.objectSize) == 1)
        {
            GameObject worldObject = Instantiate(objectData.objectPrefab, GetGridPositionInWorldCoordinates(focusedTilePosition), Quaternion.identity);
            
            Transform objectsContainerTransform = transform.Find("Objects");
            if(objectsContainerTransform == null){
                GameObject objectsContainer = new GameObject("Objects");
                objectsContainer.transform.SetParent(transform,false);
                objectsContainerTransform = objectsContainer.transform;
            } 
            worldObject.transform.SetParent(objectsContainerTransform, false); // 'false' keeps world position
            worldObject.name = objectData.objectType.ToString();

            StartCoroutine(AnimateSpawnWorldObject(worldObject,0.2f));

            ChangeAvailability(focusedTilePosition,objectData.objectSize);
        } 
        else 
        {
            return 0;
        }
            
        return 1;
    }

    IEnumerator AnimateSpawnWorldObject(GameObject worldObject, float duration)
    {
        // Squeeze icon (animate scale to zero)
        Vector3 endScale = worldObject.transform.localScale;
        Vector3 startScale = new Vector3(0,0,0);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Lerp position
            worldObject.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

    }

    Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        // add a small y offset to prevent mesh overlap
        return new Vector2Int((int)Math.Floor(worldPosition.x),(int)Math.Floor(worldPosition.z));
    }

    Vector3 GetGridPositionInWorldCoordinates(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x,0.01f,gridPosition.y);
    }

    int CheckAvailability(Vector2Int gridPosition, Vector2Int objectSize)
    {
        bool focusIsOffMap = false;
        bool isOffMap = false;
        bool isObstructed = false;
        bool isFree = false;

        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                if(gridPosition.x + x < 0 || gridPosition.x + x >= 30){
                    if(x==0 && y==0)
                        focusIsOffMap = true;
                    isOffMap = true;
                    continue;
                }
                if(gridPosition.y + y < 0 || gridPosition.y + y >= 30){
                    if(x==0 && y==0)
                        focusIsOffMap = true;
                    isOffMap = true;
                    continue;
                }
                if(!isTileAvailable[gridPosition.x + x,gridPosition.y + y] || terrainLayout[gridPosition.x + x,gridPosition.y + y] == TerrainType.Road) 
                    isObstructed = true;
                else 
                    isFree = true;
            }
        }
        // It's completely off map or the focused position if off map
        if(isOffMap==true && isObstructed==false && isFree==false || focusIsOffMap==true)
            return -1;
        // It's either partially off map or obstructed by roads or other objects
        else if(isOffMap==true || isObstructed==true)
            return 0;
        // It's completely free to use    
        return 1;
    }

    void ChangeAvailability(Vector2Int gridPosition, Vector2Int objectSize)
    {
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            { 
                isTileAvailable[gridPosition.x + x,gridPosition.y + y] = false;
            }
        }
    }
}
