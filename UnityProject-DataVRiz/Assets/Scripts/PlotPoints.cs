using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotPoints : MonoBehaviour
{
    public GameObject PointPrefab;
    public string dataFile;
    public GameObject PointHolder;

    private List<DataLine> pointsList;
    // Start is called before the first frame update
    void Start()
    {
        pointsList = TxtReader.Read("Assets/Resources/" + dataFile + ".txt");
        foreach (DataLine d in pointsList)
        {
            GameObject n = Instantiate(PointPrefab, new Vector3(d.XValue, d.YValue, d.ZValue), Quaternion.identity);
            n.transform.parent = PointHolder.transform;
            n.transform.name = d.Label;
        }
    }


}
