﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


public class PlotPoints : MonoBehaviour
{
    public GameObject PointPrefab, PointHolder, Player, DatasetSelectCanvas, DatasetSelectButton, AxisChoiceCanvas, AxisChoiceButton;
    public bool useStandardizedData = true;
    public int startIndex = 0;
    public Material defaultCubeMat;
    public int maxNumberOfIndividuals = 10000;

    private int[] lastIndexesAsked;
    private List<DataLine> pointsList;
    private List<string> labelsList;
    private List<TextAsset> textAssetsList;
    private List<GameObject> buttonsDataSelect;
    private List<GameObject>[] buttonsAxisChoice;

    private List<Dimension> dimensionsList;


    // Start is called before the first frame update
    void Start()
    {
        textAssetsList = new List<TextAsset>();
        pointsList = new List<DataLine>();
        buttonsDataSelect = new List<GameObject>();
        lastIndexesAsked = new int[4];
        QualiDimension.defaultMat = defaultCubeMat;

        buttonsAxisChoice = new List<GameObject>[4];
        for (int i = 0; i < buttonsAxisChoice.Length; i++)
        {
            buttonsAxisChoice[i] = new List<GameObject>();
        }

        InitTextAssetsList();Debug.Log(textAssetsList.Count);
        InitializeDatasetSelectButtons();
        StartVisualization(startIndex);
        ToggleButtonDataSelect(buttonsDataSelect[startIndex]);
    }

    public void StartVisualization(int datasetIndex)
    {
        ResetVisualization();
        Debug.Log("index demandé : " + datasetIndex);
        ReadDimensions(TextFromIndex(datasetIndex));
        labelsList = TxtReader.GetVariablesLabels(TextFromIndex(datasetIndex));

        lastIndexesAsked = FindFirstDimensions(dimensionsList);
        InitializeAxisChoiceButtons();
        MakeDataLines();        
        PlotPointsFunction(CalculateDefaultScale());
        Player.GetComponent<GazeInteraction2>().PointDefaultScale = CalculateDefaultScale();

    }

    public void ChangeVisualization()
    {
        ResetVisualization();
        MakeDataLines();
        PlotPointsFunction(CalculateDefaultScale());        
    }

    public void ResetVisualization()
    {
        int nb = PointHolder.transform.childCount;
        for (int i = 2; i < nb; i++)
        {
            Transform t = PointHolder.transform.GetChild(i);
            Destroy(t.gameObject);
        }
        Player.GetComponent<GazeInteraction2>().Reset();
    }
    /// <summary>
    /// Calculate a scale, varying from 0.2 to 0.6, based on how many individuals are to be displayed. For 100 points, 0.5 works well; for 1000 points 0.25 works well.
    /// </summary>
    /// <returns></returns>
    private float CalculateDefaultScale()
    {
        QualiDimension labelColumn = (QualiDimension)dimensionsList[0];
        int indivNb = labelColumn.Values.Count;
        float indivNbFloat = (float)indivNb;
        float formula = 1f;
        if (indivNb > 10) formula = 1f / (2 * Mathf.Log((indivNbFloat / 10),10f));
        if (formula < 0.2f) return 0.2f;
        else if (formula > 0.6f) return 0.6f;
        else return formula;
    }

    public void PlotPointsFunction(float pointScale)
    {
        TxtReader.StandardizeData(pointsList); //when mean and SD useful, get them here

        foreach (DataLine d in pointsList)
        {
            float xpos;
            float ypos;
            float zpos;

            if (System.Single.IsNaN(d.XValue) || System.Single.IsNaN(d.YValue) || System.Single.IsNaN(d.ZValue)) //if incorrect line, won't be visible
            {
                xpos = -10f;
                ypos = -10f;
                zpos = -10f;
                UnityEngine.Debug.Log("caught another one !");
            }
            else if (useStandardizedData)
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

            GameObject n = Instantiate(PointPrefab, new Vector3(0,0,0), Quaternion.identity,PointHolder.transform);
            Vector3 reference = PointHolder.transform.position;
            n.transform.position = new Vector3(xpos+reference.x, ypos+reference.y, zpos+reference.z);
            n.transform.name = d.Label;
            n.GetComponent<DataSphereDisplayer>().Individual = d;
            n.GetComponent<Renderer>().material = d.FactorMaterial;
            n.transform.localScale = new Vector3(pointScale,pointScale,pointScale);
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

    public bool IsValidFile(TextAsset ta) //csv and txt are the only data formats both used for statistics and usable via Unity TextAsset
    {
        ////should have used streaming Assets for this to work
        //string pathToFile;
        //pathToFile = Application.dataPath + "/Resources/DataFiles/" + fileName;
        ////Debug.Log(pathToFile);
        //if (File.Exists(pathToFile + ".txt") || File.Exists(pathToFile + ".csv")) return true;
        //else return true;

        int[] infos = TxtReader.CountSummary(ta.text);
        if (infos[2] <= maxNumberOfIndividuals && infos[0] >= 2) return true; //file has to have minimum 2 quanti variables in order to be selectable
        else return false;

    }

    public void InitTextAssetsList()
    {
        Object[] uncheckedList = Resources.LoadAll("DataFiles", typeof(TextAsset));
        foreach (object o in uncheckedList)
        {
            TextAsset t = (TextAsset) o;
            if (IsValidFile(t) && t.text !="") textAssetsList.Add(t);
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
    /// should not be displayed/can not be displayed : in that case quanti are 0 and quali are "", headers are null (default string)
    /// </summary>
    /// <param name="xIndex">Index of dimension to be displayed in X axis</param>
    /// <param name="yIndex">Index of dimension to be displayed in Z axis</param>
    /// <param name="zIndex">Index of dimension to be displayed in Y axis</param>
    /// <param name="qIndex">Index of dimension to be displayed with different materials</param>
    public void MakeDataLines()
    {
        QuantiDimension emptyQuanti = new QuantiDimension();
        QualiDimension emptyQuali = new QualiDimension();

        QuantiDimension qtx, qty, qtz;
        QualiDimension ql;

        int xIndex, yIndex, zIndex, qIndex;
        xIndex = lastIndexesAsked[0];
        yIndex = lastIndexesAsked[1];
        zIndex = lastIndexesAsked[2];
        qIndex = lastIndexesAsked[3];

        pointsList.Clear();
        if (xIndex != -1) qtx = (QuantiDimension)dimensionsList[xIndex];
        else qtx = emptyQuanti;

        if (yIndex != -1) qty = (QuantiDimension)dimensionsList[yIndex];
        else qty = emptyQuanti;

        if (zIndex != -1) qtz = (QuantiDimension)dimensionsList[zIndex];
        else qtz = emptyQuanti;

        Debug.Log("qIndex asked : " + qIndex);
        if (qIndex != -1) ql = (QualiDimension)dimensionsList[qIndex];
        else ql = emptyQuali;

        QualiDimension names = (QualiDimension)dimensionsList[0];

        for (int i = 0; i < qtx.Values.Count; i++)
        { 
            emptyQuanti.Values.Add(0.0f);
            emptyQuali.Values.Add("");

            DataLine d = new DataLine(names.Values[i], qtx.Values[i], qty.Values[i], qtz.Values[i], ql.Values[i]);
            d.XLabel = qtx.Label;
            d.YLabel = qty.Label;
            d.ZLabel = qtz.Label;
            d.QLabel = ql.Label;
            d.FactorMaterial = ql.DifferentiateFactorsColors()[ql.Values[i]];
            pointsList.Add(d);
        }

        NameAxis(xIndex, yIndex, zIndex);
        UpdateVariableSummaries();
    }

    private void UpdateVariableSummaries()
    {
        GameObject XColumn = AxisChoiceCanvas.transform.GetChild(1).gameObject;
        GameObject YColumn = AxisChoiceCanvas.transform.GetChild(2).gameObject;
        GameObject ZColumn = AxisChoiceCanvas.transform.GetChild(3).gameObject;
        GameObject QColumn = AxisChoiceCanvas.transform.GetChild(4).gameObject;

        GameObject[] goTab = new GameObject[] { XColumn, YColumn, ZColumn, QColumn };

        for (int i=0;i<lastIndexesAsked.Length;i++)
        {
            if (lastIndexesAsked[i]!=-1)
            {
                if (i!=3)
                {
                    QuantiDimension qt = (QuantiDimension)dimensionsList[lastIndexesAsked[i]];
                    goTab[i].GetComponent<Text>().text = goTab[i].transform.name
                        + "\nCurrent variable : " + qt.Label 
                        +"\nMean : " + StatsHelper.CalculateMean(qt.Values)
                        + "\n Standard deviation : " + StatsHelper.CalculateSD(qt.Values);
                }
                else
                {
                    QualiDimension ql = (QualiDimension)dimensionsList[lastIndexesAsked[i]];
                    goTab[i].GetComponent<Text>().text = goTab[i].transform.name
                        + "\nCurrent variable : " + ql.Label
                        + "\nNumber of levels : " + ql.DifferentiateFactorsColors().Count;
                }

            }
            else
            {
                goTab[i].GetComponent<Text>().text = goTab[i].transform.name + "\n Current variable : none";
            }
        }    


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

    private void AssignStartVisualizationToButtonEvent(GameObject button, int index)
    {
        button.GetComponent<ButtonActivator>().ClickEvent.AddListener(delegate { StartVisualization(index); });
        button.GetComponent<ButtonActivator>().ClickEvent.AddListener(delegate { ToggleButtonDataSelect(button); });
    }

    private void AssignAxisChangeToButtonEvent(GameObject button, int dimensionIndex, int axisIndex, int internalIndex)
    {
        button.GetComponent<ButtonActivator>().ClickEvent.AddListener(delegate { TryChangeAxis(dimensionIndex, axisIndex, internalIndex); });
    }

    private void TryChangeAxis(int dimensionIndex,int axisIndex, int internalIndex)
    {
        bool hasChanged = false;
        int correspondDimensionIndex = dimensionIndex;
        //check if variable isn't alrdy displayed on another axis
        Debug.Log("intern index : " + dimensionIndex + " axisIndex : " + axisIndex);
        if (axisIndex != 3)
        {
            if (axisIndex == 0 && lastIndexesAsked[1] != correspondDimensionIndex && lastIndexesAsked[2] != correspondDimensionIndex)
            {
                lastIndexesAsked[axisIndex] = correspondDimensionIndex;
                hasChanged = true;
            }
            else if (axisIndex == 1 && lastIndexesAsked[0] != correspondDimensionIndex && lastIndexesAsked[2] != correspondDimensionIndex)
            {
                lastIndexesAsked[axisIndex] = correspondDimensionIndex;
                hasChanged = true;
            }
            else if (axisIndex == 2 && lastIndexesAsked[1] != correspondDimensionIndex && lastIndexesAsked[0] != correspondDimensionIndex)
            {
                lastIndexesAsked[axisIndex] = correspondDimensionIndex;
                hasChanged = true;
            }
            
        }
        else if (axisIndex==3)
        {
            if (lastIndexesAsked[3] != correspondDimensionIndex)
            {
                lastIndexesAsked[3] = correspondDimensionIndex;
                hasChanged = true;
            }
        }
        if (hasChanged)
        {
            ToggleButtonAxisChoice(buttonsAxisChoice[axisIndex][internalIndex],axisIndex);
            ChangeVisualization();
        }
    }

    private void ToggleButtonDataSelect(GameObject button)
    {
        foreach (GameObject g in buttonsDataSelect)
        {
            g.GetComponent<Image>().color = Color.black;
        }
        button.GetComponent<Image>().color = Color.blue;
    }

    private void ToggleButtonAxisChoice(GameObject button, int axisIndex)
    {
        foreach (GameObject g in buttonsAxisChoice[axisIndex])
        {
            g.GetComponent<Image>().color = Color.black;
        }
        button.GetComponent<Image>().color = Color.blue;
    }

    private void InitializeDatasetSelectButtons()
    {
        int i = 0;
        int nbLines = 1+ (textAssetsList.Count / 7); //necessary lines to display the datasets available
        foreach (TextAsset ta in textAssetsList)
        {
            int idxLigne = nbLines - (i / 7) - 1; //top is line nbLines-1;
            int k = i % 7;

            GameObject b = Instantiate(DatasetSelectButton,DatasetSelectCanvas.transform,false);
            b.transform.localPosition = new Vector3(-1000 + (300 * (float)k), 200 + 150 * (float)idxLigne, 0f);

            int[] infos = TxtReader.CountSummary(ta.text); //quanti quali number
            AssignStartVisualizationToButtonEvent(b, i);
            b.transform.name = "Button" + ta.name;
            b.transform.GetChild(0).GetComponent<Text>().text = $"{ta.name} " +
                $"\n{infos[2]} individuals" +
                $"\n{infos[0]} quantitative variable(s)" +
                $"\n{infos[1]} qualitative variable(s)";
            b.SetActive(true);

            buttonsDataSelect.Add(b);

            i++;
        }
    }

    private void InitializeAxisChoiceButtons()
    {
        foreach(List<GameObject> l in buttonsAxisChoice)
        {
            l.Clear();
        }

        GameObject menu = AxisChoiceCanvas;
        Transform XColumn = AxisChoiceCanvas.transform.GetChild(1);
        Transform YColumn = AxisChoiceCanvas.transform.GetChild(2);
        Transform ZColumn = AxisChoiceCanvas.transform.GetChild(3);
        Transform QColumn = AxisChoiceCanvas.transform.GetChild(4);
        Transform[] columnTab = new Transform[] { XColumn, YColumn, ZColumn, QColumn };

        foreach (Transform tparent in columnTab)
        {
            foreach (Transform child in tparent)
            {
                Destroy(child.gameObject);
            }
        }

        int t = 0;
        int cptQt = 0;
        int cptQl = 0;
        foreach (Dimension d in dimensionsList)
        {
            if (t != 0) //skip headerDimension
            {
                if (d is QuantiDimension)
                {
                    QuantiDimension qd = (QuantiDimension)d;
                    for (int iter = 0; iter < 3; iter++)
                    {
                        GameObject b = Instantiate(AxisChoiceButton, columnTab[iter].transform, false);
                        b.transform.localPosition = new Vector3(0f, 100f + (float)cptQt * 75, 0);
                        b.transform.name = qd.Label;
                        b.transform.GetChild(0).GetComponent<Text>().text = qd.Label;

                        if (lastIndexesAsked[iter] == t) ToggleButtonAxisChoice(b, iter); //highlight selected axis at start
                        AssignAxisChangeToButtonEvent(b, t, iter, cptQt);
                        b.SetActive(true);
                        buttonsAxisChoice[iter].Add(b);
                    }
                    cptQt++;
                }
                else if (d is QualiDimension)
                {
                    QualiDimension ql = (QualiDimension)d;

                    GameObject b2 = Instantiate(AxisChoiceButton, columnTab[3].transform, false);
                    b2.transform.localPosition = new Vector3(0f, 100f + (float)cptQl * 75, 0);
                    b2.transform.name = ql.Label;
                    b2.transform.GetChild(0).GetComponent<Text>().text = ql.Label;

                    if (lastIndexesAsked[3] == t) ToggleButtonAxisChoice(b2, 3);
                    AssignAxisChangeToButtonEvent(b2, t, 3,cptQl);
                    b2.SetActive(true);
                    buttonsAxisChoice[3].Add(b2);

                    cptQl++;
                }
            }
            t++;
        }

        //assign infos for text
     }

}
