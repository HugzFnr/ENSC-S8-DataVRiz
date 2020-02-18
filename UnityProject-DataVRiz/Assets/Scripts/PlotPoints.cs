using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlotPoints : MonoBehaviour
{
    public GameObject PointPrefab;
    public string dataFileName;
    public GameObject PointHolder;

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

        foreach (DataLine d in pointsList)
        {
            GameObject n = Instantiate(PointPrefab, new Vector3(d.XValue, d.YValue, d.ZValue), Quaternion.identity);
            n.transform.parent = PointHolder.transform;
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
