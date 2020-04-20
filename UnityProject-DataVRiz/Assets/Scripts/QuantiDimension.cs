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

    public static QuantiDimension LabelColumn(int size)
    {
        QuantiDimension labelDimension = new QuantiDimension();
        for (int i = 1; i <= size; i++)
        {
            labelDimension.Values.Add(i);
        }
        labelDimension.Label = "Quanti header column";

        return labelDimension;
    }

}
