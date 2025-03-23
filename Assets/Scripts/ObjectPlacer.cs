using System;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public Material freeTileMaterial;
    public Material occupiedTileMaterial;
    public Material grassMaterial;
    public Material roadMaterial;
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
                terrainLayout[i, j] = TerrainType.Grass;
                if(j == 6) terrainLayout[i, j] = TerrainType.Road;

                isTileAvailable[i,j] = true;
            }
        }
        CreateCityTayout();
    }

    void CreateCityTayout()
    {
        GameObject cityLayout = new GameObject("City Layout");
        cityLayout.transform.SetParent(transform,false);

        for(int i =0; i < 30; i++)
        {
            for(int j =0; j < 30; j++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                tile.transform.SetParent(cityLayout.transform,false);
                tile.transform.localScale = new Vector3(0.1f, 1, 0.1f);
                tile.transform.localPosition = new Vector3(i+0.5f,0.01f,j+0.5f);
                switch(terrainLayout[i,j]){
                    case TerrainType.Grass:
                        tile.name = "Grass(" + i.ToString() + "," + j.ToString() + ")";
                        tile.GetComponent<Renderer>().material = grassMaterial;
                        break;
                    case TerrainType.Road:
                        tile.name = "Road(" + i.ToString() + "," + j.ToString() + ")";
                        tile.GetComponent<Renderer>().material = roadMaterial;
                        break;
                }
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
        placementPlane.transform.localPosition = new Vector3(focusedTilePosition.x + objectSize.x/2.0f , 0.1f ,focusedTilePosition.y + objectSize.y/2.0f);

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
            
            GameObject objectsParent = GameObject.Find("Objects");
            if(objectsParent == null){
                objectsParent = new GameObject("Objects");
            } 
            worldObject.transform.SetParent(objectsParent.transform, false); // 'false' keeps world position
            worldObject.name = objectData.objectType.ToString();

            ChangeAvailability(focusedTilePosition,objectData.objectSize);
        } 
        else 
        {
            return 0;
        }
            
        return 1;
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
