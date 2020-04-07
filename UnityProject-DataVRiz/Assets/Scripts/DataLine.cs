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

    public DataLine(string label, float x, float y, float z)
    {
        Label = label;
        XValue = x;
        YValue = y;
        ZValue = z; 

    }

    public override string ToString()
    {
        return "Label : " + Label
            +"\nX value : " + XValue
            + "\nY value : " + YValue
            + "\nZ value : " + ZValue
            + "\nStandard X : " + XStandardValue
            + "\nStandard Y : " + YStandardValue
            + "\nStandard Z : " + ZStandardValue;
    }

}
