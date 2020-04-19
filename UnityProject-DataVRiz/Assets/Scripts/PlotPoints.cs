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

    private List<Dimension> dimensionsList;
    //private List<QuantiDimension> quantisList;
    //private List<QualiDimension> qualisList;

    // Start is called before the first frame update
    void Start()
    {
        textAssetsList = new List<TextAsset>();
        pointsList = new List<DataLine>();

        InitTextAssetsList();
        TxtReader.CountSummary(TextFromIndex(startIndex)); //for debug purposes
        StartVisualization(startIndex);
    }

    public void StartVisualization(int datasetIndex)
    {
        ResetVisualization();
        ReadDimensions(TextFromIndex(datasetIndex));
        labelsList = TxtReader.GetVariablesLabels(TextFromIndex(datasetIndex));
        MakeDataLines(1, 2, 4, 5);
        PlotPointsFunction();
        NameAxis(1,2,4);

        //List<Dimension> dimensions = TxtReader.Read(TextFromIndex(datasetIndex));
        //QualiDimension qt = (QualiDimension)dimensions[0];
        //Debug.Log(qt.Label);
        //foreach (string f in qt.Values)
        //{
        //    Debug.Log(f);
        //}
    }

    public void PlotPointsFunction()
    {

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
            n.GetComponent<DataSphereDisplayer>().Individual = d;
        }
    }

    public void NameAxis(int xIndex, int yIndex, int zIndex)
    {        
        Transform axisNames = PointHolder.transform.GetChild(1);
        axisNames.transform.GetChild(0).gameObject.GetComponent<Text>().text = "X : " + labelsList[xIndex-1];
        axisNames.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Y : " + labelsList[yIndex-1];
        axisNames.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Z : " + labelsList[zIndex-1];

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

    private void ReadDimensions(string text)
    {
        dimensionsList = TxtReader.Read(text);
    }

    public void MakeDataLines(int xIndex, int yIndex, int zIndex, int qIndex)
    {
        pointsList.Clear();
        QuantiDimension qtx = (QuantiDimension)dimensionsList[xIndex];
        QuantiDimension qty = (QuantiDimension)dimensionsList[yIndex];
        QuantiDimension qtz = (QuantiDimension)dimensionsList[zIndex];
        QualiDimension ql = (QualiDimension)dimensionsList[qIndex];
        QualiDimension names = (QualiDimension)dimensionsList[0];

        for (int i = 0; i < qtx.Values.Count; i++)
        {
            DataLine d = new DataLine(names.Values[i], qtx.Values[i], qty.Values[i], qtz.Values[i], ql.Values[i]);
            d.XLabel = qtx.Label;
            d.YLabel = qty.Label;
            d.ZLabel = qtz.Label;
            d.QLabel = ql.Label;
            pointsList.Add(d);
        }
    }

}
