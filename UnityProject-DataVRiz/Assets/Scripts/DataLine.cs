using System.Collections;
using System.Collections.Generic;


public class DataLine
{
    private string _label;

    public string Label
    {
        get { return _label; }
        set { _label = value; }
    }

    //values from dataset

    private float _xValue;

    public float XValue
    {
        get { return _xValue; }
        set { _xValue = value; }
    }

    private float _yValue;

    public float YValue
    {
        get { return _yValue; }
        set { _yValue = value; }
    }

    private float _zValue;

    public float ZValue
    {
        get { return _zValue; }
        set { _zValue = value; }
    }

    private string _qValue;

    public string QValue
    {
        get { return _qValue; }
        set { _qValue = value; }
    }

    //standard values
    
    private float _xStandardValue;

    public float XStandardValue
    {
        get { return _xStandardValue; }
        set { _xStandardValue = value; }
    }

    private float _yStandardValue;

    public float YStandardValue
    {
        get { return _yStandardValue; }
        set { _yStandardValue = value; }
    }

    private float _zStandardValue;

    public float ZStandardValue
    {
        get { return _zStandardValue; }
        set { _zStandardValue = value; }
    }
    
    //labels for each dimension

    private string _xLabel;

    public string XLabel
    {
        get { return _xLabel; }
        set { _xLabel=value; }
    }

    private string _yLabel;

    public string YLabel
    {
        get { return _yLabel; }
        set { _yLabel=value; }
    }

    private string _zLabel;

    public string ZLabel
    {
        get { return _zLabel; }
        set { _zLabel=value; }
    }

    private string _qLabel;

    public string QLabel
    {
        get { return _qLabel; }
        set { _qLabel = value; }
    }

    public DataLine(string label, float x, float y, float z, string q)
    {
        Label = label;
        XValue = x;
        YValue = y;
        ZValue = z;
        QValue = q;

    }

    public override string ToString()
    {
        return "Individual's label : " + Label
            + "\n" + XLabel + " : " + XValue
            + "\n" + YLabel + " : " + YValue
            + "\n" + ZLabel + " : " + ZValue
            + "\n" + QLabel + " : " + QValue
            + "\nStandard X : " + XStandardValue
            + "\nStandard Y : " + YStandardValue
            + "\nStandard Z : " + ZStandardValue;
    }

}
