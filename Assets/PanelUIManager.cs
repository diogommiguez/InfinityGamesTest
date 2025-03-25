using UnityEngine;

public class PanelUIManager : MonoBehaviour
{
    private bool showPanel = true;
    public GameObject panelBody;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void closePanel()
    {
        showPanel = !showPanel;

        if(panelBody == null)
        {
            Debug.LogError("No panel body found. Check name.");
        }
        else
        {
            panelBody.SetActive(showPanel);
        }
    }
}
