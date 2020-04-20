using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QualiDimension : Dimension
{
    private static Color[] acceptableColors;
    private static Material[] acceptableMaterials;
    public static Material defaultMat;

    private List<string> _values;

    public List<string> Values
    {
        get { return _values; }
        set { _values = value; }
    }

    public QualiDimension()
    {
        Values = new List<string>();
        if (acceptableColors==null) acceptableColors = new Color[] { new Color(0f, 0.5f, 0f,0f), Color.red, Color.grey, Color.cyan, Color.magenta, Color.yellow };
        if (acceptableMaterials == null)
        {
            acceptableMaterials = new Material[acceptableColors.Length+1];
            for (int i=0;i<acceptableMaterials.Length-1;i++)
            {
                acceptableMaterials[i] = new Material(defaultMat);
                acceptableMaterials[i].SetColor("_EmissionColor", acceptableColors[i]);
            }
            acceptableMaterials[acceptableColors.Length] = new Material(defaultMat);
            acceptableMaterials[acceptableColors.Length].SetColor("_EmissionColor", new Color(1f, 0.5f, 0f, 1f)); //additional categories after the 6th are in orange
        }

    }

    public int NbUniqueValues()
    {
        return Values.Distinct().ToList().Count;
    }

    public Dictionary<string,Material> DifferentiateFactorsColors()
    {
        Dictionary<string, Material> dict = new Dictionary<string, Material>();

        int c = 0;
        foreach (var value in Values.Distinct())
        {
            if (c < acceptableColors.Length) dict.Add(value.ToString(), acceptableMaterials[c]);
            else dict.Add(value.ToString(), acceptableMaterials[acceptableColors.Length]);
            c++; //comedy king
        }

        return dict;
    }

    public static QualiDimension LabelColumn(int size)
    {
        QualiDimension labelDimension = new QualiDimension();
        for(int i=1;i<=size;i++)
        {
            labelDimension.Values.Add(i.ToString());
        }
        labelDimension.Label = "Quali header column";

        return labelDimension;
    }

}
