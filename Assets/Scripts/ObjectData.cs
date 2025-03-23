using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public GameObject objectPrefab; // Assign a 3D prefab in the Inspector
    public ObjectType objectType;
    public Vector2Int objectSize;
}
