using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Events;

public class GazeInteraction2 : MonoBehaviour
{
    public GvrReticlePointer scriptGvrReticlePointer;

    public Image imgCircle;
    //public UnityEvent gvrClick;
    public float totalTime = 2f;
    bool gvrStatus;
    public float gvrTimer;

    public int distanceOfRay = 20;
    private RaycastHit _hit;

    private bool _isLookingAtButton;
    private Transform lastSphereGazed=null;
    private float adaptativeScale;

    public bool isLookingAtButton
    {
        get { return _isLookingAtButton; }
        set
        {
            _isLookingAtButton = value;
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

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //creates a ray starting at the center of the camera
        if (Physics.Raycast(ray, out _hit, distanceOfRay))
        {
            if (_hit.transform.CompareTag("DataPoint"))
            {
                float distanceToSphere = Vector3.Distance(_hit.transform.position, transform.position);
                adaptativeScale = System.Math.Max(0.7f, distanceToSphere / 2.5f);                //distance goes from roughly 1 to 15

                GvrOn(false);
                _hit.transform.localScale = new Vector3(adaptativeScale, adaptativeScale, adaptativeScale); //should be 0.7f when very close, up to 5f when on the platform's edges
                _hit.transform.GetChild(0).transform.LookAt(transform.position);    
                lastSphereGazed = _hit.transform;
            }

            if (imgCircle.fillAmount == 1 && _hit.transform.CompareTag("DataPoint"))
            {
                _hit.transform.gameObject.GetComponent<DataSphereDisplayer>().ToggleDisplay(adaptativeScale);
                _hit.transform.gameObject.GetComponent<DataSphereDisplayer>().GazeOn();
                //GvrOff();
            }

            if (!(_hit.transform.CompareTag("DataPoint") || _hit.transform.CompareTag("Button")))
            {
                GvrOff();
                if (lastSphereGazed != null)
                {
                    lastSphereGazed.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    lastSphereGazed.gameObject.GetComponent<DataSphereDisplayer>().GazeOff();
                }

            }             

        }
        else if (!isLookingAtButton)
        {
            GvrOff();
            if (lastSphereGazed != null)
            {
                lastSphereGazed.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                lastSphereGazed.gameObject.GetComponent<DataSphereDisplayer>().GazeOff();
            }
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
