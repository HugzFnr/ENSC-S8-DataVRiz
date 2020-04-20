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
    private Transform lastButtonTransform = null;
    private float adaptativeScale;

    private float _pointDefaultScale;
    public float PointDefaultScale
    {
        get { return _pointDefaultScale; }
        set { _pointDefaultScale = value; }
    }

     

    private List<Transform> activatedSpheres;

    public bool isLookingAtButton
    {
        get { return _isLookingAtButton; }
        set
        {
            _isLookingAtButton = value;
        }
    }

    void Start()
    {
        activatedSpheres = new List<Transform>();
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

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)); //creates a ray starting at the center of the camera
        if (Physics.Raycast(ray, out _hit, distanceOfRay))
        {
            if (_hit.transform.CompareTag("DataPoint"))
            {
                adaptativeScale = CalculateAdaptativeScale(_hit.transform); //distance goes from roughly 1 to 15 for data spheres viewed from the plane

                GvrOn(false);
                _hit.transform.localScale = new Vector3(adaptativeScale, adaptativeScale, adaptativeScale); //goes from 0.7f when very close, up to 5f when on the platform's edges
                _hit.transform.GetChild(0).transform.LookAt(transform.position);
                if (_hit.transform != lastSphereGazed) LeaveSphere();
                lastSphereGazed = _hit.transform;
            }

            if (imgCircle.fillAmount == 1 && _hit.transform.CompareTag("DataPoint") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<DataSphereDisplayer>().ToggleDisplay(adaptativeScale);
                _hit.transform.gameObject.GetComponent<DataSphereDisplayer>().GazeOn();
                activatedSpheres.Add(_hit.transform);
                gvrStatus = false;
            }

            if (!(_hit.transform.CompareTag("DataPoint") || _hit.transform.CompareTag("Button")))
            {
                GvrOff();
                if (lastSphereGazed != null)
                {
                    LeaveSphere();
                }

            }             

        }
        else if (!isLookingAtButton)
        {
            LeaveSphere();
        }

        if (isLookingAtButton && imgCircle.fillAmount == 1 && gvrStatus)
        {
            Debug.Log("did a thing!");
            if (lastButtonTransform != null) lastButtonTransform.gameObject.GetComponent<ButtonActivator>().DoTheThing();
            gvrStatus = false;
        }

    }

    private void LeaveSphere()
    {
        GvrOff();
        if (lastSphereGazed != null)
        {
            lastSphereGazed.localScale = new Vector3(PointDefaultScale, PointDefaultScale, PointDefaultScale);
            lastSphereGazed.gameObject.GetComponent<DataSphereDisplayer>().GazeOff();
        }
    }

    public void GvrOnButton(Transform button)
    {
        lastButtonTransform = button;
        GvrOn(true);
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

    private float CalculateAdaptativeScale (Transform viewedTransform)
    {
        float distanceToObject = Vector3.Distance(viewedTransform.position, transform.position);
        return System.Math.Max(0.7f, distanceToObject / 2.5f);                //distance goes from roughly 1 to 15
    }

    public void AdaptSpheresDisplays()
    {
        foreach (Transform t in activatedSpheres)
        {
            if (t.CompareTag("DataPoint"))
            {
                t.GetChild(0).LookAt(transform.position);
                t.GetComponent<DataSphereDisplayer>().AdaptDisplay(CalculateAdaptativeScale(t));
            }
        }
    }

    public void Reset()
    {
        activatedSpheres.Clear();
        lastSphereGazed = null;
        lastButtonTransform = null;
}
}
