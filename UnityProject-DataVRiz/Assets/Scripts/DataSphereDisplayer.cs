using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataSphereDisplayer : MonoBehaviour
{
    //stocker l'objet DataLine associé


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayTest()
    {
        Debug.Log(transform.name);
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = transform.name;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
