using UnityEngine;
using UnityEngine.UI;

public class ToggleSpriteSwitcher : MonoBehaviour
{
    public Sprite showPanelSprite;
    public Sprite hidePanelSprite;
    public Image toggleSpriteImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void SwitchToggleSprite()
    {
        if(gameObject.GetComponent<Toggle>().isOn == true)
        {
            toggleSpriteImage.sprite = hidePanelSprite;
        } 
        else
        {
            toggleSpriteImage.sprite = showPanelSprite;
        }
    }

}
