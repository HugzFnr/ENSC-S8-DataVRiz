using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonActivator : MonoBehaviour
{
    public UnityEvent ClickEvent;
    
    public void DoTheThing()
    {
        ClickEvent.Invoke();
    }
}
