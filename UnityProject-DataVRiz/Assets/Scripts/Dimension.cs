using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimension : MonoBehaviour
{
    private bool _isQualitative;

    public bool IsQualitative
    {
        get { return _isQualitative; }
        set { _isQualitative = value; }
    }

    private List<float> _values;

    public List<float> Values
    {
        get { return _values; }
        set { _values = value; }
    }

    private string _label;

    public string Label
    {
        get { return _label; }
        set { _label = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
