using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputManager : MonoBehaviour
{
    string[] joysticks;

    public float playerSpeed = 3.5f;
    public bool canFly=true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Quit"))
        {
            Application.Quit();
            Debug.Log("tentative de quit");
        }

        if (Input.GetButton("Move"))
        {
            transform.position += Camera.main.transform.forward * playerSpeed * Time.deltaTime;
            //use the canFly accordingly
        }

        //foreach (string s in Input.GetJoystickNames())
        //{
        //    Debug.Log(s);
        //}
    }
}
