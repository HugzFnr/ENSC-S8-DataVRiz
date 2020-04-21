using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMenuButton : MonoBehaviour
{
    private bool menuHidden = false;

    public GameObject uiElement;

    public void ToggleUI()
    {
        if (menuHidden)
        {
            ShowUI();
        }
        else HideUI();
    }

    void ShowUI()
    {
        uiElement.SetActive(true);
        GetComponent<Image>().color = Color.black;
        transform.GetChild(0).GetComponent<Text>().text = "Hide " + uiElement.transform.name;
        menuHidden = false;
    }

    void HideUI()
    {
        uiElement.SetActive(false);
        GetComponent<Image>().color = new Color(0.3f, 0f,0.4f,1f);
        transform.GetChild(0).GetComponent<Text>().text = "Show " + uiElement.transform.name;
        menuHidden = true;
    }
}
