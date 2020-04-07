using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataSphereDisplayer : MonoBehaviour
{
    //stocker l'objet DataLine associé
    private bool isDisplayed;
    private bool isGazedAndToggled;

    private Material defaultMat;
    public Material activatedMat;

    public DataLine _dataLine;

    // Start is called before the first frame update
    void Start()
    {
        defaultMat = GetComponent<Renderer>().material;
    }

    public void GazeOn()
    {
        isGazedAndToggled = true;

    }

    public void GazeOff()
    {
        isGazedAndToggled = false;
    }

    public void ToggleDisplay(float scale)
    {
        if (!isGazedAndToggled)
        {
            if (isDisplayed) HideDetails();
            else DisplayDetails(scale);
        }
    }

    public void DisplayDetails(float scale)
    {
        Debug.Log(transform.name);
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _dataLine.ToString();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).transform.localScale = new Vector3(scale,scale,scale);

        GetComponent<Renderer>().material = activatedMat;

        isDisplayed = true;
    }

    public void HideDetails()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Renderer>().material = defaultMat;

        isDisplayed = false;
    }
}
