using UnityEngine;
using UnityEngine.UI;

public class CategoryUI : MonoBehaviour
{
    public GameObject categoryPanel; 
    public Button categoryButton;    
    public Button closePanelButton;       

    void Start()
    {
    
        categoryPanel.SetActive(false);

       
        categoryButton.onClick.AddListener(ShowCategoryPanel);
        closePanelButton.onClick.AddListener(HideCategoryPanel);
    }

   
    void ShowCategoryPanel()
    {
        categoryPanel.SetActive(true);
    }


    void HideCategoryPanel()
    {
        categoryPanel.SetActive(false);
    }
}