using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Events;

public class GazeInteraction2 : MonoBehaviour
{
    public Image imgCircle;
    //public UnityEvent gvrClick;
    public float totalTime = 2f;
    bool gvrStatus;
    public float gvrTimer;

    public int distanceOfRay = 10;
    private RaycastHit _hit;

    private bool _isLookingAtButton;

    public bool isLookingAtButton
    {
        get { return _isLookingAtButton; }
        set
        {
            _isLookingAtButton = value;
            Debug.Log("prop says : " + _isLookingAtButton);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gvrStatus)
        {
            gvrTimer += Time.deltaTime;
            imgCircle.fillAmount = gvrTimer / totalTime;
        }

        //if (gvrTimer > totalTime)
        //{
        //    gvrClick.Invoke();
        //}

        Debug.Log("frame");
        Debug.Log(isLookingAtButton);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out _hit, distanceOfRay))
        {
            if (_hit.transform.CompareTag("DataPoint"))
            {
                GvrOn(false);
            }

            if (imgCircle.fillAmount == 1 && _hit.transform.CompareTag("DataPoint"))
            {
                _hit.transform.gameObject.GetComponent<DataSphereDisplayer>().DisplayTest();
                //GvrOff();
            }
        }
        else if (!isLookingAtButton)
        {
            GvrOff();
        }

    }

    public void GvrOn(bool isButton)
    {
        isLookingAtButton = isButton;
        gvrStatus = true;
    }

    public void GvrOff()
    {
        gvrStatus = false;
        gvrTimer = 0;
        imgCircle.fillAmount = 0;
    }
}
