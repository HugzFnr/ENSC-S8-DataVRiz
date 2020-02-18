using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlotPoints : MonoBehaviour
{
    public GameObject PointPrefab;
    public string dataFileName;
    public GameObject PointHolder;
    public bool useStandardizedData = true;

    private List<DataLine> pointsList;
    // Start is called before the first frame update
    void Start()
    {
        PlotPointsFunction();
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
        }
    }

    public string LoadTextFromAsset(string path)
    {
        var textAsset = Resources.Load<TextAsset>(path);
        //Debug.Log(textAsset);
        if (textAsset != null) return textAsset.text;
        else throw new System.Exception("Data asset could not be loaded at path : " + path);
    }


}
