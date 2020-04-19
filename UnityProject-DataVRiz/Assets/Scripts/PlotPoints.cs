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

        int[] firstDims = FindFirstDimensions(dimensionsList);
        MakeDataLines(firstDims[0],firstDims[1],firstDims[2],firstDims[3]);        
        PlotPointsFunction();
 
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
                zpos = d.ZValue;
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
        if (xIndex != -1) axisNames.transform.GetChild(0).gameObject.GetComponent<Text>().text = "X : " + labelsList[xIndex - 1];
        else axisNames.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Unused X axis";

        if (yIndex != -1) axisNames.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Y : " + labelsList[yIndex - 1];
        else axisNames.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Unused Y axis";

        if (zIndex != -1) axisNames.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Z : " + labelsList[zIndex - 1];
        else axisNames.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Unused Z axis";

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
            Debug.Log(t.name);
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

    /// <summary>
    /// Initialize the pointsList with newly created DataLines corresponding to dimensionsList. If an index is ==-1, dimension
    /// should not be displayed/can not be displayed : in that case quanti are 0 and labels are null (default)
    /// </summary>
    /// <param name="xIndex">Index of dimension to be displayed in X axis</param>
    /// <param name="yIndex">Index of dimension to be displayed in Z axis</param>
    /// <param name="zIndex">Index of dimension to be displayed in Y axis</param>
    /// <param name="qIndex">Index of dimension to be displayed with different materials</param>
    public void MakeDataLines(int xIndex, int yIndex, int zIndex, int qIndex)
    {
        QuantiDimension emptyQuanti = new QuantiDimension();
        QualiDimension emptyQuali = new QualiDimension();

        QuantiDimension qtx, qty, qtz;
        QualiDimension ql;

        pointsList.Clear();
        if (xIndex != -1) qtx = (QuantiDimension)dimensionsList[xIndex];
        else qtx = emptyQuanti;

        if (yIndex != -1) qty = (QuantiDimension)dimensionsList[yIndex];
        else qty = emptyQuanti;

        if (zIndex != -1) qtz = (QuantiDimension)dimensionsList[zIndex];
        else qtz = emptyQuanti;

        if (qIndex != -1) ql = (QualiDimension)dimensionsList[qIndex];
        else ql = emptyQuali;

        QualiDimension names = (QualiDimension)dimensionsList[0];

        for (int i = 0; i < qtx.Values.Count; i++)
        { 
            emptyQuanti.Values.Add(0.0f);
            emptyQuali.Values.Add(null);

            DataLine d = new DataLine(names.Values[i], qtx.Values[i], qty.Values[i], qtz.Values[i], ql.Values[i]);
            d.XLabel = qtx.Label;
            d.YLabel = qty.Label;
            d.ZLabel = qtz.Label;
            d.QLabel = ql.Label;
            pointsList.Add(d);
        }

        NameAxis(xIndex, yIndex, zIndex);
    }
    /// <summary>
    /// Finds (and check if exists) first 3 quanti dimensions and first quali variables in the dimensionsList
    /// </summary>
    /// <returns> array is in order : first quanti, second quanti, third quanti, first quali, -1 if none found </returns>
    private int[] FindFirstDimensions(List<Dimension> ld)
    {
        int quanti1 = -1;
        int quanti2 = -1;
        int quanti3 = -1;
        int quali = -1;

        for (int i = 1; i < ld.Count; i++)
        {
            if (ld[i] is QuantiDimension)
            {
                if (quanti1 == -1) quanti1 = i;
                else if (quanti2 == -1) quanti2 = i;
                else if (quanti3 == -1) quanti3 = i;
            }
            else if (ld[i] is QualiDimension && quali==-1)
            {
                quali = i;
            }
        }

        return (new int[] { quanti1, quanti2, quanti3, quali });
    }

}
