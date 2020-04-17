using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


public class PlotPoints : MonoBehaviour
{
    public GameObject PointPrefab;
    public string dataFileName;
    public GameObject PointHolder;
    public bool useStandardizedData = true;
    public int startIndex = 0;

    private List<DataLine> pointsList;
    private List<string> labelsList;
    private List<TextAsset> textAssetsList;
    // Start is called before the first frame update
    void Start()
    {
        textAssetsList = new List<TextAsset>();
        InitTextAssetsList();
        TxtReader.CountSummary(TextFromIndex(startIndex));
        StartVisualization(startIndex);
    }

    public void StartVisualization(int datasetIndex)
    {
        ResetVisualization();
        labelsList = TxtReader.GetVariablesLabels(TextFromIndex(datasetIndex));
        PlotPointsFunction(datasetIndex);
        NameAxis();
    }

    public void PlotPointsFunction(int datasetIndex)
    {
        //Debug.Log(dataFileName);
        pointsList = TxtReader.Read(TextFromIndex(datasetIndex));
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
        if (textAsset != null) return textAsset.text;
        else throw new System.Exception("Data asset could not be loaded at path : " + path);
    }

    public bool IsValidFile(string fileName) //csv and txt are the only data formats both used for statistics and usable via Unity TextAsset
    {
        ////should have used streaming Assets for this to work
        //string pathToFile;
        //pathToFile = Application.dataPath + "/Resources/DataFiles/" + fileName;
        ////Debug.Log(pathToFile);
        //if (File.Exists(pathToFile + ".txt") || File.Exists(pathToFile + ".csv")) return true;
        //else return true;

        return true;
        
        //check dimensions via txtReader??
    }

    public void InitTextAssetsList()
    {
        Object[] uncheckedList = Resources.LoadAll("DataFiles", typeof(TextAsset));
        foreach (object o in uncheckedList)
        {
            TextAsset t = (TextAsset) o;
            if (IsValidFile(t.name) && t.text !="") textAssetsList.Add(t);            
        }
    }

    public void ResetVisualization()
    {
        int nb = PointHolder.transform.childCount;
        for (int i=2; i<nb;i++)
        {
            Transform t = PointHolder.transform.GetChild(i);
            Destroy(t.gameObject);
        }
    }

    private string TextFromIndex(int index)
    {
        return textAssetsList[index].text;
    }

}
