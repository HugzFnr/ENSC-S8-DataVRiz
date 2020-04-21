using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataSphereDisplayer : MonoBehaviour
{
    //stocker l'objet DataLine associé
    private bool isDisplayed;
    private bool isGazedAndToggled;

    //public Material activatedMat; not compatible with quanti

    private DataLine _individual;

    private float lastScaleAsked;

    public DataLine Individual
    {
        get { return _individual; }
        set { _individual = value; }
    }

    void Start()
    {
    }

    public void GazeOn()
    {
        isGazedAndToggled = true;
    }

    public void GazeOff()
    {
        isGazedAndToggled = false;

        if (isDisplayed) //when gaze goes away from the point, it still needs to be scaled up, but its text should is no longer be zoomed
        {
            ChangeScale();
        }
    }

    public void ToggleDisplay(float scale)
    {
        lastScaleAsked = scale;
        if (!isGazedAndToggled)
        {
            if (isDisplayed) HideDetails();
            else DisplayDetails();
        }
    }

    private void DisplayDetails()
    {
        transform.localScale = new Vector3(lastScaleAsked, lastScaleAsked, lastScaleAsked);

        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _individual.ToString();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).transform.localScale = new Vector3(lastScaleAsked,lastScaleAsked,lastScaleAsked);

        //GetComponent<Renderer>().material = activatedMat;

        isDisplayed = true;
    }

    private void HideDetails()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        transform.GetChild(0).gameObject.SetActive(false);
        //GetComponent<Renderer>().material = defaultMat;

        isDisplayed = false;
    }

    public void AdaptDisplay(float scale)
    {
        lastScaleAsked = scale;
        if (isDisplayed) ChangeScale();
    }

    private void ChangeScale()
    {
        transform.localScale = new Vector3(lastScaleAsked, lastScaleAsked, lastScaleAsked);
        transform.GetChild(0).transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
