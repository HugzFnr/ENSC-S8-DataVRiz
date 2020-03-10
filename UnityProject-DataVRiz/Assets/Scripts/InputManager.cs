using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputManager : MonoBehaviour
{
    List<XRNodeState> list;
    string[] joysticks;
    // Start is called before the first frame update
    void Start()
    {
        //list = new List<XRNodeState>();
        //InputTracking.GetNodeStates(list);
        //foreach (XRNodeState node in list)
        //{
        //    Debug.Log(node.uniqueID);
        //}*
        //joysticks = new string[2];
        //joysticks = Input.GetJoystickNames();
        //foreach (string j in joysticks) Debug.Log(j);


    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 display = new Vector3();
        //foreach (XRNodeState node in list)
        //{
        //    node.TryGetAcceleration(out display);
        //    Debug.Log("Accel node " + display);

        //}
        //if (Input.GetButtonDown("Button.Three")) Application.Quit();

        if (Input.GetButton("Fire1"))
        {
            Debug.Log("OUII");
        }

        if (Input.GetKey("3")) //marche pas
        {
            Debug.Log("OUII");
        }

        foreach (string s in Input.GetJoystickNames())
        {
            Debug.Log(s);
        }
    }
}
