using System;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject parentObject; // Assign this in the Inspector or find it in code
    public Material planeMaterial1;
    public Material planeMaterial2;
    private Plane fullPlane;
    public GameObject m_Cube;
    private bool[,] isTileAvailable = new bool[30, 30];

    void Start()
    {
        fullPlane = new Plane( new Vector3(0,1,0), new Vector3(0,0.01f,0) );

        // -------------------------------------------
        if (parentObject == null)
        {
            Debug.LogError("Parent Object is not assigned!");
            return;
        }

        for(int i =0; i < 30; i+=2)
        {
            for(int j =0; j < 30; j+=2)
            {
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
        
    }

    public void PlaceWorldObject(GameObject worldObjectPrefab)
    {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Initialise the enter variable
        float enter = 0.0f;
        
        if (fullPlane.Raycast(ray, out enter))
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            
            // Check if its within city bounds and snap to grid to get tile(s) position(s)
            Vector3 mouseWorldPosition = hitPoint;

            // Check weather prefab fits into the selected tile(s)
            // ...

            // Instantiante world object
            Instantiate(worldObjectPrefab, mouseWorldPosition, Quaternion.identity);
        }
    }
}
