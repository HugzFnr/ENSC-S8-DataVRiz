using System.Collections;
using System.Collections.Generic;

public class QualiDimension : Dimension
{
    private List<string> _values;

    public List<string> Values
    {
        get { return _values; }
        set { _values = value; }
    }

    public QualiDimension()
    {
        Values = new List<string>();
        IsLabelColumn = false;
    }

    private bool _isLabelColumn;

    public bool IsLabelColumn
    {
        get { return _isLabelColumn; }
        set { _isLabelColumn = value; }
    }
}
