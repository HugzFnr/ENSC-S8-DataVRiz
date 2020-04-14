using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlotPoints : MonoBehaviour
{
    public GameObject PointPrefab;
    public string dataFileName;
    public GameObject PointHolder;
    public bool useStandardizedData = true;

    private List<DataLine> pointsList;
    private List<string> labelsList;
    // Start is called before the first frame update
    void Start()
    {
        labelsList = TxtReader.GetVariablesLabels(LoadTextFromAsset("DataFiles/" + dataFileName));
        PlotPointsFunction();
        NameAxis();
    }

    public void PlotPointsFunction()
    {
        //Debug.Log(dataFileName);
        pointsList = TxtReader.Read(LoadTextFromAsset("DataFiles/" + dataFileName));
        TxtReader.StandardizeData(pointsList); //when mean and SD useful, get them here

        foreach (DataLine d in pointsList)
        {
            float xpos;
            float ypos;
            float zpos;

            if (useStandardizedData)
            {
                xpos = d.XStandardValue;
                ypos = d.YStandardValue;
                zpos = d.ZStandardValue;
            }
            else
            {
                xpos = d.XValue;
                ypos = d.YValue;
                zpos = d.XValue;
            }
            //GameObject n = new GameObject();
            //n.transform.parent = PointHolder.transform;
            GameObject n = Instantiate(PointPrefab, new Vector3(0,0,0), Quaternion.identity,PointHolder.transform);
            Vector3 reference = PointHolder.transform.position;
            n.transform.position = new Vector3(xpos+reference.x, ypos+reference.y, zpos+reference.z);
            n.transform.name = d.Label;
            d.XLabel = labelsList[0];
            d.YLabel = labelsList[1];
            d.ZLabel = labelsList[2];
            n.GetComponent<DataSphereDisplayer>().Individual = d;
        }
    }

    public void NameAxis()
    {        
        foreach (string s in labelsList) Debug.Log(s);

        Transform axisNames = PointHolder.transform.GetChild(1);
        axisNames.transform.GetChild(0).gameObject.GetComponent<Text>().text = "X : " + labelsList[0];
        axisNames.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Y : " + labelsList[1];
        axisNames.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Z : " + labelsList[2];

    }

    public string LoadTextFromAsset(string path)
    {
        var textAsset = Resources.Load<TextAsset>(path);
        //Debug.Log(textAsset);
        if (textAsset != null) return textAsset.text;
        else throw new System.Exception("Data asset could not be loaded at path : " + path);
    }


}
