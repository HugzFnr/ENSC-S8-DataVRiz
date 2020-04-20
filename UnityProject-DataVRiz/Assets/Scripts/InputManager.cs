using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputManager : MonoBehaviour
{
    public float playerSpeed = 3.5f;
    public bool canFly=true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //For Occulus
        if (Input.GetButtonDown("Quit"))
        {
            Application.Quit();
            Debug.Log("tentative de quit");
        }

        if (Input.GetButton("Move"))
        {
            transform.position += Camera.main.transform.forward * playerSpeed * Time.deltaTime;
            GetComponent<GazeInteraction2>().AdaptSpheresDisplays();
        }

        //For cardboard smartphone
        if (Input.touches.Length >= 2) Application.Quit();
        foreach (Touch t in Input.touches) //if you keep the cardboard button pressed, you keep on moving in front of you
        {
            if (t.phase ==TouchPhase.Moved || t.phase==TouchPhase.Stationary) transform.position += Camera.main.transform.forward * playerSpeed * Time.deltaTime;
            if (t.phase == TouchPhase.Ended) GetComponent<GazeInteraction2>().AdaptSpheresDisplays();
        }
    }
}
