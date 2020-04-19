using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QualiDimension : Dimension
{
    private Color[] acceptableColors;
    private List<string> _values;

    public List<string> Values
    {
        get { return _values; }
        set { _values = value; }
    }

    public QualiDimension()
    {
        Values = new List<string>();
        acceptableColors = new Color[] { Color.green, Color.red, Color.grey, Color.cyan, Color.magenta, Color.yellow };
        IsLabelColumn = false;
    }

    private bool _isLabelColumn;

    public bool IsLabelColumn
    {
        get { return _isLabelColumn; }
        set { _isLabelColumn = value; }
    }

    public int NbUniqueValues()
    {
        return Values.Distinct().ToList().Count;
    }

    public Dictionary<string,Color> DifferentiateFactorsColors()
    {
        Dictionary<string, Color> dict = new Dictionary<string, UnityEngine.Color>();
        Color orange = new Color(1f, 0.5f, 0f, 1f);

        int c = 0;
        foreach (var value in Values.Distinct())
        {
            if (c < acceptableColors.Length) dict.Add(value.ToString(), acceptableColors[c]);
            else dict.Add(value.ToString(), orange);
            c++; //comedy king
                    }

        return dict;
    }


}
