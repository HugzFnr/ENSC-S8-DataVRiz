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
        GetComponent<Image>().color = Color.white;
        transform.GetChild(0).GetComponent<Text>().text = "Hide " + uiElement.transform.name;
        menuHidden = false;
    }

    void HideUI()
    {
        uiElement.SetActive(false);
        GetComponent<Image>().color = Color.magenta;
        transform.GetChild(0).GetComponent<Text>().text = "Show " + uiElement.transform.name;
        menuHidden = true;

    }
}
