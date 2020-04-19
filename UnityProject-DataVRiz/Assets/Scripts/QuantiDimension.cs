using System.Collections;
using System.Collections.Generic;

public class QuantiDimension : Dimension
{
    private List<float> _values;

    public List<float> Values
    {
        get { return _values; }
        set { _values = value; }
    } 

    public QuantiDimension()
    {
        Values = new List<float>();
    }

}
